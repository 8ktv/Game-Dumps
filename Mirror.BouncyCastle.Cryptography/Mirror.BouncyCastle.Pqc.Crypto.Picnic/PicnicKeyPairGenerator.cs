using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

public class PicnicKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SecureRandom random;

	private PicnicParameters parameters;

	public void Init(KeyGenerationParameters param)
	{
		random = param.Random;
		parameters = ((PicnicKeyGenerationParameters)param).Parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		PicnicEngine engine = parameters.GetEngine();
		byte[] array = new byte[engine.GetSecretKeySize()];
		byte[] array2 = new byte[engine.GetPublicKeySize()];
		engine.crypto_sign_keypair(array2, array, random);
		PicnicPublicKeyParameters publicParameter = new PicnicPublicKeyParameters(parameters, array2);
		PicnicPrivateKeyParameters privateParameter = new PicnicPrivateKeyParameters(parameters, array);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}
}
