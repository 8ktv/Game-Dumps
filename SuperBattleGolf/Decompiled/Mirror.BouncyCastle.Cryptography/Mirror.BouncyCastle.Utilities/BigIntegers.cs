using System;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Utilities;

public static class BigIntegers
{
	public static readonly BigInteger Zero = BigInteger.Zero;

	public static readonly BigInteger One = BigInteger.One;

	private const int MaxIterations = 1000;

	public static byte[] AsUnsignedByteArray(BigInteger n)
	{
		return n.ToByteArrayUnsigned();
	}

	public static byte[] AsUnsignedByteArray(int length, BigInteger n)
	{
		byte[] array = n.ToByteArrayUnsigned();
		int num = array.Length;
		if (num == length)
		{
			return array;
		}
		if (num > length)
		{
			throw new ArgumentException("standard length exceeded", "n");
		}
		byte[] array2 = new byte[length];
		Array.Copy(array, 0, array2, length - num, num);
		return array2;
	}

	public static void AsUnsignedByteArray(BigInteger n, byte[] buf, int off, int len)
	{
		byte[] array = n.ToByteArrayUnsigned();
		int num = array.Length;
		if (num > len)
		{
			throw new ArgumentException("standard length exceeded", "n");
		}
		int num2 = len - num;
		Arrays.Fill(buf, off, off + num2, 0);
		Array.Copy(array, 0, buf, off + num2, num);
	}

	public static BigInteger CreateRandomBigInteger(int bitLength, SecureRandom secureRandom)
	{
		return new BigInteger(bitLength, secureRandom);
	}

	public static BigInteger CreateRandomInRange(BigInteger min, BigInteger max, SecureRandom random)
	{
		int num = min.CompareTo(max);
		if (num >= 0)
		{
			if (num > 0)
			{
				throw new ArgumentException("'min' may not be greater than 'max'");
			}
			return min;
		}
		if (min.BitLength > max.BitLength / 2)
		{
			return CreateRandomInRange(BigInteger.Zero, max.Subtract(min), random).Add(min);
		}
		for (int i = 0; i < 1000; i++)
		{
			BigInteger bigInteger = new BigInteger(max.BitLength, random);
			if (bigInteger.CompareTo(min) >= 0 && bigInteger.CompareTo(max) <= 0)
			{
				return bigInteger;
			}
		}
		return new BigInteger(max.Subtract(min).BitLength - 1, random).Add(min);
	}

	public static BigInteger FromUnsignedByteArray(byte[] buf)
	{
		return new BigInteger(1, buf);
	}

	public static BigInteger FromUnsignedByteArray(byte[] buf, int off, int length)
	{
		return new BigInteger(1, buf, off, length);
	}

	public static int GetByteLength(BigInteger n)
	{
		return n.GetLengthofByteArray();
	}

	public static int GetUnsignedByteLength(BigInteger n)
	{
		return n.GetLengthofByteArrayUnsigned();
	}

	public static BigInteger ModOddInverse(BigInteger M, BigInteger X)
	{
		if (!M.TestBit(0))
		{
			throw new ArgumentException("must be odd", "M");
		}
		if (M.SignValue != 1)
		{
			throw new ArithmeticException("BigInteger: modulus not positive");
		}
		if (X.SignValue < 0 || X.BitLength > M.BitLength)
		{
			X = X.Mod(M);
		}
		int bitLength = M.BitLength;
		uint[] array = Nat.FromBigInteger(bitLength, M);
		uint[] x = Nat.FromBigInteger(bitLength, X);
		int len = array.Length;
		uint[] array2 = Nat.Create(len);
		if (Mod.ModOddInverse(array, x, array2) == 0)
		{
			throw new ArithmeticException("BigInteger not invertible");
		}
		return Nat.ToBigInteger(len, array2);
	}

	public static BigInteger ModOddInverseVar(BigInteger M, BigInteger X)
	{
		if (!M.TestBit(0))
		{
			throw new ArgumentException("must be odd", "M");
		}
		if (M.SignValue != 1)
		{
			throw new ArithmeticException("BigInteger: modulus not positive");
		}
		if (M.Equals(One))
		{
			return Zero;
		}
		if (X.SignValue < 0 || X.BitLength > M.BitLength)
		{
			X = X.Mod(M);
		}
		if (X.Equals(One))
		{
			return One;
		}
		int bitLength = M.BitLength;
		uint[] array = Nat.FromBigInteger(bitLength, M);
		uint[] x = Nat.FromBigInteger(bitLength, X);
		int len = array.Length;
		uint[] array2 = Nat.Create(len);
		if (!Mod.ModOddInverseVar(array, x, array2))
		{
			throw new ArithmeticException("BigInteger not invertible");
		}
		return Nat.ToBigInteger(len, array2);
	}

	public static bool ModOddIsCoprime(BigInteger M, BigInteger X)
	{
		if (!M.TestBit(0))
		{
			throw new ArgumentException("must be odd", "M");
		}
		if (M.SignValue != 1)
		{
			throw new ArithmeticException("BigInteger: modulus not positive");
		}
		if (X.SignValue < 0 || X.BitLength > M.BitLength)
		{
			X = X.Mod(M);
		}
		int bitLength = M.BitLength;
		uint[] m = Nat.FromBigInteger(bitLength, M);
		uint[] x = Nat.FromBigInteger(bitLength, X);
		return Mod.ModOddIsCoprime(m, x) != 0;
	}

	public static bool ModOddIsCoprimeVar(BigInteger M, BigInteger X)
	{
		if (!M.TestBit(0))
		{
			throw new ArgumentException("must be odd", "M");
		}
		if (M.SignValue != 1)
		{
			throw new ArithmeticException("BigInteger: modulus not positive");
		}
		if (X.SignValue < 0 || X.BitLength > M.BitLength)
		{
			X = X.Mod(M);
		}
		if (X.Equals(One))
		{
			return true;
		}
		int bitLength = M.BitLength;
		uint[] m = Nat.FromBigInteger(bitLength, M);
		uint[] x = Nat.FromBigInteger(bitLength, X);
		return Mod.ModOddIsCoprimeVar(m, x);
	}
}
