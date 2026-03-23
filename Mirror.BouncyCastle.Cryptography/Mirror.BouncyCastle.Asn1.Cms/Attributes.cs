namespace Mirror.BouncyCastle.Asn1.Cms;

public class Attributes : Asn1Encodable
{
	private readonly Asn1Set m_attributes;

	public static Attributes GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Attributes result)
		{
			return result;
		}
		return new Attributes(Asn1Set.GetInstance(obj));
	}

	public static Attributes GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new Attributes(Asn1Set.GetInstance(taggedObject, declaredExplicit));
	}

	private Attributes(Asn1Set attributes)
	{
		m_attributes = attributes;
	}

	public Attributes(Asn1EncodableVector v)
	{
		m_attributes = BerSet.FromVector(v);
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
