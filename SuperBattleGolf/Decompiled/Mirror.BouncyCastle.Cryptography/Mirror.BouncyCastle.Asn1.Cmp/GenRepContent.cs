namespace Mirror.BouncyCastle.Asn1.Cmp;

public class GenRepContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static GenRepContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is GenRepContent result)
		{
			return result;
		}
		return new GenRepContent(Asn1Sequence.GetInstance(obj));
	}

	public static GenRepContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new GenRepContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private GenRepContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public GenRepContent(InfoTypeAndValue itv)
	{
		m_content = new DerSequence(itv);
	}

	public GenRepContent(params InfoTypeAndValue[] itvs)
	{
		m_content = new DerSequence(itvs);
	}

	public virtual InfoTypeAndValue[] ToInfoTypeAndValueArray()
	{
		return m_content.MapElements(InfoTypeAndValue.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
