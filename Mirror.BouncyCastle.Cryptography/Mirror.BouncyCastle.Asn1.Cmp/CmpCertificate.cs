using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class CmpCertificate : Asn1Encodable, IAsn1Choice
{
	private readonly X509CertificateStructure m_x509v3PKCert;

	private readonly int m_otherTag;

	private readonly Asn1Encodable m_otherObject;

	public virtual bool IsX509v3PKCert => m_x509v3PKCert != null;

	public virtual X509CertificateStructure X509v3PKCert => m_x509v3PKCert;

	public virtual int OtherCertTag => m_otherTag;

	public virtual Asn1Encodable OtherCert => m_otherObject;

	public static CmpCertificate GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CmpCertificate result)
		{
			return result;
		}
		if (obj is X509CertificateStructure x509v3PKCert)
		{
			return new CmpCertificate(x509v3PKCert);
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new CmpCertificate(taggedObject);
		}
		Asn1Object asn1Object = null;
		if (obj is IAsn1Convertible asn1Convertible)
		{
			asn1Object = asn1Convertible.ToAsn1Object();
		}
		else if (obj is byte[] data)
		{
			asn1Object = Asn1Object.FromByteArray(data);
		}
		if (asn1Object is Asn1TaggedObject taggedObject2)
		{
			return new CmpCertificate(taggedObject2);
		}
		return new CmpCertificate(X509CertificateStructure.GetInstance(asn1Object ?? obj));
	}

	public static CmpCertificate GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	[Obsolete("Use 'GetInstance' from tagged object instead")]
	public CmpCertificate(int type, Asn1Encodable otherCert)
	{
		m_otherTag = type;
		m_otherObject = otherCert;
	}

	internal CmpCertificate(Asn1TaggedObject taggedObject)
	{
		if (taggedObject.HasContextTag(1))
		{
			AttributeCertificate.GetInstance(taggedObject, declaredExplicit: true);
			m_otherTag = taggedObject.TagNo;
			m_otherObject = taggedObject.GetExplicitBaseObject();
			return;
		}
		throw new ArgumentException("Invalid CHOICE element", "taggedObject");
	}

	internal CmpCertificate(CmpCertificate other)
	{
		m_x509v3PKCert = other.m_x509v3PKCert;
		m_otherTag = other.m_otherTag;
		m_otherObject = other.m_otherObject;
	}

	public CmpCertificate(X509CertificateStructure x509v3PKCert)
	{
		if (x509v3PKCert.Version != 3)
		{
			throw new ArgumentException("only version 3 certificates allowed", "x509v3PKCert");
		}
		m_x509v3PKCert = x509v3PKCert;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_otherObject != null)
		{
			return new DerTaggedObject(isExplicit: true, m_otherTag, m_otherObject);
		}
		if (m_x509v3PKCert != null)
		{
			return m_x509v3PKCert.ToAsn1Object();
		}
		throw new InvalidOperationException();
	}
}
