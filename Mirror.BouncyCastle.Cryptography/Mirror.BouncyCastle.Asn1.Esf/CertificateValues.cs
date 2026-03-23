using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CertificateValues : Asn1Encodable
{
	private readonly Asn1Sequence m_certificates;

	public static CertificateValues GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CertificateValues result)
		{
			return result;
		}
		return new CertificateValues(Asn1Sequence.GetInstance(obj));
	}

	public static CertificateValues GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CertificateValues(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CertificateValues(Asn1Sequence seq)
	{
		m_certificates = seq;
		m_certificates.MapElements(X509CertificateStructure.GetInstance);
	}

	public CertificateValues(params X509CertificateStructure[] certificates)
	{
		if (certificates == null)
		{
			throw new ArgumentNullException("certificates");
		}
		m_certificates = DerSequence.FromElements(certificates);
	}

	public CertificateValues(IEnumerable<X509CertificateStructure> certificates)
	{
		if (certificates == null)
		{
			throw new ArgumentNullException("certificates");
		}
		m_certificates = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(certificates));
	}

	public X509CertificateStructure[] GetCertificates()
	{
		return m_certificates.MapElements(X509CertificateStructure.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_certificates;
	}
}
