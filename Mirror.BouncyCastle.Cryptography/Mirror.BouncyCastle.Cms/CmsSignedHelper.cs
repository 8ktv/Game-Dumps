using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Eac;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

internal static class CmsSignedHelper
{
	private static readonly Dictionary<DerObjectIdentifier, string> m_encryptionAlgs;

	private static readonly Dictionary<DerObjectIdentifier, string> m_digestAlgs;

	private static readonly Dictionary<string, string[]> m_digestAliases;

	private static readonly HashSet<DerObjectIdentifier> m_noParams;

	private static readonly Dictionary<string, DerObjectIdentifier> m_ecAlgorithms;

	private static void AddEntries(DerObjectIdentifier oid, string digest, string encryption)
	{
		m_digestAlgs.Add(oid, digest);
		m_encryptionAlgs.Add(oid, encryption);
	}

	static CmsSignedHelper()
	{
		m_encryptionAlgs = new Dictionary<DerObjectIdentifier, string>();
		m_digestAlgs = new Dictionary<DerObjectIdentifier, string>();
		m_digestAliases = new Dictionary<string, string[]>();
		m_noParams = new HashSet<DerObjectIdentifier>();
		m_ecAlgorithms = new Dictionary<string, DerObjectIdentifier>();
		AddEntries(NistObjectIdentifiers.DsaWithSha224, "SHA224", "DSA");
		AddEntries(NistObjectIdentifiers.DsaWithSha256, "SHA256", "DSA");
		AddEntries(NistObjectIdentifiers.DsaWithSha384, "SHA384", "DSA");
		AddEntries(NistObjectIdentifiers.DsaWithSha512, "SHA512", "DSA");
		AddEntries(OiwObjectIdentifiers.DsaWithSha1, "SHA1", "DSA");
		AddEntries(OiwObjectIdentifiers.MD4WithRsa, "MD4", "RSA");
		AddEntries(OiwObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
		AddEntries(OiwObjectIdentifiers.MD5WithRsa, "MD5", "RSA");
		AddEntries(OiwObjectIdentifiers.Sha1WithRsa, "SHA1", "RSA");
		AddEntries(PkcsObjectIdentifiers.MD2WithRsaEncryption, "MD2", "RSA");
		AddEntries(PkcsObjectIdentifiers.MD4WithRsaEncryption, "MD4", "RSA");
		AddEntries(PkcsObjectIdentifiers.MD5WithRsaEncryption, "MD5", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha1WithRsaEncryption, "SHA1", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha224WithRsaEncryption, "SHA224", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha256WithRsaEncryption, "SHA256", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha384WithRsaEncryption, "SHA384", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha512WithRsaEncryption, "SHA512", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha512_224WithRSAEncryption, "SHA512(224)", "RSA");
		AddEntries(PkcsObjectIdentifiers.Sha512_256WithRSAEncryption, "SHA512(256)", "RSA");
		AddEntries(NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224, "SHA3-224", "RSA");
		AddEntries(NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256, "SHA3-256", "RSA");
		AddEntries(NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384, "SHA3-384", "RSA");
		AddEntries(NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512, "SHA3-512", "RSA");
		AddEntries(X9ObjectIdentifiers.ECDsaWithSha1, "SHA1", "ECDSA");
		AddEntries(X9ObjectIdentifiers.ECDsaWithSha224, "SHA224", "ECDSA");
		AddEntries(X9ObjectIdentifiers.ECDsaWithSha256, "SHA256", "ECDSA");
		AddEntries(X9ObjectIdentifiers.ECDsaWithSha384, "SHA384", "ECDSA");
		AddEntries(X9ObjectIdentifiers.ECDsaWithSha512, "SHA512", "ECDSA");
		AddEntries(X9ObjectIdentifiers.IdDsaWithSha1, "SHA1", "DSA");
		AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_1, "SHA1", "ECDSA");
		AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_224, "SHA224", "ECDSA");
		AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_256, "SHA256", "ECDSA");
		AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_384, "SHA384", "ECDSA");
		AddEntries(EacObjectIdentifiers.id_TA_ECDSA_SHA_512, "SHA512", "ECDSA");
		AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_1, "SHA1", "RSA");
		AddEntries(EacObjectIdentifiers.id_TA_RSA_v1_5_SHA_256, "SHA256", "RSA");
		AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_1, "SHA1", "RSAandMGF1");
		AddEntries(EacObjectIdentifiers.id_TA_RSA_PSS_SHA_256, "SHA256", "RSAandMGF1");
		AddEntries(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94, "GOST3411", "GOST3410");
		AddEntries(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001, "GOST3411", "ECGOST3410");
		AddEntries(RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256, "GOST3411-2012-256", "ECGOST3410");
		AddEntries(RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512, "GOST3411-2012-512", "ECGOST3410");
		m_encryptionAlgs.Add(X9ObjectIdentifiers.IdDsa, "DSA");
		m_encryptionAlgs.Add(PkcsObjectIdentifiers.RsaEncryption, "RSA");
		m_encryptionAlgs.Add(TeleTrusTObjectIdentifiers.TeleTrusTRsaSignatureAlgorithm, "RSA");
		m_encryptionAlgs.Add(X509ObjectIdentifiers.IdEARsa, "RSA");
		m_encryptionAlgs.Add(PkcsObjectIdentifiers.IdRsassaPss, "RSAandMGF1");
		m_encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x94, "GOST3410");
		m_encryptionAlgs.Add(CryptoProObjectIdentifiers.GostR3410x2001, "ECGOST3410");
		m_encryptionAlgs.Add(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256, "ECGOST3410");
		m_encryptionAlgs.Add(RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512, "ECGOST3410");
		m_encryptionAlgs.Add(new DerObjectIdentifier("1.3.6.1.4.1.5849.1.6.2"), "ECGOST3410");
		m_encryptionAlgs.Add(new DerObjectIdentifier("1.3.6.1.4.1.5849.1.1.5"), "GOST3410");
		m_encryptionAlgs.Add(X9ObjectIdentifiers.IdECPublicKey, "ECDSA");
		m_digestAlgs.Add(PkcsObjectIdentifiers.MD2, "MD2");
		m_digestAlgs.Add(PkcsObjectIdentifiers.MD4, "MD4");
		m_digestAlgs.Add(PkcsObjectIdentifiers.MD5, "MD5");
		m_digestAlgs.Add(OiwObjectIdentifiers.IdSha1, "SHA1");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha224, "SHA224");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha256, "SHA256");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha384, "SHA384");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha512, "SHA512");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha512_224, "SHA512(224)");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha512_256, "SHA512(256)");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha3_224, "SHA3-224");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha3_256, "SHA3-256");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha3_384, "SHA3-384");
		m_digestAlgs.Add(NistObjectIdentifiers.IdSha3_512, "SHA3-512");
		m_digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD128, "RIPEMD128");
		m_digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD160, "RIPEMD160");
		m_digestAlgs.Add(TeleTrusTObjectIdentifiers.RipeMD256, "RIPEMD256");
		m_digestAlgs.Add(CryptoProObjectIdentifiers.GostR3411, "GOST3411");
		m_digestAlgs.Add(new DerObjectIdentifier("1.3.6.1.4.1.5849.1.2.1"), "GOST3411");
		m_digestAlgs.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256, "GOST3411-2012-256");
		m_digestAlgs.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512, "GOST3411-2012-512");
		m_digestAliases.Add("SHA1", new string[1] { "SHA-1" });
		m_digestAliases.Add("SHA224", new string[1] { "SHA-224" });
		m_digestAliases.Add("SHA256", new string[1] { "SHA-256" });
		m_digestAliases.Add("SHA384", new string[1] { "SHA-384" });
		m_digestAliases.Add("SHA512", new string[1] { "SHA-512" });
		m_noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
		m_noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
		m_ecAlgorithms.Add(CmsSignedGenerator.DigestSha1, X9ObjectIdentifiers.ECDsaWithSha1);
		m_ecAlgorithms.Add(CmsSignedGenerator.DigestSha224, X9ObjectIdentifiers.ECDsaWithSha224);
		m_ecAlgorithms.Add(CmsSignedGenerator.DigestSha256, X9ObjectIdentifiers.ECDsaWithSha256);
		m_ecAlgorithms.Add(CmsSignedGenerator.DigestSha384, X9ObjectIdentifiers.ECDsaWithSha384);
		m_ecAlgorithms.Add(CmsSignedGenerator.DigestSha512, X9ObjectIdentifiers.ECDsaWithSha512);
	}

	internal static string GetDigestAlgName(DerObjectIdentifier digestOid)
	{
		if (m_digestAlgs.TryGetValue(digestOid, out var value))
		{
			return value;
		}
		return digestOid.Id;
	}

	internal static AlgorithmIdentifier GetEncAlgorithmIdentifier(DerObjectIdentifier encOid, Asn1Encodable sigX509Parameters)
	{
		if (m_noParams.Contains(encOid))
		{
			return new AlgorithmIdentifier(encOid);
		}
		return new AlgorithmIdentifier(encOid, sigX509Parameters);
	}

	internal static string[] GetDigestAliases(string algName)
	{
		if (!m_digestAliases.TryGetValue(algName, out var value))
		{
			return new string[0];
		}
		return (string[])value.Clone();
	}

	internal static string GetEncryptionAlgName(DerObjectIdentifier encryptionOid)
	{
		if (m_encryptionAlgs.TryGetValue(encryptionOid, out var value))
		{
			return value;
		}
		return encryptionOid.Id;
	}

	internal static IDigest GetDigestInstance(string algorithm)
	{
		try
		{
			return DigestUtilities.GetDigest(algorithm);
		}
		catch (SecurityUtilityException)
		{
			string[] digestAliases = GetDigestAliases(algorithm);
			foreach (string algorithm2 in digestAliases)
			{
				try
				{
					return DigestUtilities.GetDigest(algorithm2);
				}
				catch (SecurityUtilityException)
				{
				}
			}
			throw;
		}
	}

	internal static ISigner GetSignatureInstance(string algorithm)
	{
		return SignerUtilities.GetSigner(algorithm);
	}

	internal static AlgorithmIdentifier FixDigestAlgID(AlgorithmIdentifier algID, IDigestAlgorithmFinder digestAlgorithmFinder)
	{
		Asn1Encodable parameters = algID.Parameters;
		if (parameters == null || DerNull.Instance.Equals(parameters))
		{
			return digestAlgorithmFinder.Find(algID.Algorithm);
		}
		return algID;
	}

	internal static DerObjectIdentifier GetEncOid(AsymmetricKeyParameter key, string digestOID)
	{
		DerObjectIdentifier value = null;
		if (key is RsaKeyParameters rsaKeyParameters)
		{
			if (!rsaKeyParameters.IsPrivate)
			{
				throw new ArgumentException("Expected RSA private key");
			}
			value = PkcsObjectIdentifiers.RsaEncryption;
		}
		else if (key is DsaPrivateKeyParameters)
		{
			if (digestOID.Equals(CmsSignedGenerator.DigestSha1))
			{
				value = X9ObjectIdentifiers.IdDsaWithSha1;
			}
			else if (digestOID.Equals(CmsSignedGenerator.DigestSha224))
			{
				value = NistObjectIdentifiers.DsaWithSha224;
			}
			else if (digestOID.Equals(CmsSignedGenerator.DigestSha256))
			{
				value = NistObjectIdentifiers.DsaWithSha256;
			}
			else if (digestOID.Equals(CmsSignedGenerator.DigestSha384))
			{
				value = NistObjectIdentifiers.DsaWithSha384;
			}
			else
			{
				if (!digestOID.Equals(CmsSignedGenerator.DigestSha512))
				{
					throw new ArgumentException("can't mix DSA with anything but SHA1/SHA2");
				}
				value = NistObjectIdentifiers.DsaWithSha512;
			}
		}
		else if (key is ECPrivateKeyParameters eCPrivateKeyParameters)
		{
			if (eCPrivateKeyParameters.AlgorithmName == "ECGOST3410")
			{
				value = CryptoProObjectIdentifiers.GostR3410x2001;
			}
			else if (eCPrivateKeyParameters.Parameters is ECGost3410Parameters { DigestParamSet: var digestParamSet })
			{
				if (digestParamSet.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256))
				{
					value = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_256;
				}
				else
				{
					if (!digestParamSet.Equals(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512))
					{
						throw new ArgumentException("can't determine GOST3410 algorithm");
					}
					value = RosstandartObjectIdentifiers.id_tc26_gost_3410_12_512;
				}
			}
			else if (!m_ecAlgorithms.TryGetValue(digestOID, out value))
			{
				throw new ArgumentException("can't mix ECDSA with anything but SHA family digests");
			}
		}
		else
		{
			if (!(key is Gost3410PrivateKeyParameters))
			{
				throw new ArgumentException("Unknown algorithm in CmsSignedGenerator.GetEncOid");
			}
			value = CryptoProObjectIdentifiers.GostR3410x94;
		}
		return value;
	}

	internal static IStore<X509V2AttributeCertificate> GetAttributeCertificates(Asn1Set attrCertSet)
	{
		List<X509V2AttributeCertificate> list = new List<X509V2AttributeCertificate>();
		if (attrCertSet != null)
		{
			foreach (Asn1Encodable item in attrCertSet)
			{
				if (item.ToAsn1Object() is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasContextTag(2))
				{
					AttributeCertificate instance = AttributeCertificate.GetInstance(asn1TaggedObject, declaredExplicit: false);
					list.Add(new X509V2AttributeCertificate(instance));
				}
			}
		}
		return CollectionUtilities.CreateStore(list);
	}

	internal static IStore<X509Certificate> GetCertificates(Asn1Set certSet)
	{
		List<X509Certificate> list = new List<X509Certificate>();
		if (certSet != null)
		{
			foreach (Asn1Encodable item in certSet)
			{
				if (item is X509CertificateStructure c)
				{
					list.Add(new X509Certificate(c));
				}
				else if (item.ToAsn1Object() is Asn1Sequence obj)
				{
					list.Add(new X509Certificate(X509CertificateStructure.GetInstance(obj)));
				}
			}
		}
		return CollectionUtilities.CreateStore(list);
	}

	internal static IStore<X509Crl> GetCrls(Asn1Set crlSet)
	{
		List<X509Crl> list = new List<X509Crl>();
		if (crlSet != null)
		{
			foreach (Asn1Encodable item in crlSet)
			{
				if (item is CertificateList c)
				{
					list.Add(new X509Crl(c));
				}
				else if (item.ToAsn1Object() is Asn1Sequence obj)
				{
					list.Add(new X509Crl(CertificateList.GetInstance(obj)));
				}
			}
		}
		return CollectionUtilities.CreateStore(list);
	}

	internal static IStore<Asn1Encodable> GetOtherRevInfos(Asn1Set crlSet, DerObjectIdentifier infoFormat)
	{
		List<Asn1Encodable> list = new List<Asn1Encodable>();
		if (crlSet != null && infoFormat != null)
		{
			foreach (Asn1Encodable item in crlSet)
			{
				if (item.ToAsn1Object() is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasContextTag(1))
				{
					OtherRevocationInfoFormat instance = OtherRevocationInfoFormat.GetInstance(asn1TaggedObject, isExplicit: false);
					if (infoFormat.Equals(instance.InfoFormat))
					{
						list.Add(instance.Info);
					}
				}
			}
		}
		return CollectionUtilities.CreateStore(list);
	}
}
