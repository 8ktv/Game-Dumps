using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Cms;

public class DefaultSignedAttributeTableGenerator : CmsAttributeTableGenerator
{
	private readonly IDictionary<DerObjectIdentifier, object> m_table;

	public DefaultSignedAttributeTableGenerator()
	{
		m_table = new Dictionary<DerObjectIdentifier, object>();
	}

	public DefaultSignedAttributeTableGenerator(AttributeTable attributeTable)
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
		DoCreateStandardAttributeTable(parameters, dictionary);
		return dictionary;
	}

	private void DoCreateStandardAttributeTable(IDictionary<CmsAttributeTableParameter, object> parameters, IDictionary<DerObjectIdentifier, object> std)
	{
		if (!std.ContainsKey(CmsAttributes.ContentType) && parameters.TryGetValue(CmsAttributeTableParameter.ContentType, out var value))
		{
			Mirror.BouncyCastle.Asn1.Cms.Attribute attribute = new Mirror.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.ContentType, new DerSet((DerObjectIdentifier)value));
			std[attribute.AttrType] = attribute;
		}
		if (!std.ContainsKey(CmsAttributes.SigningTime))
		{
			Mirror.BouncyCastle.Asn1.Cms.Attribute attribute2 = new Mirror.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.SigningTime, new DerSet(new Time(DateTime.UtcNow)));
			std[attribute2.AttrType] = attribute2;
		}
		if (!std.ContainsKey(CmsAttributes.MessageDigest))
		{
			byte[] contents = (byte[])parameters[CmsAttributeTableParameter.Digest];
			Mirror.BouncyCastle.Asn1.Cms.Attribute attribute3 = new Mirror.BouncyCastle.Asn1.Cms.Attribute(CmsAttributes.MessageDigest, new DerSet(new DerOctetString(contents)));
			std[attribute3.AttrType] = attribute3;
		}
	}

	public virtual AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters)
	{
		return new AttributeTable(CreateStandardAttributeTable(parameters));
	}
}
