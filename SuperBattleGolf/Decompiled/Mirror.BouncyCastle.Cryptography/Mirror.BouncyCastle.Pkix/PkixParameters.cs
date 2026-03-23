using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkix;

public class PkixParameters
{
	public const int PkixValidityModel = 0;

	public const int ChainValidityModel = 1;

	private HashSet<TrustAnchor> trustAnchors;

	private DateTime? date;

	private List<PkixCertPathChecker> m_checkers;

	private bool revocationEnabled = true;

	private HashSet<string> initialPolicies;

	private bool explicitPolicyRequired;

	private bool anyPolicyInhibited;

	private bool policyMappingInhibited;

	private bool policyQualifiersRejected = true;

	private List<IStore<X509V2AttributeCertificate>> m_storesAttrCert;

	private List<IStore<X509Certificate>> m_storesCert;

	private List<IStore<X509Crl>> m_storesCrl;

	private ISelector<X509V2AttributeCertificate> m_targetConstraintsAttrCert;

	private ISelector<X509Certificate> m_targetConstraintsCert;

	private bool additionalLocationsEnabled;

	private HashSet<TrustAnchor> trustedACIssuers;

	private HashSet<string> necessaryACAttributes;

	private HashSet<string> prohibitedACAttributes;

	private HashSet<PkixAttrCertChecker> attrCertCheckers;

	private int validityModel;

	private bool useDeltas;

	public virtual bool IsRevocationEnabled
	{
		get
		{
			return revocationEnabled;
		}
		set
		{
			revocationEnabled = value;
		}
	}

	public virtual bool IsExplicitPolicyRequired
	{
		get
		{
			return explicitPolicyRequired;
		}
		set
		{
			explicitPolicyRequired = value;
		}
	}

	public virtual bool IsAnyPolicyInhibited
	{
		get
		{
			return anyPolicyInhibited;
		}
		set
		{
			anyPolicyInhibited = value;
		}
	}

	public virtual bool IsPolicyMappingInhibited
	{
		get
		{
			return policyMappingInhibited;
		}
		set
		{
			policyMappingInhibited = value;
		}
	}

	public virtual bool IsPolicyQualifiersRejected
	{
		get
		{
			return policyQualifiersRejected;
		}
		set
		{
			policyQualifiersRejected = value;
		}
	}

	public virtual DateTime? Date
	{
		get
		{
			return date;
		}
		set
		{
			date = value;
		}
	}

	public virtual bool IsUseDeltasEnabled
	{
		get
		{
			return useDeltas;
		}
		set
		{
			useDeltas = value;
		}
	}

	public virtual int ValidityModel
	{
		get
		{
			return validityModel;
		}
		set
		{
			validityModel = value;
		}
	}

	public virtual bool IsAdditionalLocationsEnabled => additionalLocationsEnabled;

	public PkixParameters(ISet<TrustAnchor> trustAnchors)
	{
		SetTrustAnchors(trustAnchors);
		initialPolicies = new HashSet<string>();
		m_checkers = new List<PkixCertPathChecker>();
		m_storesAttrCert = new List<IStore<X509V2AttributeCertificate>>();
		m_storesCert = new List<IStore<X509Certificate>>();
		m_storesCrl = new List<IStore<X509Crl>>();
		trustedACIssuers = new HashSet<TrustAnchor>();
		necessaryACAttributes = new HashSet<string>();
		prohibitedACAttributes = new HashSet<string>();
		attrCertCheckers = new HashSet<PkixAttrCertChecker>();
	}

	public virtual ISet<TrustAnchor> GetTrustAnchors()
	{
		return new HashSet<TrustAnchor>(trustAnchors);
	}

	public virtual void SetTrustAnchors(ISet<TrustAnchor> tas)
	{
		if (tas == null)
		{
			throw new ArgumentNullException("tas");
		}
		trustAnchors = new HashSet<TrustAnchor>();
		foreach (TrustAnchor ta in tas)
		{
			if (ta != null)
			{
				trustAnchors.Add(ta);
			}
		}
		if (trustAnchors.Count < 1)
		{
			throw new ArgumentException("non-empty set required", "tas");
		}
	}

	public virtual ISelector<X509V2AttributeCertificate> GetTargetConstraintsAttrCert()
	{
		return (ISelector<X509V2AttributeCertificate>)(m_targetConstraintsAttrCert?.Clone());
	}

	public virtual void SetTargetConstraintsAttrCert(ISelector<X509V2AttributeCertificate> targetConstraintsAttrCert)
	{
		m_targetConstraintsAttrCert = (ISelector<X509V2AttributeCertificate>)(targetConstraintsAttrCert?.Clone());
	}

	public virtual ISelector<X509Certificate> GetTargetConstraintsCert()
	{
		return (ISelector<X509Certificate>)(m_targetConstraintsCert?.Clone());
	}

	public virtual void SetTargetConstraintsCert(ISelector<X509Certificate> targetConstraintsCert)
	{
		m_targetConstraintsCert = (ISelector<X509Certificate>)(targetConstraintsCert?.Clone());
	}

	public virtual ISet<string> GetInitialPolicies()
	{
		if (initialPolicies == null)
		{
			return new HashSet<string>();
		}
		return new HashSet<string>(initialPolicies);
	}

	public virtual void SetInitialPolicies(ISet<string> initialPolicies)
	{
		this.initialPolicies = new HashSet<string>();
		if (initialPolicies == null)
		{
			return;
		}
		foreach (string initialPolicy in initialPolicies)
		{
			if (initialPolicy != null)
			{
				this.initialPolicies.Add(initialPolicy);
			}
		}
	}

	public virtual void SetCertPathCheckers(IList<PkixCertPathChecker> checkers)
	{
		m_checkers = new List<PkixCertPathChecker>();
		if (checkers == null)
		{
			return;
		}
		foreach (PkixCertPathChecker checker in checkers)
		{
			m_checkers.Add((PkixCertPathChecker)checker.Clone());
		}
	}

	public virtual IList<PkixCertPathChecker> GetCertPathCheckers()
	{
		List<PkixCertPathChecker> list = new List<PkixCertPathChecker>(m_checkers.Count);
		foreach (PkixCertPathChecker checker in m_checkers)
		{
			list.Add((PkixCertPathChecker)checker.Clone());
		}
		return list;
	}

	public virtual void AddCertPathChecker(PkixCertPathChecker checker)
	{
		if (checker != null)
		{
			m_checkers.Add((PkixCertPathChecker)checker.Clone());
		}
	}

	public virtual object Clone()
	{
		PkixParameters pkixParameters = new PkixParameters(GetTrustAnchors());
		pkixParameters.SetParams(this);
		return pkixParameters;
	}

	protected virtual void SetParams(PkixParameters parameters)
	{
		Date = parameters.Date;
		SetCertPathCheckers(parameters.GetCertPathCheckers());
		IsAnyPolicyInhibited = parameters.IsAnyPolicyInhibited;
		IsExplicitPolicyRequired = parameters.IsExplicitPolicyRequired;
		IsPolicyMappingInhibited = parameters.IsPolicyMappingInhibited;
		IsRevocationEnabled = parameters.IsRevocationEnabled;
		SetInitialPolicies(parameters.GetInitialPolicies());
		IsPolicyQualifiersRejected = parameters.IsPolicyQualifiersRejected;
		SetTrustAnchors(parameters.GetTrustAnchors());
		m_storesAttrCert = new List<IStore<X509V2AttributeCertificate>>(parameters.m_storesAttrCert);
		m_storesCert = new List<IStore<X509Certificate>>(parameters.m_storesCert);
		m_storesCrl = new List<IStore<X509Crl>>(parameters.m_storesCrl);
		SetTargetConstraintsAttrCert(parameters.GetTargetConstraintsAttrCert());
		SetTargetConstraintsCert(parameters.GetTargetConstraintsCert());
		validityModel = parameters.validityModel;
		useDeltas = parameters.useDeltas;
		additionalLocationsEnabled = parameters.additionalLocationsEnabled;
		trustedACIssuers = new HashSet<TrustAnchor>(parameters.trustedACIssuers);
		prohibitedACAttributes = new HashSet<string>(parameters.prohibitedACAttributes);
		necessaryACAttributes = new HashSet<string>(parameters.necessaryACAttributes);
		attrCertCheckers = new HashSet<PkixAttrCertChecker>(parameters.attrCertCheckers);
	}

	public virtual IList<IStore<X509V2AttributeCertificate>> GetStoresAttrCert()
	{
		return new List<IStore<X509V2AttributeCertificate>>(m_storesAttrCert);
	}

	public virtual IList<IStore<X509Certificate>> GetStoresCert()
	{
		return new List<IStore<X509Certificate>>(m_storesCert);
	}

	public virtual IList<IStore<X509Crl>> GetStoresCrl()
	{
		return new List<IStore<X509Crl>>(m_storesCrl);
	}

	[Obsolete("Use 'SetStoresAttrCert' instead")]
	public virtual void SetAttrStoresCert(IList<IStore<X509V2AttributeCertificate>> storesAttrCert)
	{
		SetStoresAttrCert(storesAttrCert);
	}

	public virtual void SetStoresAttrCert(IList<IStore<X509V2AttributeCertificate>> storesAttrCert)
	{
		if (storesAttrCert == null)
		{
			m_storesAttrCert = new List<IStore<X509V2AttributeCertificate>>();
		}
		else
		{
			m_storesAttrCert = new List<IStore<X509V2AttributeCertificate>>(storesAttrCert);
		}
	}

	public virtual void SetStoresCert(IList<IStore<X509Certificate>> storesCert)
	{
		if (storesCert == null)
		{
			m_storesCert = new List<IStore<X509Certificate>>();
		}
		else
		{
			m_storesCert = new List<IStore<X509Certificate>>(storesCert);
		}
	}

	public virtual void SetStoresCrl(IList<IStore<X509Crl>> storesCrl)
	{
		if (storesCrl == null)
		{
			m_storesCrl = new List<IStore<X509Crl>>();
		}
		else
		{
			m_storesCrl = new List<IStore<X509Crl>>(storesCrl);
		}
	}

	public virtual void AddStoreAttrCert(IStore<X509V2AttributeCertificate> storeAttrCert)
	{
		if (storeAttrCert != null)
		{
			m_storesAttrCert.Add(storeAttrCert);
		}
	}

	public virtual void AddStoreCert(IStore<X509Certificate> storeCert)
	{
		if (storeCert != null)
		{
			m_storesCert.Add(storeCert);
		}
	}

	public virtual void AddStoreCrl(IStore<X509Crl> storeCrl)
	{
		if (storeCrl != null)
		{
			m_storesCrl.Add(storeCrl);
		}
	}

	public virtual void SetAdditionalLocationsEnabled(bool enabled)
	{
		additionalLocationsEnabled = enabled;
	}

	public virtual ISet<TrustAnchor> GetTrustedACIssuers()
	{
		return new HashSet<TrustAnchor>(trustedACIssuers);
	}

	public virtual void SetTrustedACIssuers(ISet<TrustAnchor> trustedACIssuers)
	{
		if (trustedACIssuers == null)
		{
			this.trustedACIssuers = new HashSet<TrustAnchor>();
		}
		else
		{
			this.trustedACIssuers = new HashSet<TrustAnchor>(trustedACIssuers);
		}
	}

	public virtual ISet<string> GetNecessaryACAttributes()
	{
		return new HashSet<string>(necessaryACAttributes);
	}

	public virtual void SetNecessaryACAttributes(ISet<string> necessaryACAttributes)
	{
		if (necessaryACAttributes == null)
		{
			this.necessaryACAttributes = new HashSet<string>();
		}
		else
		{
			this.necessaryACAttributes = new HashSet<string>(necessaryACAttributes);
		}
	}

	public virtual ISet<string> GetProhibitedACAttributes()
	{
		return new HashSet<string>(prohibitedACAttributes);
	}

	public virtual void SetProhibitedACAttributes(ISet<string> prohibitedACAttributes)
	{
		if (prohibitedACAttributes == null)
		{
			this.prohibitedACAttributes = new HashSet<string>();
		}
		else
		{
			this.prohibitedACAttributes = new HashSet<string>(prohibitedACAttributes);
		}
	}

	public virtual ISet<PkixAttrCertChecker> GetAttrCertCheckers()
	{
		return new HashSet<PkixAttrCertChecker>(attrCertCheckers);
	}

	public virtual void SetAttrCertCheckers(ISet<PkixAttrCertChecker> attrCertCheckers)
	{
		if (attrCertCheckers == null)
		{
			this.attrCertCheckers = new HashSet<PkixAttrCertChecker>();
		}
		else
		{
			this.attrCertCheckers = new HashSet<PkixAttrCertChecker>(attrCertCheckers);
		}
	}
}
