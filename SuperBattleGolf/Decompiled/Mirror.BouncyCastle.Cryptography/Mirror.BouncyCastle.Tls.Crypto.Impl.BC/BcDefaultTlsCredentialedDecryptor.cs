using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Tls;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcDefaultTlsCredentialedDecryptor : TlsCredentialedDecryptor, TlsCredentials
{
	protected readonly BcTlsCrypto m_crypto;

	protected readonly Certificate m_certificate;

	protected readonly AsymmetricKeyParameter m_privateKey;

	public virtual Certificate Certificate => m_certificate;

	public BcDefaultTlsCredentialedDecryptor(BcTlsCrypto crypto, Certificate certificate, AsymmetricKeyParameter privateKey)
	{
		if (crypto == null)
		{
			throw new ArgumentNullException("crypto");
		}
		if (certificate == null)
		{
			throw new ArgumentNullException("certificate");
		}
		if (certificate.IsEmpty)
		{
			throw new ArgumentException("cannot be empty", "certificate");
		}
		if (privateKey == null)
		{
			throw new ArgumentNullException("privateKey");
		}
		if (!privateKey.IsPrivate)
		{
			throw new ArgumentException("must be private", "privateKey");
		}
		if (!(privateKey is RsaKeyParameters))
		{
			throw new ArgumentException("'privateKey' type not supported: " + privateKey.GetType().FullName);
		}
		m_crypto = crypto;
		m_certificate = certificate;
		m_privateKey = privateKey;
	}

	public virtual TlsSecret Decrypt(TlsCryptoParameters cryptoParams, byte[] ciphertext)
	{
		return SafeDecryptPreMasterSecret(cryptoParams, (RsaKeyParameters)m_privateKey, ciphertext);
	}

	protected virtual TlsSecret SafeDecryptPreMasterSecret(TlsCryptoParameters cryptoParams, RsaKeyParameters rsaServerPrivateKey, byte[] encryptedPreMasterSecret)
	{
		ProtocolVersion rsaPreMasterSecretVersion = cryptoParams.RsaPreMasterSecretVersion;
		byte[] data = Mirror.BouncyCastle.Crypto.Tls.TlsRsaKeyExchange.DecryptPreMasterSecret(encryptedPreMasterSecret, 0, encryptedPreMasterSecret.Length, rsaServerPrivateKey, rsaPreMasterSecretVersion.FullVersion, m_crypto.SecureRandom);
		return m_crypto.CreateSecret(data);
	}
}
