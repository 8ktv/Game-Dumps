using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public sealed class KemRecipientInfo : Asn1Encodable
{
	private static readonly DerInteger V1 = new DerInteger(0);

	private readonly DerInteger m_cmsVersion;

	private readonly RecipientIdentifier m_rid;

	private readonly AlgorithmIdentifier m_kem;

	private readonly Asn1OctetString m_kemct;

	private readonly AlgorithmIdentifier m_kdf;

	private readonly DerInteger m_kekLength;

	private readonly Asn1OctetString m_ukm;

	private readonly AlgorithmIdentifier m_wrap;

	private readonly Asn1OctetString m_encryptedKey;

	public RecipientIdentifier RecipientIdentifier => m_rid;

	public AlgorithmIdentifier Kem => m_kem;

	public Asn1OctetString Kemct => m_kemct;

	public AlgorithmIdentifier Kdf => m_kdf;

	public AlgorithmIdentifier Wrap => m_wrap;

	public Asn1OctetString Ukm => m_ukm;

	public Asn1OctetString EncryptedKey => m_encryptedKey;

	public static KemRecipientInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KemRecipientInfo result)
		{
			return result;
		}
		return new KemRecipientInfo(Asn1Sequence.GetInstance(obj));
	}

	public static KemRecipientInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KemRecipientInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public KemRecipientInfo(RecipientIdentifier rid, AlgorithmIdentifier kem, Asn1OctetString kemct, AlgorithmIdentifier kdf, DerInteger kekLength, Asn1OctetString ukm, AlgorithmIdentifier wrap, Asn1OctetString encryptedKey)
	{
		m_cmsVersion = V1;
		m_rid = rid ?? throw new ArgumentNullException("rid");
		m_kem = kem ?? throw new ArgumentNullException("kem");
		m_kemct = kemct ?? throw new ArgumentNullException("kemct");
		m_kdf = kdf ?? throw new ArgumentNullException("kdf");
		m_kekLength = kekLength ?? throw new ArgumentNullException("kekLength");
		m_ukm = ukm;
		m_wrap = wrap ?? throw new ArgumentNullException("wrap");
		m_encryptedKey = encryptedKey ?? throw new ArgumentNullException("encryptedKey");
	}

	private KemRecipientInfo(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 8 || count > 9)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		int sequencePosition = 0;
		m_cmsVersion = DerInteger.GetInstance(seq[sequencePosition++]);
		if (!m_cmsVersion.HasValue(0))
		{
			throw new ArgumentException("Unsupported version (hex): " + m_cmsVersion.Value.ToString(16));
		}
		m_rid = RecipientIdentifier.GetInstance(seq[sequencePosition++]);
		m_kem = AlgorithmIdentifier.GetInstance(seq[sequencePosition++]);
		m_kemct = Asn1OctetString.GetInstance(seq[sequencePosition++]);
		m_kdf = AlgorithmIdentifier.GetInstance(seq[sequencePosition++]);
		m_kekLength = DerInteger.GetInstance(seq[sequencePosition++]);
		m_ukm = Asn1Utilities.ReadOptionalContextTagged(seq, ref sequencePosition, 0, state: true, Asn1OctetString.GetInstance);
		m_wrap = AlgorithmIdentifier.GetInstance(seq[sequencePosition++]);
		m_encryptedKey = Asn1OctetString.GetInstance(seq[sequencePosition++]);
		if (sequencePosition != count)
		{
			throw new ArgumentException("Unexpected elements in sequence", "seq");
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(9);
		asn1EncodableVector.Add(m_cmsVersion, m_rid, m_kem, m_kemct, m_kdf, m_kekLength);
		asn1EncodableVector.AddOptionalTagged(isExplicit: true, 0, m_ukm);
		asn1EncodableVector.Add(m_wrap, m_encryptedKey);
		return new DerSequence(asn1EncodableVector);
	}
}
