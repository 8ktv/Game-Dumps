namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiMessages : Asn1Encodable
{
	private Asn1Sequence m_content;

	public static PkiMessages GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiMessages result)
		{
			return result;
		}
		return new PkiMessages(Asn1Sequence.GetInstance(obj));
	}

	public static PkiMessages GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PkiMessages(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	internal PkiMessages(Asn1Sequence seq)
	{
		m_content = seq;
	}

	internal PkiMessages(PkiMessages other)
	{
		m_content = other.m_content;
	}

	public PkiMessages(params PkiMessage[] msgs)
	{
		m_content = new DerSequence(msgs);
	}

	public virtual PkiMessage[] ToPkiMessageArray()
	{
		return m_content.MapElements(PkiMessage.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
