using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LmsKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private LmsKeyGenerationParameters m_parameters;

	public void Init(KeyGenerationParameters parameters)
	{
		m_parameters = (LmsKeyGenerationParameters)parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		SecureRandom random = m_parameters.Random;
		byte[] nextBytes = SecureRandom.GetNextBytes(random, 16);
		LmsParameters lmsParameters = m_parameters.LmsParameters;
		LMSigParameters lMSigParameters = lmsParameters.LMSigParameters;
		LMOtsParameters lMOtsParameters = lmsParameters.LMOtsParameters;
		byte[] nextBytes2 = SecureRandom.GetNextBytes(random, lMSigParameters.M);
		LmsPrivateKeyParameters lmsPrivateKeyParameters = Lms.GenerateKeys(lMSigParameters, lMOtsParameters, 0, nextBytes, nextBytes2);
		return new AsymmetricCipherKeyPair(lmsPrivateKeyParameters.GetPublicKey(), lmsPrivateKeyParameters);
	}
}
