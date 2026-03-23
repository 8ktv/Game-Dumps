using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class RsaBlindingFactorGenerator
{
	private RsaKeyParameters key;

	private SecureRandom random;

	public void Init(ICipherParameters param)
	{
		if (param is ParametersWithRandom parametersWithRandom)
		{
			key = (RsaKeyParameters)parametersWithRandom.Parameters;
			random = parametersWithRandom.Random;
		}
		else
		{
			key = (RsaKeyParameters)param;
			random = CryptoServicesRegistrar.GetSecureRandom();
		}
		if (key.IsPrivate)
		{
			throw new ArgumentException("generator requires RSA public key");
		}
	}

	public BigInteger GenerateBlindingFactor()
	{
		if (key == null)
		{
			throw new InvalidOperationException("generator not initialised");
		}
		BigInteger modulus = key.Modulus;
		int bitLength = modulus.BitLength - 1;
		BigInteger bigInteger;
		do
		{
			bigInteger = BigIntegers.CreateRandomBigInteger(bitLength, random);
		}
		while (bigInteger.CompareTo(BigInteger.Two) < 0 || !BigIntegers.ModOddIsCoprime(modulus, bigInteger));
		return bigInteger;
	}
}
