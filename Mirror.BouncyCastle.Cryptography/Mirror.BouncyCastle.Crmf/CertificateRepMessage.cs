using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateRepMessage
{
	private readonly CertResponse[] m_resps;

	private readonly CmpCertificate[] m_caCerts;

	public static CertificateRepMessage FromPkiBody(PkiBody pkiBody)
	{
		if (!IsCertificateRepMessage(pkiBody.Type))
		{
			throw new ArgumentException("content of PKIBody wrong type: " + pkiBody.Type);
		}
		return new CertificateRepMessage(CertRepMessage.GetInstance(pkiBody.Content));
	}

	public static bool IsCertificateRepMessage(int bodyType)
	{
		switch (bodyType)
		{
		case 1:
		case 3:
		case 8:
		case 14:
			return true;
		default:
			return false;
		}
	}

	public CertificateRepMessage(CertRepMessage repMessage)
	{
		m_resps = repMessage.GetResponse();
		m_caCerts = repMessage.GetCAPubs();
	}

	public virtual CertificateResponse[] GetResponses()
	{
		return Array.ConvertAll(m_resps, (CertResponse resp) => new CertificateResponse(resp));
	}

	public virtual X509Certificate[] GetX509Certificates()
	{
		List<X509Certificate> list = new List<X509Certificate>();
		CmpCertificate[] caCerts = m_caCerts;
		foreach (CmpCertificate cmpCertificate in caCerts)
		{
			if (cmpCertificate.IsX509v3PKCert)
			{
				list.Add(new X509Certificate(cmpCertificate.X509v3PKCert));
			}
		}
		return list.ToArray();
	}

	public virtual bool IsOnlyX509PKCertificates()
	{
		bool flag = true;
		CmpCertificate[] caCerts = m_caCerts;
		foreach (CmpCertificate cmpCertificate in caCerts)
		{
			flag &= cmpCertificate.IsX509v3PKCert;
		}
		return flag;
	}

	public virtual CmpCertificate[] GetCmpCertificates()
	{
		return (CmpCertificate[])m_caCerts.Clone();
	}

	public virtual CertRepMessage ToAsn1Structure()
	{
		return new CertRepMessage(m_caCerts, m_resps);
	}
}
