using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.GM;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Rosstandart;
using Mirror.BouncyCastle.Asn1.TeleTrust;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Cms;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Tsp;

public class TspUtil
{
	private static readonly Dictionary<string, int> DigestLengths;

	private static readonly Dictionary<string, string> DigestNames;

	static TspUtil()
	{
		DigestLengths = new Dictionary<string, int>();
		DigestNames = new Dictionary<string, string>();
		DigestLengths.Add(PkcsObjectIdentifiers.MD5.Id, 16);
		DigestLengths.Add(OiwObjectIdentifiers.IdSha1.Id, 20);
		DigestLengths.Add(NistObjectIdentifiers.IdSha224.Id, 28);
		DigestLengths.Add(NistObjectIdentifiers.IdSha256.Id, 32);
		DigestLengths.Add(NistObjectIdentifiers.IdSha384.Id, 48);
		DigestLengths.Add(NistObjectIdentifiers.IdSha512.Id, 64);
		DigestLengths.Add(TeleTrusTObjectIdentifiers.RipeMD128.Id, 16);
		DigestLengths.Add(TeleTrusTObjectIdentifiers.RipeMD160.Id, 20);
		DigestLengths.Add(TeleTrusTObjectIdentifiers.RipeMD256.Id, 32);
		DigestLengths.Add(CryptoProObjectIdentifiers.GostR3411.Id, 32);
		DigestLengths.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256.Id, 32);
		DigestLengths.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512.Id, 64);
		DigestLengths.Add(GMObjectIdentifiers.sm3.Id, 32);
		DigestNames.Add(PkcsObjectIdentifiers.MD5.Id, "MD5");
		DigestNames.Add(OiwObjectIdentifiers.IdSha1.Id, "SHA1");
		DigestNames.Add(NistObjectIdentifiers.IdSha224.Id, "SHA224");
		DigestNames.Add(NistObjectIdentifiers.IdSha256.Id, "SHA256");
		DigestNames.Add(NistObjectIdentifiers.IdSha384.Id, "SHA384");
		DigestNames.Add(NistObjectIdentifiers.IdSha512.Id, "SHA512");
		DigestNames.Add(PkcsObjectIdentifiers.MD5WithRsaEncryption.Id, "MD5");
		DigestNames.Add(PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id, "SHA1");
		DigestNames.Add(PkcsObjectIdentifiers.Sha224WithRsaEncryption.Id, "SHA224");
		DigestNames.Add(PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, "SHA256");
		DigestNames.Add(PkcsObjectIdentifiers.Sha384WithRsaEncryption.Id, "SHA384");
		DigestNames.Add(PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id, "SHA512");
		DigestNames.Add(TeleTrusTObjectIdentifiers.RipeMD128.Id, "RIPEMD128");
		DigestNames.Add(TeleTrusTObjectIdentifiers.RipeMD160.Id, "RIPEMD160");
		DigestNames.Add(TeleTrusTObjectIdentifiers.RipeMD256.Id, "RIPEMD256");
		DigestNames.Add(CryptoProObjectIdentifiers.GostR3411.Id, "GOST3411");
		DigestNames.Add(OiwObjectIdentifiers.DsaWithSha1.Id, "SHA1");
		DigestNames.Add(OiwObjectIdentifiers.Sha1WithRsa.Id, "SHA1");
		DigestNames.Add(OiwObjectIdentifiers.MD5WithRsa.Id, "MD5");
		DigestNames.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_256.Id, "GOST3411-2012-256");
		DigestNames.Add(RosstandartObjectIdentifiers.id_tc26_gost_3411_12_512.Id, "GOST3411-2012-512");
		DigestNames.Add(GMObjectIdentifiers.sm3.Id, "SM3");
	}

	public static IList<TimeStampToken> GetSignatureTimestamps(SignerInformation signerInfo)
	{
		List<TimeStampToken> list = new List<TimeStampToken>();
		Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttributes = signerInfo.UnsignedAttributes;
		if (unsignedAttributes != null)
		{
			foreach (Mirror.BouncyCastle.Asn1.Cms.Attribute item in unsignedAttributes.GetAll(PkcsObjectIdentifiers.IdAASignatureTimeStampToken))
			{
				foreach (Asn1Encodable attrValue in item.AttrValues)
				{
					try
					{
						TimeStampToken timeStampToken = new TimeStampToken(Mirror.BouncyCastle.Asn1.Cms.ContentInfo.GetInstance(attrValue.ToAsn1Object()));
						TimeStampTokenInfo timeStampInfo = timeStampToken.TimeStampInfo;
						if (!Arrays.FixedTimeEquals(DigestUtilities.CalculateDigest(GetDigestAlgName(timeStampInfo.MessageImprintAlgOid), signerInfo.GetSignature()), timeStampInfo.GetMessageImprintDigest()))
						{
							throw new TspValidationException("Incorrect digest in message imprint");
						}
						list.Add(timeStampToken);
					}
					catch (SecurityUtilityException)
					{
						throw new TspValidationException("Unknown hash algorithm specified in timestamp");
					}
					catch (Exception)
					{
						throw new TspValidationException("Timestamp could not be parsed");
					}
				}
			}
		}
		return list;
	}

	public static void ValidateCertificate(X509Certificate cert)
	{
		if (cert.Version != 3)
		{
			throw new ArgumentException("Certificate must have an ExtendedKeyUsage extension.");
		}
		Asn1OctetString extensionValue = cert.GetExtensionValue(X509Extensions.ExtendedKeyUsage);
		if (extensionValue == null)
		{
			throw new TspValidationException("Certificate must have an ExtendedKeyUsage extension.");
		}
		if (!cert.GetCriticalExtensionOids().Contains(X509Extensions.ExtendedKeyUsage.Id))
		{
			throw new TspValidationException("Certificate must have an ExtendedKeyUsage extension marked as critical.");
		}
		try
		{
			ExtendedKeyUsage instance = ExtendedKeyUsage.GetInstance(Asn1Object.FromByteArray(extensionValue.GetOctets()));
			if (!instance.HasKeyPurposeId(KeyPurposeID.id_kp_timeStamping) || instance.Count != 1)
			{
				throw new TspValidationException("ExtendedKeyUsage not solely time stamping.");
			}
		}
		catch (IOException)
		{
			throw new TspValidationException("cannot process ExtendedKeyUsage extension");
		}
	}

	internal static string GetDigestAlgName(string digestAlgOid)
	{
		return CollectionUtilities.GetValueOrKey(DigestNames, digestAlgOid);
	}

	internal static int GetDigestLength(string digestAlgOid)
	{
		if (!DigestLengths.TryGetValue(digestAlgOid, out var value))
		{
			throw new TspException("digest algorithm cannot be found.");
		}
		return value;
	}

	internal static IDigest CreateDigestInstance(string digestAlgOID)
	{
		return DigestUtilities.GetDigest(GetDigestAlgName(digestAlgOID));
	}

	internal static HashSet<DerObjectIdentifier> GetCriticalExtensionOids(X509Extensions extensions)
	{
		if (extensions != null)
		{
			return new HashSet<DerObjectIdentifier>(extensions.GetCriticalExtensionOids());
		}
		return new HashSet<DerObjectIdentifier>();
	}

	internal static HashSet<DerObjectIdentifier> GetNonCriticalExtensionOids(X509Extensions extensions)
	{
		if (extensions != null)
		{
			return new HashSet<DerObjectIdentifier>(extensions.GetNonCriticalExtensionOids());
		}
		return new HashSet<DerObjectIdentifier>();
	}

	internal static IList<DerObjectIdentifier> GetExtensionOids(X509Extensions extensions)
	{
		if (extensions != null)
		{
			return new List<DerObjectIdentifier>(extensions.GetExtensionOids());
		}
		return new List<DerObjectIdentifier>();
	}
}
