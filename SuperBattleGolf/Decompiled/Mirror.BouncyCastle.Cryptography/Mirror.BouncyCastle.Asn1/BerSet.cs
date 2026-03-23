namespace Mirror.BouncyCastle.Asn1;

public class BerSet : DLSet
{
	public new static readonly BerSet Empty = new BerSet();

	public new static BerSet FromVector(Asn1EncodableVector elementVector)
	{
		if (elementVector.Count >= 1)
		{
			return new BerSet(elementVector);
		}
		return Empty;
	}

	public BerSet()
	{
	}

	public BerSet(Asn1Encodable element)
		: base(element)
	{
	}

	public BerSet(params Asn1Encodable[] elements)
		: base(elements)
	{
	}

	public BerSet(Asn1EncodableVector elementVector)
		: base(elementVector)
	{
	}

	internal BerSet(bool isSorted, Asn1Encodable[] elements)
		: base(isSorted, elements)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (1 != encoding)
		{
			return base.GetEncoding(encoding);
		}
		return new ConstructedILEncoding(0, 17, Asn1OutputStream.GetContentsEncodings(encoding, m_elements));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (1 != encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new ConstructedILEncoding(tagClass, tagNo, Asn1OutputStream.GetContentsEncodings(encoding, m_elements));
	}
}
