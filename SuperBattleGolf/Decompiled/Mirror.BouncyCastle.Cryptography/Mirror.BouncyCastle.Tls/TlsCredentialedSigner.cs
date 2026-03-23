using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsCredentialedSigner : TlsCredentials
{
	SignatureAndHashAlgorithm SignatureAndHashAlgorithm { get; }

	byte[] GenerateRawSignature(byte[] hash);

	TlsStreamSigner GetStreamSigner();
}
