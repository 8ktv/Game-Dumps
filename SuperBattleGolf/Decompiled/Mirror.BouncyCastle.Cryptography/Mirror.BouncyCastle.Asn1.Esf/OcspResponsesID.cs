using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OcspResponsesID : Asn1Encodable
{
	private readonly OcspIdentifier m_ocspIdentifier;

	private readonly OtherHash m_ocspRepHash;

	public OcspIdentifier OcspIdentifier => m_ocspIdentifier;

	public OtherHash OcspRepHash => m_ocspRepHash;

	public static OcspResponsesID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OcspResponsesID result)
		{
			return result;
		}
		return new OcspResponsesID(Asn1Sequence.GetInstance(obj));
	}

	public static OcspResponsesID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OcspResponsesID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OcspResponsesID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_ocspIdentifier = OcspIdentifier.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_ocspRepHash = OtherHash.GetInstance(seq[1]);
		}
	}

	public OcspResponsesID(OcspIdentifier ocspIdentifier)
		: this(ocspIdentifier, null)
	{
	}

	public OcspResponsesID(OcspIdentifier ocspIdentifier, OtherHash ocspRepHash)
	{
		m_ocspIdentifier = ocspIdentifier ?? throw new ArgumentNullException("ocspIdentifier");
		m_ocspRepHash = ocspRepHash;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_ocspIdentifier);
		asn1EncodableVector.AddOptional(m_ocspRepHash);
		return new DerSequence(asn1EncodableVector);
	}
}
