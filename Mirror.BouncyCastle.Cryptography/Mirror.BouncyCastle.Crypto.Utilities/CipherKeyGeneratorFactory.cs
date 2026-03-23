using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Kisa;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Ntt;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public static class CipherKeyGeneratorFactory
{
	public static CipherKeyGenerator CreateKeyGenerator(DerObjectIdentifier algorithm, SecureRandom random)
	{
		if (NistObjectIdentifiers.IdAes128Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		if (NistObjectIdentifiers.IdAes192Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 192);
		}
		if (NistObjectIdentifiers.IdAes256Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 256);
		}
		if (PkcsObjectIdentifiers.DesEde3Cbc.Equals(algorithm))
		{
			DesEdeKeyGenerator desEdeKeyGenerator = new DesEdeKeyGenerator();
			desEdeKeyGenerator.Init(new KeyGenerationParameters(random, 192));
			return desEdeKeyGenerator;
		}
		if (NttObjectIdentifiers.IdCamellia128Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		if (NttObjectIdentifiers.IdCamellia192Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 192);
		}
		if (NttObjectIdentifiers.IdCamellia256Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 256);
		}
		if (KisaObjectIdentifiers.IdSeedCbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		if (AlgorithmIdentifierFactory.CAST5_CBC.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		if (OiwObjectIdentifiers.DesCbc.Equals(algorithm))
		{
			DesKeyGenerator desKeyGenerator = new DesKeyGenerator();
			desKeyGenerator.Init(new KeyGenerationParameters(random, 64));
			return desKeyGenerator;
		}
		if (PkcsObjectIdentifiers.rc4.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		if (PkcsObjectIdentifiers.RC2Cbc.Equals(algorithm))
		{
			return CreateCipherKeyGenerator(random, 128);
		}
		throw new InvalidOperationException("cannot recognise cipher: " + algorithm);
	}

	private static CipherKeyGenerator CreateCipherKeyGenerator(SecureRandom random, int keySize)
	{
		CipherKeyGenerator cipherKeyGenerator = new CipherKeyGenerator();
		cipherKeyGenerator.Init(new KeyGenerationParameters(random, keySize));
		return cipherKeyGenerator;
	}
}
