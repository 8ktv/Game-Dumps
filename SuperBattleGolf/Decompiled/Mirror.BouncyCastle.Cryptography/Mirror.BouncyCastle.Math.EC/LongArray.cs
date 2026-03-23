using System;
using System.Text;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC;

internal struct LongArray
{
	private ulong[] m_data;

	internal static bool AreAliased(ref LongArray a, ref LongArray b)
	{
		return a.m_data == b.m_data;
	}

	internal LongArray(int intLen)
	{
		m_data = new ulong[intLen];
	}

	internal LongArray(ulong[] data)
	{
		m_data = data;
	}

	internal LongArray(ulong[] data, int off, int len)
	{
		if (off == 0 && len == data.Length)
		{
			m_data = data;
			return;
		}
		m_data = new ulong[len];
		Array.Copy(data, off, m_data, 0, len);
	}

	internal LongArray(BigInteger bigInt)
	{
		if (bigInt == null || bigInt.SignValue < 0)
		{
			throw new ArgumentException("invalid F2m field value", "bigInt");
		}
		if (bigInt.SignValue == 0)
		{
			m_data = new ulong[1];
			return;
		}
		byte[] array = bigInt.ToByteArray();
		int num = array.Length;
		int num2 = 0;
		if (array[0] == 0)
		{
			num--;
			num2 = 1;
		}
		int num3 = (num + 7) / 8;
		m_data = new ulong[num3];
		int num4 = num3 - 1;
		int num5 = num % 8 + num2;
		ulong num6 = 0uL;
		int i = num2;
		if (num2 < num5)
		{
			for (; i < num5; i++)
			{
				num6 <<= 8;
				uint num7 = array[i];
				num6 |= num7;
			}
			m_data[num4--] = num6;
		}
		while (num4 >= 0)
		{
			num6 = 0uL;
			for (int j = 0; j < 8; j++)
			{
				num6 <<= 8;
				uint num8 = array[i++];
				num6 |= num8;
			}
			m_data[num4] = num6;
			num4--;
		}
	}

	internal void CopyTo(ulong[] z, int zOff)
	{
		Array.Copy(m_data, 0, z, zOff, m_data.Length);
	}

	internal bool IsOne()
	{
		ulong[] data = m_data;
		int num = data.Length;
		if (num < 1 || data[0] != 1)
		{
			return false;
		}
		for (int i = 1; i < num; i++)
		{
			if (data[i] != 0L)
			{
				return false;
			}
		}
		return true;
	}

	internal bool IsZero()
	{
		ulong[] data = m_data;
		for (int i = 0; i < data.Length; i++)
		{
			if (data[i] != 0L)
			{
				return false;
			}
		}
		return true;
	}

	internal int GetUsedLength()
	{
		return GetUsedLengthFrom(m_data.Length);
	}

	internal int GetUsedLengthFrom(int from)
	{
		ulong[] data = m_data;
		from = System.Math.Min(from, data.Length);
		if (from < 1)
		{
			return 0;
		}
		if (data[0] != 0L)
		{
			while (data[--from] == 0L)
			{
			}
			return from + 1;
		}
		do
		{
			if (data[--from] != 0L)
			{
				return from + 1;
			}
		}
		while (from > 0);
		return 0;
	}

	internal int Degree()
	{
		int num = m_data.Length;
		ulong num2;
		do
		{
			if (num == 0)
			{
				return 0;
			}
			num2 = m_data[--num];
		}
		while (num2 == 0L);
		return (num << 6) + BitLength(num2);
	}

	private int DegreeFrom(int limit)
	{
		int num = limit + 62 >>> 6;
		ulong num2;
		do
		{
			if (num == 0)
			{
				return 0;
			}
			num2 = m_data[--num];
		}
		while (num2 == 0L);
		return (num << 6) + BitLength(num2);
	}

	private static int BitLength(ulong w)
	{
		return 64 - Longs.NumberOfLeadingZeros((long)w);
	}

	private ulong[] ResizedData(int newLen)
	{
		ulong[] array = new ulong[newLen];
		Array.Copy(m_data, 0, array, 0, System.Math.Min(m_data.Length, newLen));
		return array;
	}

	internal BigInteger ToBigInteger()
	{
		int usedLength = GetUsedLength();
		if (usedLength == 0)
		{
			return BigInteger.Zero;
		}
		ulong num = m_data[usedLength - 1];
		byte[] array = new byte[8];
		int num2 = 0;
		bool flag = false;
		for (int num3 = 7; num3 >= 0; num3--)
		{
			byte b = (byte)(num >> 8 * num3);
			if (flag || b != 0)
			{
				flag = true;
				array[num2++] = b;
			}
		}
		byte[] array2 = new byte[8 * (usedLength - 1) + num2];
		for (int i = 0; i < num2; i++)
		{
			array2[i] = array[i];
		}
		for (int num4 = usedLength - 2; num4 >= 0; num4--)
		{
			ulong num5 = m_data[num4];
			for (int num6 = 7; num6 >= 0; num6--)
			{
				array2[num2++] = (byte)(num5 >> 8 * num6);
			}
		}
		return new BigInteger(1, array2);
	}

	private static ulong ShiftUp(ulong[] x, int xOff, int count, int shift)
	{
		int num = 64 - shift;
		ulong num2 = 0uL;
		for (int i = 0; i < count; i++)
		{
			ulong num3 = x[xOff + i];
			x[xOff + i] = (num3 << shift) | num2;
			num2 = num3 >> num;
		}
		return num2;
	}

	private static ulong ShiftUp(ulong[] x, int xOff, ulong[] z, int zOff, int count, int shift)
	{
		int num = 64 - shift;
		ulong num2 = 0uL;
		for (int i = 0; i < count; i++)
		{
			ulong num3 = x[xOff + i];
			z[zOff + i] = (num3 << shift) | num2;
			num2 = num3 >> num;
		}
		return num2;
	}

	internal LongArray AddOne()
	{
		if (m_data.Length == 0)
		{
			return new LongArray(new ulong[1] { 1uL });
		}
		int newLen = System.Math.Max(1, GetUsedLength());
		ulong[] array = ResizedData(newLen);
		array[0] ^= 1uL;
		return new LongArray(array);
	}

	private void AddShiftedByBitsSafe(LongArray other, int otherDegree, int bits)
	{
		int num = otherDegree + 63 >>> 6;
		int num2 = bits >>> 6;
		int num3 = bits & 0x3F;
		if (num3 == 0)
		{
			Add(m_data, num2, other.m_data, 0, num);
			return;
		}
		ulong num4 = AddShiftedUp(m_data, num2, other.m_data, 0, num, num3);
		if (num4 != 0L)
		{
			m_data[num + num2] ^= num4;
		}
	}

	private static ulong AddShiftedUp(ulong[] x, int xOff, ulong[] y, int yOff, int count, int shift)
	{
		int num = 64 - shift;
		ulong num2 = 0uL;
		for (int i = 0; i < count; i++)
		{
			ulong num3 = y[yOff + i];
			x[xOff + i] ^= (num3 << shift) | num2;
			num2 = num3 >> num;
		}
		return num2;
	}

	private static ulong AddShiftedDown(ulong[] x, int xOff, ulong[] y, int yOff, int count, int shift)
	{
		int num = 64 - shift;
		ulong num2 = 0uL;
		int num3 = count;
		while (--num3 >= 0)
		{
			ulong num4 = y[yOff + num3];
			x[xOff + num3] ^= (num4 >> shift) | num2;
			num2 = num4 << num;
		}
		return num2;
	}

	internal void AddShiftedByWords(LongArray other, int words)
	{
		int usedLength = other.GetUsedLength();
		if (usedLength != 0)
		{
			int num = usedLength + words;
			if (num > m_data.Length)
			{
				m_data = ResizedData(num);
			}
			Add(m_data, words, other.m_data, 0, usedLength);
		}
	}

	private static void Add(ulong[] x, int xOff, ulong[] y, int yOff, int count)
	{
		Nat.XorTo64(count, y, yOff, x, xOff);
	}

	private static void Add(ulong[] x, int xOff, ulong[] y, int yOff, ulong[] z, int zOff, int count)
	{
		Nat.Xor64(count, x, xOff, y, yOff, z, zOff);
	}

	private static void AddBoth(ulong[] x, int xOff, ulong[] y1, int y1Off, ulong[] y2, int y2Off, int count)
	{
		for (int i = 0; i < count; i++)
		{
			x[xOff + i] ^= y1[y1Off + i] ^ y2[y2Off + i];
		}
	}

	private static void FlipWord(ulong[] buf, int off, int bit, ulong word)
	{
		int num = off + (bit >>> 6);
		int num2 = bit & 0x3F;
		if (num2 == 0)
		{
			buf[num] ^= word;
			return;
		}
		buf[num] ^= word << num2;
		word >>= 64 - num2;
		if (word != 0L)
		{
			buf[++num] ^= word;
		}
	}

	internal bool TestBitZero()
	{
		if (m_data.Length != 0)
		{
			return (m_data[0] & 1) != 0;
		}
		return false;
	}

	private static bool TestBit(ulong[] buf, int off, int n)
	{
		int num = n >>> 6;
		int num2 = n & 0x3F;
		ulong num3 = (ulong)(1L << num2);
		return (buf[off + num] & num3) != 0;
	}

	private static void FlipBit(ulong[] buf, int off, int n)
	{
		int num = n >>> 6;
		int num2 = n & 0x3F;
		ulong num3 = (ulong)(1L << num2);
		buf[off + num] ^= num3;
	}

	private static void MultiplyWord(ulong a, ulong[] b, int bLen, ulong[] c, int cOff)
	{
		if ((a & 1) != 0L)
		{
			Add(c, cOff, b, 0, bLen);
		}
		int num = 1;
		while ((a >>= 1) != 0L)
		{
			if ((a & 1) != 0L)
			{
				ulong num2 = AddShiftedUp(c, cOff, b, 0, bLen, num);
				if (num2 != 0L)
				{
					c[cOff + bLen] ^= num2;
				}
			}
			num++;
		}
	}

	internal LongArray ModMultiplyLD(LongArray other, int m, int[] ks)
	{
		int num = Degree();
		if (num == 0)
		{
			return this;
		}
		int num2 = other.Degree();
		if (num2 == 0)
		{
			return other;
		}
		LongArray longArray = this;
		LongArray result = other;
		if (num > num2)
		{
			longArray = other;
			result = this;
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		int num4 = num + 63 >>> 6;
		int num5 = num2 + 63 >>> 6;
		int num6 = num + num2 + 62 >>> 6;
		if (num4 == 1)
		{
			ulong num7 = longArray.m_data[0];
			if (num7 == 1)
			{
				return result;
			}
			ulong[] array = new ulong[num6];
			MultiplyWord(num7, result.m_data, num5, array, 0);
			return ReduceResult(array, 0, num6, m, ks);
		}
		int num8 = num2 + 7 + 63 >>> 6;
		int[] array2 = new int[16];
		ulong[] array3 = new ulong[num8 << 4];
		int num9 = (array2[1] = num8);
		Array.Copy(result.m_data, 0, array3, num9, num5);
		for (int i = 2; i < 16; i++)
		{
			num9 = (array2[i] = num9 + num8);
			if ((i & 1) == 0)
			{
				ShiftUp(array3, num9 >>> 1, array3, num9, num8, 1);
			}
			else
			{
				Add(array3, num8, array3, num9 - num8, array3, num9, num8);
			}
		}
		ulong[] array4 = new ulong[array3.Length];
		ShiftUp(array3, 0, array4, 0, array3.Length, 4);
		ulong[] data = longArray.m_data;
		ulong[] array5 = new ulong[num6];
		uint num10 = 15u;
		for (int num11 = 56; num11 >= 0; num11 -= 8)
		{
			for (int j = 1; j < num4; j += 2)
			{
				int num12 = (int)(data[j] >> num11);
				uint num13 = (uint)num12 & num10;
				uint num14 = (uint)(num12 >>> 4) & num10;
				AddBoth(array5, j - 1, array3, array2[num13], array4, array2[num14], num8);
			}
			ShiftUp(array5, 0, num6, 8);
		}
		for (int num15 = 56; num15 >= 0; num15 -= 8)
		{
			for (int k = 0; k < num4; k += 2)
			{
				int num16 = (int)(data[k] >> num15);
				uint num17 = (uint)num16 & num10;
				uint num18 = (uint)(num16 >>> 4) & num10;
				AddBoth(array5, k, array3, array2[num17], array4, array2[num18], num8);
			}
			if (num15 > 0)
			{
				ShiftUp(array5, 0, num6, 8);
			}
		}
		return ReduceResult(array5, 0, num6, m, ks);
	}

	internal LongArray ModMultiply(LongArray other, int m, int[] ks)
	{
		int num = Degree();
		if (num == 0)
		{
			return this;
		}
		int num2 = other.Degree();
		if (num2 == 0)
		{
			return other;
		}
		LongArray longArray = this;
		LongArray result = other;
		if (num > num2)
		{
			longArray = other;
			result = this;
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		int num4 = num + 63 >>> 6;
		int num5 = num2 + 63 >>> 6;
		int num6 = num + num2 + 62 >>> 6;
		if (num4 == 1)
		{
			ulong num7 = longArray.m_data[0];
			if (num7 == 1)
			{
				return result;
			}
			ulong[] array = new ulong[num6];
			MultiplyWord(num7, result.m_data, num5, array, 0);
			return ReduceResult(array, 0, num6, m, ks);
		}
		int num8 = num2 + 7 + 63 >>> 6;
		int[] array2 = new int[16];
		ulong[] array3 = new ulong[num8 << 4];
		int num9 = (array2[1] = num8);
		Array.Copy(result.m_data, 0, array3, num9, num5);
		for (int i = 2; i < 16; i++)
		{
			num9 = (array2[i] = num9 + num8);
			if ((i & 1) == 0)
			{
				ShiftUp(array3, num9 >>> 1, array3, num9, num8, 1);
			}
			else
			{
				Add(array3, num8, array3, num9 - num8, array3, num9, num8);
			}
		}
		ulong[] array4 = new ulong[array3.Length];
		ShiftUp(array3, 0, array4, 0, array3.Length, 4);
		ulong[] data = longArray.m_data;
		ulong[] array5 = new ulong[num6 << 3];
		uint num10 = 15u;
		for (int j = 0; j < num4; j++)
		{
			ulong num11 = data[j];
			int num12 = j;
			while (true)
			{
				uint num13 = (uint)(int)num11 & num10;
				num11 >>= 4;
				uint num14 = (uint)(int)num11 & num10;
				num11 >>= 4;
				AddBoth(array5, num12, array3, array2[num13], array4, array2[num14], num8);
				if (num11 == 0L)
				{
					break;
				}
				num12 += num6;
			}
		}
		int num15 = array5.Length;
		while ((num15 -= num6) != 0)
		{
			AddShiftedUp(array5, num15 - num6, array5, num15, num6, 8);
		}
		return ReduceResult(array5, 0, num6, m, ks);
	}

	internal LongArray Multiply(LongArray other, int m, int[] ks)
	{
		int num = Degree();
		if (num == 0)
		{
			return this;
		}
		int num2 = other.Degree();
		if (num2 == 0)
		{
			return other;
		}
		LongArray longArray = this;
		LongArray result = other;
		if (num > num2)
		{
			longArray = other;
			result = this;
			int num3 = num;
			num = num2;
			num2 = num3;
		}
		int num4 = num + 63 >>> 6;
		int num5 = num2 + 63 >>> 6;
		int num6 = num + num2 + 62 >>> 6;
		if (num4 == 1)
		{
			ulong num7 = longArray.m_data[0];
			if (num7 == 1)
			{
				return result;
			}
			ulong[] array = new ulong[num6];
			MultiplyWord(num7, result.m_data, num5, array, 0);
			return new LongArray(array, 0, num6);
		}
		int num8 = num2 + 7 + 63 >>> 6;
		int[] array2 = new int[16];
		ulong[] array3 = new ulong[num8 << 4];
		int num9 = (array2[1] = num8);
		Array.Copy(result.m_data, 0, array3, num9, num5);
		for (int i = 2; i < 16; i++)
		{
			num9 = (array2[i] = num9 + num8);
			if ((i & 1) == 0)
			{
				ShiftUp(array3, num9 >>> 1, array3, num9, num8, 1);
			}
			else
			{
				Add(array3, num8, array3, num9 - num8, array3, num9, num8);
			}
		}
		ulong[] array4 = new ulong[array3.Length];
		ShiftUp(array3, 0, array4, 0, array3.Length, 4);
		ulong[] data = longArray.m_data;
		ulong[] array5 = new ulong[num6 << 3];
		uint num10 = 15u;
		for (int j = 0; j < num4; j++)
		{
			ulong num11 = data[j];
			int num12 = j;
			while (true)
			{
				uint num13 = (uint)(int)num11 & num10;
				num11 >>= 4;
				uint num14 = (uint)(int)num11 & num10;
				num11 >>= 4;
				AddBoth(array5, num12, array3, array2[num13], array4, array2[num14], num8);
				if (num11 == 0L)
				{
					break;
				}
				num12 += num6;
			}
		}
		int num15 = array5.Length;
		while ((num15 -= num6) != 0)
		{
			AddShiftedUp(array5, num15 - num6, array5, num15, num6, 8);
		}
		return new LongArray(array5, 0, num6);
	}

	internal void Reduce(int m, int[] ks)
	{
		ulong[] data = m_data;
		int num = ReduceInPlace(data, 0, data.Length, m, ks);
		if (num < data.Length)
		{
			m_data = new ulong[num];
			Array.Copy(data, 0, m_data, 0, num);
		}
	}

	private static LongArray ReduceResult(ulong[] buf, int off, int len, int m, int[] ks)
	{
		int len2 = ReduceInPlace(buf, off, len, m, ks);
		return new LongArray(buf, off, len2);
	}

	private static int ReduceInPlace(ulong[] buf, int off, int len, int m, int[] ks)
	{
		int num = m + 63 >> 6;
		if (len < num)
		{
			return len;
		}
		int num2 = System.Math.Min(len << 6, (m << 1) - 1);
		int num3;
		for (num3 = (len << 6) - num2; num3 >= 64; num3 -= 64)
		{
			len--;
		}
		int num4 = ks.Length;
		int num5 = ks[num4 - 1];
		int num6 = ((num4 > 1) ? ks[num4 - 2] : 0);
		int num7 = System.Math.Max(m, num5 + 64);
		int num8 = num3 + System.Math.Min(num2 - num7, m - num6) >> 6;
		if (num8 > 1)
		{
			int num9 = len - num8;
			ReduceVectorWise(buf, off, len, num9, m, ks);
			while (len > num9)
			{
				buf[off + --len] = 0uL;
			}
			num2 = num9 << 6;
		}
		if (num2 > num7)
		{
			ReduceWordWise(buf, off, len, num7, m, ks);
			num2 = num7;
		}
		if (num2 > m)
		{
			ReduceBitWise(buf, off, num2, m, ks);
		}
		return num;
	}

	private static void ReduceBitWise(ulong[] buf, int off, int BitLength, int m, int[] ks)
	{
		while (--BitLength >= m)
		{
			if (TestBit(buf, off, BitLength))
			{
				ReduceBit(buf, off, BitLength, m, ks);
			}
		}
	}

	private static void ReduceBit(ulong[] buf, int off, int bit, int m, int[] ks)
	{
		FlipBit(buf, off, bit);
		int num = bit - m;
		int num2 = ks.Length;
		while (--num2 >= 0)
		{
			FlipBit(buf, off, ks[num2] + num);
		}
		FlipBit(buf, off, num);
	}

	private static void ReduceWordWise(ulong[] buf, int off, int len, int toBit, int m, int[] ks)
	{
		int num = toBit >>> 6;
		while (--len > num)
		{
			ulong num2 = buf[off + len];
			if (num2 != 0L)
			{
				buf[off + len] = 0uL;
				ReduceWord(buf, off, len << 6, num2, m, ks);
			}
		}
		int num3 = toBit & 0x3F;
		ulong num4 = buf[off + num] >> num3;
		if (num4 != 0L)
		{
			buf[off + num] ^= num4 << num3;
			ReduceWord(buf, off, toBit, num4, m, ks);
		}
	}

	private static void ReduceWord(ulong[] buf, int off, int bit, ulong word, int m, int[] ks)
	{
		int num = bit - m;
		int num2 = ks.Length;
		while (--num2 >= 0)
		{
			FlipWord(buf, off, num + ks[num2], word);
		}
		FlipWord(buf, off, num, word);
	}

	private static void ReduceVectorWise(ulong[] buf, int off, int len, int words, int m, int[] ks)
	{
		int num = (words << 6) - m;
		int num2 = ks.Length;
		while (--num2 >= 0)
		{
			FlipVector(buf, off, buf, off + words, len - words, num + ks[num2]);
		}
		FlipVector(buf, off, buf, off + words, len - words, num);
	}

	private static void FlipVector(ulong[] x, int xOff, ulong[] y, int yOff, int yLen, int bits)
	{
		xOff += bits >>> 6;
		bits &= 0x3F;
		if (bits == 0)
		{
			Add(x, xOff, y, yOff, yLen);
			return;
		}
		ulong num = AddShiftedDown(x, xOff + 1, y, yOff, yLen, 64 - bits);
		x[xOff] ^= num;
	}

	internal LongArray ModSquare(int m, int[] ks)
	{
		int usedLength = GetUsedLength();
		if (usedLength == 0)
		{
			return this;
		}
		ulong[] array = new ulong[usedLength << 1];
		Interleave.Expand64To128(m_data, 0, usedLength, array, 0);
		return new LongArray(array, 0, ReduceInPlace(array, 0, array.Length, m, ks));
	}

	internal LongArray ModSquareN(int n, int m, int[] ks)
	{
		int num = GetUsedLength();
		if (num == 0)
		{
			return this;
		}
		ulong[] array = new ulong[m + 63 >> 6 << 1];
		Array.Copy(m_data, 0, array, 0, num);
		while (--n >= 0)
		{
			Interleave.Expand64To128(array, 0, num, array, 0);
			num = ReduceInPlace(array, 0, array.Length, m, ks);
		}
		return new LongArray(array, 0, num);
	}

	internal LongArray Square(int m, int[] ks)
	{
		int usedLength = GetUsedLength();
		if (usedLength == 0)
		{
			return this;
		}
		ulong[] array = new ulong[usedLength << 1];
		Interleave.Expand64To128(m_data, 0, usedLength, array, 0);
		return new LongArray(array, 0, array.Length);
	}

	internal LongArray ModInverse(int m, int[] ks)
	{
		int num = Degree();
		switch (num)
		{
		case 0:
			throw new InvalidOperationException();
		case 1:
			return this;
		default:
		{
			LongArray longArray = Copy();
			int intLen = m + 63 >> 6;
			LongArray longArray2 = new LongArray(intLen);
			ReduceBit(longArray2.m_data, 0, m, m, ks);
			LongArray longArray3 = new LongArray(intLen);
			longArray3.m_data[0] = 1uL;
			LongArray longArray4 = new LongArray(intLen);
			int[] array = new int[2]
			{
				num,
				m + 1
			};
			LongArray[] array2 = new LongArray[2] { longArray, longArray2 };
			int[] array3 = new int[2] { 1, 0 };
			LongArray[] array4 = new LongArray[2] { longArray3, longArray4 };
			int num2 = 1;
			int num3 = array[num2];
			int num4 = array3[num2];
			int num5 = num3 - array[1 - num2];
			while (true)
			{
				if (num5 < 0)
				{
					num5 = -num5;
					array[num2] = num3;
					array3[num2] = num4;
					num2 = 1 - num2;
					num3 = array[num2];
					num4 = array3[num2];
				}
				array2[num2].AddShiftedByBitsSafe(array2[1 - num2], array[1 - num2], num5);
				int num6 = array2[num2].DegreeFrom(num3);
				if (num6 == 0)
				{
					break;
				}
				int num7 = array3[1 - num2];
				array4[num2].AddShiftedByBitsSafe(array4[1 - num2], num7, num5);
				num7 += num5;
				if (num7 > num4)
				{
					num4 = num7;
				}
				else if (num7 == num4)
				{
					num4 = array4[num2].DegreeFrom(num4);
				}
				num5 += num6 - num3;
				num3 = num6;
			}
			return array4[1 - num2];
		}
		}
	}

	public override bool Equals(object obj)
	{
		if (obj is LongArray other)
		{
			return Equals(ref other);
		}
		return false;
	}

	internal bool Equals(ref LongArray other)
	{
		if (AreAliased(ref this, ref other))
		{
			return true;
		}
		int usedLength = GetUsedLength();
		if (other.GetUsedLength() != usedLength)
		{
			return false;
		}
		for (int i = 0; i < usedLength; i++)
		{
			if (m_data[i] != other.m_data[i])
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		return Arrays.GetHashCode(m_data, 0, GetUsedLength());
	}

	public LongArray Copy()
	{
		return new LongArray(Arrays.Clone(m_data));
	}

	public override string ToString()
	{
		int usedLength = GetUsedLength();
		if (usedLength == 0)
		{
			return "0";
		}
		StringBuilder stringBuilder = new StringBuilder(usedLength * 64);
		stringBuilder.Append(Convert.ToString((long)m_data[--usedLength], 2));
		while (--usedLength >= 0)
		{
			string text = Convert.ToString((long)m_data[usedLength], 2);
			int length = text.Length;
			if (length < 64)
			{
				stringBuilder.Append('0', 64 - length);
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}
}
