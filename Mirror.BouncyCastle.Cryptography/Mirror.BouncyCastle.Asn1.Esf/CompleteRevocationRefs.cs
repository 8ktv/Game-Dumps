using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CompleteRevocationRefs : Asn1Encodable
{
	private readonly Asn1Sequence m_crlOcspRefs;

	public static CompleteRevocationRefs GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CompleteRevocationRefs result)
		{
			return result;
		}
		return new CompleteRevocationRefs(Asn1Sequence.GetInstance(obj));
	}

	public static CompleteRevocationRefs GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CompleteRevocationRefs(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CompleteRevocationRefs(Asn1Sequence seq)
	{
		m_crlOcspRefs = seq;
		m_crlOcspRefs.MapElements(CrlOcspRef.GetInstance);
	}

	public CompleteRevocationRefs(params CrlOcspRef[] crlOcspRefs)
	{
		if (crlOcspRefs == null)
		{
			throw new ArgumentNullException("crlOcspRefs");
		}
		m_crlOcspRefs = DerSequence.FromElements(crlOcspRefs);
	}

	public CompleteRevocationRefs(IEnumerable<CrlOcspRef> crlOcspRefs)
	{
		if (crlOcspRefs == null)
		{
			throw new ArgumentNullException("crlOcspRefs");
		}
		m_crlOcspRefs = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(crlOcspRefs));
	}

	public CrlOcspRef[] GetCrlOcspRefs()
	{
		return m_crlOcspRefs.MapElements(CrlOcspRef.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_crlOcspRefs;
	}
}
