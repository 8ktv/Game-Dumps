using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

[Obsolete("Will be removed as this draft proposal was rejected")]
public sealed class KyberPublicKey : Asn1Encodable
{
	private readonly byte[] m_t;

	private readonly byte[] m_rho;

	public byte[] T => Arrays.Clone(m_t);

	public byte[] Rho => Arrays.Clone(m_rho);

	public static KyberPublicKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KyberPublicKey result)
		{
			return result;
		}
		return new KyberPublicKey(Asn1Sequence.GetInstance(obj));
	}

	public static KyberPublicKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public KyberPublicKey(byte[] t, byte[] rho)
	{
		m_t = t;
		m_rho = rho;
	}

	private KyberPublicKey(Asn1Sequence seq)
	{
		m_t = Arrays.Clone(Asn1OctetString.GetInstance(seq[0]).GetOctets());
		m_rho = Arrays.Clone(Asn1OctetString.GetInstance(seq[1]).GetOctets());
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(new DerOctetString(m_t), new DerOctetString(m_rho));
	}
}
