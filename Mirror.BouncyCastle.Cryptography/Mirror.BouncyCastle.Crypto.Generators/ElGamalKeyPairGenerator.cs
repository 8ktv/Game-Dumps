using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class ElGamalKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private ElGamalKeyGenerationParameters m_parameters;

	public void Init(KeyGenerationParameters parameters)
	{
		m_parameters = (ElGamalKeyGenerationParameters)parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		ElGamalParameters parameters = m_parameters.Parameters;
		DHParameters dhParams = new DHParameters(parameters.P, parameters.G, null, 0, parameters.L);
		BigInteger x = DHKeyGeneratorHelper.CalculatePrivate(dhParams, m_parameters.Random);
		return new AsymmetricCipherKeyPair(new ElGamalPublicKeyParameters(DHKeyGeneratorHelper.CalculatePublic(dhParams, x), parameters), new ElGamalPrivateKeyParameters(x, parameters));
	}
}
