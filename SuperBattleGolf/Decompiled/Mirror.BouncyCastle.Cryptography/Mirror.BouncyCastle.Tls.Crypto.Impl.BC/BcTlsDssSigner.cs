using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public abstract class BcTlsDssSigner : BcTlsSigner
{
	protected abstract short SignatureAlgorithm { get; }

	protected BcTlsDssSigner(BcTlsCrypto crypto, AsymmetricKeyParameter privateKey)
		: base(crypto, privateKey)
	{
	}

	protected abstract IDsa CreateDsaImpl(int cryptoHashAlgorithm);

	public override byte[] GenerateRawSignature(SignatureAndHashAlgorithm algorithm, byte[] hash)
	{
		if (algorithm != null && algorithm.Signature != SignatureAlgorithm)
		{
			throw new InvalidOperationException("Invalid algorithm: " + algorithm);
		}
		int cryptoHashAlgorithm = ((algorithm == null) ? 2 : TlsCryptoUtilities.GetHash(algorithm.Hash));
		ISigner signer = new DsaDigestSigner(CreateDsaImpl(cryptoHashAlgorithm), new NullDigest());
		signer.Init(forSigning: true, new ParametersWithRandom(m_privateKey, m_crypto.SecureRandom));
		if (algorithm == null)
		{
			signer.BlockUpdate(hash, 16, 20);
		}
		else
		{
			signer.BlockUpdate(hash, 0, hash.Length);
		}
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
