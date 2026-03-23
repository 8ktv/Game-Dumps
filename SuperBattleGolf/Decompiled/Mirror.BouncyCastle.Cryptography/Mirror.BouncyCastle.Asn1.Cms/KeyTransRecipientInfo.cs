using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class KeyTransRecipientInfo : Asn1Encodable
{
	private DerInteger version;

	private RecipientIdentifier rid;

	private AlgorithmIdentifier keyEncryptionAlgorithm;

	private Asn1OctetString encryptedKey;

	public DerInteger Version => version;

	public RecipientIdentifier RecipientIdentifier => rid;

	public AlgorithmIdentifier KeyEncryptionAlgorithm => keyEncryptionAlgorithm;

	public Asn1OctetString EncryptedKey => encryptedKey;

	public static KeyTransRecipientInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KeyTransRecipientInfo result)
		{
			return result;
		}
		return new KeyTransRecipientInfo(Asn1Sequence.GetInstance(obj));
	}

	public static KeyTransRecipientInfo GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KeyTransRecipientInfo(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public KeyTransRecipientInfo(RecipientIdentifier rid, AlgorithmIdentifier keyEncryptionAlgorithm, Asn1OctetString encryptedKey)
	{
		if (rid.ToAsn1Object() is Asn1TaggedObject)
		{
			version = new DerInteger(2);
		}
		else
		{
			version = new DerInteger(0);
		}
		this.rid = rid;
		this.keyEncryptionAlgorithm = keyEncryptionAlgorithm;
		this.encryptedKey = encryptedKey;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public KeyTransRecipientInfo(Asn1Sequence seq)
	{
		version = (DerInteger)seq[0];
		rid = RecipientIdentifier.GetInstance(seq[1]);
		keyEncryptionAlgorithm = AlgorithmIdentifier.GetInstance(seq[2]);
		encryptedKey = (Asn1OctetString)seq[3];
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(version, rid, keyEncryptionAlgorithm, encryptedKey);
	}
}
