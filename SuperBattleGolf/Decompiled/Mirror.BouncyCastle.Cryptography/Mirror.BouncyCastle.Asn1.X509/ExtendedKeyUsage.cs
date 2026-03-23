using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.X509;

public class ExtendedKeyUsage : Asn1Encodable
{
	internal readonly HashSet<DerObjectIdentifier> m_usageTable = new HashSet<DerObjectIdentifier>();

	internal readonly Asn1Sequence seq;

	public int Count => m_usageTable.Count;

	public static ExtendedKeyUsage GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public static ExtendedKeyUsage GetInstance(object obj)
	{
		if (obj is ExtendedKeyUsage)
		{
			return (ExtendedKeyUsage)obj;
		}
		if (obj is X509Extension)
		{
			return GetInstance(X509Extension.ConvertValueToObject((X509Extension)obj));
		}
		if (obj == null)
		{
			return null;
		}
		return new ExtendedKeyUsage(Asn1Sequence.GetInstance(obj));
	}

	public static ExtendedKeyUsage FromExtensions(X509Extensions extensions)
	{
		return GetInstance(X509Extensions.GetExtensionParsedValue(extensions, X509Extensions.ExtendedKeyUsage));
	}

	private ExtendedKeyUsage(Asn1Sequence seq)
	{
		this.seq = seq;
		foreach (Asn1Encodable item in seq)
		{
			DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(item);
			m_usageTable.Add(instance);
		}
	}

	public ExtendedKeyUsage(params KeyPurposeID[] usages)
	{
		seq = new DerSequence(usages);
		foreach (KeyPurposeID item in usages)
		{
			m_usageTable.Add(item);
		}
	}

	public ExtendedKeyUsage(IEnumerable<DerObjectIdentifier> usages)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		foreach (DerObjectIdentifier usage in usages)
		{
			asn1EncodableVector.Add(usage);
			m_usageTable.Add(usage);
		}
		seq = new DerSequence(asn1EncodableVector);
	}

	public bool HasKeyPurposeId(KeyPurposeID keyPurposeId)
	{
		return m_usageTable.Contains(keyPurposeId);
	}

	public IList<DerObjectIdentifier> GetAllUsages()
	{
		return new List<DerObjectIdentifier>(m_usageTable);
	}

	public override Asn1Object ToAsn1Object()
	{
		return seq;
	}
}
