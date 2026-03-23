using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

public class SaberKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SaberKeyGenerationParameters saberParams;

	private int l;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		saberParams = (SaberKeyGenerationParameters)param;
		random = param.Random;
		l = saberParams.Parameters.L;
	}

	private AsymmetricCipherKeyPair GenKeyPair()
	{
		SaberEngine engine = saberParams.Parameters.Engine;
		byte[] array = new byte[engine.GetPrivateKeySize()];
		byte[] array2 = new byte[engine.GetPublicKeySize()];
		engine.crypto_kem_keypair(array2, array, random);
		SaberPublicKeyParameters publicParameter = new SaberPublicKeyParameters(saberParams.Parameters, array2);
		SaberPrivateKeyParameters privateParameter = new SaberPrivateKeyParameters(saberParams.Parameters, array);
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
