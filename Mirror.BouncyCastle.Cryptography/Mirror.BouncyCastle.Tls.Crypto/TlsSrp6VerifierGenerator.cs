using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsSrp6VerifierGenerator
{
	BigInteger GenerateVerifier(byte[] salt, byte[] identity, byte[] password);
}
