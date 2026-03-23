using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class BerBitStringParser : Asn1BitStringParser, IAsn1Convertible
{
	private readonly Asn1StreamParser m_parser;

	private ConstructedBitStream m_bitStream;

	public int PadBits => m_bitStream.PadBits;

	internal BerBitStringParser(Asn1StreamParser parser)
	{
		m_parser = parser;
	}

	public Stream GetOctetStream()
	{
		return m_bitStream = new ConstructedBitStream(m_parser, octetAligned: true);
	}

	public Stream GetBitStream()
	{
		return m_bitStream = new ConstructedBitStream(m_parser, octetAligned: false);
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

	internal static BerBitString Parse(Asn1StreamParser sp)
	{
		ConstructedBitStream constructedBitStream = new ConstructedBitStream(sp, octetAligned: false);
		byte[] data = Streams.ReadAll(constructedBitStream);
		int padBits = constructedBitStream.PadBits;
		return new BerBitString(data, padBits);
	}
}
