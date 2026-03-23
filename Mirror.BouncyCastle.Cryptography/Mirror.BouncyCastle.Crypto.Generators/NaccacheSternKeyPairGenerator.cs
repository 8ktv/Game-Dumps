using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public class NaccacheSternKeyPairGenerator : IAsymmetricCipherKeyPairGenerator
{
	private static readonly int[] smallPrimes = new int[101]
	{
		3, 5, 7, 11, 13, 17, 19, 23, 29, 31,
		37, 41, 43, 47, 53, 59, 61, 67, 71, 73,
		79, 83, 89, 97, 101, 103, 107, 109, 113, 127,
		131, 137, 139, 149, 151, 157, 163, 167, 173, 179,
		181, 191, 193, 197, 199, 211, 223, 227, 229, 233,
		239, 241, 251, 257, 263, 269, 271, 277, 281, 283,
		293, 307, 311, 313, 317, 331, 337, 347, 349, 353,
		359, 367, 373, 379, 383, 389, 397, 401, 409, 419,
		421, 431, 433, 439, 443, 449, 457, 461, 463, 467,
		479, 487, 491, 499, 503, 509, 521, 523, 541, 547,
		557
	};

	private NaccacheSternKeyGenerationParameters param;

	public void Init(KeyGenerationParameters parameters)
	{
		param = (NaccacheSternKeyGenerationParameters)parameters;
	}

	public AsymmetricCipherKeyPair GenerateKeyPair()
	{
		int strength = param.Strength;
		SecureRandom random = param.Random;
		int certainty = param.Certainty;
		IList<BigInteger> arr = FindFirstPrimes(param.CountSmallPrimes);
		arr = PermuteList(arr, random);
		BigInteger bigInteger = BigInteger.One;
		BigInteger bigInteger2 = BigInteger.One;
		for (int i = 0; i < arr.Count / 2; i++)
		{
			bigInteger = bigInteger.Multiply(arr[i]);
		}
		for (int j = arr.Count / 2; j < arr.Count; j++)
		{
			bigInteger2 = bigInteger2.Multiply(arr[j]);
		}
		BigInteger bigInteger3 = bigInteger.Multiply(bigInteger2);
		int num = strength - bigInteger3.BitLength - 48;
		BigInteger bigInteger4 = GeneratePrime(num / 2 + 1, certainty, random);
		BigInteger bigInteger5 = GeneratePrime(num / 2 + 1, certainty, random);
		long num2 = 0L;
		BigInteger val = bigInteger4.Multiply(bigInteger).ShiftLeft(1);
		BigInteger val2 = bigInteger5.Multiply(bigInteger2).ShiftLeft(1);
		BigInteger bigInteger6;
		BigInteger bigInteger7;
		BigInteger bigInteger8;
		BigInteger bigInteger9;
		BigInteger bigInteger10;
		while (true)
		{
			num2++;
			bigInteger6 = GeneratePrime(24, certainty, random);
			bigInteger7 = bigInteger6.Multiply(val).Add(BigInteger.One);
			if (!bigInteger7.IsProbablePrime(certainty, randomlySelected: true))
			{
				continue;
			}
			while (true)
			{
				bigInteger8 = GeneratePrime(24, certainty, random);
				if (!bigInteger6.Equals(bigInteger8))
				{
					bigInteger9 = bigInteger8.Multiply(val2).Add(BigInteger.One);
					if (bigInteger9.IsProbablePrime(certainty, randomlySelected: true))
					{
						break;
					}
				}
			}
			if (BigIntegers.ModOddIsCoprime(bigInteger6.Multiply(bigInteger8), bigInteger3))
			{
				bigInteger10 = bigInteger7.Multiply(bigInteger9);
				if (bigInteger10.BitLength >= strength)
				{
					break;
				}
			}
		}
		BigInteger bigInteger11 = bigInteger7.Subtract(BigInteger.One).Multiply(bigInteger9.Subtract(BigInteger.One));
		num2 = 0L;
		BigInteger bigInteger12;
		bool flag;
		do
		{
			List<BigInteger> list = new List<BigInteger>();
			for (int k = 0; k != arr.Count; k++)
			{
				BigInteger val3 = arr[k];
				BigInteger e = bigInteger11.Divide(val3);
				do
				{
					num2++;
					bigInteger12 = GeneratePrime(strength, certainty, random);
				}
				while (bigInteger12.ModPow(e, bigInteger10).Equals(BigInteger.One));
				list.Add(bigInteger12);
			}
			bigInteger12 = BigInteger.One;
			for (int l = 0; l < arr.Count; l++)
			{
				BigInteger bigInteger13 = list[l];
				BigInteger val4 = arr[l];
				bigInteger12 = bigInteger12.Multiply(bigInteger13.ModPow(bigInteger3.Divide(val4), bigInteger10)).Mod(bigInteger10);
			}
			flag = false;
			for (int m = 0; m < arr.Count; m++)
			{
				if (bigInteger12.ModPow(bigInteger11.Divide(arr[m]), bigInteger10).Equals(BigInteger.One))
				{
					flag = true;
					break;
				}
			}
		}
		while (flag || bigInteger12.ModPow(bigInteger11.ShiftRight(2), bigInteger10).Equals(BigInteger.One) || bigInteger12.ModPow(bigInteger11.Divide(bigInteger6), bigInteger10).Equals(BigInteger.One) || bigInteger12.ModPow(bigInteger11.Divide(bigInteger8), bigInteger10).Equals(BigInteger.One) || bigInteger12.ModPow(bigInteger11.Divide(bigInteger4), bigInteger10).Equals(BigInteger.One) || bigInteger12.ModPow(bigInteger11.Divide(bigInteger5), bigInteger10).Equals(BigInteger.One));
		return new AsymmetricCipherKeyPair(new NaccacheSternKeyParameters(privateKey: false, bigInteger12, bigInteger10, bigInteger3.BitLength), new NaccacheSternPrivateKeyParameters(bigInteger12, bigInteger10, bigInteger3.BitLength, arr, bigInteger11));
	}

	private static BigInteger GeneratePrime(int bitLength, int certainty, SecureRandom rand)
	{
		return new BigInteger(bitLength, certainty, rand);
	}

	private static IList<T> PermuteList<T>(IList<T> arr, SecureRandom rand)
	{
		List<T> list = new List<T>(arr.Count);
		foreach (T item in arr)
		{
			int index = rand.Next(list.Count + 1);
			list.Insert(index, item);
		}
		return list;
	}

	private static IList<BigInteger> FindFirstPrimes(int count)
	{
		List<BigInteger> list = new List<BigInteger>(count);
		for (int i = 0; i != count; i++)
		{
			list.Add(BigInteger.ValueOf(smallPrimes[i]));
		}
		return list;
	}
}
