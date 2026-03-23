using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public sealed class DilithiumPublicKeyParameters : DilithiumKeyParameters
{
	internal readonly byte[] m_rho;

	internal readonly byte[] m_t1;

	internal static byte[] GetEncoded(byte[] rho, byte[] t1)
	{
		return Arrays.Concatenate(rho, t1);
	}

	public DilithiumPublicKeyParameters(DilithiumParameters parameters, byte[] rho, byte[] t1)
		: base(isPrivate: false, parameters)
	{
		m_rho = Arrays.Clone(rho);
		m_t1 = Arrays.Clone(t1);
	}

	public DilithiumPublicKeyParameters(DilithiumParameters parameters, byte[] pkEncoded)
		: base(isPrivate: false, parameters)
	{
		m_rho = Arrays.CopyOfRange(pkEncoded, 0, 32);
		m_t1 = Arrays.CopyOfRange(pkEncoded, 32, pkEncoded.Length);
	}

	public byte[] GetEncoded()
	{
		return GetEncoded(m_rho, m_t1);
	}

	public byte[] GetRho()
	{
		return Arrays.Clone(m_rho);
	}

	public byte[] GetT1()
	{
		return Arrays.Clone(m_t1);
	}
}
