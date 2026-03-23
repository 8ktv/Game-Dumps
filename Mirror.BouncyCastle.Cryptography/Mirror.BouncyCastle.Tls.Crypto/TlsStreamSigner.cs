using System.IO;

namespace Mirror.BouncyCastle.Tls.Crypto;

public interface TlsStreamSigner
{
	Stream Stream { get; }

	byte[] GetSignature();
}
