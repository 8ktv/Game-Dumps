using System;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.X509.Store;

public class X509CertPairStoreSelector : ISelector<X509CertificatePair>, ICloneable
{
	private X509CertificatePair certPair;

	private X509CertStoreSelector forwardSelector;

	private X509CertStoreSelector reverseSelector;

	public X509CertificatePair CertPair
	{
		get
		{
			return certPair;
		}
		set
		{
			certPair = value;
		}
	}

	public X509CertStoreSelector ForwardSelector
	{
		get
		{
			return CloneSelector(forwardSelector);
		}
		set
		{
			forwardSelector = CloneSelector(value);
		}
	}

	public X509CertStoreSelector ReverseSelector
	{
		get
		{
			return CloneSelector(reverseSelector);
		}
		set
		{
			reverseSelector = CloneSelector(value);
		}
	}

	private static X509CertStoreSelector CloneSelector(X509CertStoreSelector s)
	{
		if (s != null)
		{
			return (X509CertStoreSelector)s.Clone();
		}
		return null;
	}

	public X509CertPairStoreSelector()
	{
	}

	private X509CertPairStoreSelector(X509CertPairStoreSelector o)
	{
		certPair = o.CertPair;
		forwardSelector = o.ForwardSelector;
		reverseSelector = o.ReverseSelector;
	}

	public bool Match(X509CertificatePair pair)
	{
		if (pair == null)
		{
			return false;
		}
		if (certPair != null && !certPair.Equals(pair))
		{
			return false;
		}
		if (forwardSelector != null && !forwardSelector.Match(pair.Forward))
		{
			return false;
		}
		if (reverseSelector != null && !reverseSelector.Match(pair.Reverse))
		{
			return false;
		}
		return true;
	}

	public object Clone()
	{
		return new X509CertPairStoreSelector(this);
	}
}
