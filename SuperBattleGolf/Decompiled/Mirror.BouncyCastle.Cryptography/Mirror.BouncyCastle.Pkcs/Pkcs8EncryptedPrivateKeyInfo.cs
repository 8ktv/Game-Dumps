using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Pkcs;

public class Pkcs8EncryptedPrivateKeyInfo
{
	private EncryptedPrivateKeyInfo encryptedPrivateKeyInfo;

	private static EncryptedPrivateKeyInfo parseBytes(byte[] pkcs8Encoding)
	{
		try
		{
			return EncryptedPrivateKeyInfo.GetInstance(pkcs8Encoding);
		}
		catch (ArgumentException ex)
		{
			throw new PkcsIOException("malformed data: " + ex.Message, ex);
		}
		catch (Exception ex2)
		{
			throw new PkcsIOException("malformed data: " + ex2.Message, ex2);
		}
	}

	public Pkcs8EncryptedPrivateKeyInfo(EncryptedPrivateKeyInfo encryptedPrivateKeyInfo)
	{
		this.encryptedPrivateKeyInfo = encryptedPrivateKeyInfo;
	}

	public Pkcs8EncryptedPrivateKeyInfo(byte[] encryptedPrivateKeyInfo)
		: this(parseBytes(encryptedPrivateKeyInfo))
	{
	}

	public EncryptedPrivateKeyInfo ToAsn1Structure()
	{
		return encryptedPrivateKeyInfo;
	}

	public byte[] GetEncryptedData()
	{
		return encryptedPrivateKeyInfo.GetEncryptedData();
	}

	public byte[] GetEncoded()
	{
		return encryptedPrivateKeyInfo.GetEncoded();
	}

	public PrivateKeyInfo DecryptPrivateKeyInfo(IDecryptorBuilderProvider inputDecryptorProvider)
	{
		try
		{
			ICipher cipher = inputDecryptorProvider.CreateDecryptorBuilder(encryptedPrivateKeyInfo.EncryptionAlgorithm).BuildCipher(new MemoryStream(encryptedPrivateKeyInfo.GetEncryptedData(), writable: false));
			byte[] obj;
			using (cipher.Stream)
			{
				obj = Streams.ReadAll(cipher.Stream);
			}
			return PrivateKeyInfo.GetInstance(obj);
		}
		catch (Exception ex)
		{
			throw new PkcsException("unable to read encrypted data: " + ex.Message, ex);
		}
	}
}
