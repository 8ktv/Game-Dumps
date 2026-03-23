namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class Sidh
{
	private readonly SikeEngine engine;

	internal Sidh(SikeEngine engine)
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

	internal void EphemeralKeyGeneration_B(byte[] sk, byte[] pk)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj3 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj4 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_BOB];
		ulong[][] xP = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] xQ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array6 = new uint[engine.param.MAX_INT_POINTS_BOB];
		ulong[] array7 = new ulong[engine.param.NWORDS_ORDER];
		init_basis(engine.param.B_gen, xP, xQ, array2);
		init_basis(engine.param.A_gen, pointProj2.X, pointProj3.X, pointProj4.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj2.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj3.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj4.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0]);
		engine.fpx.mp2_add(array3, array3, array3);
		engine.fpx.mp2_add(array3, array3, array4);
		engine.fpx.mp2_add(array3, array4, array5);
		engine.fpx.mp2_add(array4, array4, array3);
		engine.fpx.decode_to_digits(sk, engine.param.MSG_BYTES, array7, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(xP, xQ, array2, array7, engine.param.BOB, pointProj, array5);
		num = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Bob; num4++)
		{
			uint num5;
			for (; num < engine.param.MAX_Bob - num4; num += num5)
			{
				array[num2] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array[num2].X);
				engine.fpx.fp2copy(pointProj.Z, array[num2].Z);
				array6[num2++] = num;
				num5 = engine.param.strat_Bob[num3++];
				engine.isogeny.XTplE(pointProj, pointProj, array4, array3, num5);
			}
			engine.isogeny.Get3Isog(pointProj, array4, array3, coeff);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval3Isog(array[num6], coeff);
			}
			engine.isogeny.Eval3Isog(pointProj2, coeff);
			engine.isogeny.Eval3Isog(pointProj3, coeff);
			engine.isogeny.Eval3Isog(pointProj4, coeff);
			engine.fpx.fp2copy(array[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num2 - 1].Z, pointProj.Z);
			num = array6[num2 - 1];
			num2--;
		}
		engine.isogeny.Get3Isog(pointProj, array4, array3, coeff);
		engine.isogeny.Eval3Isog(pointProj2, coeff);
		engine.isogeny.Eval3Isog(pointProj3, coeff);
		engine.isogeny.Eval3Isog(pointProj4, coeff);
		engine.isogeny.Inv3Way(pointProj2.Z, pointProj3.Z, pointProj4.Z);
		engine.fpx.fp2mul_mont(pointProj2.X, pointProj2.Z, pointProj2.X);
		engine.fpx.fp2mul_mont(pointProj3.X, pointProj3.Z, pointProj3.X);
		engine.fpx.fp2mul_mont(pointProj4.X, pointProj4.Z, pointProj4.X);
		engine.fpx.fp2_encode(pointProj2.X, pk, 0u);
		engine.fpx.fp2_encode(pointProj3.X, pk, engine.param.FP2_ENCODED_BYTES);
		engine.fpx.fp2_encode(pointProj4.X, pk, 2 * engine.param.FP2_ENCODED_BYTES);
	}

	internal void EphemeralKeyGeneration_A(byte[] ephemeralsk, byte[] ct)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj3 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj pointProj4 = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_ALICE];
		ulong[][] xP = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] xQ = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array6 = new uint[engine.param.MAX_INT_POINTS_ALICE];
		ulong[] array7 = new ulong[engine.param.NWORDS_ORDER];
		init_basis(engine.param.A_gen, xP, xQ, array2);
		init_basis(engine.param.B_gen, pointProj2.X, pointProj3.X, pointProj4.X);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj2.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj3.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, pointProj4.Z[0]);
		engine.fpx.fpcopy(engine.param.Montgomery_one, 0L, array3[0]);
		engine.fpx.mp2_add(array3, array3, array3);
		engine.fpx.mp2_add(array3, array3, array4);
		engine.fpx.mp2_add(array3, array4, array5);
		engine.fpx.mp2_add(array4, array4, array3);
		engine.fpx.decode_to_digits(ephemeralsk, 0u, array7, engine.param.SECRETKEY_A_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(xP, xQ, array2, array7, engine.param.ALICE, pointProj, array5);
		if (engine.param.OALICE_BITS % 2 == 1)
		{
			PointProj pointProj5 = new PointProj(engine.param.NWORDS_FIELD);
			engine.isogeny.XDblE(pointProj, pointProj5, array3, array4, engine.param.OALICE_BITS - 1);
			engine.isogeny.Get2Isog(pointProj5, array3, array4);
			engine.isogeny.Eval2Isog(pointProj2, pointProj5);
			engine.isogeny.Eval2Isog(pointProj3, pointProj5);
			engine.isogeny.Eval2Isog(pointProj4, pointProj5);
			engine.isogeny.Eval2Isog(pointProj, pointProj5);
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
				array6[num2++] = num;
				num5 = engine.param.strat_Alice[num3++];
				engine.isogeny.XDblE(pointProj, pointProj, array3, array4, 2 * num5);
			}
			engine.isogeny.Get4Isog(pointProj, array3, array4, coeff);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval4Isog(array[num6], coeff);
			}
			engine.isogeny.Eval4Isog(pointProj2, coeff);
			engine.isogeny.Eval4Isog(pointProj3, coeff);
			engine.isogeny.Eval4Isog(pointProj4, coeff);
			engine.fpx.fp2copy(array[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num2 - 1].Z, pointProj.Z);
			num = array6[num2 - 1];
			num2--;
		}
		engine.isogeny.Get4Isog(pointProj, array3, array4, coeff);
		engine.isogeny.Eval4Isog(pointProj2, coeff);
		engine.isogeny.Eval4Isog(pointProj3, coeff);
		engine.isogeny.Eval4Isog(pointProj4, coeff);
		engine.isogeny.Inv3Way(pointProj2.Z, pointProj3.Z, pointProj4.Z);
		engine.fpx.fp2mul_mont(pointProj2.X, pointProj2.Z, pointProj2.X);
		engine.fpx.fp2mul_mont(pointProj3.X, pointProj3.Z, pointProj3.X);
		engine.fpx.fp2mul_mont(pointProj4.X, pointProj4.Z, pointProj4.X);
		engine.fpx.fp2_encode(pointProj2.X, ct, 0u);
		engine.fpx.fp2_encode(pointProj3.X, ct, engine.param.FP2_ENCODED_BYTES);
		engine.fpx.fp2_encode(pointProj4.X, ct, 2 * engine.param.FP2_ENCODED_BYTES);
	}

	internal void EphemeralSecretAgreement_A(byte[] ephemeralsk, byte[] pk, byte[] jinvariant)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_ALICE];
		ulong[][][] array2 = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] a = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 0u;
		uint num5 = 0u;
		uint num6 = 0u;
		uint[] array6 = new uint[engine.param.MAX_INT_POINTS_ALICE];
		ulong[] array7 = new ulong[engine.param.NWORDS_ORDER];
		engine.fpx.fp2_decode(pk, array2[0], 0u);
		engine.fpx.fp2_decode(pk, array2[1], engine.param.FP2_ENCODED_BYTES);
		engine.fpx.fp2_decode(pk, array2[2], 2 * engine.param.FP2_ENCODED_BYTES);
		engine.isogeny.GetA(array2[0], array2[1], array2[2], a);
		engine.fpx.mp_add(engine.param.Montgomery_one, engine.param.Montgomery_one, array5[0], engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(a, array5, array4);
		engine.fpx.mp_add(array5[0], array5[0], array5[0], engine.param.NWORDS_FIELD);
		engine.fpx.decode_to_digits(ephemeralsk, 0u, array7, engine.param.SECRETKEY_A_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(array2[0], array2[1], array2[2], array7, engine.param.ALICE, pointProj, a);
		if (engine.param.OALICE_BITS % 2 == 1)
		{
			PointProj pointProj2 = new PointProj(engine.param.NWORDS_FIELD);
			engine.isogeny.XDblE(pointProj, pointProj2, array4, array5, engine.param.OALICE_BITS - 1);
			engine.isogeny.Get2Isog(pointProj2, array4, array5);
			engine.isogeny.Eval2Isog(pointProj, pointProj2);
		}
		num4 = 0u;
		for (num2 = 1u; num2 < engine.param.MAX_Alice; num2++)
		{
			for (; num4 < engine.param.MAX_Alice - num2; num4 += num3)
			{
				array[num5] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array[num5].X);
				engine.fpx.fp2copy(pointProj.Z, array[num5].Z);
				array6[num5++] = num4;
				num3 = engine.param.strat_Alice[num6++];
				engine.isogeny.XDblE(pointProj, pointProj, array4, array5, 2 * num3);
			}
			engine.isogeny.Get4Isog(pointProj, array4, array5, coeff);
			for (num = 0u; num < num5; num++)
			{
				engine.isogeny.Eval4Isog(array[num], coeff);
			}
			engine.fpx.fp2copy(array[num5 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num5 - 1].Z, pointProj.Z);
			num4 = array6[num5 - 1];
			num5--;
		}
		engine.isogeny.Get4Isog(pointProj, array4, array5, coeff);
		engine.fpx.mp2_add(array4, array4, array4);
		engine.fpx.fp2sub(array4, array5, array4);
		engine.fpx.fp2add(array4, array4, array4);
		engine.isogeny.JInv(array4, array5, array3);
		engine.fpx.fp2_encode(array3, jinvariant, 0u);
	}

	internal void EphemeralSecretAgreement_B(byte[] sk, byte[] ct, byte[] jinvariant_)
	{
		PointProj pointProj = new PointProj(engine.param.NWORDS_FIELD);
		PointProj[] array = new PointProj[engine.param.MAX_INT_POINTS_BOB];
		ulong[][][] coeff = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		ulong[][][] array2 = SikeUtilities.InitArray(3u, 2u, engine.param.NWORDS_FIELD);
		ulong[][] array3 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array4 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array5 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array6 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint[] array7 = new uint[engine.param.MAX_INT_POINTS_BOB];
		ulong[] array8 = new ulong[engine.param.NWORDS_ORDER];
		engine.fpx.fp2_decode(ct, array2[0], 0u);
		engine.fpx.fp2_decode(ct, array2[1], engine.param.FP2_ENCODED_BYTES);
		engine.fpx.fp2_decode(ct, array2[2], 2 * engine.param.FP2_ENCODED_BYTES);
		engine.isogeny.GetA(array2[0], array2[1], array2[2], array6);
		engine.fpx.mp_add(engine.param.Montgomery_one, engine.param.Montgomery_one, array5[0], engine.param.NWORDS_FIELD);
		engine.fpx.mp2_add(array6, array5, array4);
		engine.fpx.mp2_sub_p2(array6, array5, array5);
		engine.fpx.decode_to_digits(sk, engine.param.MSG_BYTES, array8, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		engine.isogeny.LADDER3PT(array2[0], array2[1], array2[2], array8, engine.param.BOB, pointProj, array6);
		num = 0u;
		for (uint num4 = 1u; num4 < engine.param.MAX_Bob; num4++)
		{
			uint num5;
			for (; num < engine.param.MAX_Bob - num4; num += num5)
			{
				array[num2] = new PointProj(engine.param.NWORDS_FIELD);
				engine.fpx.fp2copy(pointProj.X, array[num2].X);
				engine.fpx.fp2copy(pointProj.Z, array[num2].Z);
				array7[num2++] = num;
				num5 = engine.param.strat_Bob[num3++];
				engine.isogeny.XTplE(pointProj, pointProj, array5, array4, num5);
			}
			engine.isogeny.Get3Isog(pointProj, array5, array4, coeff);
			for (uint num6 = 0u; num6 < num2; num6++)
			{
				engine.isogeny.Eval3Isog(array[num6], coeff);
			}
			engine.fpx.fp2copy(array[num2 - 1].X, pointProj.X);
			engine.fpx.fp2copy(array[num2 - 1].Z, pointProj.Z);
			num = array7[num2 - 1];
			num2--;
		}
		engine.isogeny.Get3Isog(pointProj, array5, array4, coeff);
		engine.fpx.fp2add(array4, array5, array6);
		engine.fpx.fp2add(array6, array6, array6);
		engine.fpx.fp2sub(array4, array5, array4);
		engine.isogeny.JInv(array6, array4, array3);
		engine.fpx.fp2_encode(array3, jinvariant_, 0u);
	}
}
