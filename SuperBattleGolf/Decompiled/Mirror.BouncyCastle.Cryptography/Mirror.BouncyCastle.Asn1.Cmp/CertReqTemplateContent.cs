using System;
using Mirror.BouncyCastle.Asn1.Crmf;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertReqTemplateContent : Asn1Encodable
{
	private readonly CertTemplate m_certTemplate;

	private readonly Asn1Sequence m_keySpec;

	public virtual CertTemplate CertTemplate => m_certTemplate;

	public virtual Asn1Sequence KeySpec => m_keySpec;

	public static CertReqTemplateContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertReqTemplateContent result)
		{
			return result;
		}
		return new CertReqTemplateContent(Asn1Sequence.GetInstance(obj));
	}

	public static CertReqTemplateContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertReqTemplateContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertReqTemplateContent(Asn1Sequence seq)
	{
		if (seq.Count != 1 && seq.Count != 2)
		{
			throw new ArgumentException("expected sequence size of 1 or 2");
		}
		m_certTemplate = CertTemplate.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_keySpec = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public CertReqTemplateContent(CertTemplate certTemplate, Asn1Sequence keySpec)
	{
		m_certTemplate = certTemplate;
		m_keySpec = keySpec;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_certTemplate);
		asn1EncodableVector.AddOptional(m_keySpec);
		return new DerSequence(asn1EncodableVector);
	}
}
