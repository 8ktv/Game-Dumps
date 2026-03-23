using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class KemBMParameter : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_kdf;

	private readonly DerInteger m_len;

	private readonly AlgorithmIdentifier m_mac;

	public virtual AlgorithmIdentifier Kdf => m_kdf;

	public virtual DerInteger Len => m_len;

	public virtual AlgorithmIdentifier Mac => m_mac;

	public static KemBMParameter GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is KemBMParameter result)
		{
			return result;
		}
		return new KemBMParameter(Asn1Sequence.GetInstance(obj));
	}

	public static KemBMParameter GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new KemBMParameter(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private KemBMParameter(Asn1Sequence seq)
	{
		if (seq.Count != 3)
		{
			throw new ArgumentException("sequence size should 3", "seq");
		}
		m_kdf = AlgorithmIdentifier.GetInstance(seq[0]);
		m_len = DerInteger.GetInstance(seq[1]);
		m_mac = AlgorithmIdentifier.GetInstance(seq[2]);
	}

	public KemBMParameter(AlgorithmIdentifier kdf, DerInteger len, AlgorithmIdentifier mac)
	{
		m_kdf = kdf;
		m_len = len;
		m_mac = mac;
	}

	public KemBMParameter(AlgorithmIdentifier kdf, long len, AlgorithmIdentifier mac)
		: this(kdf, new DerInteger(len), mac)
	{
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_kdf, m_len, m_mac);
	}
}
