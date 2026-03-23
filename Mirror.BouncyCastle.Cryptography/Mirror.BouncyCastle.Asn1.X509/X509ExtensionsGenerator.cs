using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.X509;

public class X509ExtensionsGenerator
{
	private Dictionary<DerObjectIdentifier, X509Extension> m_extensions = new Dictionary<DerObjectIdentifier, X509Extension>();

	private List<DerObjectIdentifier> m_ordering = new List<DerObjectIdentifier>();

	private static readonly HashSet<DerObjectIdentifier> m_dupsAllowed = new HashSet<DerObjectIdentifier>
	{
		X509Extensions.SubjectAlternativeName,
		X509Extensions.IssuerAlternativeName,
		X509Extensions.SubjectDirectoryAttributes,
		X509Extensions.CertificateIssuer
	};

	public bool IsEmpty => m_ordering.Count < 1;

	public void Reset()
	{
		m_extensions = new Dictionary<DerObjectIdentifier, X509Extension>();
		m_ordering = new List<DerObjectIdentifier>();
	}

	public void AddExtension(DerObjectIdentifier oid, bool critical, Asn1Encodable extValue)
	{
		byte[] derEncoded;
		try
		{
			derEncoded = extValue.GetDerEncoded();
		}
		catch (Exception ex)
		{
			throw new ArgumentException("error encoding value: " + ex);
		}
		AddExtension(oid, critical, derEncoded);
	}

	public void AddExtension(DerObjectIdentifier oid, bool critical, byte[] extValue)
	{
		if (!m_extensions.TryGetValue(oid, out var value))
		{
			m_ordering.Add(oid);
			m_extensions.Add(oid, new X509Extension(critical, new DerOctetString(extValue)));
			return;
		}
		if (!m_dupsAllowed.Contains(oid))
		{
			throw new ArgumentException("extension " + oid?.ToString() + " already added");
		}
		Asn1Sequence instance = Asn1Sequence.GetInstance(value.Value.GetOctets());
		Asn1Sequence instance2 = Asn1Sequence.GetInstance(extValue);
		DerSequence derSequence = DerSequence.Concatenate(instance, instance2);
		m_extensions[oid] = new X509Extension(value.IsCritical || critical, new DerOctetString(derSequence.GetEncoded("DER")));
	}

	public void AddExtensions(X509Extensions extensions)
	{
		foreach (DerObjectIdentifier extensionOid in extensions.ExtensionOids)
		{
			X509Extension extension = extensions.GetExtension(extensionOid);
			AddExtension(extensionOid, extension.critical, extension.Value.GetOctets());
		}
	}

	public X509Extensions Generate()
	{
		return new X509Extensions(m_ordering, m_extensions);
	}

	internal void AddExtension(DerObjectIdentifier oid, X509Extension x509Extension)
	{
		if (m_extensions.ContainsKey(oid))
		{
			throw new ArgumentException("extension " + oid?.ToString() + " already added");
		}
		m_ordering.Add(oid);
		m_extensions.Add(oid, x509Extension);
	}
}
