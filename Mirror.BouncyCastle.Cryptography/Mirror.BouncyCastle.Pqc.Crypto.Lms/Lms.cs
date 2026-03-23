using System;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public static class Lms
{
	internal static ushort D_LEAF = 33410;

	internal static ushort D_INTR = 33667;

	public static LmsPrivateKeyParameters GenerateKeys(LMSigParameters parameterSet, LMOtsParameters lmOtsParameters, int q, byte[] I, byte[] rootSeed)
	{
		if (rootSeed == null || rootSeed.Length < parameterSet.M)
		{
			throw new ArgumentException($"root seed is less than {parameterSet.M}");
		}
		int maxQ = 1 << parameterSet.H;
		return new LmsPrivateKeyParameters(parameterSet, lmOtsParameters, q, I, maxQ, rootSeed);
	}

	public static LmsSignature GenerateSign(LmsPrivateKeyParameters privateKey, byte[] message)
	{
		LmsContext lmsContext = privateKey.GenerateLmsContext();
		lmsContext.BlockUpdate(message, 0, message.Length);
		return GenerateSign(lmsContext);
	}

	public static LmsSignature GenerateSign(LmsContext context)
	{
		LMOtsSignature otsSignature = LMOts.LMOtsGenerateSignature(context.PrivateKey, context.GetQ(), context.C);
		return new LmsSignature(context.PrivateKey.Q, otsSignature, context.SigParams, context.Path);
	}

	public static bool VerifySignature(LmsPublicKeyParameters publicKey, LmsSignature S, byte[] message)
	{
		LmsContext lmsContext = publicKey.GenerateOtsContext(S);
		LmsUtilities.ByteArray(message, lmsContext);
		return VerifySignature(publicKey, lmsContext);
	}

	public static bool VerifySignature(LmsPublicKeyParameters publicKey, byte[] S, byte[] message)
	{
		LmsContext lmsContext = publicKey.GenerateLmsContext(S);
		LmsUtilities.ByteArray(message, lmsContext);
		return VerifySignature(publicKey, lmsContext);
	}

	public static bool VerifySignature(LmsPublicKeyParameters publicKey, LmsContext context)
	{
		LmsSignature lmsSignature = (LmsSignature)context.Signature;
		LMSigParameters sigParameters = lmsSignature.SigParameters;
		int h = sigParameters.H;
		byte[][] y = lmsSignature.Y;
		byte[] array = LMOts.LMOtsValidateSignatureCalculate(context);
		int num = (1 << h) + lmsSignature.Q;
		byte[] i = publicKey.GetI();
		IDigest digest = LmsUtilities.GetDigest(sigParameters);
		byte[] array2 = new byte[digest.GetDigestSize()];
		digest.BlockUpdate(i, 0, i.Length);
		LmsUtilities.U32Str(num, digest);
		LmsUtilities.U16Str((short)D_LEAF, digest);
		digest.BlockUpdate(array, 0, array.Length);
		digest.DoFinal(array2, 0);
		int num2 = 0;
		while (num > 1)
		{
			if ((num & 1) == 1)
			{
				digest.BlockUpdate(i, 0, i.Length);
				LmsUtilities.U32Str(num / 2, digest);
				LmsUtilities.U16Str((short)D_INTR, digest);
				digest.BlockUpdate(y[num2], 0, y[num2].Length);
				digest.BlockUpdate(array2, 0, array2.Length);
				digest.DoFinal(array2, 0);
			}
			else
			{
				digest.BlockUpdate(i, 0, i.Length);
				LmsUtilities.U32Str(num / 2, digest);
				LmsUtilities.U16Str((short)D_INTR, digest);
				digest.BlockUpdate(array2, 0, array2.Length);
				digest.BlockUpdate(y[num2], 0, y[num2].Length);
				digest.DoFinal(array2, 0);
			}
			num /= 2;
			num2++;
			if (num2 == y.Length && num > 1)
			{
				return false;
			}
		}
		byte[] sig = array2;
		return publicKey.MatchesT1(sig);
	}
}
