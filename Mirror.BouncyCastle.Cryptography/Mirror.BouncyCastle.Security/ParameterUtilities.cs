using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Kisa;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Nsri;
using Mirror.BouncyCastle.Asn1.Ntt;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class ParameterUtilities
{
	private static readonly IDictionary<string, string> Algorithms;

	private static readonly IDictionary<string, int> BasicIVSizes;

	static ParameterUtilities()
	{
		Algorithms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		BasicIVSizes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
		AddAlgorithm("AES", "AESWRAP");
		AddAlgorithm("AES128", SecurityUtilities.WrongAes128, NistObjectIdentifiers.IdAes128Cbc, NistObjectIdentifiers.IdAes128Ccm, NistObjectIdentifiers.IdAes128Cfb, NistObjectIdentifiers.IdAes128Ecb, NistObjectIdentifiers.IdAes128Gcm, NistObjectIdentifiers.IdAes128Ofb, NistObjectIdentifiers.IdAes128Wrap, NistObjectIdentifiers.IdAes128WrapPad);
		AddAlgorithm("AES192", SecurityUtilities.WrongAes192, NistObjectIdentifiers.IdAes192Cbc, NistObjectIdentifiers.IdAes192Ccm, NistObjectIdentifiers.IdAes192Cfb, NistObjectIdentifiers.IdAes192Ecb, NistObjectIdentifiers.IdAes192Gcm, NistObjectIdentifiers.IdAes192Ofb, NistObjectIdentifiers.IdAes192Wrap, NistObjectIdentifiers.IdAes192WrapPad);
		AddAlgorithm("AES256", SecurityUtilities.WrongAes256, NistObjectIdentifiers.IdAes256Cbc, NistObjectIdentifiers.IdAes256Ccm, NistObjectIdentifiers.IdAes256Cfb, NistObjectIdentifiers.IdAes256Ecb, NistObjectIdentifiers.IdAes256Gcm, NistObjectIdentifiers.IdAes256Ofb, NistObjectIdentifiers.IdAes256Wrap, NistObjectIdentifiers.IdAes256WrapPad);
		AddAlgorithm("ARIA");
		AddAlgorithm("ARIA128", NsriObjectIdentifiers.id_aria128_cbc, NsriObjectIdentifiers.id_aria128_ccm, NsriObjectIdentifiers.id_aria128_cfb, NsriObjectIdentifiers.id_aria128_ctr, NsriObjectIdentifiers.id_aria128_ecb, NsriObjectIdentifiers.id_aria128_gcm, NsriObjectIdentifiers.id_aria128_kw, NsriObjectIdentifiers.id_aria128_kwp, NsriObjectIdentifiers.id_aria128_ocb2, NsriObjectIdentifiers.id_aria128_ofb);
		AddAlgorithm("ARIA192", NsriObjectIdentifiers.id_aria192_cbc, NsriObjectIdentifiers.id_aria192_ccm, NsriObjectIdentifiers.id_aria192_cfb, NsriObjectIdentifiers.id_aria192_ctr, NsriObjectIdentifiers.id_aria192_ecb, NsriObjectIdentifiers.id_aria192_gcm, NsriObjectIdentifiers.id_aria192_kw, NsriObjectIdentifiers.id_aria192_kwp, NsriObjectIdentifiers.id_aria192_ocb2, NsriObjectIdentifiers.id_aria192_ofb);
		AddAlgorithm("ARIA256", NsriObjectIdentifiers.id_aria256_cbc, NsriObjectIdentifiers.id_aria256_ccm, NsriObjectIdentifiers.id_aria256_cfb, NsriObjectIdentifiers.id_aria256_ctr, NsriObjectIdentifiers.id_aria256_ecb, NsriObjectIdentifiers.id_aria256_gcm, NsriObjectIdentifiers.id_aria256_kw, NsriObjectIdentifiers.id_aria256_kwp, NsriObjectIdentifiers.id_aria256_ocb2, NsriObjectIdentifiers.id_aria256_ofb);
		AddAlgorithm("BLOWFISH", "1.3.6.1.4.1.3029.1.2", MiscObjectIdentifiers.cryptlib_algorithm_blowfish_CBC);
		AddAlgorithm("CAMELLIA", "CAMELLIAWRAP");
		AddAlgorithm("CAMELLIA128", NttObjectIdentifiers.IdCamellia128Cbc, NttObjectIdentifiers.IdCamellia128Wrap);
		AddAlgorithm("CAMELLIA192", NttObjectIdentifiers.IdCamellia192Cbc, NttObjectIdentifiers.IdCamellia192Wrap);
		AddAlgorithm("CAMELLIA256", NttObjectIdentifiers.IdCamellia256Cbc, NttObjectIdentifiers.IdCamellia256Wrap);
		AddAlgorithm("CAST5", MiscObjectIdentifiers.cast5CBC);
		AddAlgorithm("CAST6");
		AddAlgorithm("CHACHA");
		AddAlgorithm("CHACHA7539", "CHACHA20", "CHACHA20-POLY1305", PkcsObjectIdentifiers.IdAlgAeadChaCha20Poly1305);
		AddAlgorithm("DES", OiwObjectIdentifiers.DesCbc, OiwObjectIdentifiers.DesCfb, OiwObjectIdentifiers.DesEcb, OiwObjectIdentifiers.DesOfb);
		AddAlgorithm("DESEDE", "DESEDEWRAP", "TDEA", OiwObjectIdentifiers.DesEde, PkcsObjectIdentifiers.IdAlgCms3DesWrap);
		AddAlgorithm("DESEDE3", PkcsObjectIdentifiers.DesEde3Cbc);
		AddAlgorithm("GOST28147", "GOST", "GOST-28147", CryptoProObjectIdentifiers.GostR28147Gcfb);
		AddAlgorithm("HC128");
		AddAlgorithm("HC256");
		AddAlgorithm("IDEA", MiscObjectIdentifiers.as_sys_sec_alg_ideaCBC);
		AddAlgorithm("NOEKEON");
		AddAlgorithm("RC2", PkcsObjectIdentifiers.RC2Cbc, PkcsObjectIdentifiers.IdAlgCmsRC2Wrap);
		AddAlgorithm("RC4", "ARC4", PkcsObjectIdentifiers.rc4);
		AddAlgorithm("RC5", "RC5-32");
		AddAlgorithm("RC5-64");
		AddAlgorithm("RC6");
		AddAlgorithm("RIJNDAEL");
		AddAlgorithm("SALSA20");
		AddAlgorithm("SEED", KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap, KisaObjectIdentifiers.IdSeedCbc);
		AddAlgorithm("SERPENT");
		AddAlgorithm("SKIPJACK");
		AddAlgorithm("SM4");
		AddAlgorithm("TEA");
		AddAlgorithm("THREEFISH-256");
		AddAlgorithm("THREEFISH-512");
		AddAlgorithm("THREEFISH-1024");
		AddAlgorithm("TNEPRES");
		AddAlgorithm("TWOFISH");
		AddAlgorithm("VMPC");
		AddAlgorithm("VMPC-KSA3");
		AddAlgorithm("XTEA");
		AddBasicIVSizeEntries(8, "BLOWFISH", "CHACHA", "DES", "DESEDE", "DESEDE3", "SALSA20");
		AddBasicIVSizeEntries(12, "CHACHA7539");
		AddBasicIVSizeEntries(16, "AES", "AES128", "AES192", "AES256", "ARIA", "ARIA128", "ARIA192", "ARIA256", "CAMELLIA", "CAMELLIA128", "CAMELLIA192", "CAMELLIA256", "NOEKEON", "SEED", "SM4");
	}

	private static void AddAlgorithm(string canonicalName, params object[] aliases)
	{
		Algorithms[canonicalName] = canonicalName;
		foreach (object obj in aliases)
		{
			Algorithms[obj.ToString()] = canonicalName;
		}
	}

	private static void AddBasicIVSizeEntries(int size, params string[] algorithms)
	{
		foreach (string key in algorithms)
		{
			BasicIVSizes.Add(key, size);
		}
	}

	public static string GetCanonicalAlgorithmName(string algorithm)
	{
		return CollectionUtilities.GetValueOrNull(Algorithms, algorithm);
	}

	public static KeyParameter CreateKeyParameter(DerObjectIdentifier algOid, byte[] keyBytes)
	{
		return CreateKeyParameter(algOid.Id, keyBytes, 0, keyBytes.Length);
	}

	public static KeyParameter CreateKeyParameter(string algorithm, byte[] keyBytes)
	{
		return CreateKeyParameter(algorithm, keyBytes, 0, keyBytes.Length);
	}

	public static KeyParameter CreateKeyParameter(DerObjectIdentifier algOid, byte[] keyBytes, int offset, int length)
	{
		return CreateKeyParameter(algOid.Id, keyBytes, offset, length);
	}

	public static KeyParameter CreateKeyParameter(string algorithm, byte[] keyBytes, int offset, int length)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		string canonicalAlgorithmName = GetCanonicalAlgorithmName(algorithm);
		if (canonicalAlgorithmName == null)
		{
			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}
		switch (canonicalAlgorithmName)
		{
		case "DES":
			return new DesParameters(keyBytes, offset, length);
		case "DESEDE":
		case "DESEDE3":
			return new DesEdeParameters(keyBytes, offset, length);
		case "RC2":
			return new RC2Parameters(keyBytes, offset, length);
		default:
			return new KeyParameter(keyBytes, offset, length);
		}
	}

	public static ICipherParameters GetCipherParameters(DerObjectIdentifier algOid, ICipherParameters key, Asn1Object asn1Params)
	{
		return GetCipherParameters(algOid.Id, key, asn1Params);
	}

	public static ICipherParameters GetCipherParameters(string algorithm, ICipherParameters key, Asn1Object asn1Params)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (NistObjectIdentifiers.IdAes128Gcm.Id.Equals(algorithm) || NistObjectIdentifiers.IdAes192Gcm.Id.Equals(algorithm) || NistObjectIdentifiers.IdAes256Gcm.Id.Equals(algorithm))
		{
			KeyParameter key2 = (key as KeyParameter) ?? throw new ArgumentException("key data must be accessible for GCM operation");
			GcmParameters instance = GcmParameters.GetInstance(asn1Params);
			return new AeadParameters(key2, instance.IcvLen * 8, instance.GetNonce());
		}
		if (NistObjectIdentifiers.IdAes128Ccm.Id.Equals(algorithm) || NistObjectIdentifiers.IdAes192Ccm.Id.Equals(algorithm) || NistObjectIdentifiers.IdAes256Ccm.Id.Equals(algorithm))
		{
			KeyParameter key3 = (key as KeyParameter) ?? throw new ArgumentException("key data must be accessible for CCM operation");
			CcmParameters instance2 = CcmParameters.GetInstance(asn1Params);
			return new AeadParameters(key3, instance2.IcvLen * 8, instance2.GetNonce());
		}
		string canonicalAlgorithmName = GetCanonicalAlgorithmName(algorithm);
		if (canonicalAlgorithmName == null)
		{
			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}
		byte[] array = null;
		try
		{
			if (FindBasicIVSize(canonicalAlgorithmName) == -1)
			{
				switch (canonicalAlgorithmName)
				{
				default:
					goto end_IL_00fa;
				case "RIJNDAEL":
				case "SKIPJACK":
				case "TWOFISH":
					break;
				case "CAST5":
					array = Cast5CbcParameters.GetInstance(asn1Params).GetIV();
					goto end_IL_00fa;
				case "IDEA":
					array = IdeaCbcPar.GetInstance(asn1Params).GetIV();
					goto end_IL_00fa;
				case "RC2":
					array = RC2CbcParameter.GetInstance(asn1Params).GetIV();
					goto end_IL_00fa;
				}
			}
			array = ((Asn1OctetString)asn1Params).GetOctets();
			end_IL_00fa:;
		}
		catch (Exception innerException)
		{
			throw new ArgumentException("Could not process ASN.1 parameters", innerException);
		}
		if (array != null)
		{
			return new ParametersWithIV(key, array);
		}
		throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
	}

	public static Asn1Encodable GenerateParameters(DerObjectIdentifier algID, SecureRandom random)
	{
		return GenerateParameters(algID.Id, random);
	}

	public static Asn1Encodable GenerateParameters(string algorithm, SecureRandom random)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		string canonicalAlgorithmName = GetCanonicalAlgorithmName(algorithm);
		if (canonicalAlgorithmName == null)
		{
			throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised.");
		}
		int num = FindBasicIVSize(canonicalAlgorithmName);
		if (num != -1)
		{
			return CreateIVOctetString(random, num);
		}
		return canonicalAlgorithmName switch
		{
			"CAST5" => new Cast5CbcParameters(CreateIV(random, 8), 128), 
			"IDEA" => new IdeaCbcPar(CreateIV(random, 8)), 
			"RC2" => new RC2CbcParameter(CreateIV(random, 8)), 
			_ => throw new SecurityUtilityException("Algorithm " + algorithm + " not recognised."), 
		};
	}

	public static ICipherParameters WithRandom(ICipherParameters cp, SecureRandom random)
	{
		if (random != null)
		{
			cp = new ParametersWithRandom(cp, random);
		}
		return cp;
	}

	private static Asn1OctetString CreateIVOctetString(SecureRandom random, int ivLength)
	{
		return new DerOctetString(CreateIV(random, ivLength));
	}

	private static byte[] CreateIV(SecureRandom random, int ivLength)
	{
		return SecureRandom.GetNextBytes(random, ivLength);
	}

	private static int FindBasicIVSize(string canonicalName)
	{
		if (!BasicIVSizes.TryGetValue(canonicalName, out var value))
		{
			return -1;
		}
		return value;
	}
}
