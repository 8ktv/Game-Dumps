namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PopoDecKeyRespContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static PopoDecKeyRespContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PopoDecKeyRespContent result)
		{
			return result;
		}
		return new PopoDecKeyRespContent(Asn1Sequence.GetInstance(obj));
	}

	public static PopoDecKeyRespContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PopoDecKeyRespContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PopoDecKeyRespContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public virtual DerInteger[] ToIntegerArray()
	{
		return m_content.MapElements(DerInteger.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
