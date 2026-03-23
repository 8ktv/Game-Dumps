using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.X509.Extension;

public class X509ExtensionUtilities
{
	public static Asn1Object FromExtensionValue(Asn1OctetString extensionValue)
	{
		return Asn1Object.FromByteArray(extensionValue.GetOctets());
	}

	public static Asn1Object FromExtensionValue(IX509Extension extensions, DerObjectIdentifier oid)
	{
		Asn1OctetString extensionValue = extensions.GetExtensionValue(oid);
		if (extensionValue != null)
		{
			return FromExtensionValue(extensionValue);
		}
		return null;
	}
}
