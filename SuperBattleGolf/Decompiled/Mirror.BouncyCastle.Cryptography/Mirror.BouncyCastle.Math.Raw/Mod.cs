using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.Raw;

internal static class Mod
{
	private const int M30 = 1073741823;

	private const ulong M32UL = 4294967295uL;

	private static readonly int MaxStackAlloc = (Platform.Is64BitProcess ? 4096 : 1024);

	public static void CheckedModOddInverse(uint[] m, uint[] x, uint[] z)
	{
		if (ModOddInverse(m, x, z) == 0)
		{
			throw new ArithmeticException("Inverse does not exist.");
		}
	}

	public static void CheckedModOddInverseVar(uint[] m, uint[] x, uint[] z)
	{
		if (!ModOddInverseVar(m, x, z))
		{
			throw new ArithmeticException("Inverse does not exist.");
		}
	}

	public static uint Inverse32(uint d)
	{
		uint num = d;
		num *= 2 - d * num;
		num *= 2 - d * num;
		num *= 2 - d * num;
		return num * (2 - d * num);
	}

	public static ulong Inverse64(ulong d)
	{
		ulong num = d;
		num *= 2 - d * num;
		num *= 2 - d * num;
		num *= 2 - d * num;
		num *= 2 - d * num;
		return num * (2 - d * num);
	}

	public static uint ModOddInverse(uint[] m, uint[] x, uint[] z)
	{
		int num = m.Length;
		int num2 = (num << 5) - Integers.NumberOfLeadingZeros((int)m[num - 1]);
		int num3 = (num2 + 29) / 30;
		int[] t = new int[4];
		int[] array = new int[num3];
		int[] array2 = new int[num3];
		int[] array3 = new int[num3];
		int[] array4 = new int[num3];
		int[] array5 = new int[num3];
		array2[0] = 1;
		Encode30(num2, x, array4);
		Encode30(num2, m, array5);
		Array.Copy(array5, 0, array3, 0, num3);
		int theta = 0;
		int m0Inv = (int)Inverse32((uint)array5[0]);
		int maximumHDDivsteps = GetMaximumHDDivsteps(num2);
		for (int i = 0; i < maximumHDDivsteps; i += 30)
		{
			theta = HDDivsteps30(theta, array3[0], array4[0], t);
			UpdateDE30(num3, array, array2, t, m0Inv, array5);
			UpdateFG30(num3, array3, array4, t);
		}
		int num4 = array3[num3 - 1] >> 31;
		CNegate30(num3, num4, array3);
		CNormalize30(num3, num4, array, array5);
		Decode30(num2, array, z);
		return (uint)(EqualTo(num3, array3, 1) & EqualTo(num3, array4, 0));
	}

	public static bool ModOddInverseVar(uint[] m, uint[] x, uint[] z)
	{
		int num = m.Length;
		int num2 = (num << 5) - Integers.NumberOfLeadingZeros((int)m[num - 1]);
		int num3 = (num2 + 29) / 30;
		int num4 = num2 - Nat.GetBitLength(num, x);
		int[] t = new int[4];
		int[] array = new int[num3];
		int[] array2 = new int[num3];
		int[] array3 = new int[num3];
		int[] array4 = new int[num3];
		int[] array5 = new int[num3];
		array2[0] = 1;
		Encode30(num2, x, array4);
		Encode30(num2, m, array5);
		Array.Copy(array5, 0, array3, 0, num3);
		int eta = -num4;
		int num5 = num3;
		int num6 = num3;
		int m0Inv = (int)Inverse32((uint)array5[0]);
		int maximumDivsteps = GetMaximumDivsteps(num2);
		int num7 = num4;
		while (!EqualToVar(num6, array4, 0))
		{
			if (num7 >= maximumDivsteps)
			{
				return false;
			}
			num7 += 30;
			eta = Divsteps30Var(eta, array3[0], array4[0], t);
			UpdateDE30(num5, array, array2, t, m0Inv, array5);
			UpdateFG30(num6, array3, array4, t);
			num6 = TrimFG30Var(num6, array3, array4);
		}
		int num8 = array3[num6 - 1] >> 31;
		int num9 = array[num5 - 1] >> 31;
		if (num9 < 0)
		{
			num9 = Add30(num5, array, array5);
		}
		if (num8 < 0)
		{
			num9 = Negate30(num5, array);
			Negate30(num6, array3);
		}
		if (!EqualToVar(num6, array3, 1))
		{
			return false;
		}
		if (num9 < 0)
		{
			num9 = Add30(num5, array, array5);
		}
		Decode30(num2, array, z);
		return true;
	}

	public static uint ModOddIsCoprime(uint[] m, uint[] x)
	{
		int num = m.Length;
		int num2 = (num << 5) - Integers.NumberOfLeadingZeros((int)m[num - 1]);
		int num3 = (num2 + 29) / 30;
		int[] t = new int[4];
		int[] array = new int[num3];
		int[] array2 = new int[num3];
		int[] array3 = new int[num3];
		Encode30(num2, x, array2);
		Encode30(num2, m, array3);
		Array.Copy(array3, 0, array, 0, num3);
		int theta = 0;
		int maximumHDDivsteps = GetMaximumHDDivsteps(num2);
		for (int i = 0; i < maximumHDDivsteps; i += 30)
		{
			theta = HDDivsteps30(theta, array[0], array2[0], t);
			UpdateFG30(num3, array, array2, t);
		}
		int cond = array[num3 - 1] >> 31;
		CNegate30(num3, cond, array);
		return (uint)(EqualTo(num3, array, 1) & EqualTo(num3, array2, 0));
	}

	public static bool ModOddIsCoprimeVar(uint[] m, uint[] x)
	{
		int num = m.Length;
		int num2 = (num << 5) - Integers.NumberOfLeadingZeros((int)m[num - 1]);
		int num3 = (num2 + 29) / 30;
		int num4 = num2 - Nat.GetBitLength(num, x);
		int[] t = new int[4];
		int[] array = new int[num3];
		int[] array2 = new int[num3];
		int[] array3 = new int[num3];
		Encode30(num2, x, array2);
		Encode30(num2, m, array3);
		Array.Copy(array3, 0, array, 0, num3);
		int eta = -num4;
		int num5 = num3;
		int maximumDivsteps = GetMaximumDivsteps(num2);
		int num6 = num4;
		while (!EqualToVar(num5, array2, 0))
		{
			if (num6 >= maximumDivsteps)
			{
				return false;
			}
			num6 += 30;
			eta = Divsteps30Var(eta, array[0], array2[0], t);
			UpdateFG30(num5, array, array2, t);
			num5 = TrimFG30Var(num5, array, array2);
		}
		if (array[num5 - 1] >> 31 < 0)
		{
			Negate30(num5, array);
		}
		return EqualToVar(num5, array, 1);
	}

	public static uint[] Random(SecureRandom random, uint[] p)
	{
		int num = p.Length;
		uint[] array = Nat.Create(num);
		uint num2 = p[num - 1];
		num2 |= num2 >> 1;
		num2 |= num2 >> 2;
		num2 |= num2 >> 4;
		num2 |= num2 >> 8;
		num2 |= num2 >> 16;
		byte[] array2 = new byte[num << 2];
		do
		{
			random.NextBytes(array2);
			Pack.BE_To_UInt32(array2, 0, array);
			array[num - 1] &= num2;
		}
		while (Nat.Gte(num, array, p));
		return array;
	}

	private static int Add30(int len30, int[] D, int[] M)
	{
		int num = 0;
		int num2 = len30 - 1;
		for (int i = 0; i < num2; i++)
		{
			num += D[i] + M[i];
			D[i] = num & 0x3FFFFFFF;
			num >>= 30;
		}
		return (D[num2] = num + (D[num2] + M[num2])) >> 30;
	}

	private static void CNegate30(int len30, int cond, int[] D)
	{
		int num = 0;
		int num2 = len30 - 1;
		for (int i = 0; i < num2; i++)
		{
			num += (D[i] ^ cond) - cond;
			D[i] = num & 0x3FFFFFFF;
			num >>= 30;
		}
		num += (D[num2] ^ cond) - cond;
		D[num2] = num;
	}

	private static void CNormalize30(int len30, int condNegate, int[] D, int[] M)
	{
		int num = len30 - 1;
		int num2 = 0;
		int num3 = D[num] >> 31;
		for (int i = 0; i < num; i++)
		{
			int num4 = D[i] + (M[i] & num3);
			num4 = (num4 ^ condNegate) - condNegate;
			num2 += num4;
			D[i] = num2 & 0x3FFFFFFF;
			num2 >>= 30;
		}
		int num5 = D[num] + (M[num] & num3);
		num5 = (num5 ^ condNegate) - condNegate;
		num2 += num5;
		D[num] = num2;
		int num6 = 0;
		int num7 = D[num] >> 31;
		for (int j = 0; j < num; j++)
		{
			int num8 = D[j] + (M[j] & num7);
			num6 += num8;
			D[j] = num6 & 0x3FFFFFFF;
			num6 >>= 30;
		}
		int num9 = D[num] + (M[num] & num7);
		num6 += num9;
		D[num] = num6;
	}

	private static void Decode30(int bits, int[] x, uint[] z)
	{
		int i = 0;
		ulong num = 0uL;
		int num2 = 0;
		int num3 = 0;
		while (bits > 0)
		{
			for (; i < System.Math.Min(32, bits); i += 30)
			{
				num |= (ulong)((long)x[num2++] << i);
			}
			z[num3++] = (uint)num;
			num >>= 32;
			i -= 32;
			bits -= 32;
		}
	}

	private static int Divsteps30Var(int eta, int f0, int g0, int[] t)
	{
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		int num4 = 1;
		int num5 = f0;
		int num6 = g0;
		int num7 = 30;
		while (true)
		{
			int num8 = Integers.NumberOfTrailingZeros(num6 | (-1 << num7));
			num6 >>= num8;
			num <<= num8;
			num2 <<= num8;
			eta -= num8;
			num7 -= num8;
			if (num7 <= 0)
			{
				break;
			}
			int num14;
			if (eta <= 0)
			{
				eta = 2 - eta;
				int num9 = num5;
				num5 = num6;
				num6 = -num9;
				int num10 = num;
				num = num3;
				num3 = -num10;
				int num11 = num2;
				num2 = num4;
				num4 = -num11;
				int num12 = ((eta > num7) ? num7 : eta);
				int num13 = (-1 >>> 32 - num12) & 0x3F;
				num14 = (num5 * num6 * (num5 * num5 - 2)) & num13;
			}
			else
			{
				int num12 = ((eta > num7) ? num7 : eta);
				int num13 = (-1 >>> 32 - num12) & 0xF;
				num14 = num5 + (((num5 + 1) & 4) << 1);
				num14 = (num14 * -num6) & num13;
			}
			num6 += num5 * num14;
			num3 += num * num14;
			num4 += num2 * num14;
		}
		t[0] = num;
		t[1] = num2;
		t[2] = num3;
		t[3] = num4;
		return eta;
	}

	private static void Encode30(int bits, uint[] x, int[] z)
	{
		int num = 0;
		ulong num2 = 0uL;
		int num3 = 0;
		int num4 = 0;
		while (bits > 0)
		{
			if (num < System.Math.Min(30, bits))
			{
				num2 |= ((ulong)x[num3++] & 0xFFFFFFFFuL) << num;
				num += 32;
			}
			z[num4++] = (int)num2 & 0x3FFFFFFF;
			num2 >>= 30;
			num -= 30;
			bits -= 30;
		}
	}

	private static int EqualTo(int len, int[] x, int y)
	{
		int num = x[0] ^ y;
		for (int i = 1; i < len; i++)
		{
			num |= x[i];
		}
		num = (num >>> 1) | (num & 1);
		return num - 1 >> 31;
	}

	private static bool EqualToVar(int len, int[] x, int y)
	{
		int num = x[0] ^ y;
		if (num != 0)
		{
			return false;
		}
		for (int i = 1; i < len; i++)
		{
			num |= x[i];
		}
		return num == 0;
	}

	private static int GetMaximumDivsteps(int bits)
	{
		return (int)(188898L * (long)bits + ((bits < 46) ? 308405 : 181188) >> 16);
	}

	private static int GetMaximumHDDivsteps(int bits)
	{
		return (int)(150964L * (long)bits + 99243 >> 16);
	}

	private static int HDDivsteps30(int theta, int f0, int g0, int[] t)
	{
		int num = 1073741824;
		int num2 = 0;
		int num3 = 0;
		int num4 = 1073741824;
		int num5 = f0;
		int num6 = g0;
		for (int i = 0; i < 30; i++)
		{
			int num7 = theta >> 31;
			int num8 = -(num6 & 1);
			int num9 = num5 ^ num7;
			int num10 = num ^ num7;
			int num11 = num2 ^ num7;
			num6 -= num9 & num8;
			num3 -= num10 & num8;
			num4 -= num11 & num8;
			int num12 = num8 & ~num7;
			theta = (theta ^ num12) + 1;
			num5 += num6 & num12;
			num += num3 & num12;
			num2 += num4 & num12;
			num6 >>= 1;
			num3 >>= 1;
			num4 >>= 1;
		}
		t[0] = num;
		t[1] = num2;
		t[2] = num3;
		t[3] = num4;
		return theta;
	}

	private static int Negate30(int len30, int[] D)
	{
		int num = 0;
		int num2 = len30 - 1;
		for (int i = 0; i < num2; i++)
		{
			num -= D[i];
			D[i] = num & 0x3FFFFFFF;
			num >>= 30;
		}
		return (D[num2] = num - D[num2]) >> 30;
	}

	private static int TrimFG30Var(int len30, int[] F, int[] G)
	{
		int num = F[len30 - 1];
		int num2 = G[len30 - 1];
		if (((len30 - 2 >> 31) | (num ^ (num >> 31)) | (num2 ^ (num2 >> 31))) == 0)
		{
			F[len30 - 2] |= num << 30;
			G[len30 - 2] |= num2 << 30;
			len30--;
		}
		return len30;
	}

	private static void UpdateDE30(int len30, int[] D, int[] E, int[] t, int m0Inv32, int[] M)
	{
		int num = t[0];
		int num2 = t[1];
		int num3 = t[2];
		int num4 = t[3];
		int num5 = D[len30 - 1] >> 31;
		int num6 = E[len30 - 1] >> 31;
		int num7 = (num & num5) + (num2 & num6);
		int num8 = (num3 & num5) + (num4 & num6);
		int num9 = M[0];
		int num10 = D[0];
		int num11 = E[0];
		long num12 = (long)num * (long)num10 + (long)num2 * (long)num11;
		long num13 = (long)num3 * (long)num10 + (long)num4 * (long)num11;
		num7 -= (m0Inv32 * (int)num12 + num7) & 0x3FFFFFFF;
		num8 -= (m0Inv32 * (int)num13 + num8) & 0x3FFFFFFF;
		num12 += (long)num9 * (long)num7;
		num13 += (long)num9 * (long)num8;
		num12 >>= 30;
		num13 >>= 30;
		for (int i = 1; i < len30; i++)
		{
			num9 = M[i];
			num10 = D[i];
			num11 = E[i];
			num12 += (long)num * (long)num10 + (long)num2 * (long)num11 + (long)num9 * (long)num7;
			num13 += (long)num3 * (long)num10 + (long)num4 * (long)num11 + (long)num9 * (long)num8;
			D[i - 1] = (int)num12 & 0x3FFFFFFF;
			num12 >>= 30;
			E[i - 1] = (int)num13 & 0x3FFFFFFF;
			num13 >>= 30;
		}
		D[len30 - 1] = (int)num12;
		E[len30 - 1] = (int)num13;
	}

	private static void UpdateFG30(int len30, int[] F, int[] G, int[] t)
	{
		int num = t[0];
		int num2 = t[1];
		int num3 = t[2];
		int num4 = t[3];
		int num5 = F[0];
		int num6 = G[0];
		long num7 = (long)num * (long)num5 + (long)num2 * (long)num6;
		long num8 = (long)num3 * (long)num5 + (long)num4 * (long)num6;
		num7 >>= 30;
		num8 >>= 30;
		for (int i = 1; i < len30; i++)
		{
			num5 = F[i];
			num6 = G[i];
			num7 += (long)num * (long)num5 + (long)num2 * (long)num6;
			num8 += (long)num3 * (long)num5 + (long)num4 * (long)num6;
			F[i - 1] = (int)num7 & 0x3FFFFFFF;
			num7 >>= 30;
			G[i - 1] = (int)num8 & 0x3FFFFFFF;
			num8 >>= 30;
		}
		F[len30 - 1] = (int)num7;
		G[len30 - 1] = (int)num8;
	}
}
