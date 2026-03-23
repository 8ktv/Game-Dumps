using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class OriginatorInformation
{
	private readonly OriginatorInfo originatorInfo;

	public OriginatorInformation(OriginatorInfo originatorInfo)
	{
		this.originatorInfo = originatorInfo;
	}

	public virtual IStore<X509Certificate> GetCertificates()
	{
		return CmsSignedHelper.GetCertificates(originatorInfo.Certificates);
	}

	public virtual IStore<X509Crl> GetCrls()
	{
		return CmsSignedHelper.GetCrls(originatorInfo.Crls);
	}

	public virtual OriginatorInfo ToAsn1Structure()
	{
		return originatorInfo;
	}
}
