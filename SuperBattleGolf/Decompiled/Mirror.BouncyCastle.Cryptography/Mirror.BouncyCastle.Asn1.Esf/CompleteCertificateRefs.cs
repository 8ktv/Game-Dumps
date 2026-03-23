using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CompleteCertificateRefs : Asn1Encodable
{
	private readonly Asn1Sequence m_otherCertIDs;

	public static CompleteCertificateRefs GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CompleteCertificateRefs result)
		{
			return result;
		}
		return new CompleteCertificateRefs(Asn1Sequence.GetInstance(obj));
	}

	public static CompleteCertificateRefs GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CompleteCertificateRefs(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CompleteCertificateRefs(Asn1Sequence seq)
	{
		m_otherCertIDs = seq;
		m_otherCertIDs.MapElements(OtherCertID.GetInstance);
	}

	public CompleteCertificateRefs(params OtherCertID[] otherCertIDs)
	{
		if (otherCertIDs == null)
		{
			throw new ArgumentNullException("otherCertIDs");
		}
		m_otherCertIDs = DerSequence.FromElements(otherCertIDs);
	}

	public CompleteCertificateRefs(IEnumerable<OtherCertID> otherCertIDs)
	{
		if (otherCertIDs == null)
		{
			throw new ArgumentNullException("otherCertIDs");
		}
		m_otherCertIDs = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(otherCertIDs));
	}

	public OtherCertID[] GetOtherCertIDs()
	{
		return m_otherCertIDs.MapElements(OtherCertID.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_otherCertIDs;
	}
}
