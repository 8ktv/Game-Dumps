using System;
using System.Collections.Concurrent;
using Mirror.BouncyCastle.Math.Field;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC;

public abstract class AbstractFpCurve : ECCurve
{
	private static readonly ConcurrentDictionary<BigInteger, bool> KnownPrimes = new ConcurrentDictionary<BigInteger, bool>();

	protected AbstractFpCurve(BigInteger q)
		: this(q, isInternal: false)
	{
	}

	internal AbstractFpCurve(BigInteger q, bool isInternal)
		: base(FiniteFields.GetPrimeField(q))
	{
		if (isInternal)
		{
			KnownPrimes.AddOrUpdate(q, addValue: true, (BigInteger key, bool value) => true);
		}
		else if (!KnownPrimes.ContainsKey(q))
		{
			ImplCheckQ(q);
			KnownPrimes.TryAdd(q, value: false);
		}
	}

	public override bool IsValidFieldElement(BigInteger x)
	{
		if (x != null && x.SignValue >= 0)
		{
			return x.CompareTo(Field.Characteristic) < 0;
		}
		return false;
	}

	public override ECFieldElement RandomFieldElement(SecureRandom r)
	{
		BigInteger characteristic = Field.Characteristic;
		ECFieldElement eCFieldElement = FromBigInteger(ImplRandomFieldElement(r, characteristic));
		ECFieldElement b = FromBigInteger(ImplRandomFieldElement(r, characteristic));
		return eCFieldElement.Multiply(b);
	}

	public override ECFieldElement RandomFieldElementMult(SecureRandom r)
	{
		BigInteger characteristic = Field.Characteristic;
		ECFieldElement eCFieldElement = FromBigInteger(ImplRandomFieldElementMult(r, characteristic));
		ECFieldElement b = FromBigInteger(ImplRandomFieldElementMult(r, characteristic));
		return eCFieldElement.Multiply(b);
	}

	protected override ECPoint DecompressPoint(int yTilde, BigInteger X1)
	{
		ECFieldElement eCFieldElement = FromBigInteger(X1);
		ECFieldElement eCFieldElement2 = eCFieldElement.Square().Add(A).Multiply(eCFieldElement)
			.Add(B)
			.Sqrt();
		if (eCFieldElement2 == null)
		{
			throw new ArgumentException("Invalid point compression");
		}
		if (eCFieldElement2.TestBitZero() != (yTilde == 1))
		{
			eCFieldElement2 = eCFieldElement2.Negate();
		}
		return CreateRawPoint(eCFieldElement, eCFieldElement2);
	}

	private static void ImplCheckQ(BigInteger q)
	{
		int num = ECCurve.ImplGetInteger("Mirror.BouncyCastle.EC.Fp_MaxSize", 1042);
		if (q.BitLength > num)
		{
			throw new ArgumentException("Fp q value out of range");
		}
		if (!ImplIsPrime(q))
		{
			throw new ArgumentException("Fp q value not prime");
		}
	}

	private static int ImplGetIterations(int bits, int certainty)
	{
		if (bits >= 1536)
		{
			if (certainty > 100)
			{
				if (certainty > 128)
				{
					return 4 + (certainty - 128 + 1) / 2;
				}
				return 4;
			}
			return 3;
		}
		if (bits >= 1024)
		{
			if (certainty > 100)
			{
				if (certainty > 112)
				{
					return 5 + (certainty - 112 + 1) / 2;
				}
				return 5;
			}
			return 4;
		}
		if (bits >= 512)
		{
			if (certainty > 80)
			{
				if (certainty > 100)
				{
					return 7 + (certainty - 100 + 1) / 2;
				}
				return 7;
			}
			return 5;
		}
		if (certainty > 80)
		{
			return 40 + (certainty - 80 + 1) / 2;
		}
		return 40;
	}

	private static bool ImplIsPrime(BigInteger q)
	{
		if (Primes.HasAnySmallFactors(q))
		{
			return false;
		}
		int certainty = ECCurve.ImplGetInteger("Mirror.BouncyCastle.EC.Fp_Certainty", 100);
		int iterations = ImplGetIterations(q.BitLength, certainty);
		return Primes.IsMRProbablePrime(q, SecureRandom.ArbitraryRandom, iterations);
	}

	private static BigInteger ImplRandomFieldElement(SecureRandom r, BigInteger p)
	{
		BigInteger bigInteger;
		do
		{
			bigInteger = BigIntegers.CreateRandomBigInteger(p.BitLength, r);
		}
		while (bigInteger.CompareTo(p) >= 0);
		return bigInteger;
	}

	private static BigInteger ImplRandomFieldElementMult(SecureRandom r, BigInteger p)
	{
		BigInteger bigInteger;
		do
		{
			bigInteger = BigIntegers.CreateRandomBigInteger(p.BitLength, r);
		}
		while (bigInteger.SignValue <= 0 || bigInteger.CompareTo(p) >= 0);
		return bigInteger;
	}
}
