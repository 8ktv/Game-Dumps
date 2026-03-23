using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class KekIdentifier : Asn1Encodable
{
	private Asn1OctetString keyIdentifier;

	private Asn1GeneralizedTime date;

	private OtherKeyAttribute other;

	public Asn1OctetString KeyIdentifier => keyIdentifier;

	public Asn1GeneralizedTime Date => date;

	public OtherKeyAttribute Other => other;

	public static KekIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KekIdentifier result)
		{
			return result;
		}
		return new KekIdentifier(Asn1Sequence.GetInstance(obj));
	}

	public static KekIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new KekIdentifier(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public KekIdentifier(byte[] keyIdentifier, Asn1GeneralizedTime date, OtherKeyAttribute other)
	{
		this.keyIdentifier = new DerOctetString(keyIdentifier);
		this.date = date;
		this.other = other;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public KekIdentifier(Asn1Sequence seq)
	{
		keyIdentifier = (Asn1OctetString)seq[0];
		switch (seq.Count)
		{
		case 2:
			if (seq[1] is Asn1GeneralizedTime asn1GeneralizedTime)
			{
				date = asn1GeneralizedTime;
			}
			else
			{
				other = OtherKeyAttribute.GetInstance(seq[2]);
			}
			break;
		case 3:
			date = (Asn1GeneralizedTime)seq[1];
			other = OtherKeyAttribute.GetInstance(seq[2]);
			break;
		default:
			throw new ArgumentException("Invalid KekIdentifier");
		case 1:
			break;
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(keyIdentifier);
		asn1EncodableVector.AddOptional(date, other);
		return new DerSequence(asn1EncodableVector);
	}
}
