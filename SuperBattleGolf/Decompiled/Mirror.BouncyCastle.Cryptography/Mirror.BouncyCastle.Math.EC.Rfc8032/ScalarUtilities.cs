using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC.Rfc8032;

internal static class ScalarUtilities
{
	internal static void AddShifted_NP(int last, int s, uint[] Nu, uint[] Nv, uint[] p, uint[] t)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		if (s == 0)
		{
			for (int i = 0; i <= last; i++)
			{
				uint num3 = p[i];
				num2 += Nu[i];
				num2 += num3;
				num += num3;
				num += Nv[i];
				num3 = (uint)num;
				num >>= 32;
				p[i] = num3;
				num2 += num3;
				Nu[i] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		if (s < 32)
		{
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			for (int j = 0; j <= last; j++)
			{
				uint num7 = p[j];
				uint num8 = (num7 << s) | (num4 >> -s);
				num4 = num7;
				num2 += Nu[j];
				num2 += num8;
				uint num9 = Nv[j];
				uint num10 = (num9 << s) | (num6 >> -s);
				num6 = num9;
				num += num7;
				num += num10;
				num7 = (uint)num;
				num >>= 32;
				p[j] = num7;
				uint num11 = (num7 << s) | (num5 >> -s);
				num5 = num7;
				num2 += num11;
				Nu[j] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		Array.Copy(p, 0, t, 0, last);
		int num12 = s >> 5;
		int num13 = s & 0x1F;
		if (num13 == 0)
		{
			for (int k = num12; k <= last; k++)
			{
				num2 += Nu[k];
				num2 += t[k - num12];
				num += p[k];
				num += Nv[k - num12];
				p[k] = (uint)num;
				num >>= 32;
				num2 += p[k - num12];
				Nu[k] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		uint num14 = 0u;
		uint num15 = 0u;
		uint num16 = 0u;
		for (int l = num12; l <= last; l++)
		{
			uint num17 = t[l - num12];
			uint num18 = (num17 << num13) | (num14 >> -num13);
			num14 = num17;
			num2 += Nu[l];
			num2 += num18;
			uint num19 = Nv[l - num12];
			uint num20 = (num19 << num13) | (num16 >> -num13);
			num16 = num19;
			num += p[l];
			num += num20;
			p[l] = (uint)num;
			num >>= 32;
			uint num21 = p[l - num12];
			uint num22 = (num21 << num13) | (num15 >> -num13);
			num15 = num21;
			num2 += num22;
			Nu[l] = (uint)num2;
			num2 >>= 32;
		}
	}

	internal static void AddShifted_UV(int last, int s, uint[] u0, uint[] u1, uint[] v0, uint[] v1)
	{
		int num = s >> 5;
		int num2 = s & 0x1F;
		ulong num3 = 0uL;
		ulong num4 = 0uL;
		if (num2 == 0)
		{
			for (int i = num; i <= last; i++)
			{
				num3 += u0[i];
				num4 += u1[i];
				num3 += v0[i - num];
				num4 += v1[i - num];
				u0[i] = (uint)num3;
				num3 >>= 32;
				u1[i] = (uint)num4;
				num4 >>= 32;
			}
			return;
		}
		uint num5 = 0u;
		uint num6 = 0u;
		for (int j = num; j <= last; j++)
		{
			uint num7 = v0[j - num];
			uint num8 = v1[j - num];
			uint num9 = (num7 << num2) | (num5 >> -num2);
			uint num10 = (num8 << num2) | (num6 >> -num2);
			num5 = num7;
			num6 = num8;
			num3 += u0[j];
			num4 += u1[j];
			num3 += num9;
			num4 += num10;
			u0[j] = (uint)num3;
			num3 >>= 32;
			u1[j] = (uint)num4;
			num4 >>= 32;
		}
	}

	internal static int GetBitLength(int last, uint[] x)
	{
		int num = last;
		uint num2 = (uint)((int)x[num] >> 31);
		while (num > 0 && x[num] == num2)
		{
			num--;
		}
		return num * 32 + 32 - Integers.NumberOfLeadingZeros((int)(x[num] ^ num2));
	}

	internal static int GetBitLengthPositive(int last, uint[] x)
	{
		int num = last;
		while (num > 0 && x[num] == 0)
		{
			num--;
		}
		return num * 32 + 32 - Integers.NumberOfLeadingZeros((int)x[num]);
	}

	internal static bool LessThan(int last, uint[] x, uint[] y)
	{
		int num = last;
		do
		{
			if (x[num] < y[num])
			{
				return true;
			}
			if (x[num] > y[num])
			{
				return false;
			}
		}
		while (--num >= 0);
		return false;
	}

	internal static void SubShifted_NP(int last, int s, uint[] Nu, uint[] Nv, uint[] p, uint[] t)
	{
		long num = 0L;
		long num2 = 0L;
		if (s == 0)
		{
			for (int i = 0; i <= last; i++)
			{
				uint num3 = p[i];
				num2 += Nu[i];
				num2 -= num3;
				num += num3;
				num -= Nv[i];
				num3 = (uint)num;
				num >>= 32;
				p[i] = num3;
				num2 -= num3;
				Nu[i] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		if (s < 32)
		{
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			for (int j = 0; j <= last; j++)
			{
				uint num7 = p[j];
				uint num8 = (num7 << s) | (num4 >> -s);
				num4 = num7;
				num2 += Nu[j];
				num2 -= num8;
				uint num9 = Nv[j];
				uint num10 = (num9 << s) | (num6 >> -s);
				num6 = num9;
				num += num7;
				num -= num10;
				num7 = (uint)num;
				num >>= 32;
				p[j] = num7;
				uint num11 = (num7 << s) | (num5 >> -s);
				num5 = num7;
				num2 -= num11;
				Nu[j] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		Array.Copy(p, 0, t, 0, last);
		int num12 = s >> 5;
		int num13 = s & 0x1F;
		if (num13 == 0)
		{
			for (int k = num12; k <= last; k++)
			{
				num2 += Nu[k];
				num2 -= t[k - num12];
				num += p[k];
				num -= Nv[k - num12];
				p[k] = (uint)num;
				num >>= 32;
				num2 -= p[k - num12];
				Nu[k] = (uint)num2;
				num2 >>= 32;
			}
			return;
		}
		uint num14 = 0u;
		uint num15 = 0u;
		uint num16 = 0u;
		for (int l = num12; l <= last; l++)
		{
			uint num17 = t[l - num12];
			uint num18 = (num17 << num13) | (num14 >> -num13);
			num14 = num17;
			num2 += Nu[l];
			num2 -= num18;
			uint num19 = Nv[l - num12];
			uint num20 = (num19 << num13) | (num16 >> -num13);
			num16 = num19;
			num += p[l];
			num -= num20;
			p[l] = (uint)num;
			num >>= 32;
			uint num21 = p[l - num12];
			uint num22 = (num21 << num13) | (num15 >> -num13);
			num15 = num21;
			num2 -= num22;
			Nu[l] = (uint)num2;
			num2 >>= 32;
		}
	}

	internal static void SubShifted_UV(int last, int s, uint[] u0, uint[] u1, uint[] v0, uint[] v1)
	{
		int num = s >> 5;
		int num2 = s & 0x1F;
		long num3 = 0L;
		long num4 = 0L;
		if (num2 == 0)
		{
			for (int i = num; i <= last; i++)
			{
				num3 += u0[i];
				num4 += u1[i];
				num3 -= v0[i - num];
				num4 -= v1[i - num];
				u0[i] = (uint)num3;
				num3 >>= 32;
				u1[i] = (uint)num4;
				num4 >>= 32;
			}
			return;
		}
		uint num5 = 0u;
		uint num6 = 0u;
		for (int j = num; j <= last; j++)
		{
			uint num7 = v0[j - num];
			uint num8 = v1[j - num];
			uint num9 = (num7 << num2) | (num5 >> -num2);
			uint num10 = (num8 << num2) | (num6 >> -num2);
			num5 = num7;
			num6 = num8;
			num3 += u0[j];
			num4 += u1[j];
			num3 -= num9;
			num4 -= num10;
			u0[j] = (uint)num3;
			num3 >>= 32;
			u1[j] = (uint)num4;
			num4 >>= 32;
		}
	}

	internal static void Swap(ref uint[] x, ref uint[] y)
	{
		uint[] array = x;
		x = y;
		y = array;
	}
}
