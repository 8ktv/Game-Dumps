using System;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Cms;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateResponse
{
	private readonly CertResponse m_certResponse;

	public virtual bool HasEncryptedCertificate => m_certResponse.CertifiedKeyPair.CertOrEncCert.HasEncryptedCertificate;

	public CertificateResponse(CertResponse certResponse)
	{
		m_certResponse = certResponse;
	}

	public virtual CmsEnvelopedData GetEncryptedCertificate()
	{
		if (!HasEncryptedCertificate)
		{
			throw new InvalidOperationException("encrypted certificate asked for, none found");
		}
		CertifiedKeyPair certifiedKeyPair = m_certResponse.CertifiedKeyPair;
		CmsEnvelopedData cmsEnvelopedData = new CmsEnvelopedData(new Mirror.BouncyCastle.Asn1.Cms.ContentInfo(PkcsObjectIdentifiers.EnvelopedData, certifiedKeyPair.CertOrEncCert.EncryptedCert.Value));
		if (cmsEnvelopedData.GetRecipientInfos().Count != 1)
		{
			throw new InvalidOperationException("data encrypted for more than one recipient");
		}
		return cmsEnvelopedData;
	}

	public virtual CmpCertificate GetCertificate()
	{
		if (HasEncryptedCertificate)
		{
			throw new InvalidOperationException("plaintext certificate asked for, none found");
		}
		return m_certResponse.CertifiedKeyPair.CertOrEncCert.Certificate;
	}

	public virtual CertResponse ToAsn1Structure()
	{
		return m_certResponse;
	}
}
