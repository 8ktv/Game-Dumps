using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public sealed class Rfc6637Utilities
{
	private static readonly byte[] ANONYMOUS_SENDER = Hex.Decode("416E6F6E796D6F75732053656E64657220202020");

	private Rfc6637Utilities()
	{
	}

	public static string GetAgreementAlgorithm(PublicKeyPacket pubKeyData)
	{
		ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
		return eCDHPublicBcpgKey.HashAlgorithm switch
		{
			HashAlgorithmTag.Sha256 => "ECCDHwithSHA256CKDF", 
			HashAlgorithmTag.Sha384 => "ECCDHwithSHA384CKDF", 
			HashAlgorithmTag.Sha512 => "ECCDHwithSHA512CKDF", 
			_ => throw new ArgumentException("Unknown hash algorithm specified: " + eCDHPublicBcpgKey.HashAlgorithm), 
		};
	}

	public static DerObjectIdentifier GetKeyEncryptionOID(SymmetricKeyAlgorithmTag algID)
	{
		return algID switch
		{
			SymmetricKeyAlgorithmTag.Aes128 => NistObjectIdentifiers.IdAes128Wrap, 
			SymmetricKeyAlgorithmTag.Aes192 => NistObjectIdentifiers.IdAes192Wrap, 
			SymmetricKeyAlgorithmTag.Aes256 => NistObjectIdentifiers.IdAes256Wrap, 
			_ => throw new PgpException("unknown symmetric algorithm ID: " + algID), 
		};
	}

	public static int GetKeyLength(SymmetricKeyAlgorithmTag algID)
	{
		return algID switch
		{
			SymmetricKeyAlgorithmTag.Aes128 => 16, 
			SymmetricKeyAlgorithmTag.Aes192 => 24, 
			SymmetricKeyAlgorithmTag.Aes256 => 32, 
			_ => throw new PgpException("unknown symmetric algorithm ID: " + algID), 
		};
	}

	public static byte[] CreateKey(PublicKeyPacket pubKeyData, ECPoint s)
	{
		return CreateKey(pubKeyData, s.AffineXCoord.GetEncoded());
	}

	public static byte[] CreateKey(PublicKeyPacket pubKeyData, byte[] secret)
	{
		byte[] parameters = CreateUserKeyingMaterial(pubKeyData);
		ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
		return Kdf(eCDHPublicBcpgKey.HashAlgorithm, secret, GetKeyLength(eCDHPublicBcpgKey.SymmetricKeyAlgorithm), parameters);
	}

	public static byte[] CreateUserKeyingMaterial(PublicKeyPacket pubKeyData)
	{
		MemoryStream memoryStream = new MemoryStream();
		ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKeyData.Key;
		byte[] encoded = eCDHPublicBcpgKey.CurveOid.GetEncoded();
		memoryStream.Write(encoded, 1, encoded.Length - 1);
		memoryStream.WriteByte((byte)pubKeyData.Algorithm);
		memoryStream.WriteByte(3);
		memoryStream.WriteByte(1);
		memoryStream.WriteByte((byte)eCDHPublicBcpgKey.HashAlgorithm);
		memoryStream.WriteByte((byte)eCDHPublicBcpgKey.SymmetricKeyAlgorithm);
		memoryStream.Write(ANONYMOUS_SENDER, 0, ANONYMOUS_SENDER.Length);
		byte[] array = PgpPublicKey.CalculateFingerprint(pubKeyData);
		memoryStream.Write(array, 0, array.Length);
		return memoryStream.ToArray();
	}

	private static byte[] Kdf(HashAlgorithmTag digestAlg, byte[] ZB, int keyLen, byte[] parameters)
	{
		IDigest digest = PgpUtilities.CreateDigest(digestAlg);
		digest.Update(0);
		digest.Update(0);
		digest.Update(0);
		digest.Update(1);
		digest.BlockUpdate(ZB, 0, ZB.Length);
		digest.BlockUpdate(parameters, 0, parameters.Length);
		return Arrays.CopyOfRange(DigestUtilities.DoFinal(digest), 0, keyLen);
	}
}
