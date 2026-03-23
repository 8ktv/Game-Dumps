using System;

namespace Mirror.BouncyCastle.Asn1;

internal class ConstructedDerEncoding : DerEncoding
{
	private readonly DerEncoding[] m_contentsElements;

	private readonly int m_contentsLength;

	internal ConstructedDerEncoding(int tagClass, int tagNo, DerEncoding[] contentsElements)
		: base(tagClass, tagNo)
	{
		m_contentsElements = contentsElements;
		m_contentsLength = Asn1OutputStream.GetLengthOfContents(contentsElements);
	}

	protected internal override int CompareLengthAndContents(DerEncoding other)
	{
		if (!(other is ConstructedDerEncoding constructedDerEncoding))
		{
			throw new InvalidOperationException();
		}
		if (m_contentsLength != constructedDerEncoding.m_contentsLength)
		{
			return m_contentsLength - constructedDerEncoding.m_contentsLength;
		}
		int num = System.Math.Min(m_contentsElements.Length, constructedDerEncoding.m_contentsElements.Length);
		for (int i = 0; i < num; i++)
		{
			int num2 = m_contentsElements[i].CompareTo(constructedDerEncoding.m_contentsElements[i]);
			if (num2 != 0)
			{
				return num2;
			}
		}
		return m_contentsElements.Length - constructedDerEncoding.m_contentsElements.Length;
	}

	public override void Encode(Asn1OutputStream asn1Out)
	{
		asn1Out.WriteIdentifier(0x20 | m_tagClass, m_tagNo);
		asn1Out.WriteDL(m_contentsLength);
		IAsn1Encoding[] contentsElements = m_contentsElements;
		asn1Out.EncodeContents(contentsElements);
	}

	public override int GetLength()
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(m_tagNo, m_contentsLength);
	}
}
