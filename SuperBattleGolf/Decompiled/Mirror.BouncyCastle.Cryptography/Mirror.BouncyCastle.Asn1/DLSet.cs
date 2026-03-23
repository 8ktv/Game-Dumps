namespace Mirror.BouncyCastle.Asn1;

public class DLSet : DerSet
{
	public new static readonly DLSet Empty = new DLSet();

	public new static DLSet FromVector(Asn1EncodableVector elementVector)
	{
		if (elementVector.Count >= 1)
		{
			return new DLSet(elementVector);
		}
		return Empty;
	}

	public DLSet()
	{
	}

	public DLSet(Asn1Encodable element)
		: base(element)
	{
	}

	public DLSet(params Asn1Encodable[] elements)
		: base(elements, doSort: false)
	{
	}

	public DLSet(Asn1EncodableVector elementVector)
		: base(elementVector, doSort: false)
	{
	}

	internal DLSet(bool isSorted, Asn1Encodable[] elements)
		: base(isSorted, elements)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (3 == encoding)
		{
			return base.GetEncoding(encoding);
		}
		return new ConstructedDLEncoding(0, 17, Asn1OutputStream.GetContentsEncodings(2, m_elements));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (3 == encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new ConstructedDLEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(2, m_elements));
	}
}
