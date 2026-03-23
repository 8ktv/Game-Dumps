using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;
using Mirror.BouncyCastle.X509.Store;

namespace Mirror.BouncyCastle.Pkix;

public class PkixCrlUtilities
{
	public virtual ISet<X509Crl> FindCrls(X509CrlStoreSelector crlSelector, PkixParameters paramsPkix)
	{
		return FindCrls((ISelector<X509Crl>)crlSelector, paramsPkix);
	}

	public virtual ISet<X509Crl> FindCrls(ISelector<X509Crl> crlSelector, PkixParameters paramsPkix)
	{
		try
		{
			return FindCrls(crlSelector, paramsPkix.GetStoresCrl());
		}
		catch (Exception innerException)
		{
			throw new Exception("Exception obtaining complete CRLs.", innerException);
		}
	}

	public virtual ISet<X509Crl> FindCrls(X509CrlStoreSelector crlSelector, PkixParameters paramsPkix, DateTime currentDate)
	{
		return FindCrls((ISelector<X509Crl>)crlSelector, paramsPkix, currentDate);
	}

	public virtual ISet<X509Crl> FindCrls(ISelector<X509Crl> crlSelector, PkixParameters paramsPkix, DateTime currentDate)
	{
		ISet<X509Crl> set = FindCrls(crlSelector, paramsPkix);
		HashSet<X509Crl> hashSet = new HashSet<X509Crl>();
		DateTime value = currentDate;
		if (paramsPkix.Date.HasValue)
		{
			value = paramsPkix.Date.Value;
		}
		X509Certificate x509Certificate = null;
		if (crlSelector is ICheckingCertificate checkingCertificate)
		{
			x509Certificate = checkingCertificate.CertificateChecking;
		}
		foreach (X509Crl item in set)
		{
			DateTime? nextUpdate = item.NextUpdate;
			if ((!nextUpdate.HasValue || nextUpdate.Value.CompareTo(value) > 0) && (x509Certificate == null || item.ThisUpdate.CompareTo(x509Certificate.NotAfter) < 0))
			{
				hashSet.Add(item);
			}
		}
		return hashSet;
	}

	private HashSet<X509Crl> FindCrls(ISelector<X509Crl> crlSelector, IEnumerable<IStore<X509Crl>> crlStores)
	{
		HashSet<X509Crl> hashSet = new HashSet<X509Crl>();
		Exception ex = null;
		bool flag = false;
		foreach (IStore<X509Crl> crlStore in crlStores)
		{
			try
			{
				hashSet.UnionWith(crlStore.EnumerateMatches(crlSelector));
				flag = true;
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
		}
		if (!flag && ex != null)
		{
			throw new Exception("Exception searching in X.509 CRL store.", ex);
		}
		return hashSet;
	}
}
