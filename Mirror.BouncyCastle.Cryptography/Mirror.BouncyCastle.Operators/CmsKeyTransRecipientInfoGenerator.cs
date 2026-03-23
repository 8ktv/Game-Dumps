using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Operators;

public class CmsKeyTransRecipientInfoGenerator : KeyTransRecipientInfoGenerator
{
	public CmsKeyTransRecipientInfoGenerator(X509Certificate recipCert, IKeyWrapper keyWrapper)
		: base(new IssuerAndSerialNumber(recipCert.CertificateStructure), keyWrapper)
	{
	}

	public CmsKeyTransRecipientInfoGenerator(IssuerAndSerialNumber issuerAndSerial, IKeyWrapper keyWrapper)
		: base(issuerAndSerial, keyWrapper)
	{
	}

	public CmsKeyTransRecipientInfoGenerator(byte[] subjectKeyID, IKeyWrapper keyWrapper)
		: base(subjectKeyID, keyWrapper)
	{
	}
}
