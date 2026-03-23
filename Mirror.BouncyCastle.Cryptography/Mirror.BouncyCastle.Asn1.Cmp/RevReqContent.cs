namespace Mirror.BouncyCastle.Asn1.Cmp;

public class RevReqContent : Asn1Encodable
{
	private readonly Asn1Sequence m_content;

	public static RevReqContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RevReqContent result)
		{
			return result;
		}
		return new RevReqContent(Asn1Sequence.GetInstance(obj));
	}

	public static RevReqContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new RevReqContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private RevReqContent(Asn1Sequence seq)
	{
		m_content = seq;
	}

	public RevReqContent(RevDetails revDetails)
	{
		m_content = new DerSequence(revDetails);
	}

	public RevReqContent(params RevDetails[] revDetailsArray)
	{
		m_content = new DerSequence(revDetailsArray);
	}

	public virtual RevDetails[] ToRevDetailsArray()
	{
		return m_content.MapElements(RevDetails.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_content;
	}
}
