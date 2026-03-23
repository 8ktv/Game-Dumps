using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class SigPolicyQualifierInfo : Asn1Encodable
{
	private readonly DerObjectIdentifier m_sigPolicyQualifierId;

	private readonly Asn1Encodable m_sigQualifier;

	public DerObjectIdentifier SigPolicyQualifierId => m_sigPolicyQualifierId;

	public Asn1Encodable SigQualifierData => m_sigQualifier;

	[Obsolete("Use 'SigQualifierData' instead")]
	public Asn1Object SigQualifier => m_sigQualifier.ToAsn1Object();

	public static SigPolicyQualifierInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SigPolicyQualifierInfo result)
		{
			return result;
		}
		return new SigPolicyQualifierInfo(Asn1Sequence.GetInstance(obj));
	}

	public static SigPolicyQualifierInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new SigPolicyQualifierInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private SigPolicyQualifierInfo(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_sigPolicyQualifierId = DerObjectIdentifier.GetInstance(seq[0]);
		m_sigQualifier = seq[1];
	}

	public SigPolicyQualifierInfo(DerObjectIdentifier sigPolicyQualifierId, Asn1Encodable sigQualifier)
	{
		m_sigPolicyQualifierId = sigPolicyQualifierId ?? throw new ArgumentNullException("sigPolicyQualifierId");
		m_sigQualifier = sigQualifier ?? throw new ArgumentNullException("sigQualifier");
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_sigPolicyQualifierId, m_sigQualifier);
	}
}
