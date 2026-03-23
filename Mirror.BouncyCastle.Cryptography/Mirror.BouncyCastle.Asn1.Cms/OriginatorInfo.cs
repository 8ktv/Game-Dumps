using System;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class OriginatorInfo : Asn1Encodable
{
	private Asn1Set certs;

	private Asn1Set crls;

	public Asn1Set Certificates => certs;

	public Asn1Set Crls => crls;

	public static OriginatorInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OriginatorInfo result)
		{
			return result;
		}
		return new OriginatorInfo(Asn1Sequence.GetInstance(obj));
	}

	public static OriginatorInfo GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new OriginatorInfo(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public OriginatorInfo(Asn1Set certs, Asn1Set crls)
	{
		this.certs = certs;
		this.crls = crls;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public OriginatorInfo(Asn1Sequence seq)
	{
		switch (seq.Count)
		{
		case 1:
		{
			Asn1TaggedObject asn1TaggedObject = (Asn1TaggedObject)seq[0];
			switch (asn1TaggedObject.TagNo)
			{
			case 0:
				certs = Asn1Set.GetInstance(asn1TaggedObject, declaredExplicit: false);
				break;
			case 1:
				crls = Asn1Set.GetInstance(asn1TaggedObject, declaredExplicit: false);
				break;
			default:
				throw new ArgumentException("Bad tag in OriginatorInfo: " + asn1TaggedObject.TagNo);
			}
			break;
		}
		case 2:
			certs = Asn1Set.GetInstance((Asn1TaggedObject)seq[0], declaredExplicit: false);
			crls = Asn1Set.GetInstance((Asn1TaggedObject)seq[1], declaredExplicit: false);
			break;
		default:
			throw new ArgumentException("OriginatorInfo too big");
		case 0:
			break;
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, certs);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, crls);
		return new DerSequence(asn1EncodableVector);
	}
}
