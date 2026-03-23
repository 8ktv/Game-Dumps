using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsECDomain : TlsECDomain
{
	protected readonly BcTlsCrypto m_crypto;

	protected readonly TlsECConfig m_config;

	protected readonly ECDomainParameters m_domainParameters;

	public static BcTlsSecret CalculateECDHAgreement(BcTlsCrypto crypto, ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
	{
		ECDHBasicAgreement eCDHBasicAgreement = new ECDHBasicAgreement();
		eCDHBasicAgreement.Init(privateKey);
		byte[] data = BigIntegers.AsUnsignedByteArray(n: eCDHBasicAgreement.CalculateAgreement(publicKey), length: eCDHBasicAgreement.GetFieldSize());
		return crypto.AdoptLocalSecret(data);
	}

	public static ECDomainParameters GetDomainParameters(TlsECConfig ecConfig)
	{
		return GetDomainParameters(ecConfig.NamedGroup);
	}

	public static ECDomainParameters GetDomainParameters(int namedGroup)
	{
		if (!NamedGroup.RefersToASpecificCurve(namedGroup))
		{
			return null;
		}
		X9ECParameters x9ECParameters = ECKeyPairGenerator.FindECCurveByName(NamedGroup.GetCurveName(namedGroup));
		if (x9ECParameters == null)
		{
			return null;
		}
		return new ECDomainParameters(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N, x9ECParameters.H, x9ECParameters.GetSeed());
	}

	public BcTlsECDomain(BcTlsCrypto crypto, TlsECConfig ecConfig)
	{
		m_crypto = crypto;
		m_config = ecConfig;
		m_domainParameters = GetDomainParameters(ecConfig);
	}

	public virtual BcTlsSecret CalculateECDHAgreement(ECPrivateKeyParameters privateKey, ECPublicKeyParameters publicKey)
	{
		return CalculateECDHAgreement(m_crypto, privateKey, publicKey);
	}

	public virtual TlsAgreement CreateECDH()
	{
		return new BcTlsECDH(this);
	}

	public virtual ECPoint DecodePoint(byte[] encoding)
	{
		return m_domainParameters.Curve.DecodePoint(encoding);
	}

	public virtual ECPublicKeyParameters DecodePublicKey(byte[] encoding)
	{
		try
		{
			return new ECPublicKeyParameters(DecodePoint(encoding), m_domainParameters);
		}
		catch (IOException)
		{
			throw;
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(47, alertCause);
		}
	}

	public virtual byte[] EncodePoint(ECPoint point)
	{
		return point.GetEncoded(compressed: false);
	}

	public virtual byte[] EncodePublicKey(ECPublicKeyParameters publicKey)
	{
		return EncodePoint(publicKey.Q);
	}

	public virtual AsymmetricCipherKeyPair GenerateKeyPair()
	{
		ECKeyPairGenerator eCKeyPairGenerator = new ECKeyPairGenerator();
		eCKeyPairGenerator.Init(new ECKeyGenerationParameters(m_domainParameters, m_crypto.SecureRandom));
		return eCKeyPairGenerator.GenerateKeyPair();
	}
}
