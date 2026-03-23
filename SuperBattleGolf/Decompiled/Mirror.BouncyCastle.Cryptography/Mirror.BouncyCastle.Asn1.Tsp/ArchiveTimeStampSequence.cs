namespace Mirror.BouncyCastle.Asn1.Tsp;

public class ArchiveTimeStampSequence : Asn1Encodable
{
	private readonly Asn1Sequence m_archiveTimeStampChains;

	public virtual int Count => m_archiveTimeStampChains.Count;

	public static ArchiveTimeStampSequence GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ArchiveTimeStampSequence result)
		{
			return result;
		}
		return new ArchiveTimeStampSequence(Asn1Sequence.GetInstance(obj));
	}

	public static ArchiveTimeStampSequence GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ArchiveTimeStampSequence(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private ArchiveTimeStampSequence(Asn1Sequence sequence)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(sequence.Count);
		foreach (Asn1Encodable item in sequence)
		{
			asn1EncodableVector.Add(ArchiveTimeStampChain.GetInstance(item));
		}
		m_archiveTimeStampChains = new DerSequence(asn1EncodableVector);
	}

	public ArchiveTimeStampSequence(ArchiveTimeStampChain archiveTimeStampChain)
	{
		m_archiveTimeStampChains = new DerSequence(archiveTimeStampChain);
	}

	public ArchiveTimeStampSequence(ArchiveTimeStampChain[] archiveTimeStampChains)
	{
		m_archiveTimeStampChains = new DerSequence(archiveTimeStampChains);
	}

	public virtual ArchiveTimeStampChain[] GetArchiveTimeStampChains()
	{
		return m_archiveTimeStampChains.MapElements(ArchiveTimeStampChain.GetInstance);
	}

	public virtual ArchiveTimeStampSequence Append(ArchiveTimeStampChain chain)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_archiveTimeStampChains.Count + 1);
		foreach (Asn1Encodable archiveTimeStampChain in m_archiveTimeStampChains)
		{
			asn1EncodableVector.Add(archiveTimeStampChain);
		}
		asn1EncodableVector.Add(chain);
		return new ArchiveTimeStampSequence(new DerSequence(asn1EncodableVector));
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_archiveTimeStampChains;
	}
}
