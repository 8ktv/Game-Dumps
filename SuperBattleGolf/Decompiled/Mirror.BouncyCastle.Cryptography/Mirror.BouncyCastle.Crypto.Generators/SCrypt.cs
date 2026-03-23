using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class SCrypt
{
	public static byte[] Generate(byte[] P, byte[] S, int N, int r, int p, int dkLen)
	{
		if (P == null)
		{
			throw new ArgumentNullException("Passphrase P must be provided.");
		}
		if (S == null)
		{
			throw new ArgumentNullException("Salt S must be provided.");
		}
		if (N <= 1 || !IsPowerOf2(N))
		{
			throw new ArgumentException("Cost parameter N must be > 1 and a power of 2.");
		}
		if (r == 1 && N >= 65536)
		{
			throw new ArgumentException("Cost parameter N must be > 1 and < 65536.");
		}
		if (r < 1)
		{
			throw new ArgumentException("Block size r must be >= 1.");
		}
		int num = int.MaxValue / (128 * r * 8);
		if (p < 1 || p > num)
		{
			throw new ArgumentException("Parallelisation parameter p must be >= 1 and <= " + num + " (based on block size r of " + r + ")");
		}
		if (dkLen < 1)
		{
			throw new ArgumentException("Generated key length dkLen must be >= 1.");
		}
		return MFcrypt(P, S, N, r, p, dkLen);
	}

	private static byte[] MFcrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen)
	{
		int num = r * 128;
		byte[] array = SingleIterationPBKDF2(P, S, p * num);
		uint[] array2 = null;
		try
		{
			int num2 = array.Length >> 2;
			array2 = new uint[num2];
			Pack.LE_To_UInt32(array, 0, array2);
			int num3 = 0;
			int num4 = N * r;
			while (N - num3 > 2 && num4 > 1024)
			{
				num3++;
				num4 >>= 1;
			}
			int num5 = num >> 2;
			for (int i = 0; i < num2; i += num5)
			{
				SMix(array2, i, N, num3, r);
			}
			Pack.UInt32_To_LE(array2, array, 0);
			return SingleIterationPBKDF2(P, array, dkLen);
		}
		finally
		{
			ClearAll(array, array2);
		}
	}

	private static byte[] SingleIterationPBKDF2(byte[] P, byte[] S, int dkLen)
	{
		Pkcs5S2ParametersGenerator pkcs5S2ParametersGenerator = new Pkcs5S2ParametersGenerator(new Sha256Digest());
		pkcs5S2ParametersGenerator.Init(P, S, 1);
		return ((KeyParameter)pkcs5S2ParametersGenerator.GenerateDerivedMacParameters(dkLen * 8)).GetKey();
	}

	private static void SMix(uint[] B, int BOff, int N, int d, int r)
	{
		int num = Integers.NumberOfTrailingZeros(N);
		int num2 = N >> d;
		int num3 = 1 << d;
		int num4 = num2 - 1;
		int num5 = num - d;
		int num6 = r * 32;
		uint[] array = new uint[16];
		uint[] array2 = new uint[num6];
		uint[] array3 = new uint[num6];
		uint[][] array4 = new uint[num3][];
		try
		{
			Array.Copy(B, BOff, array3, 0, num6);
			for (int i = 0; i < num3; i++)
			{
				uint[] destinationArray = (array4[i] = new uint[num2 * num6]);
				int num7 = 0;
				for (int j = 0; j < num2; j += 2)
				{
					Array.Copy(array3, 0, destinationArray, num7, num6);
					num7 += num6;
					BlockMix(array3, array, array2, r);
					Array.Copy(array2, 0, destinationArray, num7, num6);
					num7 += num6;
					BlockMix(array2, array, array3, r);
				}
			}
			uint num8 = (uint)(N - 1);
			for (int k = 0; k < N; k++)
			{
				int num9 = (int)(array3[num6 - 16] & num8);
				uint[] x = array4[num9 >> num5];
				int xOff = (num9 & num4) * num6;
				Nat.Xor(num6, x, xOff, array3, 0, array2, 0);
				BlockMix(array2, array, array3, r);
			}
			Array.Copy(array3, 0, B, BOff, num6);
		}
		finally
		{
			Array[] arrays = array4;
			ClearAll(arrays);
			ClearAll(array3, array, array2);
		}
	}

	private static void BlockMix(uint[] B, uint[] X1, uint[] Y, int r)
	{
		Array.Copy(B, B.Length - 16, X1, 0, 16);
		int num = 0;
		int num2 = 0;
		int num3 = B.Length >> 1;
		for (int num4 = 2 * r; num4 > 0; num4--)
		{
			Nat512.XorTo(B, num, X1, 0);
			Salsa20Engine.SalsaCore(8, X1, X1);
			Array.Copy(X1, 0, Y, num2, 16);
			num2 = num3 + num - num2;
			num += 16;
		}
	}

	private static void Clear(Array array)
	{
		if (array != null)
		{
			Array.Clear(array, 0, array.Length);
		}
	}

	private static void ClearAll(params Array[] arrays)
	{
		for (int i = 0; i < arrays.Length; i++)
		{
			Clear(arrays[i]);
		}
	}

	private static bool IsPowerOf2(int x)
	{
		return (x & (x - 1)) == 0;
	}
}
