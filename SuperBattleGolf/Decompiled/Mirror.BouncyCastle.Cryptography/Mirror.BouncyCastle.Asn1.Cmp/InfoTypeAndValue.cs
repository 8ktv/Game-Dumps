using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class InfoTypeAndValue : Asn1Encodable
{
	private readonly DerObjectIdentifier m_infoType;

	private readonly Asn1Encodable m_infoValue;

	public virtual DerObjectIdentifier InfoType => m_infoType;

	public virtual Asn1Encodable InfoValue => m_infoValue;

	public static InfoTypeAndValue GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is InfoTypeAndValue result)
		{
			return result;
		}
		return new InfoTypeAndValue(Asn1Sequence.GetInstance(obj));
	}

	public static InfoTypeAndValue GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new InfoTypeAndValue(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private InfoTypeAndValue(Asn1Sequence seq)
	{
		m_infoType = DerObjectIdentifier.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_infoValue = seq[1];
		}
	}

	public InfoTypeAndValue(DerObjectIdentifier infoType)
		: this(infoType, null)
	{
	}

	public InfoTypeAndValue(DerObjectIdentifier infoType, Asn1Encodable infoValue)
	{
		m_infoType = infoType ?? throw new ArgumentNullException("infoType");
		m_infoValue = infoValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_infoValue == null)
		{
			return new DerSequence(m_infoType);
		}
		return new DerSequence(m_infoType, m_infoValue);
	}
}
