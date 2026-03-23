using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Frodo;

public class FrodoEngine
{
	internal static int nbar = 8;

	private static int mbar = 8;

	private static int len_seedA = 128;

	private static int len_z = 128;

	private static int len_chi = 16;

	private static int len_seedA_bytes = len_seedA / 8;

	private static int len_z_bytes = len_z / 8;

	private static int len_chi_bytes = len_chi / 8;

	private int D;

	private int q;

	private int n;

	private int B;

	private int len_sk_bytes;

	private int len_pk_bytes;

	private int len_ct_bytes;

	private short[] T_chi;

	private int len_mu;

	private int len_seedSE;

	private int len_s;

	private int len_k;

	private int len_pkh;

	private int len_ss;

	private int len_mu_bytes;

	private int len_seedSE_bytes;

	private int len_s_bytes;

	private int len_k_bytes;

	private int len_pkh_bytes;

	private int len_ss_bytes;

	private IDigest digest;

	private FrodoMatrixGenerator gen;

	public int CipherTextSize => len_ct_bytes;

	public int SessionKeySize => len_ss_bytes;

	public int PrivateKeySize => len_sk_bytes;

	public int PublicKeySize => len_pk_bytes;

	public FrodoEngine(int n, int D, int B, short[] cdf_table, IDigest digest, FrodoMatrixGenerator mGen)
	{
		this.n = n;
		this.D = D;
		q = 1 << D;
		this.B = B;
		len_mu = B * nbar * nbar;
		len_seedSE = len_mu;
		len_s = len_mu;
		len_k = len_mu;
		len_pkh = len_mu;
		len_ss = len_mu;
		len_mu_bytes = len_mu / 8;
		len_seedSE_bytes = len_seedSE / 8;
		len_s_bytes = len_s / 8;
		len_k_bytes = len_k / 8;
		len_pkh_bytes = len_pkh / 8;
		len_ss_bytes = len_ss / 8;
		len_ct_bytes = D * n * nbar / 8 + D * nbar * nbar / 8;
		len_pk_bytes = len_seedA_bytes + D * n * nbar / 8;
		len_sk_bytes = len_s_bytes + len_pk_bytes + (2 * n * nbar + len_pkh_bytes);
		T_chi = cdf_table;
		this.digest = digest;
		gen = mGen;
	}

	private short Sample(short r)
	{
		short num = (short)((r & 0xFFFF) >> 1);
		short num2 = 0;
		for (int i = 0; i < T_chi.Length; i++)
		{
			if (num > T_chi[i])
			{
				num2++;
			}
		}
		if ((r & 0xFFFF) % 2 == 1)
		{
			num2 = (short)((num2 * -1) & 0xFFFF);
		}
		return num2;
	}

	private short[] SampleMatrix(short[] r, int offset, int n1, int n2)
	{
		short[] array = new short[n1 * n2];
		for (int i = 0; i < n1; i++)
		{
			for (int j = 0; j < n2; j++)
			{
				array[i * n2 + j] = Sample(r[i * n2 + j + offset]);
			}
		}
		return array;
	}

	private short[] MatrixTranspose(short[] X, int n1, int n2)
	{
		short[] array = new short[n1 * n2];
		for (int i = 0; i < n2; i++)
		{
			for (int j = 0; j < n1; j++)
			{
				array[i * n1 + j] = X[j * n2 + i];
			}
		}
		return array;
	}

	private short[] MatrixMul(short[] X, int Xrow, int Xcol, short[] Y, int Ycol)
	{
		int num = q - 1;
		short[] array = new short[Xrow * Ycol];
		for (int i = 0; i < Xrow; i++)
		{
			for (int j = 0; j < Ycol; j++)
			{
				int num2 = 0;
				for (int k = 0; k < Xcol; k++)
				{
					num2 += X[i * Xcol + k] * Y[k * Ycol + j];
				}
				array[i * Ycol + j] = (short)(num2 & num);
			}
		}
		return array;
	}

	private short[] MatrixAdd(short[] X, short[] Y, int n1, int m1)
	{
		int num = q - 1;
		short[] array = new short[n1 * m1];
		for (int i = 0; i < n1; i++)
		{
			for (int j = 0; j < m1; j++)
			{
				array[i * m1 + j] = (short)((X[i * m1 + j] + Y[i * m1 + j]) & num);
			}
		}
		return array;
	}

	private byte[] FrodoPack(short[] C)
	{
		int num = C.Length;
		byte[] array = new byte[D * num / 8];
		short num2 = 0;
		short num3 = 0;
		short num4 = 0;
		byte b = 0;
		while (num2 < array.Length && (num3 < num || (num3 == num && b > 0)))
		{
			byte b2 = 0;
			while (b2 < 8)
			{
				int num5 = System.Math.Min(8 - b2, b);
				short num6 = (short)((1 << num5) - 1);
				byte b3 = (byte)((num4 >> b - num5) & num6);
				array[num2] = (byte)(array[num2] + (b3 << 8 - b2 - num5));
				b2 += (byte)num5;
				b -= (byte)num5;
				if (b == 0)
				{
					if (num3 >= num)
					{
						break;
					}
					num4 = C[num3];
					b = (byte)D;
					num3++;
				}
			}
			if (b2 == 8)
			{
				num2++;
			}
		}
		return array;
	}

	public void kem_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		byte[] array = new byte[len_s_bytes + len_seedSE_bytes + len_z_bytes];
		random.NextBytes(array);
		byte[] a = Arrays.CopyOfRange(array, 0, len_s_bytes);
		byte[] array2 = Arrays.CopyOfRange(array, len_s_bytes, len_s_bytes + len_seedSE_bytes);
		byte[] array3 = Arrays.CopyOfRange(array, len_s_bytes + len_seedSE_bytes, len_s_bytes + len_seedSE_bytes + len_z_bytes);
		byte[] array4 = new byte[len_seedA_bytes];
		digest.BlockUpdate(array3, 0, array3.Length);
		((IXof)digest).OutputFinal(array4, 0, array4.Length);
		short[] x = gen.GenMatrix(array4);
		byte[] array5 = new byte[2 * n * nbar * len_chi_bytes];
		digest.Update(95);
		digest.BlockUpdate(array2, 0, array2.Length);
		((IXof)digest).OutputFinal(array5, 0, array5.Length);
		short[] array6 = new short[2 * n * nbar];
		for (int i = 0; i < array6.Length; i++)
		{
			array6[i] = (short)Pack.LE_To_UInt16(array5, i * 2);
		}
		short[] array7 = SampleMatrix(array6, 0, nbar, n);
		short[] y = MatrixTranspose(array7, nbar, n);
		short[] y2 = SampleMatrix(array6, n * nbar, n, nbar);
		short[] c = MatrixAdd(MatrixMul(x, n, n, y, nbar), y2, n, nbar);
		byte[] b = FrodoPack(c);
		Array.Copy(Arrays.Concatenate(array4, b), 0, pk, 0, len_pk_bytes);
		byte[] array8 = new byte[len_pkh_bytes];
		digest.BlockUpdate(pk, 0, pk.Length);
		((IXof)digest).OutputFinal(array8, 0, array8.Length);
		Array.Copy(Arrays.Concatenate(a, pk), 0, sk, 0, len_s_bytes + len_pk_bytes);
		byte[] array9 = new byte[4];
		for (int j = 0; j < nbar; j++)
		{
			for (int k = 0; k < n; k++)
			{
				Pack.UInt16_To_LE((ushort)array7[j * n + k], array9);
				Array.Copy(array9, 0, sk, len_s_bytes + len_pk_bytes + j * n * 2 + k * 2, 2);
			}
		}
		Array.Copy(array8, 0, sk, len_sk_bytes - len_pkh_bytes, len_pkh_bytes);
	}

	private short[] FrodoUnpack(byte[] input, int n1, int n2)
	{
		short[] array = new short[n1 * n2];
		short num = 0;
		short num2 = 0;
		byte b = 0;
		byte b2 = 0;
		while (num < array.Length && (num2 < input.Length || (num2 == input.Length && b2 > 0)))
		{
			byte b3 = 0;
			while (b3 < D)
			{
				int num3 = System.Math.Min(D - b3, b2);
				short num4 = (short)(((1 << num3) - 1) & 0xFFFF);
				byte b4 = (byte)(((b & 0xFF) >> (b2 & 0xFF) - num3) & (num4 & 0xFFFF) & 0xFF);
				array[num] = (short)(((array[num] & 0xFFFF) + ((b4 & 0xFF) << D - (b3 & 0xFF) - num3)) & 0xFFFF);
				b3 += (byte)num3;
				b2 -= (byte)num3;
				b &= (byte)(~(num4 << (int)b2));
				if (b2 == 0)
				{
					if (num2 >= input.Length)
					{
						break;
					}
					b = input[num2];
					b2 = 8;
					num2++;
				}
			}
			if (b3 == D)
			{
				num++;
			}
		}
		return array;
	}

	private short[] Encode(byte[] k)
	{
		int num = 0;
		byte b = 1;
		short[] array = new short[mbar * nbar];
		for (int i = 0; i < mbar; i++)
		{
			for (int j = 0; j < nbar; j++)
			{
				int num2 = 0;
				for (int l = 0; l < B; l++)
				{
					if ((k[num] & b) == b)
					{
						num2 += 1 << l;
					}
					b <<= 1;
					if (b == 0)
					{
						b = 1;
						num++;
					}
				}
				array[i * nbar + j] = (short)(num2 * (q / (1 << B)));
			}
		}
		return array;
	}

	public void kem_enc(byte[] ct, byte[] ss, byte[] pk, SecureRandom random)
	{
		byte[] seedA = Arrays.CopyOfRange(pk, 0, len_seedA_bytes);
		byte[] input = Arrays.CopyOfRange(pk, len_seedA_bytes, len_pk_bytes);
		byte[] array = new byte[len_mu_bytes];
		random.NextBytes(array);
		byte[] array2 = new byte[len_pkh_bytes];
		digest.BlockUpdate(pk, 0, len_pk_bytes);
		((IXof)digest).OutputFinal(array2, 0, len_pkh_bytes);
		byte[] array3 = new byte[len_seedSE + len_k];
		array3[0] = 150;
		digest.BlockUpdate(array2, 0, len_pkh_bytes);
		digest.BlockUpdate(array, 0, len_mu_bytes);
		((IXof)digest).OutputFinal(array3, 1, len_seedSE_bytes + len_k_bytes);
		byte[] array4 = new byte[(2 * mbar * n + mbar * nbar) * len_chi_bytes];
		digest.BlockUpdate(array3, 0, 1 + len_seedSE_bytes);
		((IXof)digest).OutputFinal(array4, 0, array4.Length);
		short[] array5 = new short[array4.Length / 2];
		for (int i = 0; i < array5.Length; i++)
		{
			array5[i] = (short)Pack.LE_To_UInt16(array4, i * 2);
		}
		short[] x = SampleMatrix(array5, 0, mbar, n);
		short[] y = SampleMatrix(array5, mbar * n, mbar, n);
		short[] y2 = gen.GenMatrix(seedA);
		short[] c = MatrixAdd(MatrixMul(x, mbar, n, y2, n), y, mbar, n);
		byte[] array6 = FrodoPack(c);
		short[] y3 = SampleMatrix(array5, 2 * mbar * n, mbar, nbar);
		short[] y4 = FrodoUnpack(input, n, nbar);
		short[] x2 = MatrixAdd(MatrixMul(x, mbar, n, y4, nbar), y3, mbar, nbar);
		short[] y5 = Encode(array);
		short[] c2 = MatrixAdd(x2, y5, nbar, mbar);
		byte[] array7 = FrodoPack(c2);
		Array.Copy(Arrays.Concatenate(array6, array7), 0, ct, 0, len_ct_bytes);
		digest.BlockUpdate(array6, 0, array6.Length);
		digest.BlockUpdate(array7, 0, array7.Length);
		digest.BlockUpdate(array3, 1 + len_seedSE_bytes, len_k_bytes);
		((IXof)digest).OutputFinal(ss, 0, len_s_bytes);
	}

	private short[] MatrixSub(short[] X, short[] Y, int n1, int n2)
	{
		int num = q - 1;
		short[] array = new short[n1 * n2];
		for (int i = 0; i < n1; i++)
		{
			for (int j = 0; j < n2; j++)
			{
				array[i * n2 + j] = (short)((X[i * n2 + j] - Y[i * n2 + j]) & num);
			}
		}
		return array;
	}

	private byte[] Decode(short[] input)
	{
		int num = 0;
		int num2 = 8;
		int num3 = nbar * nbar / 8;
		short num4 = (short)((1 << B) - 1);
		short num5 = (short)((1 << D) - 1);
		byte[] array = new byte[num2 * B];
		for (int i = 0; i < num3; i++)
		{
			long num6 = 0L;
			for (int j = 0; j < num2; j++)
			{
				short num7 = (short)((input[num] & num5) + (1 << D - B - 1) >> D - B);
				num6 |= (long)(num7 & num4) << B * j;
				num++;
			}
			for (int j = 0; j < B; j++)
			{
				array[i * B + j] = (byte)((num6 >> 8 * j) & 0xFF);
			}
		}
		return array;
	}

	private short CTVerify(short[] a1, short[] a2, short[] b1, short[] b2)
	{
		short num = 0;
		for (short num2 = 0; num2 < a1.Length; num2++)
		{
			num |= (short)(a1[num2] ^ b1[num2]);
		}
		for (short num3 = 0; num3 < a2.Length; num3++)
		{
			num |= (short)(a2[num3] ^ b2[num3]);
		}
		if (num == 0)
		{
			return 0;
		}
		return -1;
	}

	private byte[] CTSelect(byte[] a, byte[] b, short selector)
	{
		byte[] array = new byte[a.Length];
		for (int i = 0; i < a.Length; i++)
		{
			array[i] = (byte)((~selector & a[i] & 0xFF) | (selector & b[i] & 0xFF));
		}
		return array;
	}

	public void kem_dec(byte[] ss, byte[] ct, byte[] sk)
	{
		int num = 0;
		int num2 = mbar * n * D / 8;
		byte[] array = Arrays.CopyOfRange(ct, num, num + num2);
		num += num2;
		num2 = mbar * nbar * D / 8;
		byte[] array2 = Arrays.CopyOfRange(ct, num, num + num2);
		num = 0;
		num2 = len_s_bytes;
		byte[] b = Arrays.CopyOfRange(sk, num, num + num2);
		num += num2;
		num2 = len_seedA_bytes;
		byte[] seedA = Arrays.CopyOfRange(sk, num, num + num2);
		num += num2;
		num2 = D * n * nbar / 8;
		byte[] input = Arrays.CopyOfRange(sk, num, num + num2);
		num += num2;
		num2 = n * nbar * 16 / 8;
		byte[] bs = Arrays.CopyOfRange(sk, num, num + num2);
		short[] array3 = new short[nbar * n];
		for (int i = 0; i < nbar; i++)
		{
			for (int j = 0; j < n; j++)
			{
				array3[i * n + j] = (short)Pack.LE_To_UInt16(bs, i * n * 2 + j * 2);
			}
		}
		short[] y = MatrixTranspose(array3, nbar, n);
		num += num2;
		num2 = len_pkh_bytes;
		byte[] input2 = Arrays.CopyOfRange(sk, num, num + num2);
		short[] array4 = FrodoUnpack(array, mbar, n);
		short[] array5 = FrodoUnpack(array2, mbar, nbar);
		short[] y2 = MatrixMul(array4, mbar, n, y, nbar);
		short[] input3 = MatrixSub(array5, y2, mbar, nbar);
		byte[] array6 = Decode(input3);
		byte[] array7 = new byte[len_seedSE_bytes + len_k_bytes];
		digest.BlockUpdate(input2, 0, len_pkh_bytes);
		digest.BlockUpdate(array6, 0, len_mu_bytes);
		((IXof)digest).OutputFinal(array7, 0, len_seedSE_bytes + len_k_bytes);
		byte[] a = Arrays.CopyOfRange(array7, len_seedSE_bytes, len_seedSE_bytes + len_k_bytes);
		byte[] array8 = new byte[(2 * mbar * n + mbar * mbar) * len_chi_bytes];
		digest.Update(150);
		digest.BlockUpdate(array7, 0, len_seedSE_bytes);
		((IXof)digest).OutputFinal(array8, 0, array8.Length);
		short[] array9 = new short[2 * mbar * n + mbar * nbar];
		for (int k = 0; k < array9.Length; k++)
		{
			array9[k] = (short)Pack.LE_To_UInt16(array8, k * 2);
		}
		short[] x = SampleMatrix(array9, 0, mbar, n);
		short[] y3 = SampleMatrix(array9, mbar * n, mbar, n);
		short[] y4 = gen.GenMatrix(seedA);
		short[] b2 = MatrixAdd(MatrixMul(x, mbar, n, y4, n), y3, mbar, n);
		short[] y5 = SampleMatrix(array9, 2 * mbar * n, mbar, nbar);
		short[] y6 = FrodoUnpack(input, n, nbar);
		short[] x2 = MatrixAdd(MatrixMul(x, mbar, n, y6, nbar), y5, mbar, nbar);
		short[] b3 = MatrixAdd(x2, Encode(array6), mbar, nbar);
		short selector = CTVerify(array4, array5, b2, b3);
		byte[] array10 = CTSelect(a, b, selector);
		digest.BlockUpdate(array, 0, array.Length);
		digest.BlockUpdate(array2, 0, array2.Length);
		digest.BlockUpdate(array10, 0, array10.Length);
		((IXof)digest).OutputFinal(ss, 0, len_ss_bytes);
	}
}
