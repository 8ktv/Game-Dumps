using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

public class HqcKemExtractor : IEncapsulatedSecretExtractor
{
	private HqcEngine engine;

	private HqcKeyParameters key;

	public int EncapsulationLength => key.Parameters.NBytes + key.Parameters.N1n2Bytes + 64 + 16;

	public HqcKemExtractor(HqcPrivateKeyParameters privParams)
	{
		key = privParams;
		InitCipher(key.Parameters);
	}

	private void InitCipher(HqcParameters param)
	{
		engine = param.Engine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[engine.GetSessionKeySize()];
		byte[] privateKey = ((HqcPrivateKeyParameters)key).PrivateKey;
		engine.Decaps(array, encapsulation, privateKey);
		return array;
	}
}
