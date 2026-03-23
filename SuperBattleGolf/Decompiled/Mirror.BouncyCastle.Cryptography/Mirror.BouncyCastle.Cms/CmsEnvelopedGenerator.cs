using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Kisa;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Ntt;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public abstract class CmsEnvelopedGenerator
{
	internal static readonly short[] rc2Table = new short[256]
	{
		189, 86, 234, 242, 162, 241, 172, 42, 176, 147,
		209, 156, 27, 51, 253, 208, 48, 4, 182, 220,
		125, 223, 50, 75, 247, 203, 69, 155, 49, 187,
		33, 90, 65, 159, 225, 217, 74, 77, 158, 218,
		160, 104, 44, 195, 39, 95, 128, 54, 62, 238,
		251, 149, 26, 254, 206, 168, 52, 169, 19, 240,
		166, 63, 216, 12, 120, 36, 175, 35, 82, 193,
		103, 23, 245, 102, 144, 231, 232, 7, 184, 96,
		72, 230, 30, 83, 243, 146, 164, 114, 140, 8,
		21, 110, 134, 0, 132, 250, 244, 127, 138, 66,
		25, 246, 219, 205, 20, 141, 80, 18, 186, 60,
		6, 78, 236, 179, 53, 17, 161, 136, 142, 43,
		148, 153, 183, 113, 116, 211, 228, 191, 58, 222,
		150, 14, 188, 10, 237, 119, 252, 55, 107, 3,
		121, 137, 98, 198, 215, 192, 210, 124, 106, 139,
		34, 163, 91, 5, 93, 2, 117, 213, 97, 227,
		24, 143, 85, 81, 173, 31, 11, 94, 133, 229,
		194, 87, 99, 202, 61, 108, 180, 197, 204, 112,
		178, 145, 89, 13, 71, 32, 200, 79, 88, 224,
		1, 226, 22, 56, 196, 111, 59, 15, 101, 70,
		190, 126, 45, 123, 130, 249, 64, 181, 29, 115,
		248, 235, 38, 199, 135, 151, 37, 84, 177, 40,
		170, 152, 157, 165, 100, 109, 122, 212, 16, 129,
		68, 239, 73, 214, 174, 46, 221, 118, 92, 47,
		167, 28, 201, 9, 105, 154, 131, 207, 41, 57,
		185, 233, 76, 255, 67, 171
	};

	public static readonly string DesCbc = OiwObjectIdentifiers.DesCbc.Id;

	public static readonly string DesEde3Cbc = PkcsObjectIdentifiers.DesEde3Cbc.Id;

	public static readonly string RC2Cbc = PkcsObjectIdentifiers.RC2Cbc.Id;

	public const string IdeaCbc = "1.3.6.1.4.1.188.7.1.1.2";

	public const string Cast5Cbc = "1.2.840.113533.7.66.10";

	public static readonly string Aes128Cbc = NistObjectIdentifiers.IdAes128Cbc.Id;

	public static readonly string Aes192Cbc = NistObjectIdentifiers.IdAes192Cbc.Id;

	public static readonly string Aes256Cbc = NistObjectIdentifiers.IdAes256Cbc.Id;

	public static readonly string Aes128Ccm = NistObjectIdentifiers.IdAes128Ccm.Id;

	public static readonly string Aes192Ccm = NistObjectIdentifiers.IdAes192Ccm.Id;

	public static readonly string Aes256Ccm = NistObjectIdentifiers.IdAes256Ccm.Id;

	public static readonly string Aes128Gcm = NistObjectIdentifiers.IdAes128Gcm.Id;

	public static readonly string Aes192Gcm = NistObjectIdentifiers.IdAes192Gcm.Id;

	public static readonly string Aes256Gcm = NistObjectIdentifiers.IdAes256Gcm.Id;

	public static readonly string Camellia128Cbc = NttObjectIdentifiers.IdCamellia128Cbc.Id;

	public static readonly string Camellia192Cbc = NttObjectIdentifiers.IdCamellia192Cbc.Id;

	public static readonly string Camellia256Cbc = NttObjectIdentifiers.IdCamellia256Cbc.Id;

	public static readonly string SeedCbc = KisaObjectIdentifiers.IdSeedCbc.Id;

	public static readonly string DesEde3Wrap = PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id;

	public static readonly string Aes128Wrap = NistObjectIdentifiers.IdAes128Wrap.Id;

	public static readonly string Aes192Wrap = NistObjectIdentifiers.IdAes192Wrap.Id;

	public static readonly string Aes256Wrap = NistObjectIdentifiers.IdAes256Wrap.Id;

	public static readonly string Camellia128Wrap = NttObjectIdentifiers.IdCamellia128Wrap.Id;

	public static readonly string Camellia192Wrap = NttObjectIdentifiers.IdCamellia192Wrap.Id;

	public static readonly string Camellia256Wrap = NttObjectIdentifiers.IdCamellia256Wrap.Id;

	public static readonly string SeedWrap = KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id;

	public static readonly string Gost28147Wrap = CryptoProObjectIdentifiers.id_Gost28147_89_None_KeyWrap.Id;

	public static readonly string Gost28147CryptoProWrap = CryptoProObjectIdentifiers.id_Gost28147_89_CryptoPro_KeyWrap.Id;

	public static readonly string ECDHSha1Kdf = X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme.Id;

	public static readonly string ECCDHSha1Kdf = X9ObjectIdentifiers.DHSinglePassCofactorDHSha1KdfScheme.Id;

	public static readonly string ECMqvSha1Kdf = X9ObjectIdentifiers.MqvSinglePassSha1KdfScheme.Id;

	public static readonly string ECDHSha224Kdf = SecObjectIdentifiers.dhSinglePass_stdDH_sha224kdf_scheme.Id;

	public static readonly string ECCDHSha224Kdf = SecObjectIdentifiers.dhSinglePass_cofactorDH_sha224kdf_scheme.Id;

	public static readonly string ECMqvSha224Kdf = SecObjectIdentifiers.mqvSinglePass_sha224kdf_scheme.Id;

	public static readonly string ECDHSha256Kdf = SecObjectIdentifiers.dhSinglePass_stdDH_sha256kdf_scheme.Id;

	public static readonly string ECCDHSha256Kdf = SecObjectIdentifiers.dhSinglePass_cofactorDH_sha256kdf_scheme.Id;

	public static readonly string ECMqvSha256Kdf = SecObjectIdentifiers.mqvSinglePass_sha256kdf_scheme.Id;

	public static readonly string ECDHSha384Kdf = SecObjectIdentifiers.dhSinglePass_stdDH_sha384kdf_scheme.Id;

	public static readonly string ECCDHSha384Kdf = SecObjectIdentifiers.dhSinglePass_cofactorDH_sha384kdf_scheme.Id;

	public static readonly string ECMqvSha384Kdf = SecObjectIdentifiers.mqvSinglePass_sha384kdf_scheme.Id;

	public static readonly string ECDHSha512Kdf = SecObjectIdentifiers.dhSinglePass_stdDH_sha512kdf_scheme.Id;

	public static readonly string ECCDHSha512Kdf = SecObjectIdentifiers.dhSinglePass_cofactorDH_sha512kdf_scheme.Id;

	public static readonly string ECMqvSha512Kdf = SecObjectIdentifiers.mqvSinglePass_sha512kdf_scheme.Id;

	internal readonly IList<RecipientInfoGenerator> recipientInfoGenerators = new List<RecipientInfoGenerator>();

	internal readonly SecureRandom m_random;

	internal CmsAttributeTableGenerator unprotectedAttributeGenerator;

	public CmsAttributeTableGenerator UnprotectedAttributeGenerator
	{
		get
		{
			return unprotectedAttributeGenerator;
		}
		set
		{
			unprotectedAttributeGenerator = value;
		}
	}

	protected CmsEnvelopedGenerator()
		: this(CryptoServicesRegistrar.GetSecureRandom())
	{
	}

	protected CmsEnvelopedGenerator(SecureRandom random)
	{
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		m_random = random;
	}

	public void AddKeyTransRecipient(X509Certificate cert)
	{
		Asn1KeyWrapper keyWrapper = new Asn1KeyWrapper(cert.SubjectPublicKeyInfo.Algorithm, cert);
		AddRecipientInfoGenerator(new KeyTransRecipientInfoGenerator(cert, keyWrapper));
	}

	public void AddKeyTransRecipient(string algorithm, X509Certificate cert)
	{
		Asn1KeyWrapper keyWrapper = new Asn1KeyWrapper(algorithm, cert);
		AddRecipientInfoGenerator(new KeyTransRecipientInfoGenerator(cert, keyWrapper));
	}

	public void AddKeyTransRecipient(AsymmetricKeyParameter pubKey, byte[] subKeyId)
	{
		SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);
		AddRecipientInfoGenerator(new KeyTransRecipientInfoGenerator(subKeyId, new Asn1KeyWrapper(subjectPublicKeyInfo.Algorithm, pubKey)));
	}

	public void AddKeyTransRecipient(string algorithm, AsymmetricKeyParameter pubKey, byte[] subKeyId)
	{
		SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey);
		AddRecipientInfoGenerator(new KeyTransRecipientInfoGenerator(subKeyId, new Asn1KeyWrapper(algorithm, pubKey)));
	}

	public void AddKekRecipient(string keyAlgorithm, KeyParameter key, byte[] keyIdentifier)
	{
		AddKekRecipient(keyAlgorithm, key, new KekIdentifier(keyIdentifier, null, null));
	}

	public void AddKekRecipient(string keyAlgorithm, KeyParameter key, KekIdentifier kekIdentifier)
	{
		KekRecipientInfoGenerator kekRecipientInfoGenerator = new KekRecipientInfoGenerator();
		kekRecipientInfoGenerator.KekIdentifier = kekIdentifier;
		kekRecipientInfoGenerator.KeyEncryptionKeyOID = keyAlgorithm;
		kekRecipientInfoGenerator.KeyEncryptionKey = key;
		recipientInfoGenerators.Add(kekRecipientInfoGenerator);
	}

	public void AddPasswordRecipient(CmsPbeKey pbeKey, string kekAlgorithmOid)
	{
		Pbkdf2Params parameters = new Pbkdf2Params(pbeKey.Salt, pbeKey.IterationCount);
		PasswordRecipientInfoGenerator passwordRecipientInfoGenerator = new PasswordRecipientInfoGenerator();
		passwordRecipientInfoGenerator.KeyDerivationAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdPbkdf2, parameters);
		passwordRecipientInfoGenerator.KeyEncryptionKeyOID = kekAlgorithmOid;
		passwordRecipientInfoGenerator.KeyEncryptionKey = pbeKey.GetEncoded(kekAlgorithmOid);
		recipientInfoGenerators.Add(passwordRecipientInfoGenerator);
	}

	public void AddKeyAgreementRecipient(string agreementAlgorithm, AsymmetricKeyParameter senderPrivateKey, AsymmetricKeyParameter senderPublicKey, X509Certificate recipientCert, string cekWrapAlgorithm)
	{
		List<X509Certificate> recipientCerts = new List<X509Certificate>(1) { recipientCert };
		AddKeyAgreementRecipients(agreementAlgorithm, senderPrivateKey, senderPublicKey, recipientCerts, cekWrapAlgorithm);
	}

	public void AddKeyAgreementRecipients(string agreementAlgorithm, AsymmetricKeyParameter senderPrivateKey, AsymmetricKeyParameter senderPublicKey, IEnumerable<X509Certificate> recipientCerts, string cekWrapAlgorithm)
	{
		if (!senderPrivateKey.IsPrivate)
		{
			throw new ArgumentException("Expected private key", "senderPrivateKey");
		}
		if (senderPublicKey.IsPrivate)
		{
			throw new ArgumentException("Expected public key", "senderPublicKey");
		}
		recipientInfoGenerators.Add(new KeyAgreeRecipientInfoGenerator(recipientCerts)
		{
			KeyAgreementOid = new DerObjectIdentifier(agreementAlgorithm),
			KeyEncryptionOid = new DerObjectIdentifier(cekWrapAlgorithm),
			SenderKeyPair = new AsymmetricCipherKeyPair(senderPublicKey, senderPrivateKey)
		});
	}

	public void AddKeyAgreementRecipient(string agreementAlgorithm, AsymmetricKeyParameter senderPrivateKey, AsymmetricKeyParameter senderPublicKey, byte[] recipientKeyID, AsymmetricKeyParameter recipientPublicKey, string cekWrapAlgorithm)
	{
		if (!senderPrivateKey.IsPrivate)
		{
			throw new ArgumentException("Expected private key", "senderPrivateKey");
		}
		if (senderPublicKey.IsPrivate)
		{
			throw new ArgumentException("Expected public key", "senderPublicKey");
		}
		if (recipientPublicKey.IsPrivate)
		{
			throw new ArgumentException("Expected public key", "recipientPublicKey");
		}
		recipientInfoGenerators.Add(new KeyAgreeRecipientInfoGenerator(recipientKeyID, recipientPublicKey)
		{
			KeyAgreementOid = new DerObjectIdentifier(agreementAlgorithm),
			KeyEncryptionOid = new DerObjectIdentifier(cekWrapAlgorithm),
			SenderKeyPair = new AsymmetricCipherKeyPair(senderPublicKey, senderPrivateKey)
		});
	}

	public void AddRecipientInfoGenerator(RecipientInfoGenerator recipientInfoGenerator)
	{
		recipientInfoGenerators.Add(recipientInfoGenerator);
	}

	protected internal virtual AlgorithmIdentifier GetAlgorithmIdentifier(string encryptionOid, KeyParameter encKey, Asn1Encodable asn1Params, out ICipherParameters cipherParameters)
	{
		Asn1Object asn1Object;
		if (asn1Params != null)
		{
			asn1Object = asn1Params.ToAsn1Object();
			cipherParameters = ParameterUtilities.GetCipherParameters(encryptionOid, encKey, asn1Object);
		}
		else
		{
			asn1Object = DerNull.Instance;
			cipherParameters = encKey;
		}
		return new AlgorithmIdentifier(new DerObjectIdentifier(encryptionOid), asn1Object);
	}

	protected internal virtual Asn1Encodable GenerateAsn1Parameters(string encryptionOid, byte[] encKeyBytes)
	{
		Asn1Encodable result = null;
		try
		{
			if (encryptionOid.Equals(RC2Cbc))
			{
				byte[] array = new byte[8];
				m_random.NextBytes(array);
				int num = encKeyBytes.Length * 8;
				int parameterVersion = ((num >= 256) ? num : rc2Table[num]);
				result = new RC2CbcParameter(parameterVersion, array);
			}
			else
			{
				result = ParameterUtilities.GenerateParameters(encryptionOid, m_random);
			}
		}
		catch (SecurityUtilityException)
		{
		}
		return result;
	}
}
