using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Pkix;

internal static class Rfc3280CertPathUtilities
{
	private static readonly PkixCrlUtilities CrlUtilities = new PkixCrlUtilities();

	internal static readonly string ANY_POLICY = "2.5.29.32.0";

	internal static readonly int KEY_CERT_SIGN = 5;

	internal static readonly int CRL_SIGN = 6;

	internal static readonly string[] CrlReasons = new string[11]
	{
		"unspecified", "keyCompromise", "cACompromise", "affiliationChanged", "superseded", "cessationOfOperation", "certificateHold", "unknown", "removeFromCRL", "privilegeWithdrawn",
		"aACompromise"
	};

	internal static void ProcessCrlB2(DistributionPoint dp, object cert, X509Crl crl)
	{
		IssuingDistributionPoint instance;
		try
		{
			instance = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.IssuingDistributionPoint));
		}
		catch (Exception innerException)
		{
			throw new Exception("0 Issuing distribution point extension could not be decoded.", innerException);
		}
		if (instance == null)
		{
			return;
		}
		if (instance.DistributionPoint != null)
		{
			DistributionPointName distributionPoint = IssuingDistributionPoint.GetInstance(instance).DistributionPoint;
			List<GeneralName> list = new List<GeneralName>();
			if (distributionPoint.Type == 0)
			{
				GeneralName[] names = GeneralNames.GetInstance(distributionPoint.Name).GetNames();
				for (int i = 0; i < names.Length; i++)
				{
					list.Add(names[i]);
				}
			}
			if (distributionPoint.Type == 1)
			{
				Asn1Sequence instance2 = Asn1Sequence.GetInstance(crl.IssuerDN.ToAsn1Object());
				Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(instance2.Count + 1);
				foreach (Asn1Encodable item in instance2)
				{
					asn1EncodableVector.Add(item);
				}
				asn1EncodableVector.Add(distributionPoint.Name);
				list.Add(new GeneralName(X509Name.GetInstance(new DerSequence(asn1EncodableVector))));
			}
			bool flag = false;
			if (dp.DistributionPointName != null)
			{
				distributionPoint = dp.DistributionPointName;
				GeneralName[] array = null;
				if (distributionPoint.Type == 0)
				{
					array = GeneralNames.GetInstance(distributionPoint.Name).GetNames();
				}
				if (distributionPoint.Type == 1)
				{
					if (dp.CrlIssuer != null)
					{
						array = dp.CrlIssuer.GetNames();
					}
					else
					{
						array = new GeneralName[1];
						try
						{
							array[0] = new GeneralName(PkixCertPathValidatorUtilities.GetIssuerPrincipal(cert));
						}
						catch (IOException innerException2)
						{
							throw new Exception("Could not read certificate issuer.", innerException2);
						}
					}
					for (int j = 0; j < array.Length; j++)
					{
						Asn1Sequence instance3 = Asn1Sequence.GetInstance(array[j].Name.ToAsn1Object());
						Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector(instance3.Count + 1);
						foreach (Asn1Encodable item2 in instance3)
						{
							asn1EncodableVector2.Add(item2);
						}
						asn1EncodableVector2.Add(distributionPoint.Name);
						array[j] = new GeneralName(X509Name.GetInstance(new DerSequence(asn1EncodableVector2)));
					}
				}
				if (array != null)
				{
					for (int k = 0; k < array.Length; k++)
					{
						if (list.Contains(array[k]))
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					throw new Exception("No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
				}
			}
			else
			{
				if (dp.CrlIssuer == null)
				{
					throw new Exception("Either the cRLIssuer or the distributionPoint field must be contained in DistributionPoint.");
				}
				GeneralName[] names2 = dp.CrlIssuer.GetNames();
				for (int l = 0; l < names2.Length; l++)
				{
					if (list.Contains(names2[l]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new Exception("No match for certificate CRL issuing distribution point name to cRLIssuer CRL distribution point.");
				}
			}
		}
		BasicConstraints instance4;
		try
		{
			instance4 = BasicConstraints.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue((IX509Extension)cert, X509Extensions.BasicConstraints));
		}
		catch (Exception innerException3)
		{
			throw new Exception("Basic constraints extension could not be decoded.", innerException3);
		}
		if (instance.OnlyContainsUserCerts && instance4 != null && instance4.IsCA())
		{
			throw new Exception("CA Cert CRL only contains user certificates.");
		}
		if (instance.OnlyContainsCACerts && (instance4 == null || !instance4.IsCA()))
		{
			throw new Exception("End CRL only contains CA certificates.");
		}
		if (instance.OnlyContainsAttributeCerts)
		{
			throw new Exception("onlyContainsAttributeCerts boolean is asserted.");
		}
	}

	internal static void ProcessCertBC(PkixCertPath certPath, int index, PkixNameConstraintValidator nameConstraintValidator)
	{
		IList<X509Certificate> certificates = certPath.Certificates;
		X509Certificate x509Certificate = certificates[index];
		int count = certificates.Count;
		int num = count - index;
		if (PkixCertPathValidatorUtilities.IsSelfIssued(x509Certificate) && num < count)
		{
			return;
		}
		X509Name subjectDN = x509Certificate.SubjectDN;
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(subjectDN.GetEncoded());
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Exception extracting subject name when checking subtrees.", innerException, index);
		}
		try
		{
			nameConstraintValidator.CheckPermittedDN(instance);
			nameConstraintValidator.CheckExcludedDN(instance);
		}
		catch (PkixNameConstraintValidatorException innerException2)
		{
			throw new PkixCertPathValidatorException("Subtree check for certificate subject failed.", innerException2, index);
		}
		GeneralNames instance2;
		try
		{
			instance2 = GeneralNames.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(x509Certificate, X509Extensions.SubjectAlternativeName));
		}
		catch (Exception innerException3)
		{
			throw new PkixCertPathValidatorException("Subject alternative name extension could not be decoded.", innerException3, index);
		}
		foreach (string value in X509Name.GetInstance(instance).GetValueList(X509Name.EmailAddress))
		{
			GeneralName name = new GeneralName(1, value);
			try
			{
				nameConstraintValidator.CheckPermittedName(name);
				nameConstraintValidator.CheckExcludedName(name);
			}
			catch (PkixNameConstraintValidatorException innerException4)
			{
				throw new PkixCertPathValidatorException("Subtree check for certificate subject alternative email failed.", innerException4, index);
			}
		}
		if (instance2 == null)
		{
			return;
		}
		GeneralName[] names;
		try
		{
			names = instance2.GetNames();
		}
		catch (Exception innerException5)
		{
			throw new PkixCertPathValidatorException("Subject alternative name contents could not be decoded.", innerException5, index);
		}
		GeneralName[] array = names;
		foreach (GeneralName name2 in array)
		{
			try
			{
				nameConstraintValidator.CheckPermittedName(name2);
				nameConstraintValidator.CheckExcludedName(name2);
			}
			catch (PkixNameConstraintValidatorException innerException6)
			{
				throw new PkixCertPathValidatorException("Subtree check for certificate subject alternative name failed.", innerException6, index);
			}
		}
	}

	internal static void PrepareNextCertA(PkixCertPath certPath, int index)
	{
		X509Certificate extensions = certPath.Certificates[index];
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.PolicyMappings));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Policy mappings extension could not be decoded.", innerException, index);
		}
		if (instance == null)
		{
			return;
		}
		Asn1Sequence asn1Sequence = instance;
		for (int i = 0; i < asn1Sequence.Count; i++)
		{
			DerObjectIdentifier instance3;
			DerObjectIdentifier instance4;
			try
			{
				Asn1Sequence instance2 = Asn1Sequence.GetInstance(asn1Sequence[i]);
				instance3 = DerObjectIdentifier.GetInstance(instance2[0]);
				instance4 = DerObjectIdentifier.GetInstance(instance2[1]);
			}
			catch (Exception innerException2)
			{
				throw new PkixCertPathValidatorException("Policy mappings extension contents could not be decoded.", innerException2, index);
			}
			if (ANY_POLICY.Equals(instance3.Id))
			{
				throw new PkixCertPathValidatorException("IssuerDomainPolicy is anyPolicy", null, index);
			}
			if (ANY_POLICY.Equals(instance4.Id))
			{
				throw new PkixCertPathValidatorException("SubjectDomainPolicy is anyPolicy,", null, index);
			}
		}
	}

	internal static PkixPolicyNode ProcessCertD(PkixCertPath certPath, int index, HashSet<string> acceptablePolicies, PkixPolicyNode validPolicyTree, IList<PkixPolicyNode>[] policyNodes, int inhibitAnyPolicy)
	{
		IList<X509Certificate> certificates = certPath.Certificates;
		X509Certificate x509Certificate = certificates[index];
		int count = certificates.Count;
		int num = count - index;
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(x509Certificate, X509Extensions.CertificatePolicies));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Could not read certificate policies extension from certificate.", innerException, index);
		}
		if (instance != null && validPolicyTree != null)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Asn1Encodable item in instance)
			{
				PolicyInformation instance2 = PolicyInformation.GetInstance(item.ToAsn1Object());
				DerObjectIdentifier policyIdentifier = instance2.PolicyIdentifier;
				hashSet.Add(policyIdentifier.Id);
				if (!ANY_POLICY.Equals(policyIdentifier.Id))
				{
					HashSet<PolicyQualifierInfo> qualifierSet;
					try
					{
						qualifierSet = PkixCertPathValidatorUtilities.GetQualifierSet(instance2.PolicyQualifiers);
					}
					catch (PkixCertPathValidatorException innerException2)
					{
						throw new PkixCertPathValidatorException("Policy qualifier info set could not be build.", innerException2, index);
					}
					if (!PkixCertPathValidatorUtilities.ProcessCertD1i(num, policyNodes, policyIdentifier, qualifierSet))
					{
						PkixCertPathValidatorUtilities.ProcessCertD1ii(num, policyNodes, policyIdentifier, qualifierSet);
					}
				}
			}
			if (acceptablePolicies.Count < 1 || acceptablePolicies.Contains(ANY_POLICY))
			{
				acceptablePolicies.Clear();
				acceptablePolicies.UnionWith(hashSet);
			}
			else
			{
				HashSet<string> hashSet2 = new HashSet<string>();
				foreach (string acceptablePolicy in acceptablePolicies)
				{
					if (hashSet.Contains(acceptablePolicy))
					{
						hashSet2.Add(acceptablePolicy);
					}
				}
				acceptablePolicies.Clear();
				acceptablePolicies.UnionWith(hashSet2);
			}
			if (inhibitAnyPolicy > 0 || (num < count && PkixCertPathValidatorUtilities.IsSelfIssued(x509Certificate)))
			{
				foreach (Asn1Encodable item2 in instance)
				{
					PolicyInformation instance3 = PolicyInformation.GetInstance(item2.ToAsn1Object());
					if (!ANY_POLICY.Equals(instance3.PolicyIdentifier.Id))
					{
						continue;
					}
					HashSet<PolicyQualifierInfo> qualifierSet2 = PkixCertPathValidatorUtilities.GetQualifierSet(instance3.PolicyQualifiers);
					foreach (PkixPolicyNode item3 in policyNodes[num - 1])
					{
						foreach (string expectedPolicy in item3.ExpectedPolicies)
						{
							bool flag = false;
							foreach (PkixPolicyNode child in item3.Children)
							{
								if (expectedPolicy.Equals(child.ValidPolicy))
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								HashSet<string> hashSet3 = new HashSet<string>();
								hashSet3.Add(expectedPolicy);
								PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), num, hashSet3, item3, qualifierSet2, expectedPolicy, critical: false);
								item3.AddChild(pkixPolicyNode);
								policyNodes[num].Add(pkixPolicyNode);
							}
						}
					}
					break;
				}
			}
			PkixPolicyNode pkixPolicyNode2 = validPolicyTree;
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				IList<PkixPolicyNode> list = policyNodes[num2];
				for (int i = 0; i < list.Count; i++)
				{
					PkixPolicyNode pkixPolicyNode3 = list[i];
					if (!pkixPolicyNode3.HasChildren)
					{
						pkixPolicyNode2 = PkixCertPathValidatorUtilities.RemovePolicyNode(pkixPolicyNode2, policyNodes, pkixPolicyNode3);
						if (pkixPolicyNode2 == null)
						{
							break;
						}
					}
				}
			}
			ISet<string> criticalExtensionOids = x509Certificate.GetCriticalExtensionOids();
			if (criticalExtensionOids != null)
			{
				bool isCritical = criticalExtensionOids.Contains(X509Extensions.CertificatePolicies.Id);
				foreach (PkixPolicyNode item4 in policyNodes[num])
				{
					item4.IsCritical = isCritical;
				}
			}
			return pkixPolicyNode2;
		}
		return null;
	}

	internal static void ProcessCrlB1(DistributionPoint dp, object cert, X509Crl crl)
	{
		Asn1Object extensionValue = PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.IssuingDistributionPoint);
		bool flag = false;
		if (extensionValue != null && IssuingDistributionPoint.GetInstance(extensionValue).IsIndirectCrl)
		{
			flag = true;
		}
		byte[] encoded = crl.IssuerDN.GetEncoded();
		bool flag2 = false;
		if (dp.CrlIssuer != null)
		{
			GeneralName[] names = dp.CrlIssuer.GetNames();
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i].TagNo != 4)
				{
					continue;
				}
				try
				{
					if (Arrays.AreEqual(names[i].Name.GetEncoded(), encoded))
					{
						flag2 = true;
					}
				}
				catch (IOException innerException)
				{
					throw new Exception("CRL issuer information from distribution point cannot be decoded.", innerException);
				}
			}
			if (flag2 && !flag)
			{
				throw new Exception("Distribution point contains cRLIssuer field but CRL is not indirect.");
			}
			if (!flag2)
			{
				throw new Exception("CRL issuer of CRL does not match CRL issuer of distribution point.");
			}
		}
		else if (crl.IssuerDN.Equivalent(PkixCertPathValidatorUtilities.GetIssuerPrincipal(cert), inOrder: true))
		{
			flag2 = true;
		}
		if (!flag2)
		{
			throw new Exception("Cannot find matching CRL issuer for certificate.");
		}
	}

	internal static ReasonsMask ProcessCrlD(X509Crl crl, DistributionPoint dp)
	{
		IssuingDistributionPoint instance;
		try
		{
			instance = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.IssuingDistributionPoint));
		}
		catch (Exception innerException)
		{
			throw new Exception("issuing distribution point extension could not be decoded.", innerException);
		}
		if (instance != null && instance.OnlySomeReasons != null && dp.Reasons != null)
		{
			return new ReasonsMask(dp.Reasons.IntValue).Intersect(new ReasonsMask(instance.OnlySomeReasons.IntValue));
		}
		if ((instance == null || instance.OnlySomeReasons == null) && dp.Reasons == null)
		{
			return ReasonsMask.AllReasons;
		}
		ReasonsMask reasonsMask = ((dp.Reasons != null) ? new ReasonsMask(dp.Reasons.IntValue) : ReasonsMask.AllReasons);
		ReasonsMask mask = ((instance != null) ? new ReasonsMask(instance.OnlySomeReasons.IntValue) : ReasonsMask.AllReasons);
		return reasonsMask.Intersect(mask);
	}

	internal static HashSet<AsymmetricKeyParameter> ProcessCrlF(X509Crl crl, object cert, X509Certificate defaultCRLSignCert, AsymmetricKeyParameter defaultCRLSignKey, PkixParameters paramsPKIX, IList<X509Certificate> certPathCerts)
	{
		X509CertStoreSelector x509CertStoreSelector = new X509CertStoreSelector();
		try
		{
			x509CertStoreSelector.Subject = crl.IssuerDN;
		}
		catch (IOException innerException)
		{
			throw new Exception("Subject criteria for certificate selector to find issuer certificate for CRL could not be set.", innerException);
		}
		HashSet<X509Certificate> hashSet = new HashSet<X509Certificate>();
		try
		{
			CollectionUtilities.CollectMatches(hashSet, x509CertStoreSelector, paramsPKIX.GetStoresCert());
		}
		catch (Exception innerException2)
		{
			throw new Exception("Issuer certificate for CRL cannot be searched.", innerException2);
		}
		hashSet.Add(defaultCRLSignCert);
		List<X509Certificate> list = new List<X509Certificate>();
		List<AsymmetricKeyParameter> list2 = new List<AsymmetricKeyParameter>();
		foreach (X509Certificate item in hashSet)
		{
			if (item.Equals(defaultCRLSignCert))
			{
				list.Add(item);
				list2.Add(defaultCRLSignKey);
				continue;
			}
			try
			{
				PkixCertPathBuilder pkixCertPathBuilder = new PkixCertPathBuilder();
				x509CertStoreSelector = new X509CertStoreSelector
				{
					Certificate = item
				};
				PkixBuilderParameters instance = PkixBuilderParameters.GetInstance(paramsPKIX);
				instance.SetTargetConstraintsCert(x509CertStoreSelector);
				if (certPathCerts.Contains(item))
				{
					instance.IsRevocationEnabled = false;
				}
				else
				{
					instance.IsRevocationEnabled = true;
				}
				IList<X509Certificate> certificates = pkixCertPathBuilder.Build(instance).CertPath.Certificates;
				list.Add(item);
				list2.Add(PkixCertPathValidatorUtilities.GetNextWorkingKey(certificates, 0));
			}
			catch (PkixCertPathBuilderException innerException3)
			{
				throw new Exception("CertPath for CRL signer failed to validate.", innerException3);
			}
			catch (PkixCertPathValidatorException innerException4)
			{
				throw new Exception("Public key of issuer certificate of CRL could not be retrieved.", innerException4);
			}
		}
		HashSet<AsymmetricKeyParameter> hashSet2 = new HashSet<AsymmetricKeyParameter>();
		Exception ex = null;
		for (int i = 0; i < list.Count; i++)
		{
			bool[] keyUsage = list[i].GetKeyUsage();
			if (keyUsage != null && (keyUsage.Length < 7 || !keyUsage[CRL_SIGN]))
			{
				ex = new Exception("Issuer certificate key usage extension does not permit CRL signing.");
			}
			else
			{
				hashSet2.Add(list2[i]);
			}
		}
		if (hashSet2.Count == 0 && ex == null)
		{
			throw new Exception("Cannot find a valid issuer certificate.");
		}
		if (hashSet2.Count == 0 && ex != null)
		{
			throw ex;
		}
		return hashSet2;
	}

	internal static AsymmetricKeyParameter ProcessCrlG(X509Crl crl, HashSet<AsymmetricKeyParameter> keys)
	{
		Exception innerException = null;
		foreach (AsymmetricKeyParameter key in keys)
		{
			try
			{
				crl.Verify(key);
				return key;
			}
			catch (Exception ex)
			{
				innerException = ex;
			}
		}
		throw new Exception("Cannot verify CRL.", innerException);
	}

	internal static X509Crl ProcessCrlH(HashSet<X509Crl> deltaCrls, AsymmetricKeyParameter key)
	{
		Exception ex = null;
		foreach (X509Crl deltaCrl in deltaCrls)
		{
			try
			{
				deltaCrl.Verify(key);
				return deltaCrl;
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
		}
		if (ex != null)
		{
			throw new Exception("Cannot verify delta CRL.", ex);
		}
		return null;
	}

	private static void CheckCrl(DistributionPoint dp, PkixParameters paramsPKIX, X509Certificate cert, DateTime validDate, X509Certificate defaultCRLSignCert, AsymmetricKeyParameter defaultCRLSignKey, CertStatus certStatus, ReasonsMask reasonMask, IList<X509Certificate> certPathCerts)
	{
		DateTime utcNow = DateTime.UtcNow;
		if (validDate.Ticks > utcNow.Ticks)
		{
			throw new Exception("Validation time is in future.");
		}
		ISet<X509Crl> completeCrls = PkixCertPathValidatorUtilities.GetCompleteCrls(dp, cert, utcNow, paramsPKIX);
		bool flag = false;
		Exception ex = null;
		IEnumerator<X509Crl> enumerator = completeCrls.GetEnumerator();
		while (enumerator.MoveNext() && certStatus.Status == 11 && !reasonMask.IsAllReasons)
		{
			try
			{
				X509Crl current = enumerator.Current;
				ReasonsMask reasonsMask = ProcessCrlD(current, dp);
				if (!reasonsMask.HasNewReasons(reasonMask))
				{
					continue;
				}
				HashSet<AsymmetricKeyParameter> keys = ProcessCrlF(current, cert, defaultCRLSignCert, defaultCRLSignKey, paramsPKIX, certPathCerts);
				AsymmetricKeyParameter key = ProcessCrlG(current, keys);
				X509Crl x509Crl = null;
				if (paramsPKIX.IsUseDeltasEnabled)
				{
					x509Crl = ProcessCrlH(PkixCertPathValidatorUtilities.GetDeltaCrls(utcNow, paramsPKIX, current), key);
				}
				if (paramsPKIX.ValidityModel != 1 && cert.NotAfter.Ticks < current.ThisUpdate.Ticks)
				{
					throw new Exception("No valid CRL for current time found.");
				}
				ProcessCrlB1(dp, cert, current);
				ProcessCrlB2(dp, cert, current);
				ProcessCrlC(x509Crl, current, paramsPKIX);
				ProcessCrlI(validDate, x509Crl, cert, certStatus, paramsPKIX);
				ProcessCrlJ(validDate, current, cert, certStatus);
				if (certStatus.Status == 8)
				{
					certStatus.Status = 11;
				}
				reasonMask.AddReasons(reasonsMask);
				ISet<string> criticalExtensionOids = current.GetCriticalExtensionOids();
				if (criticalExtensionOids != null)
				{
					criticalExtensionOids = new HashSet<string>(criticalExtensionOids);
					criticalExtensionOids.Remove(X509Extensions.IssuingDistributionPoint.Id);
					criticalExtensionOids.Remove(X509Extensions.DeltaCrlIndicator.Id);
					if (criticalExtensionOids.Count > 0)
					{
						throw new Exception("CRL contains unsupported critical extensions.");
					}
				}
				if (x509Crl != null)
				{
					criticalExtensionOids = x509Crl.GetCriticalExtensionOids();
					if (criticalExtensionOids != null)
					{
						criticalExtensionOids = new HashSet<string>(criticalExtensionOids);
						criticalExtensionOids.Remove(X509Extensions.IssuingDistributionPoint.Id);
						criticalExtensionOids.Remove(X509Extensions.DeltaCrlIndicator.Id);
						if (criticalExtensionOids.Count > 0)
						{
							throw new Exception("Delta CRL contains unsupported critical extension.");
						}
					}
				}
				flag = true;
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
		}
		if (!flag)
		{
			throw ex;
		}
	}

	internal static void CheckCrls(PkixParameters paramsPKIX, X509Certificate cert, DateTime validDate, X509Certificate sign, AsymmetricKeyParameter workingPublicKey, IList<X509Certificate> certPathCerts)
	{
		Exception ex = null;
		CrlDistPoint instance;
		try
		{
			instance = CrlDistPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.CrlDistributionPoints));
		}
		catch (Exception innerException)
		{
			throw new Exception("CRL distribution point extension could not be read.", innerException);
		}
		try
		{
			PkixCertPathValidatorUtilities.AddAdditionalStoresFromCrlDistributionPoint(instance, paramsPKIX);
		}
		catch (Exception innerException2)
		{
			throw new Exception("No additional CRL locations could be decoded from CRL distribution point extension.", innerException2);
		}
		CertStatus certStatus = new CertStatus();
		ReasonsMask reasonsMask = new ReasonsMask();
		bool flag = false;
		if (instance != null)
		{
			DistributionPoint[] distributionPoints;
			try
			{
				distributionPoints = instance.GetDistributionPoints();
			}
			catch (Exception innerException3)
			{
				throw new Exception("Distribution points could not be read.", innerException3);
			}
			if (distributionPoints != null)
			{
				for (int i = 0; i < distributionPoints.Length; i++)
				{
					if (certStatus.Status != 11)
					{
						break;
					}
					if (reasonsMask.IsAllReasons)
					{
						break;
					}
					PkixParameters paramsPKIX2 = (PkixParameters)paramsPKIX.Clone();
					try
					{
						CheckCrl(distributionPoints[i], paramsPKIX2, cert, validDate, sign, workingPublicKey, certStatus, reasonsMask, certPathCerts);
						flag = true;
					}
					catch (Exception ex2)
					{
						ex = ex2;
					}
				}
			}
		}
		if (certStatus.Status == 11 && !reasonsMask.IsAllReasons)
		{
			try
			{
				DistributionPoint dp = new DistributionPoint(new DistributionPointName(0, new GeneralNames(new GeneralName(4, cert.IssuerDN))), null, null);
				PkixParameters paramsPKIX3 = (PkixParameters)paramsPKIX.Clone();
				CheckCrl(dp, paramsPKIX3, cert, validDate, sign, workingPublicKey, certStatus, reasonsMask, certPathCerts);
				flag = true;
			}
			catch (Exception ex3)
			{
				ex = ex3;
			}
		}
		if (!flag)
		{
			throw ex;
		}
		if (certStatus.Status != 11)
		{
			string text = certStatus.RevocationDate.Value.ToString("ddd MMM dd HH:mm:ss K yyyy");
			throw new Exception(string.Concat("Certificate revocation after " + text, ", reason: ", CrlReasons[certStatus.Status]));
		}
		if (!reasonsMask.IsAllReasons && certStatus.Status == 11)
		{
			certStatus.Status = 12;
		}
		if (certStatus.Status == 12)
		{
			throw new Exception("Certificate status could not be determined.");
		}
	}

	internal static PkixPolicyNode PrepareCertB(PkixCertPath certPath, int index, IList<PkixPolicyNode>[] policyNodes, PkixPolicyNode validPolicyTree, int policyMapping)
	{
		IList<X509Certificate> certificates = certPath.Certificates;
		X509Certificate x509Certificate = certificates[index];
		int num = certificates.Count - index;
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(x509Certificate, X509Extensions.PolicyMappings));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Policy mappings extension could not be decoded.", innerException, index);
		}
		PkixPolicyNode pkixPolicyNode = validPolicyTree;
		if (instance != null)
		{
			Asn1Sequence asn1Sequence = instance;
			Dictionary<string, ISet<string>> dictionary = new Dictionary<string, ISet<string>>();
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < asn1Sequence.Count; i++)
			{
				Asn1Sequence obj = (Asn1Sequence)asn1Sequence[i];
				string id = ((DerObjectIdentifier)obj[0]).Id;
				string id2 = ((DerObjectIdentifier)obj[1]).Id;
				if (dictionary.TryGetValue(id, out var value))
				{
					value.Add(id2);
					continue;
				}
				value = new HashSet<string>();
				value.Add(id2);
				dictionary[id] = value;
				hashSet.Add(id);
			}
			foreach (string item in hashSet)
			{
				if (policyMapping > 0)
				{
					bool flag = false;
					foreach (PkixPolicyNode item2 in policyNodes[num])
					{
						if (item2.ValidPolicy.Equals(item))
						{
							flag = true;
							item2.ExpectedPolicies = CollectionUtilities.GetValueOrNull(dictionary, item);
							break;
						}
					}
					if (flag)
					{
						continue;
					}
					foreach (PkixPolicyNode item3 in policyNodes[num])
					{
						if (!ANY_POLICY.Equals(item3.ValidPolicy))
						{
							continue;
						}
						Asn1Sequence instance2;
						try
						{
							instance2 = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(x509Certificate, X509Extensions.CertificatePolicies));
						}
						catch (Exception innerException2)
						{
							throw new PkixCertPathValidatorException("Certificate policies extension could not be decoded.", innerException2, index);
						}
						ISet<PolicyQualifierInfo> policyQualifiers = null;
						foreach (Asn1Encodable item4 in instance2)
						{
							PolicyInformation policyInformation = null;
							try
							{
								policyInformation = PolicyInformation.GetInstance(item4.ToAsn1Object());
							}
							catch (Exception innerException3)
							{
								throw new PkixCertPathValidatorException("Policy information could not be decoded.", innerException3, index);
							}
							if (ANY_POLICY.Equals(policyInformation.PolicyIdentifier.Id))
							{
								try
								{
									policyQualifiers = PkixCertPathValidatorUtilities.GetQualifierSet(policyInformation.PolicyQualifiers);
								}
								catch (PkixCertPathValidatorException innerException4)
								{
									throw new PkixCertPathValidatorException("Policy qualifier info set could not be decoded.", innerException4, index);
								}
								break;
							}
						}
						bool critical = false;
						ISet<string> criticalExtensionOids = x509Certificate.GetCriticalExtensionOids();
						if (criticalExtensionOids != null)
						{
							critical = criticalExtensionOids.Contains(X509Extensions.CertificatePolicies.Id);
						}
						PkixPolicyNode parent = item3.Parent;
						if (ANY_POLICY.Equals(parent.ValidPolicy))
						{
							PkixPolicyNode pkixPolicyNode2 = new PkixPolicyNode(new List<PkixPolicyNode>(), num, CollectionUtilities.GetValueOrNull(dictionary, item), parent, policyQualifiers, item, critical);
							parent.AddChild(pkixPolicyNode2);
							policyNodes[num].Add(pkixPolicyNode2);
						}
						break;
					}
				}
				else
				{
					if (policyMapping > 0)
					{
						continue;
					}
					foreach (PkixPolicyNode item5 in new List<PkixPolicyNode>(policyNodes[num]))
					{
						if (!item5.ValidPolicy.Equals(item))
						{
							continue;
						}
						item5.Parent.RemoveChild(item5);
						for (int num2 = num - 1; num2 >= 0; num2--)
						{
							foreach (PkixPolicyNode item6 in new List<PkixPolicyNode>(policyNodes[num2]))
							{
								if (!item6.HasChildren)
								{
									pkixPolicyNode = PkixCertPathValidatorUtilities.RemovePolicyNode(pkixPolicyNode, policyNodes, item6);
									if (pkixPolicyNode == null)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}
		return pkixPolicyNode;
	}

	internal static ISet<X509Crl>[] ProcessCrlA1ii(DateTime currentDate, PkixParameters paramsPKIX, X509Certificate cert, X509Crl crl)
	{
		X509CrlStoreSelector x509CrlStoreSelector = new X509CrlStoreSelector();
		x509CrlStoreSelector.CertificateChecking = cert;
		try
		{
			List<X509Name> list = new List<X509Name>();
			list.Add(crl.IssuerDN);
			x509CrlStoreSelector.Issuers = list;
		}
		catch (IOException ex)
		{
			throw new Exception("Cannot extract issuer from CRL." + ex, ex);
		}
		x509CrlStoreSelector.CompleteCrlEnabled = true;
		ISet<X509Crl> set = CrlUtilities.FindCrls(x509CrlStoreSelector, paramsPKIX, currentDate);
		HashSet<X509Crl> hashSet = new HashSet<X509Crl>();
		if (paramsPKIX.IsUseDeltasEnabled)
		{
			try
			{
				hashSet.UnionWith(PkixCertPathValidatorUtilities.GetDeltaCrls(currentDate, paramsPKIX, crl));
			}
			catch (Exception innerException)
			{
				throw new Exception("Exception obtaining delta CRLs.", innerException);
			}
		}
		return new ISet<X509Crl>[2] { set, hashSet };
	}

	internal static ISet<X509Crl> ProcessCrlA1i(DateTime currentDate, PkixParameters paramsPKIX, X509Certificate cert, X509Crl crl)
	{
		HashSet<X509Crl> hashSet = new HashSet<X509Crl>();
		if (paramsPKIX.IsUseDeltasEnabled)
		{
			CrlDistPoint instance;
			try
			{
				instance = CrlDistPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(cert, X509Extensions.FreshestCrl));
			}
			catch (Exception innerException)
			{
				throw new Exception("Freshest CRL extension could not be decoded from certificate.", innerException);
			}
			if (instance == null)
			{
				try
				{
					instance = CrlDistPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(crl, X509Extensions.FreshestCrl));
				}
				catch (Exception innerException2)
				{
					throw new Exception("Freshest CRL extension could not be decoded from CRL.", innerException2);
				}
			}
			if (instance != null)
			{
				try
				{
					PkixCertPathValidatorUtilities.AddAdditionalStoresFromCrlDistributionPoint(instance, paramsPKIX);
				}
				catch (Exception innerException3)
				{
					throw new Exception("No new delta CRL locations could be added from Freshest CRL extension.", innerException3);
				}
				try
				{
					hashSet.UnionWith(PkixCertPathValidatorUtilities.GetDeltaCrls(currentDate, paramsPKIX, crl));
				}
				catch (Exception innerException4)
				{
					throw new Exception("Exception obtaining delta CRLs.", innerException4);
				}
			}
		}
		return hashSet;
	}

	internal static void ProcessCertF(PkixCertPath certPath, int index, PkixPolicyNode validPolicyTree, int explicitPolicy)
	{
		if (explicitPolicy <= 0 && validPolicyTree == null)
		{
			throw new PkixCertPathValidatorException("No valid policy tree found when one expected.", null, index);
		}
	}

	internal static void ProcessCertA(PkixCertPath certPath, PkixParameters paramsPKIX, int index, AsymmetricKeyParameter workingPublicKey, X509Name workingIssuerName, X509Certificate sign)
	{
		IList<X509Certificate> certificates = certPath.Certificates;
		X509Certificate x509Certificate = certificates[index];
		try
		{
			x509Certificate.Verify(workingPublicKey);
		}
		catch (GeneralSecurityException innerException)
		{
			throw new PkixCertPathValidatorException("Could not validate certificate signature.", innerException, index);
		}
		try
		{
			x509Certificate.CheckValidity(PkixCertPathValidatorUtilities.GetValidCertDateFromValidityModel(paramsPKIX, certPath, index));
		}
		catch (CertificateExpiredException ex)
		{
			throw new PkixCertPathValidatorException("Could not validate certificate: " + ex.Message, ex, index);
		}
		catch (CertificateNotYetValidException ex2)
		{
			throw new PkixCertPathValidatorException("Could not validate certificate: " + ex2.Message, ex2, index);
		}
		catch (Exception innerException2)
		{
			throw new PkixCertPathValidatorException("Could not validate time of certificate.", innerException2, index);
		}
		if (paramsPKIX.IsRevocationEnabled)
		{
			try
			{
				CheckCrls(paramsPKIX, x509Certificate, PkixCertPathValidatorUtilities.GetValidCertDateFromValidityModel(paramsPKIX, certPath, index), sign, workingPublicKey, certificates);
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3.InnerException;
				if (ex4 == null)
				{
					ex4 = ex3;
				}
				throw new PkixCertPathValidatorException(ex3.Message, ex4, index);
			}
		}
		X509Name issuerPrincipal = PkixCertPathValidatorUtilities.GetIssuerPrincipal(x509Certificate);
		if (!issuerPrincipal.Equivalent(workingIssuerName, inOrder: true))
		{
			throw new PkixCertPathValidatorException("IssuerName(" + issuerPrincipal?.ToString() + ") does not match SubjectName(" + workingIssuerName?.ToString() + ") of signing certificate.", null, index);
		}
	}

	internal static int PrepareNextCertI1(PkixCertPath certPath, int index, int explicitPolicy)
	{
		X509Certificate extensions = certPath.Certificates[index];
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.PolicyConstraints));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Policy constraints extension cannot be decoded.", innerException, index);
		}
		if (instance != null)
		{
			foreach (Asn1Encodable item in instance)
			{
				try
				{
					Asn1TaggedObject instance2 = Asn1TaggedObject.GetInstance(item);
					if (instance2.HasContextTag(0))
					{
						int intValueExact = DerInteger.GetInstance(instance2, declaredExplicit: false).IntValueExact;
						if (intValueExact < explicitPolicy)
						{
							return intValueExact;
						}
						break;
					}
				}
				catch (ArgumentException innerException2)
				{
					throw new PkixCertPathValidatorException("Policy constraints extension contents cannot be decoded.", innerException2, index);
				}
			}
		}
		return explicitPolicy;
	}

	internal static int PrepareNextCertI2(PkixCertPath certPath, int index, int policyMapping)
	{
		X509Certificate extensions = certPath.Certificates[index];
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.PolicyConstraints));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Policy constraints extension cannot be decoded.", innerException, index);
		}
		if (instance != null)
		{
			foreach (Asn1Encodable item in instance)
			{
				try
				{
					Asn1TaggedObject instance2 = Asn1TaggedObject.GetInstance(item);
					if (instance2.HasContextTag(1))
					{
						int intValueExact = DerInteger.GetInstance(instance2, declaredExplicit: false).IntValueExact;
						if (intValueExact < policyMapping)
						{
							return intValueExact;
						}
						break;
					}
				}
				catch (ArgumentException innerException2)
				{
					throw new PkixCertPathValidatorException("Policy constraints extension contents cannot be decoded.", innerException2, index);
				}
			}
		}
		return policyMapping;
	}

	internal static void PrepareNextCertG(PkixCertPath certPath, int index, PkixNameConstraintValidator nameConstraintValidator)
	{
		X509Certificate extensions = certPath.Certificates[index];
		NameConstraints nameConstraints = null;
		try
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.NameConstraints));
			if (instance != null)
			{
				nameConstraints = NameConstraints.GetInstance(instance);
			}
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Name constraints extension could not be decoded.", innerException, index);
		}
		if (nameConstraints == null)
		{
			return;
		}
		Asn1Sequence permittedSubtrees = nameConstraints.PermittedSubtrees;
		if (permittedSubtrees != null)
		{
			try
			{
				nameConstraintValidator.IntersectPermittedSubtree(permittedSubtrees);
			}
			catch (Exception innerException2)
			{
				throw new PkixCertPathValidatorException("Permitted subtrees cannot be build from name constraints extension.", innerException2, index);
			}
		}
		Asn1Sequence excludedSubtrees = nameConstraints.ExcludedSubtrees;
		if (excludedSubtrees == null)
		{
			return;
		}
		try
		{
			foreach (Asn1Encodable item in excludedSubtrees)
			{
				GeneralSubtree instance2 = GeneralSubtree.GetInstance(item);
				nameConstraintValidator.AddExcludedSubtree(instance2);
			}
		}
		catch (Exception innerException3)
		{
			throw new PkixCertPathValidatorException("Excluded subtrees cannot be build from name constraints extension.", innerException3, index);
		}
	}

	internal static int PrepareNextCertJ(PkixCertPath certPath, int index, int inhibitAnyPolicy)
	{
		X509Certificate extensions = certPath.Certificates[index];
		DerInteger instance;
		try
		{
			instance = DerInteger.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.InhibitAnyPolicy));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Inhibit any-policy extension cannot be decoded.", innerException, index);
		}
		if (instance != null)
		{
			int intValueExact = instance.IntValueExact;
			if (intValueExact < inhibitAnyPolicy)
			{
				return intValueExact;
			}
		}
		return inhibitAnyPolicy;
	}

	internal static void PrepareNextCertK(PkixCertPath certPath, int index)
	{
		X509Certificate extensions = certPath.Certificates[index];
		BasicConstraints instance;
		try
		{
			instance = BasicConstraints.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.BasicConstraints));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Basic constraints extension cannot be decoded.", innerException, index);
		}
		if (instance != null)
		{
			if (!instance.IsCA())
			{
				throw new PkixCertPathValidatorException("Not a CA certificate");
			}
			return;
		}
		throw new PkixCertPathValidatorException("Intermediate certificate lacks BasicConstraints");
	}

	internal static int PrepareNextCertL(PkixCertPath certPath, int index, int maxPathLength)
	{
		if (!PkixCertPathValidatorUtilities.IsSelfIssued(certPath.Certificates[index]))
		{
			if (maxPathLength <= 0)
			{
				throw new PkixCertPathValidatorException("Max path length not greater than zero", null, index);
			}
			return maxPathLength - 1;
		}
		return maxPathLength;
	}

	internal static int PrepareNextCertM(PkixCertPath certPath, int index, int maxPathLength)
	{
		X509Certificate extensions = certPath.Certificates[index];
		BasicConstraints instance;
		try
		{
			instance = BasicConstraints.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.BasicConstraints));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Basic constraints extension cannot be decoded.", innerException, index);
		}
		if (instance != null && instance.IsCA())
		{
			DerInteger pathLenConstraintInteger = instance.PathLenConstraintInteger;
			if (pathLenConstraintInteger != null)
			{
				maxPathLength = System.Math.Min(maxPathLength, pathLenConstraintInteger.IntPositiveValueExact);
			}
		}
		return maxPathLength;
	}

	internal static void PrepareNextCertN(PkixCertPath certPath, int index)
	{
		bool[] keyUsage = certPath.Certificates[index].GetKeyUsage();
		if (keyUsage != null && !keyUsage[KEY_CERT_SIGN])
		{
			throw new PkixCertPathValidatorException("Issuer certificate keyusage extension is critical and does not permit key signing.", null, index);
		}
	}

	internal static void PrepareNextCertO(PkixCertPath certPath, int index, ISet<string> criticalExtensions, IEnumerable<PkixCertPathChecker> checkers)
	{
		X509Certificate cert = certPath.Certificates[index];
		foreach (PkixCertPathChecker checker in checkers)
		{
			try
			{
				checker.Check(cert, criticalExtensions);
			}
			catch (PkixCertPathValidatorException ex)
			{
				throw new PkixCertPathValidatorException(ex.Message, ex.InnerException, index);
			}
		}
		if (criticalExtensions.Count > 0)
		{
			throw new PkixCertPathValidatorException("Certificate has unsupported critical extension.", null, index);
		}
	}

	internal static int PrepareNextCertH1(PkixCertPath certPath, int index, int explicitPolicy)
	{
		if (!PkixCertPathValidatorUtilities.IsSelfIssued(certPath.Certificates[index]) && explicitPolicy != 0)
		{
			return explicitPolicy - 1;
		}
		return explicitPolicy;
	}

	internal static int PrepareNextCertH2(PkixCertPath certPath, int index, int policyMapping)
	{
		if (!PkixCertPathValidatorUtilities.IsSelfIssued(certPath.Certificates[index]) && policyMapping != 0)
		{
			return policyMapping - 1;
		}
		return policyMapping;
	}

	internal static int PrepareNextCertH3(PkixCertPath certPath, int index, int inhibitAnyPolicy)
	{
		if (!PkixCertPathValidatorUtilities.IsSelfIssued(certPath.Certificates[index]) && inhibitAnyPolicy != 0)
		{
			return inhibitAnyPolicy - 1;
		}
		return inhibitAnyPolicy;
	}

	internal static int WrapupCertA(int explicitPolicy, X509Certificate cert)
	{
		if (!PkixCertPathValidatorUtilities.IsSelfIssued(cert) && explicitPolicy != 0)
		{
			explicitPolicy--;
		}
		return explicitPolicy;
	}

	internal static int WrapupCertB(PkixCertPath certPath, int index, int explicitPolicy)
	{
		X509Certificate extensions = certPath.Certificates[index];
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.PolicyConstraints));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Policy constraints could not be decoded.", innerException, index);
		}
		if (instance != null)
		{
			foreach (Asn1Encodable item in instance)
			{
				Asn1TaggedObject instance2 = Asn1TaggedObject.GetInstance(item);
				if (instance2.HasContextTag(0))
				{
					int intValueExact;
					try
					{
						intValueExact = DerInteger.GetInstance(instance2, declaredExplicit: false).IntValueExact;
					}
					catch (Exception innerException2)
					{
						throw new PkixCertPathValidatorException("Policy constraints requireExplicitPolicy field could not be decoded.", innerException2, index);
					}
					if (intValueExact == 0)
					{
						return 0;
					}
					break;
				}
			}
		}
		return explicitPolicy;
	}

	internal static void WrapupCertF(PkixCertPath certPath, int index, IEnumerable<PkixCertPathChecker> checkers, ISet<string> criticalExtensions)
	{
		X509Certificate cert = certPath.Certificates[index];
		foreach (PkixCertPathChecker checker in checkers)
		{
			try
			{
				checker.Check(cert, criticalExtensions);
			}
			catch (PkixCertPathValidatorException innerException)
			{
				throw new PkixCertPathValidatorException("Additional certificate path checker failed.", innerException, index);
			}
		}
		if (criticalExtensions.Count > 0)
		{
			throw new PkixCertPathValidatorException("Certificate has unsupported critical extension", null, index);
		}
	}

	internal static PkixPolicyNode WrapupCertG(PkixCertPath certPath, PkixParameters paramsPKIX, ISet<string> userInitialPolicySet, int index, IList<PkixPolicyNode>[] policyNodes, PkixPolicyNode validPolicyTree, HashSet<string> acceptablePolicies)
	{
		int count = certPath.Certificates.Count;
		if (validPolicyTree == null)
		{
			if (paramsPKIX.IsExplicitPolicyRequired)
			{
				throw new PkixCertPathValidatorException("Explicit policy requested but none available.", null, index);
			}
			return null;
		}
		IList<PkixPolicyNode>[] array;
		if (PkixCertPathValidatorUtilities.IsAnyPolicy(userInitialPolicySet))
		{
			if (paramsPKIX.IsExplicitPolicyRequired)
			{
				if (acceptablePolicies.Count < 1)
				{
					throw new PkixCertPathValidatorException("Explicit policy requested but none available.", null, index);
				}
				HashSet<PkixPolicyNode> hashSet = new HashSet<PkixPolicyNode>();
				array = policyNodes;
				for (int i = 0; i < array.Length; i++)
				{
					foreach (PkixPolicyNode item in array[i])
					{
						if (!ANY_POLICY.Equals(item.ValidPolicy))
						{
							continue;
						}
						foreach (PkixPolicyNode child in item.Children)
						{
							hashSet.Add(child);
						}
					}
				}
				foreach (PkixPolicyNode item2 in hashSet)
				{
					acceptablePolicies.Contains(item2.ValidPolicy);
				}
				if (validPolicyTree != null)
				{
					for (int num = count - 1; num >= 0; num--)
					{
						IList<PkixPolicyNode> list = policyNodes[num];
						for (int j = 0; j < list.Count; j++)
						{
							PkixPolicyNode pkixPolicyNode = list[j];
							if (!pkixPolicyNode.HasChildren)
							{
								validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree, policyNodes, pkixPolicyNode);
							}
						}
					}
				}
			}
			return validPolicyTree;
		}
		HashSet<PkixPolicyNode> hashSet2 = new HashSet<PkixPolicyNode>();
		array = policyNodes;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (PkixPolicyNode item3 in array[i])
			{
				if (!ANY_POLICY.Equals(item3.ValidPolicy))
				{
					continue;
				}
				foreach (PkixPolicyNode child2 in item3.Children)
				{
					if (!ANY_POLICY.Equals(child2.ValidPolicy))
					{
						hashSet2.Add(child2);
					}
				}
			}
		}
		foreach (PkixPolicyNode item4 in hashSet2)
		{
			if (!userInitialPolicySet.Contains(item4.ValidPolicy))
			{
				validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree, policyNodes, item4);
			}
		}
		if (validPolicyTree != null)
		{
			for (int num2 = count - 1; num2 >= 0; num2--)
			{
				IList<PkixPolicyNode> list2 = policyNodes[num2];
				for (int k = 0; k < list2.Count; k++)
				{
					PkixPolicyNode pkixPolicyNode2 = list2[k];
					if (!pkixPolicyNode2.HasChildren)
					{
						validPolicyTree = PkixCertPathValidatorUtilities.RemovePolicyNode(validPolicyTree, policyNodes, pkixPolicyNode2);
					}
				}
			}
		}
		return validPolicyTree;
	}

	internal static void ProcessCrlC(X509Crl deltaCRL, X509Crl completeCRL, PkixParameters pkixParams)
	{
		if (deltaCRL == null)
		{
			return;
		}
		IssuingDistributionPoint instance;
		try
		{
			instance = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(completeCRL, X509Extensions.IssuingDistributionPoint));
		}
		catch (Exception innerException)
		{
			throw new Exception("000 Issuing distribution point extension could not be decoded.", innerException);
		}
		if (pkixParams.IsUseDeltasEnabled)
		{
			if (!deltaCRL.IssuerDN.Equivalent(completeCRL.IssuerDN, inOrder: true))
			{
				throw new Exception("Complete CRL issuer does not match delta CRL issuer.");
			}
			IssuingDistributionPoint instance2;
			try
			{
				instance2 = IssuingDistributionPoint.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(deltaCRL, X509Extensions.IssuingDistributionPoint));
			}
			catch (Exception innerException2)
			{
				throw new Exception("Issuing distribution point extension from delta CRL could not be decoded.", innerException2);
			}
			if (!object.Equals(instance, instance2))
			{
				throw new Exception("Issuing distribution point extension from delta CRL and complete CRL does not match.");
			}
			Asn1Object extensionValue;
			try
			{
				extensionValue = PkixCertPathValidatorUtilities.GetExtensionValue(completeCRL, X509Extensions.AuthorityKeyIdentifier);
			}
			catch (Exception innerException3)
			{
				throw new Exception("Authority key identifier extension could not be extracted from complete CRL.", innerException3);
			}
			Asn1Object extensionValue2;
			try
			{
				extensionValue2 = PkixCertPathValidatorUtilities.GetExtensionValue(deltaCRL, X509Extensions.AuthorityKeyIdentifier);
			}
			catch (Exception innerException4)
			{
				throw new Exception("Authority key identifier extension could not be extracted from delta CRL.", innerException4);
			}
			if (extensionValue == null)
			{
				throw new Exception("CRL authority key identifier is null.");
			}
			if (extensionValue2 == null)
			{
				throw new Exception("Delta CRL authority key identifier is null.");
			}
			if (!extensionValue.Equals(extensionValue2))
			{
				throw new Exception("Delta CRL authority key identifier does not match complete CRL authority key identifier.");
			}
		}
	}

	internal static void ProcessCrlI(DateTime validDate, X509Crl deltacrl, object cert, CertStatus certStatus, PkixParameters pkixParams)
	{
		if (pkixParams.IsUseDeltasEnabled && deltacrl != null)
		{
			PkixCertPathValidatorUtilities.GetCertStatus(validDate, deltacrl, cert, certStatus);
		}
	}

	internal static void ProcessCrlJ(DateTime validDate, X509Crl completecrl, object cert, CertStatus certStatus)
	{
		if (certStatus.Status == 11)
		{
			PkixCertPathValidatorUtilities.GetCertStatus(validDate, completecrl, cert, certStatus);
		}
	}

	internal static PkixPolicyNode ProcessCertE(PkixCertPath certPath, int index, PkixPolicyNode validPolicyTree)
	{
		X509Certificate extensions = certPath.Certificates[index];
		Asn1Sequence instance;
		try
		{
			instance = Asn1Sequence.GetInstance(PkixCertPathValidatorUtilities.GetExtensionValue(extensions, X509Extensions.CertificatePolicies));
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Could not read certificate policies extension from certificate.", innerException, index);
		}
		if (instance == null)
		{
			validPolicyTree = null;
		}
		return validPolicyTree;
	}
}
