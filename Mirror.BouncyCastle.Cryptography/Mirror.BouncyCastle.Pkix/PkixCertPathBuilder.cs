using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCertPathBuilder
{
	private Exception certPathException;

	public virtual PkixCertPathBuilderResult Build(PkixBuilderParameters pkixParams)
	{
		ISelector<X509Certificate> targetConstraintsCert = pkixParams.GetTargetConstraintsCert();
		HashSet<X509Certificate> hashSet = new HashSet<X509Certificate>();
		try
		{
			CollectionUtilities.CollectMatches(hashSet, targetConstraintsCert, pkixParams.GetStoresCert());
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathBuilderException("Error finding target certificate.", innerException);
		}
		if (hashSet.Count < 1)
		{
			throw new PkixCertPathBuilderException("No certificate found matching targetConstraints.");
		}
		PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
		List<X509Certificate> tbvPath = new List<X509Certificate>();
		foreach (X509Certificate item in hashSet)
		{
			pkixCertPathBuilderResult = Build(item, pkixParams, tbvPath);
			if (pkixCertPathBuilderResult != null)
			{
				break;
			}
		}
		if (pkixCertPathBuilderResult == null && certPathException != null)
		{
			throw new PkixCertPathBuilderException(certPathException.Message, certPathException.InnerException);
		}
		if (pkixCertPathBuilderResult == null && certPathException == null)
		{
			throw new PkixCertPathBuilderException("Unable to find certificate chain.");
		}
		return pkixCertPathBuilderResult;
	}

	protected virtual PkixCertPathBuilderResult Build(X509Certificate tbvCert, PkixBuilderParameters pkixParams, IList<X509Certificate> tbvPath)
	{
		if (tbvPath.Contains(tbvCert))
		{
			return null;
		}
		if (pkixParams.GetExcludedCerts().Contains(tbvCert))
		{
			return null;
		}
		if (pkixParams.MaxPathLength != -1 && tbvPath.Count - 1 > pkixParams.MaxPathLength)
		{
			return null;
		}
		tbvPath.Add(tbvCert);
		PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
		PkixCertPathValidator pkixCertPathValidator = new PkixCertPathValidator();
		try
		{
			if (PkixCertPathValidatorUtilities.IsIssuerTrustAnchor(tbvCert, pkixParams.GetTrustAnchors()))
			{
				PkixCertPath certPath;
				try
				{
					certPath = new PkixCertPath(tbvPath);
				}
				catch (Exception innerException)
				{
					throw new Exception("Certification path could not be constructed from certificate list.", innerException);
				}
				PkixCertPathValidatorResult pkixCertPathValidatorResult;
				try
				{
					pkixCertPathValidatorResult = pkixCertPathValidator.Validate(certPath, pkixParams);
				}
				catch (Exception innerException2)
				{
					throw new Exception("Certification path could not be validated.", innerException2);
				}
				return new PkixCertPathBuilderResult(certPath, pkixCertPathValidatorResult.TrustAnchor, pkixCertPathValidatorResult.PolicyTree, pkixCertPathValidatorResult.SubjectPublicKey);
			}
			try
			{
				PkixCertPathValidatorUtilities.AddAdditionalStoresFromAltNames(tbvCert, pkixParams);
			}
			catch (CertificateParsingException innerException3)
			{
				throw new Exception("No additiontal X.509 stores can be added from certificate locations.", innerException3);
			}
			ISet<X509Certificate> set;
			try
			{
				set = PkixCertPathValidatorUtilities.FindIssuerCerts(tbvCert, pkixParams);
			}
			catch (Exception innerException4)
			{
				throw new Exception("Cannot find issuer certificate for certificate in certification path.", innerException4);
			}
			if (set.Count < 1)
			{
				throw new Exception("No issuer certificate for certificate in certification path found.");
			}
			foreach (X509Certificate item in set)
			{
				pkixCertPathBuilderResult = Build(item, pkixParams, tbvPath);
				if (pkixCertPathBuilderResult != null)
				{
					break;
				}
			}
		}
		catch (Exception ex)
		{
			certPathException = ex;
		}
		if (pkixCertPathBuilderResult == null)
		{
			tbvPath.Remove(tbvCert);
		}
		return pkixCertPathBuilderResult;
	}
}
