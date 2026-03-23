namespace Mirror.BouncyCastle.Asn1;

internal class ConstructedILEncoding : IAsn1Encoding
{
	private readonly int m_tagClass;

	private readonly int m_tagNo;

	private readonly IAsn1Encoding[] m_contentsElements;

	internal ConstructedILEncoding(int tagClass, int tagNo, IAsn1Encoding[] contentsElements)
	{
		m_tagClass = tagClass;
		m_tagNo = tagNo;
		m_contentsElements = contentsElements;
	}

	void IAsn1Encoding.Encode(Asn1OutputStream asn1Out)
	{
		asn1Out.WriteIdentifier(0x20 | m_tagClass, m_tagNo);
		asn1Out.WriteByte(128);
		asn1Out.EncodeContents(m_contentsElements);
		asn1Out.WriteByte(0);
		asn1Out.WriteByte(0);
	}

	int IAsn1Encoding.GetLength()
	{
		return Asn1OutputStream.GetLengthOfEncodingIL(m_tagNo, m_contentsElements);
	}
}
