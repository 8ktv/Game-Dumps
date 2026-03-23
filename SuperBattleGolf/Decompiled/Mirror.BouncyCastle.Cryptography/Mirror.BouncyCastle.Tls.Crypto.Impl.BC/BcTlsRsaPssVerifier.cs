using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsRsaPssVerifier : BcTlsVerifier
{
	private readonly int m_signatureScheme;

	public BcTlsRsaPssVerifier(BcTlsCrypto crypto, RsaKeyParameters publicKey, int signatureScheme)
		: base(crypto, publicKey)
	{
		if (!SignatureScheme.IsRsaPss(signatureScheme))
		{
			throw new ArgumentException("signatureScheme");
		}
		m_signatureScheme = signatureScheme;
	}

	public override bool VerifyRawSignature(DigitallySigned digitallySigned, byte[] hash)
	{
		SignatureAndHashAlgorithm algorithm = digitallySigned.Algorithm;
		if (algorithm == null || SignatureScheme.From(algorithm) != m_signatureScheme)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		int cryptoHashAlgorithm = SignatureScheme.GetCryptoHashAlgorithm(m_signatureScheme);
		IDigest digest = m_crypto.CreateDigest(cryptoHashAlgorithm);
		PssSigner pssSigner = PssSigner.CreateRawSigner(new RsaEngine(), digest);
		pssSigner.Init(forSigning: false, m_publicKey);
		pssSigner.BlockUpdate(hash, 0, hash.Length);
		return pssSigner.VerifySignature(digitallySigned.Signature);
	}
}
