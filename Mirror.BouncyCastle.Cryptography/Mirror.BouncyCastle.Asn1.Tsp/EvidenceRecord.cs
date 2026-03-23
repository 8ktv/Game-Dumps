using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class EvidenceRecord : Asn1Encodable
{
	private static readonly DerObjectIdentifier Oid = new DerObjectIdentifier("1.3.6.1.5.5.11.0.2.1");

	private readonly DerInteger m_version;

	private readonly Asn1Sequence m_digestAlgorithms;

	private readonly CryptoInfos m_cryptoInfos;

	private readonly EncryptionInfo m_encryptionInfo;

	private readonly ArchiveTimeStampSequence m_archiveTimeStampSequence;

	public virtual ArchiveTimeStampSequence ArchiveTimeStampSequence => m_archiveTimeStampSequence;

	public static EvidenceRecord GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is EvidenceRecord result)
		{
			return result;
		}
		return new EvidenceRecord(Asn1Sequence.GetInstance(obj));
	}

	public static EvidenceRecord GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new EvidenceRecord(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private EvidenceRecord(EvidenceRecord evidenceRecord, ArchiveTimeStampSequence replacementSequence, ArchiveTimeStamp newChainTimeStamp)
	{
		m_version = evidenceRecord.m_version;
		if (newChainTimeStamp != null)
		{
			AlgorithmIdentifier digestAlgorithmIdentifier = newChainTimeStamp.GetDigestAlgorithmIdentifier();
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			bool flag = false;
			foreach (Asn1Encodable digestAlgorithm in evidenceRecord.m_digestAlgorithms)
			{
				AlgorithmIdentifier instance = AlgorithmIdentifier.GetInstance(digestAlgorithm);
				asn1EncodableVector.Add(instance);
				if (instance.Equals(digestAlgorithmIdentifier))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				asn1EncodableVector.Add(digestAlgorithmIdentifier);
				m_digestAlgorithms = new DerSequence(asn1EncodableVector);
			}
			else
			{
				m_digestAlgorithms = evidenceRecord.m_digestAlgorithms;
			}
		}
		else
		{
			m_digestAlgorithms = evidenceRecord.m_digestAlgorithms;
		}
		m_cryptoInfos = evidenceRecord.m_cryptoInfos;
		m_encryptionInfo = evidenceRecord.m_encryptionInfo;
		m_archiveTimeStampSequence = replacementSequence;
	}

	public EvidenceRecord(CryptoInfos cryptoInfos, EncryptionInfo encryptionInfo, ArchiveTimeStamp archiveTimeStamp)
	{
		m_version = new DerInteger(1);
		m_digestAlgorithms = new DerSequence(archiveTimeStamp.GetDigestAlgorithmIdentifier());
		m_cryptoInfos = cryptoInfos;
		m_encryptionInfo = encryptionInfo;
		m_archiveTimeStampSequence = new ArchiveTimeStampSequence(new ArchiveTimeStampChain(archiveTimeStamp));
	}

	public EvidenceRecord(AlgorithmIdentifier[] digestAlgorithms, CryptoInfos cryptoInfos, EncryptionInfo encryptionInfo, ArchiveTimeStampSequence archiveTimeStampSequence)
	{
		m_version = new DerInteger(1);
		m_digestAlgorithms = new DerSequence(digestAlgorithms);
		m_cryptoInfos = cryptoInfos;
		m_encryptionInfo = encryptionInfo;
		m_archiveTimeStampSequence = archiveTimeStampSequence;
	}

	private EvidenceRecord(Asn1Sequence sequence)
	{
		if (sequence.Count < 3 && sequence.Count > 5)
		{
			throw new ArgumentException("wrong sequence size in constructor: " + sequence.Count, "sequence");
		}
		DerInteger instance = DerInteger.GetInstance(sequence[0]);
		if (!instance.HasValue(1))
		{
			throw new ArgumentException("incompatible version");
		}
		m_version = instance;
		m_digestAlgorithms = Asn1Sequence.GetInstance(sequence[1]);
		for (int i = 2; i != sequence.Count - 1; i++)
		{
			Asn1Encodable asn1Encodable = sequence[i];
			if (asn1Encodable is Asn1TaggedObject { TagNo: var tagNo } asn1TaggedObject)
			{
				switch (tagNo)
				{
				case 0:
					m_cryptoInfos = CryptoInfos.GetInstance(asn1TaggedObject, declaredExplicit: false);
					break;
				case 1:
					m_encryptionInfo = EncryptionInfo.GetInstance(asn1TaggedObject, declaredExplicit: false);
					break;
				default:
					throw new ArgumentException("unknown tag in GetInstance: " + asn1TaggedObject.TagNo);
				}
				continue;
			}
			throw new ArgumentException("unknown object in GetInstance: " + Platform.GetTypeName(asn1Encodable));
		}
		m_archiveTimeStampSequence = ArchiveTimeStampSequence.GetInstance(sequence[sequence.Count - 1]);
	}

	public virtual AlgorithmIdentifier[] GetDigestAlgorithms()
	{
		return m_digestAlgorithms.MapElements(AlgorithmIdentifier.GetInstance);
	}

	public virtual EvidenceRecord AddArchiveTimeStamp(ArchiveTimeStamp ats, bool newChain)
	{
		if (newChain)
		{
			ArchiveTimeStampChain chain = new ArchiveTimeStampChain(ats);
			return new EvidenceRecord(this, m_archiveTimeStampSequence.Append(chain), ats);
		}
		ArchiveTimeStampChain[] archiveTimeStampChains = m_archiveTimeStampSequence.GetArchiveTimeStampChains();
		if (!archiveTimeStampChains[^1].GetArchiveTimestamps()[0].GetDigestAlgorithmIdentifier().Equals(ats.GetDigestAlgorithmIdentifier()))
		{
			throw new ArgumentException("mismatch of digest algorithm in AddArchiveTimeStamp");
		}
		archiveTimeStampChains[^1] = archiveTimeStampChains[^1].Append(ats);
		return new EvidenceRecord(this, new ArchiveTimeStampSequence(archiveTimeStampChains), null);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
		asn1EncodableVector.Add(m_version);
		asn1EncodableVector.Add(m_digestAlgorithms);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 0, m_cryptoInfos);
		asn1EncodableVector.AddOptionalTagged(isExplicit: false, 1, m_encryptionInfo);
		asn1EncodableVector.Add(m_archiveTimeStampSequence);
		return new DerSequence(asn1EncodableVector);
	}
}
