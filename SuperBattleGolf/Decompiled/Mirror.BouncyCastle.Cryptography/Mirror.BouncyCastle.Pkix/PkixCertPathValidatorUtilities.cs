using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.IsisMtt;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;
using Mirror.BouncyCastle.X509.Extension;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Pkix;

internal static class PkixCertPathValidatorUtilities
{
	private static readonly PkixCrlUtilities CrlUtilities = new PkixCrlUtilities();

	internal static readonly string ANY_POLICY = "2.5.29.32.0";

	internal static readonly string CRL_NUMBER = X509Extensions.CrlNumber.Id;

	internal static readonly int KEY_CERT_SIGN = 5;

	internal static readonly int CRL_SIGN = 6;

	internal static TrustAnchor FindTrustAnchor(X509Certificate cert, ISet<TrustAnchor> trustAnchors)
	{
		IEnumerator<TrustAnchor> enumerator = trustAnchors.GetEnumerator();
		TrustAnchor trustAnchor = null;
		AsymmetricKeyParameter asymmetricKeyParameter = null;
		Exception ex = null;
		X509CertStoreSelector x509CertStoreSelector = new X509CertStoreSelector();
		try
		{
			x509CertStoreSelector.Subject = GetIssuerPrincipal(cert);
		}
		catch (IOException innerException)
		{
			throw new Exception("Cannot set subject search criteria for trust anchor.", innerException);
		}
		while (enumerator.MoveNext() && trustAnchor == null)
		{
			trustAnchor = enumerator.Current;
			if (trustAnchor.TrustedCert != null)
			{
				if (x509CertStoreSelector.Match(trustAnchor.TrustedCert))
				{
					asymmetricKeyParameter = trustAnchor.TrustedCert.GetPublicKey();
				}
				else
				{
					trustAnchor = null;
				}
			}
			else if (trustAnchor.CAName != null && trustAnchor.CAPublicKey != null)
			{
				try
				{
					X509Name issuerPrincipal = GetIssuerPrincipal(cert);
					X509Name other = new X509Name(trustAnchor.CAName);
					if (issuerPrincipal.Equivalent(other, inOrder: true))
					{
						asymmetricKeyParameter = trustAnchor.CAPublicKey;
					}
					else
					{
						trustAnchor = null;
					}
				}
				catch (InvalidParameterException)
				{
					trustAnchor = null;
				}
			}
			else
			{
				trustAnchor = null;
			}
			if (asymmetricKeyParameter != null)
			{
				try
				{
					cert.Verify(asymmetricKeyParameter);
				}
				catch (Exception ex3)
				{
					ex = ex3;
					trustAnchor = null;
				}
			}
		}
		if (trustAnchor == null && ex != null)
		{
			throw new Exception("TrustAnchor found but certificate validation failed.", ex);
		}
		return trustAnchor;
	}

	internal static bool IsIssuerTrustAnchor(X509Certificate cert, ISet<TrustAnchor> trustAnchors)
	{
		try
		{
			return FindTrustAnchor(cert, trustAnchors) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	internal static void AddAdditionalStoresFromAltNames(X509Certificate cert, PkixParameters pkixParams)
	{
		IList<IList<object>> issuerAlternativeNames = cert.GetIssuerAlternativeNames();
		if (issuerAlternativeNames == null)
		{
			return;
		}
		foreach (IList<object> item in issuerAlternativeNames)
		{
			if (item.Count >= 2 && item[0].Equals(6))
			{
				AddAdditionalStoreFromLocation((string)item[1], pkixParams);
			}
		}
	}

	internal static DateTime GetValidDate(PkixParameters paramsPKIX)
	{
		DateTime? date = paramsPKIX.Date;
		if (!date.HasValue)
		{
			return DateTime.UtcNow;
		}
		return date.Value;
	}

	internal static X509Name GetIssuerPrincipal(object obj)
	{
		if (obj is X509Certificate x509Certificate)
		{
			return x509Certificate.IssuerDN;
		}
		if (obj is X509V2AttributeCertificate x509V2AttributeCertificate)
		{
			return x509V2AttributeCertificate.Issuer.GetPrincipals()[0];
		}
		throw new InvalidOperationException();
	}

	internal static X509Name GetIssuerPrincipal(X509V2AttributeCertificate attrCert)
	{
		return attrCert.Issuer.GetPrincipals()[0];
	}

	internal static X509Name GetIssuerPrincipal(X509Certificate cert)
	{
		return cert.IssuerDN;
	}

	internal static bool IsSelfIssued(X509Certificate cert)
	{
		return cert.SubjectDN.Equivalent(cert.IssuerDN, inOrder: true);
	}

	internal static AlgorithmIdentifier GetAlgorithmIdentifier(AsymmetricKeyParameter key)
	{
		try
		{
			return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key).Algorithm;
		}
		catch (Exception innerException)
		{
			throw new PkixCertPathValidatorException("Subject public key cannot be decoded.", innerException);
		}
	}

	internal static bool IsAnyPolicy(ISet<string> policySet)
	{
		if (policySet != null && policySet.Count >= 1)
		{
			return policySet.Contains(ANY_POLICY);
		}
		return true;
	}

	internal static void AddAdditionalStoreFromLocation(string location, PkixParameters pkixParams)
	{
		if (!pkixParams.IsAdditionalLocationsEnabled)
		{
			return;
		}
		try
		{
			if (Platform.StartsWith(location, "ldap://"))
			{
				location = location.Substring(7);
				int num = location.IndexOf('/');
				if (num != -1)
				{
					_ = "ldap://" + location.Substring(0, num);
				}
				else
				{
					_ = "ldap://" + location;
				}
				throw new NotImplementedException("LDAP cert/CRL stores");
			}
		}
		catch (Exception)
		{
			throw new Exception("Exception adding X.509 stores.");
		}
	}

	private static BigInteger GetSerialNumber(object cert)
	{
		if (cert is X509Certificate)
		{
			return ((X509Certificate)cert).SerialNumber;
		}
		return ((X509V2AttributeCertificate)cert).SerialNumber;
	}

	internal static HashSet<PolicyQualifierInfo> GetQualifierSet(Asn1Sequence qualifiers)
	{
		HashSet<PolicyQualifierInfo> hashSet = new HashSet<PolicyQualifierInfo>();
		if (qualifiers != null)
		{
			foreach (Asn1Encodable qualifier in qualifiers)
			{
				try
				{
					hashSet.Add(PolicyQualifierInfo.GetInstance(qualifier.ToAsn1Object()));
				}
				catch (IOException innerException)
				{
					throw new PkixCertPathValidatorException("Policy qualifier info cannot be decoded.", innerException);
				}
			}
		}
		return hashSet;
	}

	internal static PkixPolicyNode RemovePolicyNode(PkixPolicyNode validPolicyTree, IList<PkixPolicyNode>[] policyNodes, PkixPolicyNode _node)
	{
		PkixPolicyNode parent = _node.Parent;
		if (validPolicyTree == null)
		{
			return null;
		}
		if (parent == null)
		{
			for (int i = 0; i < policyNodes.Length; i++)
			{
				policyNodes[i] = new List<PkixPolicyNode>();
			}
			return null;
		}
		parent.RemoveChild(_node);
		RemovePolicyNodeRecurse(policyNodes, _node);
		return validPolicyTree;
	}

	private static void RemovePolicyNodeRecurse(IList<PkixPolicyNode>[] policyNodes, PkixPolicyNode _node)
	{
		policyNodes[_node.Depth].Remove(_node);
		if (!_node.HasChildren)
		{
			return;
		}
		foreach (PkixPolicyNode child in _node.Children)
		{
			RemovePolicyNodeRecurse(policyNodes, child);
		}
	}

	internal static void PrepareNextCertB1(int i, IList<PkixPolicyNode>[] policyNodes, string id_p, IDictionary<string, HashSet<string>> m_idp, X509Certificate cert)
	{
		foreach (PkixPolicyNode item in policyNodes[i])
		{
			if (item.ValidPolicy.Equals(id_p))
			{
				item.ExpectedPolicies = CollectionUtilities.GetValueOrNull(m_idp, id_p);
				return;
			}
		}
		foreach (PkixPolicyNode item2 in policyNodes[i])
		{
			if (!ANY_POLICY.Equals(item2.ValidPolicy))
			{
				continue;
			}
			Asn1Sequence instance;
			try
			{
				instance = Asn1Sequence.GetInstance(GetExtensionValue(cert, X509Extensions.CertificatePolicies));
			}
			catch (Exception innerException)
			{
				throw new Exception("Certificate policies cannot be decoded.", innerException);
			}
			ISet<PolicyQualifierInfo> policyQualifiers = null;
			foreach (Asn1Encodable item3 in instance)
			{
				PolicyInformation instance2;
				try
				{
					instance2 = PolicyInformation.GetInstance(item3);
				}
				catch (Exception innerException2)
				{
					throw new Exception("Policy information cannot be decoded.", innerException2);
				}
				if (ANY_POLICY.Equals(instance2.PolicyIdentifier.Id))
				{
					try
					{
						policyQualifiers = GetQualifierSet(instance2.PolicyQualifiers);
					}
					catch (PkixCertPathValidatorException innerException3)
					{
						throw new PkixCertPathValidatorException("Policy qualifier info set could not be built.", innerException3);
					}
					break;
				}
			}
			bool critical = false;
			ISet<string> criticalExtensionOids = cert.GetCriticalExtensionOids();
			if (criticalExtensionOids != null)
			{
				critical = criticalExtensionOids.Contains(X509Extensions.CertificatePolicies.Id);
			}
			PkixPolicyNode parent = item2.Parent;
			if (ANY_POLICY.Equals(parent.ValidPolicy))
			{
				PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), i, CollectionUtilities.GetValueOrNull(m_idp, id_p), parent, policyQualifiers, id_p, critical);
				parent.AddChild(pkixPolicyNode);
				policyNodes[i].Add(pkixPolicyNode);
			}
			break;
		}
	}

	internal static PkixPolicyNode PrepareNextCertB2(int i, IList<PkixPolicyNode>[] policyNodes, string id_p, PkixPolicyNode validPolicyTree)
	{
		int num = 0;
		foreach (PkixPolicyNode item in new List<PkixPolicyNode>(policyNodes[i]))
		{
			if (!item.ValidPolicy.Equals(id_p))
			{
				num++;
				continue;
			}
			item.Parent.RemoveChild(item);
			policyNodes[i].RemoveAt(num);
			for (int num2 = i - 1; num2 >= 0; num2--)
			{
				IList<PkixPolicyNode> list = policyNodes[num2];
				for (int j = 0; j < list.Count; j++)
				{
					PkixPolicyNode pkixPolicyNode = list[j];
					if (!pkixPolicyNode.HasChildren)
					{
						validPolicyTree = RemovePolicyNode(validPolicyTree, policyNodes, pkixPolicyNode);
						if (validPolicyTree == null)
						{
							break;
						}
					}
				}
			}
		}
		return validPolicyTree;
	}

	internal static void GetCertStatus(DateTime validDate, X509Crl crl, object cert, CertStatus certStatus)
	{
		X509Crl x509Crl;
		try
		{
			x509Crl = new X509Crl(CertificateList.GetInstance((Asn1Sequence)Asn1Object.FromByteArray(crl.GetEncoded())));
		}
		catch (Exception innerException)
		{
			throw new Exception("X509Crl could not be created.", innerException);
		}
		X509CrlEntry revokedCertificate = x509Crl.GetRevokedCertificate(GetSerialNumber(cert));
		if (revokedCertificate == null)
		{
			return;
		}
		X509Name issuerPrincipal = GetIssuerPrincipal(cert);
		if (!issuerPrincipal.Equivalent(revokedCertificate.GetCertificateIssuer(), inOrder: true) && !issuerPrincipal.Equivalent(crl.IssuerDN, inOrder: true))
		{
			return;
		}
		int num = 0;
		if (revokedCertificate.HasExtensions)
		{
			try
			{
				DerEnumerated instance = DerEnumerated.GetInstance(GetExtensionValue(revokedCertificate, X509Extensions.ReasonCode));
				if (instance != null)
				{
					num = instance.IntValueExact;
				}
			}
			catch (Exception innerException2)
			{
				throw new Exception("Reason code CRL entry extension could not be decoded.", innerException2);
			}
		}
		DateTime revocationDate = revokedCertificate.RevocationDate;
		if (validDate.Ticks >= revocationDate.Ticks || (uint)num <= 2u || num == 10)
		{
			certStatus.Status = num;
			certStatus.RevocationDate = revocationDate;
		}
	}

	internal static AsymmetricKeyParameter GetNextWorkingKey(IList<X509Certificate> certs, int index)
	{
		AsymmetricKeyParameter publicKey = certs[index].GetPublicKey();
		if (!(publicKey is DsaPublicKeyParameters))
		{
			return publicKey;
		}
		DsaPublicKeyParameters dsaPublicKeyParameters = (DsaPublicKeyParameters)publicKey;
		if (dsaPublicKeyParameters.Parameters != null)
		{
			return dsaPublicKeyParameters;
		}
		for (int i = index + 1; i < certs.Count; i++)
		{
			publicKey = certs[i].GetPublicKey();
			if (!(publicKey is DsaPublicKeyParameters))
			{
				throw new PkixCertPathValidatorException("DSA parameters cannot be inherited from previous certificate.");
			}
			DsaPublicKeyParameters dsaPublicKeyParameters2 = (DsaPublicKeyParameters)publicKey;
			if (dsaPublicKeyParameters2.Parameters != null)
			{
				DsaParameters parameters = dsaPublicKeyParameters2.Parameters;
				try
				{
					return new DsaPublicKeyParameters(dsaPublicKeyParameters.Y, parameters);
				}
				catch (Exception ex)
				{
					throw new Exception(ex.Message);
				}
			}
		}
		throw new PkixCertPathValidatorException("DSA parameters cannot be inherited from previous certificate.");
	}

	internal static DateTime GetValidCertDateFromValidityModel(PkixParameters paramsPkix, PkixCertPath certPath, int index)
	{
		if (1 != paramsPkix.ValidityModel || index <= 0)
		{
			return GetValidDate(paramsPkix);
		}
		X509Certificate x509Certificate = certPath.Certificates[index - 1];
		if (index - 1 == 0)
		{
			Asn1GeneralizedTime asn1GeneralizedTime = null;
			try
			{
				byte[] array = x509Certificate.GetExtensionValue(IsisMttObjectIdentifiers.IdIsisMttATDateOfCertGen)?.GetOctets();
				if (array != null)
				{
					asn1GeneralizedTime = Asn1GeneralizedTime.GetInstance(array);
				}
			}
			catch (ArgumentException innerException)
			{
				throw new Exception("Date of cert gen extension could not be read.", innerException);
			}
			if (asn1GeneralizedTime != null)
			{
				try
				{
					return asn1GeneralizedTime.ToDateTime();
				}
				catch (ArgumentException innerException2)
				{
					throw new Exception("Date from date of cert gen extension could not be parsed.", innerException2);
				}
			}
		}
		return x509Certificate.NotBefore;
	}

	internal static void GetCrlIssuersFromDistributionPoint(DistributionPoint dp, ICollection<X509Name> issuerPrincipals, X509CrlStoreSelector selector, PkixParameters pkixParameters)
	{
		List<X509Name> list = new List<X509Name>();
		if (dp.CrlIssuer != null)
		{
			GeneralName[] names = dp.CrlIssuer.GetNames();
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i].TagNo == 4)
				{
					try
					{
						list.Add(X509Name.GetInstance(names[i].Name.ToAsn1Object()));
					}
					catch (IOException innerException)
					{
						throw new Exception("CRL issuer information from distribution point cannot be decoded.", innerException);
					}
				}
			}
		}
		else
		{
			if (dp.DistributionPointName == null)
			{
				throw new Exception("CRL issuer is omitted from distribution point but no distributionPoint field present.");
			}
			list.AddRange(issuerPrincipals);
		}
		selector.Issuers = list;
	}

	internal static ISet<X509Crl> GetCompleteCrls(DistributionPoint dp, object certObj, DateTime currentDate, PkixParameters pkixParameters)
	{
		X509Name issuerPrincipal = GetIssuerPrincipal(certObj);
		X509CrlStoreSelector x509CrlStoreSelector = new X509CrlStoreSelector();
		try
		{
			HashSet<X509Name> hashSet = new HashSet<X509Name>();
			hashSet.Add(issuerPrincipal);
			GetCrlIssuersFromDistributionPoint(dp, hashSet, x509CrlStoreSelector, pkixParameters);
		}
		catch (Exception innerException)
		{
			throw new Exception("Could not get issuer information from distribution point.", innerException);
		}
		if (certObj is X509Certificate certificateChecking)
		{
			x509CrlStoreSelector.CertificateChecking = certificateChecking;
		}
		else if (certObj is X509V2AttributeCertificate attrCertChecking)
		{
			x509CrlStoreSelector.AttrCertChecking = attrCertChecking;
		}
		x509CrlStoreSelector.CompleteCrlEnabled = true;
		ISet<X509Crl> set = CrlUtilities.FindCrls(x509CrlStoreSelector, pkixParameters, currentDate);
		if (set.Count < 1)
		{
			throw new Exception("No CRLs found for issuer \"" + issuerPrincipal?.ToString() + "\"");
		}
		return set;
	}

	internal static HashSet<X509Crl> GetDeltaCrls(DateTime currentDate, PkixParameters pkixParameters, X509Crl completeCRL)
	{
		X509CrlStoreSelector x509CrlStoreSelector = new X509CrlStoreSelector();
		try
		{
			List<X509Name> list = new List<X509Name>();
			list.Add(completeCRL.IssuerDN);
			x509CrlStoreSelector.Issuers = list;
		}
		catch (IOException innerException)
		{
			throw new Exception("Cannot extract issuer from CRL.", innerException);
		}
		BigInteger bigInteger = null;
		try
		{
			Asn1Object extensionValue = GetExtensionValue(completeCRL, X509Extensions.CrlNumber);
			if (extensionValue != null)
			{
				bigInteger = DerInteger.GetInstance(extensionValue).PositiveValue;
			}
		}
		catch (Exception innerException2)
		{
			throw new Exception("CRL number extension could not be extracted from CRL.", innerException2);
		}
		byte[] issuingDistributionPoint = null;
		try
		{
			Asn1Object extensionValue2 = GetExtensionValue(completeCRL, X509Extensions.IssuingDistributionPoint);
			if (extensionValue2 != null)
			{
				issuingDistributionPoint = extensionValue2.GetDerEncoded();
			}
		}
		catch (Exception innerException3)
		{
			throw new Exception("Issuing distribution point extension value could not be read.", innerException3);
		}
		x509CrlStoreSelector.MinCrlNumber = bigInteger?.Add(BigInteger.One);
		x509CrlStoreSelector.IssuingDistributionPoint = issuingDistributionPoint;
		x509CrlStoreSelector.IssuingDistributionPointEnabled = true;
		x509CrlStoreSelector.MaxBaseCrlNumber = bigInteger;
		ISet<X509Crl> set = CrlUtilities.FindCrls(x509CrlStoreSelector, pkixParameters, currentDate);
		HashSet<X509Crl> hashSet = new HashSet<X509Crl>();
		foreach (X509Crl item in set)
		{
			if (IsDeltaCrl(item))
			{
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	private static bool IsDeltaCrl(X509Crl crl)
	{
		return crl.GetCriticalExtensionOids().Contains(X509Extensions.DeltaCrlIndicator.Id);
	}

	internal static void AddAdditionalStoresFromCrlDistributionPoint(CrlDistPoint crldp, PkixParameters pkixParams)
	{
		if (crldp == null)
		{
			return;
		}
		DistributionPoint[] array = null;
		try
		{
			array = crldp.GetDistributionPoints();
		}
		catch (Exception innerException)
		{
			throw new Exception("Distribution points could not be read.", innerException);
		}
		for (int i = 0; i < array.Length; i++)
		{
			DistributionPointName distributionPointName = array[i].DistributionPointName;
			if (distributionPointName == null || distributionPointName.Type != 0)
			{
				continue;
			}
			GeneralName[] names = GeneralNames.GetInstance(distributionPointName.Name).GetNames();
			for (int j = 0; j < names.Length; j++)
			{
				if (names[j].TagNo == 6)
				{
					AddAdditionalStoreFromLocation(DerIA5String.GetInstance(names[j].Name).GetString(), pkixParams);
				}
			}
		}
	}

	internal static bool ProcessCertD1i(int index, IList<PkixPolicyNode>[] policyNodes, DerObjectIdentifier pOid, HashSet<PolicyQualifierInfo> pq)
	{
		foreach (PkixPolicyNode item in policyNodes[index - 1])
		{
			if (item.ExpectedPolicies.Contains(pOid.Id))
			{
				HashSet<string> hashSet = new HashSet<string>();
				hashSet.Add(pOid.Id);
				PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), index, hashSet, item, pq, pOid.Id, critical: false);
				item.AddChild(pkixPolicyNode);
				policyNodes[index].Add(pkixPolicyNode);
				return true;
			}
		}
		return false;
	}

	internal static void ProcessCertD1ii(int index, IList<PkixPolicyNode>[] policyNodes, DerObjectIdentifier _poid, HashSet<PolicyQualifierInfo> _pq)
	{
		foreach (PkixPolicyNode item in policyNodes[index - 1])
		{
			if (ANY_POLICY.Equals(item.ValidPolicy))
			{
				HashSet<string> hashSet = new HashSet<string>();
				hashSet.Add(_poid.Id);
				PkixPolicyNode pkixPolicyNode = new PkixPolicyNode(new List<PkixPolicyNode>(), index, hashSet, item, _pq, _poid.Id, critical: false);
				item.AddChild(pkixPolicyNode);
				policyNodes[index].Add(pkixPolicyNode);
				break;
			}
		}
	}

	internal static HashSet<X509Certificate> FindIssuerCerts(X509Certificate cert, PkixBuilderParameters pkixBuilderParameters)
	{
		X509CertStoreSelector x509CertStoreSelector = new X509CertStoreSelector();
		try
		{
			x509CertStoreSelector.Subject = cert.IssuerDN;
		}
		catch (IOException innerException)
		{
			throw new Exception("Subject criteria for certificate selector to find issuer certificate could not be set.", innerException);
		}
		HashSet<X509Certificate> hashSet = new HashSet<X509Certificate>();
		try
		{
			CollectionUtilities.CollectMatches(hashSet, x509CertStoreSelector, pkixBuilderParameters.GetStoresCert());
			return hashSet;
		}
		catch (Exception innerException2)
		{
			throw new Exception("Issuer certificate cannot be searched.", innerException2);
		}
	}

	internal static Asn1Object GetExtensionValue(IX509Extension extensions, DerObjectIdentifier oid)
	{
		return X509ExtensionUtilities.FromExtensionValue(extensions, oid);
	}
}
