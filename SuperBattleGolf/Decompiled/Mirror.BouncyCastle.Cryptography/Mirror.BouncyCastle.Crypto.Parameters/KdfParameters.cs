namespace Mirror.BouncyCastle.Crypto.Parameters;

public class KdfParameters : IDerivationParameters
{
	private readonly byte[] m_iv;

	private readonly byte[] m_shared;

	public KdfParameters(byte[] shared, byte[] iv)
	{
		m_shared = shared;
		m_iv = iv;
	}

	public byte[] GetSharedSecret()
	{
		return m_shared;
	}

	public byte[] GetIV()
	{
		return m_iv;
	}
}
