namespace Mirror.BouncyCastle.Asn1.Cms;

public class ScvpReqRes : Asn1Encodable
{
	private readonly ContentInfo request;

	private readonly ContentInfo response;

	public virtual ContentInfo Request => request;

	public virtual ContentInfo Response => response;

	public static ScvpReqRes GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ScvpReqRes result)
		{
			return result;
		}
		return new ScvpReqRes(Asn1Sequence.GetInstance(obj));
	}

	public static ScvpReqRes GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ScvpReqRes(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private ScvpReqRes(Asn1Sequence seq)
	{
		if (seq[0] is Asn1TaggedObject obj)
		{
			request = ContentInfo.GetInstance(obj, isExplicit: true);
			response = ContentInfo.GetInstance(seq[1]);
		}
		else
		{
			request = null;
			response = ContentInfo.GetInstance(seq[0]);
		}
	}

	public ScvpReqRes(ContentInfo response)
		: this(null, response)
	{
	}

	public ScvpReqRes(ContentInfo request, ContentInfo response)
	{
		this.request = request;
		this.response = response;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, request);
		asn1EncodableVector.Add(response);
		return new DerSequence(asn1EncodableVector);
	}
}
