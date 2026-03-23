using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security;

internal static class CertificateHelper
{
	private const string ClientAuthenticationOID = "1.3.6.1.5.5.7.3.2";

	internal static X509Certificate2 GetEligibleClientCertificate(X509CertificateCollection candidateCerts)
	{
		if (candidateCerts.Count == 0)
		{
			return null;
		}
		X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
		x509Certificate2Collection.AddRange(candidateCerts);
		return GetEligibleClientCertificate(x509Certificate2Collection);
	}

	internal static X509Certificate2 GetEligibleClientCertificate(X509Certificate2Collection candidateCerts)
	{
		if (candidateCerts.Count == 0)
		{
			return null;
		}
		X509Certificate2Enumerator enumerator = candidateCerts.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Certificate2 current = enumerator.Current;
			if (current.HasPrivateKey && IsValidClientCertificate(current))
			{
				return current;
			}
		}
		return null;
	}

	private static bool IsValidClientCertificate(X509Certificate2 cert)
	{
		X509ExtensionEnumerator enumerator = cert.Extensions.GetEnumerator();
		while (enumerator.MoveNext())
		{
			X509Extension current = enumerator.Current;
			if (current is X509EnhancedKeyUsageExtension eku && !IsValidForClientAuthenticationEKU(eku))
			{
				return false;
			}
			if (current is X509KeyUsageExtension ku && !IsValidForDigitalSignatureUsage(ku))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsValidForClientAuthenticationEKU(X509EnhancedKeyUsageExtension eku)
	{
		OidEnumerator enumerator = eku.EnhancedKeyUsages.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value == "1.3.6.1.5.5.7.3.2")
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsValidForDigitalSignatureUsage(X509KeyUsageExtension ku)
	{
		return (ku.KeyUsages & X509KeyUsageFlags.DigitalSignature) == X509KeyUsageFlags.DigitalSignature;
	}

	internal static X509Certificate2 GetEligibleClientCertificate()
	{
		X509Certificate2Collection certificates;
		using (X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
		{
			x509Store.Open(OpenFlags.OpenExistingOnly);
			certificates = x509Store.Certificates;
		}
		return GetEligibleClientCertificate(certificates);
	}
}
