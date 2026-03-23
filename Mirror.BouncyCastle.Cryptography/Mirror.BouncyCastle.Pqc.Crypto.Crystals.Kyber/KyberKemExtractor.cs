using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public sealed class KyberKemExtractor : IEncapsulatedSecretExtractor
{
	private readonly KyberKeyParameters m_key;

	private readonly KyberEngine m_engine;

	public int EncapsulationLength => m_engine.CryptoCipherTextBytes;

	public KyberKemExtractor(KyberKeyParameters privParams)
	{
		m_key = privParams;
		m_engine = m_key.Parameters.Engine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[m_engine.CryptoBytes];
		m_engine.KemDecrypt(array, encapsulation, ((KyberPrivateKeyParameters)m_key).GetEncoded());
		return array;
	}
}
