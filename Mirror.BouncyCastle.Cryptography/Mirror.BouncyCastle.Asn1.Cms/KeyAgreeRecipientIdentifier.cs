using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class KeyAgreeRecipientIdentifier : Asn1Encodable, IAsn1Choice
{
	private readonly IssuerAndSerialNumber issuerSerial;

	private readonly RecipientKeyIdentifier rKeyID;

	public IssuerAndSerialNumber IssuerAndSerialNumber => issuerSerial;

	public RecipientKeyIdentifier RKeyID => rKeyID;

	public static KeyAgreeRecipientIdentifier GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KeyAgreeRecipientIdentifier result)
		{
			return result;
		}
		if (obj is IssuerAndSerialNumber issuerAndSerialNumber)
		{
			return new KeyAgreeRecipientIdentifier(issuerAndSerialNumber);
		}
		if (obj is Asn1Sequence obj2)
		{
			return new KeyAgreeRecipientIdentifier(IssuerAndSerialNumber.GetInstance(obj2));
		}
		if (obj is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasContextTag(0))
		{
			return new KeyAgreeRecipientIdentifier(RecipientKeyIdentifier.GetInstance(asn1TaggedObject, explicitly: false));
		}
		throw new ArgumentException("Invalid KeyAgreeRecipientIdentifier: " + Platform.GetTypeName(obj), "obj");
	}

	public static KeyAgreeRecipientIdentifier GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, isExplicit, GetInstance);
	}

	public KeyAgreeRecipientIdentifier(IssuerAndSerialNumber issuerSerial)
	{
		this.issuerSerial = issuerSerial;
	}

	public KeyAgreeRecipientIdentifier(RecipientKeyIdentifier rKeyID)
	{
		this.rKeyID = rKeyID;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (issuerSerial != null)
		{
			return issuerSerial.ToAsn1Object();
		}
		return new DerTaggedObject(isExplicit: false, 0, rKeyID);
	}
}
