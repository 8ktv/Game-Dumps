using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsSrp6Server
{
	BigInteger GenerateServerCredentials();

	BigInteger CalculateSecret(BigInteger clientA);
}
