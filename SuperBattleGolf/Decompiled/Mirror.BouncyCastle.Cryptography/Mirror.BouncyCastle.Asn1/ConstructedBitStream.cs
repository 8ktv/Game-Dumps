using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class ConstructedBitStream : BaseInputStream
{
	private readonly Asn1StreamParser m_parser;

	private readonly bool m_octetAligned;

	private bool m_first = true;

	private int m_padBits;

	private Asn1BitStringParser m_currentParser;

	private Stream m_currentStream;

	internal int PadBits => m_padBits;

	internal ConstructedBitStream(Asn1StreamParser parser, bool octetAligned)
	{
		m_parser = parser;
		m_octetAligned = octetAligned;
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		Streams.ValidateBufferArguments(buffer, offset, count);
		if (count < 1)
		{
			return 0;
		}
		if (m_currentStream == null)
		{
			if (!m_first)
			{
				return 0;
			}
			m_currentParser = GetNextParser();
			if (m_currentParser == null)
			{
				return 0;
			}
			m_first = false;
			m_currentStream = m_currentParser.GetBitStream();
		}
		int num = 0;
		while (true)
		{
			int num2 = m_currentStream.Read(buffer, offset + num, count - num);
			if (num2 > 0)
			{
				num += num2;
				if (num == count)
				{
					return num;
				}
				continue;
			}
			m_padBits = m_currentParser.PadBits;
			m_currentParser = GetNextParser();
			if (m_currentParser == null)
			{
				break;
			}
			m_currentStream = m_currentParser.GetBitStream();
		}
		m_currentStream = null;
		return num;
	}

	public override int ReadByte()
	{
		if (m_currentStream == null)
		{
			if (!m_first)
			{
				return -1;
			}
			m_currentParser = GetNextParser();
			if (m_currentParser == null)
			{
				return -1;
			}
			m_first = false;
			m_currentStream = m_currentParser.GetBitStream();
		}
		while (true)
		{
			int num = m_currentStream.ReadByte();
			if (num >= 0)
			{
				return num;
			}
			m_padBits = m_currentParser.PadBits;
			m_currentParser = GetNextParser();
			if (m_currentParser == null)
			{
				break;
			}
			m_currentStream = m_currentParser.GetBitStream();
		}
		m_currentStream = null;
		return -1;
	}

	private Asn1BitStringParser GetNextParser()
	{
		IAsn1Convertible asn1Convertible = m_parser.ReadObject();
		if (asn1Convertible == null)
		{
			if (m_octetAligned && m_padBits != 0)
			{
				throw new IOException("expected octet-aligned bitstring, but found padBits: " + m_padBits);
			}
			return null;
		}
		if (asn1Convertible is Asn1BitStringParser)
		{
			if (m_padBits != 0)
			{
				throw new IOException("only the last nested bitstring can have padding");
			}
			return (Asn1BitStringParser)asn1Convertible;
		}
		throw new IOException("unknown object encountered: " + Platform.GetTypeName(asn1Convertible));
	}
}
