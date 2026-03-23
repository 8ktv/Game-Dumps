using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Encodings;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

internal sealed class BcTlsRsaEncryptor : TlsEncryptor
{
	private readonly BcTlsCrypto m_crypto;

	private readonly RsaKeyParameters m_pubKeyRsa;

	private static RsaKeyParameters CheckPublicKey(RsaKeyParameters pubKeyRsa)
	{
		if (pubKeyRsa == null || pubKeyRsa.IsPrivate)
		{
			throw new ArgumentException("No public RSA key provided", "pubKeyRsa");
		}
		return pubKeyRsa;
	}

	internal BcTlsRsaEncryptor(BcTlsCrypto crypto, RsaKeyParameters pubKeyRsa)
	{
		m_crypto = crypto;
		m_pubKeyRsa = CheckPublicKey(pubKeyRsa);
	}

	public byte[] Encrypt(byte[] input, int inOff, int length)
	{
		try
		{
			Pkcs1Encoding pkcs1Encoding = new Pkcs1Encoding(new RsaBlindedEngine());
			pkcs1Encoding.Init(forEncryption: true, new ParametersWithRandom(m_pubKeyRsa, m_crypto.SecureRandom));
			return pkcs1Encoding.ProcessBlock(input, inOff, length);
		}
		catch (InvalidCipherTextException alertCause)
		{
			throw new TlsFatalAlert(80, alertCause);
		}
	}
}
