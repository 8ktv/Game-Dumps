using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg;

public class ArmoredInputStream : BaseInputStream
{
	private static readonly byte[] decodingTable;

	private bool detectMissingChecksum;

	private Stream input;

	private bool start = true;

	private byte[] outBuf = new byte[3];

	private int bufPtr = 3;

	private Crc24 crc = new Crc24();

	private bool crcFound;

	private bool hasHeaders = true;

	private string header;

	private bool newLineFound;

	private bool clearText;

	private bool restart;

	private IList<string> headerList = new List<string>();

	private int lastC;

	private bool isEndOfStream;

	static ArmoredInputStream()
	{
		decodingTable = new byte[128];
		Arrays.Fill(decodingTable, byte.MaxValue);
		for (int i = 65; i <= 90; i++)
		{
			decodingTable[i] = (byte)(i - 65);
		}
		for (int j = 97; j <= 122; j++)
		{
			decodingTable[j] = (byte)(j - 97 + 26);
		}
		for (int k = 48; k <= 57; k++)
		{
			decodingTable[k] = (byte)(k - 48 + 52);
		}
		decodingTable[43] = 62;
		decodingTable[47] = 63;
	}

	private static int Decode(int in0, int in1, int in2, int in3, byte[] result)
	{
		if (in3 < 0)
		{
			throw new EndOfStreamException("unexpected end of file in armored stream.");
		}
		int num;
		int num2;
		if (in2 == 61)
		{
			num = decodingTable[in0];
			num2 = decodingTable[in1];
			if ((num | num2) >= 128)
			{
				throw new IOException("invalid armor");
			}
			result[2] = (byte)((num << 2) | (num2 >> 4));
			return 2;
		}
		int num3;
		if (in3 == 61)
		{
			num = decodingTable[in0];
			num2 = decodingTable[in1];
			num3 = decodingTable[in2];
			if ((num | num2 | num3) >= 128)
			{
				throw new IOException("invalid armor");
			}
			result[1] = (byte)((num << 2) | (num2 >> 4));
			result[2] = (byte)((num2 << 4) | (num3 >> 2));
			return 1;
		}
		num = decodingTable[in0];
		num2 = decodingTable[in1];
		num3 = decodingTable[in2];
		int num4 = decodingTable[in3];
		if ((num | num2 | num3 | num4) >= 128)
		{
			throw new IOException("invalid armor");
		}
		result[0] = (byte)((num << 2) | (num2 >> 4));
		result[1] = (byte)((num2 << 4) | (num3 >> 2));
		result[2] = (byte)((num3 << 6) | num4);
		return 0;
	}

	public ArmoredInputStream(Stream input)
		: this(input, hasHeaders: true)
	{
	}

	public ArmoredInputStream(Stream input, bool hasHeaders)
	{
		this.input = input;
		this.hasHeaders = hasHeaders;
		if (hasHeaders)
		{
			ParseHeaders();
		}
		start = false;
	}

	private bool ParseHeaders()
	{
		header = null;
		int num = 0;
		bool flag = false;
		headerList = new List<string>();
		if (restart)
		{
			flag = true;
		}
		else
		{
			int num2;
			while ((num2 = input.ReadByte()) >= 0)
			{
				if (num2 == 45 && (num == 0 || num == 10 || num == 13))
				{
					flag = true;
					break;
				}
				num = num2;
			}
		}
		if (flag)
		{
			StringBuilder stringBuilder = new StringBuilder("-");
			bool flag2 = false;
			bool flag3 = false;
			if (restart)
			{
				stringBuilder.Append('-');
			}
			int num2;
			while ((num2 = input.ReadByte()) >= 0)
			{
				if (num == 13 && num2 == 10)
				{
					flag3 = true;
				}
				if ((flag2 && num != 13 && num2 == 10) || (flag2 && num2 == 13))
				{
					break;
				}
				if (num2 == 13 || (num != 13 && num2 == 10))
				{
					string text = stringBuilder.ToString();
					if (text.Trim().Length < 1)
					{
						break;
					}
					if (headerList.Count > 0 && text.IndexOf(':') < 0)
					{
						throw new IOException("invalid armor header");
					}
					headerList.Add(text);
					stringBuilder.Length = 0;
				}
				if (num2 != 10 && num2 != 13)
				{
					stringBuilder.Append((char)num2);
					flag2 = false;
				}
				else if (num2 == 13 || (num != 13 && num2 == 10))
				{
					flag2 = true;
				}
				num = num2;
			}
			if (flag3)
			{
				input.ReadByte();
			}
		}
		if (headerList.Count > 0)
		{
			header = headerList[0];
		}
		clearText = "-----BEGIN PGP SIGNED MESSAGE-----".Equals(header);
		newLineFound = true;
		return flag;
	}

	public bool IsClearText()
	{
		return clearText;
	}

	public bool IsEndOfStream()
	{
		return isEndOfStream;
	}

	public string GetArmorHeaderLine()
	{
		return header;
	}

	public string[] GetArmorHeaders()
	{
		if (headerList.Count <= 1)
		{
			return null;
		}
		string[] array = new string[headerList.Count - 1];
		for (int i = 0; i != array.Length; i++)
		{
			array[i] = headerList[i + 1];
		}
		return array;
	}

	private int ReadIgnoreSpace()
	{
		int num;
		do
		{
			num = input.ReadByte();
		}
		while (num == 32 || num == 9 || num == 12 || num == 11);
		if (num >= 128)
		{
			throw new IOException("invalid armor");
		}
		return num;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		int num = 0;
		while (num < count)
		{
			int num2 = ReadByte();
			if (num2 < 0)
			{
				break;
			}
			buffer[offset + num++] = (byte)num2;
		}
		return num;
	}

	public override int ReadByte()
	{
		if (start)
		{
			if (hasHeaders)
			{
				ParseHeaders();
			}
			crc.Reset();
			start = false;
		}
		if (clearText)
		{
			int num = input.ReadByte();
			if (num == 13 || (num == 10 && lastC != 13))
			{
				newLineFound = true;
			}
			else if (newLineFound && num == 45)
			{
				num = input.ReadByte();
				if (num == 45)
				{
					clearText = false;
					start = true;
					restart = true;
				}
				else
				{
					num = input.ReadByte();
				}
				newLineFound = false;
			}
			else if (num != 10 && lastC != 13)
			{
				newLineFound = false;
			}
			lastC = num;
			if (num < 0)
			{
				isEndOfStream = true;
			}
			return num;
		}
		if (bufPtr > 2 || crcFound)
		{
			int num = ReadIgnoreSpace();
			if (num == 13 || num == 10)
			{
				num = ReadIgnoreSpace();
				while (num == 10 || num == 13)
				{
					num = ReadIgnoreSpace();
				}
				if (num == 61)
				{
					bufPtr = Decode(ReadIgnoreSpace(), ReadIgnoreSpace(), ReadIgnoreSpace(), ReadIgnoreSpace(), outBuf);
					if (bufPtr != 0)
					{
						throw new IOException("malformed crc in armored message.");
					}
					crcFound = true;
					if (Pack.BE_To_UInt24(outBuf) != (uint)crc.Value)
					{
						throw new IOException("crc check failed in armored message.");
					}
					return ReadByte();
				}
				if (num == 45)
				{
					while ((num = input.ReadByte()) >= 0 && num != 10 && num != 13)
					{
					}
					if (!crcFound && detectMissingChecksum)
					{
						throw new IOException("crc check not found");
					}
					crcFound = false;
					start = true;
					bufPtr = 3;
					if (num < 0)
					{
						isEndOfStream = true;
					}
					return -1;
				}
			}
			if (num < 0)
			{
				isEndOfStream = true;
				return -1;
			}
			bufPtr = Decode(num, ReadIgnoreSpace(), ReadIgnoreSpace(), ReadIgnoreSpace(), outBuf);
			if (bufPtr == 0)
			{
				crc.Update3(outBuf, 0);
			}
			else
			{
				for (int i = bufPtr; i < 3; i++)
				{
					crc.Update(outBuf[i]);
				}
			}
		}
		return outBuf[bufPtr++];
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			input.Dispose();
		}
		base.Dispose(disposing);
	}

	public virtual void SetDetectMissingCrc(bool detectMissing)
	{
		detectMissingChecksum = detectMissing;
	}
}
