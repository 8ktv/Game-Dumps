using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsDHDomain : TlsDHDomain
{
	protected readonly BcTlsCrypto m_crypto;

	protected readonly TlsDHConfig m_config;

	protected readonly DHParameters m_domainParameters;

	private static byte[] EncodeValue(DHParameters dh, bool padded, BigInteger x)
	{
		if (!padded)
		{
			return BigIntegers.AsUnsignedByteArray(x);
		}
		return BigIntegers.AsUnsignedByteArray(GetValueLength(dh), x);
	}

	private static int GetValueLength(DHParameters dh)
	{
		return BigIntegers.GetUnsignedByteLength(dh.P);
	}

	public static BcTlsSecret CalculateDHAgreement(BcTlsCrypto crypto, DHPrivateKeyParameters privateKey, DHPublicKeyParameters publicKey, bool padded)
	{
		DHBasicAgreement dHBasicAgreement = new DHBasicAgreement();
		dHBasicAgreement.Init(privateKey);
		BigInteger x = dHBasicAgreement.CalculateAgreement(publicKey);
		byte[] data = EncodeValue(privateKey.Parameters, padded, x);
		return crypto.AdoptLocalSecret(data);
	}

	public static DHParameters GetDomainParameters(TlsDHConfig dhConfig)
	{
		DHGroup dHGroup = TlsDHUtilities.GetDHGroup(dhConfig);
		if (dHGroup == null)
		{
			throw new ArgumentException("No DH configuration provided");
		}
		return new DHParameters(dHGroup.P, dHGroup.G, dHGroup.Q, dHGroup.L);
	}

	public BcTlsDHDomain(BcTlsCrypto crypto, TlsDHConfig dhConfig)
	{
		m_crypto = crypto;
		m_config = dhConfig;
		m_domainParameters = GetDomainParameters(dhConfig);
	}

	public virtual BcTlsSecret CalculateDHAgreement(DHPrivateKeyParameters privateKey, DHPublicKeyParameters publicKey)
	{
		return CalculateDHAgreement(m_crypto, privateKey, publicKey, m_config.IsPadded);
	}

	public virtual TlsAgreement CreateDH()
	{
		return new BcTlsDH(this);
	}

	public virtual BigInteger DecodeParameter(byte[] encoding)
	{
		if (m_config.IsPadded && GetValueLength(m_domainParameters) != encoding.Length)
		{
			throw new TlsFatalAlert(47);
		}
		return new BigInteger(1, encoding);
	}

	public virtual DHPublicKeyParameters DecodePublicKey(byte[] encoding)
	{
		try
		{
			return new DHPublicKeyParameters(DecodeParameter(encoding), m_domainParameters);
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(40, alertCause);
		}
	}

	public virtual byte[] EncodeParameter(BigInteger x)
	{
		return EncodeValue(m_domainParameters, m_config.IsPadded, x);
	}

	public virtual byte[] EncodePublicKey(DHPublicKeyParameters publicKey)
	{
		return EncodeValue(m_domainParameters, padded: true, publicKey.Y);
	}

	public virtual AsymmetricCipherKeyPair GenerateKeyPair()
	{
		DHKeyPairGenerator dHKeyPairGenerator = new DHKeyPairGenerator();
		dHKeyPairGenerator.Init(new DHKeyGenerationParameters(m_crypto.SecureRandom, m_domainParameters));
		return dHKeyPairGenerator.GenerateKeyPair();
	}
}
