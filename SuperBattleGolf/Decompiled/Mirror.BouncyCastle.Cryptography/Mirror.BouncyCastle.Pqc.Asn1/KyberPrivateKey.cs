using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

public sealed class KyberPrivateKey : Asn1Encodable
{
	private int version;

	private byte[] s;

	private KyberPublicKey publicKey;

	private byte[] hpk;

	private byte[] nonce;

	public int Version => version;

	public KyberPublicKey PublicKey => publicKey;

	public static KyberPrivateKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KyberPrivateKey result)
		{
			return result;
		}
		return new KyberPrivateKey(Asn1Sequence.GetInstance(obj));
	}

	public static KyberPrivateKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public KyberPrivateKey(int version, byte[] s, byte[] hpk, byte[] nonce, KyberPublicKey publicKey)
	{
		this.version = version;
		this.s = s;
		this.publicKey = publicKey;
		this.hpk = hpk;
		this.nonce = nonce;
	}

	public KyberPrivateKey(int version, byte[] s, byte[] hpk, byte[] nonce)
		: this(version, s, hpk, nonce, null)
	{
	}

	private KyberPrivateKey(Asn1Sequence seq)
	{
		version = DerInteger.GetInstance(seq[0]).IntValueExact;
		if (version != 0)
		{
			throw new ArgumentException("unrecognized version");
		}
		s = Arrays.Clone(Asn1OctetString.GetInstance(seq[1]).GetOctets());
		int num = 1;
		if (seq.Count == 5)
		{
			num = 0;
			publicKey = KyberPublicKey.GetInstance(seq[2]);
		}
		hpk = Arrays.Clone(Asn1OctetString.GetInstance(seq[3 - num]).GetOctets());
		nonce = Arrays.Clone(Asn1OctetString.GetInstance(seq[4 - num]).GetOctets());
	}

	public byte[] GetS()
	{
		return Arrays.Clone(s);
	}

	public byte[] GetHpk()
	{
		return Arrays.Clone(hpk);
	}

	public byte[] GetNonce()
	{
		return Arrays.Clone(nonce);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(5);
		asn1EncodableVector.Add(new DerInteger(version));
		asn1EncodableVector.Add(new DerOctetString(s));
		if (publicKey != null)
		{
			asn1EncodableVector.Add(new KyberPublicKey(publicKey.T, publicKey.Rho));
		}
		asn1EncodableVector.Add(new DerOctetString(hpk));
		asn1EncodableVector.Add(new DerOctetString(nonce));
		return new DerSequence(asn1EncodableVector);
	}
}
