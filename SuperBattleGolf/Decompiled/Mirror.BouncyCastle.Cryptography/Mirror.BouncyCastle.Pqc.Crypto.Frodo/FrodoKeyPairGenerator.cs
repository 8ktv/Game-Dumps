using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Frodo;

public class FrodoKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private FrodoParameters m_parameters;

	private SecureRandom m_random;

	public void Init(KeyGenerationParameters param)
	{
		FrodoKeyGenerationParameters frodoKeyGenerationParameters = (FrodoKeyGenerationParameters)param;
		m_parameters = frodoKeyGenerationParameters.Parameters;
		m_random = frodoKeyGenerationParameters.Random;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		FrodoEngine engine = m_parameters.Engine;
		byte[] array = new byte[engine.PrivateKeySize];
		byte[] array2 = new byte[engine.PublicKeySize];
		engine.kem_keypair(array2, array, m_random);
		return new AsymmetricCipherKeyPair(new FrodoPublicKeyParameters(m_parameters, array2), new FrodoPrivateKeyParameters(m_parameters, array));
	}
}
