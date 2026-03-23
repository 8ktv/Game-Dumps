using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Bike;

public sealed class BikePrivateKeyParameters : BikeKeyParameters
{
	internal readonly byte[] m_h0;

	internal readonly byte[] m_h1;

	internal readonly byte[] m_sigma;

	public BikePrivateKeyParameters(BikeParameters bikeParameters, byte[] h0, byte[] h1, byte[] sigma)
		: base(isPrivate: true, bikeParameters)
	{
		m_h0 = Arrays.Clone(h0);
		m_h1 = Arrays.Clone(h1);
		m_sigma = Arrays.Clone(sigma);
	}

	public byte[] GetEncoded()
	{
		return Arrays.ConcatenateAll(m_h0, m_h1, m_sigma);
	}

	public byte[] GetH0()
	{
		return Arrays.Clone(m_h0);
	}

	public byte[] GetH1()
	{
		return Arrays.Clone(m_h1);
	}

	public byte[] GetSigma()
	{
		return Arrays.Clone(m_sigma);
	}
}
