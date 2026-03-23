using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cmp;

internal static class CmpUtilities
{
	internal static byte[] CalculateCertHash(Asn1Encodable asn1Encodable, AlgorithmIdentifier signatureAlgorithm, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		return X509Utilities.CalculateDigest(digestAlgorithmFinder.Find(signatureAlgorithm) ?? throw new CmpException("cannot find digest algorithm from signature algorithm"), asn1Encodable);
	}
}
