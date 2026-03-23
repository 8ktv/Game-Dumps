using System;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class SidhCompressed
{
	private readonly SikeEngine engine;

	private static uint t_points = 2u;

	internal SidhCompressed(SikeEngine engine)
	{
		this.engine = engine;
	}

	internal void init_basis(ulong[] gen, ulong[][] XP, ulong[][] XQ, ulong[][] XR)
	{
		engine.fpx.fpcopy(gen, 0L, XP[0]);
		engine.fpx.fpcopy(gen, engine.param.NWORDS_FIELD, XP[1]);
		engine.fpx.fpcopy(gen, 2 * engine.param.NWORDS_FIELD, XQ[0]);
		engine.fpx.fpcopy(gen, 3 * engine.param.NWORDS_FIELD, XQ[1]);
		engine.fpx.fpcopy(gen, 4 * engine.param.NWORDS_FIELD, XR[0]);
		engine.fpx.fpcopy(gen, 5 * engine.param.NWORDS_FIELD, XR[1]);
	}

	internal void FormatPrivKey_B(byte[] skB)
	{
		skB[engine.param.SECRETKEY_B_BYTES - 2] &= (byte)engine.param.MASK3_BOB;
		skB[engine.param.SECRETKEY_B_BYTES - 1] &= (byte)engine.param.MASK2_BOB;
		engine.fpx.mul3(skB);
	}

	internal void random_mod_order_A(byte[] random_digits, SecureRandom random)
	{
		byte[] array = new byte[engine.param.SECRETKEY_A_BYTES];
		random.NextBytes(array);
		Array.Copy(array, 0L, random_digits, 0L, engine.param.SECRETKEY_A_BYTES);
		random_digits[0] &= 254;
		random_digits[engine.param.SECRETKEY_A_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
	}

	internal void random_mod_order_B(byte[] random_digits, SecureRandom random)
	{
		byte[] array = new byte[engine.param.SECRETKEY_B_BYTES];
		random.NextBytes(array);
		Array.Copy(array, 0L, random_digits, 0L, engine.param.SECRETKEY_A_BYTES);
		FormatPrivKey_B(random_digits);
	}

	internal void Ladder3pt_dual(PointProj[] Rs, ulong[] m, uint AliceOrBob, PointProj R, ulong[][] A24)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = ((AliceOrBob != engine.param.ALICE) ? engine.param.OBOB_BITS : engine.param.OALICE_BITS);
		engine.fpx.fp2copy(Rs[1].X, pointProj.X);
		engine.fpx.fp2copy(Rs[1].Z, pointProj.Z);
		engine.fpx.fp2copy(Rs[2].X, pointProj2.X);
		engine.fpx.fp2copy(Rs[2].Z, pointProj2.Z);
		engine.fpx.fp2copy(Rs[0].X, R.X);
		engine.fpx.fp2copy(Rs[0].Z, R.Z);
		uint num5;
		ulong option;
		for (uint num3 = 0u; num3 < num2; num3++)
		{
			int num4 = (int)((m[num3 >> (int)Internal.LOG2RADIX] >> (int)(num3 & (Internal.RADIX - 1))) & 1);
			num5 = (uint)num4 ^ num;
			num = (uint)num4;
			option = 0uL - (ulong)num5;
			engine.isogeny.SwapPoints(R, pointProj2, option);
			engine.isogeny.XDblAdd(pointProj, pointProj2, R.X, A24);
			engine.fpx.fp2mul_mont(pointProj2.X, R.Z, pointProj2.X);
		}
		num5 = 0 ^ num;
		option = 0uL - (ulong)num5;
		engine.isogeny.SwapPoints(R, pointProj2, option);
	}

	internal void Elligator2(ulong[][] a24, uint[] r, uint rIndex, ulong[][] x, byte[] bit, uint bitOffset, uint COMPorDEC)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array5 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array6 = new ulong[engine.param.NWORDS_FIELD];
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array8 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array);
		engine.fpx.fp2add(a24, a24, array7);
		engine.fpx.fpsubPRIME(array7[0], array, array7[0]);
		engine.fpx.fp2add(array7, array7, array7);
		num = r[rIndex];
		engine.fpx.fp2mul_mont(array7, engine.param.v_3_torsion[num], x);
		engine.fpx.fp2neg(x);
		if (COMPorDEC == 0)
		{
			engine.fpx.fp2add(array7, x, array8);
			engine.fpx.fp2mul_mont(array8, x, array8);
			engine.fpx.fpaddPRIME(array8[0], array, array8[0]);
			engine.fpx.fp2mul_mont(x, array8, array8);
			engine.fpx.fpsqr_mont(array8[0], array2);
			engine.fpx.fpsqr_mont(array8[1], array3);
			engine.fpx.fpaddPRIME(array2, array3, array4);
			engine.fpx.fpcopy(array4, 0L, array5);
			for (uint num2 = 0u; num2 < engine.param.OALICE_BITS - 2; num2++)
			{
				engine.fpx.fpsqr_mont(array5, array5);
			}
			for (uint num2 = 0u; num2 < engine.param.OBOB_EXPON; num2++)
			{
				engine.fpx.fpsqr_mont(array5, array6);
				engine.fpx.fpmul_mont(array5, array6, array5);
			}
			engine.fpx.fpsqr_mont(array5, array6);
			engine.fpx.fpcorrectionPRIME(array6);
			engine.fpx.fpcorrectionPRIME(array4);
			if (!Fpx.subarrayEquals(array6, array4, engine.param.NWORDS_FIELD))
			{
				engine.fpx.fp2neg(x);
				engine.fpx.fp2sub(x, array7, x);
				if (COMPorDEC == 0)
				{
					bit[bitOffset] = 1;
				}
			}
		}
		else if (bit[bitOffset] == 1)
		{
			engine.fpx.fp2neg(x);
			engine.fpx.fp2sub(x, array7, x);
		}
	}

	internal void make_positive(ulong[][] x)
	{
		uint nWORDS_FIELD = engine.param.NWORDS_FIELD;
		ulong[] b = new ulong[engine.param.NWORDS_FIELD];
		engine.fpx.from_fp2mont(x, x);
		if (!Fpx.subarrayEquals(x[0], b, nWORDS_FIELD))
		{
			if ((x[0][0] & 1) == 1)
			{
				engine.fpx.fp2neg(x);
			}
		}
		else if ((x[1][0] & 1) == 1)
		{
			engine.fpx.fp2neg(x);
		}
		engine.fpx.to_fp2mont(x, x);
	}

	internal void BiQuad_affine(ulong[][] a24, ulong[][] x0, ulong[][] x1, PointProj R)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2add(a24, a24, array);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2sub(x0, x1, array2);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2mul_mont(x0, x1, array4);
		engine.fpx.fpsubPRIME(array4[0], engine.param.Montgomery_one, array4[0]);
		engine.fpx.fp2sqr_mont(array4, array4);
		engine.fpx.fpsubPRIME(x0[0], engine.param.Montgomery_one, array3[0]);
		engine.fpx.fpcopy(x0[1], 0L, array3[1]);
		engine.fpx.fp2sqr_mont(array3, array3);
		engine.fpx.fp2mul_mont(array, x0, array5);
		engine.fpx.fp2add(array3, array5, array3);
		engine.fpx.fp2mul_mont(x1, array3, array3);
		engine.fpx.fpsubPRIME(x1[0], engine.param.Montgomery_one, array5[0]);
		engine.fpx.fpcopy(x1[1], 0L, array5[1]);
		engine.fpx.fp2sqr_mont(array5, array5);
		engine.fpx.fp2mul_mont(array, x1, array6);
		engine.fpx.fp2add(array5, array6, array5);
		engine.fpx.fp2mul_mont(x0, array5, array5);
		engine.fpx.fp2add(array3, array5, array3);
		engine.fpx.fp2add(array3, array3, array3);
		engine.fpx.fp2sqr_mont(array3, array5);
		engine.fpx.fp2mul_mont(array2, array4, array6);
		engine.fpx.fp2add(array6, array6, array6);
		engine.fpx.fp2add(array6, array6, array6);
		engine.fpx.fp2sub(array5, array6, array5);
		engine.fpx.sqrt_Fp2(array5, array5);
		make_positive(array5);
		engine.fpx.fp2add(array3, array5, R.X);
		engine.fpx.fp2add(array2, array2, R.Z);
	}

	internal void get_4_isog_dual(PointProj P, ulong[][] A24, ulong[][] C24, ulong[][][] coeff)
	{
		engine.fpx.fp2sub(P.X, P.Z, coeff[1]);
		engine.fpx.fp2add(P.X, P.Z, coeff[2]);
		engine.fpx.fp2sqr_mont(P.Z, coeff[4]);
		engine.fpx.fp2add(coeff[4], coeff[4], coeff[0]);
		engine.fpx.fp2sqr_mont(coeff[0], C24);
		engine.fpx.fp2add(coeff[0], coeff[0], coeff[0]);
		engine.fpx.fp2sqr_mont(P.X, coeff[3]);
		engine.fpx.fp2add(coeff[3], coeff[3], A24);
		engine.fpx.fp2sqr_mont(A24, A24);
	}

	internal void eval_dual_2_isog(ulong[][] X2, ulong[][] Z2, PointProj P)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2add(P.X, P.Z, array);
		engine.fpx.fp2sub(P.X, P.Z, P.Z);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2sqr_mont(P.Z, P.Z);
		engine.fpx.fp2sub(array, P.Z, P.Z);
		engine.fpx.fp2mul_mont(X2, P.Z, P.Z);
		engine.fpx.fp2mul_mont(Z2, array, P.X);
	}

	internal void eval_final_dual_2_isog(PointProj P)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		engine.fpx.fp2add(P.X, P.Z, array);
		engine.fpx.fp2mul_mont(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array, P.X);
		engine.fpx.fpcopy(P.X[0], 0L, array3);
		engine.fpx.fpcopy(P.X[1], 0L, P.X[0]);
		engine.fpx.fpcopy(array3, 0L, P.X[1]);
		engine.fpx.fpnegPRIME(P.X[1]);
		engine.fpx.fp2add(array2, array2, P.Z);
		engine.fpx.fp2add(P.Z, P.Z, P.Z);
	}

	internal void eval_dual_4_isog_shared(ulong[][] X4pZ4, ulong[][] X42, ulong[][] Z42, ulong[][][] coeff, uint coeffOffset)
	{
		engine.fpx.fp2sub(X42, Z42, coeff[coeffOffset]);
		engine.fpx.fp2add(X42, Z42, coeff[1 + coeffOffset]);
		engine.fpx.fp2sqr_mont(X4pZ4, coeff[2 + coeffOffset]);
		engine.fpx.fp2sub(coeff[2 + coeffOffset], coeff[1 + coeffOffset], coeff[2 + coeffOffset]);
	}

	internal void eval_dual_4_isog(ulong[][] A24, ulong[][] C24, ulong[][][] coeff, uint coeffOffset, PointProj P)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2add(P.X, P.Z, array);
		engine.fpx.fp2sub(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2sub(array, array2, array3);
		engine.fpx.fp2sub(C24, A24, array4);
		engine.fpx.fp2mul_mont(array3, array4, array4);
		engine.fpx.fp2mul_mont(C24, array, array3);
		engine.fpx.fp2sub(array3, array4, array3);
		engine.fpx.fp2mul_mont(array3, array, P.X);
		engine.fpx.fp2mul_mont(array4, array2, P.Z);
		engine.fpx.fp2mul_mont(coeff[coeffOffset], P.X, P.X);
		engine.fpx.fp2mul_mont(coeff[1 + coeffOffset], P.Z, array);
		engine.fpx.fp2add(P.X, array, P.X);
		engine.fpx.fp2mul_mont(coeff[2 + coeffOffset], P.Z, P.Z);
	}

	internal void eval_full_dual_4_isog(ulong[][][][] As, PointProj P)
	{
		for (uint num = 0u; num < engine.param.MAX_Alice; num++)
		{
			eval_dual_4_isog(As[engine.param.MAX_Alice - num][0], As[engine.param.MAX_Alice - num][1], As[engine.param.MAX_Alice - num - 1], 2u, P);
		}
		if (engine.param.OALICE_BITS % 2 == 1)
		{
			eval_dual_2_isog(As[engine.param.MAX_Alice][2], As[engine.param.MAX_Alice][3], P);
		}
		eval_final_dual_2_isog(P);
	}

	internal void TripleAndParabola_proj(PointProjFull R, ulong[][] l1x, ulong[][] l1z)
	{
		engine.fpx.fp2sqr_mont(R.X, l1z);
		engine.fpx.fp2add(l1z, l1z, l1x);
		engine.fpx.fp2add(l1x, l1z, l1x);
		engine.fpx.fpaddPRIME(l1x[0], engine.param.Montgomery_one, l1x[0]);
		engine.fpx.fp2add(R.Y, R.Y, l1z);
	}

	internal void Tate3_proj(PointProjFull P, PointProjFull Q, ulong[][] gX, ulong[][] gZ)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		TripleAndParabola_proj(P, array2, gZ);
		engine.fpx.fp2sub(Q.X, P.X, gX);
		engine.fpx.fp2mul_mont(array2, gX, gX);
		engine.fpx.fp2sub(P.Y, Q.Y, array);
		engine.fpx.fp2mul_mont(gZ, array, array);
		engine.fpx.fp2add(gX, array, gX);
	}

	internal void FinalExpo3(ulong[][] gX, ulong[][] gZ)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2copy(gZ, array);
		engine.fpx.fpnegPRIME(array[1]);
		engine.fpx.fp2mul_mont(gX, array, array);
		engine.fpx.fp2inv_mont_bingcd(array);
		engine.fpx.fpnegPRIME(gX[1]);
		engine.fpx.fp2mul_mont(gX, gZ, gX);
		engine.fpx.fp2mul_mont(gX, array, gX);
		for (uint num = 0u; num < engine.param.OALICE_BITS; num++)
		{
			engine.fpx.fp2sqr_mont(gX, gX);
		}
		for (uint num = 0u; num < engine.param.OBOB_EXPON - 1; num++)
		{
			engine.fpx.cube_Fp2_cycl(gX, engine.param.Montgomery_one);
		}
	}

	internal void FinalExpo3_2way(ulong[][][] gX, ulong[][][] gZ)
	{
		ulong[][][] array = SikeUtilities.InitArray(2u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] array2 = SikeUtilities.InitArray(2u, 2u, engine.param.NWORDS_FIELD);
		for (uint num = 0u; num < 2; num++)
		{
			engine.fpx.fp2copy(gZ[num], array[num]);
			engine.fpx.fpnegPRIME(array[num][1]);
			engine.fpx.fp2mul_mont(gX[num], array[num], array[num]);
		}
		engine.fpx.mont_n_way_inv(array, 2u, array2);
		for (uint num = 0u; num < 2; num++)
		{
			engine.fpx.fpnegPRIME(gX[num][1]);
			engine.fpx.fp2mul_mont(gX[num], gZ[num], gX[num]);
			engine.fpx.fp2mul_mont(gX[num], array2[num], gX[num]);
			for (uint num2 = 0u; num2 < engine.param.OALICE_BITS; num2++)
			{
				engine.fpx.fp2sqr_mont(gX[num], gX[num]);
			}
			for (uint num2 = 0u; num2 < engine.param.OBOB_EXPON - 1; num2++)
			{
				engine.fpx.cube_Fp2_cycl(gX[num], engine.param.Montgomery_one);
			}
		}
	}

	private bool FirstPoint_dual(PointProj P, PointProjFull R, byte[] ind)
	{
		PointProjFull pointProjFull = new PointProjFull(engine.param.NWORDS_FIELD);
		PointProjFull pointProjFull2 = new PointProjFull(engine.param.NWORDS_FIELD);
		ulong[][][] array = SikeUtilities.InitArray(2u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] array2 = SikeUtilities.InitArray(2u, 2u, engine.param.NWORDS_FIELD);
		ulong[] b = new ulong[engine.param.NWORDS_FIELD];
		uint nWORDS_FIELD = engine.param.NWORDS_FIELD;
		Fpx fpx = engine.fpx;
		ulong[] b_gen_3_tors = engine.param.B_gen_3_tors;
		_ = engine.param.NWORDS_FIELD;
		fpx.fpcopy(b_gen_3_tors, 0L, pointProjFull.X[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, engine.param.NWORDS_FIELD, pointProjFull.X[1]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 2 * engine.param.NWORDS_FIELD, pointProjFull.Y[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 3 * engine.param.NWORDS_FIELD, pointProjFull.Y[1]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 4 * engine.param.NWORDS_FIELD, pointProjFull2.X[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 5 * engine.param.NWORDS_FIELD, pointProjFull2.X[1]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 6 * engine.param.NWORDS_FIELD, pointProjFull2.Y[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 7 * engine.param.NWORDS_FIELD, pointProjFull2.Y[1]);
		engine.isogeny.CompletePoint(P, R);
		Tate3_proj(pointProjFull, R, array[0], array2[0]);
		Tate3_proj(pointProjFull2, R, array[1], array2[1]);
		FinalExpo3_2way(array, array2);
		engine.fpx.fp2correction(array[0]);
		engine.fpx.fp2correction(array[1]);
		uint num = ((!Fpx.subarrayEquals(array[0][1], b, nWORDS_FIELD)) ? (Fpx.subarrayEquals(array[0][1], engine.param.g_R_S_im, nWORDS_FIELD) ? 1u : 2u) : 0u);
		uint num2 = ((!Fpx.subarrayEquals(array[1][1], b, nWORDS_FIELD)) ? (Fpx.subarrayEquals(array[1][1], engine.param.g_R_S_im, nWORDS_FIELD) ? 1u : 2u) : 0u);
		if (num == 0 && num2 == 0)
		{
			return false;
		}
		if (num == 0)
		{
			ind[0] = 0;
		}
		else if (num2 == 0)
		{
			ind[0] = 1;
		}
		else if (num + num2 == 3)
		{
			ind[0] = 3;
		}
		else
		{
			ind[0] = 2;
		}
		return true;
	}

	private bool SecondPoint_dual(PointProj P, PointProjFull R, byte[] ind)
	{
		PointProjFull pointProjFull = new PointProjFull(engine.param.NWORDS_FIELD);
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] gZ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[] b = new ulong[engine.param.NWORDS_FIELD];
		uint nWORDS_FIELD = engine.param.NWORDS_FIELD;
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, 4 * ind[0] * engine.param.NWORDS_FIELD, pointProjFull.X[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, (4 * ind[0] + 1) * engine.param.NWORDS_FIELD, pointProjFull.X[1]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, (4 * ind[0] + 2) * engine.param.NWORDS_FIELD, pointProjFull.Y[0]);
		engine.fpx.fpcopy(engine.param.B_gen_3_tors, (4 * ind[0] + 3) * engine.param.NWORDS_FIELD, pointProjFull.Y[1]);
		engine.isogeny.CompletePoint(P, R);
		Tate3_proj(pointProjFull, R, array, gZ);
		FinalExpo3(array, gZ);
		engine.fpx.fp2correction(array);
		if (!Fpx.subarrayEquals(array[1], b, nWORDS_FIELD))
		{
			return true;
		}
		return false;
	}

	internal void FirstPoint3n(ulong[][] a24, ulong[][][][] As, ulong[][] x, PointProjFull R, uint[] r, byte[] ind, byte[] bitEll)
	{
		bool flag = false;
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		ulong[] a25 = new ulong[engine.param.NWORDS_FIELD];
		r[0] = 0u;
		while (!flag)
		{
			bitEll[0] = 0;
			Elligator2(a24, r, 0u, x, bitEll, 0u, 0u);
			engine.fpx.fp2copy(x, pointProj.X);
			engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj.Z[0]);
			engine.fpx.fpcopy(a25, 0L, pointProj.Z[1]);
			eval_full_dual_4_isog(As, pointProj);
			flag = FirstPoint_dual(pointProj, R, ind);
			r[0] = r[0] + 1;
		}
	}

	internal void SecondPoint3n(ulong[][] a24, ulong[][][][] As, ulong[][] x, PointProjFull R, uint[] r, byte[] ind, byte[] bitEll)
	{
		bool flag = false;
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		ulong[] a25 = new ulong[engine.param.NWORDS_FIELD];
		while (!flag)
		{
			bitEll[0] = 0;
			Elligator2(a24, r, 1u, x, bitEll, 0u, 0u);
			engine.fpx.fp2copy(x, pointProj.X);
			engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj.Z[0]);
			engine.fpx.fpcopy(a25, 0L, pointProj.Z[1]);
			eval_full_dual_4_isog(As, pointProj);
			flag = SecondPoint_dual(pointProj, R, ind);
			r[1] = r[1] + 1;
		}
	}

	internal void makeDiff(PointProjFull R, PointProjFull S, PointProj D)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint nWORDS_FIELD = engine.param.NWORDS_FIELD;
		engine.fpx.fp2sub(R.X, S.X, array);
		engine.fpx.fp2sub(R.Y, S.Y, array2);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2add(R.X, S.X, array3);
		engine.fpx.fp2mul_mont(array, array3, array3);
		engine.fpx.fp2sub(array2, array3, array2);
		engine.fpx.fp2mul_mont(D.Z, array2, array2);
		engine.fpx.fp2mul_mont(D.X, array, array);
		engine.fpx.fp2correction(array);
		engine.fpx.fp2correction(array2);
		if (Fpx.subarrayEquals(array[0], array2[0], nWORDS_FIELD) & Fpx.subarrayEquals(array[1], array2[1], nWORDS_FIELD))
		{
			engine.fpx.fp2neg(S.Y);
		}
	}

	internal void BuildOrdinary3nBasis_dual(ulong[][] a24, ulong[][][][] As, PointProjFull[] R, uint[] r, uint[] bitsEll, uint bitsEllOffset)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		ulong[][][] array = SikeUtilities.InitArray(2u, 2u, engine.param.NWORDS_FIELD);
		byte[] ind = new byte[1];
		byte[] array2 = new byte[1];
		FirstPoint3n(a24, As, array[0], R[0], r, ind, array2);
		bitsEll[bitsEllOffset] = array2[0];
		r[1] = r[0];
		SecondPoint3n(a24, As, array[1], R[1], r, ind, array2);
		bitsEll[bitsEllOffset] |= (uint)(array2[0] << 1);
		BiQuad_affine(a24, array[0], array[1], pointProj);
		eval_full_dual_4_isog(As, pointProj);
		makeDiff(R[0], R[1], pointProj);
	}

	internal void FullIsogeny_A_dual(byte[] PrivateKeyA, ulong[][][][] As, ulong[][] a24, uint sike)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_ALICE];
		ulong[][] xP = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] xQ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] array6 = SikeUtilities.InitArray(5u, 2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array7 = new uint[engine.param.MAX_INT_POINTS_ALICE];
		ulong[] array8 = new ulong[engine.param.NWORDS_ORDER];
		init_basis(engine.param.A_gen, xP, xQ, array2);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0]);
		engine.fpx.fp2add(array3, array3, array3);
		engine.fpx.fp2add(array3, array3, array4);
		engine.fpx.fp2add(array3, array4, array5);
		engine.fpx.fp2add(array4, array4, array3);
		engine.fpx.decode_to_digits(PrivateKeyA, engine.param.MSG_BYTES, array8, engine.param.SECRETKEY_A_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(xP, xQ, array2, array8, engine.param.ALICE, pointProj, array5);
		engine.fpx.fp2inv_mont(pointProj.Z);
		engine.fpx.fp2mul_mont(pointProj.X, pointProj.Z, pointProj.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj.Z[0]);
		engine.fpx.fpzero(pointProj.Z[1]);
		if (sike == 1)
		{
			engine.fpx.fp2_encode(pointProj.X, PrivateKeyA, engine.param.MSG_BYTES + engine.param.SECRETKEY_A_BYTES + engine.param.CRYPTO_PUBLICKEYBYTES);
		}
		if (engine.param.OALICE_BITS % 2 == 1)
		{
			PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
			engine.isogeny.XDblE(pointProj, pointProj2, array3, array4, engine.param.OALICE_BITS - 1);
			engine.isogeny.Get2Isog(pointProj2, array3, array4);
			engine.isogeny.Eval2Isog(pointProj, pointProj2);
			engine.fpx.fp2copy(pointProj2.X, As[engine.param.MAX_Alice][2]);
			engine.fpx.fp2copy(pointProj2.Z, As[engine.param.MAX_Alice][3]);
		}
		num = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Alice; num4++)
		{
			uint num5;
			for (; num < engine.param.MAX_Alice - num4; num += num5)
			{
				array[num2] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array[num2].X);
				engine.fpx.fp2copy(pointProj.Z, array[num2].Z);
				array7[num2++] = num;
				num5 = engine.param.strat_Alice[num3++];
				engine.isogeny.XDblE(pointProj, pointProj, array3, array4, 2 * num5);
			}
			engine.fpx.fp2copy(array3, As[num4 - 1][0]);
			engine.fpx.fp2copy(array4, As[num4 - 1][1]);
			get_4_isog_dual(pointProj, array3, array4, array6);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval4Isog(array[num6], array6);
			}
			eval_dual_4_isog_shared(array6[2], array6[3], array6[4], As[num4 - 1], 2u);
			engine.fpx.fp2copy(array[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num2 - 1].Z, pointProj.Z);
			num = array7[num2 - 1];
			num2--;
		}
		engine.fpx.fp2copy(array3, As[engine.param.MAX_Alice - 1][0]);
		engine.fpx.fp2copy(array4, As[engine.param.MAX_Alice - 1][1]);
		get_4_isog_dual(pointProj, array3, array4, array6);
		eval_dual_4_isog_shared(array6[2], array6[3], array6[4], As[engine.param.MAX_Alice - 1], 2u);
		engine.fpx.fp2copy(array3, As[engine.param.MAX_Alice][0]);
		engine.fpx.fp2copy(array4, As[engine.param.MAX_Alice][1]);
		engine.fpx.fp2inv_mont_bingcd(array4);
		engine.fpx.fp2mul_mont(array3, array4, a24);
	}

	internal void Dlogs3_dual(ulong[][][] f, int[] D, ulong[] d0, ulong[] c0, ulong[] d1, ulong[] c1)
	{
		solve_dlog(f[0], D, d0, 3u);
		solve_dlog(f[2], D, c0, 3u);
		solve_dlog(f[1], D, d1, 3u);
		solve_dlog(f[3], D, c1, 3u);
		engine.fpx.mp_sub(engine.param.Bob_order, c0, c0, engine.param.NWORDS_ORDER);
		engine.fpx.mp_sub(engine.param.Bob_order, c1, c1, engine.param.NWORDS_ORDER);
	}

	internal void BuildOrdinary3nBasis_Decomp_dual(ulong[][] A24, PointProj[] Rs, uint[] r, uint[] bitsEll, uint bitsEllIndex)
	{
		byte[] bit = new byte[2]
		{
			(byte)(bitsEll[bitsEllIndex] & 1),
			(byte)((bitsEll[bitsEllIndex] >> 1) & 1)
		};
		r[0]--;
		Elligator2(A24, r, 0u, Rs[0].X, bit, 0u, 1u);
		r[1]--;
		Elligator2(A24, r, 1u, Rs[1].X, bit, 1u, 1u);
		BiQuad_affine(A24, Rs[0].X, Rs[1].X, Rs[2]);
	}

	internal void PKADecompression_dual(byte[] SecretKeyB, byte[] CompressedPKA, PointProj R, ulong[][] A)
	{
		uint[] array = new uint[3];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PointProj[] array3 = new PointProj[3]
		{
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD)
		};
		ulong[] array4 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array5 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array6 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array7 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array8 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array9 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array10 = new ulong[engine.param.NWORDS_ORDER];
		engine.fpx.fp2_decode(CompressedPKA, A, 3 * engine.param.ORDER_B_ENCODED_BYTES);
		array8[0] = 1uL;
		engine.fpx.to_Montgomery_mod_order(array8, array8, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		byte b = (byte)((CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] & 0xFF) >> 7);
		byte[] array11 = new byte[3];
		Array.Copy(CompressedPKA, 3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES, array11, 0L, 3L);
		array[0] = (uint)(array11[0] & 0xFFFF);
		array[1] = (uint)(array11[1] & 0xFFFF);
		array[2] = (uint)(array11[2] & 0xFFFF);
		array[0] &= 127u;
		engine.fpx.fpaddPRIME(A[0], engine.param.Montgomery_one, array2[0]);
		engine.fpx.fpcopy(A[1], 0L, array2[1]);
		engine.fpx.fpaddPRIME(array2[0], engine.param.Montgomery_one, array2[0]);
		engine.fpx.fp2div2(array2, array2);
		engine.fpx.fp2div2(array2, array2);
		BuildOrdinary3nBasis_Decomp_dual(array2, array3, array, array, 2u);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0].Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[1].Z[0]);
		engine.isogeny.SwapPoints(array3[0], array3[1], (ulong)(-b));
		engine.fpx.decode_to_digits(SecretKeyB, 0u, array10, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.to_Montgomery_mod_order(array10, array4, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.decode_to_digits(CompressedPKA, 0u, array9, engine.param.ORDER_B_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.to_Montgomery_mod_order(array9, array5, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.decode_to_digits(CompressedPKA, engine.param.ORDER_B_ENCODED_BYTES, array9, engine.param.ORDER_B_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.to_Montgomery_mod_order(array9, array6, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.decode_to_digits(CompressedPKA, 2 * engine.param.ORDER_B_ENCODED_BYTES, array9, engine.param.ORDER_B_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.to_Montgomery_mod_order(array9, array7, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		if (b == 0)
		{
			engine.fpx.Montgomery_multiply_mod_order(array4, array6, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.mp_add(array6, array8, array6, engine.param.NWORDS_ORDER);
			engine.fpx.Montgomery_inversion_mod_order_bingcd(array6, array6, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
			engine.fpx.Montgomery_multiply_mod_order(array4, array7, array7, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.mp_add(array5, array7, array7, engine.param.NWORDS_ORDER);
			engine.fpx.Montgomery_multiply_mod_order(array6, array7, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array6, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			Ladder3pt_dual(array3, array6, engine.param.BOB, R, array2);
		}
		else
		{
			engine.fpx.Montgomery_multiply_mod_order(array4, array7, array7, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.mp_add(array7, array8, array7, engine.param.NWORDS_ORDER);
			engine.fpx.Montgomery_inversion_mod_order_bingcd(array7, array7, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
			engine.fpx.Montgomery_multiply_mod_order(array4, array6, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.mp_add(array5, array6, array6, engine.param.NWORDS_ORDER);
			engine.fpx.Montgomery_multiply_mod_order(array6, array7, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array6, array6, engine.param.Bob_order, engine.param.Montgomery_RB2);
			Ladder3pt_dual(array3, array6, engine.param.BOB, R, array2);
		}
		engine.isogeny.Double(R, R, array2, engine.param.OALICE_BITS);
	}

	internal void Compress_PKA_dual(ulong[] d0, ulong[] c0, ulong[] d1, ulong[] c1, ulong[][] a24, uint[] rs, byte[] CompressedPKA)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2add(a24, a24, array3);
		engine.fpx.fp2add(array3, array3, array3);
		engine.fpx.fpsubPRIME(array3[0], engine.param.Montgomery_one, array3[0]);
		engine.fpx.fpsubPRIME(array3[0], engine.param.Montgomery_one, array3[0]);
		uint num = engine.fpx.mod3(d1);
		engine.fpx.to_Montgomery_mod_order(c0, c0, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.to_Montgomery_mod_order(c1, c1, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.to_Montgomery_mod_order(d0, d0, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		engine.fpx.to_Montgomery_mod_order(d1, d1, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
		if (num != 0)
		{
			engine.fpx.Montgomery_inversion_mod_order_bingcd(d1, array2, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
			engine.fpx.Montgomery_neg(d0, engine.param.Bob_order);
			engine.fpx.Montgomery_multiply_mod_order(d0, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, 0u, engine.param.ORDER_B_ENCODED_BYTES);
			engine.fpx.Montgomery_neg(c1, engine.param.Bob_order);
			engine.fpx.Montgomery_multiply_mod_order(c1, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, engine.param.ORDER_B_ENCODED_BYTES, engine.param.ORDER_B_ENCODED_BYTES);
			engine.fpx.Montgomery_multiply_mod_order(c0, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, 2 * engine.param.ORDER_B_ENCODED_BYTES, engine.param.ORDER_B_ENCODED_BYTES);
			CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] = 0;
		}
		else
		{
			engine.fpx.Montgomery_inversion_mod_order_bingcd(d0, array2, engine.param.Bob_order, engine.param.Montgomery_RB2, engine.param.Montgomery_RB1);
			engine.fpx.Montgomery_neg(d1, engine.param.Bob_order);
			engine.fpx.Montgomery_multiply_mod_order(d1, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, 0u, engine.param.ORDER_B_ENCODED_BYTES);
			engine.fpx.Montgomery_multiply_mod_order(c1, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, engine.param.ORDER_B_ENCODED_BYTES, engine.param.ORDER_B_ENCODED_BYTES);
			engine.fpx.Montgomery_neg(c0, engine.param.Bob_order);
			engine.fpx.Montgomery_multiply_mod_order(c0, array2, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.from_Montgomery_mod_order(array, array, engine.param.Bob_order, engine.param.Montgomery_RB2);
			engine.fpx.encode_to_bytes(array, CompressedPKA, 2 * engine.param.ORDER_B_ENCODED_BYTES, engine.param.ORDER_B_ENCODED_BYTES);
			CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] = 128;
		}
		engine.fpx.fp2_encode(array3, CompressedPKA, 3 * engine.param.ORDER_B_ENCODED_BYTES);
		CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] |= (byte)rs[0];
		CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 1] = (byte)rs[1];
		CompressedPKA[3 * engine.param.ORDER_B_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 2] = (byte)rs[2];
	}

	internal uint EphemeralKeyGeneration_A_extended(byte[] PrivateKeyA, byte[] CompressedPKA)
	{
		uint[] array = new uint[3];
		int[] d = new int[engine.param.DLEN_3];
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][][] array2 = SikeUtilities.InitArray(engine.param.MAX_Alice + 1, 5u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] f = SikeUtilities.InitArray(4u, 2u, engine.param.NWORDS_FIELD);
		ulong[] c = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] c2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d3 = new ulong[engine.param.NWORDS_ORDER];
		PointProjFull[] array3 = new PointProjFull[2]
		{
			new PointProjFull(engine.param.NWORDS_FIELD),
			new PointProjFull(engine.param.NWORDS_FIELD)
		};
		FullIsogeny_A_dual(PrivateKeyA, array2, a, 1u);
		BuildOrdinary3nBasis_dual(a, array2, array3, array, array, 2u);
		Tate3_pairings(array3, f);
		Dlogs3_dual(f, d, d2, c, d3, c2);
		Compress_PKA_dual(d2, c, d3, c2, a, array, CompressedPKA);
		return 0u;
	}

	private uint EphemeralKeyGeneration_A(byte[] PrivateKeyA, byte[] CompressedPKA)
	{
		uint[] array = new uint[3];
		int[] d = new int[engine.param.DLEN_3];
		ulong[] c = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] c2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d3 = new ulong[engine.param.NWORDS_ORDER];
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] f = SikeUtilities.InitArray(4u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][][] array2 = SikeUtilities.InitArray(engine.param.MAX_Alice + 1, 5u, 2u, engine.param.NWORDS_FIELD);
		PointProjFull[] array3 = new PointProjFull[2];
		FullIsogeny_A_dual(PrivateKeyA, array2, a, 0u);
		BuildOrdinary3nBasis_dual(a, array2, array3, array, array, 2u);
		Tate3_pairings(array3, f);
		Dlogs3_dual(f, d, d2, c, d3, c2);
		Compress_PKA_dual(d2, c, d3, c2, a, array, CompressedPKA);
		return 0u;
	}

	internal uint EphemeralSecretAgreement_B(byte[] PrivateKeyB, byte[] PKA, byte[] SharedSecretB)
	{
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array = new uint[engine.param.MAX_INT_POINTS_BOB];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array4 = new PointProj[engine.param.MAX_INT_POINTS_BOB];
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PKADecompression_dual(PrivateKeyB, PKA, pointProj, a);
		engine.fpx.fp2copy(a, array6);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, engine.param.Montgomery_one, array3[0]);
		engine.fpx.fp2add(array6, array3, array2);
		engine.fpx.fp2sub(array6, array3, array3);
		num2 = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Bob; num4++)
		{
			uint num5;
			for (; num2 < engine.param.MAX_Bob - num4; num2 += num5)
			{
				array4[num3] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array4[num3].X);
				engine.fpx.fp2copy(pointProj.Z, array4[num3].Z);
				array[num3++] = num2;
				num5 = engine.param.strat_Bob[num++];
				engine.isogeny.XTplE(pointProj, pointProj, array3, array2, num5);
			}
			engine.isogeny.Get3Isog(pointProj, array3, array2, coeff);
			for (uint num6 = 0u; num6 < num3; num6++)
			{
				engine.isogeny.Eval3Isog(array4[num6], coeff);
			}
			engine.fpx.fp2copy(array4[num3 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array4[num3 - 1].Z, pointProj.Z);
			num2 = array[num3 - 1];
			num3--;
		}
		engine.isogeny.Get3Isog(pointProj, array3, array2, coeff);
		engine.fpx.fp2add(array2, array3, array6);
		engine.fpx.fp2add(array6, array6, array6);
		engine.fpx.fp2sub(array2, array3, array2);
		engine.isogeny.JInv(array6, array2, array5);
		engine.fpx.fp2_encode(array5, SharedSecretB, 0u);
		return 0u;
	}

	internal void BuildEntangledXonly(ulong[][] A, PointProj[] R, byte[] qnr, byte[] ind)
	{
		ulong[] s = new ulong[engine.param.NWORDS_FIELD];
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		ulong[][] b;
		if (engine.fpx.is_sqr_fp2(A, s))
		{
			b = engine.param.table_v_qnr;
			qnr[0] = 1;
		}
		else
		{
			b = engine.param.table_v_qr;
			qnr[0] = 0;
		}
		ind[0] = 0;
		do
		{
			engine.fpx.fp2mul_mont(A, b, num, R[0].X);
			num += 2;
			engine.fpx.fp2neg(R[0].X);
			engine.fpx.fp2add(R[0].X, A, array2);
			engine.fpx.fp2mul_mont(R[0].X, array2, array2);
			engine.fpx.fpaddPRIME(array2[0], engine.param.Montgomery_one, array2[0]);
			engine.fpx.fp2mul_mont(R[0].X, array2, array2);
			ind[0]++;
		}
		while (!engine.fpx.is_sqr_fp2(array2, s));
		ind[0]--;
		if (qnr[0] == 1)
		{
			engine.fpx.fpcopy(engine.param.table_r_qnr[ind[0]], 0L, array[0]);
		}
		else
		{
			engine.fpx.fpcopy(engine.param.table_r_qr[ind[0]], 0L, array[0]);
		}
		engine.fpx.fp2add(R[0].X, A, R[1].X);
		engine.fpx.fp2neg(R[1].X);
		engine.fpx.fp2sub(R[0].X, R[1].X, R[2].Z);
		engine.fpx.fp2sqr_mont(R[2].Z, R[2].Z);
		engine.fpx.fpcopy(array[0], 0L, array[1]);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array[0], array[0]);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2mul_mont(array2, array, R[2].X);
	}

	internal void RecoverY(ulong[][] A, PointProj[] xs, PointProjFull[] Rs)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2mul_mont(xs[2].X, xs[1].Z, array);
		engine.fpx.fp2mul_mont(xs[1].X, xs[2].Z, array2);
		engine.fpx.fp2mul_mont(xs[1].X, xs[2].X, array3);
		engine.fpx.fp2mul_mont(xs[1].Z, xs[2].Z, array4);
		engine.fpx.fp2sqr_mont(xs[1].X, array5);
		engine.fpx.fp2sqr_mont(xs[1].Z, Rs[1].X);
		engine.fpx.fp2sub(array3, array4, Rs[1].Y);
		engine.fpx.fp2mul_mont(xs[1].X, Rs[1].Y, Rs[1].Y);
		engine.fpx.fp2add(array5, Rs[1].X, array5);
		engine.fpx.fp2mul_mont(xs[2].Z, array5, array5);
		engine.fpx.fp2mul_mont(A, array2, Rs[1].X);
		engine.fpx.fp2sub(array, array2, Rs[1].Z);
		engine.fpx.fp2mul_mont(Rs[0].X, Rs[1].Z, array);
		engine.fpx.fp2add(array3, Rs[1].X, array2);
		engine.fpx.fp2add(array2, array2, array2);
		engine.fpx.fp2sub(array, array2, array);
		engine.fpx.fp2mul_mont(xs[1].Z, array, array);
		engine.fpx.fp2sub(array, array5, array);
		engine.fpx.fp2mul_mont(Rs[0].X, array, array);
		engine.fpx.fp2add(array, Rs[1].Y, Rs[1].Y);
		engine.fpx.fp2mul_mont(Rs[0].Y, array4, array);
		engine.fpx.fp2mul_mont(xs[1].X, array, Rs[1].X);
		engine.fpx.fp2add(Rs[1].X, Rs[1].X, Rs[1].X);
		engine.fpx.fp2mul_mont(xs[1].Z, array, Rs[1].Z);
		engine.fpx.fp2add(Rs[1].Z, Rs[1].Z, Rs[1].Z);
		engine.fpx.fp2inv_mont_bingcd(Rs[1].Z);
		engine.fpx.fp2mul_mont(Rs[1].X, Rs[1].Z, Rs[1].X);
		engine.fpx.fp2mul_mont(Rs[1].Y, Rs[1].Z, Rs[1].Y);
	}

	internal void BuildOrdinary2nBasis_dual(ulong[][] A, ulong[][][][] Ds, PointProjFull[] Rs, byte[] qnr, byte[] ind)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PointProj[] array3 = new PointProj[3]
		{
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD)
		};
		BuildEntangledXonly(A, array3, qnr, ind);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0].Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[1].Z[0]);
		for (uint num = 0u; num < engine.param.MAX_Bob; num++)
		{
			engine.isogeny.Eval3Isog(array3[0], Ds[engine.param.MAX_Bob - 1 - num]);
			engine.isogeny.Eval3Isog(array3[1], Ds[engine.param.MAX_Bob - 1 - num]);
			engine.isogeny.Eval3Isog(array3[2], Ds[engine.param.MAX_Bob - 1 - num]);
		}
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array2[0]);
		engine.fpx.fpaddPRIME(array2[0], array2[0], array);
		engine.fpx.fpaddPRIME(array, array, array2[0]);
		engine.fpx.fpaddPRIME(array2[0], array, array2[0]);
		engine.isogeny.CompleteMPoint(array2, array3[0], Rs[0]);
		RecoverY(array2, array3, Rs);
	}

	internal void FullIsogeny_B_dual(byte[] PrivateKeyB, ulong[][][][] Ds, ulong[][] A)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_BOB];
		ulong[][] xP = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] xQ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array5 = new uint[engine.param.MAX_INT_POINTS_BOB];
		ulong[] array6 = new ulong[engine.param.NWORDS_ORDER];
		init_basis(engine.param.B_gen, xP, xQ, array2);
		engine.fpx.fpcopy(engine.param.XQB3, 0L, pointProj2.X[0]);
		engine.fpx.fpcopy(engine.param.XQB3, engine.param.NWORDS_FIELD, pointProj2.X[1]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj2.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0]);
		engine.fpx.fp2add(array3, array3, array3);
		engine.fpx.fp2add(array3, array3, array4);
		engine.fpx.fp2add(array3, array4, A);
		engine.fpx.fp2add(array4, array4, array3);
		engine.fpx.decode_to_digits(PrivateKeyB, 0u, array6, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(xP, xQ, array2, array6, engine.param.BOB, pointProj, A);
		num = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Bob; num4++)
		{
			uint num5;
			for (; num < engine.param.MAX_Bob - num4; num += num5)
			{
				array[num2] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array[num2].X);
				engine.fpx.fp2copy(pointProj.Z, array[num2].Z);
				array5[num2++] = num;
				num5 = engine.param.strat_Bob[num3++];
				engine.isogeny.XTplE(pointProj, pointProj, array4, array3, num5);
			}
			engine.isogeny.Get3Isog(pointProj, array4, array3, coeff);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval3Isog(array[num6], coeff);
			}
			engine.isogeny.Eval3Isog(pointProj2, coeff);
			engine.fpx.fp2sub(pointProj2.X, pointProj2.Z, Ds[num4 - 1][0]);
			engine.fpx.fp2add(pointProj2.X, pointProj2.Z, Ds[num4 - 1][1]);
			engine.fpx.fp2copy(array[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num2 - 1].Z, pointProj.Z);
			num = array5[num2 - 1];
			num2--;
		}
		engine.isogeny.Get3Isog(pointProj, array4, array3, coeff);
		engine.isogeny.Eval3Isog(pointProj2, coeff);
		engine.fpx.fp2sub(pointProj2.X, pointProj2.Z, Ds[engine.param.MAX_Bob - 1][0]);
		engine.fpx.fp2add(pointProj2.X, pointProj2.Z, Ds[engine.param.MAX_Bob - 1][1]);
		engine.fpx.fp2add(array3, array4, A);
		engine.fpx.fp2sub(array3, array4, array3);
		engine.fpx.fp2inv_mont_bingcd(array3);
		engine.fpx.fp2mul_mont(array3, A, A);
		engine.fpx.fp2add(A, A, A);
	}

	internal void Dlogs2_dual(ulong[][][] f, int[] D, ulong[] d0, ulong[] c0, ulong[] d1, ulong[] c1)
	{
		solve_dlog(f[0], D, d0, 2u);
		solve_dlog(f[2], D, c0, 2u);
		solve_dlog(f[1], D, d1, 2u);
		solve_dlog(f[3], D, c1, 2u);
		engine.fpx.mp_sub(engine.param.Alice_order, c0, c0, engine.param.NWORDS_ORDER);
		engine.fpx.mp_sub(engine.param.Alice_order, c1, c1, engine.param.NWORDS_ORDER);
	}

	internal void BuildEntangledXonly_Decomp(ulong[][] A, PointProj[] R, uint qnr, uint ind)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] b = ((qnr != 1) ? engine.param.table_v_qr : engine.param.table_v_qnr);
		if (ind >= engine.param.TABLE_V_LEN / 2)
		{
			ind = 0u;
		}
		engine.fpx.fp2mul_mont(A, b, ind * 2, R[0].X);
		engine.fpx.fp2neg(R[0].X);
		engine.fpx.fp2add(R[0].X, A, array2);
		engine.fpx.fp2mul_mont(R[0].X, array2, array2);
		engine.fpx.fpaddPRIME(array2[0], engine.param.Montgomery_one, array2[0]);
		engine.fpx.fp2mul_mont(R[0].X, array2, array2);
		if (qnr == 1)
		{
			engine.fpx.fpcopy(engine.param.table_r_qnr[ind], 0L, array[0]);
		}
		else
		{
			engine.fpx.fpcopy(engine.param.table_r_qr[ind], 0L, array[0]);
		}
		engine.fpx.fp2add(R[0].X, A, R[1].X);
		engine.fpx.fp2neg(R[1].X);
		engine.fpx.fp2sub(R[0].X, R[1].X, R[2].Z);
		engine.fpx.fp2sqr_mont(R[2].Z, R[2].Z);
		engine.fpx.fpcopy(array[0], 0L, array[1]);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array[0], array[0]);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2mul_mont(array2, array, R[2].X);
	}

	internal void PKBDecompression_extended(byte[] SecretKeyA, uint SecretKeyAOffset, byte[] CompressedPKB, PointProj R, ulong[][] A, byte[] tphiBKA_t, uint tphiBKA_tOffset)
	{
		ulong num = ulong.MaxValue;
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array4 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array5 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array6 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array7 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array8 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array9 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array10 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array11 = new ulong[engine.param.NWORDS_ORDER];
		PointProj[] array12 = new PointProj[3]
		{
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD),
			new PointProj(engine.param.NWORDS_FIELD)
		};
		num >>= (int)(engine.param.MAXBITS_ORDER - engine.param.OALICE_BITS);
		engine.fpx.fp2_decode(CompressedPKB, A, 4 * engine.param.ORDER_A_ENCODED_BYTES);
		uint qnr = (uint)(CompressedPKB[4 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] & 1);
		uint ind = CompressedPKB[4 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 1];
		BuildEntangledXonly_Decomp(A, array12, qnr, ind);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array12[0].Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array12[1].Z[0]);
		engine.fpx.fpaddPRIME(A[0], engine.param.Montgomery_one, array[0]);
		engine.fpx.fpcopy(A[1], 0L, array[1]);
		engine.fpx.fpaddPRIME(array[0], engine.param.Montgomery_one, array[0]);
		engine.fpx.fp2div2(array, array);
		engine.fpx.fp2div2(array, array);
		engine.fpx.decode_to_digits(SecretKeyA, SecretKeyAOffset, array7, engine.param.SECRETKEY_A_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.decode_to_digits(CompressedPKB, 0u, array8, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.decode_to_digits(CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, array10, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.decode_to_digits(CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, array9, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.fpx.decode_to_digits(CompressedPKB, 3 * engine.param.ORDER_A_ENCODED_BYTES, array11, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		if ((array8[0] & 1) == 1)
		{
			engine.fpx.multiply(array7, array11, array3, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array3, array10, array3, engine.param.NWORDS_ORDER);
			array3[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.multiply(array7, array9, array4, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array4, array8, array4, engine.param.NWORDS_ORDER);
			array4[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.inv_mod_orderA(array4, array5);
			engine.fpx.multiply(array3, array5, array6, engine.param.NWORDS_ORDER);
			array6[engine.param.NWORDS_ORDER - 1] &= num;
			Ladder3pt_dual(array12, array6, engine.param.ALICE, R, array);
		}
		else
		{
			engine.fpx.multiply(array7, array9, array3, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array3, array8, array3, engine.param.NWORDS_ORDER);
			array3[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.multiply(array7, array11, array4, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array4, array10, array4, engine.param.NWORDS_ORDER);
			array4[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.inv_mod_orderA(array4, array5);
			engine.fpx.multiply(array5, array3, array6, engine.param.NWORDS_ORDER);
			array6[engine.param.NWORDS_ORDER - 1] &= num;
			engine.isogeny.SwapPoints(array12[0], array12[1], ulong.MaxValue);
			Ladder3pt_dual(array12, array6, engine.param.ALICE, R, array);
		}
		engine.fpx.fp2div2(A, array2);
		engine.isogeny.XTplEFast(R, R, array2, engine.param.OBOB_EXPON);
		engine.fpx.fp2_encode(R.X, tphiBKA_t, tphiBKA_tOffset);
		engine.fpx.fp2_encode(R.Z, tphiBKA_t, tphiBKA_tOffset + engine.param.FP2_ENCODED_BYTES);
		engine.fpx.encode_to_bytes(array5, tphiBKA_t, tphiBKA_tOffset + 2 * engine.param.FP2_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
	}

	internal void Compress_PKB_dual_extended(ulong[] d0, ulong[] c0, ulong[] d1, ulong[] c1, ulong[][] A, byte[] qnr, byte[] ind, byte[] CompressedPKB)
	{
		ulong num = ulong.MaxValue;
		ulong[] array = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_ORDER];
		num >>= (int)(engine.param.MAXBITS_ORDER - engine.param.OALICE_BITS);
		engine.fpx.multiply(c0, d1, array, engine.param.NWORDS_ORDER);
		engine.fpx.multiply(c1, d0, array2, engine.param.NWORDS_ORDER);
		engine.fpx.Montgomery_neg(array2, engine.param.Alice_order);
		engine.fpx.mp_add(array, array2, array2, engine.param.NWORDS_ORDER);
		array2[engine.param.NWORDS_ORDER - 1] &= num;
		engine.fpx.inv_mod_orderA(array2, array3);
		engine.fpx.multiply(d1, array3, array, engine.param.NWORDS_ORDER);
		array[engine.param.NWORDS_ORDER - 1] &= num;
		engine.fpx.encode_to_bytes(array, CompressedPKB, 0u, engine.param.ORDER_A_ENCODED_BYTES);
		engine.fpx.Montgomery_neg(d0, engine.param.Alice_order);
		engine.fpx.multiply(d0, array3, array, engine.param.NWORDS_ORDER);
		array[engine.param.NWORDS_ORDER - 1] &= num;
		engine.fpx.encode_to_bytes(array, CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
		engine.fpx.Montgomery_neg(c1, engine.param.Alice_order);
		engine.fpx.multiply(c1, array3, array, engine.param.NWORDS_ORDER);
		array[engine.param.NWORDS_ORDER - 1] &= num;
		engine.fpx.encode_to_bytes(array, CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
		engine.fpx.multiply(c0, array3, array, engine.param.NWORDS_ORDER);
		array[engine.param.NWORDS_ORDER - 1] &= num;
		engine.fpx.encode_to_bytes(array, CompressedPKB, 3 * engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
		engine.fpx.fp2_encode(A, CompressedPKB, 4 * engine.param.ORDER_A_ENCODED_BYTES);
		CompressedPKB[4 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] = qnr[0];
		CompressedPKB[4 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 1] = ind[0];
	}

	internal void PKBDecompression(byte[] SecretKeyA, uint SecretKeyAOffset, byte[] CompressedPKB, PointProj R, ulong[][] A)
	{
		ulong num = ulong.MaxValue;
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[] array2 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array4 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array5 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array6 = new ulong[engine.param.NWORDS_ORDER];
		PointProj[] array7 = new PointProj[3];
		num >>= (int)(engine.param.MAXBITS_ORDER - engine.param.OALICE_BITS);
		array4[0] = 1uL;
		engine.fpx.fp2_decode(CompressedPKB, A, 3 * engine.param.ORDER_A_ENCODED_BYTES);
		uint num2 = (uint)(CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] >> 7);
		uint qnr = (uint)(CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] & 1);
		uint ind = CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 1];
		BuildEntangledXonly_Decomp(A, array7, qnr, ind);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array7[0].Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array7[1].Z[0]);
		engine.fpx.fpaddPRIME(A[0], engine.param.Montgomery_one, array[0]);
		engine.fpx.fpcopy(A[1], 0L, array[1]);
		engine.fpx.fpaddPRIME(array[0], engine.param.Montgomery_one, array[0]);
		engine.fpx.fp2div2(array, array);
		engine.fpx.fp2div2(array, array);
		engine.fpx.decode_to_digits(SecretKeyA, SecretKeyAOffset, array5, engine.param.SECRETKEY_A_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.SwapPoints(array7[0], array7[1], 0uL - (ulong)num2);
		if (num2 == 0)
		{
			engine.fpx.decode_to_digits(CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array5, array6, array2, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array2, array4, array2, engine.param.NWORDS_ORDER);
			array2[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.inv_mod_orderA(array2, array3);
			engine.fpx.decode_to_digits(CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array5, array6, array2, engine.param.NWORDS_ORDER);
			engine.fpx.decode_to_digits(CompressedPKB, 0u, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array6, array2, array2, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array2, array3, array4, engine.param.NWORDS_ORDER);
			array4[engine.param.NWORDS_ORDER - 1] &= num;
			Ladder3pt_dual(array7, array4, engine.param.ALICE, R, array);
		}
		else
		{
			engine.fpx.decode_to_digits(CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array5, array6, array2, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array2, array4, array2, engine.param.NWORDS_ORDER);
			array2[engine.param.NWORDS_ORDER - 1] &= num;
			engine.fpx.inv_mod_orderA(array2, array3);
			engine.fpx.decode_to_digits(CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array5, array6, array2, engine.param.NWORDS_ORDER);
			engine.fpx.decode_to_digits(CompressedPKB, 0u, array6, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
			engine.fpx.mp_add(array6, array2, array2, engine.param.NWORDS_ORDER);
			engine.fpx.multiply(array2, array3, array4, engine.param.NWORDS_ORDER);
			array4[engine.param.NWORDS_ORDER - 1] &= num;
			Ladder3pt_dual(array7, array4, engine.param.ALICE, R, array);
		}
		engine.fpx.fp2div2(A, array);
		engine.isogeny.XTplEFast(R, R, array, engine.param.OBOB_EXPON);
	}

	internal void Compress_PKB_dual(ulong[] d0, ulong[] c0, ulong[] d1, ulong[] c1, ulong[][] A, byte[] qnr, byte[] ind, byte[] CompressedPKB)
	{
		ulong[] array = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		if ((d1[0] & 1) == 1)
		{
			engine.fpx.inv_mod_orderA(d1, array2);
			engine.fpx.Montgomery_neg(d0, engine.param.Alice_order);
			engine.fpx.multiply(d0, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, 0u, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			engine.fpx.Montgomery_neg(c1, engine.param.Alice_order);
			engine.fpx.multiply(c1, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[2 * engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			engine.fpx.multiply(c0, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] = 0;
		}
		else
		{
			engine.fpx.inv_mod_orderA(d0, array2);
			engine.fpx.Montgomery_neg(d1, engine.param.Alice_order);
			engine.fpx.multiply(d1, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, 0u, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			engine.fpx.multiply(c1, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[2 * engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			engine.fpx.Montgomery_neg(c0, engine.param.Alice_order);
			engine.fpx.multiply(c0, array2, array, engine.param.NWORDS_ORDER);
			engine.fpx.encode_to_bytes(array, CompressedPKB, 2 * engine.param.ORDER_A_ENCODED_BYTES, engine.param.ORDER_A_ENCODED_BYTES);
			CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES - 1] &= (byte)engine.param.MASK_ALICE;
			CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] = 128;
		}
		engine.fpx.fp2_encode(A, CompressedPKB, 3 * engine.param.ORDER_A_ENCODED_BYTES);
		CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES] |= qnr[0];
		CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 1] = ind[0];
		CompressedPKB[3 * engine.param.ORDER_A_ENCODED_BYTES + engine.param.FP2_ENCODED_BYTES + 2] = 0;
	}

	internal uint EphemeralKeyGeneration_B_extended(byte[] PrivateKeyB, byte[] CompressedPKB, uint sike)
	{
		byte[] qnr = new byte[1];
		byte[] ind = new byte[1];
		int[] d = new int[engine.param.DLEN_2];
		ulong[] c = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] c2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] d3 = new ulong[engine.param.NWORDS_ORDER];
		ulong[][][][] ds = SikeUtilities.InitArray(engine.param.MAX_Bob, 2u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] array = SikeUtilities.InitArray(4u, 2u, engine.param.NWORDS_FIELD);
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PointProjFull[] array2 = new PointProjFull[2]
		{
			new PointProjFull(engine.param.NWORDS_FIELD),
			new PointProjFull(engine.param.NWORDS_FIELD)
		};
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		FullIsogeny_B_dual(PrivateKeyB, ds, a);
		BuildOrdinary2nBasis_dual(a, ds, array2, qnr, ind);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array2[0].X[0], array2[0].X[0]);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array2[0].X[0], array2[0].X[0]);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array2[1].X[0], array2[1].X[0]);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, array2[1].X[0], array2[1].X[0]);
		Fpx fpx = engine.fpx;
		ulong[] a_basis_zero = engine.param.A_basis_zero;
		_ = engine.param.NWORDS_FIELD;
		fpx.fpcopy(a_basis_zero, 0L, pointProj.X[0]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, engine.param.NWORDS_FIELD, pointProj.X[1]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 2 * engine.param.NWORDS_FIELD, pointProj.Z[0]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 3 * engine.param.NWORDS_FIELD, pointProj.Z[1]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 4 * engine.param.NWORDS_FIELD, pointProj2.X[0]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 5 * engine.param.NWORDS_FIELD, pointProj2.X[1]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 6 * engine.param.NWORDS_FIELD, pointProj2.Z[0]);
		engine.fpx.fpcopy(engine.param.A_basis_zero, 7 * engine.param.NWORDS_FIELD, pointProj2.Z[1]);
		Tate2_pairings(pointProj, pointProj2, array2, array);
		engine.fpx.fp2correction(array[0]);
		engine.fpx.fp2correction(array[1]);
		engine.fpx.fp2correction(array[2]);
		engine.fpx.fp2correction(array[3]);
		Dlogs2_dual(array, d, d2, c, d3, c2);
		if (sike == 1)
		{
			Compress_PKB_dual_extended(d2, c, d3, c2, a, qnr, ind, CompressedPKB);
		}
		else
		{
			Compress_PKB_dual(d2, c, d3, c2, a, qnr, ind, CompressedPKB);
		}
		return 0u;
	}

	internal uint EphemeralKeyGeneration_B(byte[] PrivateKeyB, byte[] CompressedPKB)
	{
		return EphemeralKeyGeneration_B_extended(PrivateKeyB, CompressedPKB, 0u);
	}

	internal uint EphemeralSecretAgreement_A_extended(byte[] PrivateKeyA, uint PrivateKeyAOffset, byte[] PKB, byte[] SharedSecretA, uint sike)
	{
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array = new uint[engine.param.MAX_INT_POINTS_ALICE];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array4 = new PointProj[engine.param.MAX_INT_POINTS_ALICE];
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(5u, 2u, engine.param.NWORDS_FIELD);
		if (sike == 1)
		{
			PKBDecompression_extended(PrivateKeyA, PrivateKeyAOffset, PKB, pointProj, a, SharedSecretA, engine.param.FP2_ENCODED_BYTES);
		}
		else
		{
			PKBDecompression(PrivateKeyA, PrivateKeyAOffset, PKB, pointProj, a);
		}
		engine.fpx.fp2copy(a, array6);
		engine.fpx.fpaddPRIME(engine.param.Montgomery_one, engine.param.Montgomery_one, array3[0]);
		engine.fpx.fp2add(array6, array3, array2);
		engine.fpx.fpaddPRIME(array3[0], array3[0], array3[0]);
		if (engine.param.OALICE_BITS % 2 == 1)
		{
			PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
			engine.isogeny.XDblE(pointProj, pointProj2, array2, array3, engine.param.OALICE_BITS - 1);
			engine.isogeny.Get2Isog(pointProj2, array2, array3);
			engine.isogeny.Eval2Isog(pointProj, pointProj2);
		}
		num2 = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Alice; num4++)
		{
			uint num5;
			for (; num2 < engine.param.MAX_Alice - num4; num2 += num5)
			{
				array4[num3] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array4[num3].X);
				engine.fpx.fp2copy(pointProj.Z, array4[num3].Z);
				array[num3++] = num2;
				num5 = engine.param.strat_Alice[num++];
				engine.isogeny.XDblE(pointProj, pointProj, array2, array3, 2 * num5);
			}
			engine.isogeny.Get4Isog(pointProj, array2, array3, coeff);
			for (uint num6 = 0u; num6 < num3; num6++)
			{
				engine.isogeny.Eval4Isog(array4[num6], coeff);
			}
			engine.fpx.fp2copy(array4[num3 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array4[num3 - 1].Z, pointProj.Z);
			num2 = array[num3 - 1];
			num3--;
		}
		engine.isogeny.Get4Isog(pointProj, array2, array3, coeff);
		engine.fpx.fp2add(array2, array2, array2);
		engine.fpx.fp2sub(array2, array3, array2);
		engine.fpx.fp2add(array2, array2, array2);
		engine.isogeny.JInv(array2, array3, array5);
		engine.fpx.fp2_encode(array5, SharedSecretA, 0u);
		return 0u;
	}

	private uint EphemeralSecretAgreement_A(byte[] PrivateKeyA, uint PrivateKeyAOffset, byte[] PKB, byte[] SharedSecretA)
	{
		return EphemeralSecretAgreement_A_extended(PrivateKeyA, PrivateKeyAOffset, PKB, SharedSecretA, 0u);
	}

	internal byte validate_ciphertext(byte[] ephemeralsk_, byte[] CompressedPKB, byte[] xKA, uint xKAOffset, byte[] tphiBKA_t, uint tphiBKA_tOffset)
	{
		PointProj[] array = new PointProj[3];
		PointProj[] array2 = new PointProj[engine.param.MAX_INT_POINTS_BOB];
		array[0] = new PointProj(engine.param.NWORDS_FIELD);
		array[1] = new PointProj(engine.param.NWORDS_FIELD);
		array[2] = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		ulong[][] xP = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] xQ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array8 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array9 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array10 = new uint[engine.param.MAX_INT_POINTS_BOB];
		ulong[] array11 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array12 = new ulong[engine.param.NWORDS_ORDER];
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array9[0]);
		init_basis(engine.param.B_gen, xP, xQ, array3);
		engine.fpx.fp2_decode(xKA, array[0].X, xKAOffset);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array[0].Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array4[0]);
		engine.fpx.fp2add(array4, array4, array4);
		engine.fpx.fp2add(array4, array4, array5);
		engine.fpx.fp2add(array4, array5, array6);
		engine.fpx.fp2add(array5, array5, array4);
		engine.fpx.decode_to_digits(ephemeralsk_, 0u, array12, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(xP, xQ, array3, array12, engine.param.BOB, pointProj, array6);
		num = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Bob; num4++)
		{
			uint num5;
			for (; num < engine.param.MAX_Bob - num4; num += num5)
			{
				array2[num2] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array2[num2].X);
				engine.fpx.fp2copy(pointProj.Z, array2[num2].Z);
				array10[num2++] = num;
				num5 = engine.param.strat_Bob[num3++];
				engine.isogeny.XTplE(pointProj, pointProj, array5, array4, num5);
			}
			engine.isogeny.Get3Isog(pointProj, array5, array4, coeff);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval3Isog(array2[num6], coeff);
			}
			engine.isogeny.Eval3Isog(array[0], coeff);
			engine.fpx.fp2copy(array2[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array2[num2 - 1].Z, pointProj.Z);
			num = array10[num2 - 1];
			num2--;
		}
		engine.isogeny.Get3Isog(pointProj, array5, array4, coeff);
		engine.isogeny.Eval3Isog(array[0], coeff);
		engine.fpx.fp2_decode(CompressedPKB, array6, 4 * engine.param.ORDER_A_ENCODED_BYTES);
		engine.fpx.fp2_decode(tphiBKA_t, pointProj2.X, tphiBKA_tOffset);
		engine.fpx.fp2_decode(tphiBKA_t, pointProj2.Z, tphiBKA_tOffset + engine.param.FP2_ENCODED_BYTES);
		engine.fpx.decode_to_digits(tphiBKA_t, tphiBKA_tOffset + 2 * engine.param.FP2_ENCODED_BYTES, array11, engine.param.ORDER_A_ENCODED_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.Ladder(array[0], array11, array6, engine.param.OALICE_BITS, pointProj);
		engine.fpx.fp2mul_mont(pointProj.X, pointProj2.Z, array7);
		engine.fpx.fp2mul_mont(pointProj.Z, pointProj2.X, array8);
		return engine.fpx.cmp_f2elm(array7, array8);
	}

	internal void solve_dlog(ulong[][] r, int[] D, ulong[] d, uint ell)
	{
		switch (ell)
		{
		case 2u:
			if (engine.param.OALICE_BITS % (int)engine.param.W_2 == 0L)
			{
				Traverse_w_div_e_fullsigned(r, 0u, 0u, engine.param.PLEN_2 - 1, engine.param.ph2_path, engine.param.ph2_T, D, engine.param.DLEN_2, engine.param.ELL2_W, engine.param.W_2);
			}
			else
			{
				Traverse_w_notdiv_e_fullsigned(r, 0u, 0u, engine.param.PLEN_2 - 1, engine.param.ph2_path, engine.param.ph2_T1, engine.param.ph2_T2, D, engine.param.DLEN_2, ell, engine.param.ELL2_W, engine.param.ELL2_EMODW, engine.param.W_2, engine.param.OALICE_BITS);
			}
			from_base(D, d, engine.param.DLEN_2, engine.param.ELL2_W);
			break;
		case 3u:
			if (engine.param.OBOB_EXPON % (int)engine.param.W_3 == 0L)
			{
				Traverse_w_div_e_fullsigned(r, 0u, 0u, engine.param.PLEN_3 - 1, engine.param.ph3_path, engine.param.ph3_T, D, engine.param.DLEN_3, engine.param.ELL3_W, engine.param.W_3);
			}
			else
			{
				Traverse_w_notdiv_e_fullsigned(r, 0u, 0u, engine.param.PLEN_3 - 1, engine.param.ph3_path, engine.param.ph3_T1, engine.param.ph3_T2, D, engine.param.DLEN_3, ell, engine.param.ELL3_W, engine.param.ELL3_EMODW, engine.param.W_3, engine.param.OBOB_EXPON);
			}
			from_base(D, d, engine.param.DLEN_3, engine.param.ELL3_W);
			break;
		}
	}

	private void from_base(int[] D, ulong[] r, uint Dlen, uint baseNum)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[engine.param.NWORDS_ORDER];
		array[0] = baseNum;
		if (D[Dlen - 1] < 0)
		{
			array2[0] = (ulong)(-D[Dlen - 1] * (int)array[0]);
			if ((baseNum & 1) == 0)
			{
				engine.fpx.Montgomery_neg(array2, engine.param.Alice_order);
				engine.fpx.copy_words(array2, r, engine.param.NWORDS_ORDER);
			}
			else
			{
				engine.fpx.mp_sub(engine.param.Bob_order, array2, r, engine.param.NWORDS_ORDER);
			}
		}
		else
		{
			r[0] = (uint)D[Dlen - 1] * array[0];
		}
		for (uint num = Dlen - 2; num >= 1; num--)
		{
			uint num2 = baseNum;
			Arrays.Fill(array2, 0uL);
			if (D[num] < 0)
			{
				array2[0] = (ulong)(-D[num]);
				if ((baseNum & 1) == 0)
				{
					engine.fpx.Montgomery_neg(array2, engine.param.Alice_order);
				}
				else
				{
					engine.fpx.mp_sub(engine.param.Bob_order, array2, array2, engine.param.NWORDS_ORDER);
				}
			}
			else
			{
				array2[0] = (uint)D[num];
			}
			engine.fpx.mp_add(r, array2, r, engine.param.NWORDS_ORDER);
			if ((baseNum & 1) != 0 && !engine.fpx.is_orderelm_lt(r, engine.param.Bob_order))
			{
				engine.fpx.mp_sub(r, engine.param.Bob_order, r, engine.param.NWORDS_ORDER);
			}
			if ((baseNum & 1) == 0)
			{
				while (num2 > 1)
				{
					engine.fpx.mp_add(r, r, r, engine.param.NWORDS_ORDER);
					num2 /= 2;
				}
			}
			else
			{
				while (num2 > 1)
				{
					Arrays.Fill(array3, 0uL);
					engine.fpx.mp_add(r, r, array3, engine.param.NWORDS_ORDER);
					if (!engine.fpx.is_orderelm_lt(array3, engine.param.Bob_order))
					{
						engine.fpx.mp_sub(array3, engine.param.Bob_order, array3, engine.param.NWORDS_ORDER);
					}
					engine.fpx.mp_add(r, array3, r, engine.param.NWORDS_ORDER);
					if (!engine.fpx.is_orderelm_lt(r, engine.param.Bob_order))
					{
						engine.fpx.mp_sub(r, engine.param.Bob_order, r, engine.param.NWORDS_ORDER);
					}
					num2 /= 3;
				}
			}
		}
		Arrays.Fill(array2, 0uL);
		if (D[0] < 0)
		{
			array2[0] = (ulong)(-D[0]);
			if ((baseNum & 1) == 0)
			{
				engine.fpx.Montgomery_neg(array2, engine.param.Alice_order);
			}
			else
			{
				engine.fpx.mp_sub(engine.param.Bob_order, array2, array2, engine.param.NWORDS_ORDER);
			}
		}
		else
		{
			array2[0] = (uint)D[0];
		}
		engine.fpx.mp_add(r, array2, r, engine.param.NWORDS_ORDER);
		if ((baseNum & 1) != 0 && !engine.fpx.is_orderelm_lt(r, engine.param.Bob_order))
		{
			engine.fpx.mp_sub(r, engine.param.Bob_order, r, engine.param.NWORDS_ORDER);
		}
	}

	internal void Traverse_w_notdiv_e_fullsigned(ulong[][] r, uint j, uint k, uint z, uint[] P, ulong[] CT1, ulong[] CT2, int[] D, uint Dlen, uint ell, uint ellw, uint ell_emodw, uint w, uint e)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		if (z > 1)
		{
			uint num = P[z];
			engine.fpx.fp2copy(r, array);
			uint num2 = ((j != 0) ? (w * (z - num)) : (e % w + w * (z - num - 1)));
			for (uint num3 = 0u; num3 < num2; num3++)
			{
				if ((ell & 1) == 0)
				{
					engine.fpx.sqr_Fp2_cycl(array, engine.param.Montgomery_one);
				}
				else
				{
					engine.fpx.cube_Fp2_cycl(array, engine.param.Montgomery_one);
				}
			}
			Traverse_w_notdiv_e_fullsigned(array, j + (z - num), k, num, P, CT1, CT2, D, Dlen, ell, ellw, ell_emodw, w, e);
			engine.fpx.fp2copy(r, array);
			for (uint num4 = k; num4 < k + num; num4++)
			{
				if (D[num4] == 0)
				{
					continue;
				}
				if (j != 0)
				{
					if (D[num4] < 0)
					{
						engine.fpx.fp2copy(CT2, (uint)(engine.param.NWORDS_FIELD * (2 * (j + num4) * (ellw / 2) + 2 * (-D[num4] - 1))), array2);
						engine.fpx.fpnegPRIME(array2[1]);
						engine.fpx.fp2mul_mont(array, array2, array);
					}
					else
					{
						engine.fpx.fp2mul_mont(array, CT2, (uint)(engine.param.NWORDS_FIELD * (2 * ((j + num4) * (ellw / 2) + (D[num4] - 1)))), array);
					}
				}
				else if (D[num4] < 0)
				{
					engine.fpx.fp2copy(CT1, (uint)(engine.param.NWORDS_FIELD * (2 * ((j + num4) * (ellw / 2) + (-D[num4] - 1)))), array2);
					engine.fpx.fpnegPRIME(array2[1]);
					engine.fpx.fp2mul_mont(array, array2, array);
				}
				else
				{
					engine.fpx.fp2mul_mont(array, CT1, (uint)(engine.param.NWORDS_FIELD * (2 * ((j + num4) * (ellw / 2) + (D[num4] - 1)))), array);
				}
			}
			Traverse_w_notdiv_e_fullsigned(array, j, k + num, z - num, P, CT1, CT2, D, Dlen, ell, ellw, ell_emodw, w, e);
			return;
		}
		engine.fpx.fp2copy(r, array);
		engine.fpx.fp2correction(array);
		if (engine.fpx.is_felm_zero(array[1]) && Fpx.subarrayEquals(array[0], engine.param.Montgomery_one, engine.param.NWORDS_FIELD))
		{
			D[k] = 0;
			return;
		}
		if (j != 0 || k != Dlen - 1)
		{
			for (uint num5 = 1u; num5 <= ellw / 2; num5++)
			{
				if (Fpx.subarrayEquals(array, CT2, engine.param.NWORDS_FIELD * (2 * (ellw / 2) * (Dlen - 1) + 2 * (num5 - 1)), 2 * engine.param.NWORDS_FIELD))
				{
					D[k] = (int)(0 - num5);
					break;
				}
				engine.fpx.fp2copy(CT2, engine.param.NWORDS_FIELD * (2 * (ellw / 2 * (Dlen - 1) + (num5 - 1))), array2);
				engine.fpx.fpnegPRIME(array2[1]);
				engine.fpx.fpcorrectionPRIME(array2[1]);
				if (Fpx.subarrayEquals(array, array2, 2 * engine.param.NWORDS_FIELD))
				{
					D[k] = (int)num5;
					break;
				}
			}
			return;
		}
		for (uint num6 = 1u; num6 <= ell_emodw / 2; num6++)
		{
			if (Fpx.subarrayEquals(array, CT1, engine.param.NWORDS_FIELD * (2 * (ellw / 2) * (Dlen - 1) + 2 * (num6 - 1)), 2 * engine.param.NWORDS_FIELD))
			{
				D[k] = (int)(0 - num6);
				break;
			}
			engine.fpx.fp2copy(CT1, engine.param.NWORDS_FIELD * (2 * (ellw / 2 * (Dlen - 1) + (num6 - 1))), array2);
			engine.fpx.fpnegPRIME(array2[1]);
			engine.fpx.fpcorrectionPRIME(array2[1]);
			if (Fpx.subarrayEquals(array, array2, 2 * engine.param.NWORDS_FIELD))
			{
				D[k] = (int)num6;
				break;
			}
		}
	}

	internal void Traverse_w_div_e_fullsigned(ulong[][] r, uint j, uint k, uint z, uint[] P, ulong[] CT, int[] D, uint Dlen, uint ellw, uint w)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		if (z > 1)
		{
			uint num = P[z];
			engine.fpx.fp2copy(r, array);
			for (uint num2 = 0u; num2 < z - num; num2++)
			{
				if ((ellw & 1) == 0)
				{
					for (uint num3 = 0u; num3 < w; num3++)
					{
						engine.fpx.sqr_Fp2_cycl(array, engine.param.Montgomery_one);
					}
				}
				else
				{
					for (uint num4 = 0u; num4 < w; num4++)
					{
						engine.fpx.cube_Fp2_cycl(array, engine.param.Montgomery_one);
					}
				}
			}
			Traverse_w_div_e_fullsigned(array, j + (z - num), k, num, P, CT, D, Dlen, ellw, w);
			engine.fpx.fp2copy(r, array);
			for (uint num5 = k; num5 < k + num; num5++)
			{
				if (D[num5] != 0)
				{
					if (D[num5] < 0)
					{
						engine.fpx.fp2copy(CT, (uint)(engine.param.NWORDS_FIELD * (2 * ((j + num5) * (ellw / 2) + (-D[num5] - 1)))), array2);
						engine.fpx.fpnegPRIME(array2[1]);
						engine.fpx.fp2mul_mont(array, array2, array);
					}
					else
					{
						engine.fpx.fp2mul_mont(array, CT, (uint)(engine.param.NWORDS_FIELD * (2 * ((j + num5) * (ellw / 2) + (D[num5] - 1)))), array);
					}
				}
			}
			Traverse_w_div_e_fullsigned(array, j, k + num, z - num, P, CT, D, Dlen, ellw, w);
			return;
		}
		engine.fpx.fp2copy(r, array);
		engine.fpx.fp2correction(array);
		if (engine.fpx.is_felm_zero(array[1]) && Fpx.subarrayEquals(array[0], engine.param.Montgomery_one, engine.param.NWORDS_FIELD))
		{
			D[k] = 0;
			return;
		}
		for (uint num6 = 1u; num6 <= ellw / 2; num6++)
		{
			if (Fpx.subarrayEquals(array, CT, engine.param.NWORDS_FIELD * (2 * ((Dlen - 1) * (ellw / 2) + (num6 - 1))), 2 * engine.param.NWORDS_FIELD))
			{
				D[k] = (int)(0 - num6);
				break;
			}
			engine.fpx.fp2copy(CT, engine.param.NWORDS_FIELD * (2 * ((Dlen - 1) * (ellw / 2) + (num6 - 1))), array2);
			engine.fpx.fpnegPRIME(array2[1]);
			engine.fpx.fpcorrectionPRIME(array2[1]);
			if (Fpx.subarrayEquals(array, array2, 2 * engine.param.NWORDS_FIELD))
			{
				D[k] = (int)num6;
				break;
			}
		}
	}

	private void Tate3_pairings(PointProjFull[] Qj, ulong[][][] f)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array5 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array6 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array7 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array8 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array9 = new ulong[engine.param.NWORDS_FIELD];
		ulong[][][] array10 = SikeUtilities.InitArray(t_points, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] array11 = SikeUtilities.InitArray(2 * t_points, 2u, engine.param.NWORDS_FIELD);
		ulong[][] array12 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array13 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array14 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array15 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array16 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array17 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array18 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array19 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array20 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array21 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array12[0]);
		for (uint num = 0u; num < t_points; num++)
		{
			engine.fpx.fp2copy(array12, f[num]);
			engine.fpx.fp2copy(array12, f[num + t_points]);
			engine.fpx.fp2sqr_mont(Qj[num].X, array10[num]);
		}
		for (uint num2 = 0u; num2 < engine.param.OBOB_EXPON - 1; num2++)
		{
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2), array3, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2 + 1), array4, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2 + 2), array5, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2 + 3), array6, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2 + 4), array8, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * num2 + 5), array9, 0L, engine.param.NWORDS_FIELD);
			for (uint num3 = 0u; num3 < t_points; num3++)
			{
				engine.fpx.fpmul_mont(Qj[num3].X[0], array3, array13[0]);
				engine.fpx.fpmul_mont(Qj[num3].X[1], array3, array13[1]);
				engine.fpx.fpmul_mont(Qj[num3].X[0], array4, array15[0]);
				engine.fpx.fpmul_mont(Qj[num3].X[1], array4, array15[1]);
				engine.fpx.fpaddPRIME(array10[num3][0], array8, array17[0]);
				engine.fpx.fpcopy(array10[num3][1], 0L, array17[1]);
				engine.fpx.fpmul_mont(Qj[num3].X[0], array9, array18[0]);
				engine.fpx.fpmul_mont(Qj[num3].X[1], array9, array18[1]);
				engine.fpx.fp2sub(array13, Qj[num3].Y, array14);
				engine.fpx.fpaddPRIME(array14[0], array5, array14[0]);
				engine.fpx.fp2sub(array15, Qj[num3].Y, array16);
				engine.fpx.fpaddPRIME(array16[0], array6, array16[0]);
				engine.fpx.fp2mul_mont(array14, array16, array19);
				engine.fpx.fp2sub(array17, array18, array20);
				engine.fpx.fp2_conj(array20, array20);
				engine.fpx.fp2mul_mont(array19, array20, array19);
				engine.fpx.fp2sqr_mont(f[num3], array21);
				engine.fpx.fp2mul_mont(f[num3], array21, f[num3]);
				engine.fpx.fp2mul_mont(f[num3], array19, f[num3]);
				engine.fpx.fpsubPRIME(array13[1], Qj[num3].Y[0], array14[0]);
				engine.fpx.fpaddPRIME(array13[0], Qj[num3].Y[1], array14[1]);
				engine.fpx.fpnegPRIME(array14[1]);
				engine.fpx.fpaddPRIME(array14[1], array5, array14[1]);
				engine.fpx.fpsubPRIME(array15[1], Qj[num3].Y[0], array16[0]);
				engine.fpx.fpaddPRIME(array15[0], Qj[num3].Y[1], array16[1]);
				engine.fpx.fpnegPRIME(array16[1]);
				engine.fpx.fpaddPRIME(array16[1], array6, array16[1]);
				engine.fpx.fp2mul_mont(array14, array16, array19);
				engine.fpx.fp2add(array17, array18, array20);
				engine.fpx.fp2_conj(array20, array20);
				engine.fpx.fp2mul_mont(array19, array20, array19);
				engine.fpx.fp2sqr_mont(f[num3 + t_points], array21);
				engine.fpx.fp2mul_mont(f[num3 + t_points], array21, f[num3 + t_points]);
				engine.fpx.fp2mul_mont(f[num3 + t_points], array19, f[num3 + t_points]);
			}
		}
		for (uint num4 = 0u; num4 < t_points; num4++)
		{
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * (engine.param.OBOB_EXPON - 1)), array, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * (engine.param.OBOB_EXPON - 1) + 1), array2, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * (engine.param.OBOB_EXPON - 1) + 2), array3, 0L, engine.param.NWORDS_FIELD);
			Array.Copy(engine.param.T_tate3, engine.param.NWORDS_FIELD * (6 * (engine.param.OBOB_EXPON - 1) + 3), array7, 0L, engine.param.NWORDS_FIELD);
			engine.fpx.fpsubPRIME(Qj[num4].X[0], array, array13[0]);
			engine.fpx.fpcopy(Qj[num4].X[1], 0L, array13[1]);
			engine.fpx.fpmul_mont(array3, array13[0], array14[0]);
			engine.fpx.fpmul_mont(array3, array13[1], array14[1]);
			engine.fpx.fp2sub(array14, Qj[num4].Y, array15);
			engine.fpx.fpaddPRIME(array15[0], array2, array15[0]);
			engine.fpx.fp2mul_mont(array13, array15, array19);
			engine.fpx.fpsubPRIME(Qj[num4].X[0], array7, array20[0]);
			engine.fpx.fpcopy(Qj[num4].X[1], 0L, array20[1]);
			engine.fpx.fpnegPRIME(array20[1]);
			engine.fpx.fp2mul_mont(array19, array20, array19);
			engine.fpx.fp2sqr_mont(f[num4], array21);
			engine.fpx.fp2mul_mont(f[num4], array21, f[num4]);
			engine.fpx.fp2mul_mont(f[num4], array19, f[num4]);
			engine.fpx.fpaddPRIME(Qj[num4].X[0], array, array13[0]);
			engine.fpx.fpmul_mont(array3, array13[0], array14[0]);
			engine.fpx.fpsubPRIME(Qj[num4].Y[0], array14[1], array15[0]);
			engine.fpx.fpaddPRIME(Qj[num4].Y[1], array14[0], array15[1]);
			engine.fpx.fpsubPRIME(array15[1], array2, array15[1]);
			engine.fpx.fp2mul_mont(array13, array15, array19);
			engine.fpx.fpaddPRIME(Qj[num4].X[0], array7, array20[0]);
			engine.fpx.fp2mul_mont(array19, array20, array19);
			engine.fpx.fp2sqr_mont(f[num4 + t_points], array21);
			engine.fpx.fp2mul_mont(f[num4 + t_points], array21, f[num4 + t_points]);
			engine.fpx.fp2mul_mont(f[num4 + t_points], array19, f[num4 + t_points]);
		}
		engine.fpx.mont_n_way_inv(f, 2 * t_points, array11);
		for (uint num5 = 0u; num5 < 2 * t_points; num5++)
		{
			final_exponentiation_3_torsion(f[num5], array11[num5], f[num5]);
		}
	}

	private void final_exponentiation_3_torsion(ulong[][] f, ulong[][] finv, ulong[][] fout)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array);
		engine.fpx.fp2_conj(f, array2);
		engine.fpx.fp2mul_mont(array2, finv, array2);
		for (uint num = 0u; num < engine.param.OALICE_BITS; num++)
		{
			engine.fpx.sqr_Fp2_cycl(array2, array);
		}
		engine.fpx.fp2copy(array2, fout);
	}

	private void Tate2_pairings(PointProj P, PointProj Q, PointProjFull[] Qj, ulong[][][] f)
	{
		ulong[][][] array = SikeUtilities.InitArray(2 * t_points, 2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array2[0]);
		for (uint num = 0u; num < t_points; num++)
		{
			engine.fpx.fp2copy(array2, f[num]);
			engine.fpx.fp2copy(array2, f[num + t_points]);
		}
		ulong[][] x = P.X;
		ulong[][] z = P.Z;
		uint bOffset = 0u;
		uint num2 = 1u;
		ulong[] t_tate2_firststep_P = engine.param.T_tate2_firststep_P;
		ulong[] t_tate2_firststep_P2 = engine.param.T_tate2_firststep_P;
		engine.fpx.fpcopy(engine.param.T_tate2_firststep_P, 2 * engine.param.NWORDS_FIELD, array3[0]);
		engine.fpx.fpcopy(engine.param.T_tate2_firststep_P, 3 * engine.param.NWORDS_FIELD, array3[1]);
		for (uint num3 = 0u; num3 < t_points; num3++)
		{
			engine.fpx.fp2sub(Qj[num3].X, x, array4);
			engine.fpx.fp2sub(Qj[num3].Y, z, array5);
			engine.fpx.fp2mul_mont(array3, array4, array4);
			engine.fpx.fp2sub(array4, array5, array6);
			engine.fpx.fpsubPRIME(Qj[num3].X[0], engine.param.T_tate2_firststep_P, bOffset, array7[0]);
			engine.fpx.fpcopy(Qj[num3].X[1], 0L, array7[1]);
			engine.fpx.fpnegPRIME(array7[1]);
			engine.fpx.fp2mul_mont(array6, array7, array6);
			engine.fpx.fp2sqr_mont(f[num3], f[num3]);
			engine.fpx.fp2mul_mont(f[num3], array6, f[num3]);
		}
		uint num4 = 0u;
		uint bOffset2 = engine.param.NWORDS_FIELD;
		ulong[] array8 = t_tate2_firststep_P;
		ulong[] b = t_tate2_firststep_P2;
		for (uint num5 = 0u; num5 < engine.param.OALICE_BITS - 2; num5++)
		{
			t_tate2_firststep_P = engine.param.T_tate2_P;
			t_tate2_firststep_P2 = engine.param.T_tate2_P;
			ulong[] t_tate2_P = engine.param.T_tate2_P;
			bOffset = engine.param.NWORDS_FIELD * (3 * num5);
			num2 = engine.param.NWORDS_FIELD * (3 * num5 + 1);
			uint maOffset = engine.param.NWORDS_FIELD * (3 * num5 + 2);
			for (uint num6 = 0u; num6 < t_points; num6++)
			{
				engine.fpx.fpsubPRIME(array8, num4, Qj[num6].X[0], array4[1]);
				engine.fpx.fpmul_mont(t_tate2_P, maOffset, array4[1], array4[1]);
				engine.fpx.fpmul_mont(t_tate2_P, maOffset, Qj[num6].X[1], array4[0]);
				engine.fpx.fpsubPRIME(Qj[num6].Y[1], b, bOffset2, array5[1]);
				engine.fpx.fpsubPRIME(array4[1], array5[1], array6[1]);
				engine.fpx.fpsubPRIME(array4[0], Qj[num6].Y[0], array6[0]);
				engine.fpx.fpsubPRIME(Qj[num6].X[0], t_tate2_firststep_P, bOffset, array7[0]);
				engine.fpx.fpcopy(Qj[num6].X[1], 0L, array7[1]);
				engine.fpx.fpnegPRIME(array7[1]);
				engine.fpx.fp2mul_mont(array6, array7, array6);
				engine.fpx.fp2sqr_mont(f[num6], f[num6]);
				engine.fpx.fp2mul_mont(f[num6], array6, f[num6]);
			}
			array8 = t_tate2_firststep_P;
			b = t_tate2_firststep_P2;
			bOffset2 = num2;
			num4 = bOffset;
		}
		for (uint num7 = 0u; num7 < t_points; num7++)
		{
			engine.fpx.fpsubPRIME(Qj[num7].X[0], array8, num4, array6[0]);
			engine.fpx.fpcopy(Qj[num7].X[1], 0L, array6[1]);
			engine.fpx.fp2sqr_mont(f[num7], f[num7]);
			engine.fpx.fp2mul_mont(f[num7], array6, f[num7]);
		}
		x = Q.X;
		z = Q.Z;
		t_tate2_firststep_P = engine.param.T_tate2_firststep_Q;
		t_tate2_firststep_P2 = engine.param.T_tate2_firststep_Q;
		bOffset = 0u;
		num2 = engine.param.NWORDS_FIELD;
		engine.fpx.fpcopy(engine.param.T_tate2_firststep_Q, 2 * engine.param.NWORDS_FIELD, array3[0]);
		engine.fpx.fpcopy(engine.param.T_tate2_firststep_Q, 3 * engine.param.NWORDS_FIELD, array3[1]);
		for (uint num8 = 0u; num8 < t_points; num8++)
		{
			engine.fpx.fp2sub(Qj[num8].X, x, array4);
			engine.fpx.fp2sub(Qj[num8].Y, z, array5);
			engine.fpx.fp2mul_mont(array3, array4, array4);
			engine.fpx.fp2sub(array4, array5, array6);
			engine.fpx.fpsubPRIME(Qj[num8].X[0], t_tate2_firststep_P, bOffset, array7[0]);
			engine.fpx.fpcopy(Qj[num8].X[1], 0L, array7[1]);
			engine.fpx.fpnegPRIME(array7[1]);
			engine.fpx.fp2mul_mont(array6, array7, array6);
			engine.fpx.fp2sqr_mont(f[num8 + t_points], f[num8 + t_points]);
			engine.fpx.fp2mul_mont(f[num8 + t_points], array6, f[num8 + t_points]);
		}
		array8 = t_tate2_firststep_P;
		b = t_tate2_firststep_P2;
		bOffset2 = num2;
		num4 = bOffset;
		for (uint num9 = 0u; num9 < engine.param.OALICE_BITS - 2; num9++)
		{
			t_tate2_firststep_P = engine.param.T_tate2_Q;
			t_tate2_firststep_P2 = engine.param.T_tate2_Q;
			ulong[] t_tate2_P = engine.param.T_tate2_Q;
			bOffset = engine.param.NWORDS_FIELD * (3 * num9);
			num2 = engine.param.NWORDS_FIELD * (3 * num9 + 1);
			uint maOffset = engine.param.NWORDS_FIELD * (3 * num9 + 2);
			for (uint num10 = 0u; num10 < t_points; num10++)
			{
				engine.fpx.fpsubPRIME(Qj[num10].X[0], array8, num4, array4[0]);
				engine.fpx.fpmul_mont(t_tate2_P, maOffset, array4[0], array4[0]);
				engine.fpx.fpmul_mont(t_tate2_P, maOffset, Qj[num10].X[1], array4[1]);
				engine.fpx.fpsubPRIME(Qj[num10].Y[0], b, bOffset2, array5[0]);
				engine.fpx.fpsubPRIME(array4[0], array5[0], array6[0]);
				engine.fpx.fpsubPRIME(array4[1], Qj[num10].Y[1], array6[1]);
				engine.fpx.fpsubPRIME(Qj[num10].X[0], t_tate2_firststep_P, bOffset, array7[0]);
				engine.fpx.fpcopy(Qj[num10].X[1], 0L, array7[1]);
				engine.fpx.fpnegPRIME(array7[1]);
				engine.fpx.fp2mul_mont(array6, array7, array6);
				engine.fpx.fp2sqr_mont(f[num10 + t_points], f[num10 + t_points]);
				engine.fpx.fp2mul_mont(f[num10 + t_points], array6, f[num10 + t_points]);
			}
			array8 = t_tate2_firststep_P;
			b = t_tate2_firststep_P2;
			bOffset2 = num2;
			num4 = bOffset;
		}
		for (uint num11 = 0u; num11 < t_points; num11++)
		{
			engine.fpx.fpsubPRIME(Qj[num11].X[0], array8, num4, array6[0]);
			engine.fpx.fpcopy(Qj[num11].X[1], 0L, array6[1]);
			engine.fpx.fp2sqr_mont(f[num11 + t_points], f[num11 + t_points]);
			engine.fpx.fp2mul_mont(f[num11 + t_points], array6, f[num11 + t_points]);
		}
		engine.fpx.mont_n_way_inv(f, 2 * t_points, array);
		for (uint num12 = 0u; num12 < 2 * t_points; num12++)
		{
			final_exponentiation_2_torsion(f[num12], array[num12], f[num12]);
		}
	}

	private void final_exponentiation_2_torsion(ulong[][] f, ulong[][] finv, ulong[][] fout)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array);
		engine.fpx.fp2_conj(f, array2);
		engine.fpx.fp2mul_mont(array2, finv, array2);
		for (uint num = 0u; num < engine.param.OBOB_EXPON; num++)
		{
			engine.fpx.cube_Fp2_cycl(array2, array);
		}
		engine.fpx.fp2copy(array2, fout);
	}
}
