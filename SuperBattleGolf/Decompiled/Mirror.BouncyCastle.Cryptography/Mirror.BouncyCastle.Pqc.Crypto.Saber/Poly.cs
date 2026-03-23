namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

internal class Poly
{
	private const int KARATSUBA_N = 64;

	private readonly int N_SB;

	private readonly int N_SB_RES;

	private readonly int SABER_N;

	private readonly int SABER_L;

	private readonly SaberEngine engine;

	private readonly SaberUtilities utils;

	public Poly(SaberEngine engine)
	{
		this.engine = engine;
		SABER_L = engine.L;
		SABER_N = engine.N;
		N_SB = SABER_N >> 2;
		N_SB_RES = 2 * N_SB - 1;
		utils = engine.Utilities;
	}

	public void GenMatrix(short[][][] A, byte[] seed)
	{
		byte[] array = new byte[SABER_L * engine.PolyVecBytes];
		engine.Symmetric.Prf(array, seed, engine.SeedBytes, array.Length);
		for (int i = 0; i < SABER_L; i++)
		{
			utils.BS2POLVECq(array, i * engine.PolyVecBytes, A[i]);
		}
	}

	public void GenSecret(short[][] s, byte[] seed)
	{
		byte[] array = new byte[SABER_L * engine.PolyCoinBytes];
		engine.Symmetric.Prf(array, seed, engine.NoiseSeedBytes, array.Length);
		for (int i = 0; i < SABER_L; i++)
		{
			if (!engine.UsingEffectiveMasking)
			{
				Cbd(s[i], array, i * engine.PolyCoinBytes);
				continue;
			}
			for (int j = 0; j < SABER_N / 4; j++)
			{
				s[i][4 * j] = (short)(((array[j + i * engine.PolyCoinBytes] & 3) ^ 2) - 2);
				s[i][4 * j + 1] = (short)((((array[j + i * engine.PolyCoinBytes] >> 2) & 3) ^ 2) - 2);
				s[i][4 * j + 2] = (short)((((array[j + i * engine.PolyCoinBytes] >> 4) & 3) ^ 2) - 2);
				s[i][4 * j + 3] = (short)((((array[j + i * engine.PolyCoinBytes] >> 6) & 3) ^ 2) - 2);
			}
		}
	}

	private long LoadLittleEndian(byte[] x, int offset, int bytes)
	{
		long num = x[offset] & 0xFF;
		for (int i = 1; i < bytes; i++)
		{
			num |= (long)(x[offset + i] & 0xFF) << 8 * i;
		}
		return num;
	}

	private void Cbd(short[] s, byte[] buf, int offset)
	{
		int[] array = new int[4];
		int[] array2 = new int[4];
		if (engine.MU == 6)
		{
			for (int i = 0; i < SABER_N / 4; i++)
			{
				int num = (int)LoadLittleEndian(buf, offset + 3 * i, 3);
				int num2 = 0;
				for (int j = 0; j < 3; j++)
				{
					num2 += (num >> j) & 0x249249;
				}
				array[0] = num2 & 7;
				array2[0] = (num2 >> 3) & 7;
				array[1] = (num2 >> 6) & 7;
				array2[1] = (num2 >> 9) & 7;
				array[2] = (num2 >> 12) & 7;
				array2[2] = (num2 >> 15) & 7;
				array[3] = (num2 >> 18) & 7;
				array2[3] = num2 >> 21;
				s[4 * i] = (short)(array[0] - array2[0]);
				s[4 * i + 1] = (short)(array[1] - array2[1]);
				s[4 * i + 2] = (short)(array[2] - array2[2]);
				s[4 * i + 3] = (short)(array[3] - array2[3]);
			}
		}
		else if (engine.MU == 8)
		{
			for (int i = 0; i < SABER_N / 4; i++)
			{
				int num3 = (int)LoadLittleEndian(buf, offset + 4 * i, 4);
				int num4 = 0;
				for (int j = 0; j < 4; j++)
				{
					num4 += (num3 >> j) & 0x11111111;
				}
				array[0] = num4 & 0xF;
				array2[0] = (num4 >> 4) & 0xF;
				array[1] = (num4 >> 8) & 0xF;
				array2[1] = (num4 >> 12) & 0xF;
				array[2] = (num4 >> 16) & 0xF;
				array2[2] = (num4 >> 20) & 0xF;
				array[3] = (num4 >> 24) & 0xF;
				array2[3] = num4 >> 28;
				s[4 * i] = (short)(array[0] - array2[0]);
				s[4 * i + 1] = (short)(array[1] - array2[1]);
				s[4 * i + 2] = (short)(array[2] - array2[2]);
				s[4 * i + 3] = (short)(array[3] - array2[3]);
			}
		}
		else
		{
			if (engine.MU != 10)
			{
				return;
			}
			for (int i = 0; i < SABER_N / 4; i++)
			{
				long num5 = LoadLittleEndian(buf, offset + 5 * i, 5);
				long num6 = 0L;
				for (int j = 0; j < 5; j++)
				{
					num6 += (num5 >> j) & 0x842108421L;
				}
				array[0] = (int)(num6 & 0x1F);
				array2[0] = (int)((num6 >> 5) & 0x1F);
				array[1] = (int)((num6 >> 10) & 0x1F);
				array2[1] = (int)((num6 >> 15) & 0x1F);
				array[2] = (int)((num6 >> 20) & 0x1F);
				array2[2] = (int)((num6 >> 25) & 0x1F);
				array[3] = (int)((num6 >> 30) & 0x1F);
				array2[3] = (int)(num6 >> 35);
				s[4 * i] = (short)(array[0] - array2[0]);
				s[4 * i + 1] = (short)(array[1] - array2[1]);
				s[4 * i + 2] = (short)(array[2] - array2[2]);
				s[4 * i + 3] = (short)(array[3] - array2[3]);
			}
		}
	}

	private short OVERFLOWING_MUL(int x, int y)
	{
		return (short)(x * y);
	}

	private void karatsuba_simple(int[] a_1, int[] b_1, int[] result_final)
	{
		int[] array = new int[31];
		int[] array2 = new int[31];
		int[] array3 = new int[31];
		int[] array4 = new int[63];
		for (int i = 0; i < 16; i++)
		{
			int num = a_1[i];
			int num2 = a_1[i + 16];
			int num3 = a_1[i + 32];
			int num4 = a_1[i + 48];
			for (int j = 0; j < 16; j++)
			{
				int num5 = b_1[j];
				int num6 = b_1[j + 16];
				result_final[i + j] += OVERFLOWING_MUL(num, num5);
				result_final[i + j + 32] += OVERFLOWING_MUL(num2, num6);
				int num7 = num5 + num6;
				int num8 = num + num2;
				array[i + j] = (int)(array[i + j] + (long)num7 * (long)num8);
				num7 = b_1[j + 32];
				num8 = b_1[j + 48];
				result_final[i + j + 64] += OVERFLOWING_MUL(num7, num3);
				result_final[i + j + 96] += OVERFLOWING_MUL(num8, num4);
				int x = num3 + num4;
				int y = num7 + num8;
				array3[i + j] += OVERFLOWING_MUL(x, y);
				num5 += num7;
				num7 = num + num3;
				array4[i + j] += OVERFLOWING_MUL(num5, num7);
				num6 += num8;
				num8 = num2 + num4;
				array4[i + j + 32] += OVERFLOWING_MUL(num6, num8);
				num5 += num6;
				num7 += num8;
				array2[i + j] += OVERFLOWING_MUL(num5, num7);
			}
		}
		for (int i = 0; i < 31; i++)
		{
			array2[i] = array2[i] - array4[i] - array4[i + 32];
			array[i] = array[i] - result_final[i] - result_final[i + 32];
			array3[i] = array3[i] - result_final[i + 64] - result_final[i + 96];
		}
		for (int i = 0; i < 31; i++)
		{
			array4[i + 16] += array2[i];
			result_final[i + 16] += array[i];
			result_final[i + 80] += array3[i];
		}
		for (int i = 0; i < 63; i++)
		{
			array4[i] = array4[i] - result_final[i] - result_final[i + 64];
		}
		for (int i = 0; i < 63; i++)
		{
			result_final[i + 32] += array4[i];
		}
	}

	private void toom_cook_4way(short[] a1, short[] b1, short[] result)
	{
		int num = 43691;
		int num2 = 36409;
		int num3 = 61167;
		int[] array = new int[N_SB];
		int[] array2 = new int[N_SB];
		int[] array3 = new int[N_SB];
		int[] array4 = new int[N_SB];
		int[] array5 = new int[N_SB];
		int[] array6 = new int[N_SB];
		int[] array7 = new int[N_SB];
		int[] array8 = new int[N_SB];
		int[] array9 = new int[N_SB];
		int[] array10 = new int[N_SB];
		int[] array11 = new int[N_SB];
		int[] array12 = new int[N_SB];
		int[] array13 = new int[N_SB];
		int[] array14 = new int[N_SB];
		int[] array15 = new int[N_SB_RES];
		int[] array16 = new int[N_SB_RES];
		int[] array17 = new int[N_SB_RES];
		int[] array18 = new int[N_SB_RES];
		int[] array19 = new int[N_SB_RES];
		int[] array20 = new int[N_SB_RES];
		int[] array21 = new int[N_SB_RES];
		for (int i = 0; i < N_SB; i++)
		{
			int num4 = a1[i];
			int num5 = a1[i + N_SB];
			int num6 = a1[i + N_SB * 2];
			int num7 = a1[i + N_SB * 3];
			int num8 = (short)(num4 + num6);
			int num9 = (short)(num5 + num7);
			int num10 = (short)(num8 + num9);
			int num11 = (short)(num8 - num9);
			array3[i] = num10;
			array4[i] = num11;
			num8 = (short)((num4 << 2) + num6 << 1);
			num9 = (short)((num5 << 2) + num7);
			num10 = (short)(num8 + num9);
			num11 = (short)(num8 - num9);
			array5[i] = num10;
			array6[i] = num11;
			num8 = (short)((num7 << 3) + (num6 << 2) + (num5 << 1) + num4);
			array2[i] = num8;
			array7[i] = num4;
			array[i] = num7;
		}
		for (int i = 0; i < N_SB; i++)
		{
			int num4 = b1[i];
			int num5 = b1[i + N_SB];
			int num6 = b1[i + N_SB * 2];
			int num7 = b1[i + N_SB * 3];
			int num8 = num4 + num6;
			int num9 = num5 + num7;
			int num10 = num8 + num9;
			int num11 = num8 - num9;
			array10[i] = num10;
			array11[i] = num11;
			num8 = (num4 << 2) + num6 << 1;
			num9 = (num5 << 2) + num7;
			num10 = num8 + num9;
			num11 = num8 - num9;
			array12[i] = num10;
			array13[i] = num11;
			num8 = (num7 << 3) + (num6 << 2) + (num5 << 1) + num4;
			array9[i] = num8;
			array14[i] = num4;
			array8[i] = num7;
		}
		karatsuba_simple(array, array8, array15);
		karatsuba_simple(array2, array9, array16);
		karatsuba_simple(array3, array10, array17);
		karatsuba_simple(array4, array11, array18);
		karatsuba_simple(array5, array12, array19);
		karatsuba_simple(array6, array13, array20);
		karatsuba_simple(array7, array14, array21);
		for (int j = 0; j < N_SB_RES; j++)
		{
			int num4 = array15[j];
			int num5 = array16[j];
			int num6 = array17[j];
			int num7 = array18[j];
			int num8 = array19[j];
			int num9 = array20[j];
			int num10 = array21[j];
			num5 += num8;
			num9 -= num8;
			num7 = (num7 & 0xFFFF) - (num6 & 0xFFFF) >> 1;
			num8 -= num4;
			num8 -= num10 << 6;
			num8 = (num8 << 1) + num9;
			num6 += num7;
			num5 = num5 - (num6 << 6) - num6;
			num6 -= num10;
			num6 -= num4;
			num5 += 45 * num6;
			num8 = ((num8 & 0xFFFF) - (num6 << 3)) * num >> 3;
			num9 += num5;
			num5 = ((num5 & 0xFFFF) + ((num7 & 0xFFFF) << 4)) * num2 >> 1;
			num7 = -(num7 + num5);
			num9 = (30 * (num5 & 0xFFFF) - (num9 & 0xFFFF)) * num3 >> 2;
			num6 -= num8;
			num5 -= num9;
			result[j] += (short)(num10 & 0xFFFF);
			result[j + 64] += (short)(num9 & 0xFFFF);
			result[j + 128] += (short)(num8 & 0xFFFF);
			result[j + 192] += (short)(num7 & 0xFFFF);
			result[j + 256] += (short)(num6 & 0xFFFF);
			result[j + 320] += (short)(num5 & 0xFFFF);
			result[j + 384] += (short)(num4 & 0xFFFF);
		}
	}

	private void poly_mul_acc(short[] a, short[] b, short[] res)
	{
		short[] array = new short[2 * SABER_N];
		toom_cook_4way(a, b, array);
		for (int i = SABER_N; i < 2 * SABER_N; i++)
		{
			res[i - SABER_N] += (short)(array[i - SABER_N] - array[i]);
		}
	}

	public void MatrixVectorMul(short[][][] A, short[][] s, short[][] res, int transpose)
	{
		for (int i = 0; i < SABER_L; i++)
		{
			for (int j = 0; j < SABER_L; j++)
			{
				if (transpose == 1)
				{
					poly_mul_acc(A[j][i], s[j], res[i]);
				}
				else
				{
					poly_mul_acc(A[i][j], s[j], res[i]);
				}
			}
		}
	}

	public void InnerProd(short[][] b, short[][] s, short[] res)
	{
		for (int i = 0; i < SABER_L; i++)
		{
			poly_mul_acc(b[i], s[i], res);
		}
	}
}
