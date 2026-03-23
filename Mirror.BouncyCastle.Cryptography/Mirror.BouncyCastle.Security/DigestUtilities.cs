using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.GM;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.UA;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class DigestUtilities
{
	private enum DigestAlgorithm
	{
		BLAKE2B_160,
		BLAKE2B_256,
		BLAKE2B_384,
		BLAKE2B_512,
		BLAKE2S_128,
		BLAKE2S_160,
		BLAKE2S_224,
		BLAKE2S_256,
		BLAKE3_256,
		DSTU7564_256,
		DSTU7564_384,
		DSTU7564_512,
		GOST3411,
		GOST3411_2012_256,
		GOST3411_2012_512,
		KECCAK_224,
		KECCAK_256,
		KECCAK_288,
		KECCAK_384,
		KECCAK_512,
		MD2,
		MD4,
		MD5,
		NONE,
		RIPEMD128,
		RIPEMD160,
		RIPEMD256,
		RIPEMD320,
		SHA_1,
		SHA_224,
		SHA_256,
		SHA_384,
		SHA_512,
		SHA_512_224,
		SHA_512_256,
		SHA3_224,
		SHA3_256,
		SHA3_384,
		SHA3_512,
		SHAKE128_256,
		SHAKE256_512,
		SM3,
		TIGER,
		WHIRLPOOL
	}

	private static readonly Dictionary<string, string> AlgorithmMap;

	private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap;

	private static readonly Dictionary<string, DerObjectIdentifier> Oids;

	static DigestUtilities()
	{
		AlgorithmMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		AlgorithmOidMap = new Dictionary<DerObjectIdentifier, string>();
		Oids = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		Enums.GetArbitraryValue<DigestAlgorithm>().ToString();
		AlgorithmOidMap[PkcsObjectIdentifiers.MD2] = "MD2";
		AlgorithmOidMap[PkcsObjectIdentifiers.MD4] = "MD4";
		AlgorithmOidMap[PkcsObjectIdentifiers.MD5] = "MD5";
		AlgorithmMap["SHA1"] = "SHA-1";
		AlgorithmOidMap[OiwObjectIdentifiers.IdSha1] = "SHA-1";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha1] = "SHA-1";
		AlgorithmOidMap[MiscObjectIdentifiers.HMAC_SHA1] = "SHA-1";
		AlgorithmMap["SHA224"] = "SHA-224";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha224] = "SHA-224";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha224] = "SHA-224";
		AlgorithmMap["SHA256"] = "SHA-256";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha256] = "SHA-256";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha256] = "SHA-256";
		AlgorithmMap["SHA384"] = "SHA-384";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha384] = "SHA-384";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha384] = "SHA-384";
		AlgorithmMap["SHA512"] = "SHA-512";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha512] = "SHA-512";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha512] = "SHA-512";
		AlgorithmMap["SHA512/224"] = "SHA-512/224";
		AlgorithmMap["SHA512(224)"] = "SHA-512/224";
		AlgorithmMap["SHA-512(224)"] = "SHA-512/224";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha512_224] = "SHA-512/224";
		AlgorithmMap["SHA512/256"] = "SHA-512/256";
		AlgorithmMap["SHA512(256)"] = "SHA-512/256";
		AlgorithmMap["SHA-512(256)"] = "SHA-512/256";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha512_256] = "SHA-512/256";
		AlgorithmMap["RIPEMD-128"] = "RIPEMD128";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD128] = "RIPEMD128";
		AlgorithmMap["RIPEMD-160"] = "RIPEMD160";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD160] = "RIPEMD160";
		AlgorithmMap["RIPEMD-256"] = "RIPEMD256";
		AlgorithmOidMap[TeleTrusTObjectIdentifiers.RipeMD256] = "RIPEMD256";
		AlgorithmMap["RIPEMD-320"] = "RIPEMD320";
		AlgorithmOidMap[CryptoProObjectIdentifiers.GostR3411] = "GOST3411";
		AlgorithmMap["KECCAK224"] = "KECCAK-224";
		AlgorithmMap["KECCAK256"] = "KECCAK-256";
		AlgorithmMap["KECCAK288"] = "KECCAK-288";
		AlgorithmMap["KECCAK384"] = "KECCAK-384";
		AlgorithmMap["KECCAK512"] = "KECCAK-512";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha3_224] = "SHA3-224";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_224] = "SHA3-224";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha3_256] = "SHA3-256";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_256] = "SHA3-256";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha3_384] = "SHA3-384";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_384] = "SHA3-384";
		AlgorithmOidMap[NistObjectIdentifiers.IdSha3_512] = "SHA3-512";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_512] = "SHA3-512";
		AlgorithmMap["SHAKE128"] = "SHAKE128-256";
		AlgorithmOidMap[NistObjectIdentifiers.IdShake128] = "SHAKE128-256";
		AlgorithmMap["SHAKE256"] = "SHAKE256-512";
		AlgorithmOidMap[NistObjectIdentifiers.IdShake256] = "SHAKE256-512";
		AlgorithmOidMap[GMObjectIdentifiers.sm3] = "SM3";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b160] = "BLAKE2B-160";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b256] = "BLAKE2B-256";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b384] = "BLAKE2B-384";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2b512] = "BLAKE2B-512";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s128] = "BLAKE2S-128";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s160] = "BLAKE2S-160";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s224] = "BLAKE2S-224";
		AlgorithmOidMap[MiscObjectIdentifiers.id_blake2s256] = "BLAKE2S-256";
		AlgorithmOidMap[MiscObjectIdentifiers.blake3_256] = "BLAKE3-256";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256] = "GOST3411-2012-256";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512] = "GOST3411-2012-512";
		AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_256] = "DSTU7564-256";
		AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_384] = "DSTU7564-384";
		AlgorithmOidMap[UAObjectIdentifiers.dstu7564digest_512] = "DSTU7564-512";
		Oids["MD2"] = PkcsObjectIdentifiers.MD2;
		Oids["MD4"] = PkcsObjectIdentifiers.MD4;
		Oids["MD5"] = PkcsObjectIdentifiers.MD5;
		Oids["SHA-1"] = OiwObjectIdentifiers.IdSha1;
		Oids["SHA-224"] = NistObjectIdentifiers.IdSha224;
		Oids["SHA-256"] = NistObjectIdentifiers.IdSha256;
		Oids["SHA-384"] = NistObjectIdentifiers.IdSha384;
		Oids["SHA-512"] = NistObjectIdentifiers.IdSha512;
		Oids["SHA-512/224"] = NistObjectIdentifiers.IdSha512_224;
		Oids["SHA-512/256"] = NistObjectIdentifiers.IdSha512_256;
		Oids["SHA3-224"] = NistObjectIdentifiers.IdSha3_224;
		Oids["SHA3-256"] = NistObjectIdentifiers.IdSha3_256;
		Oids["SHA3-384"] = NistObjectIdentifiers.IdSha3_384;
		Oids["SHA3-512"] = NistObjectIdentifiers.IdSha3_512;
		Oids["SHAKE128-256"] = NistObjectIdentifiers.IdShake128;
		Oids["SHAKE256-512"] = NistObjectIdentifiers.IdShake256;
		Oids["RIPEMD128"] = TeleTrusTObjectIdentifiers.RipeMD128;
		Oids["RIPEMD160"] = TeleTrusTObjectIdentifiers.RipeMD160;
		Oids["RIPEMD256"] = TeleTrusTObjectIdentifiers.RipeMD256;
		Oids["GOST3411"] = CryptoProObjectIdentifiers.GostR3411;
		Oids["SM3"] = GMObjectIdentifiers.sm3;
		Oids["BLAKE2B-160"] = MiscObjectIdentifiers.id_blake2b160;
		Oids["BLAKE2B-256"] = MiscObjectIdentifiers.id_blake2b256;
		Oids["BLAKE2B-384"] = MiscObjectIdentifiers.id_blake2b384;
		Oids["BLAKE2B-512"] = MiscObjectIdentifiers.id_blake2b512;
		Oids["BLAKE2S-128"] = MiscObjectIdentifiers.id_blake2s128;
		Oids["BLAKE2S-160"] = MiscObjectIdentifiers.id_blake2s160;
		Oids["BLAKE2S-224"] = MiscObjectIdentifiers.id_blake2s224;
		Oids["BLAKE2S-256"] = MiscObjectIdentifiers.id_blake2s256;
		Oids["BLAKE3-256"] = MiscObjectIdentifiers.blake3_256;
		Oids["GOST3411-2012-256"] = RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256;
		Oids["GOST3411-2012-512"] = RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512;
		Oids["DSTU7564-256"] = UAObjectIdentifiers.dstu7564digest_256;
		Oids["DSTU7564-384"] = UAObjectIdentifiers.dstu7564digest_384;
		Oids["DSTU7564-512"] = UAObjectIdentifiers.dstu7564digest_512;
	}

	public static byte[] CalculateDigest(DerObjectIdentifier id, byte[] input)
	{
		return CalculateDigest(id.Id, input);
	}

	public static byte[] CalculateDigest(string algorithm, byte[] input)
	{
		return DoFinal(GetDigest(algorithm), input);
	}

	public static byte[] CalculateDigest(string algorithm, byte[] buf, int off, int len)
	{
		return DoFinal(GetDigest(algorithm), buf, off, len);
	}

	public static byte[] DoFinal(IDigest digest)
	{
		byte[] array = new byte[digest.GetDigestSize()];
		digest.DoFinal(array, 0);
		return array;
	}

	public static byte[] DoFinal(IDigest digest, byte[] input)
	{
		digest.BlockUpdate(input, 0, input.Length);
		return DoFinal(digest);
	}

	public static byte[] DoFinal(IDigest digest, byte[] buf, int off, int len)
	{
		digest.BlockUpdate(buf, off, len);
		return DoFinal(digest);
	}

	public static string GetAlgorithmName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
	}

	public static IDigest GetDigest(DerObjectIdentifier id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		if (AlgorithmOidMap.TryGetValue(id, out var value))
		{
			IDigest digestForMechanism = GetDigestForMechanism(value);
			if (digestForMechanism != null)
			{
				return digestForMechanism;
			}
		}
		throw new SecurityUtilityException("Digest OID not recognised.");
	}

	public static IDigest GetDigest(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		IDigest digestForMechanism = GetDigestForMechanism(GetMechanism(algorithm) ?? algorithm.ToUpperInvariant());
		if (digestForMechanism != null)
		{
			return digestForMechanism;
		}
		throw new SecurityUtilityException("Digest " + algorithm + " not recognised.");
	}

	private static IDigest GetDigestForMechanism(string mechanism)
	{
		if (!Enums.TryGetEnumValue<DigestAlgorithm>(mechanism, out var result))
		{
			return null;
		}
		return result switch
		{
			DigestAlgorithm.BLAKE2B_160 => new Blake2bDigest(160), 
			DigestAlgorithm.BLAKE2B_256 => new Blake2bDigest(256), 
			DigestAlgorithm.BLAKE2B_384 => new Blake2bDigest(384), 
			DigestAlgorithm.BLAKE2B_512 => new Blake2bDigest(512), 
			DigestAlgorithm.BLAKE2S_128 => new Blake2sDigest(128), 
			DigestAlgorithm.BLAKE2S_160 => new Blake2sDigest(160), 
			DigestAlgorithm.BLAKE2S_224 => new Blake2sDigest(224), 
			DigestAlgorithm.BLAKE2S_256 => new Blake2sDigest(256), 
			DigestAlgorithm.BLAKE3_256 => new Blake3Digest(256), 
			DigestAlgorithm.DSTU7564_256 => new Dstu7564Digest(256), 
			DigestAlgorithm.DSTU7564_384 => new Dstu7564Digest(384), 
			DigestAlgorithm.DSTU7564_512 => new Dstu7564Digest(512), 
			DigestAlgorithm.GOST3411 => new Gost3411Digest(), 
			DigestAlgorithm.GOST3411_2012_256 => new Gost3411_2012_256Digest(), 
			DigestAlgorithm.GOST3411_2012_512 => new Gost3411_2012_512Digest(), 
			DigestAlgorithm.KECCAK_224 => new KeccakDigest(224), 
			DigestAlgorithm.KECCAK_256 => new KeccakDigest(256), 
			DigestAlgorithm.KECCAK_288 => new KeccakDigest(288), 
			DigestAlgorithm.KECCAK_384 => new KeccakDigest(384), 
			DigestAlgorithm.KECCAK_512 => new KeccakDigest(512), 
			DigestAlgorithm.MD2 => new MD2Digest(), 
			DigestAlgorithm.MD4 => new MD4Digest(), 
			DigestAlgorithm.MD5 => new MD5Digest(), 
			DigestAlgorithm.NONE => new NullDigest(), 
			DigestAlgorithm.RIPEMD128 => new RipeMD128Digest(), 
			DigestAlgorithm.RIPEMD160 => new RipeMD160Digest(), 
			DigestAlgorithm.RIPEMD256 => new RipeMD256Digest(), 
			DigestAlgorithm.RIPEMD320 => new RipeMD320Digest(), 
			DigestAlgorithm.SHA_1 => new Sha1Digest(), 
			DigestAlgorithm.SHA_224 => new Sha224Digest(), 
			DigestAlgorithm.SHA_256 => new Sha256Digest(), 
			DigestAlgorithm.SHA_384 => new Sha384Digest(), 
			DigestAlgorithm.SHA_512 => new Sha512Digest(), 
			DigestAlgorithm.SHA_512_224 => new Sha512tDigest(224), 
			DigestAlgorithm.SHA_512_256 => new Sha512tDigest(256), 
			DigestAlgorithm.SHA3_224 => new Sha3Digest(224), 
			DigestAlgorithm.SHA3_256 => new Sha3Digest(256), 
			DigestAlgorithm.SHA3_384 => new Sha3Digest(384), 
			DigestAlgorithm.SHA3_512 => new Sha3Digest(512), 
			DigestAlgorithm.SHAKE128_256 => new ShakeDigest(128), 
			DigestAlgorithm.SHAKE256_512 => new ShakeDigest(256), 
			DigestAlgorithm.SM3 => new SM3Digest(), 
			DigestAlgorithm.TIGER => new TigerDigest(), 
			DigestAlgorithm.WHIRLPOOL => new WhirlpoolDigest(), 
			_ => throw new NotImplementedException(), 
		};
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
}
