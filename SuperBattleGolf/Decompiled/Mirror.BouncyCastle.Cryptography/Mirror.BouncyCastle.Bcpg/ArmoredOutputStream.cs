using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg;

public class ArmoredOutputStream : BaseOutputStream
{
	public static readonly string HeaderVersion = "Version";

	private static readonly byte[] encodingTable = new byte[64]
	{
		65, 66, 67, 68, 69, 70, 71, 72, 73, 74,
		75, 76, 77, 78, 79, 80, 81, 82, 83, 84,
		85, 86, 87, 88, 89, 90, 97, 98, 99, 100,
		101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
		111, 112, 113, 114, 115, 116, 117, 118, 119, 120,
		121, 122, 48, 49, 50, 51, 52, 53, 54, 55,
		56, 57, 43, 47
	};

	private readonly Stream outStream;

	private byte[] buf = new byte[3];

	private int bufPtr;

	private Crc24 crc = new Crc24();

	private int chunkCount;

	private int lastb;

	private bool start = true;

	private bool clearText;

	private bool newLine;

	private string type;

	private static readonly string NewLine = Environment.NewLine;

	private static readonly string headerStart = "-----BEGIN PGP ";

	private static readonly string headerTail = "-----";

	private static readonly string footerStart = "-----END PGP ";

	private static readonly string footerTail = "-----";

	private static readonly string Version = CreateVersion();

	private readonly Dictionary<string, List<string>> m_headers;

	private static void Encode(Stream outStream, byte[] data, int len)
	{
		byte[] array = new byte[4];
		int num = data[0];
		array[0] = encodingTable[(num >> 2) & 0x3F];
		switch (len)
		{
		case 1:
			array[1] = encodingTable[(num << 4) & 0x3F];
			array[2] = 61;
			array[3] = 61;
			break;
		case 2:
		{
			int num4 = data[1];
			array[1] = encodingTable[((num << 4) | (num4 >> 4)) & 0x3F];
			array[2] = encodingTable[(num4 << 2) & 0x3F];
			array[3] = 61;
			break;
		}
		case 3:
		{
			int num2 = data[1];
			int num3 = data[2];
			array[1] = encodingTable[((num << 4) | (num2 >> 4)) & 0x3F];
			array[2] = encodingTable[((num2 << 2) | (num3 >> 6)) & 0x3F];
			array[3] = encodingTable[num3 & 0x3F];
			break;
		}
		}
		outStream.Write(array, 0, array.Length);
	}

	private static void Encode3(Stream outStream, byte[] data)
	{
		int num = data[0];
		int num2 = data[1];
		int num3 = data[2];
		byte[] array = new byte[4]
		{
			encodingTable[(num >> 2) & 0x3F],
			encodingTable[((num << 4) | (num2 >> 4)) & 0x3F],
			encodingTable[((num2 << 2) | (num3 >> 6)) & 0x3F],
			encodingTable[num3 & 0x3F]
		};
		outStream.Write(array, 0, array.Length);
	}

	private static string CreateVersion()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		AssemblyTitleAttribute customAttribute = executingAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
		AssemblyInformationalVersionAttribute customAttribute2 = executingAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		if (customAttribute == null || customAttribute2 == null)
		{
			return "BouncyCastle (unknown version)";
		}
		return customAttribute.Title + " v" + customAttribute2.InformationalVersion;
	}

	public ArmoredOutputStream(Stream outStream)
		: this(outStream, addVersionHeader: true)
	{
	}

	public ArmoredOutputStream(Stream outStream, bool addVersionHeader)
	{
		this.outStream = outStream;
		m_headers = new Dictionary<string, List<string>>();
		if (addVersionHeader)
		{
			SetHeader(HeaderVersion, Version);
		}
	}

	public ArmoredOutputStream(Stream outStream, IDictionary<string, string> headers)
		: this(outStream, headers, addVersionHeader: true)
	{
	}

	public ArmoredOutputStream(Stream outStream, IDictionary<string, string> headers, bool addVersionHeader)
		: this(outStream, addVersionHeader && !headers.ContainsKey(HeaderVersion))
	{
		foreach (KeyValuePair<string, string> header in headers)
		{
			m_headers.Add(header.Key, new List<string> { header.Value });
		}
	}

	public void SetHeader(string name, string val)
	{
		if (val == null)
		{
			m_headers.Remove(name);
			return;
		}
		if (m_headers.TryGetValue(name, out var value))
		{
			value.Clear();
		}
		else
		{
			value = new List<string>(1);
			m_headers[name] = value;
		}
		value.Add(val);
	}

	public void AddHeader(string name, string val)
	{
		if (val != null && name != null)
		{
			if (!m_headers.TryGetValue(name, out var value))
			{
				value = new List<string>(1);
				m_headers[name] = value;
			}
			value.Add(val);
		}
	}

	public void ResetHeaders()
	{
		List<string> value;
		bool num = m_headers.TryGetValue(HeaderVersion, out value);
		m_headers.Clear();
		if (num)
		{
			m_headers.Add(HeaderVersion, value);
		}
	}

	public void BeginClearText(HashAlgorithmTag hashAlgorithm)
	{
		string text = hashAlgorithm switch
		{
			HashAlgorithmTag.Sha1 => "SHA1", 
			HashAlgorithmTag.Sha256 => "SHA256", 
			HashAlgorithmTag.Sha384 => "SHA384", 
			HashAlgorithmTag.Sha512 => "SHA512", 
			HashAlgorithmTag.MD2 => "MD2", 
			HashAlgorithmTag.MD5 => "MD5", 
			HashAlgorithmTag.RipeMD160 => "RIPEMD160", 
			_ => throw new IOException("unknown hash algorithm tag in beginClearText: " + hashAlgorithm), 
		};
		DoWrite("-----BEGIN PGP SIGNED MESSAGE-----" + NewLine);
		DoWrite("Hash: " + text + NewLine + NewLine);
		clearText = true;
		newLine = true;
		lastb = 0;
	}

	public void EndClearText()
	{
		clearText = false;
	}

	public override void WriteByte(byte value)
	{
		if (clearText)
		{
			outStream.WriteByte(value);
			if (newLine)
			{
				if (value != 10 || lastb != 13)
				{
					newLine = false;
				}
				if (value == 45)
				{
					outStream.WriteByte(32);
					outStream.WriteByte(45);
				}
			}
			if (value == 13 || (value == 10 && lastb != 13))
			{
				newLine = true;
			}
			lastb = value;
			return;
		}
		if (start)
		{
			switch ((PacketTag)(((value & 0x40) == 0) ? ((value & 0x3F) >> 2) : (value & 0x3F)))
			{
			case PacketTag.PublicKey:
				type = "PUBLIC KEY BLOCK";
				break;
			case PacketTag.SecretKey:
				type = "PRIVATE KEY BLOCK";
				break;
			case PacketTag.Signature:
				type = "SIGNATURE";
				break;
			default:
				type = "MESSAGE";
				break;
			}
			DoWrite(headerStart + type + headerTail + NewLine);
			if (m_headers.TryGetValue(HeaderVersion, out var value2))
			{
				WriteHeaderEntry(HeaderVersion, value2[0]);
			}
			foreach (KeyValuePair<string, List<string>> header in m_headers)
			{
				string key = header.Key;
				if (!(key != HeaderVersion))
				{
					continue;
				}
				foreach (string item in header.Value)
				{
					WriteHeaderEntry(key, item);
				}
			}
			DoWrite(NewLine);
			start = false;
		}
		if (bufPtr == 3)
		{
			crc.Update3(buf, 0);
			Encode3(outStream, buf);
			bufPtr = 0;
			if ((++chunkCount & 0xF) == 0)
			{
				DoWrite(NewLine);
			}
		}
		buf[bufPtr++] = value;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && type != null)
		{
			DoClose();
			type = null;
			start = true;
		}
		base.Dispose(disposing);
	}

	private void DoClose()
	{
		if (bufPtr > 0)
		{
			for (int i = 0; i < bufPtr; i++)
			{
				crc.Update(buf[i]);
			}
			Encode(outStream, buf, bufPtr);
		}
		DoWrite(NewLine + "=");
		Pack.UInt24_To_BE((uint)crc.Value, buf);
		Encode3(outStream, buf);
		DoWrite(NewLine);
		DoWrite(footerStart);
		DoWrite(type);
		DoWrite(footerTail);
		DoWrite(NewLine);
		outStream.Flush();
	}

	private void WriteHeaderEntry(string name, string v)
	{
		DoWrite(name + ": " + v + NewLine);
	}

	private void DoWrite(string s)
	{
		byte[] array = Strings.ToAsciiByteArray(s);
		outStream.Write(array, 0, array.Length);
	}
}
