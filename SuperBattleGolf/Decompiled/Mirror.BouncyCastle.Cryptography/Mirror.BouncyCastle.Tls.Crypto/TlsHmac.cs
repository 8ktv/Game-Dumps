namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsHmac : TlsMac
{
	int InternalBlockSize { get; }
}
