using System;
using System.IO;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pkcs;

public class Pkcs8EncryptedPrivateKeyInfoBuilder
{
	private PrivateKeyInfo privateKeyInfo;

	public Pkcs8EncryptedPrivateKeyInfoBuilder(byte[] privateKeyInfo)
		: this(PrivateKeyInfo.GetInstance(privateKeyInfo))
	{
	}

	public Pkcs8EncryptedPrivateKeyInfoBuilder(PrivateKeyInfo privateKeyInfo)
	{
		this.privateKeyInfo = privateKeyInfo;
	}

	public Pkcs8EncryptedPrivateKeyInfo Build(ICipherBuilder encryptor)
	{
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			using (Stream output = encryptor.BuildCipher(memoryStream).Stream)
			{
				privateKeyInfo.EncodeTo(output);
			}
			return new Pkcs8EncryptedPrivateKeyInfo(new EncryptedPrivateKeyInfo((AlgorithmIdentifier)encryptor.AlgorithmDetails, memoryStream.ToArray()));
		}
		catch (IOException)
		{
			throw new InvalidOperationException("cannot encode privateKeyInfo");
		}
	}
}
