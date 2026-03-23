using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class PolicyInformation : Asn1Encodable
{
	private readonly DerObjectIdentifier policyIdentifier;

	private readonly Asn1Sequence policyQualifiers;

	public DerObjectIdentifier PolicyIdentifier => policyIdentifier;

	public Asn1Sequence PolicyQualifiers => policyQualifiers;

	public static PolicyInformation GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PolicyInformation result)
		{
			return result;
		}
		return new PolicyInformation(Asn1Sequence.GetInstance(obj));
	}

	public static PolicyInformation GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new PolicyInformation(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private PolicyInformation(Asn1Sequence seq)
	{
		if (seq.Count < 1 || seq.Count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		policyIdentifier = DerObjectIdentifier.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			policyQualifiers = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public PolicyInformation(DerObjectIdentifier policyIdentifier)
	{
		this.policyIdentifier = policyIdentifier;
	}

	public PolicyInformation(DerObjectIdentifier policyIdentifier, Asn1Sequence policyQualifiers)
	{
		this.policyIdentifier = policyIdentifier;
		this.policyQualifiers = policyQualifiers;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(policyIdentifier);
		asn1EncodableVector.AddOptional(policyQualifiers);
		return new DerSequence(asn1EncodableVector);
	}
}
