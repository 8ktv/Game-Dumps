using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class SignaturePolicyId : Asn1Encodable
{
	private readonly DerObjectIdentifier m_sigPolicyIdentifier;

	private readonly OtherHashAlgAndValue m_sigPolicyHash;

	private readonly Asn1Sequence m_sigPolicyQualifiers;

	public DerObjectIdentifier SigPolicyIdentifier => m_sigPolicyIdentifier;

	public OtherHashAlgAndValue SigPolicyHash => m_sigPolicyHash;

	public static SignaturePolicyId GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SignaturePolicyId result)
		{
			return result;
		}
		return new SignaturePolicyId(Asn1Sequence.GetInstance(obj));
	}

	public static SignaturePolicyId GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new SignaturePolicyId(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private SignaturePolicyId(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 2 || count > 3)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_sigPolicyIdentifier = DerObjectIdentifier.GetInstance(seq[0]);
		m_sigPolicyHash = OtherHashAlgAndValue.GetInstance(seq[1]);
		if (count > 2)
		{
			m_sigPolicyQualifiers = Asn1Sequence.GetInstance(seq[2]);
		}
	}

	public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash)
		: this(sigPolicyIdentifier, sigPolicyHash, (SigPolicyQualifierInfo[])null)
	{
	}

	public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash, params SigPolicyQualifierInfo[] sigPolicyQualifiers)
	{
		m_sigPolicyIdentifier = sigPolicyIdentifier ?? throw new ArgumentNullException("sigPolicyIdentifier");
		m_sigPolicyHash = sigPolicyHash ?? throw new ArgumentNullException("sigPolicyHash");
		if (sigPolicyQualifiers != null)
		{
			m_sigPolicyQualifiers = DerSequence.FromElements(sigPolicyQualifiers);
		}
	}

	public SignaturePolicyId(DerObjectIdentifier sigPolicyIdentifier, OtherHashAlgAndValue sigPolicyHash, IEnumerable<SigPolicyQualifierInfo> sigPolicyQualifiers)
	{
		m_sigPolicyIdentifier = sigPolicyIdentifier ?? throw new ArgumentNullException("sigPolicyIdentifier");
		m_sigPolicyHash = sigPolicyHash ?? throw new ArgumentNullException("sigPolicyHash");
		if (sigPolicyQualifiers != null)
		{
			m_sigPolicyQualifiers = DerSequence.FromVector(Asn1EncodableVector.FromEnumerable(sigPolicyQualifiers));
		}
	}

	public SigPolicyQualifierInfo[] GetSigPolicyQualifiers()
	{
		return m_sigPolicyQualifiers?.MapElements(SigPolicyQualifierInfo.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_sigPolicyIdentifier, m_sigPolicyHash);
		asn1EncodableVector.AddOptional(m_sigPolicyQualifiers);
		return new DerSequence(asn1EncodableVector);
	}
}
