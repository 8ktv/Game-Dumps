namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal abstract class LowmcConstants
{
	internal KMatrices _LMatrix;

	internal KMatrices _KMatrix;

	internal KMatrices RConstants;

	internal KMatrices LMatrix_full;

	internal KMatrices LMatrix_inv;

	internal KMatrices KMatrix_full;

	internal KMatrices KMatrix_inv;

	internal KMatrices RConstants_full;

	private KMatricesWithPointer GET_MAT(KMatrices m, int r)
	{
		KMatricesWithPointer kMatricesWithPointer = new KMatricesWithPointer(m);
		kMatricesWithPointer.SetMatrixPointer(r * kMatricesWithPointer.GetSize());
		return kMatricesWithPointer;
	}

	internal KMatricesWithPointer LMatrix(PicnicEngine engine, int round)
	{
		switch (engine.stateSizeBits)
		{
		case 128:
		case 256:
			return GET_MAT(_LMatrix, round);
		case 129:
		case 255:
			return GET_MAT(LMatrix_full, round);
		case 192:
			return GET_MAT((engine.numRounds == 4) ? LMatrix_full : _LMatrix, round);
		default:
			return null;
		}
	}

	internal KMatricesWithPointer LMatrixInv(PicnicEngine engine, int round)
	{
		switch (engine.stateSizeBits)
		{
		case 129:
		case 255:
			return GET_MAT(LMatrix_inv, round);
		case 192:
			if (engine.numRounds != 4)
			{
				return null;
			}
			return GET_MAT(LMatrix_inv, round);
		default:
			return null;
		}
	}

	internal KMatricesWithPointer KMatrix(PicnicEngine engine, int round)
	{
		switch (engine.stateSizeBits)
		{
		case 128:
		case 256:
			return GET_MAT(_KMatrix, round);
		case 129:
		case 255:
			return GET_MAT(KMatrix_full, round);
		case 192:
			return GET_MAT((engine.numRounds == 4) ? KMatrix_full : _KMatrix, round);
		default:
			return null;
		}
	}

	internal KMatricesWithPointer KMatrixInv(PicnicEngine engine, int round)
	{
		switch (engine.stateSizeBits)
		{
		case 129:
		case 255:
			return GET_MAT(KMatrix_inv, round);
		case 192:
			if (engine.numRounds != 4)
			{
				return null;
			}
			return GET_MAT(KMatrix_inv, round);
		default:
			return null;
		}
	}

	internal KMatricesWithPointer RConstant(PicnicEngine engine, int round)
	{
		switch (engine.stateSizeBits)
		{
		case 128:
		case 256:
			return GET_MAT(RConstants, round);
		case 129:
		case 255:
			return GET_MAT(RConstants_full, round);
		case 192:
			return GET_MAT((engine.numRounds == 4) ? RConstants_full : RConstants, round);
		default:
			return null;
		}
	}
}
