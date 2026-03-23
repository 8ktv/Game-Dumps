namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsSigner
{
	byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, byte[] hash);

	TlsStreamSigner GetStreamSigner(SignatureAndHashAlgorithm algorithm);
}
