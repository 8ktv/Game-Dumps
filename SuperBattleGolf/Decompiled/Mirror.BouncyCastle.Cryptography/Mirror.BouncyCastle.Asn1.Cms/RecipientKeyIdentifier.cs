using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class RecipientKeyIdentifier : Asn1Encodable
{
	private Asn1OctetString subjectKeyIdentifier;

	private Asn1GeneralizedTime date;

	private OtherKeyAttribute other;

	public Asn1OctetString SubjectKeyIdentifier => subjectKeyIdentifier;

	public Asn1GeneralizedTime Date => date;

	public OtherKeyAttribute OtherKeyAttribute => other;

	public static RecipientKeyIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RecipientKeyIdentifier result)
		{
			return result;
		}
		return new RecipientKeyIdentifier(Asn1Sequence.GetInstance(obj));
	}

	public static RecipientKeyIdentifier GetInstance(Asn1TaggedObject ato, bool explicitly)
	{
		return new RecipientKeyIdentifier(Asn1Sequence.GetInstance(ato, explicitly));
	}

	public RecipientKeyIdentifier(Asn1OctetString subjectKeyIdentifier, Asn1GeneralizedTime date, OtherKeyAttribute other)
	{
		this.subjectKeyIdentifier = subjectKeyIdentifier;
		this.date = date;
		this.other = other;
	}

	public RecipientKeyIdentifier(byte[] subjectKeyIdentifier)
		: this(subjectKeyIdentifier, null, null)
	{
	}

	public RecipientKeyIdentifier(byte[] subjectKeyIdentifier, Asn1GeneralizedTime date, OtherKeyAttribute other)
	{
		this.subjectKeyIdentifier = new DerOctetString(subjectKeyIdentifier);
		this.date = date;
		this.other = other;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public RecipientKeyIdentifier(Asn1Sequence seq)
	{
		subjectKeyIdentifier = Asn1OctetString.GetInstance(seq[0]);
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
			throw new ArgumentException("Invalid RecipientKeyIdentifier");
		case 1:
			break;
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(subjectKeyIdentifier);
		asn1EncodableVector.AddOptional(date, other);
		return new DerSequence(asn1EncodableVector);
	}
}
