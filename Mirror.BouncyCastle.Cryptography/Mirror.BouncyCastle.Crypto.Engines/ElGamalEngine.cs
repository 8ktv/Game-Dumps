using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class ElGamalEngine : IAsymmetricBlockCipher
{
	private ElGamalKeyParameters key;

	private SecureRandom random;

	private bool forEncryption;

	private int bitSize;

	public virtual string AlgorithmName => "ElGamal";

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			key = (ElGamalKeyParameters)parametersWithRandom.Parameters;
			random = parametersWithRandom.Random;
		}
		else
		{
			key = (ElGamalKeyParameters)parameters;
			random = (forEncryption ? CryptoServicesRegistrar.GetSecureRandom() : null);
		}
		this.forEncryption = forEncryption;
		bitSize = key.Parameters.P.BitLength;
		if (forEncryption)
		{
			if (!(key is ElGamalPublicKeyParameters))
			{
				throw new ArgumentException("ElGamalPublicKeyParameters are required for encryption.");
			}
		}
		else if (!(key is ElGamalPrivateKeyParameters))
		{
			throw new ArgumentException("ElGamalPrivateKeyParameters are required for decryption.");
		}
	}

	public virtual int GetInputBlockSize()
	{
		if (forEncryption)
		{
			return (bitSize - 1) / 8;
		}
		return 2 * ((bitSize + 7) / 8);
	}

	public virtual int GetOutputBlockSize()
	{
		if (forEncryption)
		{
			return 2 * ((bitSize + 7) / 8);
		}
		return (bitSize - 1) / 8;
	}

	public virtual byte[] ProcessBlock(byte[] input, int inOff, int length)
	{
		if (key == null)
		{
			throw new InvalidOperationException("ElGamal engine not initialised");
		}
		int num = (forEncryption ? ((bitSize - 1 + 7) / 8) : GetInputBlockSize());
		if (length > num)
		{
			throw new DataLengthException("input too large for ElGamal cipher.\n");
		}
		BigInteger p = key.Parameters.P;
		byte[] array;
		if (key is ElGamalPrivateKeyParameters)
		{
			int num2 = length / 2;
			BigInteger bigInteger = new BigInteger(1, input, inOff, num2);
			BigInteger val = new BigInteger(1, input, inOff + num2, num2);
			ElGamalPrivateKeyParameters elGamalPrivateKeyParameters = (ElGamalPrivateKeyParameters)key;
			array = bigInteger.ModPow(p.Subtract(BigInteger.One).Subtract(elGamalPrivateKeyParameters.X), p).Multiply(val).Mod(p)
				.ToByteArrayUnsigned();
		}
		else
		{
			BigInteger bigInteger2 = new BigInteger(1, input, inOff, length);
			if (bigInteger2.BitLength >= p.BitLength)
			{
				throw new DataLengthException("input too large for ElGamal cipher.\n");
			}
			ElGamalPublicKeyParameters elGamalPublicKeyParameters = (ElGamalPublicKeyParameters)key;
			BigInteger other = p.Subtract(BigInteger.Two);
			BigInteger bigInteger3;
			do
			{
				bigInteger3 = new BigInteger(p.BitLength, random);
			}
			while (bigInteger3.SignValue == 0 || bigInteger3.CompareTo(other) > 0);
			BigInteger n = key.Parameters.G.ModPow(bigInteger3, p);
			BigInteger n2 = bigInteger2.Multiply(elGamalPublicKeyParameters.Y.ModPow(bigInteger3, p)).Mod(p);
			array = new byte[GetOutputBlockSize()];
			int num3 = array.Length / 2;
			BigIntegers.AsUnsignedByteArray(n, array, 0, num3);
			BigIntegers.AsUnsignedByteArray(n2, array, num3, array.Length - num3);
		}
		return array;
	}
}
