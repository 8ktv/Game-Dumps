using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

[Obsolete("Check for 'Asn1OctetStringParser' instead")]
public class BerOctetStringParser : Asn1OctetStringParser, IAsn1Convertible
{
	private readonly Asn1StreamParser m_parser;

	internal BerOctetStringParser(Asn1StreamParser parser)
	{
		m_parser = parser;
	}

	public Stream GetOctetStream()
	{
		return new ConstructedOctetStream(m_parser);
	}

	public Asn1Object ToAsn1Object()
	{
		try
		{
			return Parse(m_parser);
		}
		catch (IOException ex)
		{
			throw new Asn1ParsingException("IOException converting stream to byte array: " + ex.Message, ex);
		}
	}

	internal static BerOctetString Parse(Asn1StreamParser sp)
	{
		return new BerOctetString(Streams.ReadAll(new ConstructedOctetStream(sp)));
	}
}
