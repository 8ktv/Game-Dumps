using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class RecipientIdentifier : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1Encodable m_id;

	public bool IsTagged => m_id is Asn1TaggedObject;

	public Asn1Encodable ID
	{
		get
		{
			if (m_id is Asn1TaggedObject taggedObject)
			{
				return Asn1OctetString.GetInstance(taggedObject, declaredExplicit: false);
			}
			return IssuerAndSerialNumber.GetInstance(m_id);
		}
	}

	public static RecipientIdentifier GetInstance(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (o is RecipientIdentifier result)
		{
			return result;
		}
		if (o is IssuerAndSerialNumber id)
		{
			return new RecipientIdentifier(id);
		}
		if (o is Asn1OctetString id2)
		{
			return new RecipientIdentifier(id2);
		}
		if (o is Asn1Object id3)
		{
			return new RecipientIdentifier(id3);
		}
		throw new ArgumentException("Illegal object in RecipientIdentifier: " + Platform.GetTypeName(o), "o");
	}

	public static RecipientIdentifier GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public RecipientIdentifier(IssuerAndSerialNumber id)
	{
		m_id = id;
	}

	public RecipientIdentifier(Asn1OctetString id)
	{
		m_id = new DerTaggedObject(isExplicit: false, 0, id);
	}

	public RecipientIdentifier(Asn1Object id)
	{
		m_id = id;
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_id.ToAsn1Object();
	}
}
