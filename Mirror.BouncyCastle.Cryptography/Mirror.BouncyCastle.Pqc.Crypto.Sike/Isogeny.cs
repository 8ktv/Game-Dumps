namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class Isogeny
{
	private readonly SikeEngine engine;

	internal Isogeny(SikeEngine engine)
	{
		this.engine = engine;
	}

	internal void Double(PointProj P, PointProj Q, ulong[][] A24, uint k)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2copy(P.X, Q.X);
		engine.fpx.fp2copy(P.Z, Q.Z);
		for (int i = 0; i < k; i++)
		{
			engine.fpx.fp2add(Q.X, Q.Z, array2);
			engine.fpx.fp2sub(Q.X, Q.Z, array3);
			engine.fpx.fp2sqr_mont(array2, array5);
			engine.fpx.fp2sqr_mont(array3, array6);
			engine.fpx.fp2sub(array5, array6, array4);
			engine.fpx.fp2mul_mont(array5, array6, Q.X);
			engine.fpx.fp2mul_mont(A24, array4, array);
			engine.fpx.fp2add(array, array6, array);
			engine.fpx.fp2mul_mont(array4, array, Q.Z);
		}
	}

	internal void CompleteMPoint(ulong[][] A, PointProj P, PointProjFull R)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array8 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array9 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array2[0]);
		if (!Fpx.subarrayEquals(P.Z[0], array[0], engine.param.NWORDS_FIELD) || !Fpx.subarrayEquals(P.Z[1], array[1], engine.param.NWORDS_FIELD))
		{
			engine.fpx.fp2mul_mont(P.X, P.Z, array3);
			engine.fpx.fpsubPRIME(P.X[0], P.Z[1], array8[0]);
			engine.fpx.fpaddPRIME(P.X[1], P.Z[0], array8[1]);
			engine.fpx.fpaddPRIME(P.X[0], P.Z[1], array9[0]);
			engine.fpx.fpsubPRIME(P.X[1], P.Z[0], array9[1]);
			engine.fpx.fp2mul_mont(array8, array9, array5);
			engine.fpx.fp2mul_mont(A, array3, array8);
			engine.fpx.fp2add(array8, array5, array9);
			engine.fpx.fp2mul_mont(array3, array9, array6);
			engine.fpx.sqrt_Fp2(array6, array4);
			engine.fpx.fp2copy(P.Z, array7);
			engine.fpx.fp2inv_mont_bingcd(array7);
			engine.fpx.fp2mul_mont(P.X, array7, R.X);
			engine.fpx.fp2sqr_mont(array7, array8);
			engine.fpx.fp2mul_mont(array4, array8, R.Y);
			engine.fpx.fp2copy(array2, R.Z);
		}
		else
		{
			engine.fpx.fp2copy(array, R.X);
			engine.fpx.fp2copy(array2, R.Y);
			engine.fpx.fp2copy(array, R.Z);
		}
	}

	internal void Ladder(PointProj P, ulong[] m, ulong[][] A, uint order_bits, PointProj R)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj q = new PointProj(engine.param.NWORDS_FIELD);
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		int num = 0;
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array[0]);
		engine.fpx.fpaddPRIME(array[0], array[0], array[0]);
		engine.fpx.fp2add(A, array, array);
		engine.fpx.fp2div2(array, array);
		engine.fpx.fp2div2(array, array);
		int num2 = (int)(order_bits - 1);
		for (uint num3 = (uint)((m[num2 >> (int)Internal.LOG2RADIX] >> (int)(num2 & (Internal.RADIX - 1))) & 1); num3 == 0; num3 = (uint)((m[num2 >> (int)Internal.LOG2RADIX] >> (int)(num2 & (Internal.RADIX - 1))) & 1))
		{
			num2--;
		}
		engine.fpx.fp2copy(P.X, pointProj.X);
		engine.fpx.fp2copy(P.Z, pointProj.Z);
		XDblE(P, q, array, 1);
		int num5;
		ulong option;
		for (int num4 = num2 - 1; num4 >= 0; num4--)
		{
			uint num3 = (uint)((m[num4 >> (int)Internal.LOG2RADIX] >> (int)(num4 & (Internal.RADIX - 1))) & 1);
			num5 = (int)(num3 ^ num);
			num = (int)num3;
			option = (ulong)(-num5);
			SwapPoints(pointProj, q, option);
			XDblAddProj(pointProj, q, P.X, P.Z, array);
		}
		num5 = 0 ^ num;
		option = (ulong)(-num5);
		SwapPoints(pointProj, q, option);
		engine.fpx.fp2copy(pointProj.X, R.X);
		engine.fpx.fp2copy(pointProj.Z, R.Z);
	}

	private void XDblAddProj(PointProj P, PointProj Q, ulong[][] XPQ, ulong[][] ZPQ, ulong[][] A24)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2add(P.X, P.Z, array);
		engine.fpx.fp2sub(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array, P.X);
		engine.fpx.fp2sub(Q.X, Q.Z, array3);
		engine.fpx.fp2correction(array3);
		engine.fpx.fp2add(Q.X, Q.Z, Q.X);
		engine.fpx.fp2mul_mont(array, array3, array);
		engine.fpx.fp2sqr_mont(array2, P.Z);
		engine.fpx.fp2mul_mont(array2, Q.X, array2);
		engine.fpx.fp2sub(P.X, P.Z, array3);
		engine.fpx.fp2mul_mont(P.X, P.Z, P.X);
		engine.fpx.fp2mul_mont(array3, A24, Q.X);
		engine.fpx.fp2sub(array, array2, Q.Z);
		engine.fpx.fp2add(Q.X, P.Z, P.Z);
		engine.fpx.fp2add(array, array2, Q.X);
		engine.fpx.fp2mul_mont(P.Z, array3, P.Z);
		engine.fpx.fp2sqr_mont(Q.Z, Q.Z);
		engine.fpx.fp2sqr_mont(Q.X, Q.X);
		engine.fpx.fp2mul_mont(Q.X, ZPQ, Q.X);
		engine.fpx.fp2mul_mont(Q.Z, XPQ, Q.Z);
	}

	private void XDblE(PointProj P, PointProj Q, ulong[][] A24, int e)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2copy(P.X, Q.X);
		engine.fpx.fp2copy(P.Z, Q.Z);
		for (int i = 0; i < e; i++)
		{
			engine.fpx.fp2add(Q.X, Q.Z, array2);
			engine.fpx.fp2sub(Q.X, Q.Z, array3);
			engine.fpx.fp2sqr_mont(array2, array5);
			engine.fpx.fp2sqr_mont(array3, array6);
			engine.fpx.fp2sub(array5, array6, array4);
			engine.fpx.fp2mul_mont(array5, array6, Q.X);
			engine.fpx.fp2mul_mont(A24, array4, array);
			engine.fpx.fp2add(array, array6, array);
			engine.fpx.fp2mul_mont(array4, array, Q.Z);
		}
	}

	internal void XTplEFast(PointProj P, PointProj Q, ulong[][] A2, uint e)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		engine.fpx.copy_words(P, pointProj);
		for (int i = 0; i < e; i++)
		{
			XTplFast(pointProj, pointProj, A2);
		}
		engine.fpx.copy_words(pointProj, Q);
	}

	private void XTplFast(PointProj P, PointProj Q, ulong[][] A2)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2sqr_mont(P.X, array);
		engine.fpx.fp2sqr_mont(P.Z, array2);
		engine.fpx.fp2add(array, array2, array3);
		engine.fpx.fp2add(P.X, P.Z, array4);
		engine.fpx.fp2sqr_mont(array4, array4);
		engine.fpx.fp2sub(array4, array3, array4);
		engine.fpx.fp2mul_mont(A2, array4, array4);
		engine.fpx.fp2add(array3, array4, array4);
		engine.fpx.fp2sub(array, array2, array3);
		engine.fpx.fp2sqr_mont(array3, array3);
		engine.fpx.fp2mul_mont(array, array4, array);
		engine.fpx.fp2shl(array, 2u, array);
		engine.fpx.fp2sub(array, array3, array);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2mul_mont(array2, array4, array2);
		engine.fpx.fp2shl(array2, 2u, array2);
		engine.fpx.fp2sub(array2, array3, array2);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2mul_mont(P.X, array2, Q.X);
		engine.fpx.fp2mul_mont(P.Z, array, Q.Z);
	}

	internal void LADDER3PT(ulong[][] xP, ulong[][] xQ, ulong[][] xPQ, ulong[] m, uint AliceOrBob, PointProj R, ulong[][] A)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = ((AliceOrBob != engine.param.ALICE) ? (engine.param.OBOB_BITS - 1) : engine.param.OALICE_BITS);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array[0]);
		engine.fpx.mp2_add(array, array, array);
		engine.fpx.mp2_add(A, array, array);
		engine.fpx.fp2div2(array, array);
		engine.fpx.fp2div2(array, array);
		engine.fpx.fp2copy(xQ, pointProj.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj.Z[0]);
		engine.fpx.fp2copy(xPQ, pointProj2.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj2.Z[0]);
		engine.fpx.fp2copy(xP, R.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, R.Z[0]);
		engine.fpx.fpzero(R.Z[1]);
		uint num5;
		ulong option;
		for (uint num3 = 0u; num3 < num2; num3++)
		{
			int num4 = (int)((m[num3 >> (int)Internal.LOG2RADIX] >> (int)(num3 & (Internal.RADIX - 1))) & 1);
			num5 = (uint)num4 ^ num;
			num = (uint)num4;
			option = 0uL - (ulong)num5;
			SwapPoints(R, pointProj2, option);
			XDblAdd(pointProj, pointProj2, R.X, array);
			engine.fpx.fp2mul_mont(pointProj2.X, R.Z, pointProj2.X);
		}
		num5 = 0 ^ num;
		option = 0uL - (ulong)num5;
		SwapPoints(R, pointProj2, option);
	}

	internal void CompletePoint(PointProj P, PointProjFull R)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array8 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array8[0]);
		engine.fpx.fp2mul_mont(P.X, P.Z, array);
		engine.fpx.fpsubPRIME(P.X[0], P.Z[1], array6[0]);
		engine.fpx.fpaddPRIME(P.X[1], P.Z[0], array6[1]);
		engine.fpx.fpaddPRIME(P.X[0], P.Z[1], array7[0]);
		engine.fpx.fpsubPRIME(P.X[1], P.Z[0], array7[1]);
		engine.fpx.fp2mul_mont(array6, array7, array2);
		engine.fpx.fp2mul_mont(array, array2, array3);
		engine.fpx.sqrt_Fp2(array3, array4);
		engine.fpx.fp2copy(P.Z, array5);
		engine.fpx.fp2inv_mont_bingcd(array5);
		engine.fpx.fp2mul_mont(P.X, array5, R.X);
		engine.fpx.fp2sqr_mont(array5, array6);
		engine.fpx.fp2mul_mont(array4, array6, R.Y);
		engine.fpx.fp2copy(array8, R.Z);
	}

	internal void SwapPoints(PointProj P, PointProj Q, ulong option)
	{
		for (int i = 0; i < engine.param.NWORDS_FIELD; i++)
		{
			ulong num = option & (P.X[0][i] ^ Q.X[0][i]);
			P.X[0][i] = num ^ P.X[0][i];
			Q.X[0][i] = num ^ Q.X[0][i];
			num = option & (P.X[1][i] ^ Q.X[1][i]);
			P.X[1][i] = num ^ P.X[1][i];
			Q.X[1][i] = num ^ Q.X[1][i];
			num = option & (P.Z[0][i] ^ Q.Z[0][i]);
			P.Z[0][i] = num ^ P.Z[0][i];
			Q.Z[0][i] = num ^ Q.Z[0][i];
			num = option & (P.Z[1][i] ^ Q.Z[1][i]);
			P.Z[1][i] = num ^ P.Z[1][i];
			Q.Z[1][i] = num ^ Q.Z[1][i];
		}
	}

	internal void XDblAdd(PointProj P, PointProj Q, ulong[][] xPQ, ulong[][] A24)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(P.X, P.Z, array);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array, P.X);
		engine.fpx.mp2_sub_p2(Q.X, Q.Z, array3);
		engine.fpx.mp2_add(Q.X, Q.Z, Q.X);
		engine.fpx.fp2mul_mont(array, array3, array);
		engine.fpx.fp2sqr_mont(array2, P.Z);
		engine.fpx.fp2mul_mont(array2, Q.X, array2);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array3);
		engine.fpx.fp2mul_mont(P.X, P.Z, P.X);
		engine.fpx.fp2mul_mont(A24, array3, Q.X);
		engine.fpx.mp2_sub_p2(array, array2, Q.Z);
		engine.fpx.mp2_add(Q.X, P.Z, P.Z);
		engine.fpx.mp2_add(array, array2, Q.X);
		engine.fpx.fp2mul_mont(P.Z, array3, P.Z);
		engine.fpx.fp2sqr_mont(Q.Z, Q.Z);
		engine.fpx.fp2sqr_mont(Q.X, Q.X);
		engine.fpx.fp2mul_mont(Q.Z, xPQ, Q.Z);
	}

	internal void XDblE(PointProj P, PointProj Q, ulong[][] A24plus, ulong[][] C24, uint e)
	{
		engine.fpx.copy_words(P, Q);
		for (int i = 0; i < e; i++)
		{
			XDbl(Q, Q, A24plus, C24);
		}
	}

	internal void XDbl(PointProj P, PointProj Q, ulong[][] A24plus, ulong[][] C24)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array);
		engine.fpx.mp2_add(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2mul_mont(C24, array, Q.Z);
		engine.fpx.fp2mul_mont(array2, Q.Z, Q.X);
		engine.fpx.mp2_sub_p2(array2, array, array2);
		engine.fpx.fp2mul_mont(A24plus, array2, array);
		engine.fpx.mp2_add(Q.Z, array, Q.Z);
		engine.fpx.fp2mul_mont(Q.Z, array2, Q.Z);
	}

	private void XTpl(PointProj P, PointProj Q, ulong[][] A24minus, ulong[][] A24plus)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array7 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array);
		engine.fpx.fp2sqr_mont(array, array3);
		engine.fpx.mp2_add(P.X, P.Z, array2);
		engine.fpx.fp2sqr_mont(array2, array4);
		engine.fpx.mp2_add(P.X, P.X, array5);
		engine.fpx.mp2_add(P.Z, P.Z, array);
		engine.fpx.fp2sqr_mont(array5, array2);
		engine.fpx.mp2_sub_p2(array2, array4, array2);
		engine.fpx.mp2_sub_p2(array2, array3, array2);
		engine.fpx.fp2mul_mont(A24plus, array4, array6);
		engine.fpx.fp2mul_mont(array4, array6, array4);
		engine.fpx.fp2mul_mont(A24minus, array3, array7);
		engine.fpx.fp2mul_mont(array3, array7, array3);
		engine.fpx.mp2_sub_p2(array3, array4, array4);
		engine.fpx.mp2_sub_p2(array6, array7, array3);
		engine.fpx.fp2mul_mont(array2, array3, array2);
		engine.fpx.fp2add(array4, array2, array3);
		engine.fpx.fp2sqr_mont(array3, array3);
		engine.fpx.fp2mul_mont(array5, array3, Q.X);
		engine.fpx.fp2sub(array4, array2, array2);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2mul_mont(array, array2, Q.Z);
	}

	internal void XTplE(PointProj P, PointProj Q, ulong[][] A24minus, ulong[][] A24plus, uint e)
	{
		engine.fpx.copy_words(P, Q);
		for (int i = 0; i < e; i++)
		{
			XTpl(Q, Q, A24minus, A24plus);
		}
	}

	internal void GetA(ulong[][] xP, ulong[][] xQ, ulong[][] xR, ulong[][] A)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0]);
		engine.fpx.fp2add(xP, xQ, array2);
		engine.fpx.fp2mul_mont(xP, xQ, array);
		engine.fpx.fp2mul_mont(xR, array2, A);
		engine.fpx.fp2add(array, A, A);
		engine.fpx.fp2mul_mont(array, xR, array);
		engine.fpx.fp2sub(A, array3, A);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2add(array2, xR, array2);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2sqr_mont(A, A);
		engine.fpx.fp2inv_mont(array);
		engine.fpx.fp2mul_mont(A, array, A);
		engine.fpx.fp2sub(A, array2, A);
	}

	internal void JInv(ulong[][] A, ulong[][] C, ulong[][] jinv)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2sqr_mont(A, jinv);
		engine.fpx.fp2sqr_mont(C, array2);
		engine.fpx.fp2add(array2, array2, array);
		engine.fpx.fp2sub(jinv, array, array);
		engine.fpx.fp2sub(array, array2, array);
		engine.fpx.fp2sub(array, array2, jinv);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2mul_mont(jinv, array2, jinv);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2sqr_mont(array, array2);
		engine.fpx.fp2mul_mont(array, array2, array);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2add(array, array, array);
		engine.fpx.fp2inv_mont(jinv);
		engine.fpx.fp2mul_mont(jinv, array, jinv);
	}

	internal void Get3Isog(PointProj P, ulong[][] A24minus, ulong[][] A24plus, ulong[][][] coeff)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_sub_p2(P.X, P.Z, coeff[0]);
		engine.fpx.fp2sqr_mont(coeff[0], array);
		engine.fpx.mp2_add(P.X, P.Z, coeff[1]);
		engine.fpx.fp2sqr_mont(coeff[1], array2);
		engine.fpx.mp2_add(P.X, P.X, array4);
		engine.fpx.fp2sqr_mont(array4, array4);
		engine.fpx.fp2sub(array4, array, array3);
		engine.fpx.fp2sub(array4, array2, array4);
		engine.fpx.mp2_add(array, array4, array5);
		engine.fpx.mp2_add(array5, array5, array5);
		engine.fpx.mp2_add(array2, array5, array5);
		engine.fpx.fp2mul_mont(array3, array5, A24minus);
		engine.fpx.mp2_add(array2, array3, array5);
		engine.fpx.mp2_add(array5, array5, array5);
		engine.fpx.mp2_add(array, array5, array5);
		engine.fpx.fp2mul_mont(array4, array5, A24plus);
	}

	internal void Eval3Isog(PointProj Q, ulong[][][] coeff)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(Q.X, Q.Z, array);
		engine.fpx.mp2_sub_p2(Q.X, Q.Z, array2);
		engine.fpx.fp2mul_mont(coeff[0], array, array);
		engine.fpx.fp2mul_mont(coeff[1], array2, array2);
		engine.fpx.mp2_add(array, array2, array3);
		engine.fpx.mp2_sub_p2(array2, array, array);
		engine.fpx.fp2sqr_mont(array3, array3);
		engine.fpx.fp2sqr_mont(array, array);
		engine.fpx.fp2mul_mont(Q.X, array3, Q.X);
		engine.fpx.fp2mul_mont(Q.Z, array, Q.Z);
	}

	internal void Inv3Way(ulong[][] z1, ulong[][] z2, ulong[][] z3)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.fp2mul_mont(z1, z2, array);
		engine.fpx.fp2mul_mont(z3, array, array2);
		engine.fpx.fp2inv_mont(array2);
		engine.fpx.fp2mul_mont(z3, array2, array3);
		engine.fpx.fp2mul_mont(array3, z2, array4);
		engine.fpx.fp2mul_mont(array3, z1, z2);
		engine.fpx.fp2mul_mont(array, array2, z3);
		engine.fpx.fp2copy(array4, z1);
	}

	internal void Get2Isog(PointProj P, ulong[][] A, ulong[][] C)
	{
		engine.fpx.fp2sqr_mont(P.X, A);
		engine.fpx.fp2sqr_mont(P.Z, C);
		engine.fpx.mp2_sub_p2(C, A, A);
	}

	internal void Eval2Isog(PointProj P, PointProj Q)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(Q.X, Q.Z, array);
		engine.fpx.mp2_sub_p2(Q.X, Q.Z, array2);
		engine.fpx.mp2_add(P.X, P.Z, array3);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array4);
		engine.fpx.fp2mul_mont(array, array4, array);
		engine.fpx.fp2mul_mont(array2, array3, array2);
		engine.fpx.mp2_add(array, array2, array3);
		engine.fpx.mp2_sub_p2(array, array2, array4);
		engine.fpx.fp2mul_mont(P.X, array3, P.X);
		engine.fpx.fp2mul_mont(P.Z, array4, P.Z);
	}

	internal void Get4Isog(PointProj P, ulong[][] A24plus, ulong[][] C24, ulong[][][] coeff)
	{
		engine.fpx.mp2_sub_p2(P.X, P.Z, coeff[1]);
		engine.fpx.mp2_add(P.X, P.Z, coeff[2]);
		engine.fpx.fp2sqr_mont(P.Z, coeff[0]);
		engine.fpx.mp2_add(coeff[0], coeff[0], coeff[0]);
		engine.fpx.fp2sqr_mont(coeff[0], C24);
		engine.fpx.mp2_add(coeff[0], coeff[0], coeff[0]);
		engine.fpx.fp2sqr_mont(P.X, A24plus);
		engine.fpx.mp2_add(A24plus, A24plus, A24plus);
		engine.fpx.fp2sqr_mont(A24plus, A24plus);
	}

	internal void Eval4Isog(PointProj P, ulong[][][] coeff)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(P.X, P.Z, array);
		engine.fpx.mp2_sub_p2(P.X, P.Z, array2);
		engine.fpx.fp2mul_mont(array, coeff[1], P.X);
		engine.fpx.fp2mul_mont(array2, coeff[2], P.Z);
		engine.fpx.fp2mul_mont(array, array2, array);
		engine.fpx.fp2mul_mont(coeff[0], array, array);
		engine.fpx.mp2_add(P.X, P.Z, array2);
		engine.fpx.mp2_sub_p2(P.X, P.Z, P.Z);
		engine.fpx.fp2sqr_mont(array2, array2);
		engine.fpx.fp2sqr_mont(P.Z, P.Z);
		engine.fpx.mp2_add(array2, array, P.X);
		engine.fpx.mp2_sub_p2(P.Z, array, array);
		engine.fpx.fp2mul_mont(P.X, array2, P.X);
		engine.fpx.fp2mul_mont(P.Z, array, P.Z);
	}
}
