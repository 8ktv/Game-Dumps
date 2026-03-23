using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Falcon;

public class FalconSigner : IMessageSigner
{
	private byte[] encodedkey;

	private FalconNist nist;

	public void Init(bool forSigning, ICipherParameters param)
	{
		SecureRandom random;
		FalconParameters parameters;
		if (forSigning)
		{
			FalconPrivateKeyParameters falconPrivateKeyParameters;
			if (param is ParametersWithRandom parametersWithRandom)
			{
				falconPrivateKeyParameters = (FalconPrivateKeyParameters)parametersWithRandom.Parameters;
				random = parametersWithRandom.Random;
			}
			else
			{
				falconPrivateKeyParameters = (FalconPrivateKeyParameters)param;
				random = CryptoServicesRegistrar.GetSecureRandom();
			}
			encodedkey = falconPrivateKeyParameters.GetEncoded();
			parameters = falconPrivateKeyParameters.Parameters;
		}
		else
		{
			FalconPublicKeyParameters falconPublicKeyParameters = (FalconPublicKeyParameters)param;
			random = null;
			encodedkey = falconPublicKeyParameters.GetEncoded();
			parameters = falconPublicKeyParameters.Parameters;
		}
		nist = new FalconNist(random, (uint)parameters.LogN, (uint)parameters.NonceLength);
	}

	public byte[] GenerateSignature(byte[] message)
	{
		byte[] sm = new byte[nist.CryptoBytes];
		return nist.crypto_sign(attached: false, sm, message, 0, (uint)message.Length, encodedkey, 0);
	}

	public bool VerifySignature(byte[] message, byte[] signature)
	{
		if (signature[0] != (byte)(48 + nist.LogN))
		{
			return false;
		}
		byte[] array = new byte[nist.NonceLength];
		byte[] array2 = new byte[signature.Length - nist.NonceLength - 1];
		Array.Copy(signature, 1L, array, 0L, nist.NonceLength);
		Array.Copy(signature, nist.NonceLength + 1, array2, 0L, signature.Length - nist.NonceLength - 1);
		return nist.crypto_sign_open(attached: false, array2, array, message, encodedkey, 0) == 0;
	}
}
