using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsDHGroupVerifier
{
	bool Accept(DHGroup dhGroup);
}
