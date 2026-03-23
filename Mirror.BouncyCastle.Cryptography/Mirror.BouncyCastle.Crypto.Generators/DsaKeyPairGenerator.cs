using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class DsaKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private static readonly BigInteger One = BigInteger.One;

	private DsaKeyGenerationParameters param;

	public void Init(KeyGenerationParameters parameters)
	{
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		param = (DsaKeyGenerationParameters)parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		DsaParameters parameters = param.Parameters;
		BigInteger x = GeneratePrivateKey(parameters.Q, param.Random);
		return new AsymmetricCipherKeyPair(new DsaPublicKeyParameters(CalculatePublicKey(parameters.P, parameters.G, x), parameters), new DsaPrivateKeyParameters(x, parameters));
	}

	private static BigInteger GeneratePrivateKey(BigInteger q, SecureRandom random)
	{
		int num = q.BitLength >> 2;
		BigInteger bigInteger;
		do
		{
			bigInteger = BigIntegers.CreateRandomInRange(One, q.Subtract(One), random);
		}
		while (WNafUtilities.GetNafWeight(bigInteger) < num);
		return bigInteger;
	}

	private static BigInteger CalculatePublicKey(BigInteger p, BigInteger g, BigInteger x)
	{
		return g.ModPow(x, p);
	}
}
