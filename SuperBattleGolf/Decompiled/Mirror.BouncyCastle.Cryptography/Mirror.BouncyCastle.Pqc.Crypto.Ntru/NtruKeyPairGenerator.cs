using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.Owcpa;
using Mirror.BouncyCastle.Pqc.Crypto.Ntru.ParameterSets;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

public class NtruKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private NtruKeyGenerationParameters _keygenParameters;

	private SecureRandom _random;

	public void Init(KeyGenerationParameters parameters)
	{
		_keygenParameters = (NtruKeyGenerationParameters)parameters;
		_random = parameters.Random;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		NtruParameterSet parameterSet = _keygenParameters.NtruParameters.ParameterSet;
		byte[] array = new byte[parameterSet.SampleFgBytes()];
		_random.NextBytes(array);
		OwcpaKeyPair owcpaKeyPair = new NtruOwcpa(parameterSet).KeyPair(array);
		byte[] publicKey = owcpaKeyPair.PublicKey;
		byte[] array2 = new byte[parameterSet.NtruSecretKeyBytes()];
		byte[] privateKey = owcpaKeyPair.PrivateKey;
		Array.Copy(privateKey, 0, array2, 0, privateKey.Length);
		byte[] array3 = new byte[parameterSet.PrfKeyBytes];
		_random.NextBytes(array3);
		Array.Copy(array3, 0, array2, parameterSet.OwcpaSecretKeyBytes(), array3.Length);
		return new AsymmetricCipherKeyPair(new NtruPublicKeyParameters(_keygenParameters.NtruParameters, publicKey), new NtruPrivateKeyParameters(_keygenParameters.NtruParameters, array2));
	}
}
