using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public class DilithiumKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private SecureRandom random;

	private DilithiumParameters parameters;

	public void Init(KeyGenerationParameters param)
	{
		random = param.Random;
		parameters = ((DilithiumKeyGenerationParameters)param).Parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		parameters.GetEngine(random).GenerateKeyPair(out var rho, out var key, out var tr, out var s1_, out var s2_, out var t0_, out var encT);
		DilithiumPublicKeyParameters publicParameter = new DilithiumPublicKeyParameters(parameters, rho, encT);
		DilithiumPrivateKeyParameters privateParameter = new DilithiumPrivateKeyParameters(parameters, rho, key, tr, s1_, s2_, t0_, encT);
		return new AsymmetricCipherKeyPair(publicParameter, privateParameter);
	}
}
