using System;

namespace Mirror.BouncyCastle.Asn1;

internal class PrimitiveDerEncodingSuffixed : DerEncoding
{
	private readonly byte[] m_contentsOctets;

	private readonly byte m_contentsSuffix;

	internal PrimitiveDerEncodingSuffixed(int tagClass, int tagNo, byte[] contentsOctets, byte contentsSuffix)
		: base(tagClass, tagNo)
	{
		m_contentsOctets = contentsOctets;
		m_contentsSuffix = contentsSuffix;
	}

	protected internal override int CompareLengthAndContents(DerEncoding other)
	{
		if (other is PrimitiveDerEncodingSuffixed primitiveDerEncodingSuffixed)
		{
			return CompareSuffixed(m_contentsOctets, m_contentsSuffix, primitiveDerEncodingSuffixed.m_contentsOctets, primitiveDerEncodingSuffixed.m_contentsSuffix);
		}
		if (other is PrimitiveDerEncoding primitiveDerEncoding)
		{
			int num = primitiveDerEncoding.m_contentsOctets.Length;
			if (num == 0)
			{
				return m_contentsOctets.Length;
			}
			return CompareSuffixed(m_contentsOctets, m_contentsSuffix, primitiveDerEncoding.m_contentsOctets, primitiveDerEncoding.m_contentsOctets[num - 1]);
		}
		throw new InvalidOperationException();
	}

	public override void Encode(Asn1OutputStream asn1Out)
	{
		asn1Out.WriteIdentifier(m_tagClass, m_tagNo);
		asn1Out.WriteDL(m_contentsOctets.Length);
		asn1Out.Write(m_contentsOctets, 0, m_contentsOctets.Length - 1);
		asn1Out.WriteByte(m_contentsSuffix);
	}

	public override int GetLength()
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(m_tagNo, m_contentsOctets.Length);
	}

	private static int CompareSuffixed(byte[] octetsA, byte suffixA, byte[] octetsB, byte suffixB)
	{
		int num = octetsA.Length;
		if (num != octetsB.Length)
		{
			return num - octetsB.Length;
		}
		int num2 = num - 1;
		for (int i = 0; i < num2; i++)
		{
			byte b = octetsA[i];
			byte b2 = octetsB[i];
			if (b != b2)
			{
				return b - b2;
			}
		}
		return suffixA - suffixB;
	}
}
