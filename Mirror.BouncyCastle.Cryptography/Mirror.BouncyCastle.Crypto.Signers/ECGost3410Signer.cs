using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class ECGost3410Signer : IDsa
{
	private ECKeyParameters key;

	private SecureRandom random;

	private bool forSigning;

	public virtual string AlgorithmName => key.AlgorithmName;

	public virtual BigInteger Order => key.Parameters.N;

	public virtual void Init(bool forSigning, ICipherParameters parameters)
	{
		this.forSigning = forSigning;
		if (forSigning)
		{
			if (parameters is ParametersWithRandom parametersWithRandom)
			{
				random = parametersWithRandom.Random;
				parameters = parametersWithRandom.Parameters;
			}
			else
			{
				random = CryptoServicesRegistrar.GetSecureRandom();
			}
			if (!(parameters is ECPrivateKeyParameters eCPrivateKeyParameters))
			{
				throw new InvalidKeyException("EC private key required for signing");
			}
			key = eCPrivateKeyParameters;
		}
		else
		{
			if (!(parameters is ECPublicKeyParameters eCPublicKeyParameters))
			{
				throw new InvalidKeyException("EC public key required for verification");
			}
			key = eCPublicKeyParameters;
		}
	}

	public virtual BigInteger[] GenerateSignature(byte[] message)
	{
		if (!forSigning)
		{
			throw new InvalidOperationException("not initialized for signing");
		}
		BigInteger val = new BigInteger(1, message, bigEndian: false);
		ECDomainParameters parameters = key.Parameters;
		BigInteger n = parameters.N;
		BigInteger d = ((ECPrivateKeyParameters)key).D;
		ECMultiplier eCMultiplier = CreateBasePointMultiplier();
		BigInteger bigInteger2;
		BigInteger bigInteger3;
		while (true)
		{
			BigInteger bigInteger = BigIntegers.CreateRandomBigInteger(n.BitLength, random);
			if (bigInteger.SignValue == 0)
			{
				continue;
			}
			bigInteger2 = eCMultiplier.Multiply(parameters.G, bigInteger).Normalize().AffineXCoord.ToBigInteger().Mod(n);
			if (bigInteger2.SignValue != 0)
			{
				bigInteger3 = bigInteger.Multiply(val).Add(d.Multiply(bigInteger2)).Mod(n);
				if (bigInteger3.SignValue != 0)
				{
					break;
				}
			}
		}
		return new BigInteger[2] { bigInteger2, bigInteger3 };
	}

	public virtual bool VerifySignature(byte[] message, BigInteger r, BigInteger s)
	{
		if (forSigning)
		{
			throw new InvalidOperationException("not initialized for verification");
		}
		BigInteger x = new BigInteger(1, message, bigEndian: false);
		BigInteger n = key.Parameters.N;
		if (r.CompareTo(BigInteger.One) < 0 || r.CompareTo(n) >= 0)
		{
			return false;
		}
		if (s.CompareTo(BigInteger.One) < 0 || s.CompareTo(n) >= 0)
		{
			return false;
		}
		BigInteger val = BigIntegers.ModOddInverseVar(n, x);
		BigInteger a = s.Multiply(val).Mod(n);
		BigInteger b = n.Subtract(r).Multiply(val).Mod(n);
		ECPoint g = key.Parameters.G;
		ECPoint q = ((ECPublicKeyParameters)key).Q;
		ECPoint eCPoint = ECAlgorithms.SumOfTwoMultiplies(g, a, q, b).Normalize();
		if (eCPoint.IsInfinity)
		{
			return false;
		}
		return eCPoint.AffineXCoord.ToBigInteger().Mod(n).Equals(r);
	}

	protected virtual ECMultiplier CreateBasePointMultiplier()
	{
		return new FixedPointCombMultiplier();
	}
}
