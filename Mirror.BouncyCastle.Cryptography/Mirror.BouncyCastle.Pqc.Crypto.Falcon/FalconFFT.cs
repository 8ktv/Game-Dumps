namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconFFT
{
	private FprEngine fpre;

	internal FalconFFT()
	{
		fpre = new FprEngine();
	}

	internal FalconFFT(FprEngine fprengine)
	{
		fpre = fprengine;
	}

	internal FalconFPR[] FPC_ADD(FalconFPR a_re, FalconFPR a_im, FalconFPR b_re, FalconFPR b_im)
	{
		FalconFPR falconFPR = fpre.fpr_add(a_re, b_re);
		FalconFPR falconFPR2 = fpre.fpr_add(a_im, b_im);
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal FalconFPR[] FPC_SUB(FalconFPR a_re, FalconFPR a_im, FalconFPR b_re, FalconFPR b_im)
	{
		FalconFPR falconFPR = fpre.fpr_sub(a_re, b_re);
		FalconFPR falconFPR2 = fpre.fpr_sub(a_im, b_im);
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal FalconFPR[] FPC_MUL(FalconFPR a_re, FalconFPR a_im, FalconFPR b_re, FalconFPR b_im)
	{
		FalconFPR falconFPR = fpre.fpr_sub(fpre.fpr_mul(a_re, b_re), fpre.fpr_mul(a_im, b_im));
		FalconFPR falconFPR2 = fpre.fpr_add(fpre.fpr_mul(a_re, b_im), fpre.fpr_mul(a_im, b_re));
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal FalconFPR[] FPC_SQR(FalconFPR d_re, FalconFPR d_im, FalconFPR a_re, FalconFPR a_im)
	{
		FalconFPR falconFPR = fpre.fpr_sub(fpre.fpr_sqr(a_re), fpre.fpr_sqr(a_im));
		FalconFPR falconFPR2 = fpre.fpr_double(fpre.fpr_mul(a_re, a_im));
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal FalconFPR[] FPC_INV(FalconFPR a_re, FalconFPR a_im)
	{
		FalconFPR x = fpre.fpr_add(fpre.fpr_sqr(a_re), fpre.fpr_sqr(a_im));
		x = fpre.fpr_inv(x);
		FalconFPR falconFPR = fpre.fpr_mul(a_re, x);
		FalconFPR falconFPR2 = fpre.fpr_mul(fpre.fpr_neg(a_im), x);
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal FalconFPR[] FPC_DIV(FalconFPR a_re, FalconFPR a_im, FalconFPR b_re, FalconFPR b_im)
	{
		FalconFPR x = b_re;
		FalconFPR x2 = b_im;
		FalconFPR x3 = fpre.fpr_add(fpre.fpr_sqr(x), fpre.fpr_sqr(x2));
		x3 = fpre.fpr_inv(x3);
		x = fpre.fpr_mul(x, x3);
		x2 = fpre.fpr_mul(fpre.fpr_neg(x2), x3);
		FalconFPR falconFPR = fpre.fpr_sub(fpre.fpr_mul(a_re, x), fpre.fpr_mul(a_im, x2));
		FalconFPR falconFPR2 = fpre.fpr_add(fpre.fpr_mul(a_re, x2), fpre.fpr_mul(a_im, x));
		return new FalconFPR[2] { falconFPR, falconFPR2 };
	}

	internal void FFT(FalconFPR[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		int num2 = num;
		uint num3 = 1u;
		int num4 = 2;
		while (num3 < logn)
		{
			int num5 = num2 >> 1;
			int num6 = num4 >> 1;
			int num7 = 0;
			int num8 = 0;
			while (num7 < num6)
			{
				int num9 = num8 + num5;
				FalconFPR b_re = fpre.fpr_gm_tab[num4 + num7 << 1];
				FalconFPR b_im = fpre.fpr_gm_tab[(num4 + num7 << 1) + 1];
				for (int i = num8; i < num9; i++)
				{
					FalconFPR a_re = fsrc[f + i];
					FalconFPR a_im = fsrc[f + i + num];
					FalconFPR a_re2 = fsrc[f + i + num5];
					FalconFPR a_im2 = fsrc[f + i + num5 + num];
					FalconFPR[] array = FPC_MUL(a_re2, a_im2, b_re, b_im);
					a_re2 = array[0];
					a_im2 = array[1];
					array = FPC_ADD(a_re, a_im, a_re2, a_im2);
					fsrc[f + i] = array[0];
					fsrc[f + i + num] = array[1];
					array = FPC_SUB(a_re, a_im, a_re2, a_im2);
					fsrc[f + i + num5] = array[0];
					fsrc[f + i + num5 + num] = array[1];
				}
				num7++;
				num8 += num2;
			}
			num2 = num5;
			num3++;
			num4 <<= 1;
		}
	}

	internal void iFFT(FalconFPR[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn;
		int num2 = 1;
		int num3 = num;
		int num4 = num >> 1;
		for (int num5 = (int)logn; num5 > 1; num5--)
		{
			int num6 = num3 >> 1;
			int num7 = num2 << 1;
			int num8 = 0;
			for (int i = 0; i < num4; i += num7)
			{
				int num9 = i + num2;
				FalconFPR b_re = fpre.fpr_gm_tab[num6 + num8 << 1];
				FalconFPR b_im = fpre.fpr_neg(fpre.fpr_gm_tab[(num6 + num8 << 1) + 1]);
				for (int j = i; j < num9; j++)
				{
					FalconFPR a_re = fsrc[f + j];
					FalconFPR a_im = fsrc[f + j + num4];
					FalconFPR b_re2 = fsrc[f + j + num2];
					FalconFPR b_im2 = fsrc[f + j + num2 + num4];
					FalconFPR[] array = FPC_ADD(a_re, a_im, b_re2, b_im2);
					fsrc[f + j] = array[0];
					fsrc[f + j + num4] = array[1];
					array = FPC_SUB(a_re, a_im, b_re2, b_im2);
					a_re = array[0];
					a_im = array[1];
					array = FPC_MUL(a_re, a_im, b_re, b_im);
					fsrc[f + j + num2] = array[0];
					fsrc[f + j + num2 + num4] = array[1];
				}
				num8++;
			}
			num2 = num7;
			num3 = num6;
		}
		if (logn != 0)
		{
			FalconFPR y = fpre.fpr_p2_tab[logn];
			for (int num5 = 0; num5 < num; num5++)
			{
				fsrc[f + num5] = fpre.fpr_mul(fsrc[f + num5], y);
			}
		}
	}

	internal void poly_add(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_add(asrc[a + i], bsrc[b + i]);
		}
	}

	internal void poly_sub(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_sub(asrc[a + i], bsrc[b + i]);
		}
	}

	internal void poly_neg(FalconFPR[] asrc, int a, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_neg(asrc[a + i]);
		}
	}

	internal void poly_adj_fft(FalconFPR[] asrc, int a, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = num >> 1; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_neg(asrc[a + i]);
		}
	}

	internal void poly_mul_fft(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR a_re = asrc[a + i];
			FalconFPR a_im = asrc[a + i + num];
			FalconFPR b_re = bsrc[b + i];
			FalconFPR b_im = bsrc[b + i + num];
			FalconFPR[] array = FPC_MUL(a_re, a_im, b_re, b_im);
			asrc[a + i] = array[0];
			asrc[a + i + num] = array[1];
		}
	}

	internal void poly_muladj_fft(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR a_re = asrc[a + i];
			FalconFPR a_im = asrc[a + i + num];
			FalconFPR b_re = bsrc[b + i];
			FalconFPR b_im = fpre.fpr_neg(bsrc[b + i + num]);
			FalconFPR[] array = FPC_MUL(a_re, a_im, b_re, b_im);
			asrc[a + i] = array[0];
			asrc[a + i + num] = array[1];
		}
	}

	internal void poly_mulselfadj_fft(FalconFPR[] asrc, int a, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR x = asrc[a + i];
			FalconFPR x2 = asrc[a + i + num];
			asrc[a + i] = fpre.fpr_add(fpre.fpr_sqr(x), fpre.fpr_sqr(x2));
			asrc[a + i + num] = fpre.fpr_zero;
		}
	}

	internal void poly_mulconst(FalconFPR[] asrc, int a, FalconFPR x, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_mul(asrc[a + i], x);
		}
	}

	internal void poly_div_fft(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR a_re = asrc[a + i];
			FalconFPR a_im = asrc[a + i + num];
			FalconFPR b_re = bsrc[b + i];
			FalconFPR b_im = bsrc[b + i + num];
			FalconFPR[] array = FPC_DIV(a_re, a_im, b_re, b_im);
			asrc[a + i] = array[0];
			asrc[a + i + num] = array[1];
		}
	}

	internal void poly_invnorm2_fft(FalconFPR[] dsrc, int d, FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR x = asrc[a + i];
			FalconFPR x2 = asrc[a + i + num];
			FalconFPR x3 = bsrc[b + i];
			FalconFPR x4 = bsrc[b + i + num];
			dsrc[d + i] = fpre.fpr_inv(fpre.fpr_add(fpre.fpr_add(fpre.fpr_sqr(x), fpre.fpr_sqr(x2)), fpre.fpr_add(fpre.fpr_sqr(x3), fpre.fpr_sqr(x4))));
		}
	}

	internal void poly_add_muladj_fft(FalconFPR[] dsrc, int d, FalconFPR[] Fsrc, int F, FalconFPR[] Gsrc, int G, FalconFPR[] fsrc, int f, FalconFPR[] gsrc, int g, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR a_re = Fsrc[F + i];
			FalconFPR a_im = Fsrc[F + i + num];
			FalconFPR a_re2 = Gsrc[G + i];
			FalconFPR a_im2 = Gsrc[G + i + num];
			FalconFPR b_re = fsrc[f + i];
			FalconFPR x = fsrc[f + i + num];
			FalconFPR b_re2 = gsrc[g + i];
			FalconFPR x2 = gsrc[g + i + num];
			FalconFPR[] array = FPC_MUL(a_re, a_im, b_re, fpre.fpr_neg(x));
			FalconFPR x3 = array[0];
			FalconFPR x4 = array[1];
			FalconFPR[] array2 = FPC_MUL(a_re2, a_im2, b_re2, fpre.fpr_neg(x2));
			FalconFPR y = array2[0];
			FalconFPR y2 = array2[1];
			dsrc[d + i] = fpre.fpr_add(x3, y);
			dsrc[d + i + num] = fpre.fpr_add(x4, y2);
		}
	}

	internal void poly_mul_autoadj_fft(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			asrc[a + i] = fpre.fpr_mul(asrc[a + i], bsrc[b + i]);
			asrc[a + i + num] = fpre.fpr_mul(asrc[a + i + num], bsrc[b + i]);
		}
	}

	internal void poly_div_autoadj_fft(FalconFPR[] asrc, int a, FalconFPR[] bsrc, int b, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR y = fpre.fpr_inv(bsrc[b + i]);
			asrc[a + i] = fpre.fpr_mul(asrc[a + i], y);
			asrc[a + i + num] = fpre.fpr_mul(asrc[a + i + num], y);
		}
	}

	internal void poly_LDL_fft(FalconFPR[] g00src, int g00, FalconFPR[] g01src, int g01, FalconFPR[] g11src, int g11, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR b_re = g00src[g00 + i];
			FalconFPR b_im = g00src[g00 + i + num];
			FalconFPR falconFPR = g01src[g01 + i];
			FalconFPR falconFPR2 = g01src[g01 + i + num];
			FalconFPR a_re = g11src[g11 + i];
			FalconFPR a_im = g11src[g11 + i + num];
			FalconFPR[] array = FPC_DIV(falconFPR, falconFPR2, b_re, b_im);
			FalconFPR falconFPR3 = array[0];
			FalconFPR falconFPR4 = array[1];
			array = FPC_MUL(falconFPR3, falconFPR4, falconFPR, fpre.fpr_neg(falconFPR2));
			falconFPR = array[0];
			falconFPR2 = array[1];
			array = FPC_SUB(a_re, a_im, falconFPR, falconFPR2);
			g11src[g11 + i] = array[0];
			g11src[g11 + i + num] = array[1];
			g01src[g01 + i] = falconFPR3;
			g01src[g01 + i + num] = fpre.fpr_neg(falconFPR4);
		}
	}

	internal void poly_LDLmv_fft(FalconFPR[] d11src, int d11, FalconFPR[] l10src, int l10, FalconFPR[] g00src, int g00, FalconFPR[] g01src, int g01, FalconFPR[] g11src, int g11, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		for (int i = 0; i < num; i++)
		{
			FalconFPR b_re = g00src[g00 + i];
			FalconFPR b_im = g00src[g00 + i + num];
			FalconFPR falconFPR = g01src[g01 + i];
			FalconFPR falconFPR2 = g01src[g01 + i + num];
			FalconFPR a_re = g11src[g11 + i];
			FalconFPR a_im = g11src[g11 + i + num];
			FalconFPR[] array = FPC_DIV(falconFPR, falconFPR2, b_re, b_im);
			FalconFPR falconFPR3 = array[0];
			FalconFPR falconFPR4 = array[1];
			array = FPC_MUL(falconFPR3, falconFPR4, falconFPR, fpre.fpr_neg(falconFPR2));
			falconFPR = array[0];
			falconFPR2 = array[1];
			array = FPC_SUB(a_re, a_im, falconFPR, falconFPR2);
			d11src[d11 + i] = array[0];
			d11src[d11 + i + num] = array[1];
			l10src[l10 + i] = falconFPR3;
			l10src[l10 + i + num] = fpre.fpr_neg(falconFPR4);
		}
	}

	internal void poly_split_fft(FalconFPR[] f0src, int f0, FalconFPR[] f1src, int f1, FalconFPR[] fsrc, int f, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		int num2 = num >> 1;
		f0src[f0] = fsrc[f];
		f1src[f1] = fsrc[f + num];
		for (int i = 0; i < num2; i++)
		{
			FalconFPR a_re = fsrc[f + (i << 1)];
			FalconFPR a_im = fsrc[f + (i << 1) + num];
			FalconFPR b_re = fsrc[f + (i << 1) + 1];
			FalconFPR b_im = fsrc[f + (i << 1) + 1 + num];
			FalconFPR[] array = FPC_ADD(a_re, a_im, b_re, b_im);
			FalconFPR x = array[0];
			FalconFPR x2 = array[1];
			f0src[f0 + i] = fpre.fpr_half(x);
			f0src[f0 + i + num2] = fpre.fpr_half(x2);
			FalconFPR[] array2 = FPC_SUB(a_re, a_im, b_re, b_im);
			x = array2[0];
			x2 = array2[1];
			FalconFPR[] array3 = FPC_MUL(x, x2, fpre.fpr_gm_tab[i + num << 1], fpre.fpr_neg(fpre.fpr_gm_tab[(i + num << 1) + 1]));
			x = array3[0];
			x2 = array3[1];
			f1src[f1 + i] = fpre.fpr_half(x);
			f1src[f1 + i + num2] = fpre.fpr_half(x2);
		}
	}

	internal void poly_merge_fft(FalconFPR[] fsrc, int f, FalconFPR[] f0src, int f0, FalconFPR[] f1src, int f1, uint logn)
	{
		int num = 1 << (int)logn >> 1;
		int num2 = num >> 1;
		fsrc[f] = f0src[f0];
		fsrc[f + num] = f1src[f1];
		for (int i = 0; i < num2; i++)
		{
			FalconFPR a_re = f0src[f0 + i];
			FalconFPR a_im = f0src[f0 + i + num2];
			FalconFPR[] array = FPC_MUL(f1src[f1 + i], f1src[f1 + i + num2], fpre.fpr_gm_tab[i + num << 1], fpre.fpr_gm_tab[(i + num << 1) + 1]);
			FalconFPR b_re = array[0];
			FalconFPR b_im = array[1];
			FalconFPR[] array2 = FPC_ADD(a_re, a_im, b_re, b_im);
			FalconFPR falconFPR = array2[0];
			FalconFPR falconFPR2 = array2[1];
			fsrc[f + (i << 1)] = falconFPR;
			fsrc[f + (i << 1) + num] = falconFPR2;
			FalconFPR[] array3 = FPC_SUB(a_re, a_im, b_re, b_im);
			falconFPR = array3[0];
			falconFPR2 = array3[1];
			fsrc[f + (i << 1) + 1] = falconFPR;
			fsrc[f + (i << 1) + 1 + num] = falconFPR2;
		}
	}
}
