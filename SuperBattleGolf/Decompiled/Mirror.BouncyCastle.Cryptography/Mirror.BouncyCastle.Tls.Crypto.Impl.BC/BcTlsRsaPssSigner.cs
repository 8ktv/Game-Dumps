using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsRsaPssSigner : BcTlsSigner
{
	private readonly int m_signatureScheme;

	public BcTlsRsaPssSigner(BcTlsCrypto crypto, RsaKeyParameters privateKey, int signatureScheme)
		: base(crypto, privateKey)
	{
		if (!SignatureScheme.IsRsaPss(signatureScheme))
		{
			throw new ArgumentException("signatureScheme");
		}
		m_signatureScheme = signatureScheme;
	}

	public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, byte[] hash)
	{
		if (algorithm == null || SignatureScheme.From(algorithm) != m_signatureScheme)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		int cryptoHashAlgorithm = SignatureScheme.GetCryptoHashAlgorithm(m_signatureScheme);
		IDigest digest = m_crypto.CreateDigest(cryptoHashAlgorithm);
		PssSigner pssSigner = PssSigner.CreateRawSigner(new RsaBlindedEngine(), digest);
		pssSigner.Init(forSigning: true, new ParametersWithRandom(m_privateKey, m_crypto.SecureRandom));
		pssSigner.BlockUpdate(hash, 0, hash.Length);
		try
		{
			return pssSigner.GenerateSignature();
		}
		catch (CryptoException alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
	}
}
