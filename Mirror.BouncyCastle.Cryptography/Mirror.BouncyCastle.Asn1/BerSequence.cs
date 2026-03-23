namespace Mirror.BouncyCastle.Asn1;

public class BerSequence : DLSequence
{
	public new static readonly BerSequence Empty = new BerSequence();

	public new static BerSequence Concatenate(params Asn1Sequence[] sequences)
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

	internal new static BerSequence FromElements(Asn1Encodable[] elements)
	{
		if (elements.Length >= 1)
		{
			return new BerSequence(elements, clone: false);
		}
		return Empty;
	}

	public new static BerSequence FromSequence(Asn1Sequence sequence)
	{
		if (sequence is BerSequence result)
		{
			return result;
		}
		return FromElements(sequence.m_elements);
	}

	public new static BerSequence FromVector(Asn1EncodableVector elementVector)
	{
		if (elementVector.Count >= 1)
		{
			return new BerSequence(elementVector);
		}
		return Empty;
	}

	public BerSequence()
	{
	}

	public BerSequence(Asn1Encodable element)
		: base(element)
	{
	}

	public BerSequence(Asn1Encodable element1, Asn1Encodable element2)
		: base(element1, element2)
	{
	}

	public BerSequence(params Asn1Encodable[] elements)
		: base(elements)
	{
	}

	public BerSequence(Asn1EncodableVector elementVector)
		: base(elementVector)
	{
	}

	internal BerSequence(Asn1Encodable[] elements, bool clone)
		: base(elements, clone)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (1 != encoding)
		{
			return base.GetEncoding(encoding);
		}
		return new ConstructedILEncoding(0, 16, Asn1OutputStream.GetContentsEncodings(encoding, m_elements));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (1 != encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new ConstructedILEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(encoding, m_elements));
	}

	internal override DerBitString ToAsn1BitString()
	{
		return new BerBitString(GetConstructedBitStrings());
	}

	internal override DerExternal ToAsn1External()
	{
		return new DLExternal(this);
	}

	internal override Asn1OctetString ToAsn1OctetString()
	{
		return new BerOctetString(GetConstructedOctetStrings());
	}

	internal override Asn1Set ToAsn1Set()
	{
		return new BerSet(isSorted: false, m_elements);
	}
}
