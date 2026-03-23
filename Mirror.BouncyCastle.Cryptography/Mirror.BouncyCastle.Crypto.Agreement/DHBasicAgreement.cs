using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public class DHBasicAgreement : IBasicAgreement
{
	private DHPrivateKeyParameters key;

	private DHParameters dhParams;

	public virtual void Init(ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		if (!(parameters is DHPrivateKeyParameters dHPrivateKeyParameters))
		{
			throw new ArgumentException("DHBasicAgreement expects DHPrivateKeyParameters");
		}
		key = dHPrivateKeyParameters;
		dhParams = key.Parameters;
	}

	public virtual int GetFieldSize()
	{
		return (key.Parameters.P.BitLength + 7) / 8;
	}

	public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
	{
		if (key == null)
		{
			throw new InvalidOperationException("Agreement algorithm not initialised");
		}
		DHPublicKeyParameters obj = (DHPublicKeyParameters)pubKey;
		if (!obj.Parameters.Equals(dhParams))
		{
			throw new ArgumentException("Diffie-Hellman public key has wrong parameters.");
		}
		BigInteger p = dhParams.P;
		BigInteger y = obj.Y;
		if (y == null || y.CompareTo(BigInteger.One) <= 0 || y.CompareTo(p.Subtract(BigInteger.One)) >= 0)
		{
			throw new ArgumentException("Diffie-Hellman public key is weak");
		}
		BigInteger bigInteger = y.ModPow(key.X, p);
		if (bigInteger.Equals(BigInteger.One))
		{
			throw new InvalidOperationException("Shared key can't be 1");
		}
		return bigInteger;
	}
}
