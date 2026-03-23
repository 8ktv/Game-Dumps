using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Frodo;

public class FrodoKEMExtractor : IEncapsulatedSecretExtractor
{
	private readonly FrodoKeyParameters m_key;

	private readonly FrodoEngine m_engine;

	public int EncapsulationLength => m_engine.CipherTextSize;

	public FrodoKEMExtractor(FrodoKeyParameters privParams)
	{
		m_key = privParams;
		m_engine = privParams.Parameters.Engine;
	}

	public byte[] ExtractSecret(byte[] encapsulation)
	{
		byte[] array = new byte[m_engine.SessionKeySize];
		m_engine.kem_dec(array, encapsulation, ((FrodoPrivateKeyParameters)m_key).privateKey);
		return array;
	}
}
