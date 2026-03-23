using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Encodings;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsRsaVerifier : BcTlsVerifier
{
	public BcTlsRsaVerifier(BcTlsCrypto crypto, RsaKeyParameters publicKey)
		: base(crypto, publicKey)
	{
	}

	public override bool VerifyRawSignature(DigitallySigned digitallySigned, byte[] hash)
	{
		IDigest digest = new NullDigest();
		SignatureAndHashAlgorithm algorithm = digitallySigned.Algorithm;
		ISigner signer;
		if (algorithm != null)
		{
			if (algorithm.Signature != 1)
			{
				throw new InvalidOperationException("Invalid algorithm: " + algorithm);
			}
			signer = new RsaDigestSigner(digest, TlsUtilities.GetOidForHashAlgorithm(algorithm.Hash));
		}
		else
		{
			signer = new GenericSigner(new Pkcs1Encoding(new RsaBlindedEngine()), digest);
		}
		signer.Init(forSigning: false, m_publicKey);
		signer.BlockUpdate(hash, 0, hash.Length);
		return signer.VerifySignature(digitallySigned.Signature);
	}
}
