namespace Mirror.BouncyCastle.Pqc.Crypto.Hqc;

internal class GF2PolynomialCalculator
{
	private readonly int _vecNSize64;

	private readonly int _paramN;

	private readonly long _redMask;

	public GF2PolynomialCalculator(int vecNSize64, int paramN, ulong redMask)
	{
		_vecNSize64 = vecNSize64;
		_paramN = paramN;
		_redMask = (long)redMask;
	}

	internal void MultLongs(long[] res, long[] a, long[] b)
	{
		long[] stack = new long[_vecNSize64 << 3];
		long[] array = new long[(_vecNSize64 << 1) + 1];
		karatsuba(array, 0, a, 0, b, 0, _vecNSize64, stack, 0);
		reduce(res, array);
	}

	private void base_mul(long[] c, int cOffset, long a, long b)
	{
		long num = 0L;
		long num2 = 0L;
		long[] array = new long[16];
		long[] array2 = new long[4];
		array[0] = 0L;
		array[1] = b & 0xFFFFFFFFFFFFFFFL;
		array[2] = array[1] << 1;
		array[3] = array[2] ^ array[1];
		array[4] = array[2] << 1;
		array[5] = array[4] ^ array[1];
		array[6] = array[3] << 1;
		array[7] = array[6] ^ array[1];
		array[8] = array[4] << 1;
		array[9] = array[8] ^ array[1];
		array[10] = array[5] << 1;
		array[11] = array[10] ^ array[1];
		array[12] = array[6] << 1;
		array[13] = array[12] ^ array[1];
		array[14] = array[7] << 1;
		array[15] = array[14] ^ array[1];
		long num3 = 0L;
		long num4 = a & 0xF;
		for (int i = 0; i < 16; i++)
		{
			long num5 = num4 - i;
			num3 ^= array[i] & -(1 - ((num5 | -num5) >>> 63));
		}
		num2 = num3;
		num = 0L;
		for (byte b2 = 4; b2 < 64; b2 += 4)
		{
			num3 = 0L;
			long num6 = (a >> (int)b2) & 0xF;
			for (int j = 0; j < 16; j++)
			{
				long num7 = num6 - j;
				num3 ^= array[j] & -(1 - ((num7 | -num7) >>> 63));
			}
			num2 ^= num3 << (int)b2;
			num ^= num3 >> 64 - b2;
		}
		array2[0] = -((b >> 60) & 1);
		array2[1] = -((b >> 61) & 1);
		array2[2] = -((b >> 62) & 1);
		array2[3] = -((b >> 63) & 1);
		num2 ^= (a << 60) & array2[0];
		num ^= (a >>> 4) & array2[0];
		num2 ^= (a << 61) & array2[1];
		num ^= (a >>> 3) & array2[1];
		num2 ^= (a << 62) & array2[2];
		num ^= (a >>> 2) & array2[2];
		num2 ^= (a << 63) & array2[3];
		num ^= (a >>> 1) & array2[3];
		c[cOffset] = num2;
		c[1 + cOffset] = num;
	}

	private void karatsuba_add1(long[] alh, int alhOffset, long[] blh, int blhOffset, long[] a, int aOffset, long[] b, int bOffset, int size_l, int size_h)
	{
		for (int i = 0; i < size_h; i++)
		{
			alh[i + alhOffset] = a[i + aOffset] ^ a[i + size_l + aOffset];
			blh[i + blhOffset] = b[i + bOffset] ^ b[i + size_l + bOffset];
		}
		if (size_h < size_l)
		{
			alh[size_h + alhOffset] = a[size_h + aOffset];
			blh[size_h + blhOffset] = b[size_h + bOffset];
		}
	}

	private void karatsuba_add2(long[] o, int oOffset, long[] tmp1, int tmp1Offset, long[] tmp2, int tmp2Offset, int size_l, int size_h)
	{
		for (int i = 0; i < 2 * size_l; i++)
		{
			tmp1[i + tmp1Offset] ^= o[i + oOffset];
		}
		for (int j = 0; j < 2 * size_h; j++)
		{
			tmp1[j + tmp1Offset] ^= tmp2[j + tmp2Offset];
		}
		for (int k = 0; k < 2 * size_l; k++)
		{
			o[k + size_l + oOffset] ^= tmp1[k + tmp1Offset];
		}
	}

	private void karatsuba(long[] o, int oOffset, long[] a, int aOffset, long[] b, int bOffset, int size, long[] stack, int stackOffset)
	{
		if (size == 1)
		{
			base_mul(o, oOffset, a[aOffset], b[bOffset]);
			return;
		}
		int num = size / 2;
		int num2 = (size + 1) / 2;
		int num3 = stackOffset;
		int num4 = num3 + num2;
		int num5 = num4 + num2;
		int num6 = oOffset + num2 * 2;
		stackOffset += 4 * num2;
		int aOffset2 = aOffset + num2;
		int bOffset2 = bOffset + num2;
		karatsuba(o, oOffset, a, aOffset, b, bOffset, num2, stack, stackOffset);
		karatsuba(o, num6, a, aOffset2, b, bOffset2, num, stack, stackOffset);
		karatsuba_add1(stack, num3, stack, num4, a, aOffset, b, bOffset, num2, num);
		karatsuba(stack, num5, stack, num3, stack, num4, num2, stack, stackOffset);
		karatsuba_add2(o, oOffset, stack, num5, o, num6, num2, num);
	}

	private void reduce(long[] o, long[] a)
	{
		for (int i = 0; i < _vecNSize64; i++)
		{
			long num = a[i + _vecNSize64 - 1] >>> _paramN;
			long num2 = a[i + _vecNSize64] << (int)(64 - ((ulong)_paramN & 0x3FuL));
			o[i] = a[i] ^ num ^ num2;
		}
		o[_vecNSize64 - 1] &= _redMask;
	}

	internal static void AddLongs(long[] res, long[] a, long[] b)
	{
		for (int i = 0; i < a.Length; i++)
		{
			res[i] = a[i] ^ b[i];
		}
	}
}
