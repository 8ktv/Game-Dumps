using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherCertID : Asn1Encodable
{
	private readonly OtherHash m_otherCertHash;

	private readonly IssuerSerial m_issuerSerial;

	public OtherHash OtherCertHash => m_otherCertHash;

	public IssuerSerial IssuerSerial => m_issuerSerial;

	public static OtherCertID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherCertID result)
		{
			return result;
		}
		return new OtherCertID(Asn1Sequence.GetInstance(obj));
	}

	public static OtherCertID GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OtherCertID(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OtherCertID(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_otherCertHash = OtherHash.GetInstance(seq[0]);
		if (count > 1)
		{
			m_issuerSerial = IssuerSerial.GetInstance(seq[1]);
		}
	}

	public OtherCertID(OtherHash otherCertHash)
		: this(otherCertHash, null)
	{
	}

	public OtherCertID(OtherHash otherCertHash, IssuerSerial issuerSerial)
	{
		m_otherCertHash = otherCertHash ?? throw new ArgumentNullException("otherCertHash");
		m_issuerSerial = issuerSerial;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_otherCertHash);
		asn1EncodableVector.AddOptional(m_issuerSerial);
		return new DerSequence(asn1EncodableVector);
	}
}
