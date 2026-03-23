using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class AttributeCertificate : Asn1Encodable
{
	private readonly AttributeCertificateInfo acinfo;

	private readonly AlgorithmIdentifier signatureAlgorithm;

	private readonly DerBitString signatureValue;

	public AttributeCertificateInfo ACInfo => acinfo;

	public AlgorithmIdentifier SignatureAlgorithm => signatureAlgorithm;

	public DerBitString SignatureValue => signatureValue;

	public static AttributeCertificate GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is AttributeCertificate result)
		{
			return result;
		}
		return new AttributeCertificate(Asn1Sequence.GetInstance(obj));
	}

	public static AttributeCertificate GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new AttributeCertificate(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public AttributeCertificate(AttributeCertificateInfo acinfo, AlgorithmIdentifier signatureAlgorithm, DerBitString signatureValue)
	{
		this.acinfo = acinfo;
		this.signatureAlgorithm = signatureAlgorithm;
		this.signatureValue = signatureValue;
	}

	private AttributeCertificate(Asn1Sequence seq)
	{
		if (seq.Count != 3)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		acinfo = AttributeCertificateInfo.GetInstance(seq[0]);
		signatureAlgorithm = AlgorithmIdentifier.GetInstance(seq[1]);
		signatureValue = DerBitString.GetInstance(seq[2]);
	}

	public byte[] GetSignatureOctets()
	{
		return signatureValue.GetOctets();
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(acinfo, signatureAlgorithm, signatureValue);
	}
}
