using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CrlOcspRef : Asn1Encodable
{
	private readonly CrlListID m_crlids;

	private readonly OcspListID m_ocspids;

	private readonly OtherRevRefs m_otherRev;

	public CrlListID CrlIDs => m_crlids;

	public OcspListID OcspIDs => m_ocspids;

	public OtherRevRefs OtherRev => m_otherRev;

	public static CrlOcspRef GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlOcspRef result)
		{
			return result;
		}
		return new CrlOcspRef(Asn1Sequence.GetInstance(obj));
	}

	public static CrlOcspRef GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CrlOcspRef(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CrlOcspRef(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 0 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_crlids = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, CrlListID.GetInstance);
		m_ocspids = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, OcspListID.GetInstance);
		m_otherRev = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 2, state: true, OtherRevRefs.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public CrlOcspRef(CrlListID crlids, OcspListID ocspids, OtherRevRefs otherRev)
	{
		m_crlids = crlids;
		m_ocspids = ocspids;
		m_otherRev = otherRev;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_crlids);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_ocspids);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_otherRev);
		return DerSequence.FromVector(asn1EncodableVector);
	}
}
