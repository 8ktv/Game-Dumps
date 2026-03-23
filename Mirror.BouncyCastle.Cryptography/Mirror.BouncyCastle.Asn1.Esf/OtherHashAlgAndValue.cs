using System;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Asn1.Esf;

public class OtherHashAlgAndValue : Asn1Encodable
{
	private readonly AlgorithmIdentifier m_hashAlgorithm;

	private readonly Asn1OctetString m_hashValue;

	public AlgorithmIdentifier HashAlgorithm => m_hashAlgorithm;

	public static OtherHashAlgAndValue GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is OtherHashAlgAndValue result)
		{
			return result;
		}
		return new OtherHashAlgAndValue(Asn1Sequence.GetInstance(obj));
	}

	public static OtherHashAlgAndValue GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new OtherHashAlgAndValue(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private OtherHashAlgAndValue(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count != 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_hashAlgorithm = AlgorithmIdentifier.GetInstance(seq[0]);
		m_hashValue = Asn1OctetString.GetInstance(seq[1]);
	}

	public OtherHashAlgAndValue(AlgorithmIdentifier hashAlgorithm, byte[] hashValue)
	{
		if (hashAlgorithm == null)
		{
			throw new ArgumentNullException("hashAlgorithm");
		}
		if (hashValue == null)
		{
			throw new ArgumentNullException("hashValue");
		}
		m_hashAlgorithm = hashAlgorithm;
		m_hashValue = new DerOctetString(hashValue);
	}

	public OtherHashAlgAndValue(AlgorithmIdentifier hashAlgorithm, Asn1OctetString hashValue)
	{
		m_hashAlgorithm = hashAlgorithm ?? throw new ArgumentNullException("hashAlgorithm");
		m_hashValue = hashValue ?? throw new ArgumentNullException("hashValue");
	}

	public byte[] GetHashValue()
	{
		return m_hashValue.GetOctets();
	}

	public override Asn1Object ToAsn1Object()
	{
		return new DerSequence(m_hashAlgorithm, m_hashValue);
	}
}
