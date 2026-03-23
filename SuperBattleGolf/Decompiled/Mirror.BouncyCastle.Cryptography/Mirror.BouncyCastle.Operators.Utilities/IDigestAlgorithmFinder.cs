using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Operators.Utilities;

public interface IDigestAlgorithmFinder
{
	AlgorithmIdentifier Find(AlgorithmIdentifier signatureAlgorithm);

	AlgorithmIdentifier Find(DerObjectIdentifier digestOid);

	AlgorithmIdentifier Find(string digestName);
}
