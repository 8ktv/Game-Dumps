using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Parameters;

public abstract class ECKeyParameters : AsymmetricKeyParameter
{
	private static readonly Dictionary<string, string> Algorithms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "EC", "EC" },
		{ "ECDSA", "ECDSA" },
		{ "ECDH", "ECDH" },
		{ "ECDHC", "ECDHC" },
		{ "ECGOST3410", "ECGOST3410" },
		{ "ECMQV", "ECMQV" }
	};

	private readonly string algorithm;

	private readonly ECDomainParameters parameters;

	private readonly DerObjectIdentifier publicKeyParamSet;

	public string AlgorithmName => algorithm;

	public ECDomainParameters Parameters => parameters;

	public DerObjectIdentifier PublicKeyParamSet => publicKeyParamSet;

	protected ECKeyParameters(string algorithm, bool isPrivate, ECDomainParameters parameters)
		: base(isPrivate)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		this.algorithm = VerifyAlgorithmName(algorithm);
		this.parameters = parameters;
		publicKeyParamSet = (parameters as ECNamedDomainParameters)?.Name;
	}

	protected ECKeyParameters(string algorithm, bool isPrivate, DerObjectIdentifier publicKeyParamSet)
		: base(isPrivate)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (publicKeyParamSet == null)
		{
			throw new ArgumentNullException("publicKeyParamSet");
		}
		this.algorithm = VerifyAlgorithmName(algorithm);
		parameters = LookupParameters(publicKeyParamSet);
		this.publicKeyParamSet = publicKeyParamSet;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is ECDomainParameters obj2))
		{
			return false;
		}
		return Equals(obj2);
	}

	protected bool Equals(ECKeyParameters other)
	{
		if (parameters.Equals(other.parameters))
		{
			return Equals((AsymmetricKeyParameter)other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return parameters.GetHashCode() ^ base.GetHashCode();
	}

	internal ECKeyGenerationParameters CreateKeyGenerationParameters(SecureRandom random)
	{
		if (publicKeyParamSet != null)
		{
			return new ECKeyGenerationParameters(publicKeyParamSet, random);
		}
		return new ECKeyGenerationParameters(parameters, random);
	}

	internal static string VerifyAlgorithmName(string algorithm)
	{
		if (!Algorithms.TryGetValue(algorithm, out var value))
		{
			throw new ArgumentException("unrecognised algorithm: " + algorithm, "algorithm");
		}
		return value;
	}

	internal static ECDomainParameters LookupParameters(DerObjectIdentifier publicKeyParamSet)
	{
		if (publicKeyParamSet == null)
		{
			throw new ArgumentNullException("publicKeyParamSet");
		}
		return new ECDomainParameters(ECKeyPairGenerator.FindECCurveByOid(publicKeyParamSet) ?? throw new ArgumentException("OID is not a valid public key parameter set", "publicKeyParamSet"));
	}
}
