using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class KemCiphertextInfo : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_kem;

	private readonly Asn1OctetString m_ct;

	public virtual AlgorithmIdentifier Kem => m_kem;

	public virtual Asn1OctetString Ct => m_ct;

	public static KemCiphertextInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KemCiphertextInfo result)
		{
			return result;
		}
		return new KemCiphertextInfo(Asn1Sequence.GetInstance(obj));
	}

	public static KemCiphertextInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KemCiphertextInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private KemCiphertextInfo(Asn1Sequence seq)
	{
		if (seq.Count != 2)
		{
			throw new ArgumentException("sequence size should 2", "seq");
		}
		m_kem = AlgorithmIdentifier.GetInstance(seq[0]);
		m_ct = Asn1OctetString.GetInstance(seq[1]);
	}

	public KemCiphertextInfo(AlgorithmIdentifier kem, Asn1OctetString ct)
	{
		m_kem = kem;
		m_ct = ct;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_kem, m_ct);
	}
}
