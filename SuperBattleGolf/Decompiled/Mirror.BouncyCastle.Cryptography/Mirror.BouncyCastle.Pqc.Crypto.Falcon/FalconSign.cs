using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

internal class FalconSign
{
	private FalconFFT ffte;

	private FprEngine fpre;

	private FalconCommon common;

	internal FalconSign(FalconCommon common)
	{
		ffte = new FalconFFT();
		fpre = new FprEngine();
		this.common = common;
	}

	internal uint ffLDL_treesize(uint logn)
	{
		return logn + 1 << (int)logn;
	}

	internal void ffLDL_fft_inner(FalconFPR[] treesrc, int tree, FalconFPR[] g0src, int g0, FalconFPR[] g1src, int g1, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		if (num == 1)
		{
			treesrc[tree] = g0src[g0];
			return;
		}
		int num2 = num >> 1;
		ffte.poly_LDLmv_fft(tmpsrc, tmp, treesrc, tree, g0src, g0, g1src, g1, g0src, g0, logn);
		ffte.poly_split_fft(g1src, g1, g1src, g1 + num2, g0src, g0, logn);
		ffte.poly_split_fft(g0src, g0, g0src, g0 + num2, tmpsrc, tmp, logn);
		ffLDL_fft_inner(treesrc, tree + num, g1src, g1, g1src, g1 + num2, logn - 1, tmpsrc, tmp);
		ffLDL_fft_inner(treesrc, tree + num + (int)ffLDL_treesize(logn - 1), g0src, g0, g0src, g0 + num2, logn - 1, tmpsrc, tmp);
	}

	internal void ffLDL_fft(FalconFPR[] treesrc, int tree, FalconFPR[] g00src, int g00, FalconFPR[] g01src, int g01, FalconFPR[] g11src, int g11, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		if (num == 1)
		{
			treesrc[tree] = g00src[g00];
			return;
		}
		int num2 = num >> 1;
		int num3 = tmp;
		int num4 = tmp + num;
		tmp += num << 1;
		Array.Copy(g00src, g00, tmpsrc, num3, num);
		ffte.poly_LDLmv_fft(tmpsrc, num4, treesrc, tree, g00src, g00, g01src, g01, g11src, g11, logn);
		ffte.poly_split_fft(tmpsrc, tmp, tmpsrc, tmp + num2, tmpsrc, num3, logn);
		ffte.poly_split_fft(tmpsrc, num3, tmpsrc, num3 + num2, tmpsrc, num4, logn);
		Array.Copy(tmpsrc, tmp, tmpsrc, num4, num);
		ffLDL_fft_inner(treesrc, tree + num, tmpsrc, num4, tmpsrc, num4 + num2, logn - 1, tmpsrc, tmp);
		ffLDL_fft_inner(treesrc, tree + num + (int)ffLDL_treesize(logn - 1), tmpsrc, num3, tmpsrc, num3 + num2, logn - 1, tmpsrc, tmp);
	}

	internal void ffLDL_binary_normalize(FalconFPR[] treesrc, int tree, uint orig_logn, uint logn)
	{
		int num = 1 << (int)logn;
		if (num == 1)
		{
			treesrc[tree] = fpre.fpr_mul(fpre.fpr_sqrt(treesrc[tree]), fpre.fpr_inv_sigma[orig_logn]);
			return;
		}
		ffLDL_binary_normalize(treesrc, tree + num, orig_logn, logn - 1);
		ffLDL_binary_normalize(treesrc, tree + num + (int)ffLDL_treesize(logn - 1), orig_logn, logn - 1);
	}

	internal void smallints_to_fpr(FalconFPR[] rsrc, int r, sbyte[] tsrc, int t, uint logn)
	{
		int num = 1 << (int)logn;
		for (int i = 0; i < num; i++)
		{
			rsrc[r + i] = fpre.fpr_of(tsrc[t + i]);
		}
	}

	private int skoff_b00(uint logn)
	{
		return 0;
	}

	private int skoff_b01(uint logn)
	{
		return 1 << (int)logn;
	}

	private int skoff_b10(uint logn)
	{
		return 2 << (int)logn;
	}

	private int skoff_b11(uint logn)
	{
		return 3 << (int)logn;
	}

	private int skoff_tree(uint logn)
	{
		return 4 << (int)logn;
	}

	internal void ffSampling_fft_dyntree(SamplerZ samp, FalconFPR[] t0src, int t0, FalconFPR[] t1src, int t1, FalconFPR[] g00src, int g00, FalconFPR[] g01src, int g01, FalconFPR[] g11src, int g11, uint orig_logn, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		if (logn == 0)
		{
			FalconFPR x = g00src[g00];
			x = fpre.fpr_mul(fpre.fpr_sqrt(x), fpre.fpr_inv_sigma[orig_logn]);
			t0src[t0] = fpre.fpr_of(samp.Sample(t0src[t0], x));
			t1src[t1] = fpre.fpr_of(samp.Sample(t1src[t1], x));
			return;
		}
		int num = 1 << (int)logn;
		int num2 = num >> 1;
		ffte.poly_LDL_fft(g00src, g00, g01src, g01, g11src, g11, logn);
		ffte.poly_split_fft(tmpsrc, tmp, tmpsrc, tmp + num2, g00src, g00, logn);
		Array.Copy(tmpsrc, tmp, g00src, g00, num);
		ffte.poly_split_fft(tmpsrc, tmp, tmpsrc, tmp + num2, g11src, g11, logn);
		Array.Copy(tmpsrc, tmp, g11src, g11, num);
		Array.Copy(g01src, g01, tmpsrc, tmp, num);
		Array.Copy(g00src, g00, g01src, g01, num2);
		Array.Copy(g11src, g11, g01src, g01 + num2, num2);
		int num3 = tmp + num;
		ffte.poly_split_fft(tmpsrc, num3, tmpsrc, num3 + num2, tmpsrc, t1, logn);
		ffSampling_fft_dyntree(samp, tmpsrc, num3, tmpsrc, num3 + num2, g11src, g11, g11src, g11 + num2, g01src, g01 + num2, orig_logn, logn - 1, tmpsrc, num3 + num);
		ffte.poly_merge_fft(tmpsrc, tmp + (num << 1), tmpsrc, num3, tmpsrc, num3 + num2, logn);
		Array.Copy(tmpsrc, t1, tmpsrc, num3, num);
		ffte.poly_sub(tmpsrc, num3, tmpsrc, tmp + (num << 1), logn);
		Array.Copy(tmpsrc, tmp + (num << 1), tmpsrc, t1, num);
		ffte.poly_mul_fft(tmpsrc, tmp, tmpsrc, num3, logn);
		ffte.poly_add(tmpsrc, t0, tmpsrc, tmp, logn);
		ffte.poly_split_fft(tmpsrc, tmp, tmpsrc, tmp + num2, tmpsrc, t0, logn);
		ffSampling_fft_dyntree(samp, tmpsrc, tmp, tmpsrc, tmp + num2, g00src, g00, g00src, g00 + num2, g01src, g01, orig_logn, logn - 1, tmpsrc, tmp + num);
		ffte.poly_merge_fft(tmpsrc, t0, tmpsrc, tmp, tmpsrc, tmp + num2, logn);
	}

	internal void ffSampling_fft(SamplerZ samp, FalconFPR[] z0src, int z0, FalconFPR[] z1src, int z1, FalconFPR[] treesrc, int tree, FalconFPR[] t0src, int t0, FalconFPR[] t1src, int t1, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		switch (logn)
		{
		case 2u:
		{
			int tree2 = tree + 4;
			int tree3 = tree + 8;
			FalconFPR x5 = t1src[t1];
			FalconFPR x6 = t1src[t1 + 2];
			FalconFPR y5 = t1src[t1 + 1];
			FalconFPR y6 = t1src[t1 + 3];
			FalconFPR x7 = fpre.fpr_add(x5, y5);
			FalconFPR x8 = fpre.fpr_add(x6, y6);
			FalconFPR y7 = fpre.fpr_half(x7);
			FalconFPR y8 = fpre.fpr_half(x8);
			x7 = fpre.fpr_sub(x5, y5);
			x8 = fpre.fpr_sub(x6, y6);
			FalconFPR falconFPR3 = fpre.fpr_mul(fpre.fpr_add(x7, x8), fpre.fpr_invsqrt8);
			FalconFPR falconFPR4 = fpre.fpr_mul(fpre.fpr_sub(x8, x7), fpre.fpr_invsqrt8);
			FalconFPR falconFPR5 = falconFPR3;
			FalconFPR falconFPR6 = falconFPR4;
			FalconFPR isigma2 = treesrc[tree3 + 3];
			falconFPR3 = fpre.fpr_of(samp.Sample(falconFPR5, isigma2));
			falconFPR4 = fpre.fpr_of(samp.Sample(falconFPR6, isigma2));
			x5 = fpre.fpr_sub(falconFPR5, falconFPR3);
			x6 = fpre.fpr_sub(falconFPR6, falconFPR4);
			y5 = treesrc[tree3];
			y6 = treesrc[tree3 + 1];
			x7 = fpre.fpr_sub(fpre.fpr_mul(x5, y5), fpre.fpr_mul(x6, y6));
			x8 = fpre.fpr_add(fpre.fpr_mul(x5, y6), fpre.fpr_mul(x6, y5));
			falconFPR5 = fpre.fpr_add(x7, y7);
			falconFPR6 = fpre.fpr_add(x8, y8);
			isigma2 = treesrc[tree3 + 2];
			y7 = fpre.fpr_of(samp.Sample(falconFPR5, isigma2));
			y8 = fpre.fpr_of(samp.Sample(falconFPR6, isigma2));
			x5 = y7;
			x6 = y8;
			y5 = falconFPR3;
			y6 = falconFPR4;
			x7 = fpre.fpr_mul(fpre.fpr_sub(y5, y6), fpre.fpr_invsqrt2);
			x8 = fpre.fpr_mul(fpre.fpr_add(y5, y6), fpre.fpr_invsqrt2);
			y7 = (z1src[z1] = fpre.fpr_add(x5, x7));
			falconFPR3 = (z1src[z1 + 2] = fpre.fpr_add(x6, x8));
			y8 = (z1src[z1 + 1] = fpre.fpr_sub(x5, x7));
			falconFPR4 = (z1src[z1 + 3] = fpre.fpr_sub(x6, x8));
			y7 = fpre.fpr_sub(t1src[t1], y7);
			y8 = fpre.fpr_sub(t1src[t1 + 1], y8);
			falconFPR3 = fpre.fpr_sub(t1src[t1 + 2], falconFPR3);
			falconFPR4 = fpre.fpr_sub(t1src[t1 + 3], falconFPR4);
			x5 = y7;
			x6 = falconFPR3;
			y5 = treesrc[tree];
			y6 = treesrc[tree + 2];
			y7 = fpre.fpr_sub(fpre.fpr_mul(x5, y5), fpre.fpr_mul(x6, y6));
			falconFPR3 = fpre.fpr_add(fpre.fpr_mul(x5, y6), fpre.fpr_mul(x6, y5));
			x5 = y8;
			x6 = falconFPR4;
			y5 = treesrc[tree + 1];
			y6 = treesrc[tree + 3];
			y8 = fpre.fpr_sub(fpre.fpr_mul(x5, y5), fpre.fpr_mul(x6, y6));
			falconFPR4 = fpre.fpr_add(fpre.fpr_mul(x5, y6), fpre.fpr_mul(x6, y5));
			y7 = fpre.fpr_add(y7, t0src[t0]);
			y8 = fpre.fpr_add(y8, t0src[t0 + 1]);
			falconFPR3 = fpre.fpr_add(falconFPR3, t0src[t0 + 2]);
			falconFPR4 = fpre.fpr_add(falconFPR4, t0src[t0 + 3]);
			x5 = y7;
			x6 = falconFPR3;
			y5 = y8;
			y6 = falconFPR4;
			x7 = fpre.fpr_add(x5, y5);
			x8 = fpre.fpr_add(x6, y6);
			y7 = fpre.fpr_half(x7);
			y8 = fpre.fpr_half(x8);
			x7 = fpre.fpr_sub(x5, y5);
			x8 = fpre.fpr_sub(x6, y6);
			falconFPR3 = fpre.fpr_mul(fpre.fpr_add(x7, x8), fpre.fpr_invsqrt8);
			falconFPR4 = fpre.fpr_mul(fpre.fpr_sub(x8, x7), fpre.fpr_invsqrt8);
			falconFPR5 = falconFPR3;
			falconFPR6 = falconFPR4;
			isigma2 = treesrc[tree2 + 3];
			FalconFPR y9;
			falconFPR3 = (y9 = fpre.fpr_of(samp.Sample(falconFPR5, isigma2)));
			FalconFPR y10;
			falconFPR4 = (y10 = fpre.fpr_of(samp.Sample(falconFPR6, isigma2)));
			x5 = fpre.fpr_sub(falconFPR5, y9);
			x6 = fpre.fpr_sub(falconFPR6, y10);
			y5 = treesrc[tree2];
			y6 = treesrc[tree2 + 1];
			x7 = fpre.fpr_sub(fpre.fpr_mul(x5, y5), fpre.fpr_mul(x6, y6));
			x8 = fpre.fpr_add(fpre.fpr_mul(x5, y6), fpre.fpr_mul(x6, y5));
			falconFPR5 = fpre.fpr_add(x7, y7);
			falconFPR6 = fpre.fpr_add(x8, y8);
			isigma2 = treesrc[tree2 + 2];
			y7 = fpre.fpr_of(samp.Sample(falconFPR5, isigma2));
			y8 = fpre.fpr_of(samp.Sample(falconFPR6, isigma2));
			x5 = y7;
			x6 = y8;
			y5 = falconFPR3;
			y6 = falconFPR4;
			x7 = fpre.fpr_mul(fpre.fpr_sub(y5, y6), fpre.fpr_invsqrt2);
			x8 = fpre.fpr_mul(fpre.fpr_add(y5, y6), fpre.fpr_invsqrt2);
			z0src[z0] = fpre.fpr_add(x5, x7);
			z0src[z0 + 2] = fpre.fpr_add(x6, x8);
			z0src[z0 + 1] = fpre.fpr_sub(x5, x7);
			z0src[z0 + 3] = fpre.fpr_sub(x6, x8);
			break;
		}
		case 1u:
		{
			FalconFPR falconFPR = t1src[t1];
			FalconFPR falconFPR2 = t1src[t1 + 1];
			FalconFPR isigma = treesrc[tree + 3];
			FalconFPR y = (z1src[z1] = fpre.fpr_of(samp.Sample(falconFPR, isigma)));
			FalconFPR y2 = (z1src[z1 + 1] = fpre.fpr_of(samp.Sample(falconFPR2, isigma)));
			FalconFPR x = fpre.fpr_sub(falconFPR, y);
			FalconFPR x2 = fpre.fpr_sub(falconFPR2, y2);
			FalconFPR y3 = treesrc[tree];
			FalconFPR y4 = treesrc[tree + 1];
			FalconFPR x3 = fpre.fpr_sub(fpre.fpr_mul(x, y3), fpre.fpr_mul(x2, y4));
			FalconFPR x4 = fpre.fpr_add(fpre.fpr_mul(x, y4), fpre.fpr_mul(x2, y3));
			falconFPR = fpre.fpr_add(x3, t0src[t0]);
			falconFPR2 = fpre.fpr_add(x4, t0src[t0 + 1]);
			isigma = treesrc[tree + 2];
			z0src[z0] = fpre.fpr_of(samp.Sample(falconFPR, isigma));
			z0src[z0 + 1] = fpre.fpr_of(samp.Sample(falconFPR2, isigma));
			break;
		}
		default:
		{
			int num = 1 << (int)logn;
			int num2 = num >> 1;
			int tree2 = tree + num;
			int tree3 = tree + num + (int)ffLDL_treesize(logn - 1);
			ffte.poly_split_fft(z1src, z1, z1src, z1 + num2, t1src, t1, logn);
			ffSampling_fft(samp, tmpsrc, tmp, tmpsrc, tmp + num2, treesrc, tree3, z1src, z1, z1src, z1 + num2, logn - 1, tmpsrc, tmp + num);
			ffte.poly_merge_fft(z1src, z1, tmpsrc, tmp, tmpsrc, tmp + num2, logn);
			Array.Copy(t1src, t1, tmpsrc, tmp, num);
			ffte.poly_sub(tmpsrc, tmp, z1src, z1, logn);
			ffte.poly_mul_fft(tmpsrc, tmp, treesrc, tree, logn);
			ffte.poly_add(tmpsrc, tmp, t0src, t0, logn);
			ffte.poly_split_fft(z0src, z0, z0src, z0 + num2, tmpsrc, tmp, logn);
			ffSampling_fft(samp, tmpsrc, tmp, tmpsrc, tmp + num2, treesrc, tree2, z0src, z0, z0src, z0 + num2, logn - 1, tmpsrc, tmp + num);
			ffte.poly_merge_fft(z0src, z0, tmpsrc, tmp, tmpsrc, tmp + num2, logn);
			break;
		}
		}
	}

	internal int do_sign_tree(SamplerZ samp, short[] s2src, int s2, FalconFPR[] ex_keysrc, int expanded_key, ushort[] hmsrc, int hm, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		int num2 = tmp + num;
		int b = expanded_key + skoff_b00(logn);
		int b2 = expanded_key + skoff_b01(logn);
		int b3 = expanded_key + skoff_b10(logn);
		int b4 = expanded_key + skoff_b11(logn);
		int tree = expanded_key + skoff_tree(logn);
		for (int i = 0; i < num; i++)
		{
			tmpsrc[tmp + i] = fpre.fpr_of(hmsrc[hm + i]);
		}
		ffte.FFT(tmpsrc, tmp, logn);
		FalconFPR fpr_inverse_of_q = fpre.fpr_inverse_of_q;
		Array.Copy(tmpsrc, tmp, tmpsrc, num2, num);
		ffte.poly_mul_fft(tmpsrc, num2, ex_keysrc, b2, logn);
		ffte.poly_mulconst(tmpsrc, num2, fpre.fpr_neg(fpr_inverse_of_q), logn);
		ffte.poly_mul_fft(tmpsrc, tmp, ex_keysrc, b4, logn);
		ffte.poly_mulconst(tmpsrc, tmp, fpr_inverse_of_q, logn);
		int num3 = num2 + num;
		int num4 = num3 + num;
		ffSampling_fft(samp, tmpsrc, num3, tmpsrc, num4, ex_keysrc, tree, tmpsrc, tmp, tmpsrc, num2, logn, tmpsrc, num4 + num);
		Array.Copy(tmpsrc, num3, tmpsrc, tmp, num);
		Array.Copy(tmpsrc, num4, tmpsrc, num2, num);
		ffte.poly_mul_fft(tmpsrc, num3, ex_keysrc, b, logn);
		ffte.poly_mul_fft(tmpsrc, num4, ex_keysrc, b3, logn);
		ffte.poly_add(tmpsrc, num3, tmpsrc, num4, logn);
		Array.Copy(tmpsrc, tmp, tmpsrc, num4, num);
		ffte.poly_mul_fft(tmpsrc, num4, ex_keysrc, b2, logn);
		Array.Copy(tmpsrc, num3, tmpsrc, tmp, num);
		ffte.poly_mul_fft(tmpsrc, num2, ex_keysrc, b4, logn);
		ffte.poly_add(tmpsrc, num2, tmpsrc, num4, logn);
		ffte.iFFT(tmpsrc, tmp, logn);
		ffte.iFFT(tmpsrc, num2, logn);
		short[] array = new short[num];
		short[] array2 = new short[num];
		uint num5 = 0u;
		uint num6 = 0u;
		for (int i = 0; i < num; i++)
		{
			int num7 = hmsrc[hm + i] - (int)fpre.fpr_rint(tmpsrc[tmp + i]);
			num5 += (uint)(num7 * num7);
			num6 |= num5;
			array[i] = (short)num7;
		}
		num5 |= (uint)(int)(0L - (long)(num6 >> 31));
		for (int i = 0; i < num; i++)
		{
			array2[i] = (short)(-fpre.fpr_rint(tmpsrc[num2 + i]));
		}
		if (common.is_short_half(num5, array2, 0, logn))
		{
			Array.Copy(array2, 0, s2src, s2, num);
			Array.Copy(array, 0, tmpsrc, tmp, num);
			return 1;
		}
		return 0;
	}

	internal int do_sign_dyn(SamplerZ samp, short[] s2src, int s2, sbyte[] fsrc, int f, sbyte[] gsrc, int g, sbyte[] Fsrc, int F, sbyte[] Gsrc, int G, ushort[] hmsrc, int hm, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		int num = 1 << (int)logn;
		int num2 = tmp;
		int num3 = num2 + num;
		int num4 = num3 + num;
		int num5 = num4 + num;
		smallints_to_fpr(tmpsrc, num3, fsrc, f, logn);
		smallints_to_fpr(tmpsrc, num2, gsrc, g, logn);
		smallints_to_fpr(tmpsrc, num5, Fsrc, F, logn);
		smallints_to_fpr(tmpsrc, num4, Gsrc, G, logn);
		ffte.FFT(tmpsrc, num3, logn);
		ffte.FFT(tmpsrc, num2, logn);
		ffte.FFT(tmpsrc, num5, logn);
		ffte.FFT(tmpsrc, num4, logn);
		ffte.poly_neg(tmpsrc, num3, logn);
		ffte.poly_neg(tmpsrc, num5, logn);
		int num6 = num5 + num;
		int num7 = num6 + num;
		Array.Copy(tmpsrc, num3, tmpsrc, num6, num);
		ffte.poly_mulselfadj_fft(tmpsrc, num6, logn);
		Array.Copy(tmpsrc, num2, tmpsrc, num7, num);
		ffte.poly_muladj_fft(tmpsrc, num7, tmpsrc, num4, logn);
		ffte.poly_mulselfadj_fft(tmpsrc, num2, logn);
		ffte.poly_add(tmpsrc, num2, tmpsrc, num6, logn);
		Array.Copy(tmpsrc, num3, tmpsrc, num6, num);
		ffte.poly_muladj_fft(tmpsrc, num3, tmpsrc, num5, logn);
		ffte.poly_add(tmpsrc, num3, tmpsrc, num7, logn);
		ffte.poly_mulselfadj_fft(tmpsrc, num4, logn);
		Array.Copy(tmpsrc, num5, tmpsrc, num7, num);
		ffte.poly_mulselfadj_fft(tmpsrc, num7, logn);
		ffte.poly_add(tmpsrc, num4, tmpsrc, num7, logn);
		int g2 = num2;
		int g3 = num3;
		int num8 = num4;
		num3 = num6;
		num6 = num3 + num;
		num7 = num6 + num;
		for (int i = 0; i < num; i++)
		{
			tmpsrc[num6 + i] = fpre.fpr_of((short)hmsrc[hm + i]);
		}
		ffte.FFT(tmpsrc, num6, logn);
		FalconFPR fpr_inverse_of_q = fpre.fpr_inverse_of_q;
		Array.Copy(tmpsrc, num6, tmpsrc, num7, num);
		ffte.poly_mul_fft(tmpsrc, num7, tmpsrc, num3, logn);
		ffte.poly_mulconst(tmpsrc, num7, fpre.fpr_neg(fpr_inverse_of_q), logn);
		ffte.poly_mul_fft(tmpsrc, num6, tmpsrc, num5, logn);
		ffte.poly_mulconst(tmpsrc, num6, fpr_inverse_of_q, logn);
		Array.Copy(tmpsrc, num6, tmpsrc, num5, num * 2);
		num6 = num8 + num;
		num7 = num6 + num;
		ffSampling_fft_dyntree(samp, tmpsrc, num6, tmpsrc, num7, tmpsrc, g2, tmpsrc, g3, tmpsrc, num8, logn, logn, tmpsrc, num7 + num);
		num2 = tmp;
		num3 = num2 + num;
		num4 = num3 + num;
		num5 = num4 + num;
		Array.Copy(tmpsrc, num6, tmpsrc, num5 + num, num * 2);
		num6 = num5 + num;
		num7 = num6 + num;
		smallints_to_fpr(tmpsrc, num3, fsrc, f, logn);
		smallints_to_fpr(tmpsrc, num2, gsrc, g, logn);
		smallints_to_fpr(tmpsrc, num5, Fsrc, F, logn);
		smallints_to_fpr(tmpsrc, num4, Gsrc, G, logn);
		ffte.FFT(tmpsrc, num3, logn);
		ffte.FFT(tmpsrc, num2, logn);
		ffte.FFT(tmpsrc, num5, logn);
		ffte.FFT(tmpsrc, num4, logn);
		ffte.poly_neg(tmpsrc, num3, logn);
		ffte.poly_neg(tmpsrc, num5, logn);
		int num9 = num7 + num;
		int num10 = num9 + num;
		Array.Copy(tmpsrc, num6, tmpsrc, num9, num);
		Array.Copy(tmpsrc, num7, tmpsrc, num10, num);
		ffte.poly_mul_fft(tmpsrc, num9, tmpsrc, num2, logn);
		ffte.poly_mul_fft(tmpsrc, num10, tmpsrc, num4, logn);
		ffte.poly_add(tmpsrc, num9, tmpsrc, num10, logn);
		Array.Copy(tmpsrc, num6, tmpsrc, num10, num);
		ffte.poly_mul_fft(tmpsrc, num10, tmpsrc, num3, logn);
		Array.Copy(tmpsrc, num9, tmpsrc, num6, num);
		ffte.poly_mul_fft(tmpsrc, num7, tmpsrc, num5, logn);
		ffte.poly_add(tmpsrc, num7, tmpsrc, num10, logn);
		ffte.iFFT(tmpsrc, num6, logn);
		ffte.iFFT(tmpsrc, num7, logn);
		short[] array = new short[num];
		uint num11 = 0u;
		uint num12 = 0u;
		for (int i = 0; i < num; i++)
		{
			int num13 = hmsrc[hm + i] - (int)fpre.fpr_rint(tmpsrc[num6 + i]);
			num11 += (uint)(num13 * num13);
			num12 |= num11;
			array[i] = (short)num13;
		}
		num11 |= (uint)(int)(0L - (long)(num12 >> 31));
		short[] array2 = new short[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = (short)(-fpre.fpr_rint(tmpsrc[num7 + i]));
		}
		if (common.is_short_half(num11, array2, 0, logn))
		{
			Array.Copy(array2, 0, s2src, s2, num);
			return 1;
		}
		return 0;
	}

	internal void sign_tree(short[] sigsrc, int sig, SHAKE256 rng, FalconFPR[] ex_keysrc, int expanded_key, ushort[] hmsrc, int hm, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		SamplerZ samp;
		do
		{
			FalconRNG falconRNG = new FalconRNG();
			falconRNG.prng_init(rng);
			samp = new SamplerZ(falconRNG, fpre.fpr_sigma_min[logn], fpre);
		}
		while (do_sign_tree(samp, sigsrc, sig, ex_keysrc, expanded_key, hmsrc, hm, logn, tmpsrc, tmp) == 0);
	}

	internal void sign_dyn(short[] sigsrc, int sig, SHAKE256 rng, sbyte[] fsrc, int f, sbyte[] gsrc, int g, sbyte[] Fsrc, int F, sbyte[] Gsrc, int G, ushort[] hmsrc, int hm, uint logn, FalconFPR[] tmpsrc, int tmp)
	{
		SamplerZ samp;
		do
		{
			FalconRNG falconRNG = new FalconRNG();
			falconRNG.prng_init(rng);
			samp = new SamplerZ(falconRNG, fpre.fpr_sigma_min[logn], fpre);
		}
		while (do_sign_dyn(samp, sigsrc, sig, fsrc, f, gsrc, g, Fsrc, F, Gsrc, G, hmsrc, hm, logn, tmpsrc, tmp) == 0);
	}
}
