using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Ocsp;

public class ResponderID : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1Encodable m_id;

	public virtual X509Name Name
	{
		get
		{
			if (m_id is Asn1OctetString)
			{
				return null;
			}
			return X509Name.GetInstance(m_id);
		}
	}

	public static ResponderID GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ResponderID result)
		{
			return result;
		}
		if (obj is Asn1OctetString id)
		{
			return new ResponderID(id);
		}
		if (obj is Asn1TaggedObject asn1TaggedObject)
		{
			if (asn1TaggedObject.HasContextTag(1))
			{
				return new ResponderID(X509Name.GetInstance(asn1TaggedObject, explicitly: true));
			}
			return new ResponderID(Asn1OctetString.GetInstance(asn1TaggedObject, declaredExplicit: true));
		}
		return new ResponderID(X509Name.GetInstance(obj));
	}

	public static ResponderID GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, isExplicit, GetInstance);
	}

	public ResponderID(Asn1OctetString id)
	{
		m_id = id ?? throw new ArgumentNullException("id");
	}

	public ResponderID(X509Name id)
	{
		m_id = id ?? throw new ArgumentNullException("id");
	}

	public virtual byte[] GetKeyHash()
	{
		if (m_id is Asn1OctetString asn1OctetString)
		{
			return asn1OctetString.GetOctets();
		}
		return null;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (m_id is Asn1OctetString obj)
		{
			return new DerTaggedObject(isExplicit: true, 2, obj);
		}
		return new DerTaggedObject(isExplicit: true, 1, m_id);
	}
}
