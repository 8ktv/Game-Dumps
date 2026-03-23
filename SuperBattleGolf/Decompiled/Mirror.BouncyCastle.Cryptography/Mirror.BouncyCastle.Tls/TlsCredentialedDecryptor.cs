using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsCredentialedDecryptor : TlsCredentials
{
	TlsSecret Decrypt(TlsCryptoParameters cryptoParams, byte[] ciphertext);
}
