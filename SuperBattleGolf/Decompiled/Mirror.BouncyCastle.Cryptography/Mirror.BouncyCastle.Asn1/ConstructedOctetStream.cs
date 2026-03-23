using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class ConstructedOctetStream : BaseInputStream
{
	private readonly Asn1StreamParser m_parser;

	private bool m_first = true;

	private Stream m_currentStream;

	internal ConstructedOctetStream(Asn1StreamParser parser)
	{
		m_parser = parser;
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
			Asn1OctetStringParser nextParser = GetNextParser();
			if (nextParser == null)
			{
				return 0;
			}
			m_first = false;
			m_currentStream = nextParser.GetOctetStream();
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
			Asn1OctetStringParser nextParser2 = GetNextParser();
			if (nextParser2 == null)
			{
				break;
			}
			m_currentStream = nextParser2.GetOctetStream();
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
			Asn1OctetStringParser nextParser = GetNextParser();
			if (nextParser == null)
			{
				return -1;
			}
			m_first = false;
			m_currentStream = nextParser.GetOctetStream();
		}
		while (true)
		{
			int num = m_currentStream.ReadByte();
			if (num >= 0)
			{
				return num;
			}
			Asn1OctetStringParser nextParser2 = GetNextParser();
			if (nextParser2 == null)
			{
				break;
			}
			m_currentStream = nextParser2.GetOctetStream();
		}
		m_currentStream = null;
		return -1;
	}

	private Asn1OctetStringParser GetNextParser()
	{
		IAsn1Convertible asn1Convertible = m_parser.ReadObject();
		if (asn1Convertible == null)
		{
			return null;
		}
		if (asn1Convertible is Asn1OctetStringParser)
		{
			return (Asn1OctetStringParser)asn1Convertible;
		}
		throw new IOException("unknown object encountered: " + Platform.GetTypeName(asn1Convertible));
	}
}
