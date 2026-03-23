using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CommitmentTypeQualifier : Asn1Encodable
{
	private readonly DerObjectIdentifier m_commitmentTypeIdentifier;

	private readonly Asn1Encodable m_qualifier;

	public DerObjectIdentifier CommitmentTypeIdentifier => m_commitmentTypeIdentifier;

	public Asn1Encodable QualifierData => m_qualifier;

	[Obsolete("Use 'QualifierData' instead")]
	public Asn1Object Qualifier => m_qualifier?.ToAsn1Object();

	public static CommitmentTypeQualifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CommitmentTypeQualifier result)
		{
			return result;
		}
		return new CommitmentTypeQualifier(Asn1Sequence.GetInstance(obj));
	}

	public static CommitmentTypeQualifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CommitmentTypeQualifier(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public CommitmentTypeQualifier(DerObjectIdentifier commitmentTypeIdentifier)
		: this(commitmentTypeIdentifier, null)
	{
	}

	public CommitmentTypeQualifier(DerObjectIdentifier commitmentTypeIdentifier, Asn1Encodable qualifier)
	{
		m_commitmentTypeIdentifier = commitmentTypeIdentifier ?? throw new ArgumentNullException("commitmentTypeIdentifier");
		m_qualifier = qualifier;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public CommitmentTypeQualifier(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_commitmentTypeIdentifier = DerObjectIdentifier.GetInstance(seq[0]);
		if (count > 1)
		{
			m_qualifier = seq[1];
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_commitmentTypeIdentifier);
		asn1EncodableVector.AddOptional(m_qualifier);
		return new DerSequence(asn1EncodableVector);
	}
}
