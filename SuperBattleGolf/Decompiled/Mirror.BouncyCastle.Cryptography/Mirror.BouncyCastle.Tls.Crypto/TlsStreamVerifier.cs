using System.IO;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsStreamVerifier
{
	Stream Stream { get; }

	bool IsVerified();
}
