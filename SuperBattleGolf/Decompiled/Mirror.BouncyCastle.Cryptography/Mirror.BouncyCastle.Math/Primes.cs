using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math;

public static class Primes
{
	public sealed class MROutput
	{
		private readonly bool m_provablyComposite;

		private readonly BigInteger m_factor;

		public BigInteger Factor => m_factor;

		public bool IsProvablyComposite => m_provablyComposite;

		public bool IsNotPrimePower
		{
			get
			{
				if (m_provablyComposite)
				{
					return m_factor == null;
				}
				return false;
			}
		}

		internal static MROutput ProbablyPrime()
		{
			return new MROutput(provablyComposite: false, null);
		}

		internal static MROutput ProvablyCompositeWithFactor(BigInteger factor)
		{
			return new MROutput(provablyComposite: true, factor);
		}

		internal static MROutput ProvablyCompositeNotPrimePower()
		{
			return new MROutput(provablyComposite: true, null);
		}

		private MROutput(bool provablyComposite, BigInteger factor)
		{
			m_provablyComposite = provablyComposite;
			m_factor = factor;
		}
	}

	public sealed class STOutput
	{
		private readonly BigInteger m_prime;

		private readonly byte[] m_primeSeed;

		private readonly int m_primeGenCounter;

		public BigInteger Prime => m_prime;

		public byte[] PrimeSeed => m_primeSeed;

		public int PrimeGenCounter => m_primeGenCounter;

		internal STOutput(BigInteger prime, byte[] primeSeed, int primeGenCounter)
		{
			m_prime = prime;
			m_primeSeed = primeSeed;
			m_primeGenCounter = primeGenCounter;
		}
	}

	public static readonly int SmallFactorLimit = 211;

	private static readonly BigInteger One = BigInteger.One;

	private static readonly BigInteger Two = BigInteger.Two;

	private static readonly BigInteger Three = BigInteger.Three;

	public static STOutput GenerateSTRandomPrime(IDigest hash, int length, byte[] inputSeed)
	{
		if (hash == null)
		{
			throw new ArgumentNullException("hash");
		}
		if (length < 2)
		{
			throw new ArgumentException("must be >= 2", "length");
		}
		if (inputSeed == null)
		{
			throw new ArgumentNullException("inputSeed");
		}
		if (inputSeed.Length == 0)
		{
			throw new ArgumentException("cannot be empty", "inputSeed");
		}
		return ImplSTRandomPrime(hash, length, Arrays.Clone(inputSeed));
	}

	public static MROutput EnhancedMRProbablePrimeTest(BigInteger candidate, SecureRandom random, int iterations)
	{
		CheckCandidate(candidate, "candidate");
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		if (iterations < 1)
		{
			throw new ArgumentException("must be > 0", "iterations");
		}
		if (candidate.BitLength == 2)
		{
			return MROutput.ProbablyPrime();
		}
		if (!candidate.TestBit(0))
		{
			return MROutput.ProvablyCompositeWithFactor(Two);
		}
		BigInteger bigInteger = candidate.Subtract(One);
		BigInteger max = candidate.Subtract(Two);
		int lowestSetBit = bigInteger.GetLowestSetBit();
		BigInteger e = bigInteger.ShiftRight(lowestSetBit);
		for (int i = 0; i < iterations; i++)
		{
			BigInteger bigInteger2 = BigIntegers.CreateRandomInRange(Two, max, random);
			BigInteger bigInteger3 = bigInteger2.Gcd(candidate);
			if (bigInteger3.CompareTo(One) > 0)
			{
				return MROutput.ProvablyCompositeWithFactor(bigInteger3);
			}
			BigInteger bigInteger4 = bigInteger2.ModPow(e, candidate);
			if (bigInteger4.Equals(One) || bigInteger4.Equals(bigInteger))
			{
				continue;
			}
			bool flag = false;
			BigInteger bigInteger5 = bigInteger4;
			for (int j = 1; j < lowestSetBit; j++)
			{
				bigInteger4 = bigInteger4.Square().Mod(candidate);
				if (bigInteger4.Equals(bigInteger))
				{
					flag = true;
					break;
				}
				if (bigInteger4.Equals(One))
				{
					break;
				}
				bigInteger5 = bigInteger4;
			}
			if (flag)
			{
				continue;
			}
			if (!bigInteger4.Equals(One))
			{
				bigInteger5 = bigInteger4;
				bigInteger4 = bigInteger4.Square().Mod(candidate);
				if (!bigInteger4.Equals(One))
				{
					bigInteger5 = bigInteger4;
				}
			}
			bigInteger3 = bigInteger5.Subtract(One).Gcd(candidate);
			if (bigInteger3.CompareTo(One) > 0)
			{
				return MROutput.ProvablyCompositeWithFactor(bigInteger3);
			}
			return MROutput.ProvablyCompositeNotPrimePower();
		}
		return MROutput.ProbablyPrime();
	}

	public static bool HasAnySmallFactors(BigInteger candidate)
	{
		CheckCandidate(candidate, "candidate");
		return ImplHasAnySmallFactors(candidate);
	}

	public static bool IsMRProbablePrime(BigInteger candidate, SecureRandom random, int iterations)
	{
		CheckCandidate(candidate, "candidate");
		if (random == null)
		{
			throw new ArgumentException("cannot be null", "random");
		}
		if (iterations < 1)
		{
			throw new ArgumentException("must be > 0", "iterations");
		}
		if (candidate.BitLength == 2)
		{
			return true;
		}
		if (!candidate.TestBit(0))
		{
			return false;
		}
		BigInteger bigInteger = candidate.Subtract(One);
		BigInteger max = candidate.Subtract(Two);
		int lowestSetBit = bigInteger.GetLowestSetBit();
		BigInteger m = bigInteger.ShiftRight(lowestSetBit);
		for (int i = 0; i < iterations; i++)
		{
			BigInteger b = BigIntegers.CreateRandomInRange(Two, max, random);
			if (!ImplMRProbablePrimeToBase(candidate, bigInteger, m, lowestSetBit, b))
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsMRProbablePrimeToBase(BigInteger candidate, BigInteger baseValue)
	{
		CheckCandidate(candidate, "candidate");
		CheckCandidate(baseValue, "baseValue");
		if (baseValue.CompareTo(candidate.Subtract(One)) >= 0)
		{
			throw new ArgumentException("must be < ('candidate' - 1)", "baseValue");
		}
		if (candidate.BitLength == 2)
		{
			return true;
		}
		BigInteger bigInteger = candidate.Subtract(One);
		int lowestSetBit = bigInteger.GetLowestSetBit();
		BigInteger m = bigInteger.ShiftRight(lowestSetBit);
		return ImplMRProbablePrimeToBase(candidate, bigInteger, m, lowestSetBit, baseValue);
	}

	private static void CheckCandidate(BigInteger n, string name)
	{
		if (n == null || n.SignValue < 1 || n.BitLength < 2)
		{
			throw new ArgumentException("must be non-null and >= 2", name);
		}
	}

	private static bool ImplHasAnySmallFactors(BigInteger x)
	{
		int value = 223092870;
		int intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 2 == 0 || intValue % 3 == 0 || intValue % 5 == 0 || intValue % 7 == 0 || intValue % 11 == 0 || intValue % 13 == 0 || intValue % 17 == 0 || intValue % 19 == 0 || intValue % 23 == 0)
		{
			return true;
		}
		value = 58642669;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 29 == 0 || intValue % 31 == 0 || intValue % 37 == 0 || intValue % 41 == 0 || intValue % 43 == 0)
		{
			return true;
		}
		value = 600662303;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 47 == 0 || intValue % 53 == 0 || intValue % 59 == 0 || intValue % 61 == 0 || intValue % 67 == 0)
		{
			return true;
		}
		value = 33984931;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 71 == 0 || intValue % 73 == 0 || intValue % 79 == 0 || intValue % 83 == 0)
		{
			return true;
		}
		value = 89809099;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 89 == 0 || intValue % 97 == 0 || intValue % 101 == 0 || intValue % 103 == 0)
		{
			return true;
		}
		value = 167375713;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 107 == 0 || intValue % 109 == 0 || intValue % 113 == 0 || intValue % 127 == 0)
		{
			return true;
		}
		value = 371700317;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 131 == 0 || intValue % 137 == 0 || intValue % 139 == 0 || intValue % 149 == 0)
		{
			return true;
		}
		value = 645328247;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 151 == 0 || intValue % 157 == 0 || intValue % 163 == 0 || intValue % 167 == 0)
		{
			return true;
		}
		value = 1070560157;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 173 == 0 || intValue % 179 == 0 || intValue % 181 == 0 || intValue % 191 == 0)
		{
			return true;
		}
		value = 1596463769;
		intValue = x.Mod(BigInteger.ValueOf(value)).IntValue;
		if (intValue % 193 == 0 || intValue % 197 == 0 || intValue % 199 == 0 || intValue % 211 == 0)
		{
			return true;
		}
		return false;
	}

	private static bool ImplMRProbablePrimeToBase(BigInteger w, BigInteger wSubOne, BigInteger m, int a, BigInteger b)
	{
		BigInteger bigInteger = b.ModPow(m, w);
		if (bigInteger.Equals(One) || bigInteger.Equals(wSubOne))
		{
			return true;
		}
		for (int i = 1; i < a; i++)
		{
			bigInteger = bigInteger.Square().Mod(w);
			if (bigInteger.Equals(wSubOne))
			{
				return true;
			}
			if (bigInteger.Equals(One))
			{
				return false;
			}
		}
		return false;
	}

	private static STOutput ImplSTRandomPrime(IDigest d, int length, byte[] primeSeed)
	{
		int digestSize = d.GetDigestSize();
		int num = System.Math.Max(4, digestSize);
		if (length < 33)
		{
			int num2 = 0;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			do
			{
				Hash(d, primeSeed, array, num - digestSize);
				Inc(primeSeed, 1);
				Hash(d, primeSeed, array2, num - digestSize);
				Inc(primeSeed, 1);
				uint num3 = Pack.BE_To_UInt32(array, num - 4) ^ Pack.BE_To_UInt32(array2, num - 4);
				num3 &= (uint)(-1 >>> 32 - length);
				num3 |= (uint)((1 << length - 1) | 1);
				num2++;
				if (IsPrime32(num3))
				{
					return new STOutput(BigInteger.ValueOf(num3), primeSeed, num2);
				}
			}
			while (num2 <= 4 * length);
			throw new InvalidOperationException("Too many iterations in Shawe-Taylor Random_Prime Routine");
		}
		STOutput sTOutput = ImplSTRandomPrime(d, (length + 3) / 2, primeSeed);
		BigInteger prime = sTOutput.Prime;
		primeSeed = sTOutput.PrimeSeed;
		int num4 = sTOutput.PrimeGenCounter;
		int num5 = 8 * digestSize;
		int num6 = (length - 1) / num5;
		int num7 = num4;
		BigInteger bigInteger = HashGen(d, primeSeed, num6 + 1).Mod(One.ShiftLeft(length - 1)).SetBit(length - 1);
		BigInteger bigInteger2 = prime.ShiftLeft(1);
		BigInteger bigInteger3 = bigInteger.Subtract(One).Divide(bigInteger2).Add(One)
			.ShiftLeft(1);
		int num8 = 0;
		BigInteger bigInteger4 = bigInteger3.Multiply(prime).Add(One);
		while (true)
		{
			if (bigInteger4.BitLength > length)
			{
				bigInteger3 = One.ShiftLeft(length - 1).Subtract(One).Divide(bigInteger2)
					.Add(One)
					.ShiftLeft(1);
				bigInteger4 = bigInteger3.Multiply(prime).Add(One);
			}
			num4++;
			if (ImplHasAnySmallFactors(bigInteger4))
			{
				Inc(primeSeed, num6 + 1);
			}
			else
			{
				BigInteger bigInteger5 = HashGen(d, primeSeed, num6 + 1).Mod(bigInteger4.Subtract(Three)).Add(Two);
				bigInteger3 = bigInteger3.Add(BigInteger.ValueOf(num8));
				num8 = 0;
				BigInteger bigInteger6 = bigInteger5.ModPow(bigInteger3, bigInteger4);
				if (bigInteger4.Gcd(bigInteger6.Subtract(One)).Equals(One) && bigInteger6.ModPow(prime, bigInteger4).Equals(One))
				{
					return new STOutput(bigInteger4, primeSeed, num4);
				}
			}
			if (num4 >= 4 * length + num7)
			{
				break;
			}
			num8 += 2;
			bigInteger4 = bigInteger4.Add(bigInteger2);
		}
		throw new InvalidOperationException("Too many iterations in Shawe-Taylor Random_Prime Routine");
	}

	private static void Hash(IDigest d, byte[] input, byte[] output, int outPos)
	{
		d.BlockUpdate(input, 0, input.Length);
		d.DoFinal(output, outPos);
	}

	private static BigInteger HashGen(IDigest d, byte[] seed, int count)
	{
		int digestSize = d.GetDigestSize();
		int num = count * digestSize;
		byte[] array = new byte[num];
		for (int i = 0; i < count; i++)
		{
			num -= digestSize;
			Hash(d, seed, array, num);
			Inc(seed, 1);
		}
		return new BigInteger(1, array);
	}

	private static void Inc(byte[] seed, int c)
	{
		int num = seed.Length;
		while (c > 0 && --num >= 0)
		{
			c += seed[num];
			seed[num] = (byte)c;
			c >>= 8;
		}
	}

	private static bool IsPrime32(uint x)
	{
		if (x < 32)
		{
			return ((1 << (int)x) & 0x208A28AC) != 0;
		}
		if (((1 << (int)(x % 30)) & 0xA08A2882u) == 0L)
		{
			return false;
		}
		uint[] array = new uint[8] { 1u, 7u, 11u, 13u, 17u, 19u, 23u, 29u };
		uint num = 0u;
		int num2 = 1;
		while (true)
		{
			if (num2 < array.Length)
			{
				uint num3 = num + array[num2];
				if (x % num3 == 0)
				{
					return false;
				}
				num2++;
			}
			else
			{
				num += 30;
				if (num >> 16 != 0 || num * num >= x)
				{
					break;
				}
				num2 = 0;
			}
		}
		return true;
	}
}
