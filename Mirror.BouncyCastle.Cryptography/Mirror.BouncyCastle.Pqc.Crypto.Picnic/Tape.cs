using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal class Tape
{
	internal byte[][] tapes;

	internal int pos;

	private int nTapes;

	private readonly PicnicEngine engine;

	internal Tape(PicnicEngine engine)
	{
		this.engine = engine;
		tapes = new byte[engine.numMPCParties][];
		for (int i = 0; i < engine.numMPCParties; i++)
		{
			tapes[i] = new byte[2 * engine.andSizeBytes];
		}
		pos = 0;
		nTapes = engine.numMPCParties;
	}

	internal void SetAuxBits(byte[] input)
	{
		int num = engine.numMPCParties - 1;
		int num2 = 0;
		int stateSizeBits = engine.stateSizeBits;
		for (int i = 0; i < engine.numRounds; i++)
		{
			for (int j = 0; j < stateSizeBits; j++)
			{
				PicnicUtilities.SetBit(tapes[num], stateSizeBits + stateSizeBits * 2 * i + j, PicnicUtilities.GetBit(input, num2++));
			}
		}
	}

	internal void ComputeAuxTape(byte[] inputs)
	{
		uint[] array = new uint[PicnicEngine.LOWMC_MAX_WORDS];
		uint[] array2 = new uint[PicnicEngine.LOWMC_MAX_WORDS];
		uint[] output = new uint[PicnicEngine.LOWMC_MAX_WORDS];
		uint[] array3 = new uint[PicnicEngine.LOWMC_MAX_WORDS];
		uint[] array4 = new uint[PicnicEngine.LOWMC_MAX_WORDS];
		array4[engine.stateSizeWords - 1] = 0u;
		TapesToParityBits(array4, engine.stateSizeBits);
		KMatricesWithPointer kMatricesWithPointer = engine._lowmcConstants.KMatrixInv(engine, 0);
		engine.matrix_mul(array3, array4, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		if (inputs != null)
		{
			Pack.UInt32_To_LE(array3, 0, engine.stateSizeWords, inputs, 0);
		}
		for (int num = engine.numRounds; num > 0; num--)
		{
			kMatricesWithPointer = engine._lowmcConstants.KMatrix(engine, num);
			engine.matrix_mul(array, array3, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			Nat.XorTo(engine.stateSizeWords, array, array2);
			kMatricesWithPointer = engine._lowmcConstants.LMatrixInv(engine, num - 1);
			engine.matrix_mul(output, array2, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			if (num == 1)
			{
				Array.Copy(array4, 0, array2, 0, array4.Length);
			}
			else
			{
				pos = engine.stateSizeBits * 2 * (num - 1);
				TapesToParityBits(array2, engine.stateSizeBits);
			}
			pos = engine.stateSizeBits * 2 * (num - 1) + engine.stateSizeBits;
			engine.aux_mpc_sbox(array2, output, this);
		}
		pos = 0;
	}

	private void TapesToParityBits(uint[] output, int outputBitLen)
	{
		for (int i = 0; i < outputBitLen; i++)
		{
			PicnicUtilities.SetBitInWordArray(output, i, PicnicUtilities.Parity16(TapesToWord()));
		}
	}

	internal uint TapesToWord()
	{
		int num = pos >> 3;
		int num2 = (pos & 7) ^ 7;
		uint num3 = (uint)(1 << num2);
		uint num4 = 0 | ((tapes[0][num] & num3) << 7) | ((tapes[1][num] & num3) << 6) | ((tapes[2][num] & num3) << 5) | ((tapes[3][num] & num3) << 4) | ((tapes[4][num] & num3) << 3) | ((tapes[5][num] & num3) << 2) | ((tapes[6][num] & num3) << 1) | (tapes[7][num] & num3) | ((tapes[8][num] & num3) << 15) | ((tapes[9][num] & num3) << 14) | ((tapes[10][num] & num3) << 13) | ((tapes[11][num] & num3) << 12) | ((tapes[12][num] & num3) << 11) | ((tapes[13][num] & num3) << 10) | ((tapes[14][num] & num3) << 9) | ((tapes[15][num] & num3) << 8);
		pos++;
		return num4 >> num2;
	}
}
