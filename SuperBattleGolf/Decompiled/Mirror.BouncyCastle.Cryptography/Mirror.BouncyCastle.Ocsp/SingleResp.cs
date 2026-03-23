using System;
using Mirror.BouncyCastle.Asn1.Ocsp;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Ocsp;

public class SingleResp : X509ExtensionBase
{
	internal readonly SingleResponse resp;

	public DateTime ThisUpdate => resp.ThisUpdate.ToDateTime();

	public DateTime? NextUpdate => resp.NextUpdate?.ToDateTime();

	public X509Extensions SingleExtensions => resp.SingleExtensions;

	public SingleResp(SingleResponse resp)
	{
		this.resp = resp;
	}

	public CertificateID GetCertID()
	{
		return new CertificateID(resp.CertId);
	}

	public object GetCertStatus()
	{
		CertStatus certStatus = resp.CertStatus;
		if (certStatus.TagNo == 0)
		{
			return null;
		}
		if (certStatus.TagNo == 1)
		{
			return new RevokedStatus(RevokedInfo.GetInstance(certStatus.Status));
		}
		return new UnknownStatus();
	}

	protected override X509Extensions GetX509Extensions()
	{
		return SingleExtensions;
	}
}
