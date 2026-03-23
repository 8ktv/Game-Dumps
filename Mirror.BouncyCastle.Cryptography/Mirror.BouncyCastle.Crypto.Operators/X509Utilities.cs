using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.GM;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Crypto.Operators;

internal class X509Utilities
{
	private static readonly IDictionary<string, DerObjectIdentifier> m_algorithms;

	private static readonly IDictionary<string, Asn1Encodable> m_exParams;

	private static readonly HashSet<DerObjectIdentifier> noParams;

	static X509Utilities()
	{
		m_algorithms = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		m_exParams = new Dictionary<string, Asn1Encodable>(StringComparer.OrdinalIgnoreCase);
		noParams = new HashSet<DerObjectIdentifier>();
		m_algorithms.Add("MD2WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD2WITHRSA", PkcsObjectIdentifiers.MD2WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSAENCRYPTION", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("MD5WITHRSA", PkcsObjectIdentifiers.MD5WithRsaEncryption);
		m_algorithms.Add("SHA1WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA-1WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA1WITHRSA", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA-1WITHRSA", PkcsObjectIdentifiers.Sha1WithRsaEncryption);
		m_algorithms.Add("SHA224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA-224WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA-224WITHRSA", PkcsObjectIdentifiers.Sha224WithRsaEncryption);
		m_algorithms.Add("SHA256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA-256WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA-256WITHRSA", PkcsObjectIdentifiers.Sha256WithRsaEncryption);
		m_algorithms.Add("SHA384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA-384WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA-384WITHRSA", PkcsObjectIdentifiers.Sha384WithRsaEncryption);
		m_algorithms.Add("SHA512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA-512WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA-512WITHRSA", PkcsObjectIdentifiers.Sha512WithRsaEncryption);
		m_algorithms.Add("SHA512(224)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA-512(224)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA512(224)WITHRSA", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA-512(224)WITHRSA", PkcsObjectIdentifiers.Sha512_224WithRSAEncryption);
		m_algorithms.Add("SHA512(256)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA-512(256)WITHRSAENCRYPTION", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA512(256)WITHRSA", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA-512(256)WITHRSA", PkcsObjectIdentifiers.Sha512_256WithRSAEncryption);
		m_algorithms.Add("SHA3-224WITHRSAENCRYPTION", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224);
		m_algorithms.Add("SHA3-256WITHRSAENCRYPTION", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256);
		m_algorithms.Add("SHA3-384WITHRSAENCRYPTION", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384);
		m_algorithms.Add("SHA3-512WITHRSAENCRYPTION", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512);
		m_algorithms.Add("SHA3-224WITHRSA", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_224);
		m_algorithms.Add("SHA3-256WITHRSA", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_256);
		m_algorithms.Add("SHA3-384WITHRSA", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_384);
		m_algorithms.Add("SHA3-512WITHRSA", NistObjectIdentifiers.IdRsassaPkcs1V15WithSha3_512);
		m_algorithms.Add("SHA1WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA224WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA256WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA384WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("SHA512WITHRSAANDMGF1", PkcsObjectIdentifiers.IdRsassaPss);
		m_algorithms.Add("RIPEMD160WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD160WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD160);
		m_algorithms.Add("RIPEMD128WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD128WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD128);
		m_algorithms.Add("RIPEMD256WITHRSAENCRYPTION", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("RIPEMD256WITHRSA", TeleTrusTObjectIdentifiers.RsaSignatureWithRipeMD256);
		m_algorithms.Add("SHA1WITHDSA", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("DSAWITHSHA1", X9ObjectIdentifiers.IdDsaWithSha1);
		m_algorithms.Add("SHA224WITHDSA", NistObjectIdentifiers.DsaWithSha224);
		m_algorithms.Add("SHA256WITHDSA", NistObjectIdentifiers.DsaWithSha256);
		m_algorithms.Add("SHA384WITHDSA", NistObjectIdentifiers.DsaWithSha384);
		m_algorithms.Add("SHA512WITHDSA", NistObjectIdentifiers.DsaWithSha512);
		m_algorithms.Add("SHA1WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("ECDSAWITHSHA1", X9ObjectIdentifiers.ECDsaWithSha1);
		m_algorithms.Add("SHA224WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
		m_algorithms.Add("SHA256WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
		m_algorithms.Add("SHA384WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
		m_algorithms.Add("SHA512WITHECDSA", X9ObjectIdentifiers.ECDsaWithSha512);
		m_algorithms.Add("GOST3411WITHGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3411WITHGOST3410-94", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		m_algorithms.Add("GOST3411WITHECGOST3410", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHECGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411WITHGOST3410-2001", CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		m_algorithms.Add("GOST3411-2012-256WITHECGOST3410", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256);
		m_algorithms.Add("GOST3411-2012-256WITHECGOST3410-2012-256", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_256);
		m_algorithms.Add("GOST3411-2012-512WITHECGOST3410", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512);
		m_algorithms.Add("GOST3411-2012-512WITHECGOST3410-2012-512", RosstandartObjectIdentifiers.id_tc26_signwithdigest_gost_3410_12_512);
		m_algorithms.Add("Ed25519", EdECObjectIdentifiers.id_Ed25519);
		m_algorithms.Add("Ed448", EdECObjectIdentifiers.id_Ed448);
		m_algorithms.Add("SHA256WITHSM2", GMObjectIdentifiers.sm2sign_with_sha256);
		m_algorithms.Add("SM3WITHSM2", GMObjectIdentifiers.sm2sign_with_sm3);
		noParams.Add(X9ObjectIdentifiers.ECDsaWithSha1);
		noParams.Add(X9ObjectIdentifiers.ECDsaWithSha224);
		noParams.Add(X9ObjectIdentifiers.ECDsaWithSha256);
		noParams.Add(X9ObjectIdentifiers.ECDsaWithSha384);
		noParams.Add(X9ObjectIdentifiers.ECDsaWithSha512);
		noParams.Add(X9ObjectIdentifiers.IdDsaWithSha1);
		noParams.Add(OiwObjectIdentifiers.DsaWithSha1);
		noParams.Add(NistObjectIdentifiers.DsaWithSha224);
		noParams.Add(NistObjectIdentifiers.DsaWithSha256);
		noParams.Add(NistObjectIdentifiers.DsaWithSha384);
		noParams.Add(NistObjectIdentifiers.DsaWithSha512);
		noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x94);
		noParams.Add(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001);
		noParams.Add(EdECObjectIdentifiers.id_Ed25519);
		noParams.Add(EdECObjectIdentifiers.id_Ed448);
		AlgorithmIdentifier hashAlgId = new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance);
		m_exParams.Add("SHA1WITHRSAANDMGF1", CreatePssParams(hashAlgId, 20));
		AlgorithmIdentifier hashAlgId2 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha224, DerNull.Instance);
		m_exParams.Add("SHA224WITHRSAANDMGF1", CreatePssParams(hashAlgId2, 28));
		AlgorithmIdentifier hashAlgId3 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha256, DerNull.Instance);
		m_exParams.Add("SHA256WITHRSAANDMGF1", CreatePssParams(hashAlgId3, 32));
		AlgorithmIdentifier hashAlgId4 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha384, DerNull.Instance);
		m_exParams.Add("SHA384WITHRSAANDMGF1", CreatePssParams(hashAlgId4, 48));
		AlgorithmIdentifier hashAlgId5 = new AlgorithmIdentifier(NistObjectIdentifiers.IdSha512, DerNull.Instance);
		m_exParams.Add("SHA512WITHRSAANDMGF1", CreatePssParams(hashAlgId5, 64));
	}

	private static string GetDigestAlgName(DerObjectIdentifier digestAlgOID)
	{
		if (PkcsObjectIdentifiers.MD5.Equals(digestAlgOID))
		{
			return "MD5";
		}
		if (OiwObjectIdentifiers.IdSha1.Equals(digestAlgOID))
		{
			return "SHA1";
		}
		if (NistObjectIdentifiers.IdSha224.Equals(digestAlgOID))
		{
			return "SHA224";
		}
		if (NistObjectIdentifiers.IdSha256.Equals(digestAlgOID))
		{
			return "SHA256";
		}
		if (NistObjectIdentifiers.IdSha384.Equals(digestAlgOID))
		{
			return "SHA384";
		}
		if (NistObjectIdentifiers.IdSha512.Equals(digestAlgOID))
		{
			return "SHA512";
		}
		if (NistObjectIdentifiers.IdSha512_224.Equals(digestAlgOID))
		{
			return "SHA512(224)";
		}
		if (NistObjectIdentifiers.IdSha512_256.Equals(digestAlgOID))
		{
			return "SHA512(256)";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD128.Equals(digestAlgOID))
		{
			return "RIPEMD128";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD160.Equals(digestAlgOID))
		{
			return "RIPEMD160";
		}
		if (TeleTrusTObjectIdentifiers.RipeMD256.Equals(digestAlgOID))
		{
			return "RIPEMD256";
		}
		if (CryptoProObjectIdentifiers.GostR3411.Equals(digestAlgOID))
		{
			return "GOST3411";
		}
		if (RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256.Equals(digestAlgOID))
		{
			return "GOST3411-2012-256";
		}
		if (RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512.Equals(digestAlgOID))
		{
			return "GOST3411-2012-512";
		}
		return digestAlgOID.Id;
	}

	internal static string GetSignatureName(AlgorithmIdentifier sigAlgId)
	{
		Asn1Encodable parameters = sigAlgId.Parameters;
		if (parameters != null && !DerNull.Instance.Equals(parameters))
		{
			if (sigAlgId.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
			{
				return GetDigestAlgName(RsassaPssParameters.GetInstance(parameters).HashAlgorithm.Algorithm) + "withRSAandMGF1";
			}
			if (sigAlgId.Algorithm.Equals(X9ObjectIdentifiers.ECDsaWithSha2))
			{
				return GetDigestAlgName((DerObjectIdentifier)Asn1Sequence.GetInstance(parameters)[0]) + "withECDSA";
			}
		}
		return sigAlgId.Algorithm.Id;
	}

	private static RsassaPssParameters CreatePssParams(AlgorithmIdentifier hashAlgId, int saltSize)
	{
		return new RsassaPssParameters(hashAlgId, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, hashAlgId), new DerInteger(saltSize), new DerInteger(1));
	}

	internal static DerObjectIdentifier GetAlgorithmOid(string algorithmName)
	{
		if (m_algorithms.TryGetValue(algorithmName, out var value))
		{
			return value;
		}
		return new DerObjectIdentifier(algorithmName);
	}

	internal static AlgorithmIdentifier GetSigAlgID(DerObjectIdentifier sigOid, string algorithmName)
	{
		if (noParams.Contains(sigOid))
		{
			return new AlgorithmIdentifier(sigOid);
		}
		if (m_exParams.TryGetValue(algorithmName, out var value))
		{
			return new AlgorithmIdentifier(sigOid, value);
		}
		return new AlgorithmIdentifier(sigOid, DerNull.Instance);
	}

	internal static IEnumerable<string> GetAlgNames()
	{
		return CollectionUtilities.Proxy(m_algorithms.Keys);
	}
}
