using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsCertificate : BcTlsRawKeyCertificate
{
	protected readonly X509CertificateStructure m_certificate;

	public virtual X509CertificateStructure X509CertificateStructure => m_certificate;

	public override BigInteger SerialNumber => m_certificate.SerialNumber.Value;

	public override string SigAlgOid => m_certificate.SignatureAlgorithm.Algorithm.Id;

	public static BcTlsCertificate Convert(BcTlsCrypto crypto, TlsCertificate certificate)
	{
		if (certificate is BcTlsCertificate)
		{
			return (BcTlsCertificate)certificate;
		}
		return new BcTlsCertificate(crypto, certificate.GetEncoded());
	}

	public static X509CertificateStructure ParseCertificate(byte[] encoding)
	{
		try
		{
			return X509CertificateStructure.GetInstance(TlsUtilities.ReadAsn1Object(encoding));
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(42, alertCause);
		}
	}

	public BcTlsCertificate(BcTlsCrypto crypto, byte[] encoding)
		: this(crypto, ParseCertificate(encoding))
	{
	}

	public BcTlsCertificate(BcTlsCrypto crypto, X509CertificateStructure certificate)
		: base(crypto, certificate.SubjectPublicKeyInfo)
	{
		m_certificate = certificate;
	}

	public override byte[] GetEncoded()
	{
		return m_certificate.GetEncoded("DER");
	}

	public override byte[] GetExtension(DerObjectIdentifier extensionOid)
	{
		X509Extensions extensions = m_certificate.TbsCertificate.Extensions;
		if (extensions != null)
		{
			X509Extension extension = extensions.GetExtension(extensionOid);
			if (extension != null)
			{
				return Arrays.Clone(extension.Value.GetOctets());
			}
		}
		return null;
	}

	public override Asn1Encodable GetSigAlgParams()
	{
		return m_certificate.SignatureAlgorithm.Parameters;
	}

	protected override bool SupportsKeyUsage(int keyUsageBits)
	{
		X509Extensions extensions = m_certificate.TbsCertificate.Extensions;
		if (extensions != null)
		{
			KeyUsage keyUsage = KeyUsage.FromExtensions(extensions);
			if (keyUsage != null && (keyUsage.GetBytes()[0] & 0xFF & keyUsageBits) != keyUsageBits)
			{
				return false;
			}
		}
		return true;
	}
}
