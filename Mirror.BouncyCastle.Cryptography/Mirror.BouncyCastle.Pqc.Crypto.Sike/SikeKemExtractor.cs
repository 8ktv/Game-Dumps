using System;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

[Obsolete("Will be removed")]
public sealed class SikeKemExtractor : IEncapsulatedSecretExtractor
{
	private readonly SikeKeyParameters key;

	private SikeEngine engine;

	public int EncapsulationLength => engine.GetCipherTextSize();

	public SikeKemExtractor(SikePrivateKeyParameters privParams)
	{
		key = privParams;
		InitCipher(key.Parameters);
	}

	private void InitCipher(SikeParameters param)
	{
		engine = param.GetEngine();
		_ = (SikePrivateKeyParameters)key;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		return ExtractSecret(encapsulation, (int)engine.GetDefaultSessionKeySize());
	}

	public byte[] ExtractSecret(byte[] encapsulation, int sessionKeySizeInBits)
	{
		Console.Error.WriteLine("WARNING: the SIKE algorithm is only for research purposes, insecure");
		byte[] array = new byte[sessionKeySizeInBits / 8];
		engine.crypto_kem_dec(array, encapsulation, ((SikePrivateKeyParameters)key).GetPrivateKey());
		return array;
	}
}
