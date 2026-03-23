using System;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Math.EC.Rfc8032;

internal static class Scalar25519
{
	internal const int Size = 8;

	private const int ScalarBytes = 32;

	private const long M28L = 268435455L;

	private const int TargetLength = 254;

	private static readonly uint[] L = new uint[8] { 1559614445u, 1477600026u, 2734136534u, 350157278u, 0u, 0u, 0u, 268435456u };

	private static readonly uint[] LSq = new uint[16]
	{
		2870118761u, 3807245957u, 580428573u, 1745064566u, 3524785598u, 1036971123u, 461123738u, 2712901953u, 1268693629u, 3405925475u,
		3562992538u, 43769659u, 0u, 0u, 0u, 16777216u
	};

	private const int L0 = -50998291;

	private const int L1 = 19280294;

	private const int L2 = 127719000;

	private const int L3 = -6428113;

	private const int L4 = 5343;

	internal static bool CheckVar(byte[] s, uint[] n)
	{
		Decode(s, n);
		return !Nat256.Gte(n, L);
	}

	internal static void Decode(byte[] k, uint[] n)
	{
		Codec.Decode32(k, 0, n, 0, 8);
	}

	internal static void GetOrderWnafVar(int width, sbyte[] ws)
	{
		Wnaf.GetSignedVar(L, width, ws);
	}

	internal static void Multiply128Var(uint[] x, uint[] y128, uint[] z)
	{
		uint[] array = new uint[12];
		Nat256.Mul128(x, y128, array);
		if ((int)y128[3] < 0)
		{
			Nat256.AddTo(L, 0, array, 4, 0u);
			Nat256.SubFrom(x, 0, array, 4, 0);
		}
		byte[] array2 = new byte[48];
		Codec.Encode32(array, 0, 12, array2, 0);
		Decode(Reduce384(array2), z);
	}

	internal static byte[] Reduce384(byte[] n)
	{
		byte[] array = new byte[32];
		long num = Codec.Decode32(n, 0);
		long num2 = Codec.Decode24(n, 4) << 4;
		long num3 = Codec.Decode32(n, 7);
		long num4 = Codec.Decode24(n, 11) << 4;
		long num5 = Codec.Decode32(n, 14);
		long num6 = Codec.Decode24(n, 18) << 4;
		long num7 = Codec.Decode32(n, 21);
		long num8 = Codec.Decode24(n, 25) << 4;
		long num9 = Codec.Decode32(n, 28);
		long num10 = Codec.Decode24(n, 32) << 4;
		long num11 = Codec.Decode32(n, 35);
		long num12 = Codec.Decode24(n, 39) << 4;
		long num13 = Codec.Decode32(n, 42);
		long num14 = Codec.Decode16(n, 46) << 4;
		num14 += num13 >> 28;
		num13 &= 0xFFFFFFF;
		num5 -= num14 * -50998291;
		num6 -= num14 * 19280294;
		num7 -= num14 * 127719000;
		num8 -= num14 * -6428113;
		num9 -= num14 * 5343;
		num13 += num12 >> 28;
		num12 &= 0xFFFFFFF;
		num4 -= num13 * -50998291;
		num5 -= num13 * 19280294;
		num6 -= num13 * 127719000;
		num7 -= num13 * -6428113;
		num8 -= num13 * 5343;
		num12 += num11 >> 28;
		num11 &= 0xFFFFFFF;
		num3 -= num12 * -50998291;
		num4 -= num12 * 19280294;
		num5 -= num12 * 127719000;
		num6 -= num12 * -6428113;
		num7 -= num12 * 5343;
		num11 += num10 >> 28;
		num10 &= 0xFFFFFFF;
		num2 -= num11 * -50998291;
		num3 -= num11 * 19280294;
		num4 -= num11 * 127719000;
		num5 -= num11 * -6428113;
		num6 -= num11 * 5343;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 += num9 >> 28;
		num9 &= 0xFFFFFFF;
		long num15 = (num9 >> 27) & 1;
		num10 += num15;
		num -= num10 * -50998291;
		num2 -= num10 * 19280294;
		num3 -= num10 * 127719000;
		num4 -= num10 * -6428113;
		num5 -= num10 * 5343;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 = num9 >> 28;
		num9 &= 0xFFFFFFF;
		num10 -= num15;
		num += num10 & -50998291;
		num2 += num10 & 0x12631A6;
		num3 += num10 & 0x79CD658;
		num4 += num10 & -6428113;
		num5 += num10 & 0x14DF;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		Codec.Encode56((ulong)(num | (num2 << 28)), array, 0);
		Codec.Encode56((ulong)(num3 | (num4 << 28)), array, 7);
		Codec.Encode56((ulong)(num5 | (num6 << 28)), array, 14);
		Codec.Encode56((ulong)(num7 | (num8 << 28)), array, 21);
		Codec.Encode32((uint)num9, array, 28);
		return array;
	}

	internal static byte[] Reduce512(byte[] n)
	{
		byte[] array = new byte[32];
		long num = Codec.Decode32(n, 0);
		long num2 = Codec.Decode24(n, 4) << 4;
		long num3 = Codec.Decode32(n, 7);
		long num4 = Codec.Decode24(n, 11) << 4;
		long num5 = Codec.Decode32(n, 14);
		long num6 = Codec.Decode24(n, 18) << 4;
		long num7 = Codec.Decode32(n, 21);
		long num8 = Codec.Decode24(n, 25) << 4;
		long num9 = Codec.Decode32(n, 28);
		long num10 = Codec.Decode24(n, 32) << 4;
		long num11 = Codec.Decode32(n, 35);
		long num12 = Codec.Decode24(n, 39) << 4;
		long num13 = Codec.Decode32(n, 42);
		long num14 = Codec.Decode24(n, 46) << 4;
		long num15 = Codec.Decode32(n, 49);
		long num16 = Codec.Decode24(n, 53) << 4;
		long num17 = Codec.Decode32(n, 56);
		long num18 = Codec.Decode24(n, 60) << 4;
		long num19 = n[63];
		num10 -= num19 * -50998291;
		num11 -= num19 * 19280294;
		num12 -= num19 * 127719000;
		num13 -= num19 * -6428113;
		num14 -= num19 * 5343;
		num18 += num17 >> 28;
		num17 &= 0xFFFFFFF;
		num9 -= num18 * -50998291;
		num10 -= num18 * 19280294;
		num11 -= num18 * 127719000;
		num12 -= num18 * -6428113;
		num13 -= num18 * 5343;
		num8 -= num17 * -50998291;
		num9 -= num17 * 19280294;
		num10 -= num17 * 127719000;
		num11 -= num17 * -6428113;
		num12 -= num17 * 5343;
		num16 += num15 >> 28;
		num15 &= 0xFFFFFFF;
		num7 -= num16 * -50998291;
		num8 -= num16 * 19280294;
		num9 -= num16 * 127719000;
		num10 -= num16 * -6428113;
		num11 -= num16 * 5343;
		num6 -= num15 * -50998291;
		num7 -= num15 * 19280294;
		num8 -= num15 * 127719000;
		num9 -= num15 * -6428113;
		num10 -= num15 * 5343;
		num14 += num13 >> 28;
		num13 &= 0xFFFFFFF;
		num5 -= num14 * -50998291;
		num6 -= num14 * 19280294;
		num7 -= num14 * 127719000;
		num8 -= num14 * -6428113;
		num9 -= num14 * 5343;
		num13 += num12 >> 28;
		num12 &= 0xFFFFFFF;
		num4 -= num13 * -50998291;
		num5 -= num13 * 19280294;
		num6 -= num13 * 127719000;
		num7 -= num13 * -6428113;
		num8 -= num13 * 5343;
		num12 += num11 >> 28;
		num11 &= 0xFFFFFFF;
		num3 -= num12 * -50998291;
		num4 -= num12 * 19280294;
		num5 -= num12 * 127719000;
		num6 -= num12 * -6428113;
		num7 -= num12 * 5343;
		num11 += num10 >> 28;
		num10 &= 0xFFFFFFF;
		num2 -= num11 * -50998291;
		num3 -= num11 * 19280294;
		num4 -= num11 * 127719000;
		num5 -= num11 * -6428113;
		num6 -= num11 * 5343;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 += num9 >> 28;
		num9 &= 0xFFFFFFF;
		long num20 = (num9 >> 27) & 1;
		num10 += num20;
		num -= num10 * -50998291;
		num2 -= num10 * 19280294;
		num3 -= num10 * 127719000;
		num4 -= num10 * -6428113;
		num5 -= num10 * 5343;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		num10 = num9 >> 28;
		num9 &= 0xFFFFFFF;
		num10 -= num20;
		num += num10 & -50998291;
		num2 += num10 & 0x12631A6;
		num3 += num10 & 0x79CD658;
		num4 += num10 & -6428113;
		num5 += num10 & 0x14DF;
		num2 += num >> 28;
		num &= 0xFFFFFFF;
		num3 += num2 >> 28;
		num2 &= 0xFFFFFFF;
		num4 += num3 >> 28;
		num3 &= 0xFFFFFFF;
		num5 += num4 >> 28;
		num4 &= 0xFFFFFFF;
		num6 += num5 >> 28;
		num5 &= 0xFFFFFFF;
		num7 += num6 >> 28;
		num6 &= 0xFFFFFFF;
		num8 += num7 >> 28;
		num7 &= 0xFFFFFFF;
		num9 += num8 >> 28;
		num8 &= 0xFFFFFFF;
		Codec.Encode56((ulong)(num | (num2 << 28)), array, 0);
		Codec.Encode56((ulong)(num3 | (num4 << 28)), array, 7);
		Codec.Encode56((ulong)(num5 | (num6 << 28)), array, 14);
		Codec.Encode56((ulong)(num7 | (num8 << 28)), array, 21);
		Codec.Encode32((uint)num9, array, 28);
		return array;
	}

	internal static bool ReduceBasisVar(uint[] k, uint[] z0, uint[] z1)
	{
		uint[] x = new uint[16];
		Array.Copy(LSq, x, 16);
		uint[] y = new uint[16];
		Nat256.Square(k, y);
		y[0]++;
		uint[] array = new uint[16];
		Nat256.Mul(L, k, array);
		uint[] t = new uint[16];
		uint[] x2 = new uint[4];
		Array.Copy(L, x2, 4);
		uint[] x3 = new uint[4];
		uint[] y2 = new uint[4];
		Array.Copy(k, y2, 4);
		uint[] y3 = new uint[4] { 1u, 0u, 0u, 0u };
		int num = 1016;
		int num2 = 15;
		int bitLengthPositive = ScalarUtilities.GetBitLengthPositive(num2, y);
		while (bitLengthPositive > 254)
		{
			if (--num < 0)
			{
				return false;
			}
			int num3 = ScalarUtilities.GetBitLength(num2, array) - bitLengthPositive;
			num3 &= ~(num3 >> 31);
			if ((int)array[num2] < 0)
			{
				ScalarUtilities.AddShifted_NP(num2, num3, x, y, array, t);
				ScalarUtilities.AddShifted_UV(3, num3, x2, x3, y2, y3);
			}
			else
			{
				ScalarUtilities.SubShifted_NP(num2, num3, x, y, array, t);
				ScalarUtilities.SubShifted_UV(3, num3, x2, x3, y2, y3);
			}
			if (ScalarUtilities.LessThan(num2, x, y))
			{
				ScalarUtilities.Swap(ref x2, ref y2);
				ScalarUtilities.Swap(ref x3, ref y3);
				ScalarUtilities.Swap(ref x, ref y);
				num2 = bitLengthPositive >> 5;
				bitLengthPositive = ScalarUtilities.GetBitLengthPositive(num2, y);
			}
		}
		Array.Copy(y2, z0, 4);
		Array.Copy(y3, z1, 4);
		return true;
	}

	internal static void ToSignedDigits(int bits, uint[] z)
	{
		Nat.CAddTo(8, (int)(~z[0] & 1), L, z);
		Nat.ShiftDownBit(8, z, 1u);
	}
}
