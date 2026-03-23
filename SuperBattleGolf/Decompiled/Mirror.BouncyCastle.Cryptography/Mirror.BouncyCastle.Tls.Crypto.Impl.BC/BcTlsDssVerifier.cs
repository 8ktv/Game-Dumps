using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public abstract class BcTlsDssVerifier : BcTlsVerifier
{
	protected abstract short SignatureAlgorithm { get; }

	protected BcTlsDssVerifier(BcTlsCrypto crypto, AsymmetricKeyParameter publicKey)
		: base(crypto, publicKey)
	{
	}

	protected abstract IDsa CreateDsaImpl();

	public override bool VerifyRawSignature(DigitallySigned digitallySigned, byte[] hash)
	{
		SignatureAndHashAlgorithm algorithm = digitallySigned.Algorithm;
		if (algorithm != null && algorithm.Signature != SignatureAlgorithm)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		ISigner signer = new DsaDigestSigner(CreateDsaImpl(), new NullDigest());
		signer.Init(forSigning: false, m_publicKey);
		if (algorithm == null)
		{
			signer.BlockUpdate(hash, 16, 20);
		}
		else
		{
			signer.BlockUpdate(hash, 0, hash.Length);
		}
		return signer.VerifySignature(digitallySigned.Signature);
	}
}
