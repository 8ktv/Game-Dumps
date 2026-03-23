using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class Ed448KeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SecureRandom random;

	public virtual void Init(KeyGenerationParameters parameters)
	{
		random = parameters.Random;
	}

	public virtual AsymmetricCipherKeyPair GenerateKeyPair()
	{
		Ed448PrivateKeyParameters ed448PrivateKeyParameters = new Ed448PrivateKeyParameters(random);
		return new AsymmetricCipherKeyPair(ed448PrivateKeyParameters.GeneratePublicKey(), ed448PrivateKeyParameters);
	}
}
