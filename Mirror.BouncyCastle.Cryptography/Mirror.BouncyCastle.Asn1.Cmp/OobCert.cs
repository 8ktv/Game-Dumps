using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class OobCert : CmpCertificate
{
	public new static OobCert GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OobCert result)
		{
			return result;
		}
		if (obj is CmpCertificate other)
		{
			return new OobCert(other);
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new OobCert(taggedObject);
		}
		return new OobCert(X509CertificateStructure.GetInstance(obj));
	}

	public new static OobCert GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	[Obsolete("Use constructor from Asn1TaggedObject instead")]
	public OobCert(int type, Asn1Encodable otherCert)
		: base(type, otherCert)
	{
	}

	internal OobCert(Asn1TaggedObject taggedObject)
		: base(taggedObject)
	{
	}

	internal OobCert(CmpCertificate other)
		: base(other)
	{
	}

	public OobCert(X509CertificateStructure x509v3PKCert)
		: base(x509v3PKCert)
	{
	}
}
