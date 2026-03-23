using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

public sealed class SaberKemExtractor : IEncapsulatedSecretExtractor
{
	private readonly SaberKeyParameters key;

	private SaberEngine engine;

	public int EncapsulationLength => engine.GetCipherTextSize();

	public SaberKemExtractor(SaberKeyParameters privParams)
	{
		key = privParams;
		InitCipher(key.Parameters);
	}

	private void InitCipher(SaberParameters param)
	{
		engine = param.Engine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[engine.GetSessionKeySize()];
		engine.crypto_kem_dec(array, encapsulation, ((SaberPrivateKeyParameters)key).GetPrivateKey());
		return array;
	}
}
