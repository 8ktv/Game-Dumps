using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class RsaBlindedEngine : IAsymmetricBlockCipher
{
	private readonly IRsa core;

	private RsaKeyParameters key;

	private SecureRandom random;

	public virtual string AlgorithmName => "RSA";

	public RsaBlindedEngine()
		: this(new RsaCoreEngine())
	{
	}

	public RsaBlindedEngine(IRsa rsa)
	{
		core = rsa;
	}

	public virtual void Init(bool forEncryption, ICipherParameters param)
	{
		SecureRandom provided = null;
		if (param is ParametersWithRandom parametersWithRandom)
		{
			provided = parametersWithRandom.Random;
			param = parametersWithRandom.Parameters;
		}
		core.Init(forEncryption, param);
		key = (RsaKeyParameters)param;
		random = InitSecureRandom(key is RsaPrivateCrtKeyParameters, provided);
	}

	public virtual int GetInputBlockSize()
	{
		return core.GetInputBlockSize();
	}

	public virtual int GetOutputBlockSize()
	{
		return core.GetOutputBlockSize();
	}

	public virtual byte[] ProcessBlock(byte[] inBuf, int inOff, int inLen)
	{
		if (key == null)
		{
			throw new InvalidOperationException("RSA engine not initialised");
		}
		BigInteger input = core.ConvertInput(inBuf, inOff, inLen);
		BigInteger result = ProcessInput(input);
		return core.ConvertOutput(result);
	}

	protected virtual SecureRandom InitSecureRandom(bool needed, SecureRandom provided)
	{
		if (!needed)
		{
			return null;
		}
		return CryptoServicesRegistrar.GetSecureRandom(provided);
	}

	private BigInteger ProcessInput(BigInteger input)
	{
		if (!(key is RsaPrivateCrtKeyParameters { PublicExponent: var publicExponent, Modulus: var modulus }))
		{
			return core.ProcessBlock(input);
		}
		BigInteger bigInteger = BigIntegers.CreateRandomInRange(BigInteger.One, modulus.Subtract(BigInteger.One), random);
		BigInteger bigInteger2 = bigInteger.ModPow(publicExponent, modulus);
		BigInteger bigInteger3 = BigIntegers.ModOddInverse(modulus, bigInteger);
		BigInteger input2 = bigInteger2.Multiply(input).Mod(modulus);
		BigInteger val = core.ProcessBlock(input2);
		return bigInteger3.Multiply(val).Mod(modulus);
	}
}
