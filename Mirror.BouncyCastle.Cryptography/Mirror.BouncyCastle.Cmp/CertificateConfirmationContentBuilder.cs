using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cmp;

public sealed class CertificateConfirmationContentBuilder
{
	private readonly IDigestAlgorithmFinder m_digestAlgorithmFinder;

	private readonly List<CmpCertificate> m_acceptedCerts = new List<CmpCertificate>();

	private readonly List<AlgorithmIdentifier> m_acceptedSignatureAlgorithms = new List<AlgorithmIdentifier>();

	private readonly List<DerInteger> m_acceptedReqIDs = new List<DerInteger>();

	public CertificateConfirmationContentBuilder()
		: this(DefaultDigestAlgorithmFinder.Instance)
	{
	}

	[Obsolete("Use constructor taking 'IDigestAlgorithmFinder' instead")]
	public CertificateConfirmationContentBuilder(DefaultDigestAlgorithmIdentifierFinder digestAlgFinder)
		: this((IDigestAlgorithmFinder)digestAlgFinder)
	{
	}

	public CertificateConfirmationContentBuilder(IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		m_digestAlgorithmFinder = digestAlgorithmFinder;
	}

	public CertificateConfirmationContentBuilder AddAcceptedCertificate(X509Certificate certHolder, BigInteger certReqId)
	{
		return AddAcceptedCertificate(certHolder, new DerInteger(certReqId));
	}

	public CertificateConfirmationContentBuilder AddAcceptedCertificate(X509Certificate cert, DerInteger certReqID)
	{
		return AddAcceptedCertificate(new CmpCertificate(cert.CertificateStructure), cert.SignatureAlgorithm, certReqID);
	}

	public CertificateConfirmationContentBuilder AddAcceptedCertificate(CmpCertificate cmpCertificate, AlgorithmIdentifier signatureAlgorithm, DerInteger certReqID)
	{
		m_acceptedCerts.Add(cmpCertificate);
		m_acceptedSignatureAlgorithms.Add(signatureAlgorithm);
		m_acceptedReqIDs.Add(certReqID);
		return this;
	}

	public CertificateConfirmationContent Build()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_acceptedCerts.Count);
		for (int i = 0; i != m_acceptedCerts.Count; i++)
		{
			byte[] certHash = CmpUtilities.CalculateCertHash(m_acceptedCerts[i], m_acceptedSignatureAlgorithms[i], m_digestAlgorithmFinder);
			DerInteger certReqID = m_acceptedReqIDs[i];
			asn1EncodableVector.Add(new CertStatus(certHash, certReqID));
		}
		return new CertificateConfirmationContent(CertConfirmContent.GetInstance(new DerSequence(asn1EncodableVector)), m_digestAlgorithmFinder);
	}
}
