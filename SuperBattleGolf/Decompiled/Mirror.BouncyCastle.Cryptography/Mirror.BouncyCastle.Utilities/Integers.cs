using System;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Utilities;

public static class Integers
{
	public const int NumBits = 32;

	public const int NumBytes = 4;

	private static readonly byte[] DeBruijnTZ = new byte[32]
	{
		31, 0, 27, 1, 28, 13, 23, 2, 29, 21,
		19, 14, 24, 16, 3, 7, 30, 26, 12, 22,
		20, 18, 15, 6, 25, 11, 17, 5, 10, 4,
		9, 8
	};

	public static int HighestOneBit(int i)
	{
		return (int)HighestOneBit((uint)i);
	}

	[CLSCompliant(false)]
	public static uint HighestOneBit(uint i)
	{
		i |= i >> 1;
		i |= i >> 2;
		i |= i >> 4;
		i |= i >> 8;
		i |= i >> 16;
		return i - (i >> 1);
	}

	public static int LowestOneBit(int i)
	{
		return i & -i;
	}

	[CLSCompliant(false)]
	public static uint LowestOneBit(uint i)
	{
		return (uint)LowestOneBit((int)i);
	}

	public static int NumberOfLeadingZeros(int i)
	{
		if (i <= 0)
		{
			return (~i >> 26) & 0x20;
		}
		uint num = (uint)i;
		int num2 = 1;
		if (num >> 16 == 0)
		{
			num2 += 16;
			num <<= 16;
		}
		if (num >> 24 == 0)
		{
			num2 += 8;
			num <<= 8;
		}
		if (num >> 28 == 0)
		{
			num2 += 4;
			num <<= 4;
		}
		if (num >> 30 == 0)
		{
			num2 += 2;
			num <<= 2;
		}
		return num2 - (int)(num >> 31);
	}

	public static int NumberOfTrailingZeros(int i)
	{
		byte num = DeBruijnTZ[(i & -i) * 251226722 >>> 27];
		int num2 = ((i & 0xFFFF) | (i >>> 16)) - 1 >> 31;
		return num - num2;
	}

	public static int PopCount(int i)
	{
		return PopCount((uint)i);
	}

	[CLSCompliant(false)]
	public static int PopCount(uint u)
	{
		u -= (u >> 1) & 0x55555555;
		u = (u & 0x33333333) + ((u >> 2) & 0x33333333);
		u = (u + (u >> 4)) & 0xF0F0F0F;
		u += u >> 8;
		u += u >> 16;
		u &= 0x3F;
		return (int)u;
	}

	public static int Reverse(int i)
	{
		return (int)Reverse((uint)i);
	}

	[CLSCompliant(false)]
	public static uint Reverse(uint i)
	{
		i = Bits.BitPermuteStepSimple(i, 1431655765u, 1);
		i = Bits.BitPermuteStepSimple(i, 858993459u, 2);
		i = Bits.BitPermuteStepSimple(i, 252645135u, 4);
		return ReverseBytes(i);
	}

	public static int ReverseBytes(int i)
	{
		return (int)ReverseBytes((uint)i);
	}

	[CLSCompliant(false)]
	public static uint ReverseBytes(uint i)
	{
		return RotateLeft(i & 0xFF00FF00u, 8) | RotateLeft(i & 0xFF00FF, 24);
	}

	public static int RotateLeft(int i, int distance)
	{
		return (i << distance) | (i >>> -distance);
	}

	[CLSCompliant(false)]
	public static uint RotateLeft(uint i, int distance)
	{
		return (i << distance) | (i >> -distance);
	}

	public static int RotateRight(int i, int distance)
	{
		return (i >>> distance) | (i << -distance);
	}

	[CLSCompliant(false)]
	public static uint RotateRight(uint i, int distance)
	{
		return (i >> distance) | (i << -distance);
	}
}
