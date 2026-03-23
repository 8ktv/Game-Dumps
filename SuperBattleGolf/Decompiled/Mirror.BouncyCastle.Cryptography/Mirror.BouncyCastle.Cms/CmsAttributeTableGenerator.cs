using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Cms;

public interface CmsAttributeTableGenerator
{
	AttributeTable GetAttributes(IDictionary<CmsAttributeTableParameter, object> parameters);
}
