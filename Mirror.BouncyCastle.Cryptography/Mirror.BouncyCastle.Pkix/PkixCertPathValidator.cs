using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCertPathValidator
{
	public virtual PkixCertPathValidatorResult Validate(PkixCertPath certPath, PkixParameters paramsPkix)
	{
		if (paramsPkix.GetTrustAnchors() == null)
		{
			throw new ArgumentException("trustAnchors is null, this is not allowed for certification path validation.", "paramsPkix");
		}
		IList<X509Certificate> certificates = certPath.Certificates;
		int count = certificates.Count;
		if (count == 0)
		{
			throw new PkixCertPathValidatorException("Certification path is empty.", null, 0);
		}
		ISet<string> initialPolicies = paramsPkix.GetInitialPolicies();
		TrustAnchor trustAnchor;
		try
		{
			trustAnchor = PkixCertPathValidatorUtilities.FindTrustAnchor(certificates[certificates.Count - 1], paramsPkix.GetTrustAnchors());
			if (trustAnchor == null)
			{
				throw new PkixCertPathValidatorException("Trust anchor for certification path not found.", null, -1);
			}
			CheckCertificate(trustAnchor.TrustedCert);
		}
		catch (Exception ex)
		{
			throw new PkixCertPathValidatorException(ex.Message, ex.InnerException, certificates.Count - 1);
		}
		int num = 0;
		List<PkixPolicyNode>[] array = new List<PkixPolicyNode>[count + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new List<PkixPolicyNode>();
		}
		HashSet<string> hashSet = new HashSet<string>();
		hashSet.Add(Rfc3280CertPathUtilities.ANY_POLICY);
		PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), 0, hashSet, null, new HashSet<PolicyQualifierInfo>(), Rfc3280CertPathUtilities.ANY_POLICY, critical: false);
		array[0].Add(pkixPolicyNode);
		PkixNameConstraintValidator nameConstraintValidator = new PkixNameConstraintValidator();
		HashSet<string> acceptablePolicies = new HashSet<string>();
		int explicitPolicy = ((!paramsPkix.IsExplicitPolicyRequired) ? (count + 1) : 0);
		int inhibitAnyPolicy = ((!paramsPkix.IsAnyPolicyInhibited) ? (count + 1) : 0);
		int policyMapping = ((!paramsPkix.IsPolicyMappingInhibited) ? (count + 1) : 0);
		X509Certificate x509Certificate = trustAnchor.TrustedCert;
		X509Name workingIssuerName;
		AsymmetricKeyParameter asymmetricKeyParameter;
		try
		{
			if (x509Certificate != null)
			{
				workingIssuerName = x509Certificate.SubjectDN;
				asymmetricKeyParameter = x509Certificate.GetPublicKey();
			}
			else
			{
				workingIssuerName = new X509Name(trustAnchor.CAName);
				asymmetricKeyParameter = trustAnchor.CAPublicKey;
			}
		}
		catch (ArgumentException innerException)
		{
			throw new PkixCertPathValidatorException("Subject of trust anchor could not be (re)encoded.", innerException, -1);
		}
		try
		{
			PkixCertPathValidatorUtilities.GetAlgorithmIdentifier(asymmetricKeyParameter);
		}
		catch (PkixCertPathValidatorException innerException2)
		{
			throw new PkixCertPathValidatorException("Algorithm identifier of public key of trust anchor could not be read.", innerException2, -1);
		}
		int maxPathLength = count;
		ISelector<X509Certificate> targetConstraintsCert = paramsPkix.GetTargetConstraintsCert();
		if (targetConstraintsCert != null && !targetConstraintsCert.Match(certificates[0]))
		{
			throw new PkixCertPathValidatorException("Target certificate in certification path does not match targetConstraints.", null, 0);
		}
		IList<PkixCertPathChecker> certPathCheckers = paramsPkix.GetCertPathCheckers();
		foreach (PkixCertPathChecker item in certPathCheckers)
		{
			item.Init(forward: false);
		}
		X509Certificate x509Certificate2 = null;
		IList<PkixPolicyNode>[] policyNodes;
		for (num = certificates.Count - 1; num >= 0; num--)
		{
			int num2 = count - num;
			x509Certificate2 = certificates[num];
			try
			{
				CheckCertificate(x509Certificate2);
			}
			catch (Exception ex2)
			{
				throw new PkixCertPathValidatorException(ex2.Message, ex2.InnerException, num);
			}
			Rfc3280CertPathUtilities.ProcessCertA(certPath, paramsPkix, num, asymmetricKeyParameter, workingIssuerName, x509Certificate);
			Rfc3280CertPathUtilities.ProcessCertBC(certPath, num, nameConstraintValidator);
			int index = num;
			PkixPolicyNode validPolicyTree = pkixPolicyNode;
			policyNodes = array;
			pkixPolicyNode = Rfc3280CertPathUtilities.ProcessCertD(certPath, index, acceptablePolicies, validPolicyTree, policyNodes, inhibitAnyPolicy);
			pkixPolicyNode = Rfc3280CertPathUtilities.ProcessCertE(certPath, num, pkixPolicyNode);
			Rfc3280CertPathUtilities.ProcessCertF(certPath, num, pkixPolicyNode, explicitPolicy);
			if (num2 != count)
			{
				if (x509Certificate2 != null && x509Certificate2.Version == 1)
				{
					if (num2 != 1 || !x509Certificate2.Equals(trustAnchor.TrustedCert))
					{
						throw new PkixCertPathValidatorException("Version 1 certificates can't be used as CA ones.", null, num);
					}
				}
				else
				{
					Rfc3280CertPathUtilities.PrepareNextCertA(certPath, num);
					int index2 = num;
					policyNodes = array;
					pkixPolicyNode = Rfc3280CertPathUtilities.PrepareCertB(certPath, index2, policyNodes, pkixPolicyNode, policyMapping);
					Rfc3280CertPathUtilities.PrepareNextCertG(certPath, num, nameConstraintValidator);
					explicitPolicy = Rfc3280CertPathUtilities.PrepareNextCertH1(certPath, num, explicitPolicy);
					policyMapping = Rfc3280CertPathUtilities.PrepareNextCertH2(certPath, num, policyMapping);
					inhibitAnyPolicy = Rfc3280CertPathUtilities.PrepareNextCertH3(certPath, num, inhibitAnyPolicy);
					explicitPolicy = Rfc3280CertPathUtilities.PrepareNextCertI1(certPath, num, explicitPolicy);
					policyMapping = Rfc3280CertPathUtilities.PrepareNextCertI2(certPath, num, policyMapping);
					inhibitAnyPolicy = Rfc3280CertPathUtilities.PrepareNextCertJ(certPath, num, inhibitAnyPolicy);
					Rfc3280CertPathUtilities.PrepareNextCertK(certPath, num);
					maxPathLength = Rfc3280CertPathUtilities.PrepareNextCertL(certPath, num, maxPathLength);
					maxPathLength = Rfc3280CertPathUtilities.PrepareNextCertM(certPath, num, maxPathLength);
					Rfc3280CertPathUtilities.PrepareNextCertN(certPath, num);
					ISet<string> criticalExtensionOids = x509Certificate2.GetCriticalExtensionOids();
					if (criticalExtensionOids != null)
					{
						criticalExtensionOids = new HashSet<string>(criticalExtensionOids);
						criticalExtensionOids.Remove(X509Extensions.KeyUsage.Id);
						criticalExtensionOids.Remove(X509Extensions.CertificatePolicies.Id);
						criticalExtensionOids.Remove(X509Extensions.PolicyMappings.Id);
						criticalExtensionOids.Remove(X509Extensions.InhibitAnyPolicy.Id);
						criticalExtensionOids.Remove(X509Extensions.IssuingDistributionPoint.Id);
						criticalExtensionOids.Remove(X509Extensions.DeltaCrlIndicator.Id);
						criticalExtensionOids.Remove(X509Extensions.PolicyConstraints.Id);
						criticalExtensionOids.Remove(X509Extensions.BasicConstraints.Id);
						criticalExtensionOids.Remove(X509Extensions.SubjectAlternativeName.Id);
						criticalExtensionOids.Remove(X509Extensions.NameConstraints.Id);
					}
					else
					{
						criticalExtensionOids = new HashSet<string>();
					}
					Rfc3280CertPathUtilities.PrepareNextCertO(certPath, num, criticalExtensionOids, certPathCheckers);
					x509Certificate = x509Certificate2;
					workingIssuerName = x509Certificate.SubjectDN;
					try
					{
						asymmetricKeyParameter = PkixCertPathValidatorUtilities.GetNextWorkingKey(certPath.Certificates, num);
					}
					catch (PkixCertPathValidatorException innerException3)
					{
						throw new PkixCertPathValidatorException("Next working key could not be retrieved.", innerException3, num);
					}
					PkixCertPathValidatorUtilities.GetAlgorithmIdentifier(asymmetricKeyParameter);
				}
			}
		}
		explicitPolicy = Rfc3280CertPathUtilities.WrapupCertA(explicitPolicy, x509Certificate2);
		explicitPolicy = Rfc3280CertPathUtilities.WrapupCertB(certPath, num + 1, explicitPolicy);
		ISet<string> criticalExtensionOids2 = x509Certificate2.GetCriticalExtensionOids();
		if (criticalExtensionOids2 != null)
		{
			criticalExtensionOids2 = new HashSet<string>(criticalExtensionOids2);
			criticalExtensionOids2.Remove(X509Extensions.KeyUsage.Id);
			criticalExtensionOids2.Remove(X509Extensions.CertificatePolicies.Id);
			criticalExtensionOids2.Remove(X509Extensions.PolicyMappings.Id);
			criticalExtensionOids2.Remove(X509Extensions.InhibitAnyPolicy.Id);
			criticalExtensionOids2.Remove(X509Extensions.IssuingDistributionPoint.Id);
			criticalExtensionOids2.Remove(X509Extensions.DeltaCrlIndicator.Id);
			criticalExtensionOids2.Remove(X509Extensions.PolicyConstraints.Id);
			criticalExtensionOids2.Remove(X509Extensions.BasicConstraints.Id);
			criticalExtensionOids2.Remove(X509Extensions.SubjectAlternativeName.Id);
			criticalExtensionOids2.Remove(X509Extensions.NameConstraints.Id);
			criticalExtensionOids2.Remove(X509Extensions.CrlDistributionPoints.Id);
			criticalExtensionOids2.Remove(X509Extensions.ExtendedKeyUsage.Id);
		}
		else
		{
			criticalExtensionOids2 = new HashSet<string>();
		}
		Rfc3280CertPathUtilities.WrapupCertF(certPath, num + 1, certPathCheckers, criticalExtensionOids2);
		int index3 = num + 1;
		policyNodes = array;
		PkixPolicyNode pkixPolicyNode2 = Rfc3280CertPathUtilities.WrapupCertG(certPath, paramsPkix, initialPolicies, index3, policyNodes, pkixPolicyNode, acceptablePolicies);
		if (explicitPolicy > 0 || pkixPolicyNode2 != null)
		{
			return new PkixCertPathValidatorResult(trustAnchor, pkixPolicyNode2, x509Certificate2.GetPublicKey());
		}
		throw new PkixCertPathValidatorException("Path processing failed on policy.", null, num);
	}

	internal static void CheckCertificate(X509Certificate cert)
	{
		Exception innerException = null;
		try
		{
			if (cert.TbsCertificate != null)
			{
				return;
			}
		}
		catch (Exception ex)
		{
			innerException = ex;
		}
		throw new Exception("unable to process TBSCertificate", innerException);
	}
}
