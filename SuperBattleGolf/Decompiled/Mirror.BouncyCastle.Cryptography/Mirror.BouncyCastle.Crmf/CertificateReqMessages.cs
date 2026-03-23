using System;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Crmf;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateReqMessages
{
	private readonly CertReqMsg[] m_reqs;

	public static CertificateReqMessages FromPkiBody(PkiBody pkiBody)
	{
		if (!IsCertificateRequestMessages(pkiBody.Type))
		{
			throw new ArgumentException("content of PKIBody wrong type: " + pkiBody.Type);
		}
		return new CertificateReqMessages(CertReqMessages.GetInstance(pkiBody.Content));
	}

	public static bool IsCertificateRequestMessages(int bodyType)
	{
		switch (bodyType)
		{
		case 0:
		case 2:
		case 7:
		case 9:
		case 13:
			return true;
		default:
			return false;
		}
	}

	public CertificateReqMessages(CertReqMessages certReqMessages)
	{
		m_reqs = certReqMessages.ToCertReqMsgArray();
	}

	public virtual CertificateRequestMessage[] GetRequests()
	{
		return Array.ConvertAll(m_reqs, (CertReqMsg req) => new CertificateRequestMessage(req));
	}

	public virtual CertReqMessages ToAsn1Structure()
	{
		return new CertReqMessages(m_reqs);
	}
}
