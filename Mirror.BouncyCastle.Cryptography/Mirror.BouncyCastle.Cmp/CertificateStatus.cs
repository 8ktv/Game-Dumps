using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cmp;

public class CertificateStatus
{
	private readonly IDigestAlgorithmFinder m_digestAlgorithmFinder;

	private readonly CertStatus m_certStatus;

	public virtual PkiStatusInfo StatusInfo => m_certStatus.StatusInfo;

	public virtual DerInteger CertReqID => m_certStatus.CertReqID;

	[Obsolete("Use 'CertReqID' instead")]
	public virtual BigInteger CertRequestID => m_certStatus.CertReqID.Value;

	[Obsolete("Use constructor taking 'IDigestAlgorithmFinder' instead")]
	public CertificateStatus(DefaultDigestAlgorithmIdentifierFinder digestAlgFinder, CertStatus certStatus)
		: this((IDigestAlgorithmFinder)digestAlgFinder, certStatus)
	{
	}

	public CertificateStatus(IDigestAlgorithmFinder digestAlgorithmFinder, CertStatus certStatus)
	{
		m_digestAlgorithmFinder = digestAlgorithmFinder;
		m_certStatus = certStatus;
	}

	public virtual bool IsVerified(X509Certificate cert)
	{
		return IsVerified(new CmpCertificate(cert.CertificateStructure), cert.SignatureAlgorithm);
	}

	public virtual bool IsVerified(CmpCertificate cmpCertificate, AlgorithmIdentifier signatureAlgorithm)
	{
		byte[] b = CmpUtilities.CalculateCertHash(cmpCertificate, signatureAlgorithm, m_digestAlgorithmFinder);
		return Arrays.FixedTimeEquals(m_certStatus.CertHash.GetOctets(), b);
	}
}
