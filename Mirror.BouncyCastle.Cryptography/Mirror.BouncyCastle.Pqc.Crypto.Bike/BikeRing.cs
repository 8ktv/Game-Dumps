using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

internal sealed class BikeRing
{
	private const int PermutationCutoff = 64;

	private readonly int m_bits;

	private readonly int m_size;

	private readonly int m_sizeExt;

	private readonly Dictionary<int, int> m_halfPowers = new Dictionary<int, int>();

	internal int Size => m_size;

	internal int SizeExt => m_sizeExt;

	internal BikeRing(int r)
	{
		if ((r & 0xFFFF0001u) != 1)
		{
			throw new ArgumentException();
		}
		m_bits = r;
		m_size = r + 63 >> 6;
		m_sizeExt = m_size * 2;
		uint r2 = Mod.Inverse32((uint)(-r));
		foreach (int item in EnumerateSquarePowersInv(r))
		{
			if (item >= 64 && !m_halfPowers.ContainsKey(item))
			{
				m_halfPowers[item] = GenerateHalfPower((uint)r, r2, item);
			}
		}
	}

	internal void Add(ulong[] x, ulong[] y, ulong[] z)
	{
		Nat.Xor64(Size, x, y, z);
	}

	internal void AddTo(ulong[] x, ulong[] z)
	{
		Nat.XorTo64(Size, x, z);
	}

	internal void Copy(ulong[] x, ulong[] z)
	{
		for (int i = 0; i < Size; i++)
		{
			z[i] = x[i];
		}
	}

	internal ulong[] Create()
	{
		return new ulong[Size];
	}

	internal ulong[] CreateExt()
	{
		return new ulong[SizeExt];
	}

	internal void DecodeBytes(byte[] bs, ulong[] z)
	{
		int len = (m_bits & 0x3F) + 7 >> 3;
		Pack.LE_To_UInt64(bs, 0, z, 0, Size - 1);
		z[Size - 1] = Pack.LE_To_UInt64_Low(bs, Size - 1 << 3, len);
	}

	internal byte[] EncodeBitsTransposed(ulong[] x)
	{
		byte[] array = new byte[m_bits];
		array[0] = (byte)(x[0] & 1);
		for (int i = 1; i < m_bits; i++)
		{
			array[m_bits - i] = (byte)((x[i >> 6] >> i) & 1);
		}
		return array;
	}

	internal void EncodeBytes(ulong[] x, byte[] bs)
	{
		int len = (m_bits & 0x3F) + 7 >> 3;
		Pack.UInt64_To_LE(x, 0, Size - 1, bs, 0);
		Pack.UInt64_To_LE_Low(x[Size - 1], bs, Size - 1 << 3, len);
	}

	internal void Inv(ulong[] a, ulong[] z)
	{
		ulong[] array = Create();
		ulong[] array2 = Create();
		ulong[] array3 = Create();
		Copy(a, array);
		Copy(a, array3);
		int num = m_bits - 2;
		int num2 = 32 - Integers.NumberOfLeadingZeros(num);
		for (int i = 1; i < num2; i++)
		{
			SquareN(array, 1 << i - 1, array2);
			Multiply(array, array2, array);
			if ((num & (1 << i)) != 0)
			{
				int n = num & ((1 << i) - 1);
				SquareN(array, n, array2);
				Multiply(array3, array2, array3);
			}
		}
		Square(array3, z);
	}

	internal void Multiply(ulong[] x, ulong[] y, ulong[] z)
	{
		Multiply(x, 0, y, 0, z);
	}

	internal void Multiply(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z)
	{
		ulong[] array = CreateExt();
		ImplMultiplyAcc(x, xOff, y, yOff, array);
		Reduce(array, z);
	}

	internal void Reduce(ulong[] tt, ulong[] z)
	{
		int num = m_bits & 0x3F;
		int num2 = 64 - num;
		ulong num3 = ulong.MaxValue >> num2;
		Nat.ShiftUpBits64(Size, tt, Size, num2, tt[Size - 1], z, 0);
		AddTo(tt, z);
		z[Size - 1] &= num3;
	}

	internal void Square(ulong[] x, ulong[] z)
	{
		ulong[] array = CreateExt();
		ImplSquare(x, array);
		Reduce(array, z);
	}

	internal void SquareN(ulong[] x, int n, ulong[] z)
	{
		if (n >= 64)
		{
			ImplPermute(x, n, z);
			return;
		}
		ulong[] array = CreateExt();
		ImplSquare(x, array);
		Reduce(array, z);
		while (--n > 0)
		{
			ImplSquare(z, array);
			Reduce(array, z);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int ImplModAdd(int m, int x, int y)
	{
		int num = x + y - m;
		return num + ((num >> 31) & m);
	}

	private void ImplMultiplyAcc(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] zz)
	{
		_ = x[xOff + Size - 1];
		_ = y[yOff + Size - 1];
		_ = zz[SizeExt - 1];
		ulong[] u = new ulong[16];
		for (int i = 0; i < Size; i++)
		{
			ImplMulwAcc(u, x[xOff + i], y[yOff + i], zz, i << 1);
		}
		ulong num = zz[0];
		ulong num2 = zz[1];
		for (int j = 1; j < Size; j++)
		{
			num ^= zz[j << 1];
			zz[j] = num ^ num2;
			num2 ^= zz[(j << 1) + 1];
		}
		ulong y2 = num ^ num2;
		Nat.Xor64(Size, zz, 0, y2, zz, Size);
		int num3 = Size - 1;
		for (int k = 1; k < num3 * 2; k++)
		{
			int num4 = System.Math.Min(num3, k);
			int num5 = k - num4;
			while (num5 < num4)
			{
				ImplMulwAcc(u, x[xOff + num5] ^ x[xOff + num4], y[yOff + num5] ^ y[yOff + num4], zz, k);
				num5++;
				num4--;
			}
		}
	}

	private void ImplPermute(ulong[] x, int n, ulong[] z)
	{
		int bits = m_bits;
		int num = m_halfPowers[n];
		int num2 = ImplModAdd(bits, num, num);
		int num3 = ImplModAdd(bits, num2, num2);
		int num4 = ImplModAdd(bits, num3, num3);
		int num5 = bits - num4;
		int num6 = ImplModAdd(bits, num5, num);
		int num7 = ImplModAdd(bits, num5, num2);
		int num8 = ImplModAdd(bits, num6, num2);
		int num9 = ImplModAdd(bits, num5, num3);
		int num10 = ImplModAdd(bits, num6, num3);
		int num11 = ImplModAdd(bits, num7, num3);
		int num12 = ImplModAdd(bits, num8, num3);
		for (int i = 0; i < Size; i++)
		{
			ulong num13 = 0uL;
			for (int j = 0; j < 64; j += 8)
			{
				num5 = ImplModAdd(bits, num5, num4);
				num6 = ImplModAdd(bits, num6, num4);
				num7 = ImplModAdd(bits, num7, num4);
				num8 = ImplModAdd(bits, num8, num4);
				num9 = ImplModAdd(bits, num9, num4);
				num10 = ImplModAdd(bits, num10, num4);
				num11 = ImplModAdd(bits, num11, num4);
				num12 = ImplModAdd(bits, num12, num4);
				num13 |= ((x[num5 >> 6] >> num5) & 1) << j;
				num13 |= ((x[num6 >> 6] >> num6) & 1) << j + 1;
				num13 |= ((x[num7 >> 6] >> num7) & 1) << j + 2;
				num13 |= ((x[num8 >> 6] >> num8) & 1) << j + 3;
				num13 |= ((x[num9 >> 6] >> num9) & 1) << j + 4;
				num13 |= ((x[num10 >> 6] >> num10) & 1) << j + 5;
				num13 |= ((x[num11 >> 6] >> num11) & 1) << j + 6;
				num13 |= ((x[num12 >> 6] >> num12) & 1) << j + 7;
			}
			z[i] = num13;
		}
		z[Size - 1] &= ulong.MaxValue >> -bits;
	}

	private static IEnumerable<int> EnumerateSquarePowersInv(int r)
	{
		int rSub2 = r - 2;
		int bits = 32 - Integers.NumberOfLeadingZeros(rSub2);
		int i = 1;
		while (i < bits)
		{
			yield return 1 << i - 1;
			if ((rSub2 & (1 << i)) != 0)
			{
				yield return rSub2 & ((1 << i) - 1);
			}
			int num = i + 1;
			i = num;
		}
	}

	private static int GenerateHalfPower(uint r, uint r32, int n)
	{
		uint num = 1u;
		int num2;
		for (num2 = n; num2 >= 32; num2 -= 32)
		{
			num = (uint)((ulong)((long)(r32 * num) * (long)r + num) >> 32);
		}
		if (num2 > 0)
		{
			uint num3 = uint.MaxValue >> -num2;
			num = (uint)((ulong)((long)((r32 * num) & num3) * (long)r + num) >> num2);
		}
		return (int)num;
	}

	private static void ImplMulwAcc(ulong[] u, ulong x, ulong y, ulong[] z, int zOff)
	{
		u[1] = y;
		for (int i = 2; i < 16; i += 2)
		{
			u[i] = u[i >> 1] << 1;
			u[i + 1] = u[i] ^ y;
		}
		uint num = (uint)x;
		ulong num2 = 0uL;
		ulong num3 = u[num & 0xF] ^ (u[(num >> 4) & 0xF] << 4);
		int num4 = 56;
		do
		{
			num = (uint)(x >> num4);
			ulong num5 = u[num & 0xF] ^ (u[(num >> 4) & 0xF] << 4);
			num3 ^= num5 << num4;
			num2 ^= num5 >> -num4;
		}
		while ((num4 -= 8) > 0);
		for (int j = 0; j < 7; j++)
		{
			x = (x & 0xFEFEFEFEFEFEFEFEuL) >> 1;
			num2 ^= x & (ulong)((long)(y << j) >> 63);
		}
		z[zOff] ^= num3;
		z[zOff + 1] ^= num2;
	}

	private void ImplSquare(ulong[] x, ulong[] zz)
	{
		Interleave.Expand64To128(x, 0, Size, zz, 0);
	}
}
