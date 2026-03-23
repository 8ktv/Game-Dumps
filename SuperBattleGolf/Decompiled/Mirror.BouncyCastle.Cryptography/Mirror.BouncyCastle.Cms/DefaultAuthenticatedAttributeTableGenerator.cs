using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Cms;

public class DefaultAuthenticatedAttributeTableGenerator : CmsAttributeTableGenerator
{
	private readonly IDictionary<DerObjectIdentifier, object> m_table;

	public DefaultAuthenticatedAttributeTableGenerator()
	{
		m_table = new Dictionary<DerObjectIdentifier, object>();
	}

	public DefaultAuthenticatedAttributeTableGenerator(AttributeTable attributeTable)
	{
		if (attributeTable != null)
		{
			m_table = attributeTable.ToDictionary();
		}
		else
		{
			m_table = new Dictionary<DerObjectIdentifier, object>();
		}
	}

	protected virtual IDictionary<DerObjectIdentifier, object> CreateStandardAttributeTable(IDictionary<CmsAttributeTableParameter, object> parameters)
	{
		Dictionary<DerObjectIdentifier, object> dictionary = new Dictionary<DerObjectIdentifier, object>(m_table);
		if (!dictionary.ContainsKey(CmsAttributes.ContentType))
		{
			DerObjectIdentifier element = (DerObjectIdentifier)parameters[CmsAttributeTableParameter.ContentType];
			Attribute attribute = new Attribute(CmsAttributes.ContentType, new DerSet(element));
			dictionary[attribute.AttrType] = attribute;
		}
		if (!dictionary.ContainsKey(CmsAttributes.MessageDigest))
		{
			byte[] contents = (byte[])parameters[CmsAttributeTableParameter.Digest];
			Attribute attribute2 = new Attribute(CmsAttributes.MessageDigest, new DerSet(new DerOctetString(contents)));
			dictionary[attribute2.AttrType] = attribute2;
		}
		return dictionary;
	}

	public virtual AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters)
	{
		return new AttributeTable(CreateStandardAttributeTable(parameters));
	}
}
