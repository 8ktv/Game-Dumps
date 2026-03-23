namespace Mirror.BouncyCastle.Asn1;

internal sealed class Asn1Tag
{
	private readonly int m_tagClass;

	private readonly int m_tagNo;

	internal int TagClass => m_tagClass;

	internal int TagNo => m_tagNo;

	internal static Asn1Tag Create(int tagClass, int tagNo)
	{
		return new Asn1Tag(tagClass, tagNo);
	}

	private Asn1Tag(int tagClass, int tagNo)
	{
		m_tagClass = tagClass;
		m_tagNo = tagNo;
	}
}
