namespace Mirror.BouncyCastle.Asn1;

public class BerTaggedObject : DLTaggedObject
{
	public BerTaggedObject(int tagNo, Asn1Encodable obj)
		: base(isExplicit: true, tagNo, obj)
	{
	}

	public BerTaggedObject(int tagClass, int tagNo, Asn1Encodable obj)
		: base(isExplicit: true, tagClass, tagNo, obj)
	{
	}

	public BerTaggedObject(bool isExplicit, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagNo, obj)
	{
	}

	public BerTaggedObject(bool isExplicit, int tagClass, int tagNo, Asn1Encodable obj)
		: base(isExplicit, tagClass, tagNo, obj)
	{
	}

	internal BerTaggedObject(int explicitness, int tagClass, int tagNo, Asn1Encodable obj)
		: base(explicitness, tagClass, tagNo, obj)
	{
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		if (1 != encoding)
		{
			return base.GetEncoding(encoding);
		}
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, base.TagClass, base.TagNo);
		}
		return new TaggedILEncoding(base.TagClass, base.TagNo, asn1Object.GetEncoding(encoding));
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		if (1 != encoding)
		{
			return base.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		Asn1Object asn1Object = GetBaseObject().ToAsn1Object();
		if (!IsExplicit())
		{
			return asn1Object.GetEncodingImplicit(encoding, tagClass, tagNo);
		}
		return new TaggedILEncoding(tagClass, tagNo, asn1Object.GetEncoding(encoding));
	}

	internal override Asn1Sequence RebuildConstructed(Asn1Object asn1Object)
	{
		return new BerSequence(asn1Object);
	}

	internal override Asn1TaggedObject ReplaceTag(int tagClass, int tagNo)
	{
		return new BerTaggedObject(m_explicitness, tagClass, tagNo, m_object);
	}
}
