using System;
using System.IO;

namespace Mirror.BouncyCastle.Asn1;

internal class DLBitStringParser : Asn1BitStringParser, IAsn1Convertible
{
	private readonly DefiniteLengthInputStream m_stream;

	private int m_padBits;

	public int PadBits => m_padBits;

	internal DLBitStringParser(DefiniteLengthInputStream stream)
	{
		m_stream = stream;
	}

	public Stream GetBitStream()
	{
		return GetBitStream(octetAligned: false);
	}

	public Stream GetOctetStream()
	{
		return GetBitStream(octetAligned: true);
	}

	public Asn1Object ToAsn1Object()
	{
		try
		{
			return DerBitString.CreatePrimitive(m_stream.ToArray());
		}
		catch (IOException ex)
		{
			throw new Asn1ParsingException("IOException converting stream to byte array: " + ex.Message, ex);
		}
	}

	private Stream GetBitStream(bool octetAligned)
	{
		int remaining = m_stream.Remaining;
		if (remaining < 1)
		{
			throw new InvalidOperationException("content octets cannot be empty");
		}
		m_padBits = m_stream.ReadByte();
		if (m_padBits > 0)
		{
			if (remaining < 2)
			{
				throw new InvalidOperationException("zero length data with non-zero pad bits");
			}
			if (m_padBits > 7)
			{
				throw new InvalidOperationException("pad bits cannot be greater than 7 or less than 0");
			}
			if (octetAligned)
			{
				throw new IOException("expected octet-aligned bitstring, but found padBits: " + m_padBits);
			}
		}
		return m_stream;
	}
}
