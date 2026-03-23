using System;
using Mirror.BouncyCastle.Asn1.Tsp;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class Evidence : Asn1Encodable, IAsn1Choice
{
	private readonly TimeStampTokenEvidence m_tstEvidence;

	private readonly EvidenceRecord m_ersEvidence;

	private readonly Asn1Sequence m_otherEvidence;

	public virtual TimeStampTokenEvidence TstEvidence => m_tstEvidence;

	public virtual EvidenceRecord ErsEvidence => m_ersEvidence;

	public static Evidence GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Evidence result)
		{
			return result;
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new Evidence(Asn1Utilities.CheckContextTagClass(taggedObject));
		}
		throw new ArgumentException("Unknown object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Evidence GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, isExplicit, GetInstance);
	}

	public Evidence(TimeStampTokenEvidence tstEvidence)
	{
		m_tstEvidence = tstEvidence;
	}

	public Evidence(EvidenceRecord ersEvidence)
	{
		m_ersEvidence = ersEvidence;
	}

	private Evidence(Asn1TaggedObject tagged)
	{
		if (tagged.TagNo == 0)
		{
			m_tstEvidence = TimeStampTokenEvidence.GetInstance(tagged, isExplicit: false);
			return;
		}
		if (tagged.TagNo == 1)
		{
			m_ersEvidence = EvidenceRecord.GetInstance(tagged, declaredExplicit: false);
			return;
		}
		if (tagged.TagNo == 2)
		{
			m_otherEvidence = Asn1Sequence.GetInstance(tagged, declaredExplicit: false);
			return;
		}
		throw new ArgumentException("unknown tag in Evidence", "tagged");
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_tstEvidence != null)
		{
			return new DerTaggedObject(isExplicit: false, 0, m_tstEvidence);
		}
		if (m_ersEvidence != null)
		{
			return new DerTaggedObject(isExplicit: false, 1, m_ersEvidence);
		}
		return new DerTaggedObject(isExplicit: false, 2, m_otherEvidence);
	}
}
