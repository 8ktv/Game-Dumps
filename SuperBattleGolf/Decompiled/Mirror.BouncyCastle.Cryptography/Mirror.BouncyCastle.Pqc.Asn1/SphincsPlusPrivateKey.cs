using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

public sealed class SphincsPlusPrivateKey : Asn1Encodable
{
	private readonly int m_version;

	private readonly byte[] m_skseed;

	private readonly byte[] m_skprf;

	private readonly SphincsPlusPublicKey m_publicKey;

	public SphincsPlusPublicKey PublicKey => m_publicKey;

	public int Version => m_version;

	public static SphincsPlusPrivateKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SphincsPlusPrivateKey result)
		{
			return result;
		}
		return new SphincsPlusPrivateKey(Asn1Sequence.GetInstance(obj));
	}

	public static SphincsPlusPrivateKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public SphincsPlusPrivateKey(int version, byte[] skseed, byte[] skprf)
		: this(version, skseed, skprf, null)
	{
	}

	public SphincsPlusPrivateKey(int version, byte[] skseed, byte[] skprf, SphincsPlusPublicKey publicKey)
	{
		m_version = version;
		m_skseed = skseed;
		m_skprf = skprf;
		m_publicKey = publicKey;
	}

	private SphincsPlusPrivateKey(Asn1Sequence seq)
	{
		m_version = DerInteger.GetInstance(seq[0]).IntValueExact;
		if (m_version != 0)
		{
			throw new ArgumentException("unrecognized version");
		}
		m_skseed = Arrays.Clone(Asn1OctetString.GetInstance(seq[1]).GetOctets());
		m_skprf = Arrays.Clone(Asn1OctetString.GetInstance(seq[2]).GetOctets());
		if (seq.Count == 4)
		{
			m_publicKey = SphincsPlusPublicKey.GetInstance(seq[3]);
		}
	}

	public byte[] GetSkprf()
	{
		return Arrays.Clone(m_skprf);
	}

	public byte[] GetSkseed()
	{
		return Arrays.Clone(m_skseed);
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(4);
		asn1EncodableVector.Add(new DerInteger(m_version));
		asn1EncodableVector.Add(new DerOctetString(m_skseed));
		asn1EncodableVector.Add(new DerOctetString(m_skprf));
		if (m_publicKey != null)
		{
			asn1EncodableVector.Add(new SphincsPlusPublicKey(m_publicKey.GetPkseed(), m_publicKey.GetPkroot()));
		}
		return new DerSequence(asn1EncodableVector);
	}
}
