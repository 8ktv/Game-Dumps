namespace Mirror.BouncyCastle.Asn1.Cms;

public class MetaData : Asn1Encodable
{
	private DerBoolean hashProtected;

	private DerUtf8String fileName;

	private DerIA5String mediaType;

	private Attributes otherMetaData;

	public virtual bool IsHashProtected => hashProtected.IsTrue;

	public virtual DerUtf8String FileName => fileName;

	public virtual DerIA5String MediaType => mediaType;

	public virtual Attributes OtherMetaData => otherMetaData;

	public static MetaData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is MetaData result)
		{
			return result;
		}
		return new MetaData(Asn1Sequence.GetInstance(obj));
	}

	public static MetaData GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new MetaData(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public MetaData(DerBoolean hashProtected, DerUtf8String fileName, DerIA5String mediaType, Attributes otherMetaData)
	{
		this.hashProtected = hashProtected;
		this.fileName = fileName;
		this.mediaType = mediaType;
		this.otherMetaData = otherMetaData;
	}

	private MetaData(Asn1Sequence seq)
	{
		hashProtected = DerBoolean.GetInstance(seq[0]);
		int num = 1;
		if (num < seq.Count && seq[num] is DerUtf8String derUtf8String)
		{
			fileName = derUtf8String;
			num++;
		}
		if (num < seq.Count && seq[num] is DerIA5String derIA5String)
		{
			mediaType = derIA5String;
			num++;
		}
		if (num < seq.Count)
		{
			otherMetaData = Attributes.GetInstance(seq[num++]);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(hashProtected);
		asn1EncodableVector.AddOptional(fileName, mediaType, otherMetaData);
		return new DerSequence(asn1EncodableVector);
	}
}
