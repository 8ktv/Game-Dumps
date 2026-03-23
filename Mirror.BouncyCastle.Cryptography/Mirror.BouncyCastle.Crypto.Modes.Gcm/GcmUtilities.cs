using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Modes.Gcm;

internal static class GcmUtilities
{
	internal struct FieldElement
	{
		internal ulong n0;

		internal ulong n1;
	}

	private const uint E1 = 3774873600u;

	private const ulong E1UL = 16212958658533785600uL;

	internal static void One(out FieldElement x)
	{
		x.n0 = 9223372036854775808uL;
		x.n1 = 0uL;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AsBytes(ulong x0, ulong x1, byte[] z)
	{
		Pack.UInt64_To_BE(x0, z, 0);
		Pack.UInt64_To_BE(x1, z, 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AsBytes(ref FieldElement x, byte[] z)
	{
		AsBytes(x.n0, x.n1, z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void AsFieldElement(byte[] x, out FieldElement z)
	{
		z.n0 = Pack.BE_To_UInt64(x, 0);
		z.n1 = Pack.BE_To_UInt64(x, 8);
	}

	internal static void DivideP(ref FieldElement x, out FieldElement z)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong num = (ulong)((long)n >> 63);
		n ^= num & 0xE100000000000000uL;
		z.n0 = (n << 1) | (n2 >> 63);
		z.n1 = (n2 << 1) | (0L - num);
	}

	internal static void Multiply(byte[] x, byte[] y)
	{
		AsFieldElement(x, out var z);
		AsFieldElement(y, out var z2);
		Multiply(ref z, ref z2);
		AsBytes(ref z, x);
	}

	internal static void Multiply(ref FieldElement x, ref FieldElement y)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong n3 = y.n0;
		ulong n4 = y.n1;
		ulong num = Longs.Reverse(n);
		ulong num2 = Longs.Reverse(n2);
		ulong num3 = Longs.Reverse(n3);
		ulong num4 = Longs.Reverse(n4);
		ulong num5 = Longs.Reverse(ImplMul64(num, num3));
		ulong num6 = ImplMul64(n, n3) << 1;
		ulong num7 = Longs.Reverse(ImplMul64(num2, num4));
		ulong num8 = ImplMul64(n2, n4) << 1;
		ulong num9 = Longs.Reverse(ImplMul64(num ^ num2, num3 ^ num4));
		ulong num10 = ImplMul64(n ^ n2, n3 ^ n4) << 1;
		ulong num11 = num5;
		ulong num12 = num6 ^ num5 ^ num7 ^ num9;
		ulong num13 = num7 ^ num6 ^ num8 ^ num10;
		ulong num14 = num8;
		num12 ^= num14 ^ (num14 >> 1) ^ (num14 >> 2) ^ (num14 >> 7);
		num13 ^= (num14 << 62) ^ (num14 << 57);
		num11 ^= num13 ^ (num13 >> 1) ^ (num13 >> 2) ^ (num13 >> 7);
		num12 ^= (num13 << 63) ^ (num13 << 62) ^ (num13 << 57);
		x.n0 = num11;
		x.n1 = num12;
	}

	internal static void MultiplyP7(ref FieldElement x)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong num = n2 << 57;
		x.n0 = (n >> 7) ^ num ^ (num >> 1) ^ (num >> 2) ^ (num >> 7);
		x.n1 = (n2 >> 7) | (n << 57);
	}

	internal static void MultiplyP8(ref FieldElement x)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong num = n2 << 56;
		x.n0 = (n >> 8) ^ num ^ (num >> 1) ^ (num >> 2) ^ (num >> 7);
		x.n1 = (n2 >> 8) | (n << 56);
	}

	internal static void MultiplyP8(ref FieldElement x, out FieldElement y)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong num = n2 << 56;
		y.n0 = (n >> 8) ^ num ^ (num >> 1) ^ (num >> 2) ^ (num >> 7);
		y.n1 = (n2 >> 8) | (n << 56);
	}

	internal static void MultiplyP16(ref FieldElement x)
	{
		ulong n = x.n0;
		ulong n2 = x.n1;
		ulong num = n2 << 48;
		x.n0 = (n >> 16) ^ num ^ (num >> 1) ^ (num >> 2) ^ (num >> 7);
		x.n1 = (n2 >> 16) | (n << 48);
	}

	internal static void Square(ref FieldElement x)
	{
		ulong low;
		ulong num = Interleave.Expand64To128Rev(x.n0, out low);
		ulong low2;
		ulong num2 = Interleave.Expand64To128Rev(x.n1, out low2);
		ulong num3 = num ^ num2 ^ (num2 >> 1) ^ (num2 >> 2) ^ (num2 >> 7);
		ulong num4 = low2 ^ (num2 << 62) ^ (num2 << 57);
		x.n0 = low ^ num4 ^ (num4 >> 1) ^ (num4 >> 2) ^ (num4 >> 7);
		x.n1 = num3 ^ (low2 << 62) ^ (low2 << 57);
	}

	internal static void Xor(byte[] x, byte[] y)
	{
		int num = 0;
		do
		{
			x[num] ^= y[num];
			num++;
			x[num] ^= y[num];
			num++;
			x[num] ^= y[num];
			num++;
			x[num] ^= y[num];
			num++;
		}
		while (num < 16);
	}

	internal static void Xor(byte[] x, byte[] y, int yOff)
	{
		int num = 0;
		do
		{
			x[num] ^= y[yOff + num];
			num++;
			x[num] ^= y[yOff + num];
			num++;
			x[num] ^= y[yOff + num];
			num++;
			x[num] ^= y[yOff + num];
			num++;
		}
		while (num < 16);
	}

	internal static void Xor(byte[] x, byte[] y, int yOff, int yLen)
	{
		while (--yLen >= 0)
		{
			x[yLen] ^= y[yOff + yLen];
		}
	}

	internal static void Xor(byte[] x, int xOff, byte[] y, int yOff, int len)
	{
		while (--len >= 0)
		{
			x[xOff + len] ^= y[yOff + len];
		}
	}

	internal static void Xor(ref FieldElement x, ref FieldElement y)
	{
		x.n0 ^= y.n0;
		x.n1 ^= y.n1;
	}

	internal static void Xor(ref FieldElement x, ref FieldElement y, out FieldElement z)
	{
		z.n0 = x.n0 ^ y.n0;
		z.n1 = x.n1 ^ y.n1;
	}

	private static ulong ImplMul64(ulong x, ulong y)
	{
		ulong num = x & 0x1111111111111111L;
		ulong num2 = x & 0x2222222222222222L;
		ulong num3 = x & 0x4444444444444444L;
		ulong num4 = x & 0x8888888888888888uL;
		ulong num5 = y & 0x1111111111111111L;
		ulong num6 = y & 0x2222222222222222L;
		ulong num7 = y & 0x4444444444444444L;
		ulong num8 = y & 0x8888888888888888uL;
		ulong num9 = (num * num5) ^ (num2 * num8) ^ (num3 * num7) ^ (num4 * num6);
		ulong num10 = (num * num6) ^ (num2 * num5) ^ (num3 * num8) ^ (num4 * num7);
		ulong num11 = (num * num7) ^ (num2 * num6) ^ (num3 * num5) ^ (num4 * num8);
		ulong num12 = (num * num8) ^ (num2 * num7) ^ (num3 * num6) ^ (num4 * num5);
		num9 &= 0x1111111111111111L;
		num10 &= 0x2222222222222222L;
		num11 &= 0x4444444444444444L;
		num12 &= 0x8888888888888888uL;
		return num9 | num10 | num11 | num12;
	}
}
