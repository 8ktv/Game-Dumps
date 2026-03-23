namespace Mirror.BouncyCastle.Asn1;

public class DLBitString : DerBitString
{
	public DLBitString(byte data, int padBits)
		: base(data, padBits)
	{
	}

	public DLBitString(byte[] data)
		: this(data, 0)
	{
	}

	public DLBitString(byte[] data, int padBits)
		: base(data, padBits)
	{
	}

	public DLBitString(int namedBits)
		: base(namedBits)
	{
	}

	public DLBitString(Asn1Encodable obj)
		: this(obj.GetDerEncoded(), 0)
	{
	}

	internal DLBitString(byte[] contents, bool check)
		: base(contents, check)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (3 == encoding)
		{
			return base.GetEncoding(encoding);
		}
		return new PrimitiveEncoding(0, 3, contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (3 == encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new PrimitiveEncoding(tagClass, tagNo, contents);
	}
}
