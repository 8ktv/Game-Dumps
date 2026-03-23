using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Asn1.X509;

public class SubjectDirectoryAttributes : Asn1Encodable
{
	private readonly List<AttributeX509> m_attributes;

	public IEnumerable<AttributeX509> Attributes => CollectionUtilities.Proxy(m_attributes);

	public static SubjectDirectoryAttributes GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SubjectDirectoryAttributes result)
		{
			return result;
		}
		return new SubjectDirectoryAttributes(Asn1Sequence.GetInstance(obj));
	}

	private SubjectDirectoryAttributes(Asn1Sequence seq)
	{
		m_attributes = new List<AttributeX509>();
		foreach (Asn1Encodable item in seq)
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(item);
			m_attributes.Add(AttributeX509.GetInstance(instance));
		}
	}

	public SubjectDirectoryAttributes(IList<AttributeX509> attributes)
	{
		m_attributes = new List<AttributeX509>(attributes);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1Encodable[] elements = m_attributes.ToArray();
		return new DerSequence(elements);
	}
}
