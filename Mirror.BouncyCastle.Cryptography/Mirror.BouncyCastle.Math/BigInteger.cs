using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math;

[Serializable]
public sealed class BigInteger : IComparable, IComparable<BigInteger>, IEquatable<BigInteger>
{
	internal static readonly int[][] primeLists;

	internal static readonly int[] primeProducts;

	private const long IMASK = 4294967295L;

	private const ulong UIMASK = 4294967295uL;

	private static readonly uint[] ZeroMagnitude;

	private static readonly byte[] ZeroEncoding;

	private static readonly BigInteger[] SMALL_CONSTANTS;

	public static readonly BigInteger Zero;

	public static readonly BigInteger One;

	public static readonly BigInteger Two;

	public static readonly BigInteger Three;

	public static readonly BigInteger Four;

	public static readonly BigInteger Five;

	public static readonly BigInteger Six;

	public static readonly BigInteger Ten;

	private static readonly byte[] BitLengthTable;

	private const int chunk2 = 1;

	private const int chunk8 = 1;

	private const int chunk10 = 19;

	private const int chunk16 = 16;

	private static readonly BigInteger radix2;

	private static readonly BigInteger radix2E;

	private static readonly BigInteger radix8;

	private static readonly BigInteger radix8E;

	private static readonly BigInteger radix10;

	private static readonly BigInteger radix10E;

	private static readonly BigInteger radix16;

	private static readonly BigInteger radix16E;

	private static readonly int[] ExpWindowThresholds;

	private const int BitsPerByte = 8;

	private const int BitsPerInt = 32;

	private const int BytesPerInt = 4;

	private readonly uint[] magnitude;

	private readonly int sign;

	[NonSerialized]
	private int nBits = -1;

	[NonSerialized]
	private int nBitLength = -1;

	public int BitCount
	{
		get
		{
			if (nBits == -1)
			{
				if (sign < 0)
				{
					nBits = Not().BitCount;
				}
				else
				{
					int num = 0;
					for (int i = 0; i < magnitude.Length; i++)
					{
						num += Integers.PopCount(magnitude[i]);
					}
					nBits = num;
				}
			}
			return nBits;
		}
	}

	public int BitLength
	{
		get
		{
			if (nBitLength == -1)
			{
				nBitLength = ((sign != 0) ? CalcBitLength(sign, 0, magnitude) : 0);
			}
			return nBitLength;
		}
	}

	public int IntValue
	{
		get
		{
			if (sign == 0)
			{
				return 0;
			}
			int num = magnitude.Length;
			int num2 = (int)magnitude[num - 1];
			if (sign >= 0)
			{
				return num2;
			}
			return -num2;
		}
	}

	public int IntValueExact
	{
		get
		{
			if (BitLength > 31)
			{
				throw new ArithmeticException("BigInteger out of int range");
			}
			return IntValue;
		}
	}

	public long LongValue
	{
		get
		{
			if (sign == 0)
			{
				return 0L;
			}
			int num = magnitude.Length;
			long num2 = (long)magnitude[num - 1] & 0xFFFFFFFFL;
			if (num > 1)
			{
				num2 |= (long)(((ulong)magnitude[num - 2] & 0xFFFFFFFFuL) << 32);
			}
			if (sign >= 0)
			{
				return num2;
			}
			return -num2;
		}
	}

	public long LongValueExact
	{
		get
		{
			if (BitLength > 63)
			{
				throw new ArithmeticException("BigInteger out of long range");
			}
			return LongValue;
		}
	}

	public int SignValue => sign;

	static BigInteger()
	{
		primeLists = new int[64][]
		{
			new int[8] { 3, 5, 7, 11, 13, 17, 19, 23 },
			new int[5] { 29, 31, 37, 41, 43 },
			new int[5] { 47, 53, 59, 61, 67 },
			new int[4] { 71, 73, 79, 83 },
			new int[4] { 89, 97, 101, 103 },
			new int[4] { 107, 109, 113, 127 },
			new int[4] { 131, 137, 139, 149 },
			new int[4] { 151, 157, 163, 167 },
			new int[4] { 173, 179, 181, 191 },
			new int[4] { 193, 197, 199, 211 },
			new int[3] { 223, 227, 229 },
			new int[3] { 233, 239, 241 },
			new int[3] { 251, 257, 263 },
			new int[3] { 269, 271, 277 },
			new int[3] { 281, 283, 293 },
			new int[3] { 307, 311, 313 },
			new int[3] { 317, 331, 337 },
			new int[3] { 347, 349, 353 },
			new int[3] { 359, 367, 373 },
			new int[3] { 379, 383, 389 },
			new int[3] { 397, 401, 409 },
			new int[3] { 419, 421, 431 },
			new int[3] { 433, 439, 443 },
			new int[3] { 449, 457, 461 },
			new int[3] { 463, 467, 479 },
			new int[3] { 487, 491, 499 },
			new int[3] { 503, 509, 521 },
			new int[3] { 523, 541, 547 },
			new int[3] { 557, 563, 569 },
			new int[3] { 571, 577, 587 },
			new int[3] { 593, 599, 601 },
			new int[3] { 607, 613, 617 },
			new int[3] { 619, 631, 641 },
			new int[3] { 643, 647, 653 },
			new int[3] { 659, 661, 673 },
			new int[3] { 677, 683, 691 },
			new int[3] { 701, 709, 719 },
			new int[3] { 727, 733, 739 },
			new int[3] { 743, 751, 757 },
			new int[3] { 761, 769, 773 },
			new int[3] { 787, 797, 809 },
			new int[3] { 811, 821, 823 },
			new int[3] { 827, 829, 839 },
			new int[3] { 853, 857, 859 },
			new int[3] { 863, 877, 881 },
			new int[3] { 883, 887, 907 },
			new int[3] { 911, 919, 929 },
			new int[3] { 937, 941, 947 },
			new int[3] { 953, 967, 971 },
			new int[3] { 977, 983, 991 },
			new int[3] { 997, 1009, 1013 },
			new int[3] { 1019, 1021, 1031 },
			new int[3] { 1033, 1039, 1049 },
			new int[3] { 1051, 1061, 1063 },
			new int[3] { 1069, 1087, 1091 },
			new int[3] { 1093, 1097, 1103 },
			new int[3] { 1109, 1117, 1123 },
			new int[3] { 1129, 1151, 1153 },
			new int[3] { 1163, 1171, 1181 },
			new int[3] { 1187, 1193, 1201 },
			new int[3] { 1213, 1217, 1223 },
			new int[3] { 1229, 1231, 1237 },
			new int[3] { 1249, 1259, 1277 },
			new int[3] { 1279, 1283, 1289 }
		};
		ZeroMagnitude = new uint[0];
		ZeroEncoding = new byte[0];
		SMALL_CONSTANTS = new BigInteger[17];
		BitLengthTable = new byte[256]
		{
			0, 1, 2, 2, 3, 3, 3, 3, 4, 4,
			4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
			6, 6, 6, 6, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7, 7, 7, 7, 7, 7, 7, 7, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8
		};
		ExpWindowThresholds = new int[8] { 7, 25, 81, 241, 673, 1793, 4609, 2147483647 };
		Zero = new BigInteger(0, ZeroMagnitude, checkMag: false);
		Zero.nBits = 0;
		Zero.nBitLength = 0;
		SMALL_CONSTANTS[0] = Zero;
		for (uint num = 1u; num < SMALL_CONSTANTS.Length; num++)
		{
			BigInteger bigInteger = CreateUValueOf(num);
			bigInteger.nBits = Integers.PopCount(num);
			bigInteger.nBitLength = BitLen(num);
			SMALL_CONSTANTS[num] = bigInteger;
		}
		One = SMALL_CONSTANTS[1];
		Two = SMALL_CONSTANTS[2];
		Three = SMALL_CONSTANTS[3];
		Four = SMALL_CONSTANTS[4];
		Five = SMALL_CONSTANTS[5];
		Six = SMALL_CONSTANTS[6];
		Ten = SMALL_CONSTANTS[10];
		radix2 = Two;
		radix2E = radix2.Pow(1);
		radix8 = ValueOf(8);
		radix8E = radix8.Pow(1);
		radix10 = Ten;
		radix10E = radix10.Pow(19);
		radix16 = ValueOf(16);
		radix16E = radix16.Pow(16);
		primeProducts = new int[primeLists.Length];
		for (int i = 0; i < primeLists.Length; i++)
		{
			int[] array = primeLists[i];
			int num2 = array[0];
			for (int j = 1; j < array.Length; j++)
			{
				num2 *= array[j];
			}
			primeProducts[i] = num2;
		}
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		nBits = -1;
		nBitLength = -1;
	}

	private static int GetBytesLength(int nBits)
	{
		return (nBits + 8 - 1) / 8;
	}

	public static BigInteger Arbitrary(int sizeInBits)
	{
		return new BigInteger(sizeInBits, SecureRandom.ArbitraryRandom);
	}

	private BigInteger(int signum, uint[] mag, bool checkMag)
	{
		if (!checkMag)
		{
			sign = signum;
			magnitude = mag;
			return;
		}
		int i;
		for (i = 0; i < mag.Length && mag[i] == 0; i++)
		{
		}
		if (i == mag.Length)
		{
			sign = 0;
			magnitude = ZeroMagnitude;
			return;
		}
		sign = signum;
		if (i == 0)
		{
			magnitude = mag;
			return;
		}
		magnitude = new uint[mag.Length - i];
		Array.Copy(mag, i, magnitude, 0, magnitude.Length);
	}

	public BigInteger(string value)
		: this(value, 10)
	{
	}

	public BigInteger(string str, int radix)
	{
		if (str.Length == 0)
		{
			throw new FormatException("Zero length BigInteger");
		}
		NumberStyles style;
		int num;
		BigInteger bigInteger;
		BigInteger val;
		switch (radix)
		{
		case 2:
			style = NumberStyles.Integer;
			num = 1;
			bigInteger = radix2;
			val = radix2E;
			break;
		case 8:
			style = NumberStyles.Integer;
			num = 1;
			bigInteger = radix8;
			val = radix8E;
			break;
		case 10:
			style = NumberStyles.Integer;
			num = 19;
			bigInteger = radix10;
			val = radix10E;
			break;
		case 16:
			style = NumberStyles.AllowHexSpecifier;
			num = 16;
			bigInteger = radix16;
			val = radix16E;
			break;
		default:
			throw new FormatException("Only bases 2, 8, 10, or 16 allowed");
		}
		int i = 0;
		sign = 1;
		if (str[0] == '-')
		{
			if (str.Length == 1)
			{
				throw new FormatException("Zero length BigInteger");
			}
			sign = -1;
			i = 1;
		}
		for (; i < str.Length && int.Parse(str[i].ToString(), style) == 0; i++)
		{
		}
		if (i >= str.Length)
		{
			sign = 0;
			magnitude = ZeroMagnitude;
			return;
		}
		BigInteger bigInteger2 = Zero;
		int num2 = i + num;
		if (num2 <= str.Length)
		{
			do
			{
				string text = str.Substring(i, num);
				ulong num3 = ulong.Parse(text, style);
				BigInteger value = CreateUValueOf(num3);
				switch (radix)
				{
				case 2:
					if (num3 >= 2)
					{
						throw new FormatException("Bad character in radix 2 string: " + text);
					}
					bigInteger2 = bigInteger2.ShiftLeft(1);
					break;
				case 8:
					if (num3 >= 8)
					{
						throw new FormatException("Bad character in radix 8 string: " + text);
					}
					bigInteger2 = bigInteger2.ShiftLeft(3);
					break;
				case 16:
					bigInteger2 = bigInteger2.ShiftLeft(64);
					break;
				default:
					bigInteger2 = bigInteger2.Multiply(val);
					break;
				}
				bigInteger2 = bigInteger2.Add(value);
				i = num2;
				num2 += num;
			}
			while (num2 <= str.Length);
		}
		if (i < str.Length)
		{
			string text2 = str.Substring(i);
			BigInteger bigInteger3 = CreateUValueOf(ulong.Parse(text2, style));
			if (bigInteger2.sign > 0)
			{
				switch (radix)
				{
				case 16:
					bigInteger2 = bigInteger2.ShiftLeft(text2.Length << 2);
					break;
				default:
					bigInteger2 = bigInteger2.Multiply(bigInteger.Pow(text2.Length));
					break;
				case 2:
				case 8:
					break;
				}
				bigInteger2 = bigInteger2.Add(bigInteger3);
			}
			else
			{
				bigInteger2 = bigInteger3;
			}
		}
		magnitude = bigInteger2.magnitude;
	}

	public BigInteger(byte[] bytes)
	{
		magnitude = InitBE(bytes, 0, bytes.Length, out sign);
	}

	public BigInteger(byte[] bytes, bool bigEndian)
	{
		magnitude = (bigEndian ? InitBE(bytes, 0, bytes.Length, out sign) : InitLE(bytes, 0, bytes.Length, out sign));
	}

	public BigInteger(byte[] bytes, int offset, int length)
	{
		if (length == 0)
		{
			throw new FormatException("Zero length BigInteger");
		}
		magnitude = InitBE(bytes, offset, length, out sign);
	}

	public BigInteger(byte[] bytes, int offset, int length, bool bigEndian)
	{
		if (length <= 0)
		{
			throw new FormatException("Zero length BigInteger");
		}
		magnitude = (bigEndian ? InitBE(bytes, offset, length, out sign) : InitLE(bytes, offset, length, out sign));
	}

	private static uint[] InitBE(byte[] bytes, int offset, int length, out int sign)
	{
		if ((sbyte)bytes[offset] >= 0)
		{
			uint[] array = MakeMagnitudeBE(bytes, offset, length);
			sign = ((array.Length != 0) ? 1 : 0);
			return array;
		}
		sign = -1;
		int num = offset + length;
		int i;
		for (i = offset; i < num && (sbyte)bytes[i] == -1; i++)
		{
		}
		if (i >= num)
		{
			return One.magnitude;
		}
		int num2 = num - i;
		byte[] array2 = new byte[num2];
		int num3 = 0;
		while (num3 < num2)
		{
			array2[num3++] = (byte)(~bytes[i++]);
		}
		while (array2[--num3] == byte.MaxValue)
		{
			array2[num3] = 0;
		}
		array2[num3]++;
		return MakeMagnitudeBE(array2);
	}

	private static uint[] InitLE(byte[] bytes, int offset, int length, out int sign)
	{
		int num = offset + length;
		if ((sbyte)bytes[num - 1] >= 0)
		{
			uint[] array = MakeMagnitudeLE(bytes, offset, length);
			sign = ((array.Length != 0) ? 1 : 0);
			return array;
		}
		sign = -1;
		int num2 = length;
		while (--num2 >= 0 && bytes[offset + num2] == byte.MaxValue)
		{
		}
		if (num2 < 0)
		{
			return One.magnitude;
		}
		int num3 = num2 + 1;
		byte[] array2 = new byte[num3];
		for (int i = 0; i < num3; i++)
		{
			array2[i] = (byte)(~bytes[offset + i]);
		}
		int num4 = 0;
		while (array2[num4] == byte.MaxValue)
		{
			array2[num4++] = 0;
		}
		array2[num4]++;
		return MakeMagnitudeLE(array2);
	}

	private static uint[] MakeMagnitudeBE(byte[] bytes)
	{
		return MakeMagnitudeBE(bytes, 0, bytes.Length);
	}

	private static uint[] MakeMagnitudeBE(byte[] bytes, int offset, int length)
	{
		int num = offset + length;
		int i;
		for (i = offset; i < num && bytes[i] == 0; i++)
		{
		}
		int num2 = num - i;
		if (num2 <= 0)
		{
			return ZeroMagnitude;
		}
		int num3 = (num2 + 4 - 1) / 4;
		uint[] array = new uint[num3];
		int num4 = (num2 - 1) % 4 + 1;
		array[0] = Pack.BE_To_UInt32_Low(bytes, i, num4);
		Pack.BE_To_UInt32(bytes, i + num4, array, 1, num3 - 1);
		return array;
	}

	private static uint[] MakeMagnitudeLE(byte[] bytes)
	{
		return MakeMagnitudeLE(bytes, 0, bytes.Length);
	}

	private static uint[] MakeMagnitudeLE(byte[] bytes, int offset, int length)
	{
		int num = length;
		while (--num >= 0 && bytes[offset + num] == 0)
		{
		}
		if (num < 0)
		{
			return ZeroMagnitude;
		}
		int num2 = (num + 4) / 4;
		uint[] array = new uint[num2];
		int num3 = num % 4;
		int len = num3 + 1;
		int num4 = offset + num - num3;
		array[0] = Pack.LE_To_UInt32_Low(bytes, num4, len);
		for (int i = 1; i < num2; i++)
		{
			num4 -= 4;
			array[i] = Pack.LE_To_UInt32(bytes, num4);
		}
		return array;
	}

	public BigInteger(int sign, byte[] bytes)
		: this(sign, bytes, 0, bytes.Length, bigEndian: true)
	{
	}

	public BigInteger(int sign, byte[] bytes, bool bigEndian)
		: this(sign, bytes, 0, bytes.Length, bigEndian)
	{
	}

	public BigInteger(int sign, byte[] bytes, int offset, int length)
		: this(sign, bytes, offset, length, bigEndian: true)
	{
	}

	public BigInteger(int sign, byte[] bytes, int offset, int length, bool bigEndian)
	{
		switch (sign)
		{
		default:
			throw new FormatException("Invalid sign value");
		case 0:
			this.sign = 0;
			magnitude = ZeroMagnitude;
			break;
		case -1:
		case 1:
			magnitude = (bigEndian ? MakeMagnitudeBE(bytes, offset, length) : MakeMagnitudeLE(bytes, offset, length));
			this.sign = ((magnitude.Length >= 1) ? sign : 0);
			break;
		}
	}

	public BigInteger(int sizeInBits, Random random)
	{
		if (sizeInBits < 0)
		{
			throw new ArgumentException("sizeInBits must be non-negative");
		}
		nBits = -1;
		nBitLength = -1;
		if (sizeInBits == 0)
		{
			sign = 0;
			magnitude = ZeroMagnitude;
			return;
		}
		int bytesLength = GetBytesLength(sizeInBits);
		byte[] array = new byte[bytesLength];
		random.NextBytes(array);
		int num = 8 * bytesLength - sizeInBits;
		array[0] &= (byte)(255 >>> num);
		magnitude = MakeMagnitudeBE(array);
		sign = ((magnitude.Length >= 1) ? 1 : 0);
	}

	public BigInteger(int bitLength, int certainty, Random random)
	{
		if (bitLength < 2)
		{
			throw new ArithmeticException("bitLength < 2");
		}
		sign = 1;
		nBitLength = bitLength;
		if (bitLength == 2)
		{
			magnitude = ((random.Next(2) == 0) ? Two.magnitude : Three.magnitude);
			return;
		}
		int bytesLength = GetBytesLength(bitLength);
		byte[] array = new byte[bytesLength];
		int num = 8 * bytesLength - bitLength;
		byte b = (byte)(255u >> num);
		byte b2 = (byte)(1 << 7 - num);
		while (true)
		{
			random.NextBytes(array);
			array[0] &= b;
			array[0] |= b2;
			array[bytesLength - 1] |= 1;
			magnitude = MakeMagnitudeBE(array);
			nBits = -1;
			if (certainty < 1 || CheckProbablePrime(certainty, random, randomlySelected: true))
			{
				break;
			}
			for (int i = 1; i < magnitude.Length - 1; i++)
			{
				magnitude[i] ^= (uint)random.Next();
				if (CheckProbablePrime(certainty, random, randomlySelected: true))
				{
					return;
				}
			}
		}
	}

	public BigInteger Abs()
	{
		if (sign < 0)
		{
			return Negate();
		}
		return this;
	}

	private static uint[] AddMagnitudes(uint[] a, uint[] b)
	{
		int num = a.Length - 1;
		int num2 = b.Length - 1;
		ulong num3 = 0uL;
		while (num2 >= 0)
		{
			num3 += (ulong)((long)a[num] + (long)b[num2--]);
			a[num--] = (uint)num3;
			num3 >>= 32;
		}
		if (num3 != 0L)
		{
			while (num >= 0 && ++a[num--] == 0)
			{
			}
		}
		return a;
	}

	public BigInteger Add(BigInteger value)
	{
		if (sign == 0)
		{
			return value;
		}
		if (sign == value.sign)
		{
			return AddToMagnitude(value.magnitude);
		}
		if (value.sign == 0)
		{
			return this;
		}
		if (value.sign < 0)
		{
			return Subtract(value.Negate());
		}
		return value.Subtract(Negate());
	}

	private BigInteger AddToMagnitude(uint[] magToAdd)
	{
		uint[] array;
		uint[] array2;
		if (magnitude.Length < magToAdd.Length)
		{
			array = magToAdd;
			array2 = magnitude;
		}
		else
		{
			array = magnitude;
			array2 = magToAdd;
		}
		uint num = uint.MaxValue;
		if (array.Length == array2.Length)
		{
			num -= array2[0];
		}
		bool flag = array[0] >= num;
		uint[] array3;
		if (flag)
		{
			array3 = new uint[array.Length + 1];
			array.CopyTo(array3, 1);
		}
		else
		{
			array3 = (uint[])array.Clone();
		}
		array3 = AddMagnitudes(array3, array2);
		return new BigInteger(sign, array3, flag);
	}

	public BigInteger And(BigInteger value)
	{
		if (sign == 0 || value.sign == 0)
		{
			return Zero;
		}
		uint[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
		uint[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
		bool flag = sign < 0 && value.sign < 0;
		uint[] array3 = new uint[System.Math.Max(array.Length, array2.Length)];
		int num = array3.Length - array.Length;
		int num2 = array3.Length - array2.Length;
		for (int i = 0; i < array3.Length; i++)
		{
			uint num3 = ((i >= num) ? array[i - num] : 0u);
			uint num4 = ((i >= num2) ? array2[i - num2] : 0u);
			if (sign < 0)
			{
				num3 = ~num3;
			}
			if (value.sign < 0)
			{
				num4 = ~num4;
			}
			array3[i] = num3 & num4;
			if (flag)
			{
				array3[i] = ~array3[i];
			}
		}
		BigInteger bigInteger = new BigInteger(1, array3, checkMag: true);
		if (flag)
		{
			bigInteger = bigInteger.Not();
		}
		return bigInteger;
	}

	public BigInteger AndNot(BigInteger val)
	{
		return And(val.Not());
	}

	private static int CalcBitLength(int sign, int indx, uint[] mag)
	{
		while (true)
		{
			if (indx >= mag.Length)
			{
				return 0;
			}
			if (mag[indx] != 0)
			{
				break;
			}
			indx++;
		}
		int num = 32 * (mag.Length - indx - 1);
		uint num2 = mag[indx];
		num += BitLen(num2);
		if (sign < 0 && (num2 & (0L - (long)num2)) == num2)
		{
			do
			{
				if (++indx >= mag.Length)
				{
					num--;
					break;
				}
			}
			while (mag[indx] == 0);
		}
		return num;
	}

	private static int BitLen(byte b)
	{
		return BitLengthTable[b];
	}

	private static int BitLen(uint v)
	{
		uint num = v >> 24;
		if (num != 0)
		{
			return 24 + BitLengthTable[num];
		}
		num = v >> 16;
		if (num != 0)
		{
			return 16 + BitLengthTable[num];
		}
		num = v >> 8;
		if (num != 0)
		{
			return 8 + BitLengthTable[num];
		}
		return BitLengthTable[v];
	}

	private bool QuickPow2Check()
	{
		if (sign > 0)
		{
			return nBits == 1;
		}
		return false;
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (!(obj is BigInteger other))
		{
			throw new ArgumentException("Object is not a BigInteger", "obj");
		}
		return CompareTo(other);
	}

	public int CompareTo(BigInteger other)
	{
		if (other == null)
		{
			return 1;
		}
		if (sign >= other.sign)
		{
			if (sign <= other.sign)
			{
				if (sign != 0)
				{
					return sign * CompareNoLeadingZeros(0, magnitude, 0, other.magnitude);
				}
				return 0;
			}
			return 1;
		}
		return -1;
	}

	private static int CompareTo(int xIndx, uint[] x, int yIndx, uint[] y)
	{
		while (xIndx != x.Length && x[xIndx] == 0)
		{
			xIndx++;
		}
		while (yIndx != y.Length && y[yIndx] == 0)
		{
			yIndx++;
		}
		return CompareNoLeadingZeros(xIndx, x, yIndx, y);
	}

	private static int CompareNoLeadingZeros(int xIndx, uint[] x, int yIndx, uint[] y)
	{
		int num = x.Length - y.Length - (xIndx - yIndx);
		if (num != 0)
		{
			if (num >= 0)
			{
				return 1;
			}
			return -1;
		}
		while (xIndx < x.Length)
		{
			uint num2 = x[xIndx++];
			uint num3 = y[yIndx++];
			if (num2 != num3)
			{
				if (num2 >= num3)
				{
					return 1;
				}
				return -1;
			}
		}
		return 0;
	}

	private uint[] Divide(uint[] x, uint[] y)
	{
		int i;
		for (i = 0; i < x.Length && x[i] == 0; i++)
		{
		}
		int j;
		for (j = 0; j < y.Length && y[j] == 0; j++)
		{
		}
		int num = CompareNoLeadingZeros(i, x, j, y);
		uint[] array3;
		if (num > 0)
		{
			int num2 = CalcBitLength(1, j, y);
			int num3 = CalcBitLength(1, i, x);
			int num4 = num3 - num2;
			int k = 0;
			int l = 0;
			int num5 = num2;
			uint[] array;
			uint[] array2;
			if (num4 > 0)
			{
				array = new uint[(num4 >> 5) + 1];
				array[0] = (uint)(1 << num4 % 32);
				array2 = ShiftLeft(y, num4);
				num5 += num4;
			}
			else
			{
				array = new uint[1] { 1u };
				int num6 = y.Length - j;
				array2 = new uint[num6];
				Array.Copy(y, j, array2, 0, num6);
			}
			array3 = new uint[array.Length];
			while (true)
			{
				if (num5 < num3 || CompareNoLeadingZeros(i, x, l, array2) >= 0)
				{
					Subtract(i, x, l, array2);
					AddMagnitudes(array3, array);
					while (x[i] == 0)
					{
						if (++i == x.Length)
						{
							return array3;
						}
					}
					num3 = 32 * (x.Length - i - 1) + BitLen(x[i]);
					if (num3 <= num2)
					{
						if (num3 < num2)
						{
							return array3;
						}
						num = CompareNoLeadingZeros(i, x, j, y);
						if (num <= 0)
						{
							break;
						}
					}
				}
				num4 = num5 - num3;
				if (num4 == 1)
				{
					uint num7 = array2[l] >> 1;
					uint num8 = x[i];
					if (num7 > num8)
					{
						num4++;
					}
				}
				if (num4 < 2)
				{
					ShiftRightOneInPlace(l, array2);
					num5--;
					ShiftRightOneInPlace(k, array);
				}
				else
				{
					ShiftRightInPlace(l, array2, num4);
					num5 -= num4;
					ShiftRightInPlace(k, array, num4);
				}
				for (; array2[l] == 0; l++)
				{
				}
				for (; array[k] == 0; k++)
				{
				}
			}
		}
		else
		{
			array3 = new uint[1];
		}
		if (num == 0)
		{
			AddMagnitudes(array3, One.magnitude);
			Array.Clear(x, i, x.Length - i);
		}
		return array3;
	}

	public BigInteger Divide(BigInteger val)
	{
		if (val.sign == 0)
		{
			throw new ArithmeticException("Division by zero error");
		}
		if (sign == 0)
		{
			return Zero;
		}
		if (val.QuickPow2Check())
		{
			BigInteger bigInteger = Abs().ShiftRight(val.Abs().BitLength - 1);
			if (val.sign != sign)
			{
				return bigInteger.Negate();
			}
			return bigInteger;
		}
		uint[] x = (uint[])magnitude.Clone();
		return new BigInteger(sign * val.sign, Divide(x, val.magnitude), checkMag: true);
	}

	public BigInteger[] DivideAndRemainder(BigInteger val)
	{
		if (val.sign == 0)
		{
			throw new ArithmeticException("Division by zero error");
		}
		BigInteger[] array = new BigInteger[2];
		if (sign == 0)
		{
			array[0] = Zero;
			array[1] = Zero;
		}
		else if (val.QuickPow2Check())
		{
			int n = val.Abs().BitLength - 1;
			BigInteger bigInteger = Abs().ShiftRight(n);
			uint[] mag = LastNBits(n);
			array[0] = ((val.sign == sign) ? bigInteger : bigInteger.Negate());
			array[1] = new BigInteger(sign, mag, checkMag: true);
		}
		else
		{
			uint[] array2 = (uint[])magnitude.Clone();
			uint[] mag2 = Divide(array2, val.magnitude);
			array[0] = new BigInteger(sign * val.sign, mag2, checkMag: true);
			array[1] = new BigInteger(sign, array2, checkMag: true);
		}
		return array;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is BigInteger bigInteger))
		{
			return false;
		}
		if (sign == bigInteger.sign)
		{
			return IsEqualMagnitude(bigInteger);
		}
		return false;
	}

	public bool Equals(BigInteger other)
	{
		if (other == this)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (sign == other.sign)
		{
			return IsEqualMagnitude(other);
		}
		return false;
	}

	private bool IsEqualMagnitude(BigInteger x)
	{
		if (magnitude.Length != x.magnitude.Length)
		{
			return false;
		}
		for (int i = 0; i < magnitude.Length; i++)
		{
			if (magnitude[i] != x.magnitude[i])
			{
				return false;
			}
		}
		return true;
	}

	public BigInteger Gcd(BigInteger value)
	{
		if (value.sign == 0)
		{
			return Abs();
		}
		if (sign == 0)
		{
			return value.Abs();
		}
		BigInteger bigInteger = this;
		BigInteger bigInteger2 = value;
		while (bigInteger2.sign != 0)
		{
			BigInteger bigInteger3 = bigInteger.Mod(bigInteger2);
			bigInteger = bigInteger2;
			bigInteger2 = bigInteger3;
		}
		return bigInteger;
	}

	public override int GetHashCode()
	{
		int num = magnitude.Length;
		if (magnitude.Length != 0)
		{
			num ^= (int)magnitude[0];
			if (magnitude.Length > 1)
			{
				num ^= (int)magnitude[magnitude.Length - 1];
			}
		}
		if (sign >= 0)
		{
			return num;
		}
		return ~num;
	}

	private BigInteger Inc()
	{
		if (sign == 0)
		{
			return One;
		}
		if (sign < 0)
		{
			return new BigInteger(-1, DoSubBigLil(magnitude, One.magnitude), checkMag: true);
		}
		return AddToMagnitude(One.magnitude);
	}

	public bool IsProbablePrime(int certainty)
	{
		return IsProbablePrime(certainty, randomlySelected: false);
	}

	internal bool IsProbablePrime(int certainty, bool randomlySelected)
	{
		if (certainty <= 0)
		{
			return true;
		}
		BigInteger bigInteger = Abs();
		if (!bigInteger.TestBit(0))
		{
			return bigInteger.Equals(Two);
		}
		if (bigInteger.Equals(One))
		{
			return false;
		}
		return bigInteger.CheckProbablePrime(certainty, SecureRandom.ArbitraryRandom, randomlySelected);
	}

	private bool CheckProbablePrime(int certainty, Random random, bool randomlySelected)
	{
		int num = System.Math.Min(BitLength - 1, primeLists.Length);
		for (int i = 0; i < num; i++)
		{
			int num2 = Remainder(primeProducts[i]);
			int[] array = primeLists[i];
			foreach (int num3 in array)
			{
				if (num2 % num3 == 0)
				{
					if (BitLength < 16)
					{
						return IntValue == num3;
					}
					return false;
				}
			}
		}
		return RabinMillerTest(certainty, random, randomlySelected);
	}

	public bool RabinMillerTest(int certainty, Random random)
	{
		return RabinMillerTest(certainty, random, randomlySelected: false);
	}

	internal bool RabinMillerTest(int certainty, Random random, bool randomlySelected)
	{
		int bitLength = BitLength;
		int num = (certainty - 1) / 2 + 1;
		if (randomlySelected)
		{
			int num2 = ((bitLength >= 1024) ? 4 : ((bitLength >= 512) ? 8 : ((bitLength >= 256) ? 16 : 50)));
			if (certainty < 100)
			{
				num = System.Math.Min(num2, num);
			}
			else
			{
				num -= 50;
				num += num2;
			}
		}
		int lowestSetBitMaskFirst = GetLowestSetBitMaskFirst(4294967294u);
		BigInteger e = ShiftRight(lowestSetBitMaskFirst);
		BigInteger bigInteger = One.ShiftLeft(32 * magnitude.Length).Remainder(this);
		BigInteger bigInteger2 = Subtract(bigInteger);
		uint[] yAccum = new uint[magnitude.Length + 1];
		while (true)
		{
			BigInteger bigInteger3 = new BigInteger(BitLength, random);
			if (bigInteger3.sign == 0 || bigInteger3.CompareTo(this) >= 0 || bigInteger3.IsEqualMagnitude(bigInteger) || bigInteger3.IsEqualMagnitude(bigInteger2))
			{
				continue;
			}
			BigInteger bigInteger4 = ModPowMonty(yAccum, bigInteger3, e, this, convert: false);
			if (!bigInteger4.Equals(bigInteger))
			{
				int num3 = 0;
				while (!bigInteger4.Equals(bigInteger2))
				{
					if (++num3 == lowestSetBitMaskFirst)
					{
						return false;
					}
					bigInteger4 = ModSquareMonty(yAccum, bigInteger4, this);
					if (bigInteger4.Equals(bigInteger))
					{
						return false;
					}
				}
			}
			if (--num <= 0)
			{
				break;
			}
		}
		return true;
	}

	public BigInteger Max(BigInteger value)
	{
		if (CompareTo(value) <= 0)
		{
			return value;
		}
		return this;
	}

	public BigInteger Min(BigInteger value)
	{
		if (CompareTo(value) >= 0)
		{
			return value;
		}
		return this;
	}

	public BigInteger Mod(BigInteger m)
	{
		if (m.sign < 1)
		{
			throw new ArithmeticException("Modulus must be positive");
		}
		BigInteger bigInteger = Remainder(m);
		if (bigInteger.sign < 0)
		{
			return bigInteger.Add(m);
		}
		return bigInteger;
	}

	public BigInteger ModDivide(BigInteger y, BigInteger m)
	{
		return ModMultiply(y.ModInverse(m), m);
	}

	public BigInteger ModInverse(BigInteger m)
	{
		if (m.sign < 1)
		{
			throw new ArithmeticException("Modulus must be positive");
		}
		if (m.QuickPow2Check())
		{
			return ModInversePow2(m);
		}
		if (!ExtEuclid(Remainder(m), m, out var u1Out).Equals(One))
		{
			throw new ArithmeticException("Numbers not relatively prime.");
		}
		if (u1Out.sign < 0)
		{
			return u1Out.Add(m);
		}
		return u1Out;
	}

	private BigInteger ModInversePow2(BigInteger m)
	{
		if (!TestBit(0))
		{
			throw new ArithmeticException("Numbers not relatively prime.");
		}
		int num = m.BitLength - 1;
		long num2 = (long)Mirror.BouncyCastle.Math.Raw.Mod.Inverse64((ulong)LongValue);
		if (num < 64)
		{
			num2 &= (1L << num) - 1;
		}
		BigInteger bigInteger = ValueOf(num2);
		if (num > 64)
		{
			BigInteger val = Remainder(m);
			int num3 = 64;
			do
			{
				BigInteger n = bigInteger.Multiply(val).Remainder(m);
				bigInteger = bigInteger.Multiply(Two.Subtract(n)).Remainder(m);
				num3 <<= 1;
			}
			while (num3 < num);
		}
		if (bigInteger.sign < 0)
		{
			bigInteger = bigInteger.Add(m);
		}
		return bigInteger;
	}

	private static BigInteger ExtEuclid(BigInteger a, BigInteger b, out BigInteger u1Out)
	{
		BigInteger bigInteger = One;
		BigInteger bigInteger2 = Zero;
		BigInteger bigInteger3 = a;
		BigInteger bigInteger4 = b;
		if (bigInteger4.sign > 0)
		{
			while (true)
			{
				BigInteger[] array = bigInteger3.DivideAndRemainder(bigInteger4);
				bigInteger3 = bigInteger4;
				bigInteger4 = array[1];
				BigInteger bigInteger5 = bigInteger;
				bigInteger = bigInteger2;
				if (bigInteger4.sign <= 0)
				{
					break;
				}
				bigInteger2 = bigInteger5.Subtract(bigInteger2.Multiply(array[0]));
			}
		}
		u1Out = bigInteger;
		return bigInteger3;
	}

	private static void ZeroOut(int[] x)
	{
		Array.Clear(x, 0, x.Length);
	}

	public BigInteger ModMultiply(BigInteger y, BigInteger m)
	{
		return Multiply(y).Mod(m);
	}

	public BigInteger ModSquare(BigInteger m)
	{
		return Square().Mod(m);
	}

	public BigInteger ModPow(BigInteger e, BigInteger m)
	{
		if (m.sign < 1)
		{
			throw new ArithmeticException("Modulus must be positive");
		}
		if (m.Equals(One))
		{
			return Zero;
		}
		if (e.sign == 0)
		{
			return One;
		}
		if (sign == 0)
		{
			return Zero;
		}
		bool num = e.sign < 0;
		if (num)
		{
			e = e.Negate();
		}
		BigInteger bigInteger = Mod(m);
		if (!e.Equals(One))
		{
			bigInteger = (((m.magnitude[m.magnitude.Length - 1] & 1) != 0) ? ModPowMonty(new uint[m.magnitude.Length + 1], bigInteger, e, m, convert: true) : ModPowBarrett(bigInteger, e, m));
		}
		if (num)
		{
			bigInteger = bigInteger.ModInverse(m);
		}
		return bigInteger;
	}

	private static BigInteger ModPowBarrett(BigInteger b, BigInteger e, BigInteger m)
	{
		int num = m.magnitude.Length;
		BigInteger mr = One.ShiftLeft(num + 1 << 5);
		BigInteger yu = One.ShiftLeft(num << 6).Divide(m);
		int i = 0;
		for (int bitLength = e.BitLength; bitLength > ExpWindowThresholds[i]; i++)
		{
		}
		int num2 = 1 << i;
		BigInteger[] array = new BigInteger[num2];
		array[0] = b;
		BigInteger bigInteger = ReduceBarrett(b.Square(), m, mr, yu);
		for (int j = 1; j < num2; j++)
		{
			array[j] = ReduceBarrett(array[j - 1].Multiply(bigInteger), m, mr, yu);
		}
		uint[] windowList = GetWindowList(e.magnitude, i);
		uint num3 = windowList[0];
		uint num4 = num3 & 0xFF;
		uint num5 = num3 >> 8;
		BigInteger bigInteger2;
		if (num4 == 1)
		{
			bigInteger2 = bigInteger;
			num5--;
		}
		else
		{
			bigInteger2 = array[num4 >> 1];
		}
		int num6 = 1;
		while ((num3 = windowList[num6++]) != uint.MaxValue)
		{
			num4 = num3 & 0xFF;
			int num7 = (int)num5 + BitLen((byte)num4);
			for (int k = 0; k < num7; k++)
			{
				bigInteger2 = ReduceBarrett(bigInteger2.Square(), m, mr, yu);
			}
			bigInteger2 = ReduceBarrett(bigInteger2.Multiply(array[num4 >> 1]), m, mr, yu);
			num5 = num3 >> 8;
		}
		for (int l = 0; l < num5; l++)
		{
			bigInteger2 = ReduceBarrett(bigInteger2.Square(), m, mr, yu);
		}
		return bigInteger2;
	}

	private static BigInteger ReduceBarrett(BigInteger x, BigInteger m, BigInteger mr, BigInteger yu)
	{
		int bitLength = x.BitLength;
		int bitLength2 = m.BitLength;
		if (bitLength < bitLength2)
		{
			return x;
		}
		if (bitLength - bitLength2 > 1)
		{
			int num = m.magnitude.Length;
			BigInteger bigInteger = x.DivideWords(num - 1).Multiply(yu).DivideWords(num + 1);
			BigInteger bigInteger2 = x.RemainderWords(num + 1);
			BigInteger n = bigInteger.Multiply(m).RemainderWords(num + 1);
			x = bigInteger2.Subtract(n);
			if (x.sign < 0)
			{
				x = x.Add(mr);
			}
		}
		while (x.CompareTo(m) >= 0)
		{
			x = x.Subtract(m);
		}
		return x;
	}

	private static BigInteger ModPowMonty(uint[] yAccum, BigInteger b, BigInteger e, BigInteger m, bool convert)
	{
		int num = m.magnitude.Length;
		int num2 = 32 * num;
		bool flag = m.BitLength + 2 <= num2;
		uint mQuote = m.GetMQuote();
		if (convert)
		{
			b = b.ShiftLeft(num2).Remainder(m);
		}
		uint[] array = b.magnitude;
		if (array.Length < num)
		{
			uint[] array2 = new uint[num];
			array.CopyTo(array2, num - array.Length);
			array = array2;
		}
		int i = 0;
		if (e.magnitude.Length > 1 || e.BitCount > 2)
		{
			for (int bitLength = e.BitLength; bitLength > ExpWindowThresholds[i]; i++)
			{
			}
		}
		int num3 = 1 << i;
		uint[][] array3 = new uint[num3][];
		array3[0] = array;
		uint[] array4 = Arrays.Clone(array);
		SquareMonty(yAccum, array4, m.magnitude, mQuote, flag);
		for (int j = 1; j < num3; j++)
		{
			array3[j] = Arrays.Clone(array3[j - 1]);
			MultiplyMonty(yAccum, array3[j], array4, m.magnitude, mQuote, flag);
		}
		uint[] windowList = GetWindowList(e.magnitude, i);
		uint num4 = windowList[0];
		uint num5 = num4 & 0xFF;
		uint num6 = num4 >> 8;
		uint[] array5;
		if (num5 == 1)
		{
			array5 = array4;
			num6--;
		}
		else
		{
			array5 = Arrays.Clone(array3[num5 >> 1]);
		}
		int num7 = 1;
		while ((num4 = windowList[num7++]) != uint.MaxValue)
		{
			num5 = num4 & 0xFF;
			int num8 = (int)num6 + BitLen((byte)num5);
			for (int k = 0; k < num8; k++)
			{
				SquareMonty(yAccum, array5, m.magnitude, mQuote, flag);
			}
			MultiplyMonty(yAccum, array5, array3[num5 >> 1], m.magnitude, mQuote, flag);
			num6 = num4 >> 8;
		}
		for (int l = 0; l < num6; l++)
		{
			SquareMonty(yAccum, array5, m.magnitude, mQuote, flag);
		}
		if (convert)
		{
			MontgomeryReduce(array5, m.magnitude, mQuote);
		}
		else if (flag && CompareTo(0, array5, 0, m.magnitude) >= 0)
		{
			Subtract(0, array5, 0, m.magnitude);
		}
		return new BigInteger(1, array5, checkMag: true);
	}

	private static BigInteger ModSquareMonty(uint[] yAccum, BigInteger b, BigInteger m)
	{
		int num = m.magnitude.Length;
		int num2 = 32 * num;
		bool flag = m.BitLength + 2 <= num2;
		uint mQuote = m.GetMQuote();
		uint[] array = b.magnitude;
		uint[] array2 = new uint[num];
		array.CopyTo(array2, num - array.Length);
		SquareMonty(yAccum, array2, m.magnitude, mQuote, flag);
		if (flag && CompareTo(0, array2, 0, m.magnitude) >= 0)
		{
			Subtract(0, array2, 0, m.magnitude);
		}
		return new BigInteger(1, array2, checkMag: true);
	}

	private static uint[] GetWindowList(uint[] mag, int extraBits)
	{
		uint num = mag[0];
		int num2 = BitLen(num);
		uint[] array = new uint[((mag.Length - 1 << 5) + num2 + extraBits) / (1 + extraBits) + 1];
		int num3 = 0;
		int num4 = 33 - num2;
		num <<= num4;
		uint num5 = 1u;
		uint num6 = (uint)(1 << extraBits);
		uint num7 = 0u;
		int num8 = 0;
		while (true)
		{
			if (num4 < 32)
			{
				if (num5 < num6)
				{
					num5 = (num5 << 1) | (num >> 31);
				}
				else if ((int)num < 0)
				{
					array[num3++] = CreateWindowEntry(num5, num7);
					num5 = 1u;
					num7 = 0u;
				}
				else
				{
					num7++;
				}
				num <<= 1;
				num4++;
			}
			else
			{
				if (++num8 == mag.Length)
				{
					break;
				}
				num = mag[num8];
				num4 = 0;
			}
		}
		array[num3++] = CreateWindowEntry(num5, num7);
		array[num3] = uint.MaxValue;
		return array;
	}

	private static uint CreateWindowEntry(uint mult, uint zeros)
	{
		while ((mult & 1) == 0)
		{
			mult >>= 1;
			zeros++;
		}
		return mult | (zeros << 8);
	}

	private static uint[] Square(uint[] w, uint[] x)
	{
		int num = w.Length - 1;
		ulong num4;
		for (int num2 = x.Length - 1; num2 > 0; num2--)
		{
			ulong num3 = x[num2];
			num4 = num3 * num3 + w[num];
			w[num] = (uint)num4;
			num4 >>= 32;
			for (int num5 = num2 - 1; num5 >= 0; num5--)
			{
				ulong num6 = num3 * x[num5];
				num4 += ((ulong)w[--num] & 0xFFFFFFFFuL) + (uint)((int)num6 << 1);
				w[num] = (uint)num4;
				num4 = (num4 >> 32) + (num6 >> 31);
			}
			num4 += w[--num];
			w[num] = (uint)num4;
			if (--num >= 0)
			{
				w[num] = (uint)(num4 >> 32);
			}
			num += num2;
		}
		num4 = x[0];
		num4 = num4 * num4 + w[num];
		w[num] = (uint)num4;
		if (--num >= 0)
		{
			w[num] += (uint)(int)(num4 >> 32);
		}
		return w;
	}

	private static uint[] Multiply(uint[] x, uint[] y, uint[] z)
	{
		int num = z.Length;
		if (num < 1)
		{
			return x;
		}
		int num2 = x.Length - y.Length;
		do
		{
			long num3 = (long)z[--num] & 0xFFFFFFFFL;
			long num4 = 0L;
			if (num3 != 0L)
			{
				for (int num5 = y.Length - 1; num5 >= 0; num5--)
				{
					num4 += num3 * (long)((ulong)y[num5] & 0xFFFFFFFFuL) + (long)((ulong)x[num2 + num5] & 0xFFFFFFFFuL);
					x[num2 + num5] = (uint)num4;
					num4 >>>= 32;
				}
			}
			num2--;
			if (num2 >= 0)
			{
				x[num2] = (uint)num4;
			}
		}
		while (num > 0);
		return x;
	}

	private uint GetMQuote()
	{
		return Mirror.BouncyCastle.Math.Raw.Mod.Inverse32(0 - magnitude[magnitude.Length - 1]);
	}

	private static void MontgomeryReduce(uint[] x, uint[] m, uint mDash)
	{
		int num = m.Length;
		for (int num2 = num - 1; num2 >= 0; num2--)
		{
			uint num3 = x[num - 1];
			ulong num4 = num3 * mDash;
			ulong num5 = num4 * m[num - 1] + num3;
			num5 >>= 32;
			for (int num6 = num - 2; num6 >= 0; num6--)
			{
				num5 += num4 * m[num6] + x[num6];
				x[num6 + 1] = (uint)num5;
				num5 >>= 32;
			}
			x[0] = (uint)num5;
		}
		if (CompareTo(0, x, 0, m) >= 0)
		{
			Subtract(0, x, 0, m);
		}
	}

	private static void MultiplyMonty(uint[] a, uint[] x, uint[] y, uint[] m, uint mDash, bool smallMontyModulus)
	{
		int num = m.Length;
		if (num == 1)
		{
			x[0] = MultiplyMontyNIsOne(x[0], y[0], m[0], mDash);
			return;
		}
		uint num2 = y[num - 1];
		ulong num3 = x[num - 1];
		ulong num4 = num3 * num2;
		ulong num5 = (uint)(int)num4 * mDash;
		ulong num6 = num5 * m[num - 1];
		num4 += (uint)num6;
		num4 = (num4 >> 32) + (num6 >> 32);
		for (int num7 = num - 2; num7 >= 0; num7--)
		{
			ulong num8 = num3 * y[num7];
			num6 = num5 * m[num7];
			num4 += (num8 & 0xFFFFFFFFu) + (uint)num6;
			a[num7 + 2] = (uint)num4;
			num4 = (num4 >> 32) + (num8 >> 32) + (num6 >> 32);
		}
		a[1] = (uint)num4;
		uint num9 = (uint)(num4 >> 32);
		for (int num10 = num - 2; num10 >= 0; num10--)
		{
			uint num11 = a[num];
			ulong num12 = x[num10];
			ulong num13 = num12 * num2;
			ulong num14 = (num13 & 0xFFFFFFFFu) + num11;
			ulong num15 = (uint)(int)num14 * mDash;
			ulong num16 = num15 * m[num - 1];
			num14 += (uint)num16;
			num14 = (num14 >> 32) + (num13 >> 32) + (num16 >> 32);
			for (int num17 = num - 2; num17 >= 0; num17--)
			{
				num13 = num12 * y[num17];
				num16 = num15 * m[num17];
				num14 += (num13 & 0xFFFFFFFFu) + (uint)num16 + a[num17 + 1];
				a[num17 + 2] = (uint)num14;
				num14 = (num14 >> 32) + (num13 >> 32) + (num16 >> 32);
			}
			num14 += num9;
			a[1] = (uint)num14;
			num9 = (uint)(num14 >> 32);
		}
		a[0] = num9;
		if (!smallMontyModulus && CompareTo(0, a, 0, m) >= 0)
		{
			Subtract(0, a, 0, m);
		}
		Array.Copy(a, 1, x, 0, num);
	}

	private static void SquareMonty(uint[] a, uint[] x, uint[] m, uint mDash, bool smallMontyModulus)
	{
		int num = m.Length;
		if (num == 1)
		{
			uint num2 = x[0];
			x[0] = MultiplyMontyNIsOne(num2, num2, m[0], mDash);
			return;
		}
		ulong num3 = x[num - 1];
		ulong num4 = num3 * num3;
		ulong num5 = (uint)(int)num4 * mDash;
		ulong num6 = num5 * m[num - 1];
		num4 += (uint)num6;
		num4 = (num4 >> 32) + (num6 >> 32);
		for (int num7 = num - 2; num7 >= 0; num7--)
		{
			ulong num8 = num3 * x[num7];
			num6 = num5 * m[num7];
			num4 += (num6 & 0xFFFFFFFFu) + (uint)((int)num8 << 1);
			a[num7 + 2] = (uint)num4;
			num4 = (num4 >> 32) + (num8 >> 31) + (num6 >> 32);
		}
		a[1] = (uint)num4;
		uint num9 = (uint)(num4 >> 32);
		for (int num10 = num - 2; num10 >= 0; num10--)
		{
			uint num11 = a[num];
			ulong num12 = num11 * mDash;
			ulong num13 = num12 * m[num - 1] + num11;
			num13 >>= 32;
			for (int num14 = num - 2; num14 > num10; num14--)
			{
				num13 += num12 * m[num14] + a[num14 + 1];
				a[num14 + 2] = (uint)num13;
				num13 >>= 32;
			}
			ulong num15 = x[num10];
			ulong num16 = num15 * num15;
			ulong num17 = num12 * m[num10];
			num13 += (num16 & 0xFFFFFFFFu) + (uint)num17 + a[num10 + 1];
			a[num10 + 2] = (uint)num13;
			num13 = (num13 >> 32) + (num16 >> 32) + (num17 >> 32);
			for (int num18 = num10 - 1; num18 >= 0; num18--)
			{
				ulong num19 = num15 * x[num18];
				ulong num20 = num12 * m[num18];
				num13 += (num20 & 0xFFFFFFFFu) + (uint)((int)num19 << 1) + a[num18 + 1];
				a[num18 + 2] = (uint)num13;
				num13 = (num13 >> 32) + (num19 >> 31) + (num20 >> 32);
			}
			num13 += num9;
			a[1] = (uint)num13;
			num9 = (uint)(num13 >> 32);
		}
		a[0] = num9;
		if (!smallMontyModulus && CompareTo(0, a, 0, m) >= 0)
		{
			Subtract(0, a, 0, m);
		}
		Array.Copy(a, 1, x, 0, num);
	}

	private static uint MultiplyMontyNIsOne(uint x, uint y, uint m, uint mDash)
	{
		ulong num = (ulong)x * (ulong)y;
		uint num2 = (uint)(int)num * mDash;
		ulong num3 = m;
		ulong num4 = num3 * num2;
		num += (uint)num4;
		num = (num >> 32) + (num4 >> 32);
		if (num > num3)
		{
			num -= num3;
		}
		return (uint)num;
	}

	public BigInteger Multiply(BigInteger val)
	{
		if (val == this)
		{
			return Square();
		}
		if ((sign & val.sign) == 0)
		{
			return Zero;
		}
		if (val.QuickPow2Check())
		{
			BigInteger bigInteger = ShiftLeft(val.Abs().BitLength - 1);
			if (val.sign <= 0)
			{
				return bigInteger.Negate();
			}
			return bigInteger;
		}
		if (QuickPow2Check())
		{
			BigInteger bigInteger2 = val.ShiftLeft(Abs().BitLength - 1);
			if (sign <= 0)
			{
				return bigInteger2.Negate();
			}
			return bigInteger2;
		}
		uint[] array = new uint[magnitude.Length + val.magnitude.Length];
		Multiply(array, magnitude, val.magnitude);
		return new BigInteger(sign ^ val.sign ^ 1, array, checkMag: true);
	}

	public BigInteger Square()
	{
		if (sign == 0)
		{
			return Zero;
		}
		if (QuickPow2Check())
		{
			return ShiftLeft(Abs().BitLength - 1);
		}
		int num = magnitude.Length << 1;
		if (magnitude[0] >> 16 == 0)
		{
			num--;
		}
		uint[] array = new uint[num];
		Square(array, magnitude);
		return new BigInteger(1, array, checkMag: false);
	}

	public BigInteger Negate()
	{
		if (sign == 0)
		{
			return this;
		}
		return new BigInteger(-sign, magnitude, checkMag: false);
	}

	public BigInteger NextProbablePrime()
	{
		if (sign < 0)
		{
			throw new ArithmeticException("Cannot be called on value < 0");
		}
		if (CompareTo(Two) < 0)
		{
			return Two;
		}
		BigInteger bigInteger = Inc().SetBit(0);
		while (!bigInteger.CheckProbablePrime(100, SecureRandom.ArbitraryRandom, randomlySelected: false))
		{
			bigInteger = bigInteger.Add(Two);
		}
		return bigInteger;
	}

	public BigInteger Not()
	{
		return Inc().Negate();
	}

	public BigInteger Pow(int exp)
	{
		if (exp <= 0)
		{
			if (exp < 0)
			{
				throw new ArithmeticException("Negative exponent");
			}
			return One;
		}
		if (sign == 0)
		{
			return this;
		}
		if (QuickPow2Check())
		{
			long num = (long)exp * (long)(BitLength - 1);
			if (num > int.MaxValue)
			{
				throw new ArithmeticException("Result too large");
			}
			return One.ShiftLeft((int)num);
		}
		BigInteger bigInteger = One;
		BigInteger bigInteger2 = this;
		while (true)
		{
			if ((exp & 1) == 1)
			{
				bigInteger = bigInteger.Multiply(bigInteger2);
			}
			exp >>= 1;
			if (exp == 0)
			{
				break;
			}
			bigInteger2 = bigInteger2.Multiply(bigInteger2);
		}
		return bigInteger;
	}

	public static BigInteger ProbablePrime(int bitLength, Random random)
	{
		return new BigInteger(bitLength, 100, random);
	}

	private int Remainder(int m)
	{
		long num = 0L;
		for (int i = 0; i < magnitude.Length; i++)
		{
			long num2 = magnitude[i];
			num = ((num << 32) | num2) % m;
		}
		return (int)num;
	}

	private static uint[] Remainder(uint[] x, uint[] y)
	{
		int i;
		for (i = 0; i < x.Length && x[i] == 0; i++)
		{
		}
		int j;
		for (j = 0; j < y.Length && y[j] == 0; j++)
		{
		}
		int num = CompareNoLeadingZeros(i, x, j, y);
		if (num > 0)
		{
			int num2 = CalcBitLength(1, j, y);
			int num3 = CalcBitLength(1, i, x);
			int num4 = num3 - num2;
			int k = 0;
			int num5 = num2;
			uint[] array;
			if (num4 > 0)
			{
				array = ShiftLeft(y, num4);
				num5 += num4;
			}
			else
			{
				int num6 = y.Length - j;
				array = new uint[num6];
				Array.Copy(y, j, array, 0, num6);
			}
			while (true)
			{
				if (num5 < num3 || CompareNoLeadingZeros(i, x, k, array) >= 0)
				{
					Subtract(i, x, k, array);
					while (x[i] == 0)
					{
						if (++i == x.Length)
						{
							return x;
						}
					}
					num3 = 32 * (x.Length - i - 1) + BitLen(x[i]);
					if (num3 <= num2)
					{
						if (num3 < num2)
						{
							return x;
						}
						num = CompareNoLeadingZeros(i, x, j, y);
						if (num <= 0)
						{
							break;
						}
					}
				}
				num4 = num5 - num3;
				if (num4 == 1)
				{
					uint num7 = array[k] >> 1;
					uint num8 = x[i];
					if (num7 > num8)
					{
						num4++;
					}
				}
				if (num4 < 2)
				{
					ShiftRightOneInPlace(k, array);
					num5--;
				}
				else
				{
					ShiftRightInPlace(k, array, num4);
					num5 -= num4;
				}
				for (; array[k] == 0; k++)
				{
				}
			}
		}
		if (num == 0)
		{
			Array.Clear(x, i, x.Length - i);
		}
		return x;
	}

	public BigInteger Remainder(BigInteger n)
	{
		if (n.sign == 0)
		{
			throw new ArithmeticException("Division by zero error");
		}
		if (sign == 0)
		{
			return Zero;
		}
		if (n.magnitude.Length == 1)
		{
			int num = (int)n.magnitude[0];
			if (num > 0)
			{
				if (num == 1)
				{
					return Zero;
				}
				int num2 = Remainder(num);
				if (num2 != 0)
				{
					return new BigInteger(sign, new uint[1] { (uint)num2 }, checkMag: false);
				}
				return Zero;
			}
		}
		if (CompareNoLeadingZeros(0, magnitude, 0, n.magnitude) < 0)
		{
			return this;
		}
		uint[] mag;
		if (n.QuickPow2Check())
		{
			mag = LastNBits(n.Abs().BitLength - 1);
		}
		else
		{
			mag = (uint[])magnitude.Clone();
			mag = Remainder(mag, n.magnitude);
		}
		return new BigInteger(sign, mag, checkMag: true);
	}

	private uint[] LastNBits(int n)
	{
		if (n < 1)
		{
			return ZeroMagnitude;
		}
		int val = (n + 32 - 1) / 32;
		val = System.Math.Min(val, magnitude.Length);
		uint[] array = new uint[val];
		Array.Copy(magnitude, magnitude.Length - val, array, 0, val);
		int num = (val << 5) - n;
		if (num > 0)
		{
			array[0] &= (uint)(-1 >>> num);
		}
		return array;
	}

	private BigInteger DivideWords(int w)
	{
		int num = magnitude.Length;
		if (w >= num)
		{
			return Zero;
		}
		uint[] array = new uint[num - w];
		Array.Copy(magnitude, 0, array, 0, num - w);
		return new BigInteger(sign, array, checkMag: false);
	}

	private BigInteger RemainderWords(int w)
	{
		int num = magnitude.Length;
		if (w >= num)
		{
			return this;
		}
		uint[] array = new uint[w];
		Array.Copy(magnitude, num - w, array, 0, w);
		return new BigInteger(sign, array, checkMag: false);
	}

	private static uint[] ShiftLeft(uint[] mag, int n)
	{
		int num = n >>> 5;
		int num2 = n & 0x1F;
		int num3 = mag.Length;
		uint[] array;
		if (num2 == 0)
		{
			array = new uint[num3 + num];
			mag.CopyTo(array, 0);
		}
		else
		{
			int num4 = 0;
			int num5 = 32 - num2;
			uint num6 = mag[0] >> num5;
			if (num6 != 0)
			{
				array = new uint[num3 + num + 1];
				array[num4++] = num6;
			}
			else
			{
				array = new uint[num3 + num];
			}
			uint num7 = mag[0];
			for (int i = 0; i < num3 - 1; i++)
			{
				uint num8 = mag[i + 1];
				array[num4++] = (num7 << num2) | (num8 >> num5);
				num7 = num8;
			}
			array[num4] = mag[num3 - 1] << num2;
		}
		return array;
	}

	private static int ShiftLeftOneInPlace(int[] x, int carry)
	{
		int num = x.Length;
		while (--num >= 0)
		{
			uint num2 = (uint)x[num];
			x[num] = (int)(num2 << 1) | carry;
			carry = (int)(num2 >> 31);
		}
		return carry;
	}

	public BigInteger ShiftLeft(int n)
	{
		if (sign == 0 || magnitude.Length == 0)
		{
			return Zero;
		}
		if (n == 0)
		{
			return this;
		}
		if (n < 0)
		{
			return ShiftRight(-n);
		}
		BigInteger bigInteger = new BigInteger(sign, ShiftLeft(magnitude, n), checkMag: true);
		if (nBits != -1)
		{
			bigInteger.nBits = ((sign > 0) ? nBits : (nBits + n));
		}
		if (nBitLength != -1)
		{
			bigInteger.nBitLength = nBitLength + n;
		}
		return bigInteger;
	}

	private static void ShiftRightInPlace(int start, uint[] mag, int n)
	{
		int num = (n >>> 5) + start;
		int num2 = n & 0x1F;
		int num3 = mag.Length - 1;
		if (num != start)
		{
			int num4 = num - start;
			for (int num5 = num3; num5 >= num; num5--)
			{
				mag[num5] = mag[num5 - num4];
			}
			for (int num6 = num - 1; num6 >= start; num6--)
			{
				mag[num6] = 0u;
			}
		}
		if (num2 != 0)
		{
			int num7 = 32 - num2;
			uint num8 = mag[num3];
			for (int num9 = num3; num9 > num; num9--)
			{
				uint num10 = mag[num9 - 1];
				mag[num9] = (num8 >> num2) | (num10 << num7);
				num8 = num10;
			}
			mag[num] >>= num2;
		}
	}

	private static void ShiftRightOneInPlace(int start, uint[] mag)
	{
		int num = mag.Length;
		uint num2 = mag[num - 1];
		while (--num > start)
		{
			uint num3 = mag[num - 1];
			mag[num] = (num2 >> 1) | (num3 << 31);
			num2 = num3;
		}
		mag[start] >>= 1;
	}

	public BigInteger ShiftRight(int n)
	{
		if (n == 0)
		{
			return this;
		}
		if (n < 0)
		{
			return ShiftLeft(-n);
		}
		if (n >= BitLength)
		{
			if (sign >= 0)
			{
				return Zero;
			}
			return One.Negate();
		}
		int num = BitLength - n + 31 >> 5;
		uint[] array = new uint[num];
		int num2 = n >> 5;
		int num3 = n & 0x1F;
		if (num3 == 0)
		{
			Array.Copy(magnitude, 0, array, 0, array.Length);
		}
		else
		{
			int num4 = 32 - num3;
			int num5 = magnitude.Length - 1 - num2;
			for (int num6 = num - 1; num6 >= 0; num6--)
			{
				array[num6] = magnitude[num5--] >> num3;
				if (num5 >= 0)
				{
					array[num6] |= magnitude[num5] << num4;
				}
			}
		}
		return new BigInteger(sign, array, checkMag: false);
	}

	private static uint[] Subtract(int xStart, uint[] x, int yStart, uint[] y)
	{
		int num = x.Length;
		int num2 = y.Length;
		int num3 = 0;
		do
		{
			long num4 = (long)(((ulong)x[--num] & 0xFFFFFFFFuL) - ((ulong)y[--num2] & 0xFFFFFFFFuL)) + (long)num3;
			x[num] = (uint)num4;
			num3 = (int)(num4 >> 63);
		}
		while (num2 > yStart);
		if (num3 != 0)
		{
			while (--x[--num] == uint.MaxValue)
			{
			}
		}
		return x;
	}

	public BigInteger Subtract(BigInteger n)
	{
		if (n.sign == 0)
		{
			return this;
		}
		if (sign == 0)
		{
			return n.Negate();
		}
		if (sign != n.sign)
		{
			return Add(n.Negate());
		}
		int num = CompareNoLeadingZeros(0, magnitude, 0, n.magnitude);
		if (num == 0)
		{
			return Zero;
		}
		BigInteger bigInteger;
		BigInteger bigInteger2;
		if (num < 0)
		{
			bigInteger = n;
			bigInteger2 = this;
		}
		else
		{
			bigInteger = this;
			bigInteger2 = n;
		}
		return new BigInteger(sign * num, DoSubBigLil(bigInteger.magnitude, bigInteger2.magnitude), checkMag: true);
	}

	private static uint[] DoSubBigLil(uint[] bigMag, uint[] lilMag)
	{
		uint[] x = (uint[])bigMag.Clone();
		return Subtract(0, x, 0, lilMag);
	}

	public int GetLengthofByteArray()
	{
		return GetBytesLength(BitLength + 1);
	}

	public int GetLengthofByteArrayUnsigned()
	{
		return GetBytesLength((sign < 0) ? (BitLength + 1) : BitLength);
	}

	public byte[] ToByteArray()
	{
		return ToByteArray(unsigned: false);
	}

	public byte[] ToByteArrayUnsigned()
	{
		return ToByteArray(unsigned: true);
	}

	private byte[] ToByteArray(bool unsigned)
	{
		if (sign == 0)
		{
			if (!unsigned)
			{
				return new byte[1];
			}
			return ZeroEncoding;
		}
		byte[] array = new byte[GetBytesLength((unsigned && sign > 0) ? BitLength : (BitLength + 1))];
		int num = magnitude.Length;
		int num2 = array.Length;
		if (sign > 0)
		{
			while (num > 1)
			{
				uint n = magnitude[--num];
				num2 -= 4;
				Pack.UInt32_To_BE(n, array, num2);
			}
			uint num3;
			for (num3 = magnitude[0]; num3 > 255; num3 >>= 8)
			{
				array[--num2] = (byte)num3;
			}
			array[--num2] = (byte)num3;
		}
		else
		{
			bool flag = true;
			while (num > 1)
			{
				uint num4 = ~magnitude[--num];
				if (flag)
				{
					flag = ++num4 == 0;
				}
				num2 -= 4;
				Pack.UInt32_To_BE(num4, array, num2);
			}
			uint num5 = magnitude[0];
			if (flag)
			{
				num5--;
			}
			while (num5 > 255)
			{
				array[--num2] = (byte)(~num5);
				num5 >>= 8;
			}
			array[--num2] = (byte)(~num5);
			if (num2 != 0)
			{
				array[--num2] = byte.MaxValue;
			}
		}
		return array;
	}

	public override string ToString()
	{
		return ToString(10);
	}

	public string ToString(int radix)
	{
		switch (radix)
		{
		default:
			throw new FormatException("Only bases 2, 8, 10, 16 are allowed");
		case 2:
		case 8:
		case 10:
		case 16:
		{
			if (magnitude == null)
			{
				return "null";
			}
			if (sign == 0)
			{
				return "0";
			}
			int i;
			for (i = 0; i < magnitude.Length && magnitude[i] == 0; i++)
			{
			}
			if (i == magnitude.Length)
			{
				return "0";
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (sign == -1)
			{
				stringBuilder.Append('-');
			}
			switch (radix)
			{
			case 2:
			{
				int num5 = i;
				stringBuilder.Append(Convert.ToString(magnitude[num5], 2));
				while (++num5 < magnitude.Length)
				{
					AppendZeroExtendedString(stringBuilder, Convert.ToString(magnitude[num5], 2), 32);
				}
				break;
			}
			case 8:
			{
				int num = 1073741823;
				BigInteger bigInteger3 = Abs();
				int num2 = bigInteger3.BitLength;
				List<string> list2 = new List<string>();
				while (num2 > 30)
				{
					list2.Add(Convert.ToString(bigInteger3.IntValue & num, 8));
					bigInteger3 = bigInteger3.ShiftRight(30);
					num2 -= 30;
				}
				stringBuilder.Append(Convert.ToString(bigInteger3.IntValue, 8));
				for (int num3 = list2.Count - 1; num3 >= 0; num3--)
				{
					AppendZeroExtendedString(stringBuilder, list2[num3], 10);
				}
				break;
			}
			case 16:
			{
				int num4 = i;
				stringBuilder.Append(Convert.ToString(magnitude[num4], 16));
				while (++num4 < magnitude.Length)
				{
					AppendZeroExtendedString(stringBuilder, Convert.ToString(magnitude[num4], 16), 8);
				}
				break;
			}
			case 10:
			{
				BigInteger bigInteger = Abs();
				if (bigInteger.BitLength < 64)
				{
					stringBuilder.Append(Convert.ToString(bigInteger.LongValue, radix));
					break;
				}
				List<BigInteger> list = new List<BigInteger>();
				BigInteger bigInteger2 = ValueOf(radix);
				while (bigInteger2.CompareTo(bigInteger) <= 0)
				{
					list.Add(bigInteger2);
					bigInteger2 = bigInteger2.Square();
				}
				int count = list.Count;
				stringBuilder.EnsureCapacity(stringBuilder.Length + (1 << count));
				ToString(stringBuilder, radix, list, count, bigInteger);
				break;
			}
			}
			return stringBuilder.ToString();
		}
		}
	}

	private static void ToString(StringBuilder sb, int radix, IList<BigInteger> moduli, int scale, BigInteger pos)
	{
		if (pos.BitLength < 64)
		{
			string text = Convert.ToString(pos.LongValue, radix);
			if (sb.Length > 1 || (sb.Length == 1 && sb[0] != '-'))
			{
				AppendZeroExtendedString(sb, text, 1 << scale);
			}
			else if (pos.SignValue != 0)
			{
				sb.Append(text);
			}
		}
		else
		{
			BigInteger[] array = pos.DivideAndRemainder(moduli[--scale]);
			ToString(sb, radix, moduli, scale, array[0]);
			ToString(sb, radix, moduli, scale, array[1]);
		}
	}

	private static void AppendZeroExtendedString(StringBuilder sb, string s, int minLength)
	{
		for (int i = s.Length; i < minLength; i++)
		{
			sb.Append('0');
		}
		sb.Append(s);
	}

	private static BigInteger CreateUValueOf(uint value)
	{
		if (value == 0)
		{
			return Zero;
		}
		return new BigInteger(1, new uint[1] { value }, checkMag: false);
	}

	private static BigInteger CreateUValueOf(ulong value)
	{
		uint num = (uint)(value >> 32);
		uint num2 = (uint)value;
		if (num == 0)
		{
			return CreateUValueOf(num2);
		}
		return new BigInteger(1, new uint[2] { num, num2 }, checkMag: false);
	}

	public static BigInteger ValueOf(int value)
	{
		if (value >= 0)
		{
			if (value < SMALL_CONSTANTS.Length)
			{
				return SMALL_CONSTANTS[value];
			}
			return CreateUValueOf((uint)value);
		}
		if (value == int.MinValue)
		{
			return CreateUValueOf((uint)(~value)).Not();
		}
		return ValueOf(-value).Negate();
	}

	public static BigInteger ValueOf(long value)
	{
		if (value >= 0)
		{
			if (value < SMALL_CONSTANTS.Length)
			{
				return SMALL_CONSTANTS[value];
			}
			return CreateUValueOf((ulong)value);
		}
		if (value == long.MinValue)
		{
			return CreateUValueOf((ulong)(~value)).Not();
		}
		return ValueOf(-value).Negate();
	}

	public int GetLowestSetBit()
	{
		if (sign == 0)
		{
			return -1;
		}
		return GetLowestSetBitMaskFirst(uint.MaxValue);
	}

	private int GetLowestSetBitMaskFirst(uint firstWordMaskX)
	{
		int num = magnitude.Length;
		int num2 = 0;
		uint num3 = magnitude[--num] & firstWordMaskX;
		while (num3 == 0)
		{
			num3 = magnitude[--num];
			num2 += 32;
		}
		while ((num3 & 0xFF) == 0)
		{
			num3 >>= 8;
			num2 += 8;
		}
		while ((num3 & 1) == 0)
		{
			num3 >>= 1;
			num2++;
		}
		return num2;
	}

	public bool TestBit(int n)
	{
		if (n < 0)
		{
			throw new ArithmeticException("Bit position must not be negative");
		}
		if (sign < 0)
		{
			return !Not().TestBit(n);
		}
		int num = n / 32;
		if (num >= magnitude.Length)
		{
			return false;
		}
		return ((magnitude[magnitude.Length - 1 - num] >> n % 32) & 1) != 0;
	}

	public BigInteger Or(BigInteger value)
	{
		if (sign == 0)
		{
			return value;
		}
		if (value.sign == 0)
		{
			return this;
		}
		uint[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
		uint[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
		bool flag = sign < 0 || value.sign < 0;
		uint[] array3 = new uint[System.Math.Max(array.Length, array2.Length)];
		int num = array3.Length - array.Length;
		int num2 = array3.Length - array2.Length;
		for (int i = 0; i < array3.Length; i++)
		{
			uint num3 = ((i >= num) ? array[i - num] : 0u);
			uint num4 = ((i >= num2) ? array2[i - num2] : 0u);
			if (sign < 0)
			{
				num3 = ~num3;
			}
			if (value.sign < 0)
			{
				num4 = ~num4;
			}
			array3[i] = num3 | num4;
			if (flag)
			{
				array3[i] = ~array3[i];
			}
		}
		BigInteger bigInteger = new BigInteger(1, array3, checkMag: true);
		if (flag)
		{
			bigInteger = bigInteger.Not();
		}
		return bigInteger;
	}

	public BigInteger Xor(BigInteger value)
	{
		if (sign == 0)
		{
			return value;
		}
		if (value.sign == 0)
		{
			return this;
		}
		uint[] array = ((sign > 0) ? magnitude : Add(One).magnitude);
		uint[] array2 = ((value.sign > 0) ? value.magnitude : value.Add(One).magnitude);
		bool flag = (sign < 0 && value.sign >= 0) || (sign >= 0 && value.sign < 0);
		uint[] array3 = new uint[System.Math.Max(array.Length, array2.Length)];
		int num = array3.Length - array.Length;
		int num2 = array3.Length - array2.Length;
		for (int i = 0; i < array3.Length; i++)
		{
			uint num3 = ((i >= num) ? array[i - num] : 0u);
			uint num4 = ((i >= num2) ? array2[i - num2] : 0u);
			if (sign < 0)
			{
				num3 = ~num3;
			}
			if (value.sign < 0)
			{
				num4 = ~num4;
			}
			array3[i] = num3 ^ num4;
			if (flag)
			{
				array3[i] = ~array3[i];
			}
		}
		BigInteger bigInteger = new BigInteger(1, array3, checkMag: true);
		if (flag)
		{
			bigInteger = bigInteger.Not();
		}
		return bigInteger;
	}

	public BigInteger SetBit(int n)
	{
		if (n < 0)
		{
			throw new ArithmeticException("Bit address less than zero");
		}
		if (TestBit(n))
		{
			return this;
		}
		if (sign > 0 && n < BitLength - 1)
		{
			return FlipExistingBit(n);
		}
		return Or(One.ShiftLeft(n));
	}

	public BigInteger ClearBit(int n)
	{
		if (n < 0)
		{
			throw new ArithmeticException("Bit address less than zero");
		}
		if (!TestBit(n))
		{
			return this;
		}
		if (sign > 0 && n < BitLength - 1)
		{
			return FlipExistingBit(n);
		}
		return AndNot(One.ShiftLeft(n));
	}

	public BigInteger FlipBit(int n)
	{
		if (n < 0)
		{
			throw new ArithmeticException("Bit address less than zero");
		}
		if (sign > 0 && n < BitLength - 1)
		{
			return FlipExistingBit(n);
		}
		return Xor(One.ShiftLeft(n));
	}

	private BigInteger FlipExistingBit(int n)
	{
		uint[] array = (uint[])magnitude.Clone();
		array[array.Length - 1 - (n >> 5)] ^= (uint)(1 << n);
		return new BigInteger(sign, array, checkMag: false);
	}
}
