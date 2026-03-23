using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class RevokedStatus : CertificateStatus
{
	private readonly RevokedInfo m_revokedInfo;

	public DateTime RevocationTime => m_revokedInfo.RevocationTime.ToDateTime();

	public bool HasRevocationReason => m_revokedInfo.RevocationReason != null;

	public int RevocationReason
	{
		get
		{
			if (m_revokedInfo.RevocationReason == null)
			{
				throw new InvalidOperationException("attempt to get a reason where none is available");
			}
			return m_revokedInfo.RevocationReason.IntValueExact;
		}
	}

	public RevokedStatus(RevokedInfo revokedInfo)
	{
		m_revokedInfo = revokedInfo;
	}

	public RevokedStatus(DateTime revocationDate)
	{
		DerGeneralizedTime revocationTime = Rfc5280Asn1Utilities.CreateGeneralizedTime(revocationDate);
		m_revokedInfo = new RevokedInfo(revocationTime);
	}

	public RevokedStatus(DateTime revocationDate, int reason)
	{
		DerGeneralizedTime revocationTime = Rfc5280Asn1Utilities.CreateGeneralizedTime(revocationDate);
		m_revokedInfo = new RevokedInfo(revocationTime, new CrlReason(reason));
	}
}
