using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsEd25519Signer : BcTlsSigner
{
	public BcTlsEd25519Signer(BcTlsCrypto crypto, Ed25519PrivateKeyParameters privateKey)
		: base(crypto, privateKey)
	{
	}

	public override TlsStreamSigner GetStreamSigner(SignatureAndHashAlgorithm algorithm)
	{
		if (algorithm == null || SignatureScheme.From(algorithm) != 2055)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		Ed25519Signer ed25519Signer = new Ed25519Signer();
		ed25519Signer.Init(forSigning: true, m_privateKey);
		return new BcTlsStreamSigner(ed25519Signer);
	}
}
