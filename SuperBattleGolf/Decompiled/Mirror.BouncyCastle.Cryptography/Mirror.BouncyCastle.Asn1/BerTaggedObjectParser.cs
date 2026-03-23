using System.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class BerTaggedObjectParser : Asn1TaggedObjectParser, IAsn1Convertible
{
	internal readonly int m_tagClass;

	internal readonly int m_tagNo;

	internal readonly Asn1StreamParser m_parser;

	public virtual bool IsConstructed => true;

	public int TagClass => m_tagClass;

	public int TagNo => m_tagNo;

	internal BerTaggedObjectParser(int tagClass, int tagNo, Asn1StreamParser parser)
	{
		m_tagClass = tagClass;
		m_tagNo = tagNo;
		m_parser = parser;
	}

	public bool HasContextTag()
	{
		return m_tagClass == 128;
	}

	public bool HasContextTag(int tagNo)
	{
		if (m_tagClass == 128)
		{
			return m_tagNo == tagNo;
		}
		return false;
	}

	public bool HasTag(int tagClass, int tagNo)
	{
		if (m_tagClass == tagClass)
		{
			return m_tagNo == tagNo;
		}
		return false;
	}

	public bool HasTagClass(int tagClass)
	{
		return m_tagClass == tagClass;
	}

	public virtual IAsn1Convertible ParseBaseUniversal(bool declaredExplicit, int baseTagNo)
	{
		if (declaredExplicit)
		{
			return m_parser.ParseObject(baseTagNo);
		}
		return m_parser.ParseImplicitConstructedIL(baseTagNo);
	}

	public virtual IAsn1Convertible ParseExplicitBaseObject()
	{
		return m_parser.ReadObject();
	}

	public virtual Asn1TaggedObjectParser ParseExplicitBaseTagged()
	{
		return m_parser.ParseTaggedObject();
	}

	public virtual Asn1TaggedObjectParser ParseImplicitBaseTagged(int baseTagClass, int baseTagNo)
	{
		return new BerTaggedObjectParser(baseTagClass, baseTagNo, m_parser);
	}

	public virtual Asn1Object ToAsn1Object()
	{
		try
		{
			return m_parser.LoadTaggedIL(TagClass, TagNo);
		}
		catch (IOException ex)
		{
			throw new Asn1ParsingException(ex.Message);
		}
	}
}
