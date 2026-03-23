using System;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class CertReqMsg : Asn1Encodable
{
	private readonly CertRequest m_certReq;

	private readonly ProofOfPossession m_pop;

	private readonly Asn1Sequence m_regInfo;

	public virtual CertRequest CertReq => m_certReq;

	public virtual ProofOfPossession Pop => m_pop;

	[Obsolete("Use 'Pop' instead")]
	public virtual ProofOfPossession Popo => m_pop;

	public static CertReqMsg GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertReqMsg result)
		{
			return result;
		}
		return new CertReqMsg(Asn1Sequence.GetInstance(obj));
	}

	public static CertReqMsg GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new CertReqMsg(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	private CertReqMsg(Asn1Sequence seq)
	{
		m_certReq = CertRequest.GetInstance(seq[0]);
		for (int i = 1; i < seq.Count; i++)
		{
			object obj = seq[i];
			if (obj is Asn1TaggedObject || obj is ProofOfPossession)
			{
				m_pop = ProofOfPossession.GetInstance(obj);
			}
			else
			{
				m_regInfo = Asn1Sequence.GetInstance(obj);
			}
		}
	}

	public CertReqMsg(CertRequest certReq, ProofOfPossession popo, AttributeTypeAndValue[] regInfo)
	{
		m_certReq = certReq ?? throw new ArgumentNullException("certReq");
		m_pop = popo;
		if (regInfo != null)
		{
			m_regInfo = new DerSequence(regInfo);
		}
	}

	public virtual AttributeTypeAndValue[] GetRegInfo()
	{
		return m_regInfo?.MapElements(AttributeTypeAndValue.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_certReq);
		asn1EncodableVector.AddOptional(m_pop, m_regInfo);
		return new DerSequence(asn1EncodableVector);
	}
}
