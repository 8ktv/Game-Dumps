using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public static class Hss
{
	public static HssPrivateKeyParameters GenerateHssKeyPair(HssKeyGenerationParameters parameters)
	{
		LmsPrivateKeyParameters[] array = new LmsPrivateKeyParameters[parameters.Depth];
		LmsSignature[] collection = new LmsSignature[parameters.Depth - 1];
		byte[] nextBytes = SecureRandom.GetNextBytes(parameters.Random, parameters.GetLmsParameters(0).LMSigParameters.M);
		byte[] nextBytes2 = SecureRandom.GetNextBytes(parameters.Random, 16);
		byte[] array2 = new byte[0];
		long num = 1L;
		for (int i = 0; i < array.Length; i++)
		{
			LmsParameters lmsParameters = parameters.GetLmsParameters(i);
			if (i == 0)
			{
				array[i] = new LmsPrivateKeyParameters(lmsParameters.LMSigParameters, lmsParameters.LMOtsParameters, 0, nextBytes2, 1 << lmsParameters.LMSigParameters.H, nextBytes, isPlaceholder: false);
			}
			else
			{
				array[i] = new LmsPrivateKeyParameters(lmsParameters.LMSigParameters, lmsParameters.LMOtsParameters, -1, array2, 1 << lmsParameters.LMSigParameters.H, array2, isPlaceholder: true);
			}
			num <<= lmsParameters.LMSigParameters.H;
		}
		if (num == 0L)
		{
			num = long.MaxValue;
		}
		return new HssPrivateKeyParameters(parameters.Depth, new List<LmsPrivateKeyParameters>(array), new List<LmsSignature>(collection), 0L, num);
	}

	public static void IncrementIndex(HssPrivateKeyParameters keyPair)
	{
		lock (keyPair)
		{
			RangeTestKeys(keyPair);
			keyPair.IncIndex();
			keyPair.GetKeys()[keyPair.Level - 1].IncIndex();
		}
	}

	public static void RangeTestKeys(HssPrivateKeyParameters keyPair)
	{
		lock (keyPair)
		{
			if (keyPair.GetIndex() >= keyPair.IndexLimit)
			{
				throw new Exception("hss private key" + (keyPair.IsShard() ? " shard" : "") + " is exhausted");
			}
			int level = keyPair.Level;
			int num = level;
			IList<LmsPrivateKeyParameters> keys = keyPair.GetKeys();
			while (keys[num - 1].GetIndex() == 1 << keys[num - 1].SigParameters.H)
			{
				if (--num == 0)
				{
					throw new Exception("hss private key" + (keyPair.IsShard() ? " shard" : "") + " is exhausted the maximum limit for this HSS private key");
				}
			}
			while (num < level)
			{
				keyPair.ReplaceConsumedKey(num++);
			}
		}
	}

	public static HssSignature GenerateSignature(HssPrivateKeyParameters keyPair, byte[] message)
	{
		int level = keyPair.Level;
		LmsPrivateKeyParameters lmsPrivateKeyParameters;
		LmsSignedPubKey[] array;
		lock (keyPair)
		{
			RangeTestKeys(keyPair);
			IList<LmsPrivateKeyParameters> keys = keyPair.GetKeys();
			IList<LmsSignature> sig = keyPair.GetSig();
			lmsPrivateKeyParameters = keyPair.GetKeys()[level - 1];
			int i = 0;
			array = new LmsSignedPubKey[level - 1];
			for (; i < level - 1; i++)
			{
				array[i] = new LmsSignedPubKey(sig[i], keys[i + 1].GetPublicKey());
			}
			keyPair.IncIndex();
		}
		LmsContext lmsContext = lmsPrivateKeyParameters.GenerateLmsContext().WithSignedPublicKeys(array);
		lmsContext.BlockUpdate(message, 0, message.Length);
		return GenerateSignature(level, lmsContext);
	}

	public static HssSignature GenerateSignature(int L, LmsContext context)
	{
		return new HssSignature(L - 1, context.SignedPubKeys, Lms.GenerateSign(context));
	}

	public static bool VerifySignature(HssPublicKeyParameters publicKey, HssSignature signature, byte[] message)
	{
		int lMinus = signature.LMinus1;
		if (lMinus + 1 != publicKey.Level)
		{
			return false;
		}
		LmsSignature[] array = new LmsSignature[lMinus + 1];
		LmsPublicKeyParameters[] array2 = new LmsPublicKeyParameters[lMinus];
		for (int i = 0; i < lMinus; i++)
		{
			array[i] = signature.SignedPubKeys[i].Signature;
			array2[i] = signature.SignedPubKeys[i].PublicKey;
		}
		array[lMinus] = signature.Signature;
		LmsPublicKeyParameters publicKey2 = publicKey.LmsPublicKey;
		for (int j = 0; j < lMinus; j++)
		{
			LmsSignature s = array[j];
			byte[] message2 = array2[j].ToByteArray();
			if (!Lms.VerifySignature(publicKey2, s, message2))
			{
				return false;
			}
			try
			{
				publicKey2 = array2[j];
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}
		return Lms.VerifySignature(publicKey2, array[lMinus], message);
	}
}
