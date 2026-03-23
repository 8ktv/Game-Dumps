namespace Mirror.BouncyCastle.Asn1.Cms;

public class RecipientEncryptedKey : Asn1Encodable
{
	private readonly KeyAgreeRecipientIdentifier identifier;

	private readonly Asn1OctetString encryptedKey;

	public KeyAgreeRecipientIdentifier Identifier => identifier;

	public Asn1OctetString EncryptedKey => encryptedKey;

	public static RecipientEncryptedKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is RecipientEncryptedKey result)
		{
			return result;
		}
		return new RecipientEncryptedKey(Asn1Sequence.GetInstance(obj));
	}

	public static RecipientEncryptedKey GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new RecipientEncryptedKey(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	private RecipientEncryptedKey(Asn1Sequence seq)
	{
		identifier = KeyAgreeRecipientIdentifier.GetInstance(seq[0]);
		encryptedKey = (Asn1OctetString)seq[1];
	}

	public RecipientEncryptedKey(KeyAgreeRecipientIdentifier id, Asn1OctetString encryptedKey)
	{
		identifier = id;
		this.encryptedKey = encryptedKey;
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(identifier, encryptedKey);
	}
}
