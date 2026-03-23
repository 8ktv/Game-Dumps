using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal class HqcEngine
{
	private int n;

	private int n1;

	private int n2;

	private int k;

	private int delta;

	private int w;

	private int wr;

	private int we;

	private int g;

	private int rejectionThreshold;

	private int fft;

	private int mulParam;

	private int SEED_SIZE = 40;

	private byte G_FCT_DOMAIN = 3;

	private byte H_FCT_DOMAIN = 4;

	private byte K_FCT_DOMAIN = 5;

	private int N_BYTE;

	private int n1n2;

	private int N_BYTE_64;

	private int K_BYTE;

	private int K_BYTE_64;

	private int N1_BYTE_64;

	private int N1N2_BYTE_64;

	private int N1N2_BYTE;

	private int N1_BYTE;

	private int SALT_SIZE_BYTES = 16;

	private int[] generatorPoly;

	private int SHA512_BYTES = 64;

	private ulong RED_MASK;

	private GF2PolynomialCalculator gfCalculator;

	public HqcEngine(int n, int n1, int n2, int k, int g, int delta, int w, int wr, int we, int rejectionThreshold, int fft, int[] generatorPoly)
	{
		this.n = n;
		this.k = k;
		this.delta = delta;
		this.w = w;
		this.wr = wr;
		this.we = we;
		this.n1 = n1;
		this.n2 = n2;
		n1n2 = n1 * n2;
		this.generatorPoly = generatorPoly;
		this.g = g;
		this.rejectionThreshold = rejectionThreshold;
		this.fft = fft;
		mulParam = (n2 + 127) / 128;
		N_BYTE = Utils.GetByteSizeFromBitSize(n);
		K_BYTE = k;
		N_BYTE_64 = Utils.GetByte64SizeFromBitSize(n);
		K_BYTE_64 = Utils.GetByteSizeFromBitSize(k);
		N1_BYTE_64 = Utils.GetByteSizeFromBitSize(n1);
		N1N2_BYTE_64 = Utils.GetByte64SizeFromBitSize(n1 * n2);
		N1N2_BYTE = Utils.GetByteSizeFromBitSize(n1 * n2);
		N1_BYTE = Utils.GetByteSizeFromBitSize(n1);
		RED_MASK = (ulong)((1L << n % 64) - 1);
		gfCalculator = new GF2PolynomialCalculator(N_BYTE_64, n, RED_MASK);
	}

	public void GenKeyPair(byte[] pk, byte[] sk, byte[] seed)
	{
		byte[] array = new byte[SEED_SIZE];
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.RandomGeneratorInit(seed, null, seed.Length, 0);
		hqcKeccakRandomGenerator.Squeeze(array, 40);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator2 = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator2.SeedExpanderInit(array, array.Length);
		long[] array2 = new long[N_BYTE_64];
		long[] array3 = new long[N_BYTE_64];
		GenerateRandomFixedWeight(array2, hqcKeccakRandomGenerator2, w);
		GenerateRandomFixedWeight(array3, hqcKeccakRandomGenerator2, w);
		byte[] array4 = new byte[SEED_SIZE];
		hqcKeccakRandomGenerator.Squeeze(array4, 40);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator3 = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator3.SeedExpanderInit(array4, array4.Length);
		long[] array5 = new long[N_BYTE_64];
		GeneratePublicKeyH(array5, hqcKeccakRandomGenerator3);
		long[] array6 = new long[N_BYTE_64];
		gfCalculator.MultLongs(array6, array3, array5);
		GF2PolynomialCalculator.AddLongs(array6, array6, array2);
		byte[] array7 = new byte[N_BYTE];
		Utils.FromLongArrayToByteArray(array7, array6);
		byte[] array8 = Arrays.Concatenate(array4, array7);
		byte[] array9 = Arrays.Concatenate(array, array8);
		Array.Copy(array8, 0, pk, 0, array8.Length);
		Array.Copy(array9, 0, sk, 0, array9.Length);
	}

	public void Encaps(byte[] u, byte[] v, byte[] K, byte[] d, byte[] pk, byte[] seed, byte[] salt)
	{
		byte[] array = new byte[K_BYTE];
		byte[] output = new byte[SEED_SIZE];
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.RandomGeneratorInit(seed, null, seed.Length, 0);
		hqcKeccakRandomGenerator.Squeeze(output, 40);
		byte[] output2 = new byte[SEED_SIZE];
		hqcKeccakRandomGenerator.Squeeze(output2, 40);
		hqcKeccakRandomGenerator.Squeeze(array, K_BYTE);
		byte[] array2 = new byte[SHA512_BYTES];
		byte[] array3 = new byte[K_BYTE + SEED_SIZE + SALT_SIZE_BYTES];
		hqcKeccakRandomGenerator.Squeeze(salt, SALT_SIZE_BYTES);
		Array.Copy(array, 0, array3, 0, array.Length);
		Array.Copy(pk, 0, array3, K_BYTE, SEED_SIZE);
		Array.Copy(salt, 0, array3, K_BYTE + SEED_SIZE, SALT_SIZE_BYTES);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator2 = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator2.SHAKE256_512_ds(array2, array3, array3.Length, new byte[1] { G_FCT_DOMAIN });
		long[] h = new long[N_BYTE_64];
		byte[] s = new byte[N_BYTE];
		ExtractPublicKeys(h, s, pk);
		long[] array4 = new long[N1N2_BYTE_64];
		Encrypt(u, array4, h, s, array, array2);
		Utils.FromLongArrayToByteArray(v, array4);
		hqcKeccakRandomGenerator2.SHAKE256_512_ds(d, array, array.Length, new byte[1] { H_FCT_DOMAIN });
		byte[] array5 = new byte[K_BYTE + N_BYTE + N1N2_BYTE];
		array5 = Arrays.Concatenate(array, u);
		array5 = Arrays.Concatenate(array5, v);
		hqcKeccakRandomGenerator2.SHAKE256_512_ds(K, array5, array5.Length, new byte[1] { K_FCT_DOMAIN });
	}

	public void Decaps(byte[] ss, byte[] ct, byte[] sk)
	{
		long[] x = new long[N_BYTE_64];
		long[] y = new long[N_BYTE_64];
		byte[] array = new byte[40 + N_BYTE];
		ExtractKeysFromSecretKeys(x, y, array, sk);
		byte[] array2 = new byte[N_BYTE];
		byte[] array3 = new byte[N1N2_BYTE];
		byte[] array4 = new byte[SHA512_BYTES];
		byte[] array5 = new byte[SALT_SIZE_BYTES];
		ExtractCiphertexts(array2, array3, array4, array5, ct);
		byte[] array6 = new byte[k];
		Decrypt(array6, array6, array2, array3, y);
		byte[] array7 = new byte[SHA512_BYTES];
		byte[] array8 = new byte[K_BYTE + SALT_SIZE_BYTES + SEED_SIZE];
		Array.Copy(array6, 0, array8, 0, array6.Length);
		Array.Copy(array, 0, array8, K_BYTE, SEED_SIZE);
		Array.Copy(array5, 0, array8, K_BYTE + SEED_SIZE, SALT_SIZE_BYTES);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.SHAKE256_512_ds(array7, array8, array8.Length, new byte[1] { G_FCT_DOMAIN });
		long[] h = new long[N_BYTE_64];
		byte[] s = new byte[N_BYTE];
		ExtractPublicKeys(h, s, array);
		byte[] array9 = new byte[N_BYTE];
		byte[] array10 = new byte[N1N2_BYTE];
		long[] array11 = new long[N1N2_BYTE_64];
		Encrypt(array9, array11, h, s, array6, array7);
		Utils.FromLongArrayToByteArray(array10, array11);
		byte[] array12 = new byte[SHA512_BYTES];
		hqcKeccakRandomGenerator.SHAKE256_512_ds(array12, array6, array6.Length, new byte[1] { H_FCT_DOMAIN });
		byte[] array13 = new byte[K_BYTE + N_BYTE + N1N2_BYTE];
		array13 = Arrays.Concatenate(array6, array2);
		array13 = Arrays.Concatenate(array13, array3);
		hqcKeccakRandomGenerator.SHAKE256_512_ds(ss, array13, array13.Length, new byte[1] { K_FCT_DOMAIN });
		int num = 1;
		if (!Arrays.AreEqual(array2, array9))
		{
			num = 0;
		}
		if (!Arrays.AreEqual(array3, array10))
		{
			num = 0;
		}
		if (!Arrays.AreEqual(array4, array12))
		{
			num = 0;
		}
		if (num == 0)
		{
			for (int i = 0; i < GetSessionKeySize(); i++)
			{
				ss[i] = 0;
			}
		}
	}

	internal int GetSessionKeySize()
	{
		return SHA512_BYTES;
	}

	private void Encrypt(byte[] u, long[] v, long[] h, byte[] s, byte[] m, byte[] theta)
	{
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.SeedExpanderInit(theta, SEED_SIZE);
		long[] array = new long[N_BYTE_64];
		long[] array2 = new long[N_BYTE_64];
		long[] array3 = new long[N_BYTE_64];
		GenerateRandomFixedWeight(array2, hqcKeccakRandomGenerator, wr);
		GenerateRandomFixedWeight(array3, hqcKeccakRandomGenerator, wr);
		GenerateRandomFixedWeight(array, hqcKeccakRandomGenerator, we);
		long[] array4 = new long[N_BYTE_64];
		gfCalculator.MultLongs(array4, array3, h);
		GF2PolynomialCalculator.AddLongs(array4, array4, array2);
		Utils.FromLongArrayToByteArray(u, array4);
		byte[] array5 = new byte[n1];
		long[] array6 = new long[N1N2_BYTE_64];
		long[] array7 = new long[N_BYTE_64];
		ReedSolomon.Encode(array5, m, K_BYTE * 8, n1, k, g, generatorPoly);
		ReedMuller.Encode(array6, array5, n1, mulParam);
		Array.Copy(array6, 0, array7, 0, array6.Length);
		long[] array8 = new long[N_BYTE_64];
		Utils.FromByteArrayToLongArray(array8, s);
		long[] array9 = new long[N_BYTE_64];
		gfCalculator.MultLongs(array9, array3, array8);
		GF2PolynomialCalculator.AddLongs(array9, array9, array7);
		GF2PolynomialCalculator.AddLongs(array9, array9, array);
		Utils.ResizeArray(v, n1n2, array9, n, N1N2_BYTE_64, N1N2_BYTE_64);
	}

	private void Decrypt(byte[] output, byte[] m, byte[] u, byte[] v, long[] y)
	{
		long[] array = new long[N_BYTE_64];
		Utils.FromByteArrayToLongArray(array, u);
		long[] array2 = new long[N1N2_BYTE_64];
		Utils.FromByteArrayToLongArray(array2, v);
		long[] array3 = new long[N_BYTE_64];
		Array.Copy(array2, 0, array3, 0, array2.Length);
		long[] array4 = new long[N_BYTE_64];
		gfCalculator.MultLongs(array4, y, array);
		GF2PolynomialCalculator.AddLongs(array4, array4, array3);
		byte[] array5 = new byte[n1];
		ReedMuller.Decode(array5, array4, n1, mulParam);
		ReedSolomon.Decode(m, array5, n1, fft, delta, k, g);
		Array.Copy(m, 0, output, 0, output.Length);
	}

	private void GenerateRandomFixedWeight(long[] output, HqcKeccakRandomGenerator random, int weight)
	{
		uint[] array = new uint[wr];
		byte[] array2 = new byte[wr * 4];
		int[] array3 = new int[wr];
		int[] array4 = new int[wr];
		long[] array5 = new long[wr];
		random.ExpandSeed(array2, 4 * weight);
		Pack.LE_To_UInt32(array2, 0, array, 0, array.Length);
		for (int i = 0; i < weight; i++)
		{
			array3[i] = (int)(i + (long)((ulong)array[i] & 0xFFFFFFFFuL) % (long)(n - i));
		}
		for (int num = weight - 1; num >= 0; num--)
		{
			int num2 = 0;
			for (int j = num + 1; j < weight; j++)
			{
				if (array3[j] == array3[num])
				{
					num2 |= 1;
				}
			}
			int num3 = -num2;
			array3[num] = (num3 & num) ^ (~num3 & array3[num]);
		}
		for (int k = 0; k < weight; k++)
		{
			array4[k] = array3[k] >>> 6;
			int num4 = array3[k] & 0x3F;
			array5[k] = 1L << num4;
		}
		long num5 = 0L;
		for (int l = 0; l < N_BYTE_64; l++)
		{
			num5 = 0L;
			for (int m = 0; m < weight; m++)
			{
				int num6 = l - array4[m];
				long num7 = -(1 ^ ((num6 | -num6) >>> 31));
				num5 |= array5[m] & num7;
			}
			output[l] |= num5;
		}
	}

	private void GeneratePublicKeyH(long[] output, HqcKeccakRandomGenerator random)
	{
		byte[] array = new byte[N_BYTE];
		random.ExpandSeed(array, N_BYTE);
		long[] array2 = new long[N_BYTE_64];
		Utils.FromByteArrayToLongArray(array2, array);
		array2[N_BYTE_64 - 1] &= Utils.BitMask((ulong)n, 64uL);
		Array.Copy(array2, 0, output, 0, output.Length);
	}

	private void ExtractPublicKeys(long[] h, byte[] s, byte[] pk)
	{
		byte[] array = new byte[SEED_SIZE];
		Array.Copy(pk, 0, array, 0, array.Length);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.SeedExpanderInit(array, array.Length);
		long[] array2 = new long[N_BYTE_64];
		GeneratePublicKeyH(array2, hqcKeccakRandomGenerator);
		Array.Copy(array2, 0, h, 0, h.Length);
		Array.Copy(pk, 40, s, 0, s.Length);
	}

	private void ExtractKeysFromSecretKeys(long[] x, long[] y, byte[] pk, byte[] sk)
	{
		byte[] array = new byte[SEED_SIZE];
		Array.Copy(sk, 0, array, 0, array.Length);
		HqcKeccakRandomGenerator hqcKeccakRandomGenerator = new HqcKeccakRandomGenerator(256);
		hqcKeccakRandomGenerator.SeedExpanderInit(array, array.Length);
		GenerateRandomFixedWeight(x, hqcKeccakRandomGenerator, w);
		GenerateRandomFixedWeight(y, hqcKeccakRandomGenerator, w);
		Array.Copy(sk, SEED_SIZE, pk, 0, pk.Length);
	}

	private static void ExtractCiphertexts(byte[] u, byte[] v, byte[] d, byte[] salt, byte[] ct)
	{
		Array.Copy(ct, 0, u, 0, u.Length);
		Array.Copy(ct, u.Length, v, 0, v.Length);
		Array.Copy(ct, u.Length + v.Length, d, 0, d.Length);
		Array.Copy(ct, u.Length + v.Length + d.Length, salt, 0, salt.Length);
	}
}
