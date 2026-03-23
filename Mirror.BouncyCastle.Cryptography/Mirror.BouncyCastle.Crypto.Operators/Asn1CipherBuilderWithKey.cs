using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class Asn1CipherBuilderWithKey : ICipherBuilderWithKey, ICipherBuilder
{
	private readonly KeyParameter encKey;

	private AlgorithmIdentifier algorithmIdentifier;

	public object AlgorithmDetails => algorithmIdentifier;

	public ICipherParameters Key => encKey;

	public Asn1CipherBuilderWithKey(DerObjectIdentifier encryptionOID, int keySize, SecureRandom random)
	{
		random = CryptoServicesRegistrar.GetSecureRandom(random);
		CipherKeyGenerator cipherKeyGenerator = CipherKeyGeneratorFactory.CreateKeyGenerator(encryptionOID, random);
		encKey = cipherKeyGenerator.GenerateKeyParameter();
		algorithmIdentifier = AlgorithmIdentifierFactory.GenerateEncryptionAlgID(encryptionOID, encKey.KeyLength * 8, random);
	}

	public int GetMaxOutputSize(int inputLen)
	{
		throw new NotImplementedException();
	}

	public ICipher BuildCipher(Stream stream)
	{
		object obj = CipherFactory.CreateContentCipher(forEncryption: true, encKey, algorithmIdentifier);
		if (obj is IStreamCipher)
		{
			obj = new BufferedStreamCipher((IStreamCipher)obj);
		}
		if (stream == null)
		{
			stream = new MemoryStream();
		}
		return new BufferedCipherWrapper((IBufferedCipher)obj, stream);
	}
}
