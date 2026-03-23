namespace Mirror.BouncyCastle.Asn1;

public class DerOctetString : Asn1OctetString
{
	public DerOctetString(byte[] contents)
		: base(contents)
	{
	}

	public DerOctetString(IAsn1Convertible obj)
		: this(obj.ToAsn1Object())
	{
	}

	public DerOctetString(Asn1Encodable obj)
		: base(obj.GetEncoded("DER"))
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return new PrimitiveEncoding(0, 4, contents);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return new PrimitiveEncoding(tagClass, tagNo, contents);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return new PrimitiveDerEncoding(0, 4, contents);
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return new PrimitiveDerEncoding(tagClass, tagNo, contents);
	}

	internal static void Encode(Asn1OutputStream asn1Out, byte[] buf, int off, int len)
	{
		asn1Out.WriteIdentifier(0, 4);
		asn1Out.WriteDL(len);
		asn1Out.Write(buf, off, len);
	}
}
