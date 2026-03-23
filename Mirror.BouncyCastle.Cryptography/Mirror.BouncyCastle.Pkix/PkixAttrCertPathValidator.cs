using System;
using Mirror.BouncyCastle.X509;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Pkix;

public class PkixAttrCertPathValidator
{
	public virtual PkixCertPathValidatorResult Validate(PkixCertPath certPath, PkixParameters pkixParams)
	{
		X509V2AttributeCertificate attributeCert = ((pkixParams.GetTargetConstraintsAttrCert() as X509AttrCertStoreSelector) ?? throw new ArgumentException("TargetConstraints must be an instance of " + typeof(X509AttrCertStoreSelector).FullName, "pkixParams")).AttributeCert;
		PkixCertPath holderCertPath = Rfc3281CertPathUtilities.ProcessAttrCert1(attributeCert, pkixParams);
		PkixCertPathValidatorResult result = Rfc3281CertPathUtilities.ProcessAttrCert2(certPath, pkixParams);
		X509Certificate x509Certificate = certPath.Certificates[0];
		Rfc3281CertPathUtilities.ProcessAttrCert3(x509Certificate, pkixParams);
		Rfc3281CertPathUtilities.ProcessAttrCert4(x509Certificate, pkixParams);
		Rfc3281CertPathUtilities.ProcessAttrCert5(attributeCert, pkixParams);
		Rfc3281CertPathUtilities.ProcessAttrCert7(attributeCert, certPath, holderCertPath, pkixParams);
		Rfc3281CertPathUtilities.AdditionalChecks(attributeCert, pkixParams);
		DateTime validCertDateFromValidityModel;
		try
		{
			validCertDateFromValidityModel = PkixCertPathValidatorUtilities.GetValidCertDateFromValidityModel(pkixParams, null, -1);
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Could not get validity date from attribute certificate.", innerException);
		}
		Rfc3281CertPathUtilities.CheckCrls(attributeCert, pkixParams, x509Certificate, validCertDateFromValidityModel, certPath.Certificates);
		return result;
	}
}
