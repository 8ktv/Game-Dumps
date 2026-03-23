using System.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class DLTaggedObjectParser : BerTaggedObjectParser
{
	private readonly bool m_constructed;

	public override bool IsConstructed => m_constructed;

	internal DLTaggedObjectParser(int tagClass, int tagNo, bool constructed, Asn1StreamParser parser)
		: base(tagClass, tagNo, parser)
	{
		m_constructed = constructed;
	}

	public override IAsn1Convertible ParseBaseUniversal(bool declaredExplicit, int baseTagNo)
	{
		if (declaredExplicit)
		{
			return CheckConstructed().ParseObject(baseTagNo);
		}
		if (!m_constructed)
		{
			return m_parser.ParseImplicitPrimitive(baseTagNo);
		}
		return m_parser.ParseImplicitConstructedDL(baseTagNo);
	}

	public override IAsn1Convertible ParseExplicitBaseObject()
	{
		return CheckConstructed().ReadObject();
	}

	public override Asn1TaggedObjectParser ParseExplicitBaseTagged()
	{
		return CheckConstructed().ParseTaggedObject();
	}

	public override Asn1TaggedObjectParser ParseImplicitBaseTagged(int baseTagClass, int baseTagNo)
	{
		return new DLTaggedObjectParser(baseTagClass, baseTagNo, m_constructed, m_parser);
	}

	public override Asn1Object ToAsn1Object()
	{
		try
		{
			return m_parser.LoadTaggedDL(base.TagClass, base.TagNo, m_constructed);
		}
		catch (IOException ex)
		{
			throw new Asn1ParsingException(ex.Message);
		}
	}

	private Asn1StreamParser CheckConstructed()
	{
		if (!m_constructed)
		{
			throw new IOException("Explicit tags must be constructed (see X.690 8.14.2)");
		}
		return m_parser;
	}
}
