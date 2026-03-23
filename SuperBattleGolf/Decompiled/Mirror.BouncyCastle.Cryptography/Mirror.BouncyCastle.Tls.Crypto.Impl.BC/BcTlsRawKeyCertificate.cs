using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Signers;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsRawKeyCertificate : TlsCertificate
{
	protected readonly BcTlsCrypto m_crypto;

	protected readonly SubjectPublicKeyInfo m_keyInfo;

	protected DHPublicKeyParameters m_pubKeyDH;

	protected ECPublicKeyParameters m_pubKeyEC;

	protected Ed25519PublicKeyParameters m_pubKeyEd25519;

	protected Ed448PublicKeyParameters m_pubKeyEd448;

	protected RsaKeyParameters m_pubKeyRsa;

	public virtual SubjectPublicKeyInfo SubjectPublicKeyInfo => m_keyInfo;

	public virtual BigInteger SerialNumber => null;

	public virtual string SigAlgOid => null;

	public BcTlsRawKeyCertificate(BcTlsCrypto crypto, byte[] encoding)
		: this(crypto, SubjectPublicKeyInfo.GetInstance(encoding))
	{
	}

	public BcTlsRawKeyCertificate(BcTlsCrypto crypto, SubjectPublicKeyInfo keyInfo)
	{
		m_crypto = crypto;
		m_keyInfo = keyInfo;
	}

	public virtual TlsEncryptor CreateEncryptor(int tlsCertificateRole)
	{
		ValidateKeyUsage(32);
		if (tlsCertificateRole == 3)
		{
			m_pubKeyRsa = GetPubKeyRsa();
			return new BcTlsRsaEncryptor(m_crypto, m_pubKeyRsa);
		}
		throw new TlsFatalAlert(46);
	}

	public virtual TlsVerifier CreateVerifier(short signatureAlgorithm)
	{
		if ((uint)(signatureAlgorithm - 7) <= 1u)
		{
			int signatureScheme = SignatureScheme.From(8, signatureAlgorithm);
			Tls13Verifier tls13Verifier = CreateVerifier(signatureScheme);
			return new LegacyTls13Verifier(signatureScheme, tls13Verifier);
		}
		ValidateKeyUsage(128);
		switch (signatureAlgorithm)
		{
		case 2:
			return new BcTlsDsaVerifier(m_crypto, GetPubKeyDss());
		case 3:
			return new BcTlsECDsaVerifier(m_crypto, GetPubKeyEC());
		case 1:
			ValidateRsa_Pkcs1();
			return new BcTlsRsaVerifier(m_crypto, GetPubKeyRsa());
		case 9:
		case 10:
		case 11:
		{
			ValidateRsa_Pss_Pss(signatureAlgorithm);
			int signatureScheme3 = SignatureScheme.From(8, signatureAlgorithm);
			return new BcTlsRsaPssVerifier(m_crypto, GetPubKeyRsa(), signatureScheme3);
		}
		case 4:
		case 5:
		case 6:
		{
			ValidateRsa_Pss_Rsae();
			int signatureScheme2 = SignatureScheme.From(8, signatureAlgorithm);
			return new BcTlsRsaPssVerifier(m_crypto, GetPubKeyRsa(), signatureScheme2);
		}
		default:
			throw new TlsFatalAlert(46);
		}
	}

	public virtual Tls13Verifier CreateVerifier(int signatureScheme)
	{
		ValidateKeyUsage(128);
		switch (signatureScheme)
		{
		case 515:
		case 1027:
		case 1283:
		case 1539:
		case 2074:
		case 2075:
		case 2076:
		{
			int cryptoHashAlgorithm4 = SignatureScheme.GetCryptoHashAlgorithm(signatureScheme);
			IDigest digest3 = m_crypto.CreateDigest(cryptoHashAlgorithm4);
			DsaDigestSigner dsaDigestSigner = new DsaDigestSigner(new ECDsaSigner(), digest3);
			((ISigner)dsaDigestSigner).Init(forSigning: false, (ICipherParameters)GetPubKeyEC());
			return new BcTls13Verifier(dsaDigestSigner);
		}
		case 2055:
		{
			Ed25519Signer ed25519Signer = new Ed25519Signer();
			ed25519Signer.Init(forSigning: false, GetPubKeyEd25519());
			return new BcTls13Verifier(ed25519Signer);
		}
		case 2056:
		{
			Ed448Signer ed448Signer = new Ed448Signer(TlsUtilities.EmptyBytes);
			ed448Signer.Init(forSigning: false, GetPubKeyEd448());
			return new BcTls13Verifier(ed448Signer);
		}
		case 513:
		case 1025:
		case 1281:
		case 1537:
		{
			ValidateRsa_Pkcs1();
			int cryptoHashAlgorithm3 = SignatureScheme.GetCryptoHashAlgorithm(signatureScheme);
			RsaDigestSigner rsaDigestSigner = new RsaDigestSigner(m_crypto.CreateDigest(cryptoHashAlgorithm3), TlsCryptoUtilities.GetOidForHash(cryptoHashAlgorithm3));
			rsaDigestSigner.Init(forSigning: false, GetPubKeyRsa());
			return new BcTls13Verifier(rsaDigestSigner);
		}
		case 2057:
		case 2058:
		case 2059:
		{
			ValidateRsa_Pss_Pss(SignatureScheme.GetSignatureAlgorithm(signatureScheme));
			int cryptoHashAlgorithm2 = SignatureScheme.GetCryptoHashAlgorithm(signatureScheme);
			IDigest digest2 = m_crypto.CreateDigest(cryptoHashAlgorithm2);
			PssSigner pssSigner2 = new PssSigner(new RsaEngine(), digest2, digest2.GetDigestSize());
			pssSigner2.Init(forSigning: false, GetPubKeyRsa());
			return new BcTls13Verifier(pssSigner2);
		}
		case 2052:
		case 2053:
		case 2054:
		{
			ValidateRsa_Pss_Rsae();
			int cryptoHashAlgorithm = SignatureScheme.GetCryptoHashAlgorithm(signatureScheme);
			IDigest digest = m_crypto.CreateDigest(cryptoHashAlgorithm);
			PssSigner pssSigner = new PssSigner(new RsaEngine(), digest, digest.GetDigestSize());
			pssSigner.Init(forSigning: false, GetPubKeyRsa());
			return new BcTls13Verifier(pssSigner);
		}
		default:
			throw new TlsFatalAlert(46);
		}
	}

	public virtual byte[] GetEncoded()
	{
		return m_keyInfo.GetEncoded("DER");
	}

	public virtual byte[] GetExtension(DerObjectIdentifier extensionOid)
	{
		return null;
	}

	public virtual Asn1Encodable GetSigAlgParams()
	{
		return null;
	}

	public virtual short GetLegacySignatureAlgorithm()
	{
		AsymmetricKeyParameter publicKey = GetPublicKey();
		if (publicKey.IsPrivate)
		{
			throw new TlsFatalAlert(80);
		}
		if (!SupportsKeyUsage(128))
		{
			return -1;
		}
		if (publicKey is RsaKeyParameters)
		{
			return 1;
		}
		if (publicKey is DsaPublicKeyParameters)
		{
			return 2;
		}
		if (publicKey is ECPublicKeyParameters)
		{
			return 3;
		}
		return -1;
	}

	public virtual DHPublicKeyParameters GetPubKeyDH()
	{
		try
		{
			return (DHPublicKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual DsaPublicKeyParameters GetPubKeyDss()
	{
		try
		{
			return (DsaPublicKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual ECPublicKeyParameters GetPubKeyEC()
	{
		try
		{
			return (ECPublicKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual Ed25519PublicKeyParameters GetPubKeyEd25519()
	{
		try
		{
			return (Ed25519PublicKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual Ed448PublicKeyParameters GetPubKeyEd448()
	{
		try
		{
			return (Ed448PublicKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual RsaKeyParameters GetPubKeyRsa()
	{
		try
		{
			return (RsaKeyParameters)GetPublicKey();
		}
		catch (InvalidCastException alertCause)
		{
			throw new TlsFatalAlert(46, alertCause);
		}
	}

	public virtual bool SupportsSignatureAlgorithm(short signatureAlgorithm)
	{
		return SupportsSignatureAlgorithm(signatureAlgorithm, 128);
	}

	public virtual bool SupportsSignatureAlgorithmCA(short signatureAlgorithm)
	{
		return SupportsSignatureAlgorithm(signatureAlgorithm, 4);
	}

	public virtual TlsCertificate CheckUsageInRole(int tlsCertificateRole)
	{
		switch (tlsCertificateRole)
		{
		case 1:
			ValidateKeyUsage(8);
			m_pubKeyDH = GetPubKeyDH();
			return this;
		case 2:
			ValidateKeyUsage(8);
			m_pubKeyEC = GetPubKeyEC();
			return this;
		default:
			throw new TlsFatalAlert(46);
		}
	}

	protected virtual AsymmetricKeyParameter GetPublicKey()
	{
		try
		{
			return PublicKeyFactory.CreateKey(m_keyInfo);
		}
		catch (Exception alertCause)
		{
			throw new TlsFatalAlert(43, alertCause);
		}
	}

	protected virtual bool SupportsKeyUsage(int keyUsageBits)
	{
		return true;
	}

	protected virtual bool SupportsRsa_Pkcs1()
	{
		return RsaUtilities.SupportsPkcs1(m_keyInfo.Algorithm);
	}

	protected virtual bool SupportsRsa_Pss_Pss(short signatureAlgorithm)
	{
		AlgorithmIdentifier algorithm = m_keyInfo.Algorithm;
		return RsaUtilities.SupportsPss_Pss(signatureAlgorithm, algorithm);
	}

	protected virtual bool SupportsRsa_Pss_Rsae()
	{
		return RsaUtilities.SupportsPss_Rsae(m_keyInfo.Algorithm);
	}

	protected virtual bool SupportsSignatureAlgorithm(short signatureAlgorithm, int keyUsage)
	{
		if (!SupportsKeyUsage(keyUsage))
		{
			return false;
		}
		AsymmetricKeyParameter publicKey = GetPublicKey();
		switch (signatureAlgorithm)
		{
		case 1:
			if (SupportsRsa_Pkcs1())
			{
				return publicKey is RsaKeyParameters;
			}
			return false;
		case 2:
			return publicKey is DsaPublicKeyParameters;
		case 3:
		case 26:
		case 27:
		case 28:
			return publicKey is ECPublicKeyParameters;
		case 7:
			return publicKey is Ed25519PublicKeyParameters;
		case 8:
			return publicKey is Ed448PublicKeyParameters;
		case 4:
		case 5:
		case 6:
			if (SupportsRsa_Pss_Rsae())
			{
				return publicKey is RsaKeyParameters;
			}
			return false;
		case 9:
		case 10:
		case 11:
			if (SupportsRsa_Pss_Pss(signatureAlgorithm))
			{
				return publicKey is RsaKeyParameters;
			}
			return false;
		default:
			return false;
		}
	}

	public virtual void ValidateKeyUsage(int keyUsageBits)
	{
		if (!SupportsKeyUsage(keyUsageBits))
		{
			throw new TlsFatalAlert(46);
		}
	}

	protected virtual void ValidateRsa_Pkcs1()
	{
		if (!SupportsRsa_Pkcs1())
		{
			throw new TlsFatalAlert(46);
		}
	}

	protected virtual void ValidateRsa_Pss_Pss(short signatureAlgorithm)
	{
		if (!SupportsRsa_Pss_Pss(signatureAlgorithm))
		{
			throw new TlsFatalAlert(46);
		}
	}

	protected virtual void ValidateRsa_Pss_Rsae()
	{
		if (!SupportsRsa_Pss_Rsae())
		{
			throw new TlsFatalAlert(46);
		}
	}
}
