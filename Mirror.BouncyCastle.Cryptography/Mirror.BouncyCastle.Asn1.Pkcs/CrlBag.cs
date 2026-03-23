using System;

namespace Mirror.BouncyCastle.Asn1.Pkcs;

public class CrlBag : Asn1Encodable
{
	private readonly DerObjectIdentifier m_crlID;

	private readonly Asn1Encodable m_crlValue;

	public virtual DerObjectIdentifier CrlID => m_crlID;

	public virtual Asn1Encodable CrlValue => m_crlValue;

	public static CrlBag GetInstance(object obj)
	{
		if (obj is CrlBag result)
		{
			return result;
		}
		if (obj == null)
		{
			return null;
		}
		return new CrlBag(Asn1Sequence.GetInstance(obj));
	}

	private CrlBag(Asn1Sequence seq)
	{
		if (seq.Count != 2)
		{
			throw new ArgumentException("Wrong number of elements in sequence", "seq");
		}
		m_crlID = DerObjectIdentifier.GetInstance(seq[0]);
		m_crlValue = Asn1TaggedObject.GetInstance(seq[1]).GetExplicitBaseObject();
	}

	public CrlBag(DerObjectIdentifier crlID, Asn1Encodable crlValue)
	{
		m_crlID = crlID;
		m_crlValue = crlValue;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_crlID, new DerTaggedObject(0, m_crlValue));
	}
}
