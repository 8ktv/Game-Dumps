using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherRevVals : Asn1Encodable
{
	private readonly DerObjectIdentifier m_otherRevValType;

	private readonly Asn1Encodable m_otherRevVals;

	public DerObjectIdentifier OtherRevValType => m_otherRevValType;

	public Asn1Encodable OtherRevValsData => m_otherRevVals;

	[Obsolete("Use 'OtherRevValsData' instead")]
	public Asn1Object OtherRevValsObject => m_otherRevVals.ToAsn1Object();

	public static OtherRevVals GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherRevVals result)
		{
			return result;
		}
		return new OtherRevVals(Asn1Sequence.GetInstance(obj));
	}

	public static OtherRevVals GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new OtherRevVals(Asn1Sequence.GetInstance(obj, explicitly));
	}

	private OtherRevVals(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_otherRevValType = DerObjectIdentifier.GetInstance(seq[0]);
		m_otherRevVals = seq[1];
	}

	public OtherRevVals(DerObjectIdentifier otherRevValType, Asn1Encodable otherRevVals)
	{
		m_otherRevValType = otherRevValType ?? throw new ArgumentNullException("otherRevValType");
		m_otherRevVals = otherRevVals ?? throw new ArgumentNullException("otherRevVals");
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_otherRevValType, m_otherRevVals);
	}
}
