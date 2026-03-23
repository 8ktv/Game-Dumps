using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsECDsa13Signer : BcTlsSigner
{
	private readonly int m_signatureScheme;

	public BcTlsECDsa13Signer(BcTlsCrypto crypto, ECPrivateKeyParameters privateKey, int signatureScheme)
		: base(crypto, privateKey)
	{
		if (!SignatureScheme.IsECDsa(signatureScheme))
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
		ISigner signer = new DsaDigestSigner(new ECDsaSigner(new HMacDsaKCalculator(m_crypto.CreateDigest(cryptoHashAlgorithm))), new NullDigest());
		signer.Init(forSigning: true, new ParametersWithRandom(m_privateKey, m_crypto.SecureRandom));
		signer.BlockUpdate(hash, 0, hash.Length);
		try
		{
			return signer.GenerateSignature();
		}
		catch (CryptoException alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
	}
}
