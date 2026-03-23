using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class CcmParameters : Asn1Encodable
{
	private const int DefaultIcvLen = 12;

	private readonly byte[] m_nonce;

	private readonly int m_icvLen;

	public int IcvLen => m_icvLen;

	public static CcmParameters GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is CcmParameters result)
		{
			return result;
		}
		return new CcmParameters(Asn1Sequence.GetInstance(obj));
	}

	public static CcmParameters GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new CcmParameters(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private CcmParameters(Asn1Sequence seq)
	{
		int count = seq.Count;
		if (count < 1 || count > 2)
		{
			throw new ArgumentException("Bad sequence size: " + count, "seq");
		}
		m_nonce = Asn1OctetString.GetInstance(seq[0]).GetOctets();
		if (count > 1)
		{
			m_icvLen = DerInteger.GetInstance(seq[1]).IntValueExact;
		}
		else
		{
			m_icvLen = 12;
		}
	}

	public CcmParameters(byte[] nonce, int icvLen)
	{
		m_nonce = Arrays.Clone(nonce);
		m_icvLen = icvLen;
	}

	public byte[] GetNonce()
	{
		return Arrays.Clone(m_nonce);
	}

	public override Asn1Object ToAsn1Object()
	{
		DerOctetString derOctetString = new DerOctetString(m_nonce);
		if (m_icvLen != 12)
		{
			return new DerSequence(derOctetString, new DerInteger(m_icvLen));
		}
		return new DerSequence(derOctetString);
	}
}
