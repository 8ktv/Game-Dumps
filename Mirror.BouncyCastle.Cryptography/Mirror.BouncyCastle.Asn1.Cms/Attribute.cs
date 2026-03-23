using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class Attribute : Asn1Encodable
{
	private DerObjectIdentifier attrType;

	private Asn1Set attrValues;

	public DerObjectIdentifier AttrType => attrType;

	public Asn1Set AttrValues => attrValues;

	public static Attribute GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Attribute result)
		{
			return result;
		}
		return new Attribute(Asn1Sequence.GetInstance(obj));
	}

	public static Attribute GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new Attribute(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public Attribute(Asn1Sequence seq)
	{
		attrType = (DerObjectIdentifier)seq[0];
		attrValues = (Asn1Set)seq[1];
	}

	public Attribute(DerObjectIdentifier attrType, Asn1Set attrValues)
	{
		this.attrType = attrType;
		this.attrValues = attrValues;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(attrType, attrValues);
	}
}
