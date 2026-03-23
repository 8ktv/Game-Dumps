namespace Mirror.BouncyCastle.Asn1.Tsp;

public class ArchiveTimeStampChain : Asn1Encodable
{
	private readonly Asn1Sequence m_archiveTimeStamps;

	public static ArchiveTimeStampChain GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ArchiveTimeStampChain result)
		{
			return result;
		}
		return new ArchiveTimeStampChain(Asn1Sequence.GetInstance(obj));
	}

	public static ArchiveTimeStampChain GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ArchiveTimeStampChain(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public ArchiveTimeStampChain(ArchiveTimeStamp archiveTimeStamp)
	{
		m_archiveTimeStamps = new DerSequence(archiveTimeStamp);
	}

	public ArchiveTimeStampChain(ArchiveTimeStamp[] archiveTimeStamps)
	{
		m_archiveTimeStamps = new DerSequence(archiveTimeStamps);
	}

	private ArchiveTimeStampChain(Asn1Sequence sequence)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(sequence.Count);
		foreach (Asn1Encodable item in sequence)
		{
			asn1EncodableVector.Add(ArchiveTimeStamp.GetInstance(item));
		}
		m_archiveTimeStamps = new DerSequence(asn1EncodableVector);
	}

	public virtual ArchiveTimeStamp[] GetArchiveTimestamps()
	{
		return m_archiveTimeStamps.MapElements(ArchiveTimeStamp.GetInstance);
	}

	public virtual ArchiveTimeStampChain Append(ArchiveTimeStamp archiveTimeStamp)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_archiveTimeStamps.Count + 1);
		foreach (Asn1Encodable archiveTimeStamp2 in m_archiveTimeStamps)
		{
			asn1EncodableVector.Add(archiveTimeStamp2);
		}
		asn1EncodableVector.Add(archiveTimeStamp);
		return new ArchiveTimeStampChain(new DerSequence(asn1EncodableVector));
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_archiveTimeStamps;
	}
}
