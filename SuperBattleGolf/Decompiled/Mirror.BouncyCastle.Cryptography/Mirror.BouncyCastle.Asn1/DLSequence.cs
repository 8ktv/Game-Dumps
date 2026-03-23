namespace Mirror.BouncyCastle.Asn1;

public class DLSequence : DerSequence
{
	public new static readonly DLSequence Empty = new DLSequence();

	public new static DLSequence Concatenate(params Asn1Sequence[] sequences)
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

	internal new static DLSequence FromElements(Asn1Encodable[] elements)
	{
		if (elements.Length >= 1)
		{
			return new DLSequence(elements, clone: false);
		}
		return Empty;
	}

	public new static DLSequence FromSequence(Asn1Sequence sequence)
	{
		if (sequence is DLSequence result)
		{
			return result;
		}
		return FromElements(sequence.m_elements);
	}

	public new static DLSequence FromVector(Asn1EncodableVector elementVector)
	{
		if (elementVector.Count >= 1)
		{
			return new DLSequence(elementVector);
		}
		return Empty;
	}

	public DLSequence()
	{
	}

	public DLSequence(Asn1Encodable element)
		: base(element)
	{
	}

	public DLSequence(Asn1Encodable element1, Asn1Encodable element2)
		: base(element1, element2)
	{
	}

	public DLSequence(params Asn1Encodable[] elements)
		: base(elements)
	{
	}

	public DLSequence(Asn1EncodableVector elementVector)
		: base(elementVector)
	{
	}

	internal DLSequence(Asn1Encodable[] elements, bool clone)
		: base(elements, clone)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (3 == encoding)
		{
			return base.GetEncoding(encoding);
		}
		return new ConstructedDLEncoding(0, 16, Asn1OutputStream.GetContentsEncodings(2, m_elements));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (3 == encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new ConstructedDLEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(2, m_elements));
	}

	internal override DerBitString ToAsn1BitString()
	{
		return new DLBitString(BerBitString.FlattenBitStrings(GetConstructedBitStrings()), check: false);
	}

	internal override DerExternal ToAsn1External()
	{
		return new DLExternal(this);
	}

	internal override Asn1Set ToAsn1Set()
	{
		return new DLSet(isSorted: false, m_elements);
	}
}
