using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.Agreement.Kdf;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class AgreementUtilities
{
	private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap;

	static AgreementUtilities()
	{
		AlgorithmOidMap = new Dictionary<DerObjectIdentifier, string>();
		AlgorithmOidMap[X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme] = "ECDHWITHSHA1KDF";
		AlgorithmOidMap[X9ObjectIdentifiers.DHSinglePassCofactorDHSha1KdfScheme] = "ECCDHWITHSHA1KDF";
		AlgorithmOidMap[X9ObjectIdentifiers.MqvSinglePassSha1KdfScheme] = "ECMQVWITHSHA1KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_stdDH_sha224kdf_scheme] = "ECDHWITHSHA224KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_cofactorDH_sha224kdf_scheme] = "ECCDHWITHSHA224KDF";
		AlgorithmOidMap[SecObjectIdentifiers.mqvSinglePass_sha224kdf_scheme] = "ECMQVWITHSHA224KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_stdDH_sha256kdf_scheme] = "ECDHWITHSHA256KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_cofactorDH_sha256kdf_scheme] = "ECCDHWITHSHA256KDF";
		AlgorithmOidMap[SecObjectIdentifiers.mqvSinglePass_sha256kdf_scheme] = "ECMQVWITHSHA256KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_stdDH_sha384kdf_scheme] = "ECDHWITHSHA384KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_cofactorDH_sha384kdf_scheme] = "ECCDHWITHSHA384KDF";
		AlgorithmOidMap[SecObjectIdentifiers.mqvSinglePass_sha384kdf_scheme] = "ECMQVWITHSHA384KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_stdDH_sha512kdf_scheme] = "ECDHWITHSHA512KDF";
		AlgorithmOidMap[SecObjectIdentifiers.dhSinglePass_cofactorDH_sha512kdf_scheme] = "ECCDHWITHSHA512KDF";
		AlgorithmOidMap[SecObjectIdentifiers.mqvSinglePass_sha512kdf_scheme] = "ECMQVWITHSHA512KDF";
		AlgorithmOidMap[EdECObjectIdentifiers.id_X25519] = "X25519";
		AlgorithmOidMap[EdECObjectIdentifiers.id_X448] = "X448";
	}

	public static string GetAlgorithmName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
	}

	public static IBasicAgreement GetBasicAgreement(DerObjectIdentifier oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (AlgorithmOidMap.TryGetValue(oid, out var value))
		{
			IBasicAgreement basicAgreementForMechanism = GetBasicAgreementForMechanism(value);
			if (basicAgreementForMechanism != null)
			{
				return basicAgreementForMechanism;
			}
		}
		throw new SecurityUtilityException("Basic Agreement OID not recognised.");
	}

	public static IBasicAgreement GetBasicAgreement(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		IBasicAgreement basicAgreementForMechanism = GetBasicAgreementForMechanism(GetMechanism(algorithm) ?? algorithm.ToUpperInvariant());
		if (basicAgreementForMechanism != null)
		{
			return basicAgreementForMechanism;
		}
		throw new SecurityUtilityException("Basic Agreement " + algorithm + " not recognised.");
	}

	private static IBasicAgreement GetBasicAgreementForMechanism(string mechanism)
	{
		switch (mechanism)
		{
		case "DH":
		case "DIFFIEHELLMAN":
			return new DHBasicAgreement();
		case "ECDH":
			return new ECDHBasicAgreement();
		case "ECDHC":
		case "ECCDH":
			return new ECDHCBasicAgreement();
		case "ECMQV":
			return new ECMqvBasicAgreement();
		default:
			return null;
		}
	}

	public static IBasicAgreement GetBasicAgreementWithKdf(DerObjectIdentifier agreeAlgOid, DerObjectIdentifier wrapAlgOid)
	{
		return GetBasicAgreementWithKdf(agreeAlgOid, wrapAlgOid?.Id);
	}

	public static IBasicAgreement GetBasicAgreementWithKdf(DerObjectIdentifier oid, string wrapAlgorithm)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (wrapAlgorithm == null)
		{
			throw new ArgumentNullException("wrapAlgorithm");
		}
		if (AlgorithmOidMap.TryGetValue(oid, out var value))
		{
			IBasicAgreement basicAgreementWithKdfForMechanism = GetBasicAgreementWithKdfForMechanism(value, wrapAlgorithm);
			if (basicAgreementWithKdfForMechanism != null)
			{
				return basicAgreementWithKdfForMechanism;
			}
		}
		throw new SecurityUtilityException("Basic Agreement (with KDF) OID not recognised.");
	}

	public static IBasicAgreement GetBasicAgreementWithKdf(string agreeAlgorithm, string wrapAlgorithm)
	{
		if (agreeAlgorithm == null)
		{
			throw new ArgumentNullException("agreeAlgorithm");
		}
		if (wrapAlgorithm == null)
		{
			throw new ArgumentNullException("wrapAlgorithm");
		}
		IBasicAgreement basicAgreementWithKdfForMechanism = GetBasicAgreementWithKdfForMechanism(GetMechanism(agreeAlgorithm) ?? agreeAlgorithm.ToUpperInvariant(), wrapAlgorithm);
		if (basicAgreementWithKdfForMechanism != null)
		{
			return basicAgreementWithKdfForMechanism;
		}
		throw new SecurityUtilityException("Basic Agreement (with KDF) " + agreeAlgorithm + " not recognised.");
	}

	private static IBasicAgreement GetBasicAgreementWithKdfForMechanism(string mechanism, string wrapAlgorithm)
	{
		switch (mechanism)
		{
		case "DHWITHSHA1KDF":
		case "ECDHWITHSHA1KDF":
			return new ECDHWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha1Digest()));
		case "ECDHWITHSHA224KDF":
			return new ECDHWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha224Digest()));
		case "ECDHWITHSHA256KDF":
			return new ECDHWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha256Digest()));
		case "ECDHWITHSHA384KDF":
			return new ECDHWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha384Digest()));
		case "ECDHWITHSHA512KDF":
			return new ECDHWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha512Digest()));
		case "ECCDHWITHSHA1KDF":
			return new ECDHCWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha1Digest()));
		case "ECCDHWITHSHA224KDF":
			return new ECDHCWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha224Digest()));
		case "ECCDHWITHSHA256KDF":
			return new ECDHCWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha256Digest()));
		case "ECCDHWITHSHA384KDF":
			return new ECDHCWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha384Digest()));
		case "ECCDHWITHSHA512KDF":
			return new ECDHCWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha512Digest()));
		case "ECMQVWITHSHA1KDF":
			return new ECMqvWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha1Digest()));
		case "ECMQVWITHSHA224KDF":
			return new ECMqvWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha224Digest()));
		case "ECMQVWITHSHA256KDF":
			return new ECMqvWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha256Digest()));
		case "ECMQVWITHSHA384KDF":
			return new ECMqvWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha384Digest()));
		case "ECMQVWITHSHA512KDF":
			return new ECMqvWithKdfBasicAgreement(wrapAlgorithm, new ECDHKekGenerator(new Sha512Digest()));
		default:
			return null;
		}
	}

	public static IRawAgreement GetRawAgreement(DerObjectIdentifier oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (AlgorithmOidMap.TryGetValue(oid, out var value))
		{
			IRawAgreement rawAgreementForMechanism = GetRawAgreementForMechanism(value);
			if (rawAgreementForMechanism != null)
			{
				return rawAgreementForMechanism;
			}
		}
		throw new SecurityUtilityException("Raw Agreement OID not recognised.");
	}

	public static IRawAgreement GetRawAgreement(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		IRawAgreement rawAgreementForMechanism = GetRawAgreementForMechanism(GetMechanism(algorithm) ?? algorithm.ToUpperInvariant());
		if (rawAgreementForMechanism != null)
		{
			return rawAgreementForMechanism;
		}
		throw new SecurityUtilityException("Raw Agreement " + algorithm + " not recognised.");
	}

	private static IRawAgreement GetRawAgreementForMechanism(string mechanism)
	{
		if (mechanism == "X25519")
		{
			return new X25519Agreement();
		}
		if (mechanism == "X448")
		{
			return new X448Agreement();
		}
		return null;
	}

	private static string GetMechanism(string algorithm)
	{
		if (DerObjectIdentifier.TryFromID(algorithm, out var oid) && AlgorithmOidMap.TryGetValue(oid, out var value))
		{
			return value;
		}
		return null;
	}
}
