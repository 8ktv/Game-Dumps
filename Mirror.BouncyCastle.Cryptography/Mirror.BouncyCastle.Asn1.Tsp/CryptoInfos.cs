using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class CryptoInfos : Asn1Encodable
{
	private readonly Asn1Sequence m_attributes;

	public static CryptoInfos GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CryptoInfos result)
		{
			return result;
		}
		return new CryptoInfos(Asn1Sequence.GetInstance(obj));
	}

	public static CryptoInfos GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CryptoInfos(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CryptoInfos(Asn1Sequence attributes)
	{
		m_attributes = attributes;
	}

	public CryptoInfos(Attribute[] attrs)
	{
		m_attributes = new DerSequence(attrs);
	}

	public virtual Attribute[] GetAttributes()
	{
		return m_attributes.MapElements(Attribute.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_attributes;
	}
}
