using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class IssuerSerial : Asn1Encodable
{
	private readonly GeneralNames issuer;

	private readonly DerInteger serial;

	private readonly DerBitString issuerUid;

	public GeneralNames Issuer => issuer;

	public DerInteger Serial => serial;

	public DerBitString IssuerUid => issuerUid;

	public static IssuerSerial GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is IssuerSerial result)
		{
			return result;
		}
		return new IssuerSerial(Asn1Sequence.GetInstance(obj));
	}

	public static IssuerSerial GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new IssuerSerial(Asn1Sequence.GetInstance(obj, explicitly));
	}

	private IssuerSerial(Asn1Sequence seq)
	{
		if (seq.Count != 2 && seq.Count != 3)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		issuer = GeneralNames.GetInstance(seq[0]);
		serial = DerInteger.GetInstance(seq[1]);
		if (seq.Count == 3)
		{
			issuerUid = DerBitString.GetInstance(seq[2]);
		}
	}

	public IssuerSerial(GeneralNames issuer, DerInteger serial)
	{
		this.issuer = issuer;
		this.serial = serial;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(issuer, serial);
		asn1EncodableVector.AddOptional(issuerUid);
		return new DerSequence(asn1EncodableVector);
	}
}
