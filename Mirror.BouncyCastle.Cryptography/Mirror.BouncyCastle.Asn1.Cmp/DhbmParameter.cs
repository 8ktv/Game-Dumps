using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class DhbmParameter : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_owf;

	private readonly AlgorithmIdentifier m_mac;

	public virtual AlgorithmIdentifier Owf => m_owf;

	public virtual AlgorithmIdentifier Mac => m_mac;

	public static DhbmParameter GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DhbmParameter result)
		{
			return result;
		}
		return new DhbmParameter(Asn1Sequence.GetInstance(obj));
	}

	public static DhbmParameter GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new DhbmParameter(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private DhbmParameter(Asn1Sequence sequence)
	{
		if (sequence.Count != 2)
		{
			throw new ArgumentException("expecting sequence size of 2");
		}
		m_owf = AlgorithmIdentifier.GetInstance(sequence[0]);
		m_mac = AlgorithmIdentifier.GetInstance(sequence[1]);
	}

	public DhbmParameter(AlgorithmIdentifier owf, AlgorithmIdentifier mac)
	{
		m_owf = owf;
		m_mac = mac;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_owf, m_mac);
	}
}
