using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public class ECDHCBasicAgreement : IBasicAgreement
{
	private ECPrivateKeyParameters privKey;

	public virtual void Init(ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		if (!(parameters is ECPrivateKeyParameters eCPrivateKeyParameters))
		{
			throw new ArgumentException("ECDHCBasicAgreement expects ECPrivateKeyParameters");
		}
		privKey = eCPrivateKeyParameters;
	}

	public virtual int GetFieldSize()
	{
		return privKey.Parameters.Curve.FieldElementEncodingLength;
	}

	public virtual BigInteger CalculateAgreement(ICipherParameters pubKey)
	{
		ECPublicKeyParameters eCPublicKeyParameters = (ECPublicKeyParameters)pubKey;
		ECDomainParameters parameters = privKey.Parameters;
		if (!parameters.Equals(eCPublicKeyParameters.Parameters))
		{
			throw new InvalidOperationException("ECDHC public key has wrong domain parameters");
		}
		BigInteger b = parameters.H.Multiply(privKey.D).Mod(parameters.N);
		ECPoint eCPoint = ECAlgorithms.CleanPoint(parameters.Curve, eCPublicKeyParameters.Q);
		if (eCPoint.IsInfinity)
		{
			throw new InvalidOperationException("Infinity is not a valid public key for ECDHC");
		}
		ECPoint eCPoint2 = eCPoint.Multiply(b).Normalize();
		if (eCPoint2.IsInfinity)
		{
			throw new InvalidOperationException("Infinity is not a valid agreement value for ECDHC");
		}
		return eCPoint2.AffineXCoord.ToBigInteger();
	}
}
