using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Crmf;

public class PkiArchiveOptions : Asn1Encodable, IAsn1Choice
{
	public const int encryptedPrivKey = 0;

	public const int keyGenParameters = 1;

	public const int archiveRemGenPrivKey = 2;

	private readonly Asn1Encodable value;

	public virtual int Type
	{
		get
		{
			if (value is EncryptedKey)
			{
				return 0;
			}
			if (value is Asn1OctetString)
			{
				return 1;
			}
			return 2;
		}
	}

	public virtual Asn1Encodable Value => value;

	public static PkiArchiveOptions GetInstance(object obj)
	{
		if (obj is PkiArchiveOptions result)
		{
			return result;
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new PkiArchiveOptions(Asn1Utilities.CheckContextTagClass(taggedObject));
		}
		throw new ArgumentException("Invalid object: " + Platform.GetTypeName(obj), "obj");
	}

	private PkiArchiveOptions(Asn1TaggedObject tagged)
	{
		switch (tagged.TagNo)
		{
		case 0:
			value = EncryptedKey.GetInstance(tagged.GetExplicitBaseObject());
			break;
		case 1:
			value = Asn1OctetString.GetInstance(tagged, declaredExplicit: false);
			break;
		case 2:
			value = DerBoolean.GetInstance(tagged, declaredExplicit: false);
			break;
		default:
			throw new ArgumentException("unknown tag number: " + tagged.TagNo, "tagged");
		}
	}

	public PkiArchiveOptions(EncryptedKey encKey)
	{
		value = encKey;
	}

	public PkiArchiveOptions(Asn1OctetString keyGenParameters)
	{
		value = keyGenParameters;
	}

	public PkiArchiveOptions(bool archiveRemGenPrivKey)
	{
		value = DerBoolean.GetInstance(archiveRemGenPrivKey);
	}

	public override Asn1Object ToAsn1Object()
	{
		if (value is EncryptedKey)
		{
			return new DerTaggedObject(isExplicit: true, 0, value);
		}
		if (value is Asn1OctetString)
		{
			return new DerTaggedObject(isExplicit: false, 1, value);
		}
		return new DerTaggedObject(isExplicit: false, 2, value);
	}
}
