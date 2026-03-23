using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class NaccacheSternEngine : IAsymmetricBlockCipher
{
	private bool forEncryption;

	private NaccacheSternKeyParameters key;

	private IList<BigInteger>[] lookup;

	public string AlgorithmName => "NaccacheStern";

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		this.forEncryption = forEncryption;
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		key = (NaccacheSternKeyParameters)parameters;
		if (this.forEncryption)
		{
			return;
		}
		NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)key;
		IList<BigInteger> smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
		lookup = new IList<BigInteger>[smallPrimesList.Count];
		for (int i = 0; i < smallPrimesList.Count; i++)
		{
			BigInteger bigInteger = smallPrimesList[i];
			int intValue = bigInteger.IntValue;
			lookup[i] = new List<BigInteger>(intValue);
			lookup[i].Add(BigInteger.One);
			BigInteger bigInteger2 = BigInteger.Zero;
			for (int j = 1; j < intValue; j++)
			{
				bigInteger2 = bigInteger2.Add(naccacheSternPrivateKeyParameters.PhiN);
				BigInteger e = bigInteger2.Divide(bigInteger);
				lookup[i].Add(naccacheSternPrivateKeyParameters.G.ModPow(e, naccacheSternPrivateKeyParameters.Modulus));
			}
		}
	}

	public virtual int GetInputBlockSize()
	{
		if (forEncryption)
		{
			return (key.LowerSigmaBound + 7) / 8 - 1;
		}
		return key.Modulus.BitLength / 8 + 1;
	}

	public virtual int GetOutputBlockSize()
	{
		if (forEncryption)
		{
			return key.Modulus.BitLength / 8 + 1;
		}
		return (key.LowerSigmaBound + 7) / 8 - 1;
	}

	public virtual byte[] ProcessBlock(byte[] inBytes, int inOff, int length)
	{
		if (key == null)
		{
			throw new InvalidOperationException("NaccacheStern engine not initialised");
		}
		if (length > GetInputBlockSize() + 1)
		{
			throw new DataLengthException("input too large for Naccache-Stern cipher.\n");
		}
		if (!forEncryption && length < GetInputBlockSize())
		{
			throw new InvalidCipherTextException("BlockLength does not match modulus for Naccache-Stern cipher.\n");
		}
		BigInteger bigInteger = new BigInteger(1, inBytes, inOff, length);
		if (forEncryption)
		{
			return Encrypt(bigInteger);
		}
		List<BigInteger> list = new List<BigInteger>();
		NaccacheSternPrivateKeyParameters naccacheSternPrivateKeyParameters = (NaccacheSternPrivateKeyParameters)key;
		IList<BigInteger> smallPrimesList = naccacheSternPrivateKeyParameters.SmallPrimesList;
		for (int i = 0; i < smallPrimesList.Count; i++)
		{
			BigInteger item = bigInteger.ModPow(naccacheSternPrivateKeyParameters.PhiN.Divide(smallPrimesList[i]), naccacheSternPrivateKeyParameters.Modulus);
			IList<BigInteger> list2 = lookup[i];
			if (lookup[i].Count != smallPrimesList[i].IntValue)
			{
				throw new InvalidCipherTextException("Error in lookup Array for " + smallPrimesList[i].IntValue + ": Size mismatch. Expected ArrayList with length " + smallPrimesList[i].IntValue + " but found ArrayList of length " + lookup[i].Count);
			}
			int num = list2.IndexOf(item);
			if (num == -1)
			{
				throw new InvalidCipherTextException("Lookup failed");
			}
			list.Add(BigInteger.ValueOf(num));
		}
		return ChineseRemainder(list, smallPrimesList).ToByteArray();
	}

	public virtual byte[] Encrypt(BigInteger plain)
	{
		byte[] array = new byte[key.Modulus.BitLength / 8 + 1];
		byte[] array2 = key.G.ModPow(plain, key.Modulus).ToByteArray();
		Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
		return array;
	}

	public virtual byte[] AddCryptedBlocks(byte[] block1, byte[] block2)
	{
		if (forEncryption)
		{
			if (block1.Length > GetOutputBlockSize() || block2.Length > GetOutputBlockSize())
			{
				throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
			}
		}
		else if (block1.Length > GetInputBlockSize() || block2.Length > GetInputBlockSize())
		{
			throw new InvalidCipherTextException("BlockLength too large for simple addition.\n");
		}
		BigInteger bigInteger = new BigInteger(1, block1);
		BigInteger val = new BigInteger(1, block2);
		BigInteger bigInteger2 = bigInteger.Multiply(val).Mod(key.Modulus);
		byte[] array = new byte[key.Modulus.BitLength / 8 + 1];
		byte[] array2 = bigInteger2.ToByteArray();
		Array.Copy(array2, 0, array, array.Length - array2.Length, array2.Length);
		return array;
	}

	public virtual byte[] ProcessData(byte[] data)
	{
		if (data.Length > GetInputBlockSize())
		{
			int inputBlockSize = GetInputBlockSize();
			int outputBlockSize = GetOutputBlockSize();
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[(data.Length / inputBlockSize + 1) * outputBlockSize];
			while (num < data.Length)
			{
				byte[] array2;
				if (num + inputBlockSize < data.Length)
				{
					array2 = ProcessBlock(data, num, inputBlockSize);
					num += inputBlockSize;
				}
				else
				{
					array2 = ProcessBlock(data, num, data.Length - num);
					num += data.Length - num;
				}
				if (array2 != null)
				{
					array2.CopyTo(array, num2);
					num2 += array2.Length;
					continue;
				}
				throw new InvalidCipherTextException("cipher returned null");
			}
			byte[] array3 = new byte[num2];
			Array.Copy(array, 0, array3, 0, num2);
			return array3;
		}
		return ProcessBlock(data, 0, data.Length);
	}

	private static BigInteger ChineseRemainder(IList<BigInteger> congruences, IList<BigInteger> primes)
	{
		BigInteger bigInteger = BigInteger.Zero;
		BigInteger bigInteger2 = BigInteger.One;
		for (int i = 0; i < primes.Count; i++)
		{
			bigInteger2 = bigInteger2.Multiply(primes[i]);
		}
		for (int j = 0; j < primes.Count; j++)
		{
			BigInteger bigInteger3 = primes[j];
			BigInteger bigInteger4 = bigInteger2.Divide(bigInteger3);
			BigInteger val = bigInteger4.ModInverse(bigInteger3);
			BigInteger bigInteger5 = bigInteger4.Multiply(val);
			bigInteger5 = bigInteger5.Multiply(congruences[j]);
			bigInteger = bigInteger.Add(bigInteger5);
		}
		return bigInteger.Mod(bigInteger2);
	}
}
