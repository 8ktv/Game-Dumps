using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cryptlib;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpEncryptedDataGenerator : IStreamGenerator
{
	private abstract class EncMethod : ContainedPacket
	{
		protected byte[] sessionInfo;

		protected SymmetricKeyAlgorithmTag encAlgorithm;

		protected KeyParameter key;

		public abstract void AddSessionInfo(byte[] si, SecureRandom random);
	}

	private class PbeMethod : EncMethod
	{
		private S2k s2k;

		internal PbeMethod(SymmetricKeyAlgorithmTag encAlgorithm, S2k s2k, KeyParameter key)
		{
			base.encAlgorithm = encAlgorithm;
			this.s2k = s2k;
			base.key = key;
		}

		public KeyParameter GetKey()
		{
			return key;
		}

		public override void AddSessionInfo(byte[] si, SecureRandom random)
		{
			IBufferedCipher cipher = CipherUtilities.GetCipher(PgpUtilities.GetSymmetricCipherName(encAlgorithm) + "/CFB/NoPadding");
			byte[] iv = new byte[cipher.GetBlockSize()];
			cipher.Init(forEncryption: true, new ParametersWithRandom(new ParametersWithIV(key, iv), random));
			sessionInfo = cipher.DoFinal(si, 0, si.Length - 2);
		}

		public override void Encode(BcpgOutputStream pOut)
		{
			SymmetricKeyEncSessionPacket p = new SymmetricKeyEncSessionPacket(encAlgorithm, s2k, sessionInfo);
			pOut.WritePacket(p);
		}
	}

	private class PubMethod : EncMethod
	{
		internal PgpPublicKey pubKey;

		internal bool sessionKeyObfuscation;

		internal byte[][] data;

		internal PubMethod(PgpPublicKey pubKey, bool sessionKeyObfuscation)
		{
			this.pubKey = pubKey;
			this.sessionKeyObfuscation = sessionKeyObfuscation;
		}

		public override void AddSessionInfo(byte[] sessionInfo, SecureRandom random)
		{
			byte[] encryptedSessionInfo = EncryptSessionInfo(sessionInfo, random);
			data = ProcessSessionInfo(encryptedSessionInfo);
		}

		private byte[] EncryptSessionInfo(byte[] sessionInfo, SecureRandom random)
		{
			AsymmetricKeyParameter asymmetricKeyParameter = pubKey.GetKey();
			if (pubKey.Algorithm != PublicKeyAlgorithmTag.ECDH)
			{
				IBufferedCipher cipher;
				switch (pubKey.Algorithm)
				{
				case PublicKeyAlgorithmTag.RsaGeneral:
				case PublicKeyAlgorithmTag.RsaEncrypt:
					cipher = CipherUtilities.GetCipher("RSA//PKCS1Padding");
					break;
				case PublicKeyAlgorithmTag.ElGamalEncrypt:
				case PublicKeyAlgorithmTag.ElGamalGeneral:
					cipher = CipherUtilities.GetCipher("ElGamal/ECB/PKCS1Padding");
					break;
				case PublicKeyAlgorithmTag.Dsa:
					throw new PgpException("Can't use DSA for encryption.");
				case PublicKeyAlgorithmTag.ECDsa:
					throw new PgpException("Can't use ECDSA for encryption.");
				case PublicKeyAlgorithmTag.EdDsa:
					throw new PgpException("Can't use EdDSA for encryption.");
				default:
					throw new PgpException("unknown asymmetric algorithm: " + pubKey.Algorithm);
				}
				cipher.Init(forEncryption: true, new ParametersWithRandom(asymmetricKeyParameter, random));
				return cipher.DoFinal(sessionInfo);
			}
			ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)pubKey.PublicKeyPacket.Key;
			DerObjectIdentifier curveOid = eCDHPublicBcpgKey.CurveOid;
			if (EdECObjectIdentifiers.id_X25519.Equals(curveOid) || CryptlibObjectIdentifiers.curvey25519.Equals(curveOid))
			{
				X25519KeyPairGenerator x25519KeyPairGenerator = new X25519KeyPairGenerator();
				x25519KeyPairGenerator.Init(new X25519KeyGenerationParameters(random));
				AsymmetricCipherKeyPair asymmetricCipherKeyPair = x25519KeyPairGenerator.GenerateKeyPair();
				X25519Agreement x25519Agreement = new X25519Agreement();
				x25519Agreement.Init(asymmetricCipherKeyPair.Private);
				byte[] array = new byte[x25519Agreement.AgreementSize];
				x25519Agreement.CalculateAgreement(asymmetricKeyParameter, array, 0);
				byte[] array2 = new byte[1 + X25519PublicKeyParameters.KeySize];
				((X25519PublicKeyParameters)asymmetricCipherKeyPair.Public).Encode(array2, 1);
				array2[0] = 64;
				return EncryptSessionInfo(eCDHPublicBcpgKey, sessionInfo, array, array2, random);
			}
			if (EdECObjectIdentifiers.id_X448.Equals(curveOid))
			{
				X448KeyPairGenerator x448KeyPairGenerator = new X448KeyPairGenerator();
				x448KeyPairGenerator.Init(new X448KeyGenerationParameters(random));
				AsymmetricCipherKeyPair asymmetricCipherKeyPair2 = x448KeyPairGenerator.GenerateKeyPair();
				X448Agreement x448Agreement = new X448Agreement();
				x448Agreement.Init(asymmetricCipherKeyPair2.Private);
				byte[] array3 = new byte[x448Agreement.AgreementSize];
				x448Agreement.CalculateAgreement(asymmetricKeyParameter, array3, 0);
				byte[] array4 = new byte[1 + X448PublicKeyParameters.KeySize];
				((X448PublicKeyParameters)asymmetricCipherKeyPair2.Public).Encode(array4, 1);
				array4[0] = 64;
				return EncryptSessionInfo(eCDHPublicBcpgKey, sessionInfo, array3, array4, random);
			}
			ECDomainParameters parameters = ((ECPublicKeyParameters)asymmetricKeyParameter).Parameters;
			ECKeyPairGenerator eCKeyPairGenerator = new ECKeyPairGenerator();
			eCKeyPairGenerator.Init(new ECKeyGenerationParameters(parameters, random));
			AsymmetricCipherKeyPair asymmetricCipherKeyPair3 = eCKeyPairGenerator.GenerateKeyPair();
			ECDHBasicAgreement eCDHBasicAgreement = new ECDHBasicAgreement();
			eCDHBasicAgreement.Init(asymmetricCipherKeyPair3.Private);
			byte[] secret = BigIntegers.AsUnsignedByteArray(n: eCDHBasicAgreement.CalculateAgreement(asymmetricKeyParameter), length: eCDHBasicAgreement.GetFieldSize());
			byte[] encoded = ((ECPublicKeyParameters)asymmetricCipherKeyPair3.Public).Q.GetEncoded(compressed: false);
			return EncryptSessionInfo(eCDHPublicBcpgKey, sessionInfo, secret, encoded, random);
		}

		private byte[] EncryptSessionInfo(ECDHPublicBcpgKey ecPubKey, byte[] sessionInfo, byte[] secret, byte[] ephPubEncoding, SecureRandom random)
		{
			KeyParameter parameters = new KeyParameter(Rfc6637Utilities.CreateKey(pubKey.PublicKeyPacket, secret));
			IWrapper wrapper = PgpUtilities.CreateWrapper(ecPubKey.SymmetricKeyAlgorithm);
			wrapper.Init(forWrapping: true, new ParametersWithRandom(parameters, random));
			byte[] array = PgpPad.PadSessionData(sessionInfo, sessionKeyObfuscation);
			byte[] array2 = wrapper.Wrap(array, 0, array.Length);
			byte[] encoded = new MPInteger(new BigInteger(1, ephPubEncoding)).GetEncoded();
			byte[] array3 = new byte[encoded.Length + 1 + array2.Length];
			Array.Copy(encoded, 0, array3, 0, encoded.Length);
			array3[encoded.Length] = (byte)array2.Length;
			Array.Copy(array2, 0, array3, encoded.Length + 1, array2.Length);
			return array3;
		}

		private byte[][] ProcessSessionInfo(byte[] encryptedSessionInfo)
		{
			switch (pubKey.Algorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
				return new byte[1][] { ConvertToEncodedMpi(encryptedSessionInfo) };
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
			{
				int num = encryptedSessionInfo.Length / 2;
				byte[] array = new byte[num];
				byte[] array2 = new byte[num];
				Array.Copy(encryptedSessionInfo, 0, array, 0, num);
				Array.Copy(encryptedSessionInfo, num, array2, 0, num);
				return new byte[2][]
				{
					ConvertToEncodedMpi(array),
					ConvertToEncodedMpi(array2)
				};
			}
			case PublicKeyAlgorithmTag.ECDH:
				return new byte[1][] { encryptedSessionInfo };
			default:
				throw new PgpException("unknown asymmetric algorithm: " + pubKey.Algorithm);
			}
		}

		private byte[] ConvertToEncodedMpi(byte[] encryptedSessionInfo)
		{
			try
			{
				return new MPInteger(new BigInteger(1, encryptedSessionInfo)).GetEncoded();
			}
			catch (IOException ex)
			{
				throw new PgpException("Invalid MPI encoding: " + ex.Message, ex);
			}
		}

		public override void Encode(BcpgOutputStream pOut)
		{
			PublicKeyEncSessionPacket p = new PublicKeyEncSessionPacket(pubKey.KeyId, pubKey.Algorithm, data);
			pOut.WritePacket(p);
		}
	}

	private BcpgOutputStream pOut;

	private CipherStream cOut;

	private IBufferedCipher c;

	private bool withIntegrityPacket;

	private bool oldFormat;

	private DigestStream digestOut;

	private readonly List<EncMethod> methods = new List<EncMethod>();

	private readonly SymmetricKeyAlgorithmTag defAlgorithm;

	private readonly SecureRandom rand;

	public PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag encAlgorithm)
	{
		defAlgorithm = encAlgorithm;
		rand = CryptoServicesRegistrar.GetSecureRandom();
	}

	public PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag encAlgorithm, bool withIntegrityPacket)
	{
		defAlgorithm = encAlgorithm;
		this.withIntegrityPacket = withIntegrityPacket;
		rand = CryptoServicesRegistrar.GetSecureRandom();
	}

	public PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag encAlgorithm, SecureRandom random)
	{
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		defAlgorithm = encAlgorithm;
		rand = random;
	}

	public PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag encAlgorithm, bool withIntegrityPacket, SecureRandom random)
	{
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		defAlgorithm = encAlgorithm;
		rand = random;
		this.withIntegrityPacket = withIntegrityPacket;
	}

	public PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag encAlgorithm, SecureRandom random, bool oldFormat)
	{
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		defAlgorithm = encAlgorithm;
		rand = random;
		this.oldFormat = oldFormat;
	}

	public void AddMethod(char[] passPhrase, HashAlgorithmTag s2kDigest)
	{
		DoAddMethod(PgpUtilities.EncodePassPhrase(passPhrase, utf8: false), clearPassPhrase: true, s2kDigest);
	}

	public void AddMethodUtf8(char[] passPhrase, HashAlgorithmTag s2kDigest)
	{
		DoAddMethod(PgpUtilities.EncodePassPhrase(passPhrase, utf8: true), clearPassPhrase: true, s2kDigest);
	}

	public void AddMethodRaw(byte[] rawPassPhrase, HashAlgorithmTag s2kDigest)
	{
		DoAddMethod(rawPassPhrase, clearPassPhrase: false, s2kDigest);
	}

	internal void DoAddMethod(byte[] rawPassPhrase, bool clearPassPhrase, HashAlgorithmTag s2kDigest)
	{
		S2k s2k = PgpUtilities.GenerateS2k(s2kDigest, 96, rand);
		methods.Add(new PbeMethod(defAlgorithm, s2k, PgpUtilities.DoMakeKeyFromPassPhrase(defAlgorithm, s2k, rawPassPhrase, clearPassPhrase)));
	}

	public void AddMethod(PgpPublicKey key)
	{
		AddMethod(key, sessionKeyObfuscation: true);
	}

	public void AddMethod(PgpPublicKey key, bool sessionKeyObfuscation)
	{
		if (!key.IsEncryptionKey)
		{
			throw new ArgumentException("passed in key not an encryption key!");
		}
		methods.Add(new PubMethod(key, sessionKeyObfuscation));
	}

	private void AddCheckSum(byte[] sessionInfo)
	{
		int num = 0;
		for (int i = 1; i < sessionInfo.Length - 2; i++)
		{
			num += sessionInfo[i];
		}
		sessionInfo[^2] = (byte)(num >> 8);
		sessionInfo[^1] = (byte)num;
	}

	private byte[] CreateSessionInfo(SymmetricKeyAlgorithmTag algorithm, KeyParameter key)
	{
		int keyLength = key.KeyLength;
		byte[] array = new byte[keyLength + 3];
		array[0] = (byte)algorithm;
		key.CopyTo(array, 1, keyLength);
		AddCheckSum(array);
		return array;
	}

	private Stream Open(Stream outStr, long length, byte[] buffer)
	{
		if (cOut != null)
		{
			throw new InvalidOperationException("generator already in open state");
		}
		if (methods.Count == 0)
		{
			throw new InvalidOperationException("No encryption methods specified");
		}
		if (outStr == null)
		{
			throw new ArgumentNullException("outStr");
		}
		pOut = new BcpgOutputStream(outStr);
		KeyParameter keyParameter;
		if (methods.Count == 1)
		{
			if (methods[0] is PbeMethod pbeMethod)
			{
				keyParameter = pbeMethod.GetKey();
			}
			else
			{
				if (!(methods[0] is PubMethod pubMethod))
				{
					throw new InvalidOperationException();
				}
				keyParameter = PgpUtilities.MakeRandomKey(defAlgorithm, rand);
				byte[] si = CreateSessionInfo(defAlgorithm, keyParameter);
				try
				{
					pubMethod.AddSessionInfo(si, rand);
				}
				catch (Exception innerException)
				{
					throw new PgpException("exception encrypting session key", innerException);
				}
			}
			pOut.WritePacket(methods[0]);
		}
		else
		{
			keyParameter = PgpUtilities.MakeRandomKey(defAlgorithm, rand);
			byte[] si2 = CreateSessionInfo(defAlgorithm, keyParameter);
			foreach (EncMethod method in methods)
			{
				try
				{
					method.AddSessionInfo(si2, rand);
				}
				catch (Exception innerException2)
				{
					throw new PgpException("exception encrypting session key", innerException2);
				}
				pOut.WritePacket(method);
			}
		}
		string symmetricCipherName = PgpUtilities.GetSymmetricCipherName(defAlgorithm);
		if (symmetricCipherName == null)
		{
			throw new PgpException("null cipher specified");
		}
		try
		{
			symmetricCipherName = ((!withIntegrityPacket) ? (symmetricCipherName + "/OpenPGPCFB/NoPadding") : (symmetricCipherName + "/CFB/NoPadding"));
			c = CipherUtilities.GetCipher(symmetricCipherName);
			byte[] iv = new byte[c.GetBlockSize()];
			c.Init(forEncryption: true, new ParametersWithRandom(new ParametersWithIV(keyParameter, iv), rand));
			if (buffer == null)
			{
				if (withIntegrityPacket)
				{
					pOut = new BcpgOutputStream(outStr, PacketTag.SymmetricEncryptedIntegrityProtected, length + c.GetBlockSize() + 2 + 1 + 22);
					pOut.WriteByte(1);
				}
				else
				{
					pOut = new BcpgOutputStream(outStr, PacketTag.SymmetricKeyEncrypted, length + c.GetBlockSize() + 2, oldFormat);
				}
			}
			else if (withIntegrityPacket)
			{
				pOut = new BcpgOutputStream(outStr, PacketTag.SymmetricEncryptedIntegrityProtected, buffer);
				pOut.WriteByte(1);
			}
			else
			{
				pOut = new BcpgOutputStream(outStr, PacketTag.SymmetricKeyEncrypted, buffer);
			}
			int blockSize = c.GetBlockSize();
			byte[] array = new byte[blockSize + 2];
			rand.NextBytes(array, 0, blockSize);
			Array.Copy(array, array.Length - 4, array, array.Length - 2, 2);
			Stream stream = (cOut = new CipherStream(pOut, null, c));
			if (withIntegrityPacket)
			{
				IDigest writeDigest = PgpUtilities.CreateDigest(HashAlgorithmTag.Sha1);
				stream = (digestOut = new DigestStream(stream, null, writeDigest));
			}
			stream.Write(array, 0, array.Length);
			return new WrappedGeneratorStream(this, stream);
		}
		catch (Exception innerException3)
		{
			throw new PgpException("Exception creating cipher", innerException3);
		}
	}

	public Stream Open(Stream outStr, long length)
	{
		return Open(outStr, length, null);
	}

	public Stream Open(Stream outStr, byte[] buffer)
	{
		return Open(outStr, 0L, buffer);
	}

	[Obsolete("Dispose any opened Stream directly")]
	public void Close()
	{
		if (cOut != null)
		{
			if (digestOut != null)
			{
				new BcpgOutputStream(digestOut, PacketTag.ModificationDetectionCode, 20L).Flush();
				digestOut.Flush();
				byte[] array = DigestUtilities.DoFinal(digestOut.WriteDigest);
				cOut.Write(array, 0, array.Length);
			}
			cOut.Flush();
			try
			{
				pOut.Write(c.DoFinal());
				pOut.Finish();
			}
			catch (Exception ex)
			{
				throw new IOException(ex.Message, ex);
			}
			cOut = null;
			pOut = null;
		}
	}
}
