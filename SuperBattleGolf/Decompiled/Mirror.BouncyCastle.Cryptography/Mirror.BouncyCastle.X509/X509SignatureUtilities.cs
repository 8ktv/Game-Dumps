using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.X509;

internal class X509SignatureUtilities
{
	internal static bool AreEquivalentAlgorithms(AlgorithmIdentifier id1, AlgorithmIdentifier id2)
	{
		if (!id1.Algorithm.Equals(id2.Algorithm))
		{
			return false;
		}
		if (IsAbsentOrEmptyParameters(id1.Parameters) && IsAbsentOrEmptyParameters(id2.Parameters))
		{
			return true;
		}
		return object.Equals(id1.Parameters, id2.Parameters);
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
		return digestAlgOID.GetID();
	}

	internal static string GetSignatureName(AlgorithmIdentifier sigAlgID)
	{
		DerObjectIdentifier algorithm = sigAlgID.Algorithm;
		Asn1Encodable parameters = sigAlgID.Parameters;
		if (!IsAbsentOrEmptyParameters(parameters))
		{
			if (PkcsObjectIdentifiers.IdRsassaPss.Equals(algorithm))
			{
				return GetDigestAlgName(RsassaPssParameters.GetInstance(parameters).HashAlgorithm.Algorithm) + "withRSAandMGF1";
			}
			if (X9ObjectIdentifiers.ECDsaWithSha2.Equals(algorithm))
			{
				return GetDigestAlgName((DerObjectIdentifier)Asn1Sequence.GetInstance(parameters)[0]) + "withECDSA";
			}
		}
		return SignerUtilities.GetEncodingName(algorithm) ?? algorithm.GetID();
	}

	private static bool IsAbsentOrEmptyParameters(Asn1Encodable parameters)
	{
		if (parameters != null)
		{
			return DerNull.Instance.Equals(parameters);
		}
		return true;
	}
}
