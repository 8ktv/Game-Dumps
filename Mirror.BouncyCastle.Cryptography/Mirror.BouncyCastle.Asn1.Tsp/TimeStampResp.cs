using Mirror.BouncyCastle.Asn1.Cmp;
using Mirror.BouncyCastle.Asn1.Cms;

namespace Mirror.BouncyCastle.Asn1.Tsp;

public class TimeStampResp : Asn1Encodable
{
	private readonly PkiStatusInfo m_pkiStatusInfo;

	private readonly ContentInfo m_timeStampToken;

	public PkiStatusInfo Status => m_pkiStatusInfo;

	public ContentInfo TimeStampToken => m_timeStampToken;

	public static TimeStampResp GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TimeStampResp result)
		{
			return result;
		}
		return new TimeStampResp(Asn1Sequence.GetInstance(obj));
	}

	public static TimeStampResp GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new TimeStampResp(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private TimeStampResp(Asn1Sequence seq)
	{
		m_pkiStatusInfo = PkiStatusInfo.GetInstance(seq[0]);
		if (seq.Count > 1)
		{
			m_timeStampToken = ContentInfo.GetInstance(seq[1]);
		}
	}

	public TimeStampResp(PkiStatusInfo pkiStatusInfo, ContentInfo timeStampToken)
	{
		m_pkiStatusInfo = pkiStatusInfo;
		m_timeStampToken = timeStampToken;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(2);
		asn1EncodableVector.Add(m_pkiStatusInfo);
		asn1EncodableVector.AddOptional(m_timeStampToken);
		return new DerSequence(asn1EncodableVector);
	}
}
