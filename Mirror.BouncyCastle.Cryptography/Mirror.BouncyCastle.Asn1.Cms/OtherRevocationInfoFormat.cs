namespace Mirror.BouncyCastle.Asn1.Cms;

public class OtherRevocationInfoFormat : Asn1Encodable
{
	private readonly DerObjectIdentifier otherRevInfoFormat;

	private readonly Asn1Encodable otherRevInfo;

	public virtual DerObjectIdentifier InfoFormat => otherRevInfoFormat;

	public virtual Asn1Encodable Info => otherRevInfo;

	public static OtherRevocationInfoFormat GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherRevocationInfoFormat result)
		{
			return result;
		}
		return new OtherRevocationInfoFormat(Asn1Sequence.GetInstance(obj));
	}

	public static OtherRevocationInfoFormat GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new OtherRevocationInfoFormat(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	public OtherRevocationInfoFormat(DerObjectIdentifier otherRevInfoFormat, Asn1Encodable otherRevInfo)
	{
		this.otherRevInfoFormat = otherRevInfoFormat;
		this.otherRevInfo = otherRevInfo;
	}

	private OtherRevocationInfoFormat(Asn1Sequence seq)
	{
		otherRevInfoFormat = DerObjectIdentifier.GetInstance(seq[0]);
		otherRevInfo = seq[1];
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(otherRevInfoFormat, otherRevInfo);
	}
}
