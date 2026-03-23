using System;
using System.Globalization;
using System.Text;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Net;

namespace Mirror.BouncyCastle.Asn1.X509;

public class GeneralName : Asn1Encodable, IAsn1Choice
{
	public const int OtherName = 0;

	public const int Rfc822Name = 1;

	public const int DnsName = 2;

	public const int X400Address = 3;

	public const int DirectoryName = 4;

	public const int EdiPartyName = 5;

	public const int UniformResourceIdentifier = 6;

	public const int IPAddress = 7;

	public const int RegisteredID = 8;

	private readonly int m_tag;

	private readonly Asn1Encodable m_object;

	public int TagNo => m_tag;

	public Asn1Encodable Name => m_object;

	public static GeneralName GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is GeneralName result)
		{
			return result;
		}
		return GetInstanceSelection(Asn1TaggedObject.GetInstance(obj));
	}

	public static GeneralName GetInstance(Asn1TaggedObject tagObj, bool explicitly)
	{
		return Asn1Utilities.GetInstanceFromChoice(tagObj, explicitly, GetInstance);
	}

	private static GeneralName GetInstanceSelection(Asn1TaggedObject taggedObject)
	{
		if (taggedObject.HasContextTag())
		{
			int tagNo = taggedObject.TagNo;
			switch (tagNo)
			{
			case 0:
			case 3:
			case 5:
				return new GeneralName(tagNo, Asn1Sequence.GetInstance(taggedObject, declaredExplicit: false));
			case 1:
			case 2:
			case 6:
				return new GeneralName(tagNo, DerIA5String.GetInstance(taggedObject, declaredExplicit: false));
			case 4:
				return new GeneralName(tagNo, X509Name.GetInstance(taggedObject, explicitly: true));
			case 7:
				return new GeneralName(tagNo, Asn1OctetString.GetInstance(taggedObject, declaredExplicit: false));
			case 8:
				return new GeneralName(tagNo, DerObjectIdentifier.GetInstance(taggedObject, declaredExplicit: false));
			}
		}
		throw new ArgumentException("unknown tag: " + Asn1Utilities.GetTagText(taggedObject));
	}

	public GeneralName(X509Name directoryName)
	{
		m_tag = 4;
		m_object = directoryName;
	}

	public GeneralName(Asn1Object name, int tag)
	{
		m_tag = tag;
		m_object = name;
	}

	public GeneralName(int tag, Asn1Encodable name)
	{
		m_tag = tag;
		m_object = name;
	}

	public GeneralName(int tag, string name)
	{
		m_tag = tag;
		switch (tag)
		{
		case 1:
		case 2:
		case 6:
			m_object = new DerIA5String(name);
			break;
		case 4:
			m_object = new X509Name(name);
			break;
		case 7:
		{
			byte[] contents = ToGeneralNameEncoding(name) ?? throw new ArgumentException("IP Address is invalid", "name");
			m_object = new DerOctetString(contents);
			break;
		}
		case 8:
			m_object = new DerObjectIdentifier(name);
			break;
		default:
			throw new ArgumentException($"can't process string for tag: {Asn1Utilities.GetTagText(128, tag)}", "tag");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerTaggedObject(m_tag == 4, m_tag, m_object);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(m_tag);
		stringBuilder.Append(": ");
		switch (m_tag)
		{
		case 1:
		case 2:
		case 6:
			stringBuilder.Append(DerIA5String.GetInstance(m_object).GetString());
			break;
		case 4:
			stringBuilder.Append(X509Name.GetInstance(m_object).ToString());
			break;
		default:
			stringBuilder.Append(m_object.ToString());
			break;
		}
		return stringBuilder.ToString();
	}

	private byte[] ToGeneralNameEncoding(string ip)
	{
		if (Mirror.BouncyCastle.Utilities.Net.IPAddress.IsValidIPv6WithNetmask(ip) || Mirror.BouncyCastle.Utilities.Net.IPAddress.IsValidIPv6(ip))
		{
			int num = Platform.IndexOf(ip, '/');
			if (num < 0)
			{
				byte[] array = new byte[16];
				CopyInts(ParseIPv6(ip), array, 0);
				return array;
			}
			byte[] array2 = new byte[32];
			int[] parsedIp = ParseIPv6(ip.Substring(0, num));
			CopyInts(parsedIp, array2, 0);
			string text = ip.Substring(num + 1);
			parsedIp = ((Platform.IndexOf(text, ':') <= 0) ? ParseIPv6Mask(text) : ParseIPv6(text));
			CopyInts(parsedIp, array2, 16);
			return array2;
		}
		if (Mirror.BouncyCastle.Utilities.Net.IPAddress.IsValidIPv4WithNetmask(ip) || Mirror.BouncyCastle.Utilities.Net.IPAddress.IsValidIPv4(ip))
		{
			int num2 = Platform.IndexOf(ip, '/');
			if (num2 < 0)
			{
				byte[] array3 = new byte[4];
				ParseIPv4(ip, array3, 0);
				return array3;
			}
			byte[] array4 = new byte[8];
			ParseIPv4(ip.Substring(0, num2), array4, 0);
			string text2 = ip.Substring(num2 + 1);
			if (Platform.IndexOf(text2, '.') > 0)
			{
				ParseIPv4(text2, array4, 4);
			}
			else
			{
				ParseIPv4Mask(text2, array4, 4);
			}
			return array4;
		}
		return null;
	}

	private static void CopyInts(int[] parsedIp, byte[] addr, int offSet)
	{
		for (int i = 0; i != parsedIp.Length; i++)
		{
			addr[i * 2 + offSet] = (byte)(parsedIp[i] >> 8);
			addr[i * 2 + 1 + offSet] = (byte)parsedIp[i];
		}
	}

	private static void ParseIPv4(string ip, byte[] addr, int offset)
	{
		string[] array = ip.Split('.', '/');
		foreach (string s in array)
		{
			addr[offset++] = (byte)int.Parse(s);
		}
	}

	private static void ParseIPv4Mask(string mask, byte[] addr, int offset)
	{
		int num;
		for (num = int.Parse(mask); num >= 8; num -= 8)
		{
			addr[offset++] = byte.MaxValue;
		}
		if (num > 0)
		{
			addr[offset] = (byte)(65280 >> num);
		}
	}

	private static int[] ParseIPv6(string ip)
	{
		if (Platform.StartsWith(ip, "::"))
		{
			ip = ip.Substring(1);
		}
		else if (Platform.EndsWith(ip, "::"))
		{
			ip = ip.Substring(0, ip.Length - 1);
		}
		int num = 0;
		int[] array = new int[8];
		int num2 = -1;
		string[] array2 = ip.Split(new char[1] { ':' });
		foreach (string text in array2)
		{
			if (text.Length == 0)
			{
				num2 = num;
				array[num++] = 0;
				continue;
			}
			if (Platform.IndexOf(text, '.') < 0)
			{
				array[num++] = int.Parse(text, NumberStyles.AllowHexSpecifier);
				continue;
			}
			string[] array3 = text.Split(new char[1] { '.' });
			array[num++] = (int.Parse(array3[0]) << 8) | int.Parse(array3[1]);
			array[num++] = (int.Parse(array3[2]) << 8) | int.Parse(array3[3]);
		}
		if (num != array.Length)
		{
			Array.Copy(array, num2, array, array.Length - (num - num2), num - num2);
			for (int j = num2; j != array.Length - (num - num2); j++)
			{
				array[j] = 0;
			}
		}
		return array;
	}

	private static int[] ParseIPv6Mask(string mask)
	{
		int[] array = new int[8];
		int num = int.Parse(mask);
		int num2 = 0;
		while (num >= 16)
		{
			array[num2++] = 65535;
			num -= 16;
		}
		if (num > 0)
		{
			array[num2] = 65535 >> 16 - num;
		}
		return array;
	}
}
