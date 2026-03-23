namespace Mirror.BouncyCastle.Asn1;

public class DerSequenceParser : Asn1SequenceParser, IAsn1Convertible
{
	private readonly Asn1StreamParser m_parser;

	internal DerSequenceParser(Asn1StreamParser parser)
	{
		m_parser = parser;
	}

	public IAsn1Convertible ReadObject()
	{
		return m_parser.ReadObject();
	}

	public Asn1Object ToAsn1Object()
	{
		return DLSequence.FromVector(m_parser.ReadVector());
	}
}
