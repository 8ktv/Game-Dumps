using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public class DilithiumSigner : IMessageSigner
{
	private DilithiumPrivateKeyParameters privKey;

	private DilithiumPublicKeyParameters pubKey;

	private SecureRandom random;

	public void Init(bool forSigning, ICipherParameters param)
	{
		if (forSigning)
		{
			if (param is ParametersWithRandom parametersWithRandom)
			{
				privKey = (DilithiumPrivateKeyParameters)parametersWithRandom.Parameters;
				random = parametersWithRandom.Random;
			}
			else
			{
				privKey = (DilithiumPrivateKeyParameters)param;
				random = null;
			}
		}
		else
		{
			pubKey = (DilithiumPublicKeyParameters)param;
			random = null;
		}
	}

	public byte[] GenerateSignature(byte[] message)
	{
		DilithiumEngine engine = privKey.Parameters.GetEngine(random);
		byte[] array = new byte[engine.CryptoBytes];
		engine.Sign(array, array.Length, message, message.Length, privKey.m_rho, privKey.m_k, privKey.m_tr, privKey.m_t0, privKey.m_s1, privKey.m_s2);
		return array;
	}

	public bool VerifySignature(byte[] message, byte[] signature)
	{
		return pubKey.Parameters.GetEngine(random).SignOpen(message, signature, signature.Length, pubKey.m_rho, pubKey.m_t1);
	}
}
