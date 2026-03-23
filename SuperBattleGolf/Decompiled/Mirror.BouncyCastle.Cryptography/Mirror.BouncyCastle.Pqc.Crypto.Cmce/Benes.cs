using System;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal abstract class Benes
{
	private static readonly ulong[] TransposeMasks = new ulong[6] { 6148914691236517205uL, 3689348814741910323uL, 1085102592571150095uL, 71777214294589695uL, 281470681808895uL, 4294967295uL };

	protected readonly int SYS_N;

	protected readonly int SYS_T;

	protected readonly int GFBITS;

	internal Benes(int n, int t, int m)
	{
		SYS_N = n;
		SYS_T = t;
		GFBITS = m;
	}

	internal static void Transpose64x64(ulong[] output, ulong[] input)
	{
		Transpose64x64(output, input, 0);
	}

	internal static void Transpose64x64(ulong[] output, ulong[] input, int offset)
	{
		Array.Copy(input, offset, output, offset, 64);
		int num = 5;
		do
		{
			ulong m = TransposeMasks[num];
			int num2 = 1 << num;
			for (int i = offset; i < offset + 64; i += num2 * 2)
			{
				for (int j = i; j < i + num2; j += 4)
				{
					Bits.BitPermuteStep2(ref output[j + num2], ref output[j], m, num2);
					Bits.BitPermuteStep2(ref output[j + num2 + 1], ref output[j + 1], m, num2);
					Bits.BitPermuteStep2(ref output[j + num2 + 2], ref output[j + 2], m, num2);
					Bits.BitPermuteStep2(ref output[j + num2 + 3], ref output[j + 3], m, num2);
				}
			}
		}
		while (--num >= 2);
		do
		{
			ulong m2 = TransposeMasks[num];
			int num3 = 1 << num;
			for (int k = offset; k < offset + 64; k += num3 * 2)
			{
				for (int l = k; l < k + num3; l++)
				{
					Bits.BitPermuteStep2(ref output[l + num3], ref output[l], m2, num3);
				}
			}
		}
		while (--num >= 0);
	}

	internal abstract void SupportGen(ushort[] s, byte[] c);
}
