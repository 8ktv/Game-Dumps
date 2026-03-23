using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

public class CmcePublicKey : Asn1Object
{
	private byte[] t;

	public byte[] T => Arrays.Clone(t);

	public static CmcePublicKey GetInstance(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (o is CmcePublicKey result)
		{
			return result;
		}
		return new CmcePublicKey(Asn1Sequence.GetInstance(o));
	}

	public static CmcePublicKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public CmcePublicKey(byte[] t)
	{
		this.t = t;
	}

	[Obsolete("Use 'GetInstance' instead")]
	public CmcePublicKey(Asn1Sequence seq)
	{
		t = Arrays.Clone(Asn1OctetString.GetInstance(seq[0]).GetOctets());
	}

	public Asn1Object ToAsn1Primitive()
	{
		return new DerSequence(new DerOctetString(t));
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return ToAsn1Primitive().GetEncoding(encoding);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return ToAsn1Primitive().GetEncodingImplicit(encoding, tagClass, tagNo);
	}

	internal override DerEncoding GetEncodingDer()
	{
		return ToAsn1Primitive().GetEncodingDer();
	}

	internal override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
	{
		return ToAsn1Primitive().GetEncodingDerImplicit(tagClass, tagNo);
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		return ToAsn1Primitive().CallAsn1Equals(asn1Object);
	}

	protected override int Asn1GetHashCode()
	{
		return ToAsn1Primitive().CallAsn1GetHashCode();
	}
}
