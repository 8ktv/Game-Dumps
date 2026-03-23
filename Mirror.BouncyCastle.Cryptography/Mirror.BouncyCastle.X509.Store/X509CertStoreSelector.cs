using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509.Store;

public class X509CertStoreSelector : ISelector<X509Certificate>, ICloneable
{
	private byte[] authorityKeyIdentifier;

	private int basicConstraints = -1;

	private X509Certificate certificate;

	private DateTime? certificateValid;

	private ISet<DerObjectIdentifier> extendedKeyUsage;

	private bool ignoreX509NameOrdering;

	private X509Name issuer;

	private bool[] keyUsage;

	private ISet<DerObjectIdentifier> policy;

	private DateTime? privateKeyValid;

	private BigInteger serialNumber;

	private X509Name subject;

	private byte[] subjectKeyIdentifier;

	private SubjectPublicKeyInfo subjectPublicKey;

	private DerObjectIdentifier subjectPublicKeyAlgID;

	public byte[] AuthorityKeyIdentifier
	{
		get
		{
			return Arrays.Clone(authorityKeyIdentifier);
		}
		set
		{
			authorityKeyIdentifier = Arrays.Clone(value);
		}
	}

	public int BasicConstraints
	{
		get
		{
			return basicConstraints;
		}
		set
		{
			if (value < -2)
			{
				throw new ArgumentException("value can't be less than -2", "value");
			}
			basicConstraints = value;
		}
	}

	public X509Certificate Certificate
	{
		get
		{
			return certificate;
		}
		set
		{
			certificate = value;
		}
	}

	public DateTime? CertificateValid
	{
		get
		{
			return certificateValid;
		}
		set
		{
			certificateValid = value;
		}
	}

	public ISet<DerObjectIdentifier> ExtendedKeyUsage
	{
		get
		{
			return CopySet(extendedKeyUsage);
		}
		set
		{
			extendedKeyUsage = CopySet(value);
		}
	}

	public bool IgnoreX509NameOrdering
	{
		get
		{
			return ignoreX509NameOrdering;
		}
		set
		{
			ignoreX509NameOrdering = value;
		}
	}

	public X509Name Issuer
	{
		get
		{
			return issuer;
		}
		set
		{
			issuer = value;
		}
	}

	public bool[] KeyUsage
	{
		get
		{
			return Arrays.Clone(keyUsage);
		}
		set
		{
			keyUsage = Arrays.Clone(value);
		}
	}

	public ISet<DerObjectIdentifier> Policy
	{
		get
		{
			return CopySet(policy);
		}
		set
		{
			policy = CopySet(value);
		}
	}

	public DateTime? PrivateKeyValid
	{
		get
		{
			return privateKeyValid;
		}
		set
		{
			privateKeyValid = value;
		}
	}

	public BigInteger SerialNumber
	{
		get
		{
			return serialNumber;
		}
		set
		{
			serialNumber = value;
		}
	}

	public X509Name Subject
	{
		get
		{
			return subject;
		}
		set
		{
			subject = value;
		}
	}

	public byte[] SubjectKeyIdentifier
	{
		get
		{
			return Arrays.Clone(subjectKeyIdentifier);
		}
		set
		{
			subjectKeyIdentifier = Arrays.Clone(value);
		}
	}

	public SubjectPublicKeyInfo SubjectPublicKey
	{
		get
		{
			return subjectPublicKey;
		}
		set
		{
			subjectPublicKey = value;
		}
	}

	public DerObjectIdentifier SubjectPublicKeyAlgID
	{
		get
		{
			return subjectPublicKeyAlgID;
		}
		set
		{
			subjectPublicKeyAlgID = value;
		}
	}

	public X509CertStoreSelector()
	{
	}

	public X509CertStoreSelector(X509CertStoreSelector o)
	{
		authorityKeyIdentifier = o.AuthorityKeyIdentifier;
		basicConstraints = o.BasicConstraints;
		certificate = o.Certificate;
		certificateValid = o.CertificateValid;
		extendedKeyUsage = o.ExtendedKeyUsage;
		ignoreX509NameOrdering = o.IgnoreX509NameOrdering;
		issuer = o.Issuer;
		keyUsage = o.KeyUsage;
		policy = o.Policy;
		privateKeyValid = o.PrivateKeyValid;
		serialNumber = o.SerialNumber;
		subject = o.Subject;
		subjectKeyIdentifier = o.SubjectKeyIdentifier;
		subjectPublicKey = o.SubjectPublicKey;
		subjectPublicKeyAlgID = o.SubjectPublicKeyAlgID;
	}

	public virtual object Clone()
	{
		return new X509CertStoreSelector(this);
	}

	public virtual bool Match(X509Certificate c)
	{
		if (c == null)
		{
			return false;
		}
		if (!MatchExtension(authorityKeyIdentifier, c, X509Extensions.AuthorityKeyIdentifier))
		{
			return false;
		}
		if (basicConstraints != -1)
		{
			int num = c.GetBasicConstraints();
			if (basicConstraints == -2)
			{
				if (num != -1)
				{
					return false;
				}
			}
			else if (num < basicConstraints)
			{
				return false;
			}
		}
		if (certificate != null && !certificate.Equals(c))
		{
			return false;
		}
		if (certificateValid.HasValue && !c.IsValid(certificateValid.Value))
		{
			return false;
		}
		if (extendedKeyUsage != null)
		{
			IList<DerObjectIdentifier> list = c.GetExtendedKeyUsage();
			if (list != null)
			{
				foreach (DerObjectIdentifier item in extendedKeyUsage)
				{
					if (!list.Contains(item))
					{
						return false;
					}
				}
			}
		}
		if (issuer != null && !issuer.Equivalent(c.IssuerDN, !ignoreX509NameOrdering))
		{
			return false;
		}
		if (keyUsage != null)
		{
			bool[] array = c.GetKeyUsage();
			if (array != null)
			{
				for (int i = 0; i < 9; i++)
				{
					if (keyUsage[i] && !array[i])
					{
						return false;
					}
				}
			}
		}
		if (policy != null)
		{
			Asn1OctetString extensionValue = c.GetExtensionValue(X509Extensions.CertificatePolicies);
			if (extensionValue == null)
			{
				return false;
			}
			Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
			if (policy.Count < 1 && instance.Count < 1)
			{
				return false;
			}
			bool flag = false;
			foreach (PolicyInformation item2 in instance)
			{
				if (policy.Contains(item2.PolicyIdentifier))
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
		if (privateKeyValid.HasValue)
		{
			Asn1OctetString extensionValue2 = c.GetExtensionValue(X509Extensions.PrivateKeyUsagePeriod);
			if (extensionValue2 == null)
			{
				return false;
			}
			PrivateKeyUsagePeriod instance2 = PrivateKeyUsagePeriod.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
			DateTime value = privateKeyValid.Value;
			DateTime value2 = instance2.NotAfter.ToDateTime();
			DateTime value3 = instance2.NotBefore.ToDateTime();
			if (value.CompareTo(value2) > 0 || value.CompareTo(value3) < 0)
			{
				return false;
			}
		}
		if (serialNumber != null && !serialNumber.Equals(c.SerialNumber))
		{
			return false;
		}
		if (subject != null && !subject.Equivalent(c.SubjectDN, !ignoreX509NameOrdering))
		{
			return false;
		}
		if (!MatchExtension(subjectKeyIdentifier, c, X509Extensions.SubjectKeyIdentifier))
		{
			return false;
		}
		SubjectPublicKeyInfo subjectPublicKeyInfo = c.SubjectPublicKeyInfo;
		if (subjectPublicKey != null && !subjectPublicKey.Equals(subjectPublicKeyInfo))
		{
			return false;
		}
		if (subjectPublicKeyAlgID != null && !subjectPublicKeyAlgID.Equals(subjectPublicKeyInfo.Algorithm))
		{
			return false;
		}
		return true;
	}

	protected internal int GetHashCodeOfSubjectKeyIdentifier()
	{
		return Arrays.GetHashCode(subjectKeyIdentifier);
	}

	protected internal bool MatchesIssuer(X509CertStoreSelector other)
	{
		return IssuersMatch(issuer, other.issuer);
	}

	protected internal bool MatchesSerialNumber(X509CertStoreSelector other)
	{
		return object.Equals(serialNumber, other.serialNumber);
	}

	protected internal bool MatchesSubjectKeyIdentifier(X509CertStoreSelector other)
	{
		return Arrays.AreEqual(subjectKeyIdentifier, other.subjectKeyIdentifier);
	}

	private static bool IssuersMatch(X509Name a, X509Name b)
	{
		return a?.Equivalent(b, inOrder: true) ?? (b == null);
	}

	private static ISet<T> CopySet<T>(ISet<T> s)
	{
		if (s != null)
		{
			return new HashSet<T>(s);
		}
		return null;
	}

	private static bool MatchExtension(byte[] b, X509Certificate c, DerObjectIdentifier oid)
	{
		if (b == null)
		{
			return true;
		}
		Asn1OctetString extensionValue = c.GetExtensionValue(oid);
		if (extensionValue == null)
		{
			return false;
		}
		return Arrays.AreEqual(b, extensionValue.GetOctets());
	}
}
