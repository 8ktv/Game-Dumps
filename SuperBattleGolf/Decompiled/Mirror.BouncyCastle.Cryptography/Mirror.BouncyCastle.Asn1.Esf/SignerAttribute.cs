using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class SignerAttribute : Asn1Encodable
{
	private readonly Asn1Sequence m_claimedAttributes;

	private readonly AttributeCertificate m_certifiedAttributes;

	public virtual Asn1Sequence ClaimedAttributes => m_claimedAttributes;

	public virtual AttributeCertificate CertifiedAttributes => m_certifiedAttributes;

	public static SignerAttribute GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignerAttribute result)
		{
			return result;
		}
		return new SignerAttribute(Asn1Sequence.GetInstance(obj), dummy: true);
	}

	private SignerAttribute(Asn1Sequence seq, bool dummy)
	{
		Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[0], 128);
		if (instance.TagNo == 0)
		{
			m_claimedAttributes = Asn1Sequence.GetInstance(instance, declaredExplicit: true);
			return;
		}
		if (instance.TagNo == 1)
		{
			m_certifiedAttributes = AttributeCertificate.GetInstance(instance, declaredExplicit: true);
			return;
		}
		throw new ArgumentException("illegal tag.", "seq");
	}

	public SignerAttribute(Asn1Sequence claimedAttributes)
	{
		m_claimedAttributes = claimedAttributes ?? throw new ArgumentNullException("claimedAttributes");
	}

	public SignerAttribute(AttributeCertificate certifiedAttributes)
	{
		m_certifiedAttributes = certifiedAttributes ?? throw new ArgumentNullException("certifiedAttributes");
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_claimedAttributes == null)
		{
			return new DerSequence(new DerTaggedObject(1, m_certifiedAttributes));
		}
		return new DerSequence(new DerTaggedObject(0, m_claimedAttributes));
	}
}
