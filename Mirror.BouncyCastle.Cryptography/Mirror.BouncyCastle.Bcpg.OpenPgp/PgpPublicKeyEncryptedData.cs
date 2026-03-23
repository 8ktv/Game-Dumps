using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cryptlib;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpPublicKeyEncryptedData : PgpEncryptedData
{
	private PublicKeyEncSessionPacket keyData;

	public long KeyId => keyData.KeyId;

	internal PgpPublicKeyEncryptedData(PublicKeyEncSessionPacket keyData, InputStreamPacket encData)
		: base(encData)
	{
		this.keyData = keyData;
	}

	private static IBufferedCipher GetKeyCipher(PublicKeyAlgorithmTag algorithm)
	{
		try
		{
			switch (algorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
				return CipherUtilities.GetCipher("RSA//PKCS1Padding");
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
				return CipherUtilities.GetCipher("ElGamal/ECB/PKCS1Padding");
			default:
				throw new PgpException("unknown asymmetric algorithm: " + algorithm);
			}
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new PgpException("Exception creating cipher", innerException);
		}
	}

	private bool ConfirmCheckSum(byte[] sessionInfo)
	{
		int num = 0;
		for (int i = 1; i != sessionInfo.Length - 2; i++)
		{
			num += sessionInfo[i] & 0xFF;
		}
		if (sessionInfo[^2] == (byte)(num >> 8))
		{
			return sessionInfo[^1] == (byte)num;
		}
		return false;
	}

	public SymmetricKeyAlgorithmTag GetSymmetricAlgorithm(PgpPrivateKey privKey)
	{
		return (SymmetricKeyAlgorithmTag)RecoverSessionData(privKey)[0];
	}

	public Stream GetDataStream(PgpPrivateKey privKey)
	{
		byte[] array = RecoverSessionData(privKey);
		if (!ConfirmCheckSum(array))
		{
			throw new PgpKeyValidationException("key checksum failed");
		}
		SymmetricKeyAlgorithmTag symmetricKeyAlgorithmTag = (SymmetricKeyAlgorithmTag)array[0];
		if (symmetricKeyAlgorithmTag == SymmetricKeyAlgorithmTag.Null)
		{
			return encData.GetInputStream();
		}
		string symmetricCipherName = PgpUtilities.GetSymmetricCipherName(symmetricKeyAlgorithmTag);
		string text = symmetricCipherName;
		IBufferedCipher cipher;
		try
		{
			text = ((!(encData is SymmetricEncIntegrityPacket)) ? (text + "/OpenPGPCFB/NoPadding") : (text + "/CFB/NoPadding"));
			cipher = CipherUtilities.GetCipher(text);
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new PgpException("exception creating cipher", innerException);
		}
		try
		{
			KeyParameter parameters = ParameterUtilities.CreateKeyParameter(symmetricCipherName, array, 1, array.Length - 3);
			byte[] array2 = new byte[cipher.GetBlockSize()];
			cipher.Init(forEncryption: false, new ParametersWithIV(parameters, array2));
			encStream = BcpgInputStream.Wrap(new CipherStream(encData.GetInputStream(), cipher, null));
			if (encData is SymmetricEncIntegrityPacket)
			{
				truncStream = new TruncatedStream(encStream);
				IDigest readDigest = PgpUtilities.CreateDigest(HashAlgorithmTag.Sha1);
				encStream = new DigestStream(truncStream, readDigest, null);
			}
			if (Streams.ReadFully(encStream, array2, 0, array2.Length) < array2.Length)
			{
				throw new EndOfStreamException("unexpected end of stream.");
			}
			int num = encStream.ReadByte();
			int num2 = encStream.ReadByte();
			if (num < 0 || num2 < 0)
			{
				throw new EndOfStreamException("unexpected end of stream.");
			}
			return encStream;
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException2)
		{
			throw new PgpException("Exception starting decryption", innerException2);
		}
	}

	private byte[] RecoverSessionData(PgpPrivateKey privKey)
	{
		byte[][] encSessionKey = keyData.GetEncSessionKey();
		if (keyData.Algorithm != PublicKeyAlgorithmTag.ECDH)
		{
			IBufferedCipher keyCipher = GetKeyCipher(keyData.Algorithm);
			try
			{
				keyCipher.Init(forEncryption: false, privKey.Key);
			}
			catch (InvalidKeyException innerException)
			{
				throw new PgpException("error setting asymmetric cipher", innerException);
			}
			if (keyData.Algorithm == PublicKeyAlgorithmTag.RsaEncrypt || keyData.Algorithm == PublicKeyAlgorithmTag.RsaGeneral)
			{
				byte[] array = encSessionKey[0];
				keyCipher.ProcessBytes(array, 2, array.Length - 2);
			}
			else
			{
				int size = (((ElGamalPrivateKeyParameters)privKey.Key).Parameters.P.BitLength + 7) / 8;
				ProcessEncodedMpi(keyCipher, size, encSessionKey[0]);
				ProcessEncodedMpi(keyCipher, size, encSessionKey[1]);
			}
			try
			{
				return keyCipher.DoFinal();
			}
			catch (Exception innerException2)
			{
				throw new PgpException("exception decrypting secret key", innerException2);
			}
		}
		ECDHPublicBcpgKey obj = (ECDHPublicBcpgKey)privKey.PublicKeyPacket.Key;
		byte[] array2 = encSessionKey[0];
		int num = (((array2[0] & 0xFF) << 8) + (array2[1] & 0xFF) + 7) / 8;
		if (2 + num + 1 > array2.Length)
		{
			throw new PgpException("encoded length out of range");
		}
		byte[] array3 = new byte[num];
		Array.Copy(array2, 2, array3, 0, num);
		int num2 = array2[num + 2];
		if (2 + num + 1 + num2 > array2.Length)
		{
			throw new PgpException("encoded length out of range");
		}
		byte[] array4 = new byte[num2];
		Array.Copy(array2, 2 + num + 1, array4, 0, array4.Length);
		DerObjectIdentifier curveOid = obj.CurveOid;
		byte[] array5;
		if (EdECObjectIdentifiers.id_X25519.Equals(curveOid) || CryptlibObjectIdentifiers.curvey25519.Equals(curveOid))
		{
			if (array3.Length != 1 + X25519PublicKeyParameters.KeySize || 64 != array3[0])
			{
				throw new ArgumentException("Invalid X25519 public key");
			}
			X25519PublicKeyParameters publicKey = new X25519PublicKeyParameters(array3, 1);
			X25519Agreement x25519Agreement = new X25519Agreement();
			x25519Agreement.Init(privKey.Key);
			array5 = new byte[x25519Agreement.AgreementSize];
			x25519Agreement.CalculateAgreement(publicKey, array5, 0);
		}
		else if (EdECObjectIdentifiers.id_X448.Equals(curveOid))
		{
			if (array3.Length != 1 + X448PublicKeyParameters.KeySize || 64 != array3[0])
			{
				throw new ArgumentException("Invalid X448 public key");
			}
			X448PublicKeyParameters publicKey2 = new X448PublicKeyParameters(array3, 1);
			X448Agreement x448Agreement = new X448Agreement();
			x448Agreement.Init(privKey.Key);
			array5 = new byte[x448Agreement.AgreementSize];
			x448Agreement.CalculateAgreement(publicKey2, array5, 0);
		}
		else
		{
			ECDomainParameters parameters = ((ECPrivateKeyParameters)privKey.Key).Parameters;
			ECPublicKeyParameters pubKey = new ECPublicKeyParameters(parameters.Curve.DecodePoint(array3), parameters);
			ECDHBasicAgreement eCDHBasicAgreement = new ECDHBasicAgreement();
			eCDHBasicAgreement.Init(privKey.Key);
			array5 = BigIntegers.AsUnsignedByteArray(n: eCDHBasicAgreement.CalculateAgreement(pubKey), length: eCDHBasicAgreement.GetFieldSize());
		}
		KeyParameter parameters2 = new KeyParameter(Rfc6637Utilities.CreateKey(privKey.PublicKeyPacket, array5));
		IWrapper wrapper = PgpUtilities.CreateWrapper(obj.SymmetricKeyAlgorithm);
		wrapper.Init(forWrapping: false, parameters2);
		return PgpPad.UnpadSessionData(wrapper.Unwrap(array4, 0, array4.Length));
	}

	private static void ProcessEncodedMpi(IBufferedCipher cipher, int size, byte[] mpiEnc)
	{
		if (mpiEnc.Length - 2 > size)
		{
			cipher.ProcessBytes(mpiEnc, 3, mpiEnc.Length - 3);
			return;
		}
		byte[] array = new byte[size];
		Array.Copy(mpiEnc, 2, array, array.Length - (mpiEnc.Length - 2), mpiEnc.Length - 2);
		cipher.ProcessBytes(array, 0, array.Length);
	}
}
