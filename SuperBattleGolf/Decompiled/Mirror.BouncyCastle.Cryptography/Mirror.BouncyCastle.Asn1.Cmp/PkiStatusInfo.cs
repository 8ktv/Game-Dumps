using System;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class PkiStatusInfo : Asn1Encodable
{
	private readonly DerInteger m_status;

	private readonly PkiFreeText m_statusString;

	private readonly DerBitString m_failInfo;

	public BigInteger Status => m_status.Value;

	public PkiFreeText StatusString => m_statusString;

	public DerBitString FailInfo => m_failInfo;

	public static PkiStatusInfo GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is PkiStatusInfo result)
		{
			return result;
		}
		return new PkiStatusInfo(Asn1Sequence.GetInstance(obj));
	}

	public static PkiStatusInfo GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return new PkiStatusInfo(Asn1Sequence.GetInstance(obj, isExplicit));
	}

	[Obsolete("Use 'GetInstance' instead")]
	public PkiStatusInfo(Asn1Sequence seq)
	{
		m_status = DerInteger.GetInstance(seq[0]);
		m_statusString = null;
		m_failInfo = null;
		if (seq.Count > 2)
		{
			m_statusString = PkiFreeText.GetInstance(seq[1]);
			m_failInfo = DerBitString.GetInstance(seq[2]);
		}
		else if (seq.Count > 1)
		{
			object obj = seq[1];
			if (obj is DerBitString)
			{
				m_failInfo = DerBitString.GetInstance(obj);
			}
			else
			{
				m_statusString = PkiFreeText.GetInstance(obj);
			}
		}
	}

	public PkiStatusInfo(int status)
	{
		m_status = new DerInteger(status);
		m_statusString = null;
		m_failInfo = null;
	}

	public PkiStatusInfo(int status, PkiFreeText statusString)
	{
		m_status = new DerInteger(status);
		m_statusString = statusString;
		m_failInfo = null;
	}

	public PkiStatusInfo(int status, PkiFreeText statusString, PkiFailureInfo failInfo)
	{
		m_status = new DerInteger(status);
		m_statusString = statusString;
		m_failInfo = failInfo;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_status);
		asn1EncodableVector.AddOptional(m_statusString, m_failInfo);
		return new DerSequence(asn1EncodableVector);
	}
}
