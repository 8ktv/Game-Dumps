using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509.Store;

public class X509CrlStoreSelector : ISelector<X509Crl>, ICloneable, ICheckingCertificate
{
	private X509Certificate certificateChecking;

	private DateTime? dateAndTime;

	private IList<X509Name> issuers;

	private BigInteger maxCrlNumber;

	private BigInteger minCrlNumber;

	private X509V2AttributeCertificate attrCertChecking;

	private bool completeCrlEnabled;

	private bool deltaCrlIndicatorEnabled;

	private byte[] issuingDistributionPoint;

	private bool issuingDistributionPointEnabled;

	private BigInteger maxBaseCrlNumber;

	public X509Certificate CertificateChecking
	{
		get
		{
			return certificateChecking;
		}
		set
		{
			certificateChecking = value;
		}
	}

	public DateTime? DateAndTime
	{
		get
		{
			return dateAndTime;
		}
		set
		{
			dateAndTime = value;
		}
	}

	public IList<X509Name> Issuers
	{
		get
		{
			return new List<X509Name>(issuers);
		}
		set
		{
			issuers = new List<X509Name>(value);
		}
	}

	public BigInteger MaxCrlNumber
	{
		get
		{
			return maxCrlNumber;
		}
		set
		{
			maxCrlNumber = value;
		}
	}

	public BigInteger MinCrlNumber
	{
		get
		{
			return minCrlNumber;
		}
		set
		{
			minCrlNumber = value;
		}
	}

	public X509V2AttributeCertificate AttrCertChecking
	{
		get
		{
			return attrCertChecking;
		}
		set
		{
			attrCertChecking = value;
		}
	}

	public bool CompleteCrlEnabled
	{
		get
		{
			return completeCrlEnabled;
		}
		set
		{
			completeCrlEnabled = value;
		}
	}

	public bool DeltaCrlIndicatorEnabled
	{
		get
		{
			return deltaCrlIndicatorEnabled;
		}
		set
		{
			deltaCrlIndicatorEnabled = value;
		}
	}

	public byte[] IssuingDistributionPoint
	{
		get
		{
			return Arrays.Clone(issuingDistributionPoint);
		}
		set
		{
			issuingDistributionPoint = Arrays.Clone(value);
		}
	}

	public bool IssuingDistributionPointEnabled
	{
		get
		{
			return issuingDistributionPointEnabled;
		}
		set
		{
			issuingDistributionPointEnabled = value;
		}
	}

	public BigInteger MaxBaseCrlNumber
	{
		get
		{
			return maxBaseCrlNumber;
		}
		set
		{
			maxBaseCrlNumber = value;
		}
	}

	public X509CrlStoreSelector()
	{
	}

	public X509CrlStoreSelector(X509CrlStoreSelector o)
	{
		certificateChecking = o.CertificateChecking;
		dateAndTime = o.DateAndTime;
		issuers = o.Issuers;
		maxCrlNumber = o.MaxCrlNumber;
		minCrlNumber = o.MinCrlNumber;
		deltaCrlIndicatorEnabled = o.DeltaCrlIndicatorEnabled;
		completeCrlEnabled = o.CompleteCrlEnabled;
		maxBaseCrlNumber = o.MaxBaseCrlNumber;
		attrCertChecking = o.AttrCertChecking;
		issuingDistributionPointEnabled = o.IssuingDistributionPointEnabled;
		issuingDistributionPoint = o.IssuingDistributionPoint;
	}

	public virtual object Clone()
	{
		return new X509CrlStoreSelector(this);
	}

	public virtual bool Match(X509Crl c)
	{
		if (c == null)
		{
			return false;
		}
		if (dateAndTime.HasValue)
		{
			DateTime value = dateAndTime.Value;
			DateTime thisUpdate = c.ThisUpdate;
			DateTime? nextUpdate = c.NextUpdate;
			if (value.CompareTo(thisUpdate) < 0 || !nextUpdate.HasValue || value.CompareTo(nextUpdate.Value) >= 0)
			{
				return false;
			}
		}
		if (issuers != null)
		{
			X509Name issuerDN = c.IssuerDN;
			bool flag = false;
			foreach (X509Name issuer in issuers)
			{
				if (issuer.Equivalent(issuerDN, inOrder: true))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (maxCrlNumber != null || minCrlNumber != null)
		{
			Asn1OctetString extensionValue = c.GetExtensionValue(X509Extensions.CrlNumber);
			if (extensionValue == null)
			{
				return false;
			}
			BigInteger positiveValue = DerInteger.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).PositiveValue;
			if (maxCrlNumber != null && positiveValue.CompareTo(maxCrlNumber) > 0)
			{
				return false;
			}
			if (minCrlNumber != null && positiveValue.CompareTo(minCrlNumber) < 0)
			{
				return false;
			}
		}
		DerInteger derInteger = null;
		try
		{
			Asn1OctetString extensionValue2 = c.GetExtensionValue(X509Extensions.DeltaCrlIndicator);
			if (extensionValue2 != null)
			{
				derInteger = DerInteger.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
			}
		}
		catch (Exception)
		{
			return false;
		}
		if (derInteger == null)
		{
			if (DeltaCrlIndicatorEnabled)
			{
				return false;
			}
		}
		else
		{
			if (CompleteCrlEnabled)
			{
				return false;
			}
			if (maxBaseCrlNumber != null && derInteger.PositiveValue.CompareTo(maxBaseCrlNumber) > 0)
			{
				return false;
			}
		}
		if (issuingDistributionPointEnabled)
		{
			Asn1OctetString extensionValue3 = c.GetExtensionValue(X509Extensions.IssuingDistributionPoint);
			if (issuingDistributionPoint == null)
			{
				if (extensionValue3 != null)
				{
					return false;
				}
			}
			else if (!Arrays.AreEqual(extensionValue3.GetOctets(), issuingDistributionPoint))
			{
				return false;
			}
		}
		return true;
	}
}
