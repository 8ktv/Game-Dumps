using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsECDsaVerifier : BcTlsDssVerifier
{
	protected override short SignatureAlgorithm => 3;

	public BcTlsECDsaVerifier(BcTlsCrypto crypto, ECPublicKeyParameters publicKey)
		: base(crypto, publicKey)
	{
	}

	protected override IDsa CreateDsaImpl()
	{
		return new ECDsaSigner();
	}
}
