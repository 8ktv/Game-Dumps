using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertRepMessage : Asn1Encodable
{
	private readonly Asn1Sequence m_caPubs;

	private readonly Asn1Sequence m_response;

	public static CertRepMessage GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertRepMessage result)
		{
			return result;
		}
		return new CertRepMessage(Asn1Sequence.GetInstance(obj));
	}

	public static CertRepMessage GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertRepMessage(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertRepMessage(Asn1Sequence seq)
	{
		int index = 0;
		if (seq.Count > 1)
		{
			m_caPubs = Asn1Sequence.GetInstance((Asn1TaggedObject)seq[index++], declaredExplicit: true);
		}
		m_response = Asn1Sequence.GetInstance(seq[index]);
	}

	public CertRepMessage(CmpCertificate[] caPubs, CertResponse[] response)
	{
		if (response == null)
		{
			throw new ArgumentNullException("response");
		}
		Asn1Encodable[] elements;
		if (caPubs != null && caPubs.Length != 0)
		{
			elements = caPubs;
			m_caPubs = new DerSequence(elements);
		}
		elements = response;
		m_response = new DerSequence(elements);
	}

	public virtual CmpCertificate[] GetCAPubs()
	{
		return m_caPubs?.MapElements(CmpCertificate.GetInstance);
	}

	public virtual CertResponse[] GetResponse()
	{
		return m_response.MapElements(CertResponse.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_caPubs);
		asn1EncodableVector.Add(m_response);
		return new DerSequence(asn1EncodableVector);
	}
}
