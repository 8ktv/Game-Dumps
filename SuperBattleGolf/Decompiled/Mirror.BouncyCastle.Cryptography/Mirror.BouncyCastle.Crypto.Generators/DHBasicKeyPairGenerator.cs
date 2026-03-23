using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Generators;

[Obsolete("Use 'DHKeyPairGenerator' instead")]
public class DHBasicKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private DHKeyGenerationParameters m_parameters;

	public virtual void Init(KeyGenerationParameters parameters)
	{
		m_parameters = (DHKeyGenerationParameters)parameters;
	}

	public virtual AsymmetricCipherKeyPair GenerateKeyPair()
	{
		DHParameters parameters = m_parameters.Parameters;
		BigInteger x = DHKeyGeneratorHelper.CalculatePrivate(parameters, m_parameters.Random);
		return new AsymmetricCipherKeyPair(new DHPublicKeyParameters(DHKeyGeneratorHelper.CalculatePublic(parameters, x), parameters), new DHPrivateKeyParameters(x, parameters));
	}
}
