using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Bsi;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Eac;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.GM;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Signers;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class SignerUtilities
{
	private static readonly Dictionary<string, string> AlgorithmMap;

	private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap;

	private static readonly HashSet<string> NoRandom;

	private static readonly Dictionary<string, DerObjectIdentifier> Oids;

	public static ICollection<string> Algorithms => CollectionUtilities.ReadOnly(Oids.Keys);

	static SignerUtilities()
	{
		AlgorithmMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		AlgorithmOidMap = new Dictionary<DerObjectIdentifier, string>();
		NoRandom = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		Oids = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		AlgorithmMap["MD2WITHRSA"] = "MD2withRSA";
		AlgorithmMap["MD2WITHRSAENCRYPTION"] = "MD2withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.MD2WithRsaEncryption] = "MD2withRSA";
		AlgorithmMap["MD4WITHRSA"] = "MD4withRSA";
		AlgorithmMap["MD4WITHRSAENCRYPTION"] = "MD4withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.MD4WithRsaEncryption] = "MD4withRSA";
		AlgorithmOidMap[OiwObjectIdentifiers.MD4WithRsa] = "MD4withRSA";
		AlgorithmOidMap[OiwObjectIdentifiers.MD4WithRsaEncryption] = "MD4withRSA";
		AlgorithmMap["MD5WITHRSA"] = "MD5withRSA";
		AlgorithmMap["MD5WITHRSAENCRYPTION"] = "MD5withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.MD5WithRsaEncryption] = "MD5withRSA";
		AlgorithmOidMap[OiwObjectIdentifiers.MD5WithRsa] = "MD5withRSA";
		AlgorithmMap["SHA1WITHRSA"] = "SHA-1withRSA";
		AlgorithmMap["SHA-1WITHRSA"] = "SHA-1withRSA";
		AlgorithmMap["SHA1WITHRSAENCRYPTION"] = "SHA-1withRSA";
		AlgorithmMap["SHA-1WITHRSAENCRYPTION"] = "SHA-1withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha1WithRsaEncryption] = "SHA-1withRSA";
		AlgorithmOidMap[OiwObjectIdentifiers.Sha1WithRsa] = "SHA-1withRSA";
		AlgorithmMap["SHA224WITHRSA"] = "SHA-224withRSA";
		AlgorithmMap["SHA-224WITHRSA"] = "SHA-224withRSA";
		AlgorithmMap["SHA224WITHRSAENCRYPTION"] = "SHA-224withRSA";
		AlgorithmMap["SHA-224WITHRSAENCRYPTION"] = "SHA-224withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha224WithRsaEncryption] = "SHA-224withRSA";
		AlgorithmMap["SHA256WITHRSA"] = "SHA-256withRSA";
		AlgorithmMap["SHA-256WITHRSA"] = "SHA-256withRSA";
		AlgorithmMap["SHA256WITHRSAENCRYPTION"] = "SHA-256withRSA";
		AlgorithmMap["SHA-256WITHRSAENCRYPTION"] = "SHA-256withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha256WithRsaEncryption] = "SHA-256withRSA";
		AlgorithmMap["SHA384WITHRSA"] = "SHA-384withRSA";
		AlgorithmMap["SHA-384WITHRSA"] = "SHA-384withRSA";
		AlgorithmMap["SHA384WITHRSAENCRYPTION"] = "SHA-384withRSA";
		AlgorithmMap["SHA-384WITHRSAENCRYPTION"] = "SHA-384withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha384WithRsaEncryption] = "SHA-384withRSA";
		AlgorithmMap["SHA512WITHRSA"] = "SHA-512withRSA";
		AlgorithmMap["SHA-512WITHRSA"] = "SHA-512withRSA";
		AlgorithmMap["SHA512WITHRSAENCRYPTION"] = "SHA-512withRSA";
		AlgorithmMap["SHA-512WITHRSAENCRYPTION"] = "SHA-512withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha512WithRsaEncryption] = "SHA-512withRSA";
		AlgorithmMap["SHA512(224)WITHRSA"] = "SHA-512(224)withRSA";
		AlgorithmMap["SHA-512(224)WITHRSA"] = "SHA-512(224)withRSA";
		AlgorithmMap["SHA512(224)WITHRSAENCRYPTION"] = "SHA-512(224)withRSA";
		AlgorithmMap["SHA-512(224)WITHRSAENCRYPTION"] = "SHA-512(224)withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha512_224WithRSAEncryption] = "SHA-512(224)withRSA";
		AlgorithmMap["SHA512(256)WITHRSA"] = "SHA-512(256)withRSA";
		AlgorithmMap["SHA-512(256)WITHRSA"] = "SHA-512(256)withRSA";
		AlgorithmMap["SHA512(256)WITHRSAENCRYPTION"] = "SHA-512(256)withRSA";
		AlgorithmMap["SHA-512(256)WITHRSAENCRYPTION"] = "SHA-512(256)withRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.Sha512_256WithRSAEncryption] = "SHA-512(256)withRSA";
		AlgorithmMap["SHA3-224WITHRSA"] = "SHA3-224withRSA";
		AlgorithmMap["SHA3-224WITHRSAENCRYPTION"] = "SHA3-224withRSA";
		AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224] = "SHA3-224withRSA";
		AlgorithmMap["SHA3-256WITHRSA"] = "SHA3-256withRSA";
		AlgorithmMap["SHA3-256WITHRSAENCRYPTION"] = "SHA3-256withRSA";
		AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256] = "SHA3-256withRSA";
		AlgorithmMap["SHA3-384WITHRSA"] = "SHA3-384withRSA";
		AlgorithmMap["SHA3-384WITHRSAENCRYPTION"] = "SHA3-384withRSA";
		AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384] = "SHA3-384withRSA";
		AlgorithmMap["SHA3-512WITHRSA"] = "SHA3-512withRSA";
		AlgorithmMap["SHA3-512WITHRSAENCRYPTION"] = "SHA3-512withRSA";
		AlgorithmOidMap[NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512] = "SHA3-512withRSA";
		AlgorithmMap["PSSWITHRSA"] = "PSSwithRSA";
		AlgorithmMap["RSASSA-PSS"] = "PSSwithRSA";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdRsassaPss] = "PSSwithRSA";
		AlgorithmMap["RSAPSS"] = "PSSwithRSA";
		AlgorithmMap["SHA1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA-1WITHRSAANDMGF1"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA-1WITHRSA/PSS"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA1WITHRSASSA-PSS"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA-1WITHRSASSA-PSS"] = "SHA-1withRSAandMGF1";
		AlgorithmMap["SHA224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA-224WITHRSAANDMGF1"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA-224WITHRSA/PSS"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA224WITHRSASSA-PSS"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA-224WITHRSASSA-PSS"] = "SHA-224withRSAandMGF1";
		AlgorithmMap["SHA256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA-256WITHRSAANDMGF1"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA-256WITHRSA/PSS"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA256WITHRSASSA-PSS"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA-256WITHRSASSA-PSS"] = "SHA-256withRSAandMGF1";
		AlgorithmMap["SHA384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA-384WITHRSAANDMGF1"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA-384WITHRSA/PSS"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA384WITHRSASSA-PSS"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA-384WITHRSASSA-PSS"] = "SHA-384withRSAandMGF1";
		AlgorithmMap["SHA512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["SHA-512WITHRSAANDMGF1"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["SHA512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["SHA-512WITHRSA/PSS"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["SHA512WITHRSASSA-PSS"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["SHA-512WITHRSASSA-PSS"] = "SHA-512withRSAandMGF1";
		AlgorithmMap["RIPEMD128WITHRSA"] = "RIPEMD128withRSA";
		AlgorithmMap["RIPEMD128WITHRSAENCRYPTION"] = "RIPEMD128withRSA";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128] = "RIPEMD128withRSA";
		AlgorithmMap["RIPEMD160WITHRSA"] = "RIPEMD160withRSA";
		AlgorithmMap["RIPEMD160WITHRSAENCRYPTION"] = "RIPEMD160withRSA";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160] = "RIPEMD160withRSA";
		AlgorithmMap["RIPEMD256WITHRSA"] = "RIPEMD256withRSA";
		AlgorithmMap["RIPEMD256WITHRSAENCRYPTION"] = "RIPEMD256withRSA";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256] = "RIPEMD256withRSA";
		AlgorithmMap["NONEWITHRSA"] = "RSA";
		AlgorithmMap["RSAWITHNONE"] = "RSA";
		AlgorithmMap["RAWRSA"] = "RSA";
		AlgorithmMap["RAWRSAPSS"] = "RAWRSASSA-PSS";
		AlgorithmMap["NONEWITHRSAPSS"] = "RAWRSASSA-PSS";
		AlgorithmMap["NONEWITHRSASSA-PSS"] = "RAWRSASSA-PSS";
		AlgorithmMap["NONEWITHDSA"] = "NONEwithDSA";
		AlgorithmMap["DSAWITHNONE"] = "NONEwithDSA";
		AlgorithmMap["RAWDSA"] = "NONEwithDSA";
		AlgorithmMap["DSA"] = "SHA-1withDSA";
		AlgorithmMap["DSAWITHSHA1"] = "SHA-1withDSA";
		AlgorithmMap["DSAWITHSHA-1"] = "SHA-1withDSA";
		AlgorithmMap["SHA/DSA"] = "SHA-1withDSA";
		AlgorithmMap["SHA1/DSA"] = "SHA-1withDSA";
		AlgorithmMap["SHA-1/DSA"] = "SHA-1withDSA";
		AlgorithmMap["SHA1WITHDSA"] = "SHA-1withDSA";
		AlgorithmMap["SHA-1WITHDSA"] = "SHA-1withDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.IdDsaWithSha1] = "SHA-1withDSA";
		AlgorithmOidMap[OiwObjectIdentifiers.DsaWithSha1] = "SHA-1withDSA";
		AlgorithmMap["DSAWITHSHA224"] = "SHA-224withDSA";
		AlgorithmMap["DSAWITHSHA-224"] = "SHA-224withDSA";
		AlgorithmMap["SHA224/DSA"] = "SHA-224withDSA";
		AlgorithmMap["SHA-224/DSA"] = "SHA-224withDSA";
		AlgorithmMap["SHA224WITHDSA"] = "SHA-224withDSA";
		AlgorithmMap["SHA-224WITHDSA"] = "SHA-224withDSA";
		AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha224] = "SHA-224withDSA";
		AlgorithmMap["DSAWITHSHA256"] = "SHA-256withDSA";
		AlgorithmMap["DSAWITHSHA-256"] = "SHA-256withDSA";
		AlgorithmMap["SHA256/DSA"] = "SHA-256withDSA";
		AlgorithmMap["SHA-256/DSA"] = "SHA-256withDSA";
		AlgorithmMap["SHA256WITHDSA"] = "SHA-256withDSA";
		AlgorithmMap["SHA-256WITHDSA"] = "SHA-256withDSA";
		AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha256] = "SHA-256withDSA";
		AlgorithmMap["DSAWITHSHA384"] = "SHA-384withDSA";
		AlgorithmMap["DSAWITHSHA-384"] = "SHA-384withDSA";
		AlgorithmMap["SHA384/DSA"] = "SHA-384withDSA";
		AlgorithmMap["SHA-384/DSA"] = "SHA-384withDSA";
		AlgorithmMap["SHA384WITHDSA"] = "SHA-384withDSA";
		AlgorithmMap["SHA-384WITHDSA"] = "SHA-384withDSA";
		AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha384] = "SHA-384withDSA";
		AlgorithmMap["DSAWITHSHA512"] = "SHA-512withDSA";
		AlgorithmMap["DSAWITHSHA-512"] = "SHA-512withDSA";
		AlgorithmMap["SHA512/DSA"] = "SHA-512withDSA";
		AlgorithmMap["SHA-512/DSA"] = "SHA-512withDSA";
		AlgorithmMap["SHA512WITHDSA"] = "SHA-512withDSA";
		AlgorithmMap["SHA-512WITHDSA"] = "SHA-512withDSA";
		AlgorithmOidMap[NistObjectIdentifiers.DsaWithSha512] = "SHA-512withDSA";
		AlgorithmMap["NONEWITHECDSA"] = "NONEwithECDSA";
		AlgorithmMap["ECDSAWITHNONE"] = "NONEwithECDSA";
		AlgorithmMap["ECDSA"] = "SHA-1withECDSA";
		AlgorithmMap["SHA1/ECDSA"] = "SHA-1withECDSA";
		AlgorithmMap["SHA-1/ECDSA"] = "SHA-1withECDSA";
		AlgorithmMap["ECDSAWITHSHA1"] = "SHA-1withECDSA";
		AlgorithmMap["ECDSAWITHSHA-1"] = "SHA-1withECDSA";
		AlgorithmMap["SHA1WITHECDSA"] = "SHA-1withECDSA";
		AlgorithmMap["SHA-1WITHECDSA"] = "SHA-1withECDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha1] = "SHA-1withECDSA";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.ECSignWithSha1] = "SHA-1withECDSA";
		AlgorithmMap["SHA224/ECDSA"] = "SHA-224withECDSA";
		AlgorithmMap["SHA-224/ECDSA"] = "SHA-224withECDSA";
		AlgorithmMap["ECDSAWITHSHA224"] = "SHA-224withECDSA";
		AlgorithmMap["ECDSAWITHSHA-224"] = "SHA-224withECDSA";
		AlgorithmMap["SHA224WITHECDSA"] = "SHA-224withECDSA";
		AlgorithmMap["SHA-224WITHECDSA"] = "SHA-224withECDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha224] = "SHA-224withECDSA";
		AlgorithmMap["SHA256/ECDSA"] = "SHA-256withECDSA";
		AlgorithmMap["SHA-256/ECDSA"] = "SHA-256withECDSA";
		AlgorithmMap["ECDSAWITHSHA256"] = "SHA-256withECDSA";
		AlgorithmMap["ECDSAWITHSHA-256"] = "SHA-256withECDSA";
		AlgorithmMap["SHA256WITHECDSA"] = "SHA-256withECDSA";
		AlgorithmMap["SHA-256WITHECDSA"] = "SHA-256withECDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha256] = "SHA-256withECDSA";
		AlgorithmMap["SHA384/ECDSA"] = "SHA-384withECDSA";
		AlgorithmMap["SHA-384/ECDSA"] = "SHA-384withECDSA";
		AlgorithmMap["ECDSAWITHSHA384"] = "SHA-384withECDSA";
		AlgorithmMap["ECDSAWITHSHA-384"] = "SHA-384withECDSA";
		AlgorithmMap["SHA384WITHECDSA"] = "SHA-384withECDSA";
		AlgorithmMap["SHA-384WITHECDSA"] = "SHA-384withECDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha384] = "SHA-384withECDSA";
		AlgorithmMap["SHA512/ECDSA"] = "SHA-512withECDSA";
		AlgorithmMap["SHA-512/ECDSA"] = "SHA-512withECDSA";
		AlgorithmMap["ECDSAWITHSHA512"] = "SHA-512withECDSA";
		AlgorithmMap["ECDSAWITHSHA-512"] = "SHA-512withECDSA";
		AlgorithmMap["SHA512WITHECDSA"] = "SHA-512withECDSA";
		AlgorithmMap["SHA-512WITHECDSA"] = "SHA-512withECDSA";
		AlgorithmOidMap[X9ObjectIdentifiers.ECDsaWithSha512] = "SHA-512withECDSA";
		AlgorithmMap["RIPEMD160/ECDSA"] = "RIPEMD160withECDSA";
		AlgorithmMap["ECDSAWITHRIPEMD160"] = "RIPEMD160withECDSA";
		AlgorithmMap["RIPEMD160WITHECDSA"] = "RIPEMD160withECDSA";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.ECSignWithRipeMD160] = "RIPEMD160withECDSA";
		AlgorithmMap["NONEWITHCVC-ECDSA"] = "NONEwithCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHNONE"] = "NONEwithCVC-ECDSA";
		AlgorithmMap["SHA1/CVC-ECDSA"] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["SHA-1/CVC-ECDSA"] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA1"] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA-1"] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["SHA1WITHCVC-ECDSA"] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["SHA-1WITHCVC-ECDSA"] = "SHA-1withCVC-ECDSA";
		AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_1] = "SHA-1withCVC-ECDSA";
		AlgorithmMap["SHA224/CVC-ECDSA"] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["SHA-224/CVC-ECDSA"] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA224"] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA-224"] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["SHA224WITHCVC-ECDSA"] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["SHA-224WITHCVC-ECDSA"] = "SHA-224withCVC-ECDSA";
		AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_224] = "SHA-224withCVC-ECDSA";
		AlgorithmMap["SHA256/CVC-ECDSA"] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["SHA-256/CVC-ECDSA"] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA256"] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA-256"] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["SHA256WITHCVC-ECDSA"] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["SHA-256WITHCVC-ECDSA"] = "SHA-256withCVC-ECDSA";
		AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_256] = "SHA-256withCVC-ECDSA";
		AlgorithmMap["SHA384/CVC-ECDSA"] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["SHA-384/CVC-ECDSA"] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA384"] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA-384"] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["SHA384WITHCVC-ECDSA"] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["SHA-384WITHCVC-ECDSA"] = "SHA-384withCVC-ECDSA";
		AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_384] = "SHA-384withCVC-ECDSA";
		AlgorithmMap["SHA512/CVC-ECDSA"] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["SHA-512/CVC-ECDSA"] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA512"] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["CVC-ECDSAWITHSHA-512"] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["SHA512WITHCVC-ECDSA"] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["SHA-512WITHCVC-ECDSA"] = "SHA-512withCVC-ECDSA";
		AlgorithmOidMap[EacObjectIdentifiers.id_TA_ECDSA_SHA_512] = "SHA-512withCVC-ECDSA";
		AlgorithmMap["NONEWITHPLAIN-ECDSA"] = "NONEwithPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHNONE"] = "NONEwithPLAIN-ECDSA";
		AlgorithmMap["SHA1/PLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["SHA-1/PLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA1"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA-1"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["SHA1WITHPLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["SHA-1WITHPLAIN-ECDSA"] = "SHA-1withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA1] = "SHA-1withPLAIN-ECDSA";
		AlgorithmMap["SHA224/PLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["SHA-224/PLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA224"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA-224"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["SHA224WITHPLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["SHA-224WITHPLAIN-ECDSA"] = "SHA-224withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA224] = "SHA-224withPLAIN-ECDSA";
		AlgorithmMap["SHA256/PLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["SHA-256/PLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA256"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA-256"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["SHA256WITHPLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["SHA-256WITHPLAIN-ECDSA"] = "SHA-256withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA256] = "SHA-256withPLAIN-ECDSA";
		AlgorithmMap["SHA384/PLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["SHA-384/PLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA384"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA-384"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["SHA384WITHPLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["SHA-384WITHPLAIN-ECDSA"] = "SHA-384withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA384] = "SHA-384withPLAIN-ECDSA";
		AlgorithmMap["SHA512/PLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["SHA-512/PLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA512"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHSHA-512"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["SHA512WITHPLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["SHA-512WITHPLAIN-ECDSA"] = "SHA-512withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_SHA512] = "SHA-512withPLAIN-ECDSA";
		AlgorithmMap["RIPEMD160/PLAIN-ECDSA"] = "RIPEMD160withPLAIN-ECDSA";
		AlgorithmMap["PLAIN-ECDSAWITHRIPEMD160"] = "RIPEMD160withPLAIN-ECDSA";
		AlgorithmMap["RIPEMD160WITHPLAIN-ECDSA"] = "RIPEMD160withPLAIN-ECDSA";
		AlgorithmOidMap[BsiObjectIdentifiers.ecdsa_plain_RIPEMD160] = "RIPEMD160withPLAIN-ECDSA";
		AlgorithmMap["SHA1WITHECNR"] = "SHA-1withECNR";
		AlgorithmMap["SHA-1WITHECNR"] = "SHA-1withECNR";
		AlgorithmMap["SHA224WITHECNR"] = "SHA-224withECNR";
		AlgorithmMap["SHA-224WITHECNR"] = "SHA-224withECNR";
		AlgorithmMap["SHA256WITHECNR"] = "SHA-256withECNR";
		AlgorithmMap["SHA-256WITHECNR"] = "SHA-256withECNR";
		AlgorithmMap["SHA384WITHECNR"] = "SHA-384withECNR";
		AlgorithmMap["SHA-384WITHECNR"] = "SHA-384withECNR";
		AlgorithmMap["SHA512WITHECNR"] = "SHA-512withECNR";
		AlgorithmMap["SHA-512WITHECNR"] = "SHA-512withECNR";
		AlgorithmMap["GOST-3410"] = "GOST3410";
		AlgorithmMap["GOST-3410-94"] = "GOST3410";
		AlgorithmMap["GOST3411WITHGOST3410"] = "GOST3410";
		AlgorithmMap["GOST3411/GOST3410"] = "GOST3410";
		AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94] = "GOST3410";
		AlgorithmMap["ECGOST-3410"] = "ECGOST3410";
		AlgorithmMap["GOST-3410-2001"] = "ECGOST3410";
		AlgorithmMap["GOST3411WITHECGOST3410"] = "ECGOST3410";
		AlgorithmMap["GOST3411/ECGOST3410"] = "ECGOST3410";
		AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001] = "ECGOST3410";
		AlgorithmMap["GOST-3410-2012-256"] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST3411WITHECGOST3410-2012-256"] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST3411-2012-256WITHECGOST3410"] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST3411-2012-256WITHECGOST3410-2012-256"] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST3411-2012-256/ECGOST3410"] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST3411-2012-256/ECGOST3410-2012-256"] = "ECGOST3410-2012-256";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256] = "ECGOST3410-2012-256";
		AlgorithmMap["GOST-3410-2012-512"] = "ECGOST3410-2012-512";
		AlgorithmMap["GOST3411WITHECGOST3410-2012-512"] = "ECGOST3410-2012-512";
		AlgorithmMap["GOST3411-2012-512WITHECGOST3410"] = "ECGOST3410-2012-512";
		AlgorithmMap["GOST3411-2012-512WITHECGOST3410-2012-512"] = "ECGOST3410-2012-512";
		AlgorithmMap["GOST3411-2012-512/ECGOST3410"] = "ECGOST3410-2012-512";
		AlgorithmMap["GOST3411-2012-512/ECGOST3410-2012-512"] = "ECGOST3410-2012-512";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512] = "ECGOST3410-2012-512";
		AlgorithmMap["ED25519"] = "Ed25519";
		AlgorithmOidMap[EdECObjectIdentifiers.id_Ed25519] = "Ed25519";
		AlgorithmMap["ED25519CTX"] = "Ed25519ctx";
		AlgorithmMap["ED25519PH"] = "Ed25519ph";
		AlgorithmMap["ED448"] = "Ed448";
		AlgorithmOidMap[EdECObjectIdentifiers.id_Ed448] = "Ed448";
		AlgorithmMap["ED448PH"] = "Ed448ph";
		AlgorithmMap["SHA256WITHSM2"] = "SHA256withSM2";
		AlgorithmOidMap[GMObjectIdentifiers.sm2sign_with_sha256] = "SHA256withSM2";
		AlgorithmMap["SM3WITHSM2"] = "SM3withSM2";
		AlgorithmOidMap[GMObjectIdentifiers.sm2sign_with_sm3] = "SM3withSM2";
		NoRandom.Add("Ed25519");
		NoRandom.Add("Ed25519ctx");
		NoRandom.Add("Ed25519ph");
		NoRandom.Add("Ed448");
		NoRandom.Add("Ed448ph");
		Oids["MD2withRSA"] = PkcsObjectIdentifiers.MD2WithRsaEncryption;
		Oids["MD4withRSA"] = PkcsObjectIdentifiers.MD4WithRsaEncryption;
		Oids["MD5withRSA"] = PkcsObjectIdentifiers.MD5WithRsaEncryption;
		Oids["SHA-1withRSA"] = PkcsObjectIdentifiers.Sha1WithRsaEncryption;
		Oids["SHA-224withRSA"] = PkcsObjectIdentifiers.Sha224WithRsaEncryption;
		Oids["SHA-256withRSA"] = PkcsObjectIdentifiers.Sha256WithRsaEncryption;
		Oids["SHA-384withRSA"] = PkcsObjectIdentifiers.Sha384WithRsaEncryption;
		Oids["SHA-512withRSA"] = PkcsObjectIdentifiers.Sha512WithRsaEncryption;
		Oids["SHA-512(224)withRSA"] = PkcsObjectIdentifiers.Sha512_224WithRSAEncryption;
		Oids["SHA-512(256)withRSA"] = PkcsObjectIdentifiers.Sha512_256WithRSAEncryption;
		Oids["SHA3-224withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224;
		Oids["SHA3-256withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256;
		Oids["SHA3-384withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384;
		Oids["SHA3-512withRSA"] = NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512;
		Oids["PSSwithRSA"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["SHA-1withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["SHA-224withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["SHA-256withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["SHA-384withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["SHA-512withRSAandMGF1"] = PkcsObjectIdentifiers.IdRsassaPss;
		Oids["RIPEMD128withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128;
		Oids["RIPEMD160withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160;
		Oids["RIPEMD256withRSA"] = TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256;
		Oids["SHA-1withDSA"] = X9ObjectIdentifiers.IdDsaWithSha1;
		Oids["SHA-1withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha1;
		Oids["SHA-224withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha224;
		Oids["SHA-256withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha256;
		Oids["SHA-384withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha384;
		Oids["SHA-512withECDSA"] = X9ObjectIdentifiers.ECDsaWithSha512;
		Oids["RIPEMD160withECDSA"] = TeleTrusTObjectIdentifiers.ECSignWithRipeMD160;
		Oids["SHA-1withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_1;
		Oids["SHA-224withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_224;
		Oids["SHA-256withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_256;
		Oids["SHA-384withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_384;
		Oids["SHA-512withCVC-ECDSA"] = EacObjectIdentifiers.id_TA_ECDSA_SHA_512;
		Oids["SHA-1withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA1;
		Oids["SHA-224withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA224;
		Oids["SHA-256withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA256;
		Oids["SHA-384withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA384;
		Oids["SHA-512withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_SHA512;
		Oids["RIPEMD160withPLAIN-ECDSA"] = BsiObjectIdentifiers.ecdsa_plain_RIPEMD160;
		Oids["GOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94;
		Oids["ECGOST3410"] = CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001;
		Oids["ECGOST3410-2012-256"] = RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256;
		Oids["ECGOST3410-2012-512"] = RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512;
		Oids["Ed25519"] = EdECObjectIdentifiers.id_Ed25519;
		Oids["Ed448"] = EdECObjectIdentifiers.id_Ed448;
		Oids["SHA256withSM2"] = GMObjectIdentifiers.sm2sign_with_sha256;
		Oids["SM3withSM2"] = GMObjectIdentifiers.sm2sign_with_sm3;
	}

	public static Asn1Encodable GetDefaultX509Parameters(DerObjectIdentifier id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		if (!AlgorithmOidMap.TryGetValue(id, out var value))
		{
			return DerNull.Instance;
		}
		return GetDefaultX509ParametersForMechanism(value);
	}

	public static Asn1Encodable GetDefaultX509Parameters(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		return GetDefaultX509ParametersForMechanism(GetMechanism(algorithm) ?? algorithm);
	}

	private static Asn1Encodable GetDefaultX509ParametersForMechanism(string mechanism)
	{
		if (mechanism == "PSSwithRSA")
		{
			return GetPssX509Parameters("SHA-1");
		}
		if (Platform.EndsWith(mechanism, "withRSAandMGF1"))
		{
			return GetPssX509Parameters(mechanism.Substring(0, mechanism.Length - "withRSAandMGF1".Length));
		}
		return DerNull.Instance;
	}

	public static string GetEncodingName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
	}

	private static string GetMechanism(string algorithm)
	{
		if (AlgorithmMap.TryGetValue(algorithm, out var value))
		{
			return value;
		}
		if (DerObjectIdentifier.TryFromID(algorithm, out var oid) && AlgorithmOidMap.TryGetValue(oid, out var value2))
		{
			return value2;
		}
		return null;
	}

	public static DerObjectIdentifier GetObjectIdentifier(string mechanism)
	{
		if (mechanism == null)
		{
			throw new ArgumentNullException("mechanism");
		}
		mechanism = GetMechanism(mechanism) ?? mechanism;
		return CollectionUtilities.GetValueOrNull(Oids, mechanism);
	}

	private static Asn1Encodable GetPssX509Parameters(string digestName)
	{
		AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(DigestUtilities.GetObjectIdentifier(digestName), DerNull.Instance);
		AlgorithmIdentifier maskGenAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, algorithmIdentifier);
		int digestSize = DigestUtilities.GetDigest(digestName).GetDigestSize();
		return new RsassaPssParameters(algorithmIdentifier, maskGenAlgorithm, new DerInteger(digestSize), new DerInteger(1));
	}

	public static ISigner GetSigner(DerObjectIdentifier id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		if (AlgorithmOidMap.TryGetValue(id, out var value))
		{
			ISigner signerForMechanism = GetSignerForMechanism(value);
			if (signerForMechanism != null)
			{
				return signerForMechanism;
			}
		}
		throw new SecurityUtilityException("Signer OID not recognised.");
	}

	public static ISigner GetSigner(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		ISigner signerForMechanism = GetSignerForMechanism(GetMechanism(algorithm) ?? algorithm.ToUpperInvariant());
		if (signerForMechanism != null)
		{
			return signerForMechanism;
		}
		throw new SecurityUtilityException("Signer " + algorithm + " not recognised.");
	}

	private static ISigner GetSignerForMechanism(string mechanism)
	{
		if (Platform.StartsWith(mechanism, "Ed"))
		{
			if (mechanism.Equals("Ed25519"))
			{
				return new Ed25519Signer();
			}
			if (mechanism.Equals("Ed25519ctx"))
			{
				return new Ed25519ctxSigner(Arrays.EmptyBytes);
			}
			if (mechanism.Equals("Ed25519ph"))
			{
				return new Ed25519phSigner(Arrays.EmptyBytes);
			}
			if (mechanism.Equals("Ed448"))
			{
				return new Ed448Signer(Arrays.EmptyBytes);
			}
			if (mechanism.Equals("Ed448ph"))
			{
				return new Ed448phSigner(Arrays.EmptyBytes);
			}
		}
		if (mechanism.Equals("RSA"))
		{
			return new RsaDigestSigner((IDigest)new NullDigest(), (AlgorithmIdentifier)null);
		}
		if (mechanism.Equals("RAWRSASSA-PSS"))
		{
			return PssSigner.CreateRawSigner(new RsaBlindedEngine(), new Sha1Digest());
		}
		if (mechanism.Equals("PSSwithRSA"))
		{
			return new PssSigner(new RsaBlindedEngine(), new Sha1Digest());
		}
		if (Platform.EndsWith(mechanism, "withRSA"))
		{
			return new RsaDigestSigner(DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with"))));
		}
		if (Platform.EndsWith(mechanism, "withRSAandMGF1"))
		{
			IDigest digest = DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with")));
			return new PssSigner(new RsaBlindedEngine(), digest);
		}
		if (Platform.EndsWith(mechanism, "withDSA"))
		{
			IDigest digest2 = DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with")));
			return new DsaDigestSigner(new DsaSigner(), digest2);
		}
		if (Platform.EndsWith(mechanism, "withECDSA"))
		{
			IDigest digest3 = DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with")));
			return new DsaDigestSigner(new ECDsaSigner(), digest3);
		}
		if (Platform.EndsWith(mechanism, "withCVC-ECDSA") || Platform.EndsWith(mechanism, "withPLAIN-ECDSA"))
		{
			IDigest digest4 = DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with")));
			return new DsaDigestSigner(new ECDsaSigner(), digest4, PlainDsaEncoding.Instance);
		}
		if (Platform.EndsWith(mechanism, "withECNR"))
		{
			IDigest digest5 = DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with")));
			return new DsaDigestSigner(new ECNRSigner(), digest5);
		}
		if (Platform.EndsWith(mechanism, "withSM2"))
		{
			return new SM2Signer(DigestUtilities.GetDigest(mechanism.Substring(0, mechanism.LastIndexOf("with"))));
		}
		if (mechanism.Equals("GOST3410"))
		{
			return new Gost3410DigestSigner(new Gost3410Signer(), new Gost3411Digest());
		}
		if (Platform.StartsWith(mechanism, "ECGOST3410"))
		{
			if (mechanism.Equals("ECGOST3410"))
			{
				return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411Digest());
			}
			if (mechanism.Equals("ECGOST3410-2012-256"))
			{
				return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411_2012_256Digest());
			}
			if (mechanism.Equals("ECGOST3410-2012-512"))
			{
				return new Gost3410DigestSigner(new ECGost3410Signer(), new Gost3411_2012_512Digest());
			}
		}
		if (Platform.EndsWith(mechanism, "/ISO9796-2"))
		{
			if (mechanism.Equals("SHA1WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new Sha1Digest(), isImplicit: true);
			}
			if (mechanism.Equals("MD5WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new MD5Digest(), isImplicit: true);
			}
			if (mechanism.Equals("RIPEMD160WITHRSA/ISO9796-2"))
			{
				return new Iso9796d2Signer(new RsaBlindedEngine(), new RipeMD160Digest(), isImplicit: true);
			}
		}
		if (Platform.EndsWith(mechanism, "/X9.31"))
		{
			string text = mechanism.Substring(0, mechanism.Length - "/X9.31".Length);
			int num = Platform.IndexOf(text, "WITH");
			if (num > 0)
			{
				int num2 = num + "WITH".Length;
				if (text.Substring(num2, text.Length - num2).Equals("RSA"))
				{
					RsaBlindedEngine cipher = new RsaBlindedEngine();
					IDigest digest6 = DigestUtilities.GetDigest(text.Substring(0, num));
					return new X931Signer(cipher, digest6);
				}
			}
		}
		return null;
	}

	public static ISigner InitSigner(DerObjectIdentifier algorithmOid, bool forSigning, AsymmetricKeyParameter privateKey, SecureRandom random)
	{
		if (algorithmOid == null)
		{
			throw new ArgumentNullException("algorithmOid");
		}
		if (AlgorithmOidMap.TryGetValue(algorithmOid, out var value))
		{
			ISigner signerForMechanism = GetSignerForMechanism(value);
			if (signerForMechanism != null)
			{
				ICipherParameters cipherParameters = privateKey;
				if (forSigning && !NoRandom.Contains(value))
				{
					cipherParameters = ParameterUtilities.WithRandom(cipherParameters, random);
				}
				signerForMechanism.Init(forSigning, cipherParameters);
				return signerForMechanism;
			}
		}
		throw new SecurityUtilityException("Signer OID not recognised.");
	}

	public static ISigner InitSigner(string algorithm, bool forSigning, AsymmetricKeyParameter privateKey, SecureRandom random)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		string text = GetMechanism(algorithm) ?? algorithm.ToUpperInvariant();
		ISigner signerForMechanism = GetSignerForMechanism(text);
		if (signerForMechanism != null)
		{
			ICipherParameters cipherParameters = privateKey;
			if (forSigning && !NoRandom.Contains(text))
			{
				cipherParameters = ParameterUtilities.WithRandom(cipherParameters, random);
			}
			signerForMechanism.Init(forSigning, cipherParameters);
			return signerForMechanism;
		}
		throw new SecurityUtilityException("Signer " + algorithm + " not recognised.");
	}
}
