using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class CertificateRepMessageBuilder
{
	private readonly List<CertResponse> m_responses = new List<CertResponse>();

	private readonly CmpCertificate[] m_caCerts;

	public CertificateRepMessageBuilder(params X509Certificate[] caCerts)
	{
		m_caCerts = Array.ConvertAll(caCerts, (X509Certificate caCert) => new CmpCertificate(caCert.CertificateStructure));
	}

	public virtual CertificateRepMessageBuilder AddCertificateResponse(CertificateResponse response)
	{
		m_responses.Add(response.ToAsn1Structure());
		return this;
	}

	public virtual CertificateRepMessage Build()
	{
		CmpCertificate[] array = m_caCerts;
		if (array.Length < 1)
		{
			array = null;
		}
		CertRepMessage repMessage = new CertRepMessage(array, m_responses.ToArray());
		m_responses.Clear();
		return new CertificateRepMessage(repMessage);
	}
}
