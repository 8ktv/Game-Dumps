using System;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class OriginatorIdentifierOrKey : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1Encodable m_id;

	public Asn1Encodable ID => m_id;

	public IssuerAndSerialNumber IssuerAndSerialNumber => m_id as IssuerAndSerialNumber;

	public SubjectKeyIdentifier SubjectKeyIdentifier
	{
		get
		{
			if (m_id is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasContextTag(0))
			{
				return SubjectKeyIdentifier.GetInstance(asn1TaggedObject, explicitly: false);
			}
			return null;
		}
	}

	public OriginatorPublicKey OriginatorPublicKey
	{
		get
		{
			if (m_id is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasContextTag(1))
			{
				return OriginatorPublicKey.GetInstance(asn1TaggedObject, explicitly: false);
			}
			return null;
		}
	}

	public static OriginatorIdentifierOrKey GetInstance(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (o is OriginatorIdentifierOrKey result)
		{
			return result;
		}
		if (o is IssuerAndSerialNumber id)
		{
			return new OriginatorIdentifierOrKey(id);
		}
		if (o is Asn1Sequence obj)
		{
			return new OriginatorIdentifierOrKey(IssuerAndSerialNumber.GetInstance(obj));
		}
		if (o is Asn1TaggedObject asn1TaggedObject)
		{
			if (asn1TaggedObject.HasContextTag(0))
			{
				return new OriginatorIdentifierOrKey(SubjectKeyIdentifier.GetInstance(asn1TaggedObject, explicitly: false));
			}
			if (asn1TaggedObject.HasContextTag(1))
			{
				return new OriginatorIdentifierOrKey(OriginatorPublicKey.GetInstance(asn1TaggedObject, explicitly: false));
			}
		}
		throw new ArgumentException("Invalid OriginatorIdentifierOrKey: " + Platform.GetTypeName(o), "o");
	}

	public static OriginatorIdentifierOrKey GetInstance(Asn1TaggedObject o, bool explicitly)
	{
		return Asn1Utilities.GetInstanceFromChoice(o, explicitly, GetInstance);
	}

	public OriginatorIdentifierOrKey(IssuerAndSerialNumber id)
	{
		m_id = id;
	}

	public OriginatorIdentifierOrKey(SubjectKeyIdentifier id)
	{
		m_id = new DerTaggedObject(isExplicit: false, 0, id);
	}

	public OriginatorIdentifierOrKey(OriginatorPublicKey id)
	{
		m_id = new DerTaggedObject(isExplicit: false, 1, id);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_id.ToAsn1Object();
	}
}
