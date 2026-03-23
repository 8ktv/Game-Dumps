using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

public class CmcePrivateKey : Asn1Object
{
	private int version;

	private byte[] delta;

	private byte[] c;

	private byte[] g;

	private byte[] alpha;

	private byte[] s;

	private CmcePublicKey publicKey;

	public int Version => version;

	public byte[] Delta => Arrays.Clone(delta);

	public byte[] C => Arrays.Clone(c);

	public byte[] G => Arrays.Clone(g);

	public byte[] Alpha => Arrays.Clone(alpha);

	public byte[] S => Arrays.Clone(s);

	public CmcePublicKey PublicKey => publicKey;

	public static CmcePrivateKey GetInstance(object o)
	{
		if (o == null)
		{
			return null;
		}
		if (o is CmcePrivateKey result)
		{
			return result;
		}
		return new CmcePrivateKey(Asn1Sequence.GetInstance(o));
	}

	public static CmcePrivateKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public CmcePrivateKey(int version, byte[] delta, byte[] c, byte[] g, byte[] alpha, byte[] s, CmcePublicKey pubKey = null)
	{
		if (version != 0)
		{
			throw new Exception("unrecognized version");
		}
		this.version = version;
		this.delta = Arrays.Clone(delta);
		this.c = Arrays.Clone(c);
		this.g = Arrays.Clone(g);
		this.alpha = Arrays.Clone(alpha);
		this.s = Arrays.Clone(s);
		publicKey = pubKey;
	}

	private CmcePrivateKey(Asn1Sequence seq)
	{
		version = DerInteger.GetInstance(seq[0]).IntValueExact;
		if (version != 0)
		{
			throw new Exception("unrecognized version");
		}
		delta = Arrays.Clone(Asn1OctetString.GetInstance(seq[1]).GetOctets());
		c = Arrays.Clone(Asn1OctetString.GetInstance(seq[2]).GetOctets());
		g = Arrays.Clone(Asn1OctetString.GetInstance(seq[3]).GetOctets());
		alpha = Arrays.Clone(Asn1OctetString.GetInstance(seq[4]).GetOctets());
		s = Arrays.Clone(Asn1OctetString.GetInstance(seq[5]).GetOctets());
		if (seq.Count == 7)
		{
			publicKey = CmcePublicKey.GetInstance(seq[6]);
		}
	}

	public Asn1Object ToAsn1Primitive()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(7);
		asn1EncodableVector.Add(new DerInteger(version));
		asn1EncodableVector.Add(new DerOctetString(delta));
		asn1EncodableVector.Add(new DerOctetString(c));
		asn1EncodableVector.Add(new DerOctetString(g));
		asn1EncodableVector.Add(new DerOctetString(alpha));
		asn1EncodableVector.Add(new DerOctetString(s));
		if (publicKey != null)
		{
			asn1EncodableVector.Add(new CmcePublicKey(publicKey.T));
		}
		return new DerSequence(asn1EncodableVector);
	}

	internal override IAsn1Encoding GetEncoding(int encoding)
	{
		return ToAsn1Primitive().GetEncoding(encoding);
	}

	internal override IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo)
	{
		return ToAsn1Primitive().GetEncodingImplicit(encoding, tagClass, tagNo);
	}

	internal sealed override DerEncoding GetEncodingDer()
	{
		return ToAsn1Primitive().GetEncodingDer();
	}

	internal sealed override DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo)
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
