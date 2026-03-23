namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsVerifier
{
	TlsStreamVerifier GetStreamVerifier(DigitallySigned digitallySigned);

	bool VerifyRawSignature(DigitallySigned digitallySigned, byte[] hash);
}
