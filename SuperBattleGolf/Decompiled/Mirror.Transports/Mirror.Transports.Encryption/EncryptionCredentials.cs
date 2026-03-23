using System;
using System.IO;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;
using UnityEngine;

namespace Mirror.Transports.Encryption;

public class EncryptionCredentials
{
	private class SerializedPair
	{
		public string PublicKeyFingerprint;

		public string PublicKey;

		public string PrivateKey;
	}

	private const int PrivateKeyBits = 256;

	public byte[] PublicKeySerialized;

	public ECPrivateKeyParameters PrivateKey;

	public string PublicKeyFingerprint;

	private EncryptionCredentials()
	{
	}

	public static EncryptionCredentials Generate()
	{
		ECKeyPairGenerator eCKeyPairGenerator = new ECKeyPairGenerator();
		eCKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 256));
		AsymmetricCipherKeyPair asymmetricCipherKeyPair = eCKeyPairGenerator.GenerateKeyPair();
		byte[] array = SerializePublicKey((ECPublicKeyParameters)asymmetricCipherKeyPair.Public);
		return new EncryptionCredentials
		{
			PublicKeySerialized = array,
			PublicKeyFingerprint = PubKeyFingerprint(new ArraySegment<byte>(array)),
			PrivateKey = (ECPrivateKeyParameters)asymmetricCipherKeyPair.Private
		};
	}

	public static byte[] SerializePublicKey(AsymmetricKeyParameter publicKey)
	{
		return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey).ToAsn1Object().GetDerEncoded();
	}

	public static AsymmetricKeyParameter DeserializePublicKey(ArraySegment<byte> pubKey)
	{
		return PublicKeyFactory.CreateKey(new MemoryStream(pubKey.Array, pubKey.Offset, pubKey.Count, writable: false));
	}

	public static byte[] SerializePrivateKey(AsymmetricKeyParameter privateKey)
	{
		return PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey).ToAsn1Object().GetDerEncoded();
	}

	public static AsymmetricKeyParameter DeserializePrivateKey(ArraySegment<byte> privateKey)
	{
		return PrivateKeyFactory.CreateKey(new MemoryStream(privateKey.Array, privateKey.Offset, privateKey.Count, writable: false));
	}

	public static string PubKeyFingerprint(ArraySegment<byte> publicKeyBytes)
	{
		Sha256Digest sha256Digest = new Sha256Digest();
		byte[] array = new byte[sha256Digest.GetDigestSize()];
		sha256Digest.BlockUpdate(publicKeyBytes.Array, publicKeyBytes.Offset, publicKeyBytes.Count);
		sha256Digest.DoFinal(array, 0);
		return BitConverter.ToString(array).Replace("-", "").ToLowerInvariant();
	}

	public void SaveToFile(string path)
	{
		string contents = JsonUtility.ToJson(new SerializedPair
		{
			PublicKeyFingerprint = PublicKeyFingerprint,
			PublicKey = Convert.ToBase64String(PublicKeySerialized),
			PrivateKey = Convert.ToBase64String(SerializePrivateKey(PrivateKey))
		});
		File.WriteAllText(path, contents);
	}

	public static EncryptionCredentials LoadFromFile(string path)
	{
		SerializedPair serializedPair = JsonUtility.FromJson<SerializedPair>(File.ReadAllText(path));
		byte[] array = Convert.FromBase64String(serializedPair.PublicKey);
		byte[] array2 = Convert.FromBase64String(serializedPair.PrivateKey);
		if (serializedPair.PublicKeyFingerprint != PubKeyFingerprint(new ArraySegment<byte>(array)))
		{
			throw new Exception("Saved public key fingerprint does not match public key.");
		}
		return new EncryptionCredentials
		{
			PublicKeySerialized = array,
			PublicKeyFingerprint = serializedPair.PublicKeyFingerprint,
			PrivateKey = (ECPrivateKeyParameters)DeserializePrivateKey(new ArraySegment<byte>(array2))
		};
	}
}
