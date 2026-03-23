using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Ess;

public class SigningCertificate : Asn1Encodable
{
	private Asn1Sequence certs;

	private Asn1Sequence policies;

	public static SigningCertificate GetInstance(object o)
	{
		if (o == null || o is SigningCertificate)
		{
			return (SigningCertificate)o;
		}
		if (o is Asn1Sequence)
		{
			return new SigningCertificate((Asn1Sequence)o);
		}
		throw new ArgumentException("unknown object in 'SigningCertificate' factory : " + Platform.GetTypeName(o) + ".");
	}

	public SigningCertificate(Asn1Sequence seq)
	{
		if (seq.Count < 1 || seq.Count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		certs = Asn1Sequence.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			policies = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public SigningCertificate(EssCertID essCertID)
	{
		certs = new DerSequence(essCertID);
	}

	public EssCertID[] GetCerts()
	{
		return certs.MapElements(EssCertID.GetInstance);
	}

	public PolicyInformation[] GetPolicies()
	{
		return policies?.MapElements(PolicyInformation.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(certs);
		asn1EncodableVector.AddOptional(policies);
		return new DerSequence(asn1EncodableVector);
	}
}
