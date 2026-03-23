namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsCipherExt
{
	int GetPlaintextDecodeLimit(int ciphertextLimit);

	int GetPlaintextEncodeLimit(int ciphertextLimit);
}
