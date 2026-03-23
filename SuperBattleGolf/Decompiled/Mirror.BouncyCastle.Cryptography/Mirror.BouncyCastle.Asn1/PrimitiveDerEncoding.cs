using System;

namespace Mirror.BouncyCastle.Asn1;

internal class PrimitiveDerEncoding : DerEncoding
{
	internal readonly byte[] m_contentsOctets;

	internal PrimitiveDerEncoding(int tagClass, int tagNo, byte[] contentsOctets)
		: base(tagClass, tagNo)
	{
		m_contentsOctets = contentsOctets;
	}

	protected internal override int CompareLengthAndContents(DerEncoding other)
	{
		if (other is PrimitiveDerEncodingSuffixed primitiveDerEncodingSuffixed)
		{
			return -primitiveDerEncodingSuffixed.CompareLengthAndContents(this);
		}
		if (!(other is PrimitiveDerEncoding primitiveDerEncoding))
		{
			throw new InvalidOperationException();
		}
		int num = m_contentsOctets.Length;
		if (num != primitiveDerEncoding.m_contentsOctets.Length)
		{
			return num - primitiveDerEncoding.m_contentsOctets.Length;
		}
		for (int i = 0; i < num; i++)
		{
			byte b = m_contentsOctets[i];
			byte b2 = primitiveDerEncoding.m_contentsOctets[i];
			if (b != b2)
			{
				return b - b2;
			}
		}
		return 0;
	}

	public override void Encode(Asn1OutputStream asn1Out)
	{
		asn1Out.WriteIdentifier(m_tagClass, m_tagNo);
		asn1Out.WriteDL(m_contentsOctets.Length);
		asn1Out.Write(m_contentsOctets, 0, m_contentsOctets.Length);
	}

	public override int GetLength()
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(m_tagNo, m_contentsOctets.Length);
	}
}
