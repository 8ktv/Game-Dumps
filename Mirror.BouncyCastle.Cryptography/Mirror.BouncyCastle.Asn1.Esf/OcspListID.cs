using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OcspListID : Asn1Encodable
{
	private readonly Asn1Sequence m_ocspResponses;

	public static OcspListID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OcspListID result)
		{
			return result;
		}
		return new OcspListID(Asn1Sequence.GetInstance(obj));
	}

	public static OcspListID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OcspListID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OcspListID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 1)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_ocspResponses = Asn1Sequence.GetInstance(seq[0]);
		m_ocspResponses.MapElements(OcspResponsesID.GetInstance);
	}

	public OcspListID(params OcspResponsesID[] ocspResponses)
	{
		if (ocspResponses == null)
		{
			throw new ArgumentNullException("ocspResponses");
		}
		m_ocspResponses = DerSequence.FromElements(ocspResponses);
	}

	public OcspListID(IEnumerable<OcspResponsesID> ocspResponses)
	{
		if (ocspResponses == null)
		{
			throw new ArgumentNullException("ocspResponses");
		}
		m_ocspResponses = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(ocspResponses));
	}

	public OcspResponsesID[] GetOcspResponses()
	{
		return m_ocspResponses.MapElements(OcspResponsesID.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_ocspResponses);
	}
}
