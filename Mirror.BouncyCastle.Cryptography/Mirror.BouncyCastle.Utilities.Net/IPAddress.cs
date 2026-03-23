namespace Mirror.BouncyCastle.Utilities.Net;

public class IPAddress
{
	public static bool IsValid(string address)
	{
		if (!IsValidIPv4(address))
		{
			return IsValidIPv6(address);
		}
		return true;
	}

	public static bool IsValidWithNetMask(string address)
	{
		if (!IsValidIPv4WithNetmask(address))
		{
			return IsValidIPv6WithNetmask(address);
		}
		return true;
	}

	public static bool IsValidIPv4(string address)
	{
		int length = address.Length;
		if (length < 7 || length > 15)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			int num2 = Platform.IndexOf(address, '.', num);
			if (!IsParseableIPv4Octet(address, num, num2))
			{
				return false;
			}
			num = num2 + 1;
		}
		return IsParseableIPv4Octet(address, num, length);
	}

	public static bool IsValidIPv4WithNetmask(string address)
	{
		int num = Platform.IndexOf(address, '/');
		if (num < 1)
		{
			return false;
		}
		string address2 = address.Substring(0, num);
		string text = address.Substring(num + 1);
		if (IsValidIPv4(address2))
		{
			if (!IsValidIPv4(text))
			{
				return IsParseableIPv4Mask(text);
			}
			return true;
		}
		return false;
	}

	public static bool IsValidIPv6(string address)
	{
		if (address.Length == 0)
		{
			return false;
		}
		if (address[0] != ':' && GetDigitHexadecimal(address, 0) < 0)
		{
			return false;
		}
		int num = 0;
		string text = address + ":";
		bool flag = false;
		int num2 = 0;
		int num3;
		while (num2 < text.Length && (num3 = Platform.IndexOf(text, ':', num2)) >= num2)
		{
			if (num == 8)
			{
				return false;
			}
			if (num2 != num3)
			{
				string text2 = text.Substring(num2, num3 - num2);
				if (num3 == text.Length - 1 && Platform.IndexOf(text2, '.') > 0)
				{
					if (++num == 8)
					{
						return false;
					}
					if (!IsValidIPv4(text2))
					{
						return false;
					}
				}
				else if (!IsParseableIPv6Segment(text, num2, num3))
				{
					return false;
				}
			}
			else
			{
				if (num3 != 1 && num3 != text.Length - 1 && flag)
				{
					return false;
				}
				flag = true;
			}
			num2 = num3 + 1;
			num++;
		}
		return num == 8 || flag;
	}

	public static bool IsValidIPv6WithNetmask(string address)
	{
		int num = Platform.IndexOf(address, '/');
		if (num < 1)
		{
			return false;
		}
		string address2 = address.Substring(0, num);
		string text = address.Substring(num + 1);
		if (IsValidIPv6(address2))
		{
			if (!IsValidIPv6(text))
			{
				return IsParseableIPv6Mask(text);
			}
			return true;
		}
		return false;
	}

	private static bool IsParseableIPv4Mask(string s)
	{
		return IsParseableDecimal(s, 0, s.Length, 2, allowLeadingZero: false, 0, 32);
	}

	private static bool IsParseableIPv4Octet(string s, int pos, int end)
	{
		return IsParseableDecimal(s, pos, end, 3, allowLeadingZero: true, 0, 255);
	}

	private static bool IsParseableIPv6Mask(string s)
	{
		return IsParseableDecimal(s, 0, s.Length, 3, allowLeadingZero: false, 1, 128);
	}

	private static bool IsParseableIPv6Segment(string s, int pos, int end)
	{
		return IsParseableHexadecimal(s, pos, end, 4, allowLeadingZero: true, 0, 65535);
	}

	private static bool IsParseableDecimal(string s, int pos, int end, int maxLength, bool allowLeadingZero, int minValue, int maxValue)
	{
		int num = end - pos;
		if (num < 1 || num > maxLength)
		{
			return false;
		}
		if (num > 1 && !allowLeadingZero && s[pos] == '0')
		{
			return false;
		}
		int num2 = 0;
		while (pos < end)
		{
			int digitDecimal = GetDigitDecimal(s, pos++);
			if (digitDecimal < 0)
			{
				return false;
			}
			num2 *= 10;
			num2 += digitDecimal;
		}
		return num2 >= minValue && num2 <= maxValue;
	}

	private static bool IsParseableHexadecimal(string s, int pos, int end, int maxLength, bool allowLeadingZero, int minValue, int maxValue)
	{
		int num = end - pos;
		if (num < 1 || num > maxLength)
		{
			return false;
		}
		if (num > 1 && !allowLeadingZero && s[pos] == '0')
		{
			return false;
		}
		int num2 = 0;
		while (pos < end)
		{
			int digitHexadecimal = GetDigitHexadecimal(s, pos++);
			if (digitHexadecimal < 0)
			{
				return false;
			}
			num2 *= 16;
			num2 += digitHexadecimal;
		}
		return num2 >= minValue && num2 <= maxValue;
	}

	private static int GetDigitDecimal(string s, int pos)
	{
		uint num = (uint)(s[pos] - 48);
		if (num > 9)
		{
			return -1;
		}
		return (int)num;
	}

	private static int GetDigitHexadecimal(string s, int pos)
	{
		uint num = (uint)(s[pos] | 0x20);
		num -= (uint)((num >= 97) ? 87 : 48);
		if (num > 16)
		{
			return -1;
		}
		return (int)num;
	}
}
