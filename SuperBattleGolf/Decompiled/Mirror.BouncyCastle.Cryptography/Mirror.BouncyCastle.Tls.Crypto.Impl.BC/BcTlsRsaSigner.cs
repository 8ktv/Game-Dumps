using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Encodings;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsRsaSigner : BcTlsSigner
{
	private readonly RsaKeyParameters m_publicKey;

	public BcTlsRsaSigner(BcTlsCrypto crypto, RsaKeyParameters privateKey, RsaKeyParameters publicKey)
		: base(crypto, privateKey)
	{
		m_publicKey = publicKey;
	}

	public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, byte[] hash)
	{
		IDigest digest = new NullDigest();
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
		signer.Init(forSigning: true, new ParametersWithRandom(m_privateKey, m_crypto.SecureRandom));
		signer.BlockUpdate(hash, 0, hash.Length);
		try
		{
			byte[] array = signer.GenerateSignature();
			signer.Init(forSigning: false, m_publicKey);
			signer.BlockUpdate(hash, 0, hash.Length);
			if (signer.VerifySignature(array))
			{
				return array;
			}
		}
		catch (CryptoException alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
		throw new TlsFatalAlert(80);
	}
}
