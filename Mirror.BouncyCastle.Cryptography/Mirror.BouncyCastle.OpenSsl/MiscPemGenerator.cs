using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;
using Mirror.BouncyCastle.Utilities.IO.Pem;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.OpenSsl;

public class MiscPemGenerator : PemObjectGenerator
{
	private readonly object obj;

	private readonly string algorithm;

	private readonly char[] password;

	private readonly SecureRandom random;

	public MiscPemGenerator(object obj)
		: this(obj, null, null, null)
	{
	}

	public MiscPemGenerator(object obj, string algorithm, char[] password, SecureRandom random)
	{
		this.obj = obj;
		this.algorithm = algorithm;
		this.password = password;
		this.random = random;
	}

	private static PemObject CreatePemObject(object obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (obj is AsymmetricCipherKeyPair asymmetricCipherKeyPair)
		{
			return CreatePemObject(asymmetricCipherKeyPair.Private);
		}
		if (obj is PemObject result)
		{
			return result;
		}
		if (obj is PemObjectGenerator pemObjectGenerator)
		{
			return pemObjectGenerator.Generate();
		}
		string keyType;
		byte[] content;
		if (obj is X509Certificate x509Certificate)
		{
			keyType = "CERTIFICATE";
			try
			{
				content = x509Certificate.GetEncoded();
			}
			catch (CertificateEncodingException ex)
			{
				throw new IOException("Cannot Encode object: " + ex.ToString());
			}
		}
		else if (obj is X509Crl x509Crl)
		{
			keyType = "X509 CRL";
			try
			{
				content = x509Crl.GetEncoded();
			}
			catch (CrlException ex2)
			{
				throw new IOException("Cannot Encode object: " + ex2.ToString());
			}
		}
		else if (obj is AsymmetricKeyParameter asymmetricKeyParameter)
		{
			content = ((!asymmetricKeyParameter.IsPrivate) ? EncodePublicKey(asymmetricKeyParameter, out keyType) : EncodePrivateKey(asymmetricKeyParameter, out keyType));
		}
		else if (obj is PrivateKeyInfo info)
		{
			content = EncodePrivateKeyInfo(info, out keyType);
		}
		else if (obj is SubjectPublicKeyInfo info2)
		{
			content = EncodePublicKeyInfo(info2, out keyType);
		}
		else if (obj is X509V2AttributeCertificate x509V2AttributeCertificate)
		{
			keyType = "ATTRIBUTE CERTIFICATE";
			content = x509V2AttributeCertificate.GetEncoded();
		}
		else if (obj is Pkcs8EncryptedPrivateKeyInfo pkcs8EncryptedPrivateKeyInfo)
		{
			keyType = "ENCRYPTED PRIVATE KEY";
			content = pkcs8EncryptedPrivateKeyInfo.GetEncoded();
		}
		else if (obj is Pkcs10CertificationRequest pkcs10CertificationRequest)
		{
			keyType = "CERTIFICATE REQUEST";
			content = pkcs10CertificationRequest.GetEncoded();
		}
		else if (obj is Mirror.BouncyCastle.Asn1.Cms.ContentInfo contentInfo)
		{
			keyType = "PKCS7";
			content = contentInfo.GetEncoded();
		}
		else
		{
			if (!(obj is Mirror.BouncyCastle.Asn1.Pkcs.ContentInfo contentInfo2))
			{
				throw new PemGenerationException("Object type not supported: " + Platform.GetTypeName(obj));
			}
			keyType = "PKCS7";
			content = contentInfo2.GetEncoded();
		}
		return new PemObject(keyType, content);
	}

	private static PemObject CreatePemObject(object obj, string algorithm, char[] password, SecureRandom random)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		if (obj is AsymmetricCipherKeyPair asymmetricCipherKeyPair)
		{
			return CreatePemObject(asymmetricCipherKeyPair.Private, algorithm, password, random);
		}
		string keyType = null;
		byte[] array = null;
		if (obj is AsymmetricKeyParameter { IsPrivate: not false } asymmetricKeyParameter)
		{
			array = EncodePrivateKey(asymmetricKeyParameter, out keyType);
		}
		if (keyType == null || array == null)
		{
			throw new PemGenerationException("Object type not supported: " + Platform.GetTypeName(obj));
		}
		string text = algorithm.ToUpperInvariant();
		if (text == "DESEDE")
		{
			text = "DES-EDE3-CBC";
		}
		byte[] array2 = new byte[Platform.StartsWith(text, "AES-") ? 16 : 8];
		random.NextBytes(array2);
		byte[] content = PemUtilities.Crypt(encrypt: true, array, password, text, array2);
		List<PemHeader> list = new List<PemHeader>(2);
		list.Add(new PemHeader("Proc-Type", "4,ENCRYPTED"));
		list.Add(new PemHeader("DEK-Info", text + "," + Hex.ToHexString(array2, upperCase: true)));
		return new PemObject(keyType, list, content);
	}

	public PemObject Generate()
	{
		try
		{
			if (algorithm != null)
			{
				return CreatePemObject(obj, algorithm, password, random);
			}
			return CreatePemObject(obj);
		}
		catch (IOException innerException)
		{
			throw new PemGenerationException("encoding exception", innerException);
		}
	}

	private static byte[] EncodePrivateKey(AsymmetricKeyParameter akp, out string keyType)
	{
		return EncodePrivateKeyInfo(PrivateKeyInfoFactory.CreatePrivateKeyInfo(akp), out keyType);
	}

	private static byte[] EncodePrivateKeyInfo(PrivateKeyInfo info, out string keyType)
	{
		AlgorithmIdentifier privateKeyAlgorithm = info.PrivateKeyAlgorithm;
		DerObjectIdentifier derObjectIdentifier = privateKeyAlgorithm.Algorithm;
		if (derObjectIdentifier.Equals(PkcsObjectIdentifiers.RsaEncryption))
		{
			keyType = "RSA PRIVATE KEY";
			return info.ParsePrivateKey().GetEncoded();
		}
		if (derObjectIdentifier.Equals(X9ObjectIdentifiers.IdECPublicKey) || derObjectIdentifier.Equals(CryptoProObjectIdentifiers.GostR3410x2001))
		{
			keyType = "EC PRIVATE KEY";
			return info.ParsePrivateKey().GetEncoded();
		}
		if (derObjectIdentifier.Equals(X9ObjectIdentifiers.IdDsa) || derObjectIdentifier.Equals(OiwObjectIdentifiers.DsaWithSha1))
		{
			keyType = "DSA PRIVATE KEY";
			DsaParameter instance = DsaParameter.GetInstance(privateKeyAlgorithm.Parameters);
			BigInteger value = DerInteger.GetInstance(info.ParsePrivateKey()).Value;
			BigInteger value2 = instance.G.ModPow(value, instance.P);
			return new DerSequence(new DerInteger(0), new DerInteger(instance.P), new DerInteger(instance.Q), new DerInteger(instance.G), new DerInteger(value2), new DerInteger(value)).GetEncoded();
		}
		keyType = "PRIVATE KEY";
		return info.GetEncoded();
	}

	private static byte[] EncodePublicKey(AsymmetricKeyParameter akp, out string keyType)
	{
		return EncodePublicKeyInfo(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(akp), out keyType);
	}

	private static byte[] EncodePublicKeyInfo(SubjectPublicKeyInfo info, out string keyType)
	{
		keyType = "PUBLIC KEY";
		return info.GetEncoded();
	}
}
