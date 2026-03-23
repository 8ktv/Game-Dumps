using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class CertificateID : IEquatable<CertificateID>
{
	[Obsolete("Use 'OiwObjectIdentifiers.IdSha1.Id' instead")]
	public const string HashSha1 = "1.3.14.3.2.26";

	public static readonly AlgorithmIdentifier DigestSha1 = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);

	private readonly CertID m_id;

	public string HashAlgOid => m_id.HashAlgorithm.Algorithm.Id;

	public BigInteger SerialNumber => m_id.SerialNumber.Value;

	public CertificateID(CertID id)
	{
		m_id = id ?? throw new ArgumentNullException("id");
	}

	[Obsolete("Will be removed")]
	public CertificateID(string hashAlgorithm, X509Certificate issuerCert, BigInteger serialNumber)
	{
		AlgorithmIdentifier digestAlgorithm = new AlgorithmIdentifier(new DerObjectIdentifier(hashAlgorithm), DerNull.Instance);
		m_id = CreateCertID(digestAlgorithm, issuerCert, new DerInteger(serialNumber));
	}

	public CertificateID(AlgorithmIdentifier digestAlgorithm, X509Certificate issuerCert, BigInteger serialNumber)
	{
		m_id = CreateCertID(digestAlgorithm, issuerCert, new DerInteger(serialNumber));
	}

	public CertificateID(IDigestFactory digestFactory, X509Certificate issuerCert, BigInteger serialNumber)
	{
		m_id = CreateCertID(digestFactory, issuerCert, new DerInteger(serialNumber));
	}

	public byte[] GetIssuerNameHash()
	{
		return m_id.IssuerNameHash.GetOctets();
	}

	public byte[] GetIssuerKeyHash()
	{
		return m_id.IssuerKeyHash.GetOctets();
	}

	public bool MatchesIssuer(X509Certificate issuerCert)
	{
		return CreateCertID(m_id.HashAlgorithm, issuerCert, m_id.SerialNumber).Equals(m_id);
	}

	public bool MatchesIssuer(IDigestFactory digestFactory, X509Certificate issuerCert)
	{
		if (!m_id.HashAlgorithm.Equals(digestFactory.AlgorithmDetails))
		{
			throw new ArgumentException("digest factory does not match required digest algorithm");
		}
		return CreateCertID(digestFactory, issuerCert, m_id.SerialNumber).Equals(m_id);
	}

	public CertID ToAsn1Object()
	{
		return m_id;
	}

	public bool Equals(CertificateID other)
	{
		if (this != other)
		{
			return m_id.Equals(other?.m_id);
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as CertificateID);
	}

	public override int GetHashCode()
	{
		return m_id.GetHashCode();
	}

	public static CertificateID DeriveCertificateID(CertificateID original, BigInteger newSerialNumber)
	{
		CertID certID = original.ToAsn1Object();
		return new CertificateID(new CertID(certID.HashAlgorithm, certID.IssuerNameHash, certID.IssuerKeyHash, new DerInteger(newSerialNumber)));
	}

	private static CertID CreateCertID(AlgorithmIdentifier digestAlgorithm, X509Certificate issuerCert, DerInteger serialNumber)
	{
		try
		{
			X509Name subjectDN = issuerCert.SubjectDN;
			byte[] contents = X509Utilities.CalculateDigest(digestAlgorithm, subjectDN);
			byte[] bytes = issuerCert.SubjectPublicKeyInfo.PublicKey.GetBytes();
			byte[] contents2 = DigestUtilities.CalculateDigest(digestAlgorithm.Algorithm, bytes);
			return new CertID(digestAlgorithm, new DerOctetString(contents), new DerOctetString(contents2), serialNumber);
		}
		catch (Exception ex)
		{
			throw new OcspException("problem creating ID: " + ex, ex);
		}
	}

	private static CertID CreateCertID(IDigestFactory digestFactory, X509Certificate issuerCert, DerInteger serialNumber)
	{
		try
		{
			X509Name subjectDN = issuerCert.SubjectDN;
			byte[] contents = X509Utilities.CalculateDigest(digestFactory, subjectDN);
			byte[] bytes = issuerCert.SubjectPublicKeyInfo.PublicKey.GetBytes();
			byte[] contents2 = X509Utilities.CalculateDigest(digestFactory, bytes, 0, bytes.Length);
			return new CertID((AlgorithmIdentifier)digestFactory.AlgorithmDetails, new DerOctetString(contents), new DerOctetString(contents2), serialNumber);
		}
		catch (Exception ex)
		{
			throw new OcspException("problem creating ID: " + ex, ex);
		}
	}
}
