namespace Mirror.BouncyCastle.Asn1.X500;

public class Rdn : Asn1Encodable
{
	private readonly Asn1Set m_values;

	public virtual bool IsMultiValued => m_values.Count > 1;

	public virtual int Count => m_values.Count;

	public static Rdn GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Rdn result)
		{
			return result;
		}
		return new Rdn(Asn1Set.GetInstance(obj));
	}

	public static Rdn GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new Rdn(Asn1Set.GetInstance(taggedObject, declaredExplicit));
	}

	private Rdn(Asn1Set values)
	{
		m_values = values;
	}

	public Rdn(DerObjectIdentifier oid, Asn1Encodable value)
		: this(new AttributeTypeAndValue(oid, value))
	{
	}

	public Rdn(AttributeTypeAndValue attrTAndV)
	{
		m_values = new DerSet(attrTAndV);
	}

	public Rdn(AttributeTypeAndValue[] aAndVs)
	{
		m_values = new DerSet(aAndVs);
	}

	public virtual AttributeTypeAndValue GetFirst()
	{
		if (m_values.Count != 0)
		{
			return AttributeTypeAndValue.GetInstance(m_values[0]);
		}
		return null;
	}

	public virtual AttributeTypeAndValue[] GetTypesAndValues()
	{
		return m_values.MapElements(AttributeTypeAndValue.GetInstance);
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_values;
	}
}
