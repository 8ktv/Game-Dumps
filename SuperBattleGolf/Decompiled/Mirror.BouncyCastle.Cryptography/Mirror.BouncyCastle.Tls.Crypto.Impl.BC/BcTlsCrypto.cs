using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement.Srp;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Prng;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsCrypto : AbstractTlsCrypto
{
	private readonly SecureRandom m_entropySource;

	public override SecureRandom SecureRandom => m_entropySource;

	public BcTlsCrypto()
		: this(CryptoServicesRegistrar.GetSecureRandom())
	{
	}

	public BcTlsCrypto(SecureRandom entropySource)
	{
		if (entropySource == null)
		{
			throw new ArgumentNullException("entropySource");
		}
		m_entropySource = entropySource;
	}

	internal virtual BcTlsSecret AdoptLocalSecret(byte[] data)
	{
		return new BcTlsSecret(this, data);
	}

	public override TlsCertificate CreateCertificate(short type, byte[] encoding)
	{
		return type switch
		{
			0 => new BcTlsCertificate(this, encoding), 
			2 => new BcTlsRawKeyCertificate(this, encoding), 
			_ => throw new TlsFatalAlert(80), 
		};
	}

	public override TlsCipher CreateCipher(TlsCryptoParameters cryptoParams, int encryptionAlgorithm, int macAlgorithm)
	{
		switch (encryptionAlgorithm)
		{
		case 8:
		case 12:
		case 14:
		case 22:
		case 28:
			return CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 16, macAlgorithm);
		case 7:
			return CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 24, macAlgorithm);
		case 9:
		case 13:
		case 23:
			return CreateCipher_Cbc(cryptoParams, encryptionAlgorithm, 32, macAlgorithm);
		case 15:
			return CreateCipher_Aes_Ccm(cryptoParams, 16, 16);
		case 16:
			return CreateCipher_Aes_Ccm(cryptoParams, 16, 8);
		case 10:
			return CreateCipher_Aes_Gcm(cryptoParams, 16, 16);
		case 17:
			return CreateCipher_Aes_Ccm(cryptoParams, 32, 16);
		case 18:
			return CreateCipher_Aes_Ccm(cryptoParams, 32, 8);
		case 11:
			return CreateCipher_Aes_Gcm(cryptoParams, 32, 16);
		case 24:
			return CreateCipher_Aria_Gcm(cryptoParams, 16, 16);
		case 25:
			return CreateCipher_Aria_Gcm(cryptoParams, 32, 16);
		case 19:
			return CreateCipher_Camellia_Gcm(cryptoParams, 16, 16);
		case 20:
			return CreateCipher_Camellia_Gcm(cryptoParams, 32, 16);
		case 21:
			return CreateChaCha20Poly1305(cryptoParams);
		case 0:
			return CreateNullCipher(cryptoParams, macAlgorithm);
		case 26:
			return CreateCipher_SM4_Ccm(cryptoParams);
		case 27:
			return CreateCipher_SM4_Gcm(cryptoParams);
		default:
			throw new TlsFatalAlert(80);
		}
	}

	public override TlsDHDomain CreateDHDomain(TlsDHConfig dhConfig)
	{
		return new BcTlsDHDomain(this, dhConfig);
	}

	public override TlsECDomain CreateECDomain(TlsECConfig ecConfig)
	{
		return ecConfig.NamedGroup switch
		{
			29 => new BcX25519Domain(this), 
			30 => new BcX448Domain(this), 
			_ => new BcTlsECDomain(this, ecConfig), 
		};
	}

	public override TlsNonceGenerator CreateNonceGenerator(byte[] additionalSeedMaterial)
	{
		int cryptoHashAlgorithm = 4;
		IDigest digest = CreateDigest(cryptoHashAlgorithm);
		byte[] array = new byte[2 * TlsCryptoUtilities.GetHashOutputSize(cryptoHashAlgorithm)];
		SecureRandom.NextBytes(array);
		DigestRandomGenerator digestRandomGenerator = new DigestRandomGenerator(digest);
		digestRandomGenerator.AddSeedMaterial(additionalSeedMaterial);
		digestRandomGenerator.AddSeedMaterial(array);
		return new BcTlsNonceGenerator(digestRandomGenerator);
	}

	public override bool HasAnyStreamVerifiers(IList<SignatureAndHashAlgorithm> signatureAndHashAlgorithms)
	{
		foreach (SignatureAndHashAlgorithm signatureAndHashAlgorithm in signatureAndHashAlgorithms)
		{
			int num = SignatureScheme.From(signatureAndHashAlgorithm);
			if ((uint)(num - 2055) <= 1u)
			{
				return true;
			}
		}
		return false;
	}

	public override bool HasAnyStreamVerifiersLegacy(short[] clientCertificateTypes)
	{
		return false;
	}

	public override bool HasCryptoHashAlgorithm(int cryptoHashAlgorithm)
	{
		if ((uint)(cryptoHashAlgorithm - 1) <= 7u)
		{
			return true;
		}
		return false;
	}

	public override bool HasCryptoSignatureAlgorithm(int cryptoSignatureAlgorithm)
	{
		switch (cryptoSignatureAlgorithm)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
			return true;
		default:
			return false;
		}
	}

	public override bool HasDHAgreement()
	{
		return true;
	}

	public override bool HasECDHAgreement()
	{
		return true;
	}

	public override bool HasEncryptionAlgorithm(int encryptionAlgorithm)
	{
		switch (encryptionAlgorithm)
		{
		case 0:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
		case 27:
		case 28:
			return true;
		default:
			return false;
		}
	}

	public override bool HasHkdfAlgorithm(int cryptoHashAlgorithm)
	{
		if ((uint)(cryptoHashAlgorithm - 4) <= 3u)
		{
			return true;
		}
		return false;
	}

	public override bool HasMacAlgorithm(int macAlgorithm)
	{
		if ((uint)(macAlgorithm - 1) <= 4u)
		{
			return true;
		}
		return false;
	}

	public override bool HasNamedGroup(int namedGroup)
	{
		return NamedGroup.RefersToASpecificGroup(namedGroup);
	}

	public override bool HasRsaEncryption()
	{
		return true;
	}

	public override bool HasSignatureAlgorithm(short signatureAlgorithm)
	{
		switch (signatureAlgorithm)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 26:
		case 27:
		case 28:
			return true;
		default:
			return false;
		}
	}

	public override bool HasSignatureAndHashAlgorithm(SignatureAndHashAlgorithm sigAndHashAlgorithm)
	{
		short signature = sigAndHashAlgorithm.Signature;
		if (sigAndHashAlgorithm.Hash == 1)
		{
			if (1 == signature)
			{
				return HasSignatureAlgorithm(signature);
			}
			return false;
		}
		return HasSignatureAlgorithm(signature);
	}

	public override bool HasSignatureScheme(int signatureScheme)
	{
		if (signatureScheme == 1800)
		{
			return false;
		}
		short signatureAlgorithm = SignatureScheme.GetSignatureAlgorithm(signatureScheme);
		if (SignatureScheme.GetCryptoHashAlgorithm(signatureScheme) == 1)
		{
			if (1 == signatureAlgorithm)
			{
				return HasSignatureAlgorithm(signatureAlgorithm);
			}
			return false;
		}
		return HasSignatureAlgorithm(signatureAlgorithm);
	}

	public override bool HasSrpAuthentication()
	{
		return true;
	}

	public override TlsSecret CreateSecret(byte[] data)
	{
		return AdoptLocalSecret(Arrays.Clone(data));
	}

	public override TlsSecret GenerateRsaPreMasterSecret(ProtocolVersion version)
	{
		byte[] array = new byte[48];
		SecureRandom.NextBytes(array);
		TlsUtilities.WriteVersion(version, array, 0);
		return AdoptLocalSecret(array);
	}

	public virtual IDigest CloneDigest(int cryptoHashAlgorithm, IDigest digest)
	{
		return cryptoHashAlgorithm switch
		{
			1 => new MD5Digest((MD5Digest)digest), 
			2 => new Sha1Digest((Sha1Digest)digest), 
			3 => new Sha224Digest((Sha224Digest)digest), 
			4 => new Sha256Digest((Sha256Digest)digest), 
			5 => new Sha384Digest((Sha384Digest)digest), 
			6 => new Sha512Digest((Sha512Digest)digest), 
			7 => new SM3Digest((SM3Digest)digest), 
			8 => new Gost3411_2012_256Digest((Gost3411_2012_256Digest)digest), 
			_ => throw new ArgumentException("invalid CryptoHashAlgorithm: " + cryptoHashAlgorithm), 
		};
	}

	public virtual IDigest CreateDigest(int cryptoHashAlgorithm)
	{
		return cryptoHashAlgorithm switch
		{
			1 => new MD5Digest(), 
			2 => new Sha1Digest(), 
			3 => new Sha224Digest(), 
			4 => new Sha256Digest(), 
			5 => new Sha384Digest(), 
			6 => new Sha512Digest(), 
			7 => new SM3Digest(), 
			8 => new Gost3411_2012_256Digest(), 
			_ => throw new ArgumentException("invalid CryptoHashAlgorithm: " + cryptoHashAlgorithm), 
		};
	}

	public override TlsHash CreateHash(int cryptoHashAlgorithm)
	{
		return new BcTlsHash(this, cryptoHashAlgorithm);
	}

	protected virtual IBlockCipher CreateBlockCipher(int encryptionAlgorithm)
	{
		switch (encryptionAlgorithm)
		{
		case 7:
			return CreateDesEdeEngine();
		case 8:
		case 9:
			return CreateAesEngine();
		case 22:
		case 23:
			return CreateAriaEngine();
		case 12:
		case 13:
			return CreateCamelliaEngine();
		case 14:
			return CreateSeedEngine();
		case 28:
			return CreateSM4Engine();
		default:
			throw new TlsFatalAlert(80);
		}
	}

	protected virtual IBlockCipher CreateCbcBlockCipher(IBlockCipher blockCipher)
	{
		return new CbcBlockCipher(blockCipher);
	}

	protected virtual IBlockCipher CreateCbcBlockCipher(int encryptionAlgorithm)
	{
		return CreateCbcBlockCipher(CreateBlockCipher(encryptionAlgorithm));
	}

	protected virtual TlsCipher CreateChaCha20Poly1305(TlsCryptoParameters cryptoParams)
	{
		BcChaCha20Poly1305 encryptCipher = new BcChaCha20Poly1305(isEncrypting: true);
		BcChaCha20Poly1305 decryptCipher = new BcChaCha20Poly1305(isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, 32, 16, 2);
	}

	protected virtual TlsAeadCipher CreateCipher_Aes_Ccm(TlsCryptoParameters cryptoParams, int cipherKeySize, int macSize)
	{
		BcTlsCcmImpl encryptCipher = new BcTlsCcmImpl(CreateAeadCipher_Aes_Ccm(), isEncrypting: true);
		BcTlsCcmImpl decryptCipher = new BcTlsCcmImpl(CreateAeadCipher_Aes_Ccm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, cipherKeySize, macSize, 1);
	}

	protected virtual TlsAeadCipher CreateCipher_Aes_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize, int macSize)
	{
		BcTlsAeadCipherImpl encryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Aes_Gcm(), isEncrypting: true);
		BcTlsAeadCipherImpl decryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Aes_Gcm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, cipherKeySize, macSize, 3);
	}

	protected virtual TlsAeadCipher CreateCipher_Aria_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize, int macSize)
	{
		BcTlsAeadCipherImpl encryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Aria_Gcm(), isEncrypting: true);
		BcTlsAeadCipherImpl decryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Aria_Gcm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, cipherKeySize, macSize, 3);
	}

	protected virtual TlsAeadCipher CreateCipher_Camellia_Gcm(TlsCryptoParameters cryptoParams, int cipherKeySize, int macSize)
	{
		BcTlsAeadCipherImpl encryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Camellia_Gcm(), isEncrypting: true);
		BcTlsAeadCipherImpl decryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_Camellia_Gcm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, cipherKeySize, macSize, 3);
	}

	protected virtual TlsCipher CreateCipher_Cbc(TlsCryptoParameters cryptoParams, int encryptionAlgorithm, int cipherKeySize, int macAlgorithm)
	{
		BcTlsBlockCipherImpl encryptCipher = new BcTlsBlockCipherImpl(CreateCbcBlockCipher(encryptionAlgorithm), isEncrypting: true);
		BcTlsBlockCipherImpl decryptCipher = new BcTlsBlockCipherImpl(CreateCbcBlockCipher(encryptionAlgorithm), isEncrypting: false);
		TlsHmac clientMac = CreateMac(cryptoParams, macAlgorithm);
		TlsHmac serverMac = CreateMac(cryptoParams, macAlgorithm);
		return new TlsBlockCipher(cryptoParams, encryptCipher, decryptCipher, clientMac, serverMac, cipherKeySize);
	}

	protected virtual TlsAeadCipher CreateCipher_SM4_Ccm(TlsCryptoParameters cryptoParams)
	{
		BcTlsCcmImpl encryptCipher = new BcTlsCcmImpl(CreateAeadCipher_SM4_Ccm(), isEncrypting: true);
		BcTlsCcmImpl decryptCipher = new BcTlsCcmImpl(CreateAeadCipher_SM4_Ccm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, 16, 16, 1);
	}

	protected virtual TlsAeadCipher CreateCipher_SM4_Gcm(TlsCryptoParameters cryptoParams)
	{
		BcTlsAeadCipherImpl encryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_SM4_Gcm(), isEncrypting: true);
		BcTlsAeadCipherImpl decryptCipher = new BcTlsAeadCipherImpl(CreateAeadCipher_SM4_Gcm(), isEncrypting: false);
		return new TlsAeadCipher(cryptoParams, encryptCipher, decryptCipher, 16, 16, 3);
	}

	protected virtual TlsNullCipher CreateNullCipher(TlsCryptoParameters cryptoParams, int macAlgorithm)
	{
		return new TlsNullCipher(cryptoParams, CreateMac(cryptoParams, macAlgorithm), CreateMac(cryptoParams, macAlgorithm));
	}

	protected virtual IBlockCipher CreateAesEngine()
	{
		return AesUtilities.CreateEngine();
	}

	protected virtual IBlockCipher CreateAriaEngine()
	{
		return new AriaEngine();
	}

	protected virtual IBlockCipher CreateCamelliaEngine()
	{
		return new CamelliaEngine();
	}

	protected virtual IBlockCipher CreateDesEdeEngine()
	{
		return new DesEdeEngine();
	}

	protected virtual IBlockCipher CreateSeedEngine()
	{
		return new SeedEngine();
	}

	protected virtual IBlockCipher CreateSM4Engine()
	{
		return new SM4Engine();
	}

	protected virtual CcmBlockCipher CreateCcmMode(IBlockCipher engine)
	{
		return new CcmBlockCipher(engine);
	}

	protected virtual IAeadCipher CreateGcmMode(IBlockCipher engine)
	{
		return new GcmBlockCipher(engine);
	}

	protected virtual CcmBlockCipher CreateAeadCipher_Aes_Ccm()
	{
		return CreateCcmMode(CreateAesEngine());
	}

	protected virtual IAeadCipher CreateAeadCipher_Aes_Gcm()
	{
		return CreateGcmMode(CreateAesEngine());
	}

	protected virtual IAeadCipher CreateAeadCipher_Aria_Gcm()
	{
		return CreateGcmMode(CreateAriaEngine());
	}

	protected virtual IAeadCipher CreateAeadCipher_Camellia_Gcm()
	{
		return CreateGcmMode(CreateCamelliaEngine());
	}

	protected virtual CcmBlockCipher CreateAeadCipher_SM4_Ccm()
	{
		return CreateCcmMode(CreateSM4Engine());
	}

	protected virtual IAeadCipher CreateAeadCipher_SM4_Gcm()
	{
		return CreateGcmMode(CreateSM4Engine());
	}

	public override TlsHmac CreateHmac(int macAlgorithm)
	{
		if ((uint)(macAlgorithm - 1) <= 4u)
		{
			return CreateHmacForHash(TlsCryptoUtilities.GetHashForHmac(macAlgorithm));
		}
		throw new ArgumentException("invalid MacAlgorithm: " + macAlgorithm);
	}

	public override TlsHmac CreateHmacForHash(int cryptoHashAlgorithm)
	{
		return new BcTlsHmac(new HMac(CreateDigest(cryptoHashAlgorithm)));
	}

	protected virtual TlsHmac CreateHmac_Ssl(int macAlgorithm)
	{
		return macAlgorithm switch
		{
			1 => new BcSsl3Hmac(CreateDigest(1)), 
			2 => new BcSsl3Hmac(CreateDigest(2)), 
			3 => new BcSsl3Hmac(CreateDigest(4)), 
			4 => new BcSsl3Hmac(CreateDigest(5)), 
			5 => new BcSsl3Hmac(CreateDigest(6)), 
			_ => throw new TlsFatalAlert(80), 
		};
	}

	protected virtual TlsHmac CreateMac(TlsCryptoParameters cryptoParams, int macAlgorithm)
	{
		if (TlsImplUtilities.IsSsl(cryptoParams))
		{
			return CreateHmac_Ssl(macAlgorithm);
		}
		return CreateHmac(macAlgorithm);
	}

	public override TlsSrp6Client CreateSrp6Client(TlsSrpConfig srpConfig)
	{
		BigInteger[] explicitNG = srpConfig.GetExplicitNG();
		Srp6GroupParameters srp6GroupParameters = new Srp6GroupParameters(explicitNG[0], explicitNG[1]);
		Srp6Client srp6Client = new Srp6Client();
		srp6Client.Init(srp6GroupParameters, CreateDigest(2), SecureRandom);
		return new BcTlsSrp6Client(srp6Client);
	}

	public override TlsSrp6Server CreateSrp6Server(TlsSrpConfig srpConfig, BigInteger srpVerifier)
	{
		BigInteger[] explicitNG = srpConfig.GetExplicitNG();
		Srp6GroupParameters srp6GroupParameters = new Srp6GroupParameters(explicitNG[0], explicitNG[1]);
		Srp6Server srp6Server = new Srp6Server();
		srp6Server.Init(srp6GroupParameters, srpVerifier, CreateDigest(2), SecureRandom);
		return new BcTlsSrp6Server(srp6Server);
	}

	public override TlsSrp6VerifierGenerator CreateSrp6VerifierGenerator(TlsSrpConfig srpConfig)
	{
		BigInteger[] explicitNG = srpConfig.GetExplicitNG();
		Srp6VerifierGenerator srp6VerifierGenerator = new Srp6VerifierGenerator();
		srp6VerifierGenerator.Init(explicitNG[0], explicitNG[1], CreateDigest(2));
		return new BcTlsSrp6VerifierGenerator(srp6VerifierGenerator);
	}

	public override TlsSecret HkdfInit(int cryptoHashAlgorithm)
	{
		return AdoptLocalSecret(new byte[TlsCryptoUtilities.GetHashOutputSize(cryptoHashAlgorithm)]);
	}
}
