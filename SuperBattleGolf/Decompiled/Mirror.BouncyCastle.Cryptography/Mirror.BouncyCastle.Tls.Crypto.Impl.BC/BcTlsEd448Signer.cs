using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsEd448Signer : BcTlsSigner
{
	public BcTlsEd448Signer(BcTlsCrypto crypto, Ed448PrivateKeyParameters privateKey)
		: base(crypto, privateKey)
	{
	}

	public override TlsStreamSigner GetStreamSigner(SignatureAndHashAlgorithm algorithm)
	{
		if (algorithm == null || SignatureScheme.From(algorithm) != 2056)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		Ed448Signer ed448Signer = new Ed448Signer(TlsUtilities.EmptyBytes);
		ed448Signer.Init(forSigning: true, m_privateKey);
		return new BcTlsStreamSigner(ed448Signer);
	}
}
