using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Asn1;

public sealed class SphincsPlusPublicKey : Asn1Encodable
{
	private readonly byte[] m_pkseed;

	private readonly byte[] m_pkroot;

	public static SphincsPlusPublicKey GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is SphincsPlusPublicKey result)
		{
			return result;
		}
		return new SphincsPlusPublicKey(Asn1Sequence.GetInstance(obj));
	}

	public static SphincsPlusPublicKey GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public SphincsPlusPublicKey(byte[] pkseed, byte[] pkroot)
	{
		m_pkseed = pkseed;
		m_pkroot = pkroot;
	}

	private SphincsPlusPublicKey(Asn1Sequence seq)
	{
		m_pkseed = Arrays.Clone(Asn1OctetString.GetInstance(seq[0]).GetOctets());
		m_pkroot = Arrays.Clone(Asn1OctetString.GetInstance(seq[1]).GetOctets());
	}

	public byte[] GetPkroot()
	{
		return Arrays.Clone(m_pkroot);
	}

	public byte[] GetPkseed()
	{
		return Arrays.Clone(m_pkseed);
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(new Asn1EncodableVector
		{
			new DerOctetString(m_pkseed),
			new DerOctetString(m_pkroot)
		});
	}
}
