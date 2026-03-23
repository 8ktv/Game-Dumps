using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class OriginatorInfoGenerator
{
	private readonly List<Asn1Encodable> origCerts;

	private readonly List<Asn1Encodable> origCrls;

	public OriginatorInfoGenerator(X509Certificate origCert)
	{
		origCerts = new List<Asn1Encodable> { origCert.CertificateStructure };
		origCrls = null;
	}

	public OriginatorInfoGenerator(IStore<X509Certificate> x509Certs)
		: this(x509Certs, null, null, null)
	{
	}

	public OriginatorInfoGenerator(IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls)
		: this(x509Certs, x509Crls, null, null)
	{
	}

	public OriginatorInfoGenerator(IStore<X509Certificate> x509Certs, IStore<X509Crl> x509Crls, IStore<X509V2AttributeCertificate> x509AttrCerts, IStore<OtherRevocationInfoFormat> otherRevocationInfos)
	{
		List<Asn1Encodable> list = null;
		if (x509Certs != null || x509AttrCerts != null)
		{
			list = new List<Asn1Encodable>();
			if (x509Certs != null)
			{
				list.AddRange(CmsUtilities.GetCertificatesFromStore(x509Certs));
			}
			if (x509AttrCerts != null)
			{
				list.AddRange(CmsUtilities.GetAttributeCertificatesFromStore(x509AttrCerts));
			}
		}
		List<Asn1Encodable> list2 = null;
		if (x509Crls != null || otherRevocationInfos != null)
		{
			list2 = new List<Asn1Encodable>();
			if (x509Crls != null)
			{
				list2.AddRange(CmsUtilities.GetCrlsFromStore(x509Crls));
			}
			if (otherRevocationInfos != null)
			{
				list2.AddRange(CmsUtilities.GetOtherRevocationInfosFromStore(otherRevocationInfos));
			}
		}
		origCerts = list;
		origCrls = list2;
	}

	public virtual OriginatorInfo Generate()
	{
		Asn1Set certs = ((origCerts == null) ? null : CmsUtilities.CreateDerSetFromList(origCerts));
		Asn1Set crls = ((origCrls == null) ? null : CmsUtilities.CreateDerSetFromList(origCrls));
		return new OriginatorInfo(certs, crls);
	}
}
