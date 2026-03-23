using System;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class RevRepContentBuilder
{
	private readonly Asn1EncodableVector m_status = new Asn1EncodableVector();

	private readonly Asn1EncodableVector m_revCerts = new Asn1EncodableVector();

	private readonly Asn1EncodableVector m_crls = new Asn1EncodableVector();

	public virtual RevRepContentBuilder Add(PkiStatusInfo status)
	{
		m_status.Add(status);
		return this;
	}

	public virtual RevRepContentBuilder Add(PkiStatusInfo status, CertId certId)
	{
		if (m_status.Count != m_revCerts.Count)
		{
			throw new InvalidOperationException("status and revCerts sequence must be in common order");
		}
		m_status.Add(status);
		m_revCerts.Add(certId);
		return this;
	}

	public virtual RevRepContentBuilder AddCrl(CertificateList crl)
	{
		m_crls.Add(crl);
		return this;
	}

	public virtual RevRepContent Build()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(new DerSequence(m_status));
		if (m_revCerts.Count != 0)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 0, new DerSequence(m_revCerts)));
		}
		if (m_crls.Count != 0)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 1, new DerSequence(m_crls)));
		}
		return RevRepContent.GetInstance(new DerSequence(asn1EncodableVector));
	}
}
