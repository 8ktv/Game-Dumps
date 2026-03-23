using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC.Rfc8032;

internal static class Wnaf
{
	internal static void GetSignedVar(uint[] n, int width, sbyte[] ws)
	{
		uint[] array = new uint[n.Length * 2];
		uint num = 0 - (n[^1] >> 31);
		int num2 = array.Length;
		int num3 = n.Length;
		while (--num3 >= 0)
		{
			uint num4 = n[num3];
			array[--num2] = (num4 >> 16) | (num << 16);
			num = (array[--num2] = num4);
		}
		int num5 = 0;
		int num6 = 32 - width;
		int num7 = 0;
		int num8 = 0;
		while (num8 < array.Length)
		{
			uint num9 = array[num8];
			while (num5 < 16)
			{
				int num10 = (int)(num9 >> num5);
				int num11 = Integers.NumberOfTrailingZeros((num7 ^ num10) | 0x10000);
				if (num11 > 0)
				{
					num5 += num11;
					continue;
				}
				int num12 = (num10 | 1) << num6;
				num7 = num12 >> 31;
				ws[(num8 << 4) + num5] = (sbyte)(num12 >> num6);
				num5 += width;
			}
			num8++;
			num5 -= 16;
		}
	}
}
