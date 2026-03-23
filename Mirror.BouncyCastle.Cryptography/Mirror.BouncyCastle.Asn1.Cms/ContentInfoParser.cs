namespace Mirror.BouncyCastle.Asn1.Cms;

public class ContentInfoParser
{
	private readonly DerObjectIdentifier m_contentType;

	private readonly Asn1TaggedObjectParser m_content;

	public DerObjectIdentifier ContentType => m_contentType;

	public ContentInfoParser(Asn1SequenceParser seq)
	{
		m_contentType = (DerObjectIdentifier)seq.ReadObject();
		m_content = (Asn1TaggedObjectParser)seq.ReadObject();
	}

	public IAsn1Convertible GetContent(int tag)
	{
		if (m_content == null)
		{
			return null;
		}
		return Asn1Utilities.ParseExplicitContextBaseObject(m_content, 0);
	}
}
