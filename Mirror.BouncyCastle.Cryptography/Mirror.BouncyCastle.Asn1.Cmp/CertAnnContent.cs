using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CertAnnContent : CmpCertificate
{
	public new static CertAnnContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertAnnContent result)
		{
			return result;
		}
		if (obj is CmpCertificate other)
		{
			return new CertAnnContent(other);
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new CertAnnContent(taggedObject);
		}
		return new CertAnnContent(X509CertificateStructure.GetInstance(obj));
	}

	public new static CertAnnContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	[Obsolete("Use 'GetInstance' from tagged object instead")]
	public CertAnnContent(int type, Asn1Object otherCert)
		: base(type, otherCert)
	{
	}

	internal CertAnnContent(Asn1TaggedObject taggedObject)
		: base(taggedObject)
	{
	}

	internal CertAnnContent(CmpCertificate other)
		: base(other)
	{
	}

	public CertAnnContent(X509CertificateStructure x509v3PKCert)
		: base(x509v3PKCert)
	{
	}
}
