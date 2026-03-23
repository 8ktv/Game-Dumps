using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Smime;

public class SmimeEncryptionKeyPreferenceAttribute : AttributeX509
{
	public SmimeEncryptionKeyPreferenceAttribute(IssuerAndSerialNumber issAndSer)
		: base(SmimeAttributes.EncrypKeyPref, new DerSet(new DerTaggedObject(isExplicit: false, 0, issAndSer)))
	{
	}

	public SmimeEncryptionKeyPreferenceAttribute(RecipientKeyIdentifier rKeyID)
		: base(SmimeAttributes.EncrypKeyPref, new DerSet(new DerTaggedObject(isExplicit: false, 1, rKeyID)))
	{
	}

	public SmimeEncryptionKeyPreferenceAttribute(Asn1OctetString sKeyID)
		: base(SmimeAttributes.EncrypKeyPref, new DerSet(new DerTaggedObject(isExplicit: false, 2, sKeyID)))
	{
	}
}
