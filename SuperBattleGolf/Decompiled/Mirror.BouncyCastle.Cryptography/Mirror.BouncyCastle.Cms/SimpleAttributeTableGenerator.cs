using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Cms;

public class SimpleAttributeTableGenerator : CmsAttributeTableGenerator
{
	private readonly AttributeTable attributes;

	public SimpleAttributeTableGenerator(AttributeTable attributes)
	{
		this.attributes = attributes;
	}

	public virtual AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters)
	{
		return attributes;
	}
}
