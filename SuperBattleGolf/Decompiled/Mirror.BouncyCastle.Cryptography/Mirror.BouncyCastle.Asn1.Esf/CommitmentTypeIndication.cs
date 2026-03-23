using System;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class CommitmentTypeIndication : Asn1Encodable
{
	private readonly DerObjectIdentifier m_commitmentTypeId;

	private readonly Asn1Sequence m_commitmentTypeQualifier;

	public DerObjectIdentifier CommitmentTypeID => m_commitmentTypeId;

	public Asn1Sequence CommitmentTypeQualifier => m_commitmentTypeQualifier;

	public static CommitmentTypeIndication GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CommitmentTypeIndication result)
		{
			return result;
		}
		return new CommitmentTypeIndication(Asn1Sequence.GetInstance(obj));
	}

	public static CommitmentTypeIndication GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CommitmentTypeIndication(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public CommitmentTypeIndication(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_commitmentTypeId = DerObjectIdentifier.GetInstance(seq[0]);
		if (count > 1)
		{
			m_commitmentTypeQualifier = Asn1Sequence.GetInstance(seq[1]);
		}
	}

	public CommitmentTypeIndication(DerObjectIdentifier commitmentTypeId)
		: this(commitmentTypeId, null)
	{
	}

	public CommitmentTypeIndication(DerObjectIdentifier commitmentTypeId, Asn1Sequence commitmentTypeQualifier)
	{
		m_commitmentTypeId = commitmentTypeId ?? throw new ArgumentNullException("commitmentTypeId");
		m_commitmentTypeQualifier = commitmentTypeQualifier;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_commitmentTypeId);
		asn1EncodableVector.AddOptional(m_commitmentTypeQualifier);
		return new DerSequence(asn1EncodableVector);
	}
}
