using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateResponseBuilder
{
	private readonly DerInteger m_certReqID;

	private readonly PkiStatusInfo m_statusInfo;

	private CertifiedKeyPair m_certKeyPair;

	private Asn1OctetString m_rspInfo;

	public CertificateResponseBuilder(DerInteger certReqID, PkiStatusInfo statusInfo)
	{
		m_certReqID = certReqID;
		m_statusInfo = statusInfo;
	}

	public virtual CertificateResponseBuilder WithCertificate(X509Certificate certificate)
	{
		if (m_certKeyPair != null)
		{
			throw new InvalidOperationException("certificate in response already set");
		}
		CmpCertificate certificate2 = new CmpCertificate(certificate.CertificateStructure);
		m_certKeyPair = new CertifiedKeyPair(new CertOrEncCert(certificate2));
		return this;
	}

	public virtual CertificateResponseBuilder WithCertificate(CmpCertificate cmpCertificate)
	{
		if (m_certKeyPair != null)
		{
			throw new InvalidOperationException("certificate in response already set");
		}
		m_certKeyPair = new CertifiedKeyPair(new CertOrEncCert(cmpCertificate));
		return this;
	}

	public virtual CertificateResponseBuilder WithCertificate(CmsEnvelopedData encryptedCertificate)
	{
		if (m_certKeyPair != null)
		{
			throw new InvalidOperationException("certificate in response already set");
		}
		EncryptedKey encryptedKey = new EncryptedKey(EnvelopedData.GetInstance(encryptedCertificate.ContentInfo.Content));
		m_certKeyPair = new CertifiedKeyPair(new CertOrEncCert(encryptedKey));
		return this;
	}

	public virtual CertificateResponseBuilder WithResponseInfo(byte[] responseInfo)
	{
		if (m_rspInfo != null)
		{
			throw new InvalidOperationException("response info already set");
		}
		m_rspInfo = new DerOctetString(responseInfo);
		return this;
	}

	public virtual CertificateResponse Build()
	{
		return new CertificateResponse(new CertResponse(m_certReqID, m_statusInfo, m_certKeyPair, m_rspInfo));
	}
}
