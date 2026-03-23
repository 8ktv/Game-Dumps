using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

internal static class PKMacValueGenerator
{
	internal static PKMacValue Generate(PKMacBuilder builder, char[] password, SubjectPublicKeyInfo keyInfo)
	{
		IMacFactory macFactory = builder.Build(password);
		return new PKMacValue(macValue: X509Utilities.GenerateMac(macFactory, keyInfo), algID: (AlgorithmIdentifier)macFactory.AlgorithmDetails);
	}
}
