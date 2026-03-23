using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public class KyberKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private KyberParameters KyberParams;

	private SecureRandom random;

	private void Initialize(KeyGenerationParameters param)
	{
		KyberParams = ((KyberKeyGenerationParameters)param).Parameters;
		random = param.Random;
	}

	private AsymmetricCipherKeyPair GenKeyPair()
	{
		KyberEngine engine = KyberParams.Engine;
		engine.Init(random);
		engine.GenerateKemKeyPair(out var t, out var rho, out var s, out var hpk, out var nonce);
		KyberPublicKeyParameters publicParameter = new KyberPublicKeyParameters(KyberParams, t, rho);
		KyberPrivateKeyParameters privateParameter = new KyberPrivateKeyParameters(KyberParams, s, hpk, nonce, t, rho);
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
