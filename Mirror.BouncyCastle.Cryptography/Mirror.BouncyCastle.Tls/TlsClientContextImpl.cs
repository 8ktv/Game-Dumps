using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

internal class TlsClientContextImpl : AbstractTlsContext, TlsClientContext, TlsContext
{
	public override bool IsServer => false;

	internal TlsClientContextImpl(TlsCrypto crypto)
		: base(crypto, 1)
	{
	}
}
