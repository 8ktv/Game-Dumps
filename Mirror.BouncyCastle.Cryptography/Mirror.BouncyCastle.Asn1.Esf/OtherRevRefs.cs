using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherRevRefs : Asn1Encodable
{
	private readonly DerObjectIdentifier m_otherRevRefType;

	private readonly Asn1Encodable m_otherRevRefs;

	public DerObjectIdentifier OtherRevRefType => m_otherRevRefType;

	public Asn1Encodable OtherRevRefsData => m_otherRevRefs;

	[Obsolete("Use 'OtherRevRefsData' instead")]
	public Asn1Object OtherRevRefsObject => m_otherRevRefs.ToAsn1Object();

	public static OtherRevRefs GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherRevRefs result)
		{
			return result;
		}
		return new OtherRevRefs(Asn1Sequence.GetInstance(obj));
	}

	public static OtherRevRefs GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OtherRevRefs(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OtherRevRefs(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_otherRevRefType = DerObjectIdentifier.GetInstance(seq[0]);
		m_otherRevRefs = seq[1];
	}

	public OtherRevRefs(DerObjectIdentifier otherRevRefType, Asn1Encodable otherRevRefs)
	{
		m_otherRevRefType = otherRevRefType ?? throw new ArgumentNullException("otherRevRefType");
		m_otherRevRefs = otherRevRefs ?? throw new ArgumentNullException("otherRevRefs");
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_otherRevRefType, m_otherRevRefs);
	}
}
