namespace Mirror.BouncyCastle.Asn1;

public class DerTaggedObject : Asn1TaggedObject
{
	public DerTaggedObject(int tagNo, Asn1Encodable obj)
		: base(isExplicit: true, tagNo, obj)
	{
	}

	public DerTaggedObject(int tagClass, int tagNo, Asn1Encodable obj)
		: base(isExplicit: true, tagClass, tagNo, obj)
	{
	}

	public DerTaggedObject(bool isExplicit, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagNo, obj)
	{
	}

	public DerTaggedObject(bool isExplicit, int tagClass, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagClass, tagNo, obj)
	{
	}

	internal DerTaggedObject(int explicitness, int tagClass, int tagNo, Asn1Encodable obj)
		: base(explicitness, tagClass, tagNo, obj)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		encoding = 3;
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, base.TagClass, base.TagNo);
		}
		return new TaggedDLEncoding(base.TagClass, base.TagNo, asn1Object.GetEncoding(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		encoding = 3;
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new TaggedDLEncoding(tagClass, tagNo, asn1Object.GetEncoding(encoding));
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingDerImplicit(base.TagClass, base.TagNo);
		}
		return new TaggedDerEncoding(base.TagClass, base.TagNo, asn1Object.GetEncodingDer());
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingDerImplicit(tagClass, tagNo);
		}
		return new TaggedDerEncoding(tagClass, tagNo, asn1Object.GetEncodingDer());
	}

	internal override Asn1Sequence RebuildConstructed(Asn1Object asn1Object)
	{
		return new DerSequence(asn1Object);
	}

	internal override Asn1TaggedObject ReplaceTag(int tagClass, int tagNo)
	{
		return new DerTaggedObject(m_explicitness, tagClass, tagNo, m_object);
	}
}
