using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsDsaVerifier : BcTlsDssVerifier
{
	protected override short SignatureAlgorithm => 2;

	public BcTlsDsaVerifier(BcTlsCrypto crypto, DsaPublicKeyParameters publicKey)
		: base(crypto, publicKey)
	{
	}

	protected override IDsa CreateDsaImpl()
	{
		return new DsaSigner();
	}
}
