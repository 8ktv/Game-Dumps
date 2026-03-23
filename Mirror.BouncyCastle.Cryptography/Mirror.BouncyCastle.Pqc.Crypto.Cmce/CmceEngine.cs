using System;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal class CmceEngine<GFImpl> : ICmceEngine where GFImpl : struct, GF
{
	private int SYS_N;

	private int SYS_T;

	private int GFBITS;

	private int IRR_BYTES;

	private int COND_BYTES;

	private int PK_NROWS;

	private int PK_NCOLS;

	private int PK_ROW_BYTES;

	private int SYND_BYTES;

	private int GFMASK;

	private int[] poly;

	private int defaultKeySize;

	private readonly GFImpl gf;

	private readonly Benes benes;

	private bool usePadding;

	private bool countErrorIndices;

	private bool usePivots;

	public int IrrBytes => IRR_BYTES;

	public int CondBytes => COND_BYTES;

	public int PrivateKeySize => COND_BYTES + IRR_BYTES + SYS_N / 8 + 40;

	public int PublicKeySize
	{
		get
		{
			if (!usePadding)
			{
				return PK_NROWS * PK_NCOLS / 8;
			}
			return PK_NROWS * (SYS_N / 8 - (PK_NROWS - 1) / 8);
		}
	}

	public int CipherTextSize => SYND_BYTES;

	public int DefaultSessionKeySize => defaultKeySize;

	internal CmceEngine(int m, int n, int t, int[] p, bool usePivots, int defaultKeySize)
	{
		this.usePivots = usePivots;
		SYS_N = n;
		SYS_T = t;
		GFBITS = m;
		poly = p;
		this.defaultKeySize = defaultKeySize;
		IRR_BYTES = SYS_T * 2;
		COND_BYTES = (1 << GFBITS - 4) * (2 * GFBITS - 1);
		PK_NROWS = SYS_T * GFBITS;
		PK_NCOLS = SYS_N - PK_NROWS;
		PK_ROW_BYTES = (PK_NCOLS + 7) / 8;
		SYND_BYTES = (PK_NROWS + 7) / 8;
		GFMASK = (1 << GFBITS) - 1;
		gf = default(GFImpl);
		if (GFBITS == 12)
		{
			benes = new Benes12(SYS_N, SYS_T, GFBITS);
		}
		else
		{
			benes = new Benes13(SYS_N, SYS_T, GFBITS);
		}
		usePadding = SYS_T % 8 != 0;
		countErrorIndices = 1 << GFBITS > SYS_N;
	}

	public byte[] GeneratePublicKeyFromPrivateKey(byte[] sk)
	{
		byte[] array = new byte[PublicKeySize];
		ushort[] pi = new ushort[1 << GFBITS];
		ulong[] pivots = new ulong[1];
		uint[] array2 = new uint[1 << GFBITS];
		byte[] array3 = new byte[SYS_N / 8 + (1 << GFBITS) * 4];
		int num = array3.Length - 32 - IRR_BYTES - (1 << GFBITS) * 4;
		IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		digest.Update(64);
		digest.BlockUpdate(sk, 0, 32);
		((IXof)digest).OutputFinal(array3, 0, array3.Length);
		for (int i = 0; i < 1 << GFBITS; i++)
		{
			array2[i] = Utils.Load4(array3, num + i * 4);
		}
		PKGen(array, sk, array2, pi, pivots);
		return array;
	}

	public byte[] DecompressPrivateKey(byte[] sk)
	{
		byte[] array = new byte[PrivateKeySize];
		Array.Copy(sk, 0, array, 0, sk.Length);
		byte[] array2 = new byte[SYS_N / 8 + (1 << GFBITS) * 4 + IRR_BYTES + 32];
		IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		digest.Update(64);
		digest.BlockUpdate(sk, 0, 32);
		((IXof)digest).OutputFinal(array2, 0, array2.Length);
		if (sk.Length <= 40)
		{
			ushort[] array3 = new ushort[SYS_T];
			byte[] array4 = new byte[IRR_BYTES];
			int num = array2.Length - 32 - IRR_BYTES;
			for (int i = 0; i < SYS_T; i++)
			{
				array3[i] = Utils.LoadGF(array2, num + i * 2, GFMASK);
			}
			GenerateIrrPoly(array3);
			for (int j = 0; j < SYS_T; j++)
			{
				Utils.StoreGF(array4, j * 2, array3[j]);
			}
			Array.Copy(array4, 0, array, 40, IRR_BYTES);
		}
		if (sk.Length <= 40 + IRR_BYTES)
		{
			uint[] array5 = new uint[1 << GFBITS];
			ushort[] array6 = new ushort[1 << GFBITS];
			int num2 = array2.Length - 32 - IRR_BYTES - (1 << GFBITS) * 4;
			for (int k = 0; k < 1 << GFBITS; k++)
			{
				array5[k] = Utils.Load4(array2, num2 + k * 4);
			}
			if (usePivots)
			{
				ulong[] pivots = new ulong[1];
				PKGen(null, array, array5, array6, pivots);
			}
			else
			{
				long[] array7 = new long[1 << GFBITS];
				for (int l = 0; l < 1 << GFBITS; l++)
				{
					array7[l] = (long)(((ulong)array5[l] << 31) | (uint)l);
				}
				Sort64(array7, 0, array7.Length);
				for (int m = 0; m < 1 << GFBITS; m++)
				{
					array6[m] = (ushort)(array7[m] & GFMASK);
				}
			}
			byte[] array8 = new byte[COND_BYTES];
			ControlBitsFromPermutation(array8, array6, GFBITS, 1 << GFBITS);
			Array.Copy(array8, 0, array, IRR_BYTES + 40, array8.Length);
		}
		Array.Copy(array2, 0, array, PrivateKeySize - SYS_N / 8, SYS_N / 8);
		return array;
	}

	public void KemKeypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		byte[] array = new byte[1];
		byte[] array2 = new byte[32];
		array[0] = 64;
		random.NextBytes(array2);
		byte[] array3 = new byte[SYS_N / 8 + (1 << GFBITS) * 4 + SYS_T * 2 + 32];
		int num = 0;
		byte[] sourceArray = array2;
		ulong[] array4 = new ulong[1];
		IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		ushort[] pi;
		int num2;
		while (true)
		{
			digest.BlockUpdate(array, 0, array.Length);
			digest.BlockUpdate(array2, 0, array2.Length);
			((IXof)digest).OutputFinal(array3, 0, array3.Length);
			num2 = array3.Length - 32;
			array2 = Arrays.CopyOfRange(array3, num2, num2 + 32);
			Array.Copy(sourceArray, 0, sk, 0, 32);
			sourceArray = Arrays.CopyOfRange(array2, 0, 32);
			ushort[] array5 = new ushort[SYS_T];
			int num3 = array3.Length - 32 - 2 * SYS_T;
			num2 = num3;
			for (int i = 0; i < SYS_T; i++)
			{
				array5[i] = Utils.LoadGF(array3, num3 + i * 2, GFMASK);
			}
			if (GenerateIrrPoly(array5) != -1)
			{
				num = 40;
				for (int j = 0; j < SYS_T; j++)
				{
					Utils.StoreGF(sk, num + j * 2, array5[j]);
				}
				uint[] array6 = new uint[1 << GFBITS];
				num2 -= (1 << GFBITS) * 4;
				for (int k = 0; k < 1 << GFBITS; k++)
				{
					array6[k] = Utils.Load4(array3, num2 + k * 4);
				}
				pi = new ushort[1 << GFBITS];
				if (PKGen(pk, sk, array6, pi, array4) != -1)
				{
					break;
				}
			}
		}
		byte[] array7 = new byte[COND_BYTES];
		ControlBitsFromPermutation(array7, pi, GFBITS, 1 << GFBITS);
		Array.Copy(array7, 0, sk, IRR_BYTES + 40, array7.Length);
		num2 -= SYS_N / 8;
		Array.Copy(array3, num2, sk, sk.Length - SYS_N / 8, SYS_N / 8);
		if (!usePivots)
		{
			Utils.Store8(sk, 32, 4294967295uL);
		}
		else
		{
			Utils.Store8(sk, 32, array4[0]);
		}
	}

	private void Syndrome(byte[] cipher_text, byte[] pk, byte[] error_vector)
	{
		short[] array = new short[SYS_N / 8];
		int num = 0;
		int num2 = PK_NROWS % 8;
		for (int i = 0; i < SYND_BYTES; i++)
		{
			cipher_text[i] = 0;
		}
		for (int i = 0; i < PK_NROWS; i++)
		{
			for (int j = 0; j < SYS_N / 8; j++)
			{
				array[j] = 0;
			}
			for (int j = 0; j < PK_ROW_BYTES; j++)
			{
				array[SYS_N / 8 - PK_ROW_BYTES + j] = pk[num + j];
			}
			if (usePadding)
			{
				for (int j = SYS_N / 8 - 1; j >= SYS_N / 8 - PK_ROW_BYTES; j--)
				{
					array[j] = (short)((((array[j] & 0xFF) << num2) | ((array[j - 1] & 0xFF) >> 8 - num2)) & 0xFF);
				}
			}
			array[i / 8] |= (short)(1 << i % 8);
			byte b = 0;
			for (int j = 0; j < SYS_N / 8; j++)
			{
				b ^= (byte)(array[j] & error_vector[j]);
			}
			b ^= (byte)(b >> 4);
			b ^= (byte)(b >> 2);
			b ^= (byte)(b >> 1);
			b &= 1;
			cipher_text[i / 8] |= (byte)(b << i % 8);
			num += PK_ROW_BYTES;
		}
	}

	private void GenerateErrorVector(byte[] error_vector, SecureRandom random)
	{
		ushort[] array = new ushort[SYS_T * 2];
		ushort[] array2 = new ushort[SYS_T];
		byte[] array3 = new byte[SYS_T];
		int num2;
		do
		{
			IL_0026:
			if (countErrorIndices)
			{
				byte[] array4 = new byte[SYS_T * 4];
				random.NextBytes(array4);
				for (int i = 0; i < SYS_T * 2; i++)
				{
					array[i] = Utils.LoadGF(array4, i * 2, GFMASK);
				}
				int num = 0;
				for (int j = 0; j < SYS_T * 2; j++)
				{
					if (num >= SYS_T)
					{
						break;
					}
					if (array[j] < SYS_N)
					{
						array2[num++] = array[j];
					}
				}
				if (num < SYS_T)
				{
					goto IL_0026;
				}
			}
			else
			{
				byte[] array4 = new byte[SYS_T * 2];
				random.NextBytes(array4);
				for (int k = 0; k < SYS_T; k++)
				{
					array2[k] = Utils.LoadGF(array4, k * 2, GFMASK);
				}
			}
			num2 = 0;
			for (int l = 1; l < SYS_T; l++)
			{
				if (num2 == 1)
				{
					break;
				}
				for (int m = 0; m < l; m++)
				{
					if (array2[l] == array2[m])
					{
						num2 = 1;
						break;
					}
				}
			}
		}
		while (num2 != 0);
		for (int n = 0; n < SYS_T; n++)
		{
			array3[n] = (byte)(1 << (array2[n] & 7));
		}
		for (short num3 = 0; num3 < SYS_N / 8; num3++)
		{
			error_vector[num3] = 0;
			for (int num4 = 0; num4 < SYS_T; num4++)
			{
				short num5 = SameMask32(num3, (short)(array2[num4] >> 3));
				num5 &= 0xFF;
				error_vector[num3] |= (byte)(array3[num4] & num5);
			}
		}
	}

	private void Encrypt(byte[] cipher_text, byte[] pk, byte[] error_vector, SecureRandom random)
	{
		GenerateErrorVector(error_vector, random);
		Syndrome(cipher_text, pk, error_vector);
	}

	public int KemEnc(byte[] cipher_text, byte[] key, byte[] pk, SecureRandom random)
	{
		byte[] array = new byte[SYS_N / 8];
		int num = 0;
		if (usePadding)
		{
			num = CheckPKPadding(pk);
		}
		Encrypt(cipher_text, pk, array, random);
		IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		digest.Update(1);
		digest.BlockUpdate(array, 0, array.Length);
		digest.BlockUpdate(cipher_text, 0, cipher_text.Length);
		((IXof)digest).OutputFinal(key, 0, key.Length);
		if (usePadding)
		{
			byte b = (byte)num;
			b ^= 0xFF;
			for (int i = 0; i < SYND_BYTES; i++)
			{
				cipher_text[i] &= b;
			}
			for (int i = 0; i < 32; i++)
			{
				key[i] &= b;
			}
			return num;
		}
		return 0;
	}

	public int KemDec(byte[] key, byte[] cipher_text, byte[] sk)
	{
		byte[] array = new byte[SYS_N / 8];
		byte[] array2 = new byte[1 + SYS_N / 8 + SYND_BYTES];
		int num = 0;
		if (usePadding)
		{
			num = CheckCPadding(cipher_text);
		}
		short num2 = (byte)Decrypt(array, sk, cipher_text);
		num2--;
		num2 >>= 8;
		num2 &= 0xFF;
		array2[0] = (byte)(num2 & 1);
		for (int i = 0; i < SYS_N / 8; i++)
		{
			array2[1 + i] = (byte)((~num2 & sk[i + 40 + IRR_BYTES + COND_BYTES]) | (num2 & array[i]));
		}
		for (int i = 0; i < SYND_BYTES; i++)
		{
			array2[1 + SYS_N / 8 + i] = cipher_text[i];
		}
		DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		IDigest digest = DigestUtilities.GetDigest(NistObjectIdentifiers.IdShake256);
		digest.BlockUpdate(array2, 0, array2.Length);
		((IXof)digest).OutputFinal(key, 0, key.Length);
		if (usePadding)
		{
			byte b = (byte)num;
			for (int i = 0; i < key.Length; i++)
			{
				key[i] |= b;
			}
			return num;
		}
		return 0;
	}

	private int Decrypt(byte[] error_vector, byte[] sk, byte[] cipher_text)
	{
		ushort[] array = new ushort[SYS_T + 1];
		ushort[] array2 = new ushort[SYS_N];
		ushort[] array3 = new ushort[SYS_T * 2];
		ushort[] array4 = new ushort[SYS_T * 2];
		ushort[] array5 = new ushort[SYS_T + 1];
		ushort[] array6 = new ushort[SYS_N];
		byte[] array7 = new byte[SYS_N / 8];
		for (int i = 0; i < SYND_BYTES; i++)
		{
			array7[i] = cipher_text[i];
		}
		for (int j = SYND_BYTES; j < SYS_N / 8; j++)
		{
			array7[j] = 0;
		}
		for (int k = 0; k < SYS_T; k++)
		{
			array[k] = Utils.LoadGF(sk, 40 + k * 2, GFMASK);
		}
		array[SYS_T] = 1;
		benes.SupportGen(array2, sk);
		Synd(array3, array, array2, array7);
		BM(array5, array3);
		Root(array6, array5, array2);
		for (int l = 0; l < SYS_N / 8; l++)
		{
			error_vector[l] = 0;
		}
		int num = 0;
		for (int m = 0; m < SYS_N; m++)
		{
			ushort num2 = (ushort)(gf.GFIsZero(array6[m]) & 1);
			error_vector[m / 8] |= (byte)(num2 << m % 8);
			num += num2;
		}
		Synd(array4, array, array2, error_vector);
		int num3 = num;
		num3 ^= SYS_T;
		for (int n = 0; n < SYS_T * 2; n++)
		{
			num3 |= array3[n] ^ array4[n];
		}
		num3--;
		num3 >>= 15;
		num3 &= 1;
		_ = num3 ^ 1;
		return num3 ^ 1;
	}

	private static int Min(ushort a, int b)
	{
		if (a < b)
		{
			return a;
		}
		return b;
	}

	private void BM(ushort[] output, ushort[] s)
	{
		ushort num = 0;
		ushort num2 = 0;
		ushort[] array = new ushort[SYS_T + 1];
		ushort[] array2 = new ushort[SYS_T + 1];
		ushort[] array3 = new ushort[SYS_T + 1];
		ushort num3 = 1;
		for (int i = 0; i < SYS_T + 1; i++)
		{
			array2[i] = (array3[i] = 0);
		}
		array3[1] = (array2[0] = 1);
		for (num = 0; num < 2 * SYS_T; num++)
		{
			uint num4 = 0u;
			for (int j = 0; j <= Min(num, SYS_T); j++)
			{
				num4 ^= gf.GFMulExt(array2[j], s[num - j]);
			}
			ushort num5 = gf.GFReduce(num4);
			ushort num6 = num5;
			num6--;
			num6 >>= 15;
			num6 &= 1;
			num6--;
			ushort num7 = num;
			num7 -= (ushort)(2 * num2);
			num7 >>= 15;
			num7 &= 1;
			num7--;
			num7 &= num6;
			for (int k = 0; k <= SYS_T; k++)
			{
				array[k] = array2[k];
			}
			ushort left = gf.GFFrac(num3, num5);
			for (int l = 0; l <= SYS_T; l++)
			{
				array2[l] ^= (ushort)(gf.GFMul(left, array3[l]) & num6);
			}
			num2 = (ushort)((num2 & ~num7) | ((num + 1 - num2) & num7));
			for (int num8 = SYS_T - 1; num8 >= 0; num8--)
			{
				array3[num8 + 1] = (ushort)((array3[num8] & ~num7) | (array[num8] & num7));
			}
			array3[0] = 0;
			num3 = (ushort)((num3 & ~num7) | (num5 & num7));
		}
		for (int m = 0; m <= SYS_T; m++)
		{
			output[m] = array2[SYS_T - m];
		}
	}

	private void Synd(ushort[] output, ushort[] f, ushort[] L, byte[] r)
	{
		ushort num = (ushort)(r[0] & 1);
		ushort num2 = L[0];
		ushort input = Eval(f, num2);
		ushort left = (output[0] = (ushort)(gf.GFInv(gf.GFSq(input)) & -num));
		for (int i = 1; i < 2 * SYS_T; i++)
		{
			left = (output[i] = gf.GFMul(left, num2));
		}
		for (int j = 1; j < SYS_N; j++)
		{
			ushort right = (ushort)((r[j / 8] >> j % 8) & 1);
			ushort num3 = L[j];
			ushort input2 = Eval(f, num3);
			ushort left2 = gf.GFInv(gf.GFSq(input2));
			ushort num4 = gf.GFMul(left2, right);
			output[0] ^= num4;
			for (int k = 1; k < 2 * SYS_T; k++)
			{
				num4 = gf.GFMul(num4, num3);
				output[k] ^= num4;
			}
		}
	}

	private int MovColumns(byte[][] mat, ushort[] pi, ulong[] pivots)
	{
		ulong[] array = new ulong[64];
		int[] array2 = new int[32];
		ulong num = 1uL;
		byte[] array3 = new byte[9];
		int num2 = PK_NROWS - 32;
		int num3 = num2 / 8;
		int num4 = num2 % 8;
		if (usePadding)
		{
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					array3[j] = mat[num2 + i][num3 + j];
				}
				for (int j = 0; j < 8; j++)
				{
					array3[j] = (byte)(((array3[j] & 0xFF) >> num4) | (array3[j + 1] << 8 - num4));
				}
				array[i] = Utils.Load8(array3, 0);
			}
		}
		else
		{
			for (int i = 0; i < 32; i++)
			{
				array[i] = Utils.Load8(mat[num2 + i], num3);
			}
		}
		pivots[0] = 0uL;
		for (int i = 0; i < 32; i++)
		{
			ulong num5 = array[i];
			for (int j = i + 1; j < 32; j++)
			{
				num5 |= array[j];
			}
			if (num5 == 0L)
			{
				return -1;
			}
			int num6 = (array2[i] = Ctz(num5));
			pivots[0] |= num << num6;
			for (int j = i + 1; j < 32; j++)
			{
				ulong num7 = (array[i] >> num6) & 1;
				num7--;
				array[i] ^= array[j] & num7;
			}
			for (int j = i + 1; j < 32; j++)
			{
				ulong num7 = (array[j] >> num6) & 1;
				num7 = 0 - num7;
				array[j] ^= array[i] & num7;
			}
		}
		for (int j = 0; j < 32; j++)
		{
			for (int k = j + 1; k < 64; k++)
			{
				ulong num8 = (ulong)(pi[num2 + j] ^ pi[num2 + k]);
				num8 &= SameMask64((ushort)k, (ushort)array2[j]);
				pi[num2 + j] ^= (ushort)num8;
				pi[num2 + k] ^= (ushort)num8;
			}
		}
		for (int i = 0; i < PK_NROWS; i++)
		{
			ulong num5;
			if (usePadding)
			{
				for (int k = 0; k < 9; k++)
				{
					array3[k] = mat[i][num3 + k];
				}
				for (int k = 0; k < 8; k++)
				{
					array3[k] = (byte)(((array3[k] & 0xFF) >> num4) | (array3[k + 1] << 8 - num4));
				}
				num5 = Utils.Load8(array3, 0);
			}
			else
			{
				num5 = Utils.Load8(mat[i], num3);
			}
			for (int j = 0; j < 32; j++)
			{
				ulong num8 = num5 >> j;
				num8 ^= num5 >> array2[j];
				num8 &= 1;
				num5 ^= num8 << array2[j];
				num5 ^= num8 << j;
			}
			if (usePadding)
			{
				Utils.Store8(array3, 0, num5);
				mat[i][num3 + 8] = (byte)(((mat[i][num3 + 8] & 0xFF) >> num4 << num4) | ((array3[7] & 0xFF) >> 8 - num4));
				mat[i][num3] = (byte)(((array3[0] & 0xFF) << num4) | ((mat[i][num3] & 0xFF) << 8 - num4 >> 8 - num4));
				for (int k = 7; k >= 1; k--)
				{
					mat[i][num3 + k] = (byte)(((array3[k] & 0xFF) << num4) | ((array3[k - 1] & 0xFF) >> 8 - num4));
				}
			}
			else
			{
				Utils.Store8(mat[i], num3, num5);
			}
		}
		return 0;
	}

	private static int Ctz(ulong input)
	{
		ulong num = 72340172838076673uL;
		ulong num2 = 0uL;
		ulong num3 = ~input;
		for (int i = 0; i < 8; i++)
		{
			num &= num3 >> i;
			num2 += num;
		}
		ulong num4 = num2 & 0x808080808080808L;
		num4 |= num4 >> 1;
		num4 |= num4 >> 2;
		ulong num5 = num2;
		num2 >>= 8;
		num5 += num2 & num4;
		for (int j = 2; j < 8; j++)
		{
			num4 &= num4 >> 8;
			num2 >>= 8;
			num5 += num2 & num4;
		}
		return (int)num5 & 0xFF;
	}

	private static ulong SameMask64(ushort x, ushort y)
	{
		return (ulong)((long)(x ^ y) - 1L >> 63);
	}

	private static byte SameMask32(short x, short y)
	{
		return (byte)((x ^ y) - 1 >> 31);
	}

	private static void Layer(ushort[] p, byte[] output, int ptrIndex, int s, int n)
	{
		int num = 1 << s;
		int num2 = 0;
		for (int i = 0; i < n; i += num * 2)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = p[i + j] ^ p[i + j + num];
				int num4 = (output[ptrIndex + (num2 >> 3)] >> (num2 & 7)) & 1;
				num4 = -num4;
				num3 &= num4;
				p[i + j] ^= (ushort)num3;
				p[i + j + num] ^= (ushort)num3;
				num2++;
			}
		}
	}

	private static void ControlBitsFromPermutation(byte[] output, ushort[] pi, long w, long n)
	{
		int[] temp = new int[(int)(2 * n)];
		ushort[] array = new ushort[(int)n];
		ushort num2;
		do
		{
			for (int i = 0; i < ((2 * w - 1) * n / 2 + 7) / 8; i++)
			{
				output[i] = 0;
			}
			CBRecursion(output, 0L, 1L, pi, 0, w, n, temp);
			for (int i = 0; i < n; i++)
			{
				array[i] = (ushort)i;
			}
			int num = 0;
			for (int i = 0; i < w; i++)
			{
				Layer(array, output, num, i, (int)n);
				num += (int)n >> 4;
			}
			for (int i = (int)(w - 2); i >= 0; i--)
			{
				Layer(array, output, num, i, (int)n);
				num += (int)n >> 4;
			}
			num2 = 0;
			for (int i = 0; i < n; i++)
			{
				num2 |= (ushort)(pi[i] ^ array[i]);
			}
		}
		while (num2 != 0);
	}

	private static short GetQShort(int[] temp, int q_index)
	{
		int num = q_index / 2;
		if (q_index % 2 == 0)
		{
			return (short)temp[num];
		}
		return (short)((temp[num] & 0xFFFF0000u) >> 16);
	}

	private static void CBRecursion(byte[] output, long pos, long step, ushort[] pi, int qIndex, long w, long n, int[] temp)
	{
		if (w == 1)
		{
			output[(int)(pos >> 3)] ^= (byte)(GetQShort(temp, qIndex) << (int)(pos & 7));
			return;
		}
		if (pi != null)
		{
			for (long num = 0L; num < n; num++)
			{
				temp[(int)num] = ((pi[(int)num] ^ 1) << 16) | pi[(int)(num ^ 1)];
			}
		}
		else
		{
			for (long num = 0L; num < n; num++)
			{
				ushort num2 = (ushort)GetQShort(temp, (int)(qIndex + num));
				ushort num3 = (ushort)GetQShort(temp, (int)(qIndex + (num ^ 1)));
				temp[(int)num] = ((num2 ^ 1) << 16) | num3;
			}
		}
		Sort32(temp, 0, (int)n);
		for (long num = 0L; num < n; num++)
		{
			int num4 = temp[(int)num] & 0xFFFF;
			int num5 = num4;
			if (num < num5)
			{
				num5 = (int)num;
			}
			temp[(int)(n + num)] = (num4 << 16) | num5;
		}
		for (long num = 0L; num < n; num++)
		{
			temp[(int)num] = (int)((uint)(temp[(int)num] << 16) | num);
		}
		Sort32(temp, 0, (int)n);
		for (long num = 0L; num < n; num++)
		{
			temp[(int)num] = (temp[(int)num] << 16) + (temp[(int)(n + num)] >> 16);
		}
		Sort32(temp, 0, (int)n);
		if (w <= 10)
		{
			for (long num = 0L; num < n; num++)
			{
				temp[(int)(n + num)] = ((temp[(int)num] & 0xFFFF) << 10) | (temp[(int)(n + num)] & 0x3FF);
			}
			for (long num6 = 1L; num6 < w - 1; num6++)
			{
				for (long num = 0L; num < n; num++)
				{
					temp[(int)num] = (int)((uint)((temp[(int)(n + num)] & -1024) << 6) | num);
				}
				Sort32(temp, 0, (int)n);
				for (long num = 0L; num < n; num++)
				{
					temp[(int)num] = (temp[(int)num] << 20) | temp[(int)(n + num)];
				}
				Sort32(temp, 0, (int)n);
				for (long num = 0L; num < n; num++)
				{
					int num7 = temp[(int)num] & 0xFFFFF;
					int num8 = (temp[(int)num] & 0xFFC00) | (temp[(int)(n + num)] & 0x3FF);
					if (num7 < num8)
					{
						num8 = num7;
					}
					temp[(int)(n + num)] = num8;
				}
			}
			for (long num = 0L; num < n; num++)
			{
				temp[(int)(n + num)] &= 1023;
			}
		}
		else
		{
			for (long num = 0L; num < n; num++)
			{
				temp[(int)(n + num)] = (temp[(int)num] << 16) | (temp[(int)(n + num)] & 0xFFFF);
			}
			for (long num6 = 1L; num6 < w - 1; num6++)
			{
				for (long num = 0L; num < n; num++)
				{
					temp[(int)num] = (int)((uint)(temp[(int)(n + num)] & -65536) | num);
				}
				Sort32(temp, 0, (int)n);
				for (long num = 0L; num < n; num++)
				{
					temp[(int)num] = (temp[(int)num] << 16) | (temp[(int)(n + num)] & 0xFFFF);
				}
				if (num6 < w - 2)
				{
					for (long num = 0L; num < n; num++)
					{
						temp[(int)(n + num)] = (temp[(int)num] & -65536) | (temp[(int)(n + num)] >> 16);
					}
					Sort32(temp, (int)n, (int)(n * 2));
					for (long num = 0L; num < n; num++)
					{
						temp[(int)(n + num)] = (temp[(int)(n + num)] << 16) | (temp[(int)num] & 0xFFFF);
					}
				}
				Sort32(temp, 0, (int)n);
				for (long num = 0L; num < n; num++)
				{
					int num9 = (temp[(int)(n + num)] & -65536) | (temp[(int)num] & 0xFFFF);
					if (num9 < temp[(int)(n + num)])
					{
						temp[(int)(n + num)] = num9;
					}
				}
			}
			for (long num = 0L; num < n; num++)
			{
				temp[(int)(n + num)] &= 65535;
			}
		}
		if (pi != null)
		{
			for (long num = 0L; num < n; num++)
			{
				temp[(int)num] = (int)((pi[(int)num] << 16) + num);
			}
		}
		else
		{
			for (long num = 0L; num < n; num++)
			{
				temp[(int)num] = (int)((GetQShort(temp, (int)(qIndex + num)) << 16) + num);
			}
		}
		Sort32(temp, 0, (int)n);
		for (long num10 = 0L; num10 < n / 2; num10++)
		{
			long num11 = 2 * num10;
			int num12 = temp[(int)(n + num11)] & 1;
			int num13 = (int)(num11 + num12);
			int num14 = num13 ^ 1;
			output[(int)(pos >> 3)] ^= (byte)(num12 << (int)(pos & 7));
			pos += step;
			temp[(int)(n + num11)] = (temp[(int)num11] << 16) | num13;
			temp[(int)(n + num11 + 1)] = (temp[(int)(num11 + 1)] << 16) | num14;
		}
		Sort32(temp, (int)n, (int)(n * 2));
		pos += (2 * w - 3) * step * (n / 2);
		for (long num15 = 0L; num15 < n / 2; num15++)
		{
			long num16 = 2 * num15;
			int num17 = temp[(int)(n + num16)] & 1;
			int num18 = (int)(num16 + num17);
			int num19 = num18 ^ 1;
			output[(int)(pos >> 3)] ^= (byte)(num17 << (int)(pos & 7));
			pos += step;
			temp[(int)num16] = (num18 << 16) | (temp[(int)(n + num16)] & 0xFFFF);
			temp[(int)(num16 + 1)] = (num19 << 16) | (temp[(int)(n + num16 + 1)] & 0xFFFF);
		}
		Sort32(temp, 0, (int)n);
		pos -= (2 * w - 2) * step * (n / 2);
		short[] array = new short[(int)n * 4];
		for (long num6 = 0L; num6 < n * 2; num6++)
		{
			array[(int)(num6 * 2)] = (short)temp[(int)num6];
			array[(int)(num6 * 2 + 1)] = (short)((temp[(int)num6] & 0xFFFF0000u) >> 16);
		}
		for (long num10 = 0L; num10 < n / 2; num10++)
		{
			array[(int)num10] = (short)((temp[(int)(2 * num10)] & 0xFFFF) >> 1);
			array[(int)(num10 + n / 2)] = (short)((temp[(int)(2 * num10 + 1)] & 0xFFFF) >> 1);
		}
		for (long num6 = 0L; num6 < n / 2; num6++)
		{
			temp[(int)(n + n / 4 + num6)] = (array[(int)(num6 * 2 + 1)] << 16) | array[(int)(num6 * 2)];
		}
		CBRecursion(output, pos, step * 2, null, (int)(n + n / 4) * 2, w - 1, n / 2, temp);
		CBRecursion(output, pos + step, step * 2, null, (int)((n + n / 4) * 2 + n / 2), w - 1, n / 2, temp);
	}

	private int PKGen(byte[] pk, byte[] sk, uint[] perm, ushort[] pi, ulong[] pivots)
	{
		ushort[] array = new ushort[SYS_T + 1];
		array[SYS_T] = 1;
		for (int i = 0; i < SYS_T; i++)
		{
			array[i] = Utils.LoadGF(sk, 40 + i * 2, GFMASK);
		}
		long[] array2 = new long[1 << GFBITS];
		for (int i = 0; i < 1 << GFBITS; i++)
		{
			array2[i] = (long)(((ulong)perm[i] << 31) | (uint)i);
		}
		Sort64(array2, 0, array2.Length);
		for (int i = 1; i < 1 << GFBITS; i++)
		{
			if (array2[i - 1] >> 31 == array2[i] >> 31)
			{
				return -1;
			}
		}
		ushort[] array3 = new ushort[SYS_N];
		for (int i = 0; i < 1 << GFBITS; i++)
		{
			pi[i] = (ushort)(array2[i] & GFMASK);
		}
		for (int i = 0; i < SYS_N; i++)
		{
			array3[i] = Utils.Bitrev(pi[i], GFBITS);
		}
		ushort[] array4 = new ushort[SYS_N];
		Root(array4, array, array3);
		for (int i = 0; i < SYS_N; i++)
		{
			array4[i] = gf.GFInv(array4[i]);
		}
		byte[][] array5 = new byte[PK_NROWS][];
		for (int i = 0; i < PK_NROWS; i++)
		{
			array5[i] = new byte[SYS_N / 8];
		}
		for (int i = 0; i < SYS_T; i++)
		{
			for (int j = 0; j < SYS_N; j += 8)
			{
				ulong lo = array4[j] | ((ulong)array4[j + 2] << 16) | ((ulong)array4[j + 4] << 32) | ((ulong)array4[j + 6] << 48);
				ulong hi = array4[j + 1] | ((ulong)array4[j + 3] << 16) | ((ulong)array4[j + 5] << 32) | ((ulong)array4[j + 7] << 48);
				Bits.BitPermuteStep2(ref hi, ref lo, 71777214294589695uL, 8);
				lo = Interleave.Transpose(lo);
				hi = Interleave.Transpose(hi);
				array5[i * GFBITS][j / 8] = (byte)lo;
				array5[i * GFBITS + 1][j / 8] = (byte)(lo >> 8);
				array5[i * GFBITS + 2][j / 8] = (byte)(lo >> 16);
				array5[i * GFBITS + 3][j / 8] = (byte)(lo >> 24);
				array5[i * GFBITS + 4][j / 8] = (byte)(lo >> 32);
				array5[i * GFBITS + 5][j / 8] = (byte)(lo >> 40);
				array5[i * GFBITS + 6][j / 8] = (byte)(lo >> 48);
				array5[i * GFBITS + 7][j / 8] = (byte)(lo >> 56);
				array5[i * GFBITS + 8][j / 8] = (byte)hi;
				array5[i * GFBITS + 9][j / 8] = (byte)(hi >> 8);
				array5[i * GFBITS + 10][j / 8] = (byte)(hi >> 16);
				array5[i * GFBITS + 11][j / 8] = (byte)(hi >> 24);
				if (GFBITS > 12)
				{
					array5[i * GFBITS + 12][j / 8] = (byte)(hi >> 32);
				}
			}
			for (int j = 0; j < SYS_N; j++)
			{
				array4[j] = gf.GFMul(array4[j], array3[j]);
			}
		}
		for (int k = 0; k < PK_NROWS; k++)
		{
			int i = k >> 3;
			int j = k & 7;
			if (usePivots && k == PK_NROWS - 32 && MovColumns(array5, pi, pivots) != 0)
			{
				return -1;
			}
			byte[] array6 = array5[k];
			for (int l = k + 1; l < PK_NROWS; l++)
			{
				byte[] array7 = array5[l];
				byte b = (byte)(array6[i] ^ array7[i]);
				b = (byte)(b >> j);
				b &= 1;
				int m = 0;
				byte b2 = (byte)(-b);
				for (int num = SYS_N / 8 - 4; m <= num; m += 4)
				{
					array6[m] ^= (byte)(array7[m] & b2);
					array6[m + 1] ^= (byte)(array7[m + 1] & b2);
					array6[m + 2] ^= (byte)(array7[m + 2] & b2);
					array6[m + 3] ^= (byte)(array7[m + 3] & b2);
				}
				byte b3 = (byte)(-b);
				for (; m < SYS_N / 8; m++)
				{
					array6[m] ^= (byte)(array7[m] & b3);
				}
			}
			if (((array6[i] >> j) & 1) == 0)
			{
				return -1;
			}
			for (int l = 0; l < PK_NROWS; l++)
			{
				if (l != k)
				{
					byte[] array8 = array5[l];
					byte b4 = (byte)(array8[i] >> j);
					b4 &= 1;
					int n = 0;
					byte b5 = (byte)(-b4);
					for (int num2 = SYS_N / 8 - 4; n <= num2; n += 4)
					{
						array8[n] ^= (byte)(array6[n] & b5);
						array8[n + 1] ^= (byte)(array6[n + 1] & b5);
						array8[n + 2] ^= (byte)(array6[n + 2] & b5);
						array8[n + 3] ^= (byte)(array6[n + 3] & b5);
					}
					byte b6 = (byte)(-b4);
					for (; n < SYS_N / 8; n++)
					{
						array8[n] ^= (byte)(array6[n] & b6);
					}
				}
			}
		}
		if (pk != null)
		{
			if (usePadding)
			{
				int num3 = (PK_NROWS - 1) / 8;
				int num4 = SYS_N / 8;
				int num5 = 0;
				int num6 = PK_NROWS % 8;
				if (num6 == 0)
				{
					int num7 = num4 - num3;
					for (int i = 0; i < PK_NROWS; i++)
					{
						Array.Copy(array5[i], num3, pk, num5, num7);
						num5 += num7;
					}
				}
				else
				{
					for (int i = 0; i < PK_NROWS; i++)
					{
						byte[] bs = array5[i];
						ulong num8 = Pack.LE_To_UInt64(bs, num3);
						int j;
						for (j = num3 + 8; j < num4 - 8; j += 8)
						{
							ulong num9 = Pack.LE_To_UInt64(bs, j);
							Pack.UInt64_To_LE((num8 >> num6) | (num9 << 64 - num6), pk, num5);
							num5 += 8;
							num8 = num9;
						}
						int num10 = num4 - j;
						ulong num11 = Pack.LE_To_UInt64_Low(bs, j, num10);
						Pack.UInt64_To_LE((num8 >> num6) | (num11 << 64 - num6), pk, num5);
						num5 += 8;
						Pack.UInt64_To_LE_Low(num11 >> num6, pk, num5, num10);
						num5 += num10;
					}
				}
			}
			else
			{
				int num12 = (SYS_N - PK_NROWS + 7) / 8;
				for (int i = 0; i < PK_NROWS; i++)
				{
					Array.Copy(array5[i], PK_NROWS / 8, pk, num12 * i, num12);
				}
			}
		}
		return 0;
	}

	private ushort Eval(ushort[] f, ushort a)
	{
		ushort num = f[SYS_T];
		for (int num2 = SYS_T - 1; num2 >= 0; num2--)
		{
			num = gf.GFMul(num, a);
			num ^= f[num2];
		}
		return num;
	}

	private void Root(ushort[] output, ushort[] f, ushort[] L)
	{
		for (int i = 0; i < SYS_N; i++)
		{
			output[i] = Eval(f, L[i]);
		}
	}

	private int GenerateIrrPoly(ushort[] field)
	{
		ushort[][] array = new ushort[SYS_T + 1][];
		array[0] = new ushort[SYS_T];
		array[0][0] = 1;
		array[1] = new ushort[SYS_T];
		Array.Copy(field, 0, array[1], 0, SYS_T);
		uint[] temp = new uint[SYS_T * 2 - 1];
		int i;
		for (i = 2; i < SYS_T; i += 2)
		{
			array[i] = new ushort[SYS_T];
			gf.GFSqrPoly(SYS_T, poly, array[i], array[i >> 1], temp);
			array[i + 1] = new ushort[SYS_T];
			gf.GFMulPoly(SYS_T, poly, array[i + 1], array[i], field, temp);
		}
		if (i == SYS_T)
		{
			array[i] = new ushort[SYS_T];
			gf.GFSqrPoly(SYS_T, poly, array[i], array[i >> 1], temp);
		}
		for (int j = 0; j < SYS_T; j++)
		{
			for (int k = j + 1; k < SYS_T; k++)
			{
				ushort num = gf.GFIsZero(array[j][j]);
				for (int l = j; l < SYS_T + 1; l++)
				{
					array[l][j] ^= (ushort)(array[l][k] & num);
				}
			}
			if (array[j][j] == 0)
			{
				return -1;
			}
			ushort right = gf.GFInv(array[j][j]);
			for (int m = j; m < SYS_T + 1; m++)
			{
				array[m][j] = gf.GFMul(array[m][j], right);
			}
			for (int n = 0; n < SYS_T; n++)
			{
				if (n != j)
				{
					ushort right2 = array[j][n];
					for (int num2 = j; num2 <= SYS_T; num2++)
					{
						array[num2][n] ^= gf.GFMul(array[num2][j], right2);
					}
				}
			}
		}
		Array.Copy(array[SYS_T], field, SYS_T);
		return 0;
	}

	private int CheckPKPadding(byte[] pk)
	{
		byte b = 0;
		for (int i = 0; i < PK_NROWS; i++)
		{
			b |= pk[i * PK_ROW_BYTES + PK_ROW_BYTES - 1];
		}
		b = (byte)((b & 0xFF) >> PK_NCOLS % 8);
		b--;
		b = (byte)((b & 0xFF) >> 7);
		return b - 1;
	}

	private int CheckCPadding(byte[] c)
	{
		return (byte)(((byte)((byte)((c[SYND_BYTES - 1] & 0xFF) >> PK_NROWS % 8) - 1) & 0xFF) >> 7) - 1;
	}

	private static void Sort32(int[] temp, int from, int to)
	{
		int num = to - from;
		if (num < 2)
		{
			return;
		}
		int i;
		for (i = 1; i < num - i; i += i)
		{
		}
		for (int num2 = i; num2 > 0; num2 >>= 1)
		{
			for (int j = 0; j < num - num2; j++)
			{
				if ((j & num2) == 0)
				{
					int num3 = temp[from + j + num2] ^ temp[from + j];
					int num4 = temp[from + j + num2] - temp[from + j];
					num4 ^= num3 & (num4 ^ temp[from + j + num2]);
					num4 >>= 31;
					num4 &= num3;
					temp[from + j] ^= num4;
					temp[from + j + num2] ^= num4;
				}
			}
			for (int num5 = i; num5 > num2; num5 >>= 1)
			{
				for (int k = 0; k < num - num5; k++)
				{
					if ((k & num2) == 0)
					{
						int num6 = temp[from + k + num2];
						for (int num7 = num5; num7 > num2; num7 >>= 1)
						{
							int num8 = temp[from + k + num7] ^ num6;
							int num9 = temp[from + k + num7] - num6;
							num9 ^= num8 & (num9 ^ temp[from + k + num7]);
							num9 >>= 31;
							num9 &= num8;
							num6 ^= num9;
							temp[from + k + num7] ^= num9;
						}
						temp[from + k + num2] = num6;
					}
				}
			}
		}
	}

	private static void Sort64(long[] temp, int from, int to)
	{
		int num = to - from;
		if (num < 2)
		{
			return;
		}
		int i;
		for (i = 1; i < num - i; i += i)
		{
		}
		for (int num2 = i; num2 > 0; num2 >>= 1)
		{
			for (int j = 0; j < num - num2; j++)
			{
				if ((j & num2) == 0)
				{
					long num3 = temp[from + j + num2] - temp[from + j];
					num3 >>= 63;
					num3 &= temp[from + j] ^ temp[from + j + num2];
					temp[from + j] ^= num3;
					temp[from + j + num2] ^= num3;
				}
			}
			for (int num4 = i; num4 > num2; num4 >>= 1)
			{
				for (int k = 0; k < num - num4; k++)
				{
					if ((k & num2) == 0)
					{
						long num5 = temp[from + k + num2];
						for (int num6 = num4; num6 > num2; num6 >>= 1)
						{
							long num7 = temp[from + k + num6] - num5;
							num7 >>= 63;
							num7 &= num5 ^ temp[from + k + num6];
							num5 ^= num7;
							temp[from + k + num6] ^= num7;
						}
						temp[from + k + num2] = num5;
					}
				}
			}
		}
	}
}
