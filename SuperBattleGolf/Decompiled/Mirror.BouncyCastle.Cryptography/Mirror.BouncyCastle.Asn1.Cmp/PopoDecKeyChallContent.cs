namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PopoDecKeyChallContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static PopoDecKeyChallContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PopoDecKeyChallContent result)
		{
			return result;
		}
		return new PopoDecKeyChallContent(Asn1Sequence.GetInstance(obj));
	}

	public static PopoDecKeyChallContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PopoDecKeyChallContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PopoDecKeyChallContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public virtual Challenge[] ToChallengeArray()
	{
		return m_content.MapElements(Challenge.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
