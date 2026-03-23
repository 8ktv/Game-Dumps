using System;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class CrlID : Asn1Encodable
{
	private readonly DerIA5String m_crlUrl;

	private readonly DerInteger m_crlNum;

	private readonly Asn1GeneralizedTime m_crlTime;

	public DerIA5String CrlUrl => m_crlUrl;

	public DerInteger CrlNum => m_crlNum;

	public Asn1GeneralizedTime CrlTime => m_crlTime;

	public static CrlID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CrlID result)
		{
			return result;
		}
		return new CrlID(Asn1Sequence.GetInstance(obj));
	}

	public static CrlID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CrlID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public CrlID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 0 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_crlUrl = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, DerIA5String.GetInstance);
		m_crlNum = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 1, state: true, DerInteger.GetInstance);
		m_crlTime = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 2, state: true, Asn1GeneralizedTime.GetInstance);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_crlUrl);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 1, m_crlNum);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 2, m_crlTime);
		return new DerSequence(asn1EncodableVector);
	}
}
