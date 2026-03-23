using System;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class ArchiveTimeStamp : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_digestAlgorithm;

	private readonly Attributes m_attributes;

	private readonly Asn1Sequence m_reducedHashTree;

	private readonly Mirror.BouncyCastle.Asn1.Cms.ContentInfo m_timeStamp;

	public virtual Mirror.BouncyCastle.Asn1.Cms.ContentInfo TimeStamp => m_timeStamp;

	public static ArchiveTimeStamp GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ArchiveTimeStamp result)
		{
			return result;
		}
		return new ArchiveTimeStamp(Asn1Sequence.GetInstance(obj));
	}

	public static ArchiveTimeStamp GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ArchiveTimeStamp(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public ArchiveTimeStamp(AlgorithmIdentifier digestAlgorithm, PartialHashtree[] reducedHashTree, Mirror.BouncyCastle.Asn1.Cms.ContentInfo timeStamp)
		: this(digestAlgorithm, null, reducedHashTree, timeStamp)
	{
	}

	public ArchiveTimeStamp(Mirror.BouncyCastle.Asn1.Cms.ContentInfo timeStamp)
		: this(null, null, null, timeStamp)
	{
	}

	public ArchiveTimeStamp(AlgorithmIdentifier digestAlgorithm, Attributes attributes, PartialHashtree[] reducedHashTree, Mirror.BouncyCastle.Asn1.Cms.ContentInfo timeStamp)
	{
		m_digestAlgorithm = digestAlgorithm;
		m_attributes = attributes;
		if (reducedHashTree != null)
		{
			m_reducedHashTree = new DerSequence(reducedHashTree);
		}
		else
		{
			m_reducedHashTree = null;
		}
		m_timeStamp = timeStamp;
	}

	private ArchiveTimeStamp(Asn1Sequence sequence)
	{
		if (sequence.Count < 1 || sequence.Count > 4)
		{
			throw new ArgumentException("wrong sequence size in constructor: " + sequence.Count, "sequence");
		}
		AlgorithmIdentifier digestAlgorithm = null;
		Attributes attributes = null;
		Asn1Sequence reducedHashTree = null;
		for (int i = 0; i < sequence.Count - 1; i++)
		{
			if (sequence[i] is Asn1TaggedObject asn1TaggedObject)
			{
				switch (asn1TaggedObject.TagNo)
				{
				case 0:
					digestAlgorithm = AlgorithmIdentifier.GetInstance(asn1TaggedObject, explicitly: false);
					break;
				case 1:
					attributes = Attributes.GetInstance(asn1TaggedObject, declaredExplicit: false);
					break;
				case 2:
					reducedHashTree = Asn1Sequence.GetInstance(asn1TaggedObject, declaredExplicit: false);
					break;
				default:
					throw new ArgumentException("invalid tag no in constructor: " + asn1TaggedObject.TagNo);
				}
			}
		}
		m_digestAlgorithm = digestAlgorithm;
		m_attributes = attributes;
		m_reducedHashTree = reducedHashTree;
		m_timeStamp = Mirror.BouncyCastle.Asn1.Cms.ContentInfo.GetInstance(sequence[sequence.Count - 1]);
	}

	public virtual AlgorithmIdentifier GetDigestAlgorithmIdentifier()
	{
		return m_digestAlgorithm ?? GetTimeStampInfo().MessageImprint.HashAlgorithm;
	}

	public virtual byte[] GetTimeStampDigestValue()
	{
		return GetTimeStampInfo().MessageImprint.GetHashedMessage();
	}

	private TstInfo GetTimeStampInfo()
	{
		if (!CmsObjectIdentifiers.SignedData.Equals(m_timeStamp.ContentType))
		{
			throw new InvalidOperationException("cannot identify algorithm identifier for digest");
		}
		Mirror.BouncyCastle.Asn1.Cms.ContentInfo encapContentInfo = Mirror.BouncyCastle.Asn1.Cms.SignedData.GetInstance(m_timeStamp.Content).EncapContentInfo;
		if (!PkcsObjectIdentifiers.IdCTTstInfo.Equals(encapContentInfo.ContentType))
		{
			throw new InvalidOperationException("cannot parse time stamp");
		}
		return TstInfo.GetInstance(Asn1OctetString.GetInstance(encapContentInfo.Content).GetOctets());
	}

	public virtual AlgorithmIdentifier DigestAlgorithm()
	{
		return m_digestAlgorithm;
	}

	public virtual PartialHashtree GetHashTreeLeaf()
	{
		if (m_reducedHashTree == null)
		{
			return null;
		}
		return PartialHashtree.GetInstance(m_reducedHashTree[0]);
	}

	public virtual PartialHashtree[] GetReducedHashTree()
	{
		return m_reducedHashTree?.MapElements(PartialHashtree.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_digestAlgorithm);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_attributes);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 2, m_reducedHashTree);
		asn1EncodableVector.Add(m_timeStamp);
		return new DerSequence(asn1EncodableVector);
	}
}
