using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public abstract class PkixAttrCertChecker
{
	public abstract ISet<DerObjectIdentifier> GetSupportedExtensions();

	public abstract void Check(X509V2AttributeCertificate attrCert, PkixCertPath certPath, PkixCertPath holderCertPath, ICollection<string> unresolvedCritExts);

	public abstract PkixAttrCertChecker Clone();
}
