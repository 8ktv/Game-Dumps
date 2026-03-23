using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Asn1.X509;

public class X509Extensions : Asn1Encodable
{
	public static readonly DerObjectIdentifier SubjectDirectoryAttributes = new DerObjectIdentifier("2.5.29.9");

	public static readonly DerObjectIdentifier SubjectKeyIdentifier = new DerObjectIdentifier("2.5.29.14");

	public static readonly DerObjectIdentifier KeyUsage = new DerObjectIdentifier("2.5.29.15");

	public static readonly DerObjectIdentifier PrivateKeyUsagePeriod = new DerObjectIdentifier("2.5.29.16");

	public static readonly DerObjectIdentifier SubjectAlternativeName = new DerObjectIdentifier("2.5.29.17");

	public static readonly DerObjectIdentifier IssuerAlternativeName = new DerObjectIdentifier("2.5.29.18");

	public static readonly DerObjectIdentifier BasicConstraints = new DerObjectIdentifier("2.5.29.19");

	public static readonly DerObjectIdentifier CrlNumber = new DerObjectIdentifier("2.5.29.20");

	public static readonly DerObjectIdentifier ReasonCode = new DerObjectIdentifier("2.5.29.21");

	public static readonly DerObjectIdentifier InstructionCode = new DerObjectIdentifier("2.5.29.23");

	public static readonly DerObjectIdentifier InvalidityDate = new DerObjectIdentifier("2.5.29.24");

	public static readonly DerObjectIdentifier DeltaCrlIndicator = new DerObjectIdentifier("2.5.29.27");

	public static readonly DerObjectIdentifier IssuingDistributionPoint = new DerObjectIdentifier("2.5.29.28");

	public static readonly DerObjectIdentifier CertificateIssuer = new DerObjectIdentifier("2.5.29.29");

	public static readonly DerObjectIdentifier NameConstraints = new DerObjectIdentifier("2.5.29.30");

	public static readonly DerObjectIdentifier CrlDistributionPoints = new DerObjectIdentifier("2.5.29.31");

	public static readonly DerObjectIdentifier CertificatePolicies = new DerObjectIdentifier("2.5.29.32");

	public static readonly DerObjectIdentifier PolicyMappings = new DerObjectIdentifier("2.5.29.33");

	public static readonly DerObjectIdentifier AuthorityKeyIdentifier = new DerObjectIdentifier("2.5.29.35");

	public static readonly DerObjectIdentifier PolicyConstraints = new DerObjectIdentifier("2.5.29.36");

	public static readonly DerObjectIdentifier ExtendedKeyUsage = new DerObjectIdentifier("2.5.29.37");

	public static readonly DerObjectIdentifier FreshestCrl = new DerObjectIdentifier("2.5.29.46");

	public static readonly DerObjectIdentifier InhibitAnyPolicy = new DerObjectIdentifier("2.5.29.54");

	public static readonly DerObjectIdentifier AuthorityInfoAccess = new DerObjectIdentifier("1.3.6.1.5.5.7.1.1");

	public static readonly DerObjectIdentifier SubjectInfoAccess = new DerObjectIdentifier("1.3.6.1.5.5.7.1.11");

	public static readonly DerObjectIdentifier LogoType = new DerObjectIdentifier("1.3.6.1.5.5.7.1.12");

	public static readonly DerObjectIdentifier BiometricInfo = new DerObjectIdentifier("1.3.6.1.5.5.7.1.2");

	public static readonly DerObjectIdentifier QCStatements = new DerObjectIdentifier("1.3.6.1.5.5.7.1.3");

	public static readonly DerObjectIdentifier AuditIdentity = new DerObjectIdentifier("1.3.6.1.5.5.7.1.4");

	public static readonly DerObjectIdentifier NoRevAvail = new DerObjectIdentifier("2.5.29.56");

	public static readonly DerObjectIdentifier TargetInformation = new DerObjectIdentifier("2.5.29.55");

	public static readonly DerObjectIdentifier ExpiredCertsOnCrl = new DerObjectIdentifier("2.5.29.60");

	public static readonly DerObjectIdentifier SubjectAltPublicKeyInfo = new DerObjectIdentifier("2.5.29.72");

	public static readonly DerObjectIdentifier AltSignatureAlgorithm = new DerObjectIdentifier("2.5.29.73");

	public static readonly DerObjectIdentifier AltSignatureValue = new DerObjectIdentifier("2.5.29.74");

	private readonly Dictionary<DerObjectIdentifier, X509Extension> m_extensions = new Dictionary<DerObjectIdentifier, X509Extension>();

	private readonly List<DerObjectIdentifier> m_ordering;

	public IEnumerable<DerObjectIdentifier> ExtensionOids => CollectionUtilities.Proxy(m_ordering);

	public static X509Extension GetExtension(X509Extensions extensions, DerObjectIdentifier oid)
	{
		return extensions?.GetExtension(oid);
	}

	public static Asn1Object GetExtensionParsedValue(X509Extensions extensions, DerObjectIdentifier oid)
	{
		return extensions?.GetExtensionParsedValue(oid);
	}

	public static Asn1OctetString GetExtensionValue(X509Extensions extensions, DerObjectIdentifier oid)
	{
		return extensions?.GetExtensionValue(oid);
	}

	public static X509Extensions GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return GetInstance(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public static X509Extensions GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is X509Extensions result)
		{
			return result;
		}
		if (obj is Asn1Sequence seq)
		{
			return new X509Extensions(seq);
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return GetInstance(Asn1Utilities.CheckContextTagClass(taggedObject).GetBaseObject().ToAsn1Object());
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private X509Extensions(Asn1Sequence seq)
	{
		m_ordering = new List<DerObjectIdentifier>();
		foreach (Asn1Encodable item in seq)
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(item);
			if (instance.Count < 2 || instance.Count > 3)
			{
				throw new ArgumentException("Bad sequence size: " + instance.Count);
			}
			DerObjectIdentifier instance2 = DerObjectIdentifier.GetInstance(instance[0]);
			bool critical = instance.Count == 3 && DerBoolean.GetInstance(instance[1]).IsTrue;
			Asn1OctetString instance3 = Asn1OctetString.GetInstance(instance[instance.Count - 1]);
			if (m_extensions.ContainsKey(instance2))
			{
				throw new ArgumentException("repeated extension found: " + instance2);
			}
			m_extensions.Add(instance2, new X509Extension(critical, instance3));
			m_ordering.Add(instance2);
		}
	}

	public X509Extensions(IDictionary<DerObjectIdentifier, X509Extension> extensions)
		: this(null, extensions)
	{
	}

	public X509Extensions(IList<DerObjectIdentifier> ordering, IDictionary<DerObjectIdentifier, X509Extension> extensions)
	{
		if (ordering == null)
		{
			m_ordering = new List<DerObjectIdentifier>(extensions.Keys);
		}
		else
		{
			m_ordering = new List<DerObjectIdentifier>(ordering);
		}
		foreach (DerObjectIdentifier item in m_ordering)
		{
			m_extensions.Add(item, extensions[item]);
		}
	}

	public X509Extensions(IList<DerObjectIdentifier> oids, IList<X509Extension> values)
	{
		m_ordering = new List<DerObjectIdentifier>(oids);
		int num = 0;
		foreach (DerObjectIdentifier item in m_ordering)
		{
			m_extensions.Add(item, values[num++]);
		}
	}

	public X509Extension GetExtension(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(m_extensions, oid);
	}

	public Asn1Object GetExtensionParsedValue(DerObjectIdentifier oid)
	{
		return GetExtension(oid)?.GetParsedValue();
	}

	public Asn1OctetString GetExtensionValue(DerObjectIdentifier oid)
	{
		return GetExtension(oid)?.Value;
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_ordering.Count);
		foreach (DerObjectIdentifier item in m_ordering)
		{
			X509Extension x509Extension = m_extensions[item];
			if (x509Extension.IsCritical)
			{
				asn1EncodableVector.Add(new DerSequence(item, DerBoolean.True, x509Extension.Value));
			}
			else
			{
				asn1EncodableVector.Add(new DerSequence(item, x509Extension.Value));
			}
		}
		return new DerSequence(asn1EncodableVector);
	}

	public bool Equivalent(X509Extensions other)
	{
		if (m_extensions.Count != other.m_extensions.Count)
		{
			return false;
		}
		foreach (KeyValuePair<DerObjectIdentifier, X509Extension> extension in m_extensions)
		{
			if (!extension.Value.Equals(other.GetExtension(extension.Key)))
			{
				return false;
			}
		}
		return true;
	}

	public DerObjectIdentifier[] GetExtensionOids()
	{
		return m_ordering.ToArray();
	}

	public DerObjectIdentifier[] GetNonCriticalExtensionOids()
	{
		return GetExtensionOids(isCritical: false);
	}

	public DerObjectIdentifier[] GetCriticalExtensionOids()
	{
		return GetExtensionOids(isCritical: true);
	}

	private DerObjectIdentifier[] GetExtensionOids(bool isCritical)
	{
		List<DerObjectIdentifier> list = new List<DerObjectIdentifier>();
		foreach (DerObjectIdentifier item in m_ordering)
		{
			if (m_extensions[item].IsCritical == isCritical)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}
}
