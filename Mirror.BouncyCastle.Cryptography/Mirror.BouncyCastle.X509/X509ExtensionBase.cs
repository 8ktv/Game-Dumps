using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.X509;

public abstract class X509ExtensionBase : IX509Extension
{
	protected abstract X509Extensions GetX509Extensions();

	protected virtual ISet<string> GetExtensionOids(bool critical)
	{
		X509Extensions x509Extensions = GetX509Extensions();
		if (x509Extensions == null)
		{
			return null;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (DerObjectIdentifier extensionOid in x509Extensions.ExtensionOids)
		{
			if (x509Extensions.GetExtension(extensionOid).IsCritical == critical)
			{
				hashSet.Add(extensionOid.Id);
			}
		}
		return hashSet;
	}

	public virtual ISet<string> GetNonCriticalExtensionOids()
	{
		return GetExtensionOids(critical: false);
	}

	public virtual ISet<string> GetCriticalExtensionOids()
	{
		return GetExtensionOids(critical: true);
	}

	public virtual X509Extension GetExtension(DerObjectIdentifier oid)
	{
		return GetX509Extensions()?.GetExtension(oid);
	}

	public virtual Asn1Object GetExtensionParsedValue(DerObjectIdentifier oid)
	{
		return GetX509Extensions()?.GetExtensionParsedValue(oid);
	}

	public virtual Asn1OctetString GetExtensionValue(DerObjectIdentifier oid)
	{
		return GetX509Extensions()?.GetExtensionValue(oid);
	}
}
