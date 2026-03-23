namespace Mirror.BouncyCastle.Asn1;

public class DLTaggedObject : DerTaggedObject
{
	public DLTaggedObject(int tagNo, Asn1Encodable obj)
		: base(tagNo, obj)
	{
	}

	public DLTaggedObject(int tagClass, int tagNo, Asn1Encodable obj)
		: base(tagClass, tagNo, obj)
	{
	}

	public DLTaggedObject(bool isExplicit, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagNo, obj)
	{
	}

	public DLTaggedObject(bool isExplicit, int tagClass, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagClass, tagNo, obj)
	{
	}

	internal DLTaggedObject(int explicitness, int tagClass, int tagNo, Asn1Encodable obj)
		: base(explicitness, tagClass, tagNo, obj)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (3 == encoding)
		{
			return base.GetEncoding(encoding);
		}
		encoding = 2;
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, base.TagClass, base.TagNo);
		}
		return new TaggedDLEncoding(base.TagClass, base.TagNo, asn1Object.GetEncoding(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (3 == encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		encoding = 2;
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new TaggedDLEncoding(tagClass, tagNo, asn1Object.GetEncoding(encoding));
	}

	internal override Asn1Sequence RebuildConstructed(Asn1Object asn1Object)
	{
		return new DLSequence(asn1Object);
	}

	internal override Asn1TaggedObject ReplaceTag(int tagClass, int tagNo)
	{
		return new DLTaggedObject(m_explicitness, tagClass, tagNo, m_object);
	}
}
