using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Fpe;

internal static class SP80038G
{
	internal static readonly string FPE_DISABLED = "Mirror.BouncyCastle.Fpe.Disable";

	internal static readonly string FF1_DISABLED = "Mirror.BouncyCastle.Fpe.Disable_Ff1";

	private static readonly int BLOCK_SIZE = 16;

	private static readonly double LOG2 = System.Math.Log(2.0);

	private static readonly double TWO_TO_96 = System.Math.Pow(2.0, 96.0);

	public static byte[] DecryptFF1(IBlockCipher cipher, int radix, byte[] tweak, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: true, radix, buf, off, len);
		int num = len / 2;
		int num2 = len - num;
		ushort[] a = ToShort(buf, off, num);
		ushort[] b = ToShort(buf, off + num, num2);
		return ToByte(DecFF1(cipher, radix, tweak, len, num, num2, a, b));
	}

	public static ushort[] DecryptFF1w(IBlockCipher cipher, int radix, byte[] tweak, ushort[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: true, radix, buf, off, len);
		int num = len / 2;
		int num2 = len - num;
		ushort[] array = new ushort[num];
		ushort[] array2 = new ushort[num2];
		Array.Copy(buf, off, array, 0, num);
		Array.Copy(buf, off + num, array2, 0, num2);
		return DecFF1(cipher, radix, tweak, len, num, num2, array, array2);
	}

	private static ushort[] DecFF1(IBlockCipher cipher, int radix, byte[] T, int n, int u, int v, ushort[] A, ushort[] B)
	{
		int t = T.Length;
		int num = CalculateB_FF1(radix, v);
		int d = (num + 7) & -4;
		byte[] p = CalculateP_FF1(radix, (byte)u, n, t);
		BigInteger bigInteger = BigInteger.ValueOf(radix);
		BigInteger[] array = CalculateModUV(bigInteger, u, v);
		int num2 = u;
		for (int num3 = 9; num3 >= 0; num3--)
		{
			BigInteger n2 = CalculateY_FF1(cipher, bigInteger, T, num, d, num3, p, A);
			num2 = n - num2;
			BigInteger m = array[num3 & 1];
			BigInteger x = Num(bigInteger, B).Subtract(n2).Mod(m);
			ushort[] array2 = B;
			B = A;
			A = array2;
			Str(bigInteger, x, num2, array2, 0);
		}
		return Arrays.Concatenate(A, B);
	}

	public static byte[] DecryptFF3(IBlockCipher cipher, int radix, byte[] tweak64, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak64.Length != 8)
		{
			throw new ArgumentException();
		}
		return ImplDecryptFF3(cipher, radix, tweak64, buf, off, len);
	}

	public static byte[] DecryptFF3_1(IBlockCipher cipher, int radix, byte[] tweak56, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak56.Length != 7)
		{
			throw new ArgumentException("tweak should be 56 bits");
		}
		byte[] tweak57 = CalculateTweak64_FF3_1(tweak56);
		return ImplDecryptFF3(cipher, radix, tweak57, buf, off, len);
	}

	public static ushort[] DecryptFF3_1w(IBlockCipher cipher, int radix, byte[] tweak56, ushort[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak56.Length != 7)
		{
			throw new ArgumentException("tweak should be 56 bits");
		}
		byte[] tweak57 = CalculateTweak64_FF3_1(tweak56);
		return ImplDecryptFF3w(cipher, radix, tweak57, buf, off, len);
	}

	public static byte[] EncryptFF1(IBlockCipher cipher, int radix, byte[] tweak, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: true, radix, buf, off, len);
		int num = len / 2;
		int num2 = len - num;
		ushort[] a = ToShort(buf, off, num);
		ushort[] b = ToShort(buf, off + num, num2);
		return ToByte(EncFF1(cipher, radix, tweak, len, num, num2, a, b));
	}

	public static ushort[] EncryptFF1w(IBlockCipher cipher, int radix, byte[] tweak, ushort[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: true, radix, buf, off, len);
		int num = len / 2;
		int num2 = len - num;
		ushort[] array = new ushort[num];
		ushort[] array2 = new ushort[num2];
		Array.Copy(buf, off, array, 0, num);
		Array.Copy(buf, off + num, array2, 0, num2);
		return EncFF1(cipher, radix, tweak, len, num, num2, array, array2);
	}

	private static ushort[] EncFF1(IBlockCipher cipher, int radix, byte[] T, int n, int u, int v, ushort[] A, ushort[] B)
	{
		int t = T.Length;
		int num = CalculateB_FF1(radix, v);
		int d = (num + 7) & -4;
		byte[] p = CalculateP_FF1(radix, (byte)u, n, t);
		BigInteger bigInteger = BigInteger.ValueOf(radix);
		BigInteger[] array = CalculateModUV(bigInteger, u, v);
		int num2 = v;
		for (int i = 0; i < 10; i++)
		{
			BigInteger value = CalculateY_FF1(cipher, bigInteger, T, num, d, i, p, B);
			num2 = n - num2;
			BigInteger m = array[i & 1];
			BigInteger x = Num(bigInteger, A).Add(value).Mod(m);
			ushort[] array2 = A;
			A = B;
			B = array2;
			Str(bigInteger, x, num2, array2, 0);
		}
		return Arrays.Concatenate(A, B);
	}

	public static byte[] EncryptFF3(IBlockCipher cipher, int radix, byte[] tweak64, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak64.Length != 8)
		{
			throw new ArgumentException();
		}
		return ImplEncryptFF3(cipher, radix, tweak64, buf, off, len);
	}

	public static ushort[] EncryptFF3w(IBlockCipher cipher, int radix, byte[] tweak64, ushort[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak64.Length != 8)
		{
			throw new ArgumentException();
		}
		return ImplEncryptFF3w(cipher, radix, tweak64, buf, off, len);
	}

	public static ushort[] EncryptFF3_1w(IBlockCipher cipher, int radix, byte[] tweak56, ushort[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak56.Length != 7)
		{
			throw new ArgumentException("tweak should be 56 bits");
		}
		byte[] tweak57 = CalculateTweak64_FF3_1(tweak56);
		return EncryptFF3w(cipher, radix, tweak57, buf, off, len);
	}

	public static byte[] EncryptFF3_1(IBlockCipher cipher, int radix, byte[] tweak56, byte[] buf, int off, int len)
	{
		CheckArgs(cipher, isFF1: false, radix, buf, off, len);
		if (tweak56.Length != 7)
		{
			throw new ArgumentException("tweak should be 56 bits");
		}
		byte[] tweak57 = CalculateTweak64_FF3_1(tweak56);
		return EncryptFF3(cipher, radix, tweak57, buf, off, len);
	}

	private static int CalculateB_FF1(int radix, int v)
	{
		int num = Integers.NumberOfTrailingZeros(radix);
		int num2 = num * v;
		int num3 = radix >> num;
		if (num3 != 1)
		{
			num2 += BigInteger.ValueOf(num3).Pow(v).BitLength;
		}
		return (num2 + 7) / 8;
	}

	private static BigInteger[] CalculateModUV(BigInteger bigRadix, int u, int v)
	{
		BigInteger[] array = new BigInteger[2];
		array[0] = bigRadix.Pow(u);
		array[1] = array[0];
		if (v != u)
		{
			array[1] = array[1].Multiply(bigRadix);
		}
		return array;
	}

	private static byte[] CalculateP_FF1(int radix, byte uLow, int n, int t)
	{
		byte[] array = new byte[BLOCK_SIZE];
		array[0] = 1;
		array[1] = 2;
		array[2] = 1;
		array[3] = 0;
		array[4] = (byte)(radix >> 8);
		array[5] = (byte)radix;
		array[6] = 10;
		array[7] = uLow;
		Pack.UInt32_To_BE((uint)n, array, 8);
		Pack.UInt32_To_BE((uint)t, array, 12);
		return array;
	}

	private static byte[] CalculateTweak64_FF3_1(byte[] tweak56)
	{
		return new byte[8]
		{
			tweak56[0],
			tweak56[1],
			tweak56[2],
			(byte)(tweak56[3] & 0xF0),
			tweak56[4],
			tweak56[5],
			tweak56[6],
			(byte)(tweak56[3] << 4)
		};
	}

	private static BigInteger CalculateY_FF1(IBlockCipher cipher, BigInteger bigRadix, byte[] T, int b, int d, int round, byte[] P, ushort[] AB)
	{
		int num = T.Length;
		int num2 = -(num + b + 1) & 0xF;
		byte[] array = new byte[num + num2 + 1 + b];
		Array.Copy(T, 0, array, 0, num);
		array[num + num2] = (byte)round;
		BigIntegers.AsUnsignedByteArray(Num(bigRadix, AB), array, array.Length - b, b);
		byte[] array2 = Prf(cipher, Arrays.Concatenate(P, array));
		byte[] array3 = array2;
		if (d > BLOCK_SIZE)
		{
			int num3 = (d + BLOCK_SIZE - 1) / BLOCK_SIZE;
			array3 = new byte[num3 * BLOCK_SIZE];
			uint num4 = Pack.BE_To_UInt32(array2, BLOCK_SIZE - 4);
			Array.Copy(array2, 0, array3, 0, BLOCK_SIZE);
			for (uint num5 = 1u; num5 < num3; num5++)
			{
				int num6 = (int)(num5 * BLOCK_SIZE);
				Array.Copy(array2, 0, array3, num6, BLOCK_SIZE - 4);
				Pack.UInt32_To_BE(num4 ^ num5, array3, num6 + BLOCK_SIZE - 4);
				cipher.ProcessBlock(array3, num6, array3, num6);
			}
		}
		return new BigInteger(1, array3, 0, d);
	}

	private static BigInteger CalculateY_FF3(IBlockCipher cipher, BigInteger bigRadix, byte[] T, int wOff, uint round, ushort[] AB)
	{
		byte[] array = new byte[BLOCK_SIZE];
		Pack.UInt32_To_BE(Pack.BE_To_UInt32(T, wOff) ^ round, array, 0);
		BigIntegers.AsUnsignedByteArray(Num(bigRadix, AB), array, 4, BLOCK_SIZE - 4);
		Array.Reverse((Array)array);
		cipher.ProcessBlock(array, 0, array, 0);
		byte[] bytes = array;
		return new BigInteger(1, bytes, bigEndian: false);
	}

	private static void CheckArgs(IBlockCipher cipher, bool isFF1, int radix, ushort[] buf, int off, int len)
	{
		CheckCipher(cipher);
		if (radix < 2 || radix > 65536)
		{
			throw new ArgumentException();
		}
		CheckData(isFF1, radix, buf, off, len);
	}

	private static void CheckArgs(IBlockCipher cipher, bool isFF1, int radix, byte[] buf, int off, int len)
	{
		CheckCipher(cipher);
		if (radix < 2 || radix > 256)
		{
			throw new ArgumentException();
		}
		CheckData(isFF1, radix, buf, off, len);
	}

	private static void CheckCipher(IBlockCipher cipher)
	{
		if (BLOCK_SIZE != cipher.GetBlockSize())
		{
			throw new ArgumentException();
		}
	}

	private static void CheckData(bool isFF1, int radix, ushort[] buf, int off, int len)
	{
		CheckLength(isFF1, radix, len);
		for (int i = 0; i < len; i++)
		{
			if ((buf[off + i] & 0xFFFF) >= radix)
			{
				throw new ArgumentException("input data outside of radix");
			}
		}
	}

	private static void CheckData(bool isFF1, int radix, byte[] buf, int off, int len)
	{
		CheckLength(isFF1, radix, len);
		for (int i = 0; i < len; i++)
		{
			if ((buf[off + i] & 0xFF) >= radix)
			{
				throw new ArgumentException("input data outside of radix");
			}
		}
	}

	private static void CheckLength(bool isFF1, int radix, int len)
	{
		if (len < 2 || System.Math.Pow(radix, len) < 1000000.0)
		{
			throw new ArgumentException("input too short");
		}
		if (!isFF1)
		{
			int num = 2 * (int)System.Math.Floor(System.Math.Log(TWO_TO_96) / System.Math.Log(radix));
			if (len > num)
			{
				throw new ArgumentException("maximum input length is " + num);
			}
		}
	}

	private static byte[] ImplDecryptFF3(IBlockCipher cipher, int radix, byte[] tweak64, byte[] buf, int off, int len)
	{
		int num = len / 2;
		int num2 = len - num;
		ushort[] a = ToShort(buf, off, num2);
		ushort[] b = ToShort(buf, off + num2, num);
		return ToByte(DecFF3_1(cipher, radix, tweak64, len, num, num2, a, b));
	}

	private static ushort[] ImplDecryptFF3w(IBlockCipher cipher, int radix, byte[] tweak64, ushort[] buf, int off, int len)
	{
		int num = len / 2;
		int num2 = len - num;
		ushort[] array = new ushort[num2];
		ushort[] array2 = new ushort[num];
		Array.Copy(buf, off, array, 0, num2);
		Array.Copy(buf, off + num2, array2, 0, num);
		return DecFF3_1(cipher, radix, tweak64, len, num, num2, array, array2);
	}

	private static ushort[] DecFF3_1(IBlockCipher cipher, int radix, byte[] T, int n, int v, int u, ushort[] A, ushort[] B)
	{
		BigInteger bigInteger = BigInteger.ValueOf(radix);
		BigInteger[] array = CalculateModUV(bigInteger, v, u);
		int num = u;
		Array.Reverse((Array)A);
		Array.Reverse((Array)B);
		for (int num2 = 7; num2 >= 0; num2--)
		{
			num = n - num;
			BigInteger m = array[1 - (num2 & 1)];
			int wOff = 4 - (num2 & 1) * 4;
			BigInteger n2 = CalculateY_FF3(cipher, bigInteger, T, wOff, (uint)num2, A);
			BigInteger x = Num(bigInteger, B).Subtract(n2).Mod(m);
			ushort[] array2 = B;
			B = A;
			A = array2;
			Str(bigInteger, x, num, array2, 0);
		}
		Array.Reverse((Array)A);
		Array.Reverse((Array)B);
		return Arrays.Concatenate(A, B);
	}

	private static byte[] ImplEncryptFF3(IBlockCipher cipher, int radix, byte[] tweak64, byte[] buf, int off, int len)
	{
		int num = len / 2;
		int num2 = len - num;
		ushort[] a = ToShort(buf, off, num2);
		ushort[] b = ToShort(buf, off + num2, num);
		return ToByte(EncFF3_1(cipher, radix, tweak64, len, num, num2, a, b));
	}

	private static ushort[] ImplEncryptFF3w(IBlockCipher cipher, int radix, byte[] tweak64, ushort[] buf, int off, int len)
	{
		int num = len / 2;
		int num2 = len - num;
		ushort[] array = new ushort[num2];
		ushort[] array2 = new ushort[num];
		Array.Copy(buf, off, array, 0, num2);
		Array.Copy(buf, off + num2, array2, 0, num);
		return EncFF3_1(cipher, radix, tweak64, len, num, num2, array, array2);
	}

	private static ushort[] EncFF3_1(IBlockCipher cipher, int radix, byte[] t, int n, int v, int u, ushort[] a, ushort[] b)
	{
		BigInteger bigInteger = BigInteger.ValueOf(radix);
		BigInteger[] array = CalculateModUV(bigInteger, v, u);
		int num = v;
		Array.Reverse((Array)a);
		Array.Reverse((Array)b);
		for (uint num2 = 0u; num2 < 8; num2++)
		{
			num = n - num;
			BigInteger m = array[1 - (num2 & 1)];
			int wOff = (int)(4 - (num2 & 1) * 4);
			BigInteger value = CalculateY_FF3(cipher, bigInteger, t, wOff, num2, b);
			BigInteger x = Num(bigInteger, a).Add(value).Mod(m);
			ushort[] array2 = a;
			a = b;
			b = array2;
			Str(bigInteger, x, num, array2, 0);
		}
		Array.Reverse((Array)a);
		Array.Reverse((Array)b);
		return Arrays.Concatenate(a, b);
	}

	private static BigInteger Num(BigInteger R, ushort[] x)
	{
		BigInteger bigInteger = BigInteger.Zero;
		for (int i = 0; i < x.Length; i++)
		{
			bigInteger = bigInteger.Multiply(R).Add(BigInteger.ValueOf(x[i] & 0xFFFF));
		}
		return bigInteger;
	}

	private static byte[] Prf(IBlockCipher c, byte[] x)
	{
		if (x.Length % BLOCK_SIZE != 0)
		{
			throw new ArgumentException();
		}
		int num = x.Length / BLOCK_SIZE;
		byte[] array = new byte[BLOCK_SIZE];
		for (int i = 0; i < num; i++)
		{
			Bytes.XorTo(BLOCK_SIZE, x, i * BLOCK_SIZE, array, 0);
			c.ProcessBlock(array, 0, array, 0);
		}
		return array;
	}

	private static void Str(BigInteger R, BigInteger x, int m, ushort[] output, int off)
	{
		if (x.SignValue < 0)
		{
			throw new ArgumentException();
		}
		for (int i = 1; i <= m; i++)
		{
			BigInteger[] array = x.DivideAndRemainder(R);
			output[off + m - i] = (ushort)array[1].IntValue;
			x = array[0];
		}
		if (x.SignValue != 0)
		{
			throw new ArgumentException();
		}
	}

	private static byte[] ToByte(ushort[] buf)
	{
		byte[] array = new byte[buf.Length];
		for (int i = 0; i != array.Length; i++)
		{
			array[i] = (byte)buf[i];
		}
		return array;
	}

	private static ushort[] ToShort(byte[] buf, int off, int len)
	{
		ushort[] array = new ushort[len];
		for (int i = 0; i != array.Length; i++)
		{
			array[i] = (ushort)(buf[off + i] & 0xFF);
		}
		return array;
	}
}
