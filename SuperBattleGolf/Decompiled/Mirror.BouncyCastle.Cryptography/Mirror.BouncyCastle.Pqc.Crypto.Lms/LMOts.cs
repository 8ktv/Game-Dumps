using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public static class LMOts
{
	private static ushort D_PBLC = 32896;

	private static int ITER_K = 20;

	private static int ITER_PREV = 23;

	private static int ITER_J = 22;

	internal static int SEED_RANDOMISER_INDEX = -3;

	internal static int MAX_HASH = 32;

	internal static ushort D_MESG = 33153;

	public static int Coef(byte[] S, int i, int w)
	{
		int num = i * w / 8;
		int num2 = 8 / w;
		int num3 = w * (~i & (num2 - 1));
		int num4 = (1 << w) - 1;
		return (S[num] >> num3) & num4;
	}

	public static int Cksm(byte[] S, int sLen, LMOtsParameters parameters)
	{
		int num = 0;
		int w = parameters.W;
		int num2 = (1 << w) - 1;
		for (int i = 0; i < sLen * 8 / parameters.W; i++)
		{
			num = num + num2 - Coef(S, i, parameters.W);
		}
		return num << parameters.Ls;
	}

	public static LMOtsPublicKey LmsOtsGeneratePublicKey(LMOtsPrivateKey privateKey)
	{
		byte[] k = LmsOtsGeneratePublicKey(privateKey.Parameters, privateKey.I, privateKey.Q, privateKey.MasterSecret);
		return new LMOtsPublicKey(privateKey.Parameters, privateKey.I, privateKey.Q, k);
	}

	internal static byte[] LmsOtsGeneratePublicKey(LMOtsParameters parameters, byte[] I, int q, byte[] masterSecret)
	{
		IDigest digest = LmsUtilities.GetDigest(parameters);
		byte[] array = Composer.Compose().Bytes(I).U32Str(q)
			.U16Str(D_PBLC)
			.PadUntil(0, 22)
			.Build();
		digest.BlockUpdate(array, 0, array.Length);
		IDigest digest2 = LmsUtilities.GetDigest(parameters);
		byte[] array2 = Composer.Compose().Bytes(I).U32Str(q)
			.PadUntil(0, 23 + digest2.GetDigestSize())
			.Build();
		SeedDerive seedDerive = new SeedDerive(I, masterSecret, LmsUtilities.GetDigest(parameters))
		{
			Q = q,
			J = 0
		};
		int p = parameters.P;
		int n = parameters.N;
		int num = (1 << parameters.W) - 1;
		for (ushort num2 = 0; num2 < p; num2++)
		{
			seedDerive.DeriveSeed(num2 < p - 1, array2, ITER_PREV);
			Pack.UInt16_To_BE(num2, array2, ITER_K);
			for (int i = 0; i < num; i++)
			{
				array2[ITER_J] = (byte)i;
				digest2.BlockUpdate(array2, 0, array2.Length);
				digest2.DoFinal(array2, ITER_PREV);
			}
			digest.BlockUpdate(array2, ITER_PREV, n);
		}
		byte[] array3 = new byte[digest.GetDigestSize()];
		digest.DoFinal(array3, 0);
		return array3;
	}

	public static LMOtsSignature lm_ots_generate_signature(LMSigParameters sigParams, LMOtsPrivateKey privateKey, byte[][] path, byte[] message, bool preHashed)
	{
		byte[] array = new byte[MAX_HASH + 2];
		byte[] c;
		if (!preHashed)
		{
			LmsContext signatureContext = privateKey.GetSignatureContext(sigParams, path);
			LmsUtilities.ByteArray(message, 0, message.Length, signatureContext);
			c = signatureContext.C;
			array = signatureContext.GetQ();
		}
		else
		{
			int n = privateKey.Parameters.N;
			c = new byte[n];
			Array.Copy(message, 0, array, 0, n);
		}
		return LMOtsGenerateSignature(privateKey, array, c);
	}

	public static LMOtsSignature LMOtsGenerateSignature(LMOtsPrivateKey privateKey, byte[] Q, byte[] C)
	{
		LMOtsParameters parameters = privateKey.Parameters;
		int n = parameters.N;
		int p = parameters.P;
		int w = parameters.W;
		byte[] array = new byte[p * n];
		IDigest digest = LmsUtilities.GetDigest(parameters);
		SeedDerive derivationFunction = privateKey.GetDerivationFunction();
		int num = Cksm(Q, n, parameters);
		Q[n] = (byte)((num >> 8) & 0xFF);
		Q[n + 1] = (byte)num;
		byte[] array2 = Composer.Compose().Bytes(privateKey.I).U32Str(privateKey.Q)
			.PadUntil(0, ITER_PREV + n)
			.Build();
		derivationFunction.J = 0;
		for (ushort num2 = 0; num2 < p; num2++)
		{
			Pack.UInt16_To_BE(num2, array2, ITER_K);
			derivationFunction.DeriveSeed(num2 < p - 1, array2, ITER_PREV);
			int num3 = Coef(Q, num2, w);
			for (int i = 0; i < num3; i++)
			{
				array2[ITER_J] = (byte)i;
				digest.BlockUpdate(array2, 0, ITER_PREV + n);
				digest.DoFinal(array2, ITER_PREV);
			}
			Array.Copy(array2, ITER_PREV, array, n * num2, n);
		}
		return new LMOtsSignature(parameters, C, array);
	}

	public static bool LMOtsValidateSignature(LMOtsPublicKey publicKey, LMOtsSignature signature, byte[] message, bool prehashed)
	{
		if (!signature.ParamType.Equals(publicKey.Parameters))
		{
			throw new LmsException("public key and signature ots types do not match");
		}
		return Arrays.AreEqual(LMOtsValidateSignatureCalculate(publicKey, signature, message), publicKey.K);
	}

	public static byte[] LMOtsValidateSignatureCalculate(LMOtsPublicKey publicKey, LMOtsSignature signature, byte[] message)
	{
		LmsContext lmsContext = publicKey.CreateOtsContext(signature);
		LmsUtilities.ByteArray(message, lmsContext);
		return LMOtsValidateSignatureCalculate(lmsContext);
	}

	public static byte[] LMOtsValidateSignatureCalculate(LmsContext context)
	{
		LMOtsPublicKey publicKey = context.PublicKey;
		LMOtsParameters parameters = publicKey.Parameters;
		object signature = context.Signature;
		LMOtsSignature lMOtsSignature = ((!(signature is LmsSignature lmsSignature)) ? ((LMOtsSignature)signature) : lmsSignature.OtsSignature);
		int n = parameters.N;
		int w = parameters.W;
		int p = parameters.P;
		byte[] q = context.GetQ();
		int num = Cksm(q, n, parameters);
		q[n] = (byte)((num >> 8) & 0xFF);
		q[n + 1] = (byte)num;
		byte[] i = publicKey.I;
		int q2 = publicKey.Q;
		IDigest digest = LmsUtilities.GetDigest(parameters);
		LmsUtilities.ByteArray(i, digest);
		LmsUtilities.U32Str(q2, digest);
		LmsUtilities.U16Str((short)D_PBLC, digest);
		byte[] array = Composer.Compose().Bytes(i).U32Str(q2)
			.PadUntil(0, ITER_PREV + n)
			.Build();
		int num2 = (1 << w) - 1;
		byte[] y = lMOtsSignature.Y;
		IDigest digest2 = LmsUtilities.GetDigest(parameters);
		for (ushort num3 = 0; num3 < p; num3++)
		{
			Pack.UInt16_To_BE(num3, array, ITER_K);
			Array.Copy(y, num3 * n, array, ITER_PREV, n);
			for (int j = Coef(q, num3, w); j < num2; j++)
			{
				array[ITER_J] = (byte)j;
				digest2.BlockUpdate(array, 0, ITER_PREV + n);
				digest2.DoFinal(array, ITER_PREV);
			}
			digest.BlockUpdate(array, ITER_PREV, n);
		}
		byte[] array2 = new byte[n];
		digest.DoFinal(array2, 0);
		return array2;
	}
}
