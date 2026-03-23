using System;

namespace Mirror.BouncyCastle.Asn1;

internal class TaggedDerEncoding : DerEncoding
{
	private readonly DerEncoding m_contentsElement;

	private readonly int m_contentsLength;

	internal TaggedDerEncoding(int tagClass, int tagNo, DerEncoding contentsElement)
		: base(tagClass, tagNo)
	{
		m_contentsElement = contentsElement;
		m_contentsLength = contentsElement.GetLength();
	}

	protected internal override int CompareLengthAndContents(DerEncoding other)
	{
		if (!(other is TaggedDerEncoding taggedDerEncoding))
		{
			throw new InvalidOperationException();
		}
		if (m_contentsLength != taggedDerEncoding.m_contentsLength)
		{
			return m_contentsLength - taggedDerEncoding.m_contentsLength;
		}
		return m_contentsElement.CompareTo(taggedDerEncoding.m_contentsElement);
	}

	public override void Encode(Asn1OutputStream asn1Out)
	{
		asn1Out.WriteIdentifier(0x20 | m_tagClass, m_tagNo);
		asn1Out.WriteDL(m_contentsLength);
		m_contentsElement.Encode(asn1Out);
	}

	public override int GetLength()
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(m_tagNo, m_contentsLength);
	}
}
