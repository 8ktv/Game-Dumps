using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Crypto.Agreement;

public class ECDHBasicAgreement : IBasicAgreement
{
	protected internal ECPrivateKeyParameters privKey;

	public virtual void Init(ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		if (!(parameters is ECPrivateKeyParameters eCPrivateKeyParameters))
		{
			throw new ArgumentException("ECDHBasicAgreement expects ECPrivateKeyParameters");
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
			throw new InvalidOperationException("ECDH public key has wrong domain parameters");
		}
		BigInteger bigInteger = privKey.D;
		ECPoint eCPoint = ECAlgorithms.CleanPoint(parameters.Curve, eCPublicKeyParameters.Q);
		if (eCPoint.IsInfinity)
		{
			throw new InvalidOperationException("Infinity is not a valid public key for ECDH");
		}
		BigInteger h = parameters.H;
		if (!h.Equals(BigInteger.One))
		{
			bigInteger = parameters.HInv.Multiply(bigInteger).Mod(parameters.N);
			eCPoint = ECAlgorithms.ReferenceMultiply(eCPoint, h);
		}
		ECPoint eCPoint2 = eCPoint.Multiply(bigInteger).Normalize();
		if (eCPoint2.IsInfinity)
		{
			throw new InvalidOperationException("Infinity is not a valid agreement value for ECDH");
		}
		return eCPoint2.AffineXCoord.ToBigInteger();
	}
}
