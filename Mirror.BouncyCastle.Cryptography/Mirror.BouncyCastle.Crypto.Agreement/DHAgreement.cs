using System;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public class DHAgreement
{
	private DHPrivateKeyParameters key;

	private DHParameters dhParams;

	private BigInteger privateValue;

	private SecureRandom random;

	public void Init(ICipherParameters parameters)
	{
		AsymmetricKeyParameter asymmetricKeyParameter;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			random = parametersWithRandom.Random;
			asymmetricKeyParameter = (AsymmetricKeyParameter)parametersWithRandom.Parameters;
		}
		else
		{
			random = CryptoServicesRegistrar.GetSecureRandom();
			asymmetricKeyParameter = (AsymmetricKeyParameter)parameters;
		}
		if (!(asymmetricKeyParameter is DHPrivateKeyParameters dHPrivateKeyParameters))
		{
			throw new ArgumentException("DHEngine expects DHPrivateKeyParameters");
		}
		key = dHPrivateKeyParameters;
		dhParams = dHPrivateKeyParameters.Parameters;
	}

	public BigInteger CalculateMessage()
	{
		DHKeyPairGenerator dHKeyPairGenerator = new DHKeyPairGenerator();
		dHKeyPairGenerator.Init(new DHKeyGenerationParameters(random, dhParams));
		AsymmetricCipherKeyPair asymmetricCipherKeyPair = dHKeyPairGenerator.GenerateKeyPair();
		privateValue = ((DHPrivateKeyParameters)asymmetricCipherKeyPair.Private).X;
		return ((DHPublicKeyParameters)asymmetricCipherKeyPair.Public).Y;
	}

	public BigInteger CalculateAgreement(DHPublicKeyParameters pub, BigInteger message)
	{
		if (pub == null)
		{
			throw new ArgumentNullException("pub");
		}
		if (message == null)
		{
			throw new ArgumentNullException("message");
		}
		if (!pub.Parameters.Equals(dhParams))
		{
			throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
		}
		BigInteger p = dhParams.P;
		BigInteger y = pub.Y;
		if (y == null || y.CompareTo(BigInteger.One) <= 0 || y.CompareTo(p.Subtract(BigInteger.One)) >= 0)
		{
			throw new ArgumentException("Diffie-Hellman public key is weak");
		}
		BigInteger bigInteger = y.ModPow(privateValue, p);
		if (bigInteger.Equals(BigInteger.One))
		{
			throw new InvalidOperationException("Shared key can't be 1");
		}
		return message.ModPow(key.X, p).Multiply(bigInteger).Mod(p);
	}
}
