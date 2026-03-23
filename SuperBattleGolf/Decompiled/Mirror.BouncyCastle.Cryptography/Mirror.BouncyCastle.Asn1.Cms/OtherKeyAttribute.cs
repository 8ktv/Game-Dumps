using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class OtherKeyAttribute : Asn1Encodable
{
	private DerObjectIdentifier keyAttrId;

	private Asn1Encodable keyAttr;

	public DerObjectIdentifier KeyAttrId => keyAttrId;

	public Asn1Encodable KeyAttr => keyAttr;

	public static OtherKeyAttribute GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherKeyAttribute result)
		{
			return result;
		}
		return new OtherKeyAttribute(Asn1Sequence.GetInstance(obj));
	}

	public static OtherKeyAttribute GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OtherKeyAttribute(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public OtherKeyAttribute(Asn1Sequence seq)
	{
		keyAttrId = (DerObjectIdentifier)seq[0];
		keyAttr = seq[1];
	}

	public OtherKeyAttribute(DerObjectIdentifier keyAttrId, Asn1Encodable keyAttr)
	{
		this.keyAttrId = keyAttrId;
		this.keyAttr = keyAttr;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(keyAttrId, keyAttr);
	}
}
