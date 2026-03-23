using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Iana;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Paddings;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Security;

public static class MacUtilities
{
	private static readonly Dictionary<string, string> AlgorithmMap;

	private static readonly Dictionary<DerObjectIdentifier, string> AlgorithmOidMap;

	static MacUtilities()
	{
		AlgorithmMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		AlgorithmOidMap = new Dictionary<DerObjectIdentifier, string>();
		AlgorithmOidMap[IanaObjectIdentifiers.HmacMD5] = "HMAC-MD5";
		AlgorithmOidMap[IanaObjectIdentifiers.HmacRipeMD160] = "HMAC-RIPEMD160";
		AlgorithmOidMap[IanaObjectIdentifiers.HmacSha1] = "HMAC-SHA1";
		AlgorithmOidMap[IanaObjectIdentifiers.HmacTiger] = "HMAC-TIGER";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha1] = "HMAC-SHA1";
		AlgorithmOidMap[MiscObjectIdentifiers.HMAC_SHA1] = "HMAC-SHA1";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha224] = "HMAC-SHA224";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha256] = "HMAC-SHA256";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha384] = "HMAC-SHA384";
		AlgorithmOidMap[PkcsObjectIdentifiers.IdHmacWithSha512] = "HMAC-SHA512";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_224] = "HMAC-SHA3-224";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_256] = "HMAC-SHA3-256";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_384] = "HMAC-SHA3-384";
		AlgorithmOidMap[NistObjectIdentifiers.IdHMacWithSha3_512] = "HMAC-SHA3-512";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_hmac_gost_3411_12_256] = "HMAC-GOST3411-2012-256";
		AlgorithmOidMap[RosstandartObjectIdentifiers.id_tc26_hmac_gost_3411_12_512] = "HMAC-GOST3411-2012-512";
		AlgorithmMap["DES"] = "DESMAC";
		AlgorithmMap["DES/CFB8"] = "DESMAC/CFB8";
		AlgorithmMap["DES64"] = "DESMAC64";
		AlgorithmMap["DESEDE"] = "DESEDEMAC";
		AlgorithmOidMap[PkcsObjectIdentifiers.DesEde3Cbc] = "DESEDEMAC";
		AlgorithmMap["DESEDE/CFB8"] = "DESEDEMAC/CFB8";
		AlgorithmMap["DESISO9797MAC"] = "DESWITHISO9797";
		AlgorithmMap["DESEDE64"] = "DESEDEMAC64";
		AlgorithmMap["DESEDE64WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
		AlgorithmMap["DESEDEISO9797ALG1MACWITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
		AlgorithmMap["DESEDEISO9797ALG1WITHISO7816-4PADDING"] = "DESEDEMAC64WITHISO7816-4PADDING";
		AlgorithmMap["ISO9797ALG3"] = "ISO9797ALG3MAC";
		AlgorithmMap["ISO9797ALG3MACWITHISO7816-4PADDING"] = "ISO9797ALG3WITHISO7816-4PADDING";
		AlgorithmMap["SKIPJACK"] = "SKIPJACKMAC";
		AlgorithmMap["SKIPJACK/CFB8"] = "SKIPJACKMAC/CFB8";
		AlgorithmMap["IDEA"] = "IDEAMAC";
		AlgorithmMap["IDEA/CFB8"] = "IDEAMAC/CFB8";
		AlgorithmMap["RC2"] = "RC2MAC";
		AlgorithmMap["RC2/CFB8"] = "RC2MAC/CFB8";
		AlgorithmMap["RC5"] = "RC5MAC";
		AlgorithmMap["RC5/CFB8"] = "RC5MAC/CFB8";
		AlgorithmMap["GOST28147"] = "GOST28147MAC";
		AlgorithmMap["VMPC"] = "VMPCMAC";
		AlgorithmMap["VMPC-MAC"] = "VMPCMAC";
		AlgorithmMap["SIPHASH"] = "SIPHASH-2-4";
		AlgorithmMap["PBEWITHHMACSHA"] = "PBEWITHHMACSHA1";
		AlgorithmOidMap[OiwObjectIdentifiers.IdSha1] = "PBEWITHHMACSHA1";
	}

	public static byte[] CalculateMac(string algorithm, ICipherParameters cp, byte[] input)
	{
		IMac mac = GetMac(algorithm);
		mac.Init(cp);
		mac.BlockUpdate(input, 0, input.Length);
		return DoFinal(mac);
	}

	public static byte[] DoFinal(IMac mac)
	{
		byte[] array = new byte[mac.GetMacSize()];
		mac.DoFinal(array, 0);
		return array;
	}

	public static byte[] DoFinal(IMac mac, byte[] input)
	{
		mac.BlockUpdate(input, 0, input.Length);
		return DoFinal(mac);
	}

	public static string GetAlgorithmName(DerObjectIdentifier oid)
	{
		return CollectionUtilities.GetValueOrNull(AlgorithmOidMap, oid);
	}

	public static IMac GetMac(DerObjectIdentifier id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		if (AlgorithmOidMap.TryGetValue(id, out var value))
		{
			IMac macForMechanism = GetMacForMechanism(value);
			if (macForMechanism != null)
			{
				return macForMechanism;
			}
		}
		throw new SecurityUtilityException("Mac OID not recognised.");
	}

	public static IMac GetMac(string algorithm)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		IMac macForMechanism = GetMacForMechanism(GetMechanism(algorithm) ?? algorithm.ToUpperInvariant());
		if (macForMechanism != null)
		{
			return macForMechanism;
		}
		throw new SecurityUtilityException("Mac " + algorithm + " not recognised.");
	}

	private static IMac GetMacForMechanism(string mechanism)
	{
		if (Platform.StartsWith(mechanism, "PBEWITH"))
		{
			mechanism = mechanism.Substring("PBEWITH".Length);
		}
		if (Platform.StartsWith(mechanism, "HMAC"))
		{
			string algorithm = ((!Platform.StartsWith(mechanism, "HMAC-") && !Platform.StartsWith(mechanism, "HMAC/")) ? mechanism.Substring(4) : mechanism.Substring(5));
			return new HMac(DigestUtilities.GetDigest(algorithm));
		}
		switch (mechanism)
		{
		case "AESCMAC":
			return new CMac(AesUtilities.CreateEngine());
		case "DESMAC":
			return new CbcBlockCipherMac(new DesEngine());
		case "DESMAC/CFB8":
			return new CfbBlockCipherMac(new DesEngine());
		case "DESMAC64":
			return new CbcBlockCipherMac(new DesEngine(), 64);
		case "DESEDECMAC":
			return new CMac(new DesEdeEngine());
		case "DESEDEMAC":
			return new CbcBlockCipherMac(new DesEdeEngine());
		case "DESEDEMAC/CFB8":
			return new CfbBlockCipherMac(new DesEdeEngine());
		case "DESEDEMAC64":
			return new CbcBlockCipherMac(new DesEdeEngine(), 64);
		case "DESEDEMAC64WITHISO7816-4PADDING":
			return new CbcBlockCipherMac(new DesEdeEngine(), 64, new ISO7816d4Padding());
		case "DESWITHISO9797":
		case "ISO9797ALG3MAC":
			return new ISO9797Alg3Mac(new DesEngine());
		case "ISO9797ALG3WITHISO7816-4PADDING":
			return new ISO9797Alg3Mac(new DesEngine(), new ISO7816d4Padding());
		case "SKIPJACKMAC":
			return new CbcBlockCipherMac(new SkipjackEngine());
		case "SKIPJACKMAC/CFB8":
			return new CfbBlockCipherMac(new SkipjackEngine());
		case "IDEAMAC":
			return new CbcBlockCipherMac(new IdeaEngine());
		case "IDEAMAC/CFB8":
			return new CfbBlockCipherMac(new IdeaEngine());
		case "RC2MAC":
			return new CbcBlockCipherMac(new RC2Engine());
		case "RC2MAC/CFB8":
			return new CfbBlockCipherMac(new RC2Engine());
		case "RC5MAC":
			return new CbcBlockCipherMac(new RC532Engine());
		case "RC5MAC/CFB8":
			return new CfbBlockCipherMac(new RC532Engine());
		case "GOST28147MAC":
			return new Gost28147Mac();
		case "VMPCMAC":
			return new VmpcMac();
		case "SIPHASH-2-4":
			return new SipHash();
		default:
			return null;
		}
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
}
