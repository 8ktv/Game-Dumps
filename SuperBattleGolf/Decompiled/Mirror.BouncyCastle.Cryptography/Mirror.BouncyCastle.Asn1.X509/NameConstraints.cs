using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.X509;

public class NameConstraints : Asn1Encodable
{
	private readonly Asn1Sequence m_permitted;

	private readonly Asn1Sequence m_excluded;

	public Asn1Sequence PermittedSubtrees => m_permitted;

	public Asn1Sequence ExcludedSubtrees => m_excluded;

	public static NameConstraints GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is NameConstraints result)
		{
			return result;
		}
		return new NameConstraints(Asn1Sequence.GetInstance(obj));
	}

	public static NameConstraints GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public NameConstraints(Asn1Sequence seq)
	{
		foreach (Asn1TaggedObject item in seq)
		{
			switch (item.TagNo)
			{
			case 0:
				m_permitted = Asn1Sequence.GetInstance(item, declaredExplicit: false);
				break;
			case 1:
				m_excluded = Asn1Sequence.GetInstance(item, declaredExplicit: false);
				break;
			}
		}
	}

	public NameConstraints(IList<GeneralSubtree> permitted, IList<GeneralSubtree> excluded)
	{
		if (permitted != null)
		{
			m_permitted = CreateSequence(permitted);
		}
		if (excluded != null)
		{
			m_excluded = CreateSequence(excluded);
		}
	}

	private DerSequence CreateSequence(IList<GeneralSubtree> subtrees)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(subtrees.Count);
		foreach (GeneralSubtree subtree in subtrees)
		{
			asn1EncodableVector.Add(subtree);
		}
		return new DerSequence(asn1EncodableVector);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_permitted);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_excluded);
		return new DerSequence(asn1EncodableVector);
	}
}
