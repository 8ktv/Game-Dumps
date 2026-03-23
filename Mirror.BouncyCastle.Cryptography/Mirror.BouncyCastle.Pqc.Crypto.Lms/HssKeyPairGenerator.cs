using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class HssKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private HssKeyGenerationParameters m_parameters;

	public void Init(KeyGenerationParameters parameters)
	{
		m_parameters = (HssKeyGenerationParameters)parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		HssPrivateKeyParameters hssPrivateKeyParameters = Hss.GenerateHssKeyPair(m_parameters);
		return new AsymmetricCipherKeyPair(hssPrivateKeyParameters.GetPublicKey(), hssPrivateKeyParameters);
	}
}
