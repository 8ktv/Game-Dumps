using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class ErrorMsgContent : Asn1Encodable
{
	private readonly PkiStatusInfo m_pkiStatusInfo;

	private readonly DerInteger m_errorCode;

	private readonly PkiFreeText m_errorDetails;

	public virtual PkiStatusInfo PkiStatusInfo => m_pkiStatusInfo;

	public virtual DerInteger ErrorCode => m_errorCode;

	public virtual PkiFreeText ErrorDetails => m_errorDetails;

	public static ErrorMsgContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is ErrorMsgContent result)
		{
			return result;
		}
		return new ErrorMsgContent(Asn1Sequence.GetInstance(obj));
	}

	public static ErrorMsgContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new ErrorMsgContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	private ErrorMsgContent(Asn1Sequence seq)
	{
		m_pkiStatusInfo = PkiStatusInfo.GetInstance(seq[0]);
		for (int i = 1; i < seq.Count; i++)
		{
			Asn1Encodable asn1Encodable = seq[i];
			if (asn1Encodable is DerInteger)
			{
				m_errorCode = DerInteger.GetInstance(asn1Encodable);
			}
			else
			{
				m_errorDetails = PkiFreeText.GetInstance(asn1Encodable);
			}
		}
	}

	public ErrorMsgContent(PkiStatusInfo pkiStatusInfo)
		: this(pkiStatusInfo, null, null)
	{
	}

	public ErrorMsgContent(PkiStatusInfo pkiStatusInfo, DerInteger errorCode, PkiFreeText errorDetails)
	{
		m_pkiStatusInfo = pkiStatusInfo ?? throw new ArgumentNullException("pkiStatusInfo");
		m_errorCode = errorCode;
		m_errorDetails = errorDetails;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(3);
		asn1EncodableVector.Add(m_pkiStatusInfo);
		asn1EncodableVector.AddOptional(m_errorCode, m_errorDetails);
		return new DerSequence(asn1EncodableVector);
	}
}
