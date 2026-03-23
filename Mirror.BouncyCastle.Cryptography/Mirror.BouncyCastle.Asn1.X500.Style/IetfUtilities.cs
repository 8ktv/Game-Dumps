using System;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Asn1.X500.Style;

public abstract class IetfUtilities
{
	internal static string Unescape(string elt)
	{
		if (elt.Length < 1)
		{
			return elt;
		}
		if (elt.IndexOf('\\') < 0 && elt.IndexOf('"') < 0)
		{
			return elt.Trim();
		}
		bool flag = false;
		bool flag2 = false;
		StringBuilder stringBuilder = new StringBuilder(elt.Length);
		int num = 0;
		if (elt[0] == '\\' && elt[1] == '#')
		{
			num = 2;
			stringBuilder.Append("\\#");
		}
		bool flag3 = false;
		int num2 = 0;
		char c = Convert.ToChar(0);
		for (int i = num; i != elt.Length; i++)
		{
			char c2 = elt[i];
			if (c2 != ' ')
			{
				flag3 = true;
			}
			switch (c2)
			{
			case '"':
				if (!flag)
				{
					flag2 = !flag2;
					continue;
				}
				stringBuilder.Append(c2);
				flag = false;
				continue;
			case '\\':
				if (!(flag || flag2))
				{
					flag = true;
					num2 = stringBuilder.Length;
					continue;
				}
				break;
			}
			if (c2 == ' ' && !flag && !flag3)
			{
				continue;
			}
			if (flag && IsHexDigit(c2))
			{
				if (c != 0)
				{
					stringBuilder.Append(Convert.ToChar(ConvertHex(c) * 16 + ConvertHex(c2)));
					flag = false;
					c = Convert.ToChar(0);
				}
				else
				{
					c = c2;
				}
			}
			else
			{
				stringBuilder.Append(c2);
				flag = false;
			}
		}
		if (stringBuilder.Length > 0)
		{
			while (stringBuilder[stringBuilder.Length - 1] == ' ' && num2 != stringBuilder.Length - 1)
			{
				stringBuilder.Length--;
			}
		}
		return stringBuilder.ToString();
	}

	private static bool IsHexDigit(char c)
	{
		if (('0' > c || c > '9') && ('a' > c || c > 'f'))
		{
			if ('A' <= c)
			{
				return c <= 'F';
			}
			return false;
		}
		return true;
	}

	private static int ConvertHex(char c)
	{
		if ('0' <= c && c <= '9')
		{
			return c - 48;
		}
		if ('a' <= c && c <= 'f')
		{
			return c - 97 + 10;
		}
		return c - 65 + 10;
	}

	public static string ValueToString(Asn1Encodable value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (value is IAsn1String asn1String && !(value is DerUniversalString))
		{
			string text = asn1String.GetString();
			if (text.Length > 0 && text[0] == '#')
			{
				stringBuilder.Append('\\');
			}
			stringBuilder.Append(text);
		}
		else
		{
			try
			{
				stringBuilder.Append('#');
				stringBuilder.Append(Hex.ToHexString(value.ToAsn1Object().GetEncoded("DER")));
			}
			catch (IOException innerException)
			{
				throw new ArgumentException("Other value has no encoded form", innerException);
			}
		}
		int num = stringBuilder.Length;
		int num2 = 0;
		if (stringBuilder.Length >= 2 && stringBuilder[0] == '\\' && stringBuilder[1] == '#')
		{
			num2 += 2;
		}
		while (num2 != num)
		{
			switch (stringBuilder[num2])
			{
			case '"':
			case '+':
			case ',':
			case ';':
			case '<':
			case '=':
			case '>':
			case '\\':
				stringBuilder.Insert(num2, "\\");
				num2 += 2;
				num++;
				break;
			default:
				num2++;
				break;
			}
		}
		int i = 0;
		if (stringBuilder.Length > 0)
		{
			for (; stringBuilder.Length > i && stringBuilder[i] == ' '; i += 2)
			{
				stringBuilder.Insert(i, "\\");
			}
		}
		int num3 = stringBuilder.Length - 1;
		while (num3 >= 0 && stringBuilder[num3] == ' ')
		{
			stringBuilder.Insert(num3, "\\");
			num3--;
		}
		return stringBuilder.ToString();
	}

	public static string Canonicalize(string s)
	{
		string text = s.ToLowerInvariant();
		if (text.Length > 0 && text[0] == '#' && DecodeObject(text) is IAsn1String asn1String)
		{
			text = asn1String.GetString().ToLowerInvariant();
		}
		if (text.Length > 1)
		{
			int i;
			for (i = 0; i + 1 < text.Length && text[i] == '\\' && text[i + 1] == ' '; i += 2)
			{
			}
			int num = text.Length - 1;
			while (num - 1 > 0 && text[num - 1] == '\\' && text[num] == ' ')
			{
				num -= 2;
			}
			if (i > 0 || num < text.Length - 1)
			{
				text = text.Substring(i, num + 1 - i);
			}
		}
		return StripInternalSpaces(text);
	}

	public static string CanonicalString(Asn1Encodable value)
	{
		return Canonicalize(ValueToString(value));
	}

	private static Asn1Object DecodeObject(string oValue)
	{
		try
		{
			return Asn1Object.FromByteArray(Hex.DecodeStrict(oValue, 1, oValue.Length - 1));
		}
		catch (IOException ex)
		{
			throw new InvalidOperationException("unknown encoding in name: " + ex);
		}
	}

	public static string StripInternalSpaces(string str)
	{
		if (str.IndexOf("  ") < 0)
		{
			return str;
		}
		StringBuilder stringBuilder = new StringBuilder();
		char c = str[0];
		stringBuilder.Append(c);
		for (int i = 1; i < str.Length; i++)
		{
			char c2 = str[i];
			if (' ' != c || ' ' != c2)
			{
				stringBuilder.Append(c2);
				c = c2;
			}
		}
		return stringBuilder.ToString();
	}

	public static bool RdnAreEqual(Rdn rdn1, Rdn rdn2)
	{
		if (rdn1.Count != rdn2.Count)
		{
			return false;
		}
		AttributeTypeAndValue[] typesAndValues = rdn1.GetTypesAndValues();
		AttributeTypeAndValue[] typesAndValues2 = rdn2.GetTypesAndValues();
		if (typesAndValues.Length != typesAndValues2.Length)
		{
			return false;
		}
		for (int i = 0; i != typesAndValues.Length; i++)
		{
			if (!AtvAreEqual(typesAndValues[i], typesAndValues2[i]))
			{
				return false;
			}
		}
		return true;
	}

	private static bool AtvAreEqual(AttributeTypeAndValue atv1, AttributeTypeAndValue atv2)
	{
		if (atv1 == atv2)
		{
			return true;
		}
		if (atv1 == null || atv2 == null)
		{
			return false;
		}
		DerObjectIdentifier type = atv1.Type;
		DerObjectIdentifier type2 = atv2.Type;
		if (!type.Equals(type2))
		{
			return false;
		}
		string text = CanonicalString(atv1.Value);
		string value = CanonicalString(atv2.Value);
		if (!text.Equals(value))
		{
			return false;
		}
		return true;
	}
}
