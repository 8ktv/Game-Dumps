using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Pkix;

public class PkixAttrCertPathBuilder
{
	private Exception certPathException;

	public virtual PkixCertPathBuilderResult Build(PkixBuilderParameters pkixParams)
	{
		if (!(pkixParams.GetTargetConstraintsAttrCert() is X509AttrCertStoreSelector attrCertSelector))
		{
			throw new PkixCertPathBuilderException("TargetConstraints must be an instance of " + typeof(X509AttrCertStoreSelector).FullName + " for " + typeof(PkixAttrCertPathBuilder).FullName + " class.");
		}
		HashSet<X509V2AttributeCertificate> hashSet;
		try
		{
			hashSet = FindAttributeCertificates(attrCertSelector, pkixParams.GetStoresAttrCert());
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathBuilderException("Error finding target attribute certificate.", innerException);
		}
		if (hashSet.Count == 0)
		{
			throw new PkixCertPathBuilderException("No attribute certificate found matching targetConstraints.");
		}
		PkixCertPathBuilderResult pkixCertPathBuilderResult = null;
		foreach (X509V2AttributeCertificate item in hashSet)
		{
			X509CertStoreSelector x509CertStoreSelector = new X509CertStoreSelector();
			X509Name[] principals = item.Issuer.GetPrincipals();
			HashSet<X509Certificate> hashSet2 = new HashSet<X509Certificate>();
			for (int i = 0; i < principals.Length; i++)
			{
				try
				{
					x509CertStoreSelector.Subject = principals[i];
					CollectionUtilities.CollectMatches(hashSet2, x509CertStoreSelector, pkixParams.GetStoresCert());
				}
				catch (Exception innerException2)
				{
					throw new PkixCertPathBuilderException("Public key certificate for attribute certificate cannot be searched.", innerException2);
				}
			}
			if (hashSet2.Count < 1)
			{
				throw new PkixCertPathBuilderException("Public key certificate for attribute certificate cannot be found.");
			}
			List<X509Certificate> tbvPath = new List<X509Certificate>();
			foreach (X509Certificate item2 in hashSet2)
			{
				pkixCertPathBuilderResult = Build(item, item2, pkixParams, tbvPath);
				if (pkixCertPathBuilderResult != null)
				{
					break;
				}
			}
			if (pkixCertPathBuilderResult != null)
			{
				break;
			}
		}
		if (pkixCertPathBuilderResult == null && certPathException != null)
		{
			throw new PkixCertPathBuilderException("Possible certificate chain could not be validated.", certPathException);
		}
		if (pkixCertPathBuilderResult == null && certPathException == null)
		{
			throw new PkixCertPathBuilderException("Unable to find certificate chain.");
		}
		return pkixCertPathBuilderResult;
	}

	private PkixCertPathBuilderResult Build(X509V2AttributeCertificate attrCert, X509Certificate tbvCert, PkixBuilderParameters pkixParams, IList<X509Certificate> tbvPath)
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
		PkixAttrCertPathValidator pkixAttrCertPathValidator = new PkixAttrCertPathValidator();
		try
		{
			if (PkixCertPathValidatorUtilities.IsIssuerTrustAnchor(tbvCert, pkixParams.GetTrustAnchors()))
			{
				PkixCertPath certPath = new PkixCertPath(tbvPath);
				PkixCertPathValidatorResult pkixCertPathValidatorResult;
				try
				{
					pkixCertPathValidatorResult = pkixAttrCertPathValidator.Validate(certPath, pkixParams);
				}
				catch (Exception innerException)
				{
					throw new Exception("Certification path could not be validated.", innerException);
				}
				return new PkixCertPathBuilderResult(certPath, pkixCertPathValidatorResult.TrustAnchor, pkixCertPathValidatorResult.PolicyTree, pkixCertPathValidatorResult.SubjectPublicKey);
			}
			try
			{
				PkixCertPathValidatorUtilities.AddAdditionalStoresFromAltNames(tbvCert, pkixParams);
			}
			catch (CertificateParsingException innerException2)
			{
				throw new Exception("No additional X.509 stores can be added from certificate locations.", innerException2);
			}
			HashSet<X509Certificate> hashSet;
			try
			{
				hashSet = PkixCertPathValidatorUtilities.FindIssuerCerts(tbvCert, pkixParams);
			}
			catch (Exception innerException3)
			{
				throw new Exception("Cannot find issuer certificate for certificate in certification path.", innerException3);
			}
			if (hashSet.Count < 1)
			{
				throw new Exception("No issuer certificate for certificate in certification path found.");
			}
			foreach (X509Certificate item in hashSet)
			{
				if (!PkixCertPathValidatorUtilities.IsSelfIssued(item))
				{
					pkixCertPathBuilderResult = Build(attrCert, item, pkixParams, tbvPath);
					if (pkixCertPathBuilderResult != null)
					{
						break;
					}
				}
			}
		}
		catch (Exception innerException4)
		{
			certPathException = new Exception("No valid certification path could be build.", innerException4);
		}
		if (pkixCertPathBuilderResult == null)
		{
			tbvPath.Remove(tbvCert);
		}
		return pkixCertPathBuilderResult;
	}

	internal static HashSet<X509V2AttributeCertificate> FindAttributeCertificates(ISelector<X509V2AttributeCertificate> attrCertSelector, IList<IStore<X509V2AttributeCertificate>> attrCertStores)
	{
		HashSet<X509V2AttributeCertificate> hashSet = new HashSet<X509V2AttributeCertificate>();
		foreach (IStore<X509V2AttributeCertificate> attrCertStore in attrCertStores)
		{
			try
			{
				hashSet.UnionWith(attrCertStore.EnumerateMatches(attrCertSelector));
			}
			catch (Exception innerException)
			{
				throw new Exception("Problem while picking certificates from X.509 store.", innerException);
			}
		}
		return hashSet;
	}
}
