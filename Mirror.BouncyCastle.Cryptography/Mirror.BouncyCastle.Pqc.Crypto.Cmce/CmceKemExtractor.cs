using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

public sealed class CmceKemExtractor : IEncapsulatedSecretExtractor
{
	private ICmceEngine engine;

	private CmceKeyParameters key;

	public int EncapsulationLength => engine.CipherTextSize;

	public CmceKemExtractor(CmcePrivateKeyParameters privParams)
	{
		key = privParams;
		InitCipher(key.Parameters);
	}

	private void InitCipher(CmceParameters param)
	{
		engine = param.Engine;
		CmcePrivateKeyParameters cmcePrivateKeyParameters = (CmcePrivateKeyParameters)key;
		if (cmcePrivateKeyParameters.privateKey.Length < engine.PrivateKeySize)
		{
			key = new CmcePrivateKeyParameters(cmcePrivateKeyParameters.Parameters, engine.DecompressPrivateKey(cmcePrivateKeyParameters.privateKey));
		}
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		return ExtractSecret(encapsulation, engine.DefaultSessionKeySize);
	}

	private byte[] ExtractSecret(byte[] encapsulation, int sessionKeySizeInBits)
	{
		byte[] result = new byte[sessionKeySizeInBits / 8];
		engine.KemDec(result, encapsulation, ((CmcePrivateKeyParameters)key).privateKey);
		return result;
	}
}
