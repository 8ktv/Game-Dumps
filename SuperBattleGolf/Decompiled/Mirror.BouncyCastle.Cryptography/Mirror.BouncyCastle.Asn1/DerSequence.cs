namespace Mirror.BouncyCastle.Asn1;

public class DerSequence : Asn1Sequence
{
	public static readonly DerSequence Empty = new DerSequence();

	public static DerSequence Concatenate(params Asn1Sequence[] sequences)
	{
		if (sequences == null)
		{
			return Empty;
		}
		return sequences.Length switch
		{
			0 => Empty, 
			1 => FromSequence(sequences[0]), 
			_ => FromElements(Asn1Sequence.ConcatenateElements(sequences)), 
		};
	}

	internal static DerSequence FromElements(Asn1Encodable[] elements)
	{
		if (elements.Length >= 1)
		{
			return new DerSequence(elements, clone: false);
		}
		return Empty;
	}

	public static DerSequence FromSequence(Asn1Sequence sequence)
	{
		if (sequence is DerSequence result)
		{
			return result;
		}
		return FromElements(sequence.m_elements);
	}

	public static DerSequence FromVector(Asn1EncodableVector elementVector)
	{
		if (elementVector.Count >= 1)
		{
			return new DerSequence(elementVector);
		}
		return Empty;
	}

	public DerSequence()
	{
	}

	public DerSequence(Asn1Encodable element)
		: base(element)
	{
	}

	public DerSequence(Asn1Encodable element1, Asn1Encodable element2)
		: base(element1, element2)
	{
	}

	public DerSequence(params Asn1Encodable[] elements)
		: base(elements)
	{
	}

	public DerSequence(Asn1EncodableVector elementVector)
		: base(elementVector)
	{
	}

	internal DerSequence(Asn1Encodable[] elements, bool clone)
		: base(elements, clone)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new ConstructedDLEncoding(0, 16, Asn1OutputStream.GetContentsEncodings(3, m_elements));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new ConstructedDLEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(3, m_elements));
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new ConstructedDerEncoding(0, 16, Asn1OutputStream.GetContentsEncodingsDer(m_elements));
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new ConstructedDerEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodingsDer(m_elements));
	}

	internal override DerBitString ToAsn1BitString()
	{
		return new DerBitString(BerBitString.FlattenBitStrings(GetConstructedBitStrings()), check: false);
	}

	internal override DerExternal ToAsn1External()
	{
		return new DerExternal(this);
	}

	internal override Asn1OctetString ToAsn1OctetString()
	{
		return new DerOctetString(BerOctetString.FlattenOctetStrings(GetConstructedOctetStrings()));
	}

	internal override Asn1Set ToAsn1Set()
	{
		return new DLSet(isSorted: false, m_elements);
	}

	internal static int GetEncodingLength(int contentsLength)
	{
		return Asn1OutputStream.GetLengthOfEncodingDL(16, contentsLength);
	}
}
