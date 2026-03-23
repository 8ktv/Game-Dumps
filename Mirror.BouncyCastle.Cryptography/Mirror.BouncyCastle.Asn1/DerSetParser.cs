namespace Mirror.BouncyCastle.Asn1;

public class DerSetParser : Asn1SetParser, IAsn1Convertible
{
	private readonly Asn1StreamParser m_parser;

	internal DerSetParser(Asn1StreamParser parser)
	{
		m_parser = parser;
	}

	public IAsn1Convertible ReadObject()
	{
		return m_parser.ReadObject();
	}

	public Asn1Object ToAsn1Object()
	{
		return DLSet.FromVector(m_parser.ReadVector());
	}
}
