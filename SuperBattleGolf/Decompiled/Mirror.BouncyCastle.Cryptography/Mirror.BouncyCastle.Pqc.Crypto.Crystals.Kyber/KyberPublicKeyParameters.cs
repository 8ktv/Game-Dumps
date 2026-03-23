using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public sealed class KyberPublicKeyParameters : KyberKeyParameters
{
	private readonly byte[] m_t;

	private readonly byte[] m_rho;

	internal static byte[] GetEncoded(byte[] t, byte[] rho)
	{
		return Arrays.Concatenate(t, rho);
	}

	public KyberPublicKeyParameters(KyberParameters parameters, byte[] t, byte[] rho)
		: base(isPrivate: false, parameters)
	{
		m_t = Arrays.Clone(t);
		m_rho = Arrays.Clone(rho);
	}

	public KyberPublicKeyParameters(KyberParameters parameters, byte[] encoding)
		: base(isPrivate: false, parameters)
	{
		m_t = Arrays.CopyOfRange(encoding, 0, encoding.Length - KyberEngine.SymBytes);
		m_rho = Arrays.CopyOfRange(encoding, encoding.Length - KyberEngine.SymBytes, encoding.Length);
	}

	public byte[] GetEncoded()
	{
		return GetEncoded(m_t, m_rho);
	}

	public byte[] GetRho()
	{
		return Arrays.Clone(m_rho);
	}

	public byte[] GetT()
	{
		return Arrays.Clone(m_t);
	}
}
