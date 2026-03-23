using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class AttributeTable
{
	private readonly Dictionary<DerObjectIdentifier, object> m_attributes;

	public Attribute this[DerObjectIdentifier oid]
	{
		get
		{
			if (!m_attributes.TryGetValue(oid, out var value))
			{
				return null;
			}
			if (value is IList<Attribute> list)
			{
				return list[0];
			}
			if (value is Attribute result)
			{
				return result;
			}
			throw new InvalidOperationException();
		}
	}

	public int Count
	{
		get
		{
			int num = 0;
			foreach (object value in m_attributes.Values)
			{
				if (value is IList<Attribute> list)
				{
					num += list.Count;
					continue;
				}
				if (value is Attribute)
				{
					num++;
					continue;
				}
				throw new InvalidOperationException();
			}
			return num;
		}
	}

	public AttributeTable(IDictionary<DerObjectIdentifier, object> attrs)
	{
		m_attributes = new Dictionary<DerObjectIdentifier, object>(attrs);
	}

	public AttributeTable(Asn1EncodableVector v)
	{
		m_attributes = new Dictionary<DerObjectIdentifier, object>(v.Count);
		foreach (Asn1Encodable item in v)
		{
			AddAttribute(Attribute.GetInstance(item));
		}
	}

	public AttributeTable(Asn1Set s)
	{
		m_attributes = new Dictionary<DerObjectIdentifier, object>(s.Count);
		foreach (Asn1Encodable item in s)
		{
			AddAttribute(Attribute.GetInstance(item));
		}
	}

	public AttributeTable(Attributes attrs)
		: this(Asn1Set.GetInstance(attrs.ToAsn1Object()))
	{
	}

	private void AddAttribute(Attribute a)
	{
		DerObjectIdentifier attrType = a.AttrType;
		if (!m_attributes.TryGetValue(attrType, out var value))
		{
			m_attributes[attrType] = a;
			return;
		}
		if (value is IList<Attribute> list)
		{
			list.Add(a);
			return;
		}
		if (value is Attribute item)
		{
			List<Attribute> list2 = new List<Attribute>();
			list2.Add(item);
			list2.Add(a);
			m_attributes[attrType] = list2;
			return;
		}
		throw new InvalidOperationException();
	}

	public Asn1EncodableVector GetAll(DerObjectIdentifier oid)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		if (m_attributes.TryGetValue(oid, out var value))
		{
			if (value is IList<Attribute> list)
			{
				foreach (Attribute item in list)
				{
					asn1EncodableVector.Add(item);
				}
			}
			else
			{
				if (!(value is Attribute element))
				{
					throw new InvalidOperationException();
				}
				asn1EncodableVector.Add(element);
			}
		}
		return asn1EncodableVector;
	}

	public IDictionary<DerObjectIdentifier, object> ToDictionary()
	{
		return new Dictionary<DerObjectIdentifier, object>(m_attributes);
	}

	public Asn1EncodableVector ToAsn1EncodableVector()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		foreach (object value in m_attributes.Values)
		{
			if (value is IList<Attribute> list)
			{
				foreach (Attribute item in list)
				{
					asn1EncodableVector.Add(item);
				}
			}
			else
			{
				if (!(value is Attribute element))
				{
					throw new InvalidOperationException();
				}
				asn1EncodableVector.Add(element);
			}
		}
		return asn1EncodableVector;
	}

	public Attributes ToAttributes()
	{
		return new Attributes(ToAsn1EncodableVector());
	}

	public AttributeTable Add(params Attribute[] attributes)
	{
		if (attributes == null || attributes.Length < 1)
		{
			return this;
		}
		AttributeTable attributeTable = new AttributeTable(m_attributes);
		foreach (Attribute a in attributes)
		{
			attributeTable.AddAttribute(a);
		}
		return attributeTable;
	}

	public AttributeTable Add(DerObjectIdentifier attrType, Asn1Encodable attrValue)
	{
		AttributeTable attributeTable = new AttributeTable(m_attributes);
		attributeTable.AddAttribute(new Attribute(attrType, new DerSet(attrValue)));
		return attributeTable;
	}

	public AttributeTable Remove(DerObjectIdentifier attrType)
	{
		AttributeTable attributeTable = new AttributeTable(m_attributes);
		attributeTable.m_attributes.Remove(attrType);
		return attributeTable;
	}
}
