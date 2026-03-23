namespace Mirror.BouncyCastle.Asn1.Cms;

public class TimeStampedData : Asn1Encodable
{
	private DerInteger version;

	private DerIA5String dataUri;

	private MetaData metaData;

	private Asn1OctetString content;

	private Evidence temporalEvidence;

	public virtual DerIA5String DataUri => dataUri;

	public MetaData MetaData => metaData;

	public Asn1OctetString Content => content;

	public Evidence TemporalEvidence => temporalEvidence;

	public static TimeStampedData GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TimeStampedData result)
		{
			return result;
		}
		return new TimeStampedData(Asn1Sequence.GetInstance(obj));
	}

	public static TimeStampedData GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new TimeStampedData(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public TimeStampedData(DerIA5String dataUri, MetaData metaData, Asn1OctetString content, Evidence temporalEvidence)
	{
		version = new DerInteger(1);
		this.dataUri = dataUri;
		this.metaData = metaData;
		this.content = content;
		this.temporalEvidence = temporalEvidence;
	}

	private TimeStampedData(Asn1Sequence seq)
	{
		version = DerInteger.GetInstance(seq[0]);
		int num = 1;
		if (seq[num] is DerIA5String derIA5String)
		{
			dataUri = derIA5String;
			num++;
		}
		if (seq[num] is MetaData || seq[num] is Asn1Sequence)
		{
			metaData = MetaData.GetInstance(seq[num++]);
		}
		if (seq[num] is Asn1OctetString asn1OctetString)
		{
			content = asn1OctetString;
			num++;
		}
		temporalEvidence = Evidence.GetInstance(seq[num]);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(version);
		asn1EncodableVector.AddOptional(dataUri, metaData, content);
		asn1EncodableVector.Add(temporalEvidence);
		return new BerSequence(asn1EncodableVector);
	}
}
