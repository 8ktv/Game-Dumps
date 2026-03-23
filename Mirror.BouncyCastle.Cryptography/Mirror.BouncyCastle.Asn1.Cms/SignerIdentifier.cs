using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class SignerIdentifier : Asn1Encodable, IAsn1Choice
{
	private Asn1Encodable id;

	public bool IsTagged => id is Asn1TaggedObject;

	public Asn1Encodable ID
	{
		get
		{
			if (id is Asn1TaggedObject taggedObject)
			{
				return Asn1OctetString.GetInstance(taggedObject, declaredExplicit: false);
			}
			return id;
		}
	}

	public static SignerIdentifier GetInstance(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (o is SignerIdentifier result)
		{
			return result;
		}
		if (o is IssuerAndSerialNumber issuerAndSerialNumber)
		{
			return new SignerIdentifier(issuerAndSerialNumber);
		}
		if (o is Asn1OctetString asn1OctetString)
		{
			return new SignerIdentifier(asn1OctetString);
		}
		if (o is Asn1Object asn1Object)
		{
			return new SignerIdentifier(asn1Object);
		}
		throw new ArgumentException("Illegal object in SignerIdentifier: " + Platform.GetTypeName(o), "o");
	}

	public static SignerIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public SignerIdentifier(IssuerAndSerialNumber id)
	{
		this.id = id;
	}

	public SignerIdentifier(Asn1OctetString id)
	{
		this.id = new DerTaggedObject(isExplicit: false, 0, id);
	}

	public SignerIdentifier(Asn1Object id)
	{
		this.id = id;
	}

	public override Asn1Object ToAsn1Object()
	{
		return id.ToAsn1Object();
	}
}
