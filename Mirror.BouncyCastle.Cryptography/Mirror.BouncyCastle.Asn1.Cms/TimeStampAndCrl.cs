using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class TimeStampAndCrl : Asn1Encodable
{
	private ContentInfo timeStamp;

	private CertificateList crl;

	public virtual ContentInfo TimeStampToken => timeStamp;

	public virtual CertificateList Crl => crl;

	public static TimeStampAndCrl GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TimeStampAndCrl result)
		{
			return result;
		}
		return new TimeStampAndCrl(Asn1Sequence.GetInstance(obj));
	}

	public static TimeStampAndCrl GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new TimeStampAndCrl(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public TimeStampAndCrl(ContentInfo timeStamp)
	{
		this.timeStamp = timeStamp;
	}

	private TimeStampAndCrl(Asn1Sequence seq)
	{
		timeStamp = ContentInfo.GetInstance(seq[0]);
		if (seq.Count == 2)
		{
			crl = CertificateList.GetInstance(seq[1]);
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(timeStamp);
		asn1EncodableVector.AddOptional(crl);
		return new DerSequence(asn1EncodableVector);
	}
}
