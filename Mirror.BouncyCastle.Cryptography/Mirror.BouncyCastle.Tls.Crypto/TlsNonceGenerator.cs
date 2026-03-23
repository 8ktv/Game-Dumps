namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsNonceGenerator
{
	byte[] GenerateNonce(int size);
}
