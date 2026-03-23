using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

internal class TlsServerContextImpl : AbstractTlsContext, TlsServerContext, TlsContext
{
	public override bool IsServer => true;

	internal TlsServerContextImpl(TlsCrypto crypto)
		: base(crypto, 0)
	{
	}
}
