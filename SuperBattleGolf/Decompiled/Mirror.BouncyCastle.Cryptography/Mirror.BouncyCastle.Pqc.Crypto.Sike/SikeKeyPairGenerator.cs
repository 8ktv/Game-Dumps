using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

[Obsolete("Will be removed")]
public sealed class SikeKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SikeKeyGenerationParameters sikeParams;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		sikeParams = (SikeKeyGenerationParameters)param;
		random = param.Random;
	}

	private AsymmetricCipherKeyPair GenKeyPair()
	{
		SikeEngine engine = sikeParams.Parameters.GetEngine();
		byte[] array = new byte[engine.GetPrivateKeySize()];
		byte[] array2 = new byte[engine.GetPublicKeySize()];
		engine.crypto_kem_keypair(array2, array, random);
		SikePublicKeyParameters publicParameter = new SikePublicKeyParameters(sikeParams.Parameters, array2);
		SikePrivateKeyParameters privateParameter = new SikePrivateKeyParameters(sikeParams.Parameters, array);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}

	public void Init(KeyGenerationParameters param)
	{
		Initialize(param);
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		return GenKeyPair();
	}
}
