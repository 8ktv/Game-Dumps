using System;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

internal sealed class SaberEngine
{
	internal const int SABER_EP = 10;

	internal const int SABER_N = 256;

	private const int SABER_SEEDBYTES = 32;

	private const int SABER_NOISE_SEEDBYTES = 32;

	private const int SABER_KEYBYTES = 32;

	private const int SABER_HASHBYTES = 32;

	private readonly int SABER_L;

	private readonly int SABER_MU;

	private readonly int SABER_ET;

	private readonly int SABER_POLYCOINBYTES;

	private readonly int SABER_EQ;

	private readonly int SABER_POLYBYTES;

	private readonly int SABER_POLYVECBYTES;

	private readonly int SABER_POLYCOMPRESSEDBYTES;

	private readonly int SABER_POLYVECCOMPRESSEDBYTES;

	private readonly int SABER_SCALEBYTES_KEM;

	private readonly int SABER_INDCPA_PUBLICKEYBYTES;

	private readonly int SABER_INDCPA_SECRETKEYBYTES;

	private readonly int SABER_PUBLICKEYBYTES;

	private readonly int SABER_SECRETKEYBYTES;

	private readonly int SABER_BYTES_CCA_DEC;

	private readonly int defaultKeySize;

	private int h1;

	private int h2;

	private Symmetric symmetric;

	private SaberUtilities utils;

	private Poly poly;

	private readonly bool usingAes;

	private readonly bool usingEffectiveMasking;

	public bool UsingAes => usingAes;

	public bool UsingEffectiveMasking => usingEffectiveMasking;

	public Symmetric Symmetric => symmetric;

	public int EQ => SABER_EQ;

	public int N => 256;

	public int EP => 10;

	public int KeyBytes => 32;

	public int L => SABER_L;

	public int ET => SABER_ET;

	public int PolyBytes => SABER_POLYBYTES;

	public int PolyVecBytes => SABER_POLYVECBYTES;

	public int SeedBytes => 32;

	public int PolyCoinBytes => SABER_POLYCOINBYTES;

	public int NoiseSeedBytes => 32;

	public int MU => SABER_MU;

	public SaberUtilities Utilities => utils;

	public int GetSessionKeySize()
	{
		return defaultKeySize / 8;
	}

	public int GetCipherTextSize()
	{
		return SABER_BYTES_CCA_DEC;
	}

	public int GetPublicKeySize()
	{
		return SABER_PUBLICKEYBYTES;
	}

	public int GetPrivateKeySize()
	{
		return SABER_SECRETKEYBYTES;
	}

	internal SaberEngine(int l, int defaultKeySize, bool usingAes, bool usingEffectiveMasking)
	{
		this.defaultKeySize = defaultKeySize;
		this.usingAes = usingAes;
		this.usingEffectiveMasking = usingEffectiveMasking;
		SABER_L = l;
		switch (l)
		{
		case 2:
			SABER_MU = 10;
			SABER_ET = 3;
			break;
		case 3:
			SABER_MU = 8;
			SABER_ET = 4;
			break;
		default:
			SABER_MU = 6;
			SABER_ET = 6;
			break;
		}
		if (usingAes)
		{
			symmetric = new Symmetric.AesSymmetric();
		}
		else
		{
			symmetric = new Symmetric.ShakeSymmetric();
		}
		if (usingEffectiveMasking)
		{
			SABER_EQ = 12;
			SABER_POLYCOINBYTES = 64;
		}
		else
		{
			SABER_EQ = 13;
			SABER_POLYCOINBYTES = SABER_MU * 256 / 8;
		}
		SABER_POLYBYTES = SABER_EQ * 256 / 8;
		SABER_POLYVECBYTES = SABER_L * SABER_POLYBYTES;
		SABER_POLYCOMPRESSEDBYTES = 320;
		SABER_POLYVECCOMPRESSEDBYTES = SABER_L * SABER_POLYCOMPRESSEDBYTES;
		SABER_SCALEBYTES_KEM = SABER_ET * 256 / 8;
		SABER_INDCPA_PUBLICKEYBYTES = SABER_POLYVECCOMPRESSEDBYTES + 32;
		SABER_INDCPA_SECRETKEYBYTES = SABER_POLYVECBYTES;
		SABER_PUBLICKEYBYTES = SABER_INDCPA_PUBLICKEYBYTES;
		SABER_SECRETKEYBYTES = SABER_INDCPA_SECRETKEYBYTES + SABER_INDCPA_PUBLICKEYBYTES + 32 + 32;
		SABER_BYTES_CCA_DEC = SABER_POLYVECCOMPRESSEDBYTES + SABER_SCALEBYTES_KEM;
		h1 = 1 << SABER_EQ - 10 - 1;
		h2 = 256 - (1 << 10 - SABER_ET - 1) + (1 << SABER_EQ - 10 - 1);
		utils = new SaberUtilities(this);
		poly = new Poly(this);
	}

	private void indcpa_kem_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		short[][][] array = new short[SABER_L][][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[][] array2 = new short[SABER_L][];
			for (int j = 0; j < SABER_L; j++)
			{
				short[] array3 = new short[256];
				array2[j] = array3;
			}
			array[i] = array2;
		}
		short[][] array4 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array5 = new short[256];
			array4[i] = array5;
		}
		short[][] array6 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array7 = new short[256];
			array6[i] = array7;
		}
		byte[] array8 = new byte[32];
		byte[] array9 = new byte[32];
		random.NextBytes(array8);
		symmetric.Prf(array8, array8, 32, 32);
		random.NextBytes(array9);
		poly.GenMatrix(array, array8);
		poly.GenSecret(array4, array9);
		poly.MatrixVectorMul(array, array4, array6, 1);
		for (int i = 0; i < SABER_L; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				array6[i][j] = (short)(((array6[i][j] + h1) & 0xFFFF) >> SABER_EQ - 10);
			}
		}
		utils.POLVECq2BS(sk, array4);
		utils.POLVECp2BS(pk, array6);
		Array.Copy(array8, 0, pk, SABER_POLYVECCOMPRESSEDBYTES, array8.Length);
	}

	public int crypto_kem_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		indcpa_kem_keypair(pk, sk, random);
		for (int i = 0; i < SABER_INDCPA_PUBLICKEYBYTES; i++)
		{
			sk[i + SABER_INDCPA_SECRETKEYBYTES] = pk[i];
		}
		symmetric.Hash_h(sk, pk, SABER_SECRETKEYBYTES - 64);
		byte[] array = new byte[32];
		random.NextBytes(array);
		Array.Copy(array, 0, sk, SABER_SECRETKEYBYTES - 32, array.Length);
		return 0;
	}

	private void indcpa_kem_enc(byte[] m, byte[] seed_sp, byte[] pk, byte[] ciphertext)
	{
		short[][][] array = new short[SABER_L][][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[][] array2 = new short[SABER_L][];
			for (int j = 0; j < SABER_L; j++)
			{
				short[] array3 = new short[256];
				array2[j] = array3;
			}
			array[i] = array2;
		}
		short[][] array4 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array5 = new short[256];
			array4[i] = array5;
		}
		short[][] array6 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array7 = new short[256];
			array6[i] = array7;
		}
		short[][] array8 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array9 = new short[256];
			array8[i] = array9;
		}
		short[] array10 = new short[256];
		short[] array11 = new short[256];
		byte[] seed = Arrays.CopyOfRange(pk, SABER_POLYVECCOMPRESSEDBYTES, pk.Length);
		poly.GenMatrix(array, seed);
		poly.GenSecret(array4, seed_sp);
		poly.MatrixVectorMul(array, array4, array6, 0);
		for (int i = 0; i < SABER_L; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				array6[i][j] = (short)(((array6[i][j] + h1) & 0xFFFF) >> SABER_EQ - 10);
			}
		}
		utils.POLVECp2BS(ciphertext, array6);
		utils.BS2POLVECp(pk, array8);
		poly.InnerProd(array8, array4, array11);
		utils.BS2POLmsg(m, array10);
		for (int j = 0; j < 256; j++)
		{
			array11[j] = (short)(((array11[j] - (array10[j] << 9) + h1) & 0xFFFF) >> 10 - SABER_ET);
		}
		utils.POLT2BS(ciphertext, SABER_POLYVECCOMPRESSEDBYTES, array11);
	}

	public int crypto_kem_enc(byte[] c, byte[] k, byte[] pk, SecureRandom random)
	{
		byte[] array = new byte[64];
		byte[] array2 = new byte[64];
		byte[] array3 = new byte[32];
		random.NextBytes(array3);
		symmetric.Hash_h(array3, array3, 0);
		Array.Copy(array3, 0, array2, 0, 32);
		symmetric.Hash_h(array2, pk, 32);
		symmetric.Hash_g(array, array2);
		indcpa_kem_enc(array2, Arrays.CopyOfRange(array, 32, array.Length), pk, c);
		symmetric.Hash_h(array, c, 32);
		byte[] array4 = new byte[32];
		symmetric.Hash_h(array4, array, 0);
		Array.Copy(array4, 0, k, 0, defaultKeySize / 8);
		return 0;
	}

	private void indcpa_kem_dec(byte[] sk, byte[] ciphertext, byte[] m)
	{
		short[][] array = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array2 = new short[256];
			array[i] = array2;
		}
		short[][] array3 = new short[SABER_L][];
		for (int i = 0; i < SABER_L; i++)
		{
			short[] array4 = new short[256];
			array3[i] = array4;
		}
		short[] array5 = new short[256];
		short[] array6 = new short[256];
		utils.BS2POLVECq(sk, 0, array);
		utils.BS2POLVECp(ciphertext, array3);
		poly.InnerProd(array3, array, array5);
		utils.BS2POLT(ciphertext, SABER_POLYVECCOMPRESSEDBYTES, array6);
		for (int i = 0; i < 256; i++)
		{
			array5[i] = (short)(((array5[i] + h2 - (array6[i] << 10 - SABER_ET)) & 0xFFFF) >> 9);
		}
		utils.POLmsg2BS(m, array5);
	}

	public int crypto_kem_dec(byte[] k, byte[] c, byte[] sk)
	{
		byte[] array = new byte[SABER_BYTES_CCA_DEC];
		byte[] array2 = new byte[64];
		byte[] array3 = new byte[64];
		byte[] pk = Arrays.CopyOfRange(sk, SABER_INDCPA_SECRETKEYBYTES, sk.Length);
		indcpa_kem_dec(sk, c, array2);
		for (int i = 0; i < 32; i++)
		{
			array2[32 + i] = sk[SABER_SECRETKEYBYTES - 64 + i];
		}
		symmetric.Hash_g(array3, array2);
		indcpa_kem_enc(array2, Arrays.CopyOfRange(array3, 32, array3.Length), pk, array);
		int num = verify(c, array, SABER_BYTES_CCA_DEC);
		symmetric.Hash_h(array3, c, 32);
		cmov(array3, sk, SABER_SECRETKEYBYTES - 32, 32, (byte)num);
		byte[] array4 = new byte[32];
		symmetric.Hash_h(array4, array3, 0);
		Array.Copy(array4, 0, k, 0, defaultKeySize / 8);
		return 0;
	}

	private static int verify(byte[] a, byte[] b, int len)
	{
		int num = 0;
		for (int i = 0; i < len; i++)
		{
			num |= a[i] ^ b[i];
		}
		return -num >>> 31;
	}

	private static void cmov(byte[] r, byte[] x, int x_offset, int len, byte b)
	{
		b = (byte)(-b);
		for (int i = 0; i < len; i++)
		{
			r[i] ^= (byte)(b & (x[i + x_offset] ^ r[i]));
		}
	}
}
