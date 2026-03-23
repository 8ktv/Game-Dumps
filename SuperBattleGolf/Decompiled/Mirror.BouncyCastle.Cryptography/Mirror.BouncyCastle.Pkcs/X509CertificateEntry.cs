using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkcs;

public class X509CertificateEntry : Pkcs12Entry
{
	private readonly X509Certificate cert;

	public X509Certificate Certificate => cert;

	public X509CertificateEntry(X509Certificate cert)
		: base(new Dictionary<DerObjectIdentifier, Asn1Encodable>())
	{
		this.cert = cert;
	}

	public X509CertificateEntry(X509Certificate cert, IDictionary<DerObjectIdentifier, Asn1Encodable> attributes)
		: base(attributes)
	{
		this.cert = cert;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is X509CertificateEntry x509CertificateEntry))
		{
			return false;
		}
		return cert.Equals(x509CertificateEntry.cert);
	}

	public override int GetHashCode()
	{
		return ~cert.GetHashCode();
	}
}
