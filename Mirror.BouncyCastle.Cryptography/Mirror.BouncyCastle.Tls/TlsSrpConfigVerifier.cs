using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsSrpConfigVerifier
{
	bool Accept(TlsSrpConfig srpConfig);
}
