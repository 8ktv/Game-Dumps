using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

internal sealed class Fpx
{
	private readonly SikeEngine engine;

	internal Fpx(SikeEngine engine)
	{
		this.engine = engine;
	}

	private void mp_shiftl1(ulong[] x, uint nwords)
	{
		for (int num = (int)(nwords - 1); num > 0; num--)
		{
			x[num] = (x[num] << 1) ^ (x[num - 1] >> (int)(Internal.RADIX - 1));
		}
		x[0] <<= 1;
	}

	internal void sqr_Fp2_cycl(ulong[][] a, ulong[] one)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		fpaddPRIME(a[0], a[1], array);
		fpsqr_mont(array, array);
		fpsubPRIME(array, one, a[1]);
		fpsqr_mont(a[0], array);
		fpaddPRIME(array, array, array);
		fpsubPRIME(array, one, a[0]);
	}

	internal void mont_n_way_inv(ulong[][][] vec, uint n, ulong[][][] output)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		fp2copy(vec[0], output[0]);
		for (int i = 1; i < n; i++)
		{
			fp2mul_mont(output[i - 1], vec[i], output[i]);
		}
		fp2copy(output[n - 1], array);
		fp2inv_mont_bingcd(array);
		for (int i = (int)(n - 1); i >= 1; i--)
		{
			fp2mul_mont(output[i - 1], array, output[i]);
			fp2mul_mont(array, vec[i], array);
		}
		fp2copy(array, output[0]);
	}

	internal void fpcopy(ulong[] a, long aOffset, ulong[] c)
	{
		for (uint num = 0u; num < engine.param.NWORDS_FIELD; num++)
		{
			c[num] = a[num + aOffset];
		}
	}

	internal void mp2_add(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		mp_add(a[0], b[0], c[0], engine.param.NWORDS_FIELD);
		mp_add(a[1], b[1], c[1], engine.param.NWORDS_FIELD);
	}

	internal void fp2correction(ulong[][] a)
	{
		fpcorrectionPRIME(a[0]);
		fpcorrectionPRIME(a[1]);
	}

	internal ulong mp_add(ulong[] a, ulong[] b, ulong[] c, uint nwords)
	{
		ulong num = 0uL;
		for (uint num2 = 0u; num2 < nwords; num2++)
		{
			ulong num3 = a[num2] + num;
			c[num2] = b[num2] + num3;
			num = is_digit_lessthan_ct(num3, num) | is_digit_lessthan_ct(c[num2], num3);
		}
		return num;
	}

	private ulong mp_add(ulong[] a, uint aOffset, ulong[] b, ulong[] c, uint cOffset, uint nwords)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < nwords; num2++)
		{
			ulong num3 = a[num2 + aOffset] + num;
			c[num2 + cOffset] = b[num2] + num3;
			num = is_digit_lessthan_ct(num3, num) | is_digit_lessthan_ct(c[num2 + cOffset], num3);
		}
		return num;
	}

	private ulong mp_add(ulong[] a, uint aOffset, ulong[] b, uint bOffset, ulong[] c, uint cOffset, uint nwords)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < nwords; num2++)
		{
			ulong num3 = a[num2 + aOffset] + num;
			c[num2 + cOffset] = b[num2 + bOffset] + num3;
			num = is_digit_lessthan_ct(num3, num) | is_digit_lessthan_ct(c[num2 + cOffset], num3);
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ulong is_digit_lessthan_ct(ulong x, ulong y)
	{
		return (x ^ ((x ^ y) | ((x - y) ^ y))) >> (int)(Internal.RADIX - 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ulong is_digit_nonzero_ct(ulong x)
	{
		return (x | (0 - x)) >> (int)(Internal.RADIX - 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ulong is_digit_zero_ct(ulong x)
	{
		return 1 ^ is_digit_nonzero_ct(x);
	}

	internal void fp2neg(ulong[][] a)
	{
		fpnegPRIME(a[0]);
		fpnegPRIME(a[1]);
	}

	internal bool is_felm_zero(ulong[] x)
	{
		for (uint num = 0u; num < engine.param.NWORDS_FIELD; num++)
		{
			if (x[num] != 0L)
			{
				return false;
			}
		}
		return true;
	}

	private bool is_felm_lt(ulong[] x, ulong[] y)
	{
		for (int num = (int)(engine.param.NWORDS_FIELD - 1); num >= 0; num--)
		{
			if (x[num] < y[num])
			{
				return true;
			}
			if (x[num] > y[num])
			{
				return false;
			}
		}
		return false;
	}

	private static bool is_felm_even(ulong[] x)
	{
		return (x[0] & 1) == 0;
	}

	internal bool is_sqr_fp2(ulong[][] a, ulong[] s)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[engine.param.NWORDS_FIELD];
		fpsqr_mont(a[0], array);
		fpsqr_mont(a[1], array2);
		fpaddPRIME(array, array2, array3);
		fpcopy(array3, 0L, s);
		for (uint num = 0u; num < engine.param.OALICE_BITS - 2; num++)
		{
			fpsqr_mont(s, s);
		}
		for (uint num = 0u; num < engine.param.OBOB_EXPON; num++)
		{
			fpsqr_mont(s, array4);
			fpmul_mont(s, array4, s);
		}
		fpsqr_mont(s, array4);
		fpcorrectionPRIME(array4);
		fpcorrectionPRIME(array3);
		if (!subarrayEquals(array4, array3, engine.param.NWORDS_FIELD))
		{
			return false;
		}
		return true;
	}

	private uint fpinv_mont_bingcd_partial(ulong[] a, ulong[] x1)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		fpcopy(a, 0L, array);
		fpcopy(engine.param.PRIME, 0L, array2);
		fpzero(x1);
		x1[0] = 1uL;
		fpzero(array3);
		uint num = 0u;
		while (!is_felm_zero(array2))
		{
			uint num2 = ++num / Internal.RADIX + 1;
			if (num2 < engine.param.NWORDS_FIELD)
			{
				if (is_felm_even(array2))
				{
					mp_shiftr1(array2);
					mp_shiftl1(x1, num2);
				}
				else if (is_felm_even(array))
				{
					mp_shiftr1(array);
					mp_shiftl1(array3, num2);
				}
				else if (!is_felm_lt(array2, array))
				{
					mp_sub(array2, array, array2, engine.param.NWORDS_FIELD);
					mp_shiftr1(array2);
					mp_add(x1, array3, array3, num2);
					mp_shiftl1(x1, num2);
				}
				else
				{
					mp_sub(array, array2, array, engine.param.NWORDS_FIELD);
					mp_shiftr1(array);
					mp_add(x1, array3, x1, num2);
					mp_shiftl1(array3, num2);
				}
			}
			else if (is_felm_even(array2))
			{
				mp_shiftr1(array2);
				mp_shiftl1(x1, engine.param.NWORDS_FIELD);
			}
			else if (is_felm_even(array))
			{
				mp_shiftr1(array);
				mp_shiftl1(array3, engine.param.NWORDS_FIELD);
			}
			else if (!is_felm_lt(array2, array))
			{
				mp_sub(array2, array, array2, engine.param.NWORDS_FIELD);
				mp_shiftr1(array2);
				mp_add(x1, array3, array3, engine.param.NWORDS_FIELD);
				mp_shiftl1(x1, engine.param.NWORDS_FIELD);
			}
			else
			{
				mp_sub(array, array2, array, engine.param.NWORDS_FIELD);
				mp_shiftr1(array);
				mp_add(x1, array3, x1, engine.param.NWORDS_FIELD);
				mp_shiftl1(array3, engine.param.NWORDS_FIELD);
			}
		}
		if (is_felm_lt(engine.param.PRIME, x1))
		{
			mp_sub(x1, engine.param.PRIME, x1, engine.param.NWORDS_FIELD);
		}
		return num;
	}

	private void power2_setup(ulong[] x, int mark, uint nwords)
	{
		uint num;
		for (num = 0u; num < nwords; num++)
		{
			x[num] = 0uL;
		}
		num = 0u;
		while (mark >= 0)
		{
			if (mark < Internal.RADIX)
			{
				x[num] = (ulong)(1L << mark);
			}
			mark -= (int)Internal.RADIX;
			num++;
		}
	}

	private void fpinv_mont_bingcd(ulong[] a)
	{
		if (!is_felm_zero(a))
		{
			ulong[] array = new ulong[engine.param.NWORDS_FIELD];
			ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
			uint num = fpinv_mont_bingcd_partial(a, array);
			if (num <= engine.param.MAXBITS_FIELD)
			{
				fpmul_mont(array, engine.param.Montgomery_R2, array);
				num += engine.param.MAXBITS_FIELD;
			}
			fpmul_mont(array, engine.param.Montgomery_R2, array);
			power2_setup(array2, (int)(2 * engine.param.MAXBITS_FIELD - num), engine.param.NWORDS_FIELD);
			fpmul_mont(array, array2, a);
		}
	}

	internal void fp2inv_mont_bingcd(ulong[][] a)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		fpsqr_mont(a[0], array[0]);
		fpsqr_mont(a[1], array[1]);
		fpaddPRIME(array[0], array[1], array[0]);
		fpinv_mont_bingcd(array[0]);
		fpnegPRIME(a[1]);
		fpmul_mont(a[0], array[0], a[0]);
		fpmul_mont(a[1], array[0], a[1]);
	}

	internal void fp2div2(ulong[][] a, ulong[][] c)
	{
		fpdiv2_PRIME(a[0], c[0]);
		fpdiv2_PRIME(a[1], c[1]);
	}

	private void fpdiv2_PRIME(ulong[] a, ulong[] c)
	{
		ulong num = 0uL;
		ulong num2 = 0 - (a[0] & 1);
		for (ulong num3 = 0uL; num3 < engine.param.NWORDS_FIELD; num3++)
		{
			ulong num4 = a[num3] + num;
			c[num3] = (engine.param.PRIME[num3] & num2) + num4;
			num = is_digit_lessthan_ct(num4, num) | is_digit_lessthan_ct(c[num3], num4);
		}
		mp_shiftr1(c);
	}

	private void mp_subPRIME_p2(ulong[] a, ulong[] b, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] - b[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2], b[num2]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num5 = c[num2] + num;
			c[num2] = engine.param.PRIMEx2[num2] + num5;
			num = is_digit_lessthan_ct(num5, num) | is_digit_lessthan_ct(c[num2], num5);
		}
	}

	private void mp_subPRIME_p4(ulong[] a, ulong[] b, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] - b[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2], b[num2]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num5 = c[num2] + num;
			c[num2] = engine.param.PRIMEx4[num2] + num5;
			num = is_digit_lessthan_ct(num5, num) | is_digit_lessthan_ct(c[num2], num5);
		}
	}

	private ulong digit_x_digit(ulong a, ulong b, out ulong low)
	{
		ulong num = 4294967295uL;
		ulong num2 = 18446744069414584320uL;
		ulong num3 = a & num;
		ulong num4 = a >> 32;
		ulong num5 = b & num;
		ulong num6 = b >> 32;
		ulong num7 = num3 * num5;
		ulong num8 = num3 * num6;
		ulong num9 = num4 * num5;
		ulong num10 = num4 * num6;
		low = num7 & num;
		ulong num11 = num7 >> 32;
		ulong num12 = num9 & num;
		ulong num13 = num8 & num;
		ulong num14 = num11 + num12 + num13;
		ulong num15 = num14 >> 32;
		low ^= num14 << 32;
		ulong num16 = num9 >> 32;
		num12 = num8 >> 32;
		num13 = num10 & num;
		num14 = num16 + num12 + num13 + num15;
		ulong num17 = num14 & num;
		num15 = num14 & num2;
		return num17 ^ ((num10 & num2) + num15);
	}

	private void rdc_mont(ulong[] ma, ulong[] mc)
	{
		ulong num = engine.param.PRIME_ZERO_WORDS;
		ulong num2 = 0uL;
		ulong num3 = 0uL;
		ulong num4 = 0uL;
		for (ulong num5 = 0uL; num5 < engine.param.NWORDS_FIELD; num5++)
		{
			mc[num5] = 0uL;
		}
		ulong low;
		ulong num8;
		for (ulong num5 = 0uL; num5 < engine.param.NWORDS_FIELD; num5++)
		{
			for (ulong num6 = 0uL; num6 < num5; num6++)
			{
				if (num6 < num5 - engine.param.PRIME_ZERO_WORDS + 1)
				{
					ulong num7 = digit_x_digit(mc[num6], engine.param.PRIMEp1[num5 - num6], out low);
					num4 += low;
					num7 += is_digit_lessthan_ct(num4, low);
					num3 += num7;
					num2 += is_digit_lessthan_ct(num3, num7);
				}
			}
			num8 = ma[num5];
			num4 += num8;
			num8 = is_digit_lessthan_ct(num4, num8);
			num3 += num8;
			num8 &= is_digit_zero_ct(num3);
			num2 += num8;
			mc[num5] = num4;
			num4 = num3;
			num3 = num2;
			num2 = 0uL;
		}
		for (ulong num5 = engine.param.NWORDS_FIELD; num5 < 2 * engine.param.NWORDS_FIELD - 1; num5++)
		{
			if (num != 0)
			{
				num--;
			}
			for (ulong num6 = num5 - engine.param.NWORDS_FIELD + 1; num6 < engine.param.NWORDS_FIELD; num6++)
			{
				if (num6 < engine.param.NWORDS_FIELD - num)
				{
					ulong num7 = digit_x_digit(mc[num6], engine.param.PRIMEp1[num5 - num6], out low);
					num4 += low;
					num7 += is_digit_lessthan_ct(num4, low);
					num3 += num7;
					num2 += is_digit_lessthan_ct(num3, num7);
				}
			}
			num8 = ma[num5];
			num4 += num8;
			num8 = is_digit_lessthan_ct(num4, num8);
			num3 += num8;
			num8 &= is_digit_zero_ct(num3);
			num2 += num8;
			mc[num5 - engine.param.NWORDS_FIELD] = num4;
			num4 = num3;
			num3 = num2;
			num2 = 0uL;
		}
		num8 = ma[2 * engine.param.NWORDS_FIELD - 1];
		num4 += num8;
		num8 = is_digit_lessthan_ct(num4, num8);
		mc[engine.param.NWORDS_FIELD - 1] = num4;
	}

	internal static bool subarrayEquals(ulong[] a, ulong[] b, uint length)
	{
		for (uint num = 0u; num < length; num++)
		{
			if (a[num] != b[num])
			{
				return false;
			}
		}
		return true;
	}

	internal static bool subarrayEquals(ulong[][] a, ulong[][] b, uint length)
	{
		int num = b[0].Length;
		for (uint num2 = 0u; num2 < length; num2++)
		{
			if (a[num2 / num][num2 % num] != b[num2 / num][num2 % num])
			{
				return false;
			}
		}
		return true;
	}

	internal static bool subarrayEquals(ulong[][] a, ulong[][] b, uint bOffset, uint length)
	{
		int num = b[0].Length;
		for (uint num2 = 0u; num2 < length; num2++)
		{
			if (a[num2 / num][num2 % num] != b[(num2 + bOffset) / num][(num2 + bOffset) % num])
			{
				return false;
			}
		}
		return true;
	}

	internal static bool subarrayEquals(ulong[][] a, ulong[] b, uint bOffset, uint length)
	{
		int num = a[0].Length;
		for (uint num2 = 0u; num2 < length; num2++)
		{
			if (a[num2 / num][num2 % num] != b[num2 + bOffset])
			{
				return false;
			}
		}
		return true;
	}

	internal void sqrt_Fp2(ulong[][] u, ulong[][] y)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[engine.param.NWORDS_FIELD];
		fpsqr_mont(u[0], array);
		fpsqr_mont(u[1], array2);
		fpaddPRIME(array, array2, array);
		fpcopy(array, 0L, array2);
		for (uint num = 0u; num < engine.param.OALICE_BITS - 2; num++)
		{
			fpsqr_mont(array2, array2);
		}
		for (uint num = 0u; num < engine.param.OBOB_EXPON; num++)
		{
			fpsqr_mont(array2, array);
			fpmul_mont(array2, array, array2);
		}
		fpaddPRIME(u[0], array2, array);
		fpdiv2_PRIME(array, array);
		fpcopy(array, 0L, array3);
		fpinv_chain_mont(array3);
		fpmul_mont(array, array3, array2);
		fpmul_mont(array3, u[1], array3);
		fpdiv2_PRIME(array3, array3);
		fpsqr_mont(array2, array4);
		fpcorrectionPRIME(array);
		fpcorrectionPRIME(array4);
		if (subarrayEquals(array, array4, engine.param.NWORDS_FIELD))
		{
			fpcopy(array2, 0L, y[0]);
			fpcopy(array3, 0L, y[1]);
		}
		else
		{
			fpnegPRIME(array2);
			fpcopy(array3, 0L, y[0]);
			fpcopy(array2, 0L, y[1]);
		}
	}

	internal void fp2sqr_mont(ulong[][] a, ulong[][] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[engine.param.NWORDS_FIELD];
		mp_add(a[0], a[1], array, engine.param.NWORDS_FIELD);
		mp_subPRIME_p4(a[0], a[1], array2);
		mp_add(a[0], a[0], array3, engine.param.NWORDS_FIELD);
		fpmul_mont(array, array2, c[0]);
		fpmul_mont(array3, a[1], c[1]);
	}

	internal void fpaddPRIME(ulong[] a, ulong[] b, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] + num;
			c[num2] = b[num2] + num3;
			num = is_digit_lessthan_ct(num3, num) | is_digit_lessthan_ct(c[num2], num3);
		}
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num4 = c[num2] - engine.param.PRIMEx2[num2];
			ulong num5 = is_digit_lessthan_ct(c[num2], engine.param.PRIMEx2[num2]) | (num & is_digit_zero_ct(num4));
			c[num2] = num4 - num;
			num = num5;
		}
		ulong num6 = 0 - num;
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num7 = c[num2] + num;
			c[num2] = (engine.param.PRIMEx2[num2] & num6) + num7;
			num = is_digit_lessthan_ct(num7, num) | is_digit_lessthan_ct(c[num2], num7);
		}
	}

	internal void cube_Fp2_cycl(ulong[][] a, ulong[] one)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		fpaddPRIME(a[0], a[0], array);
		fpsqr_mont(array, array);
		fpsubPRIME(array, one, array);
		fpmul_mont(a[1], array, a[1]);
		fpsubPRIME(array, one, array);
		fpsubPRIME(array, one, array);
		fpmul_mont(a[0], array, a[0]);
	}

	internal void fpsubPRIME(ulong[] a, ulong[] b, uint bOffset, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] - b[num2 + bOffset];
			ulong num4 = is_digit_lessthan_ct(a[num2], b[num2 + bOffset]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		ulong num5 = 0 - num;
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num6 = c[num2] + num;
			c[num2] = (engine.param.PRIMEx2[num2] & num5) + num6;
			num = is_digit_lessthan_ct(num6, num) | is_digit_lessthan_ct(c[num2], num6);
		}
	}

	internal void fpsubPRIME(ulong[] a, uint aOffset, ulong[] b, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2 + aOffset] - b[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2 + aOffset], b[num2]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		ulong num5 = 0 - num;
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num6 = c[num2] + num;
			c[num2] = (engine.param.PRIMEx2[num2] & num5) + num6;
			num = is_digit_lessthan_ct(num6, num) | is_digit_lessthan_ct(c[num2], num6);
		}
	}

	internal void fpsubPRIME(ulong[] a, ulong[] b, ulong[] c)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] - b[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2], b[num2]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		ulong num5 = 0 - num;
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num6 = c[num2] + num;
			c[num2] = (engine.param.PRIMEx2[num2] & num5) + num6;
			num = is_digit_lessthan_ct(num6, num) | is_digit_lessthan_ct(c[num2], num6);
		}
	}

	internal void fpnegPRIME(ulong[] a)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = engine.param.PRIMEx2[num2] - a[num2];
			ulong num4 = is_digit_lessthan_ct(engine.param.PRIMEx2[num2], a[num2]) | (num & is_digit_zero_ct(num3));
			a[num2] = num3 - num;
			num = num4;
		}
	}

	internal void from_fp2mont(ulong[][] ma, ulong[][] c)
	{
		from_mont(ma[0], c[0]);
		from_mont(ma[1], c[1]);
	}

	internal void fp2_encode(ulong[][] x, byte[] enc, uint encOffset)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		from_fp2mont(x, array);
		encode_to_bytes(array[0], enc, encOffset, engine.param.FP2_ENCODED_BYTES / 2);
		encode_to_bytes(array[1], enc, encOffset + engine.param.FP2_ENCODED_BYTES / 2, engine.param.FP2_ENCODED_BYTES / 2);
	}

	internal void fp2_decode(byte[] x, ulong[][] dec, uint xOffset)
	{
		decode_to_digits(x, xOffset, dec[0], engine.param.FP2_ENCODED_BYTES / 2, engine.param.NWORDS_FIELD);
		decode_to_digits(x, xOffset + engine.param.FP2_ENCODED_BYTES / 2, dec[1], engine.param.FP2_ENCODED_BYTES / 2, engine.param.NWORDS_FIELD);
		to_fp2mont(dec, dec);
	}

	internal void to_Montgomery_mod_order(ulong[] a, ulong[] mc, ulong[] order, ulong[] Montgomery_rprime, ulong[] Montgomery_Rprime)
	{
		Montgomery_multiply_mod_order(a, Montgomery_Rprime, mc, order, Montgomery_rprime);
	}

	internal void Montgomery_multiply_mod_order(ulong[] ma, ulong[] mb, ulong[] mc, ulong[] order, ulong[] Montgomery_rprime)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		ulong[] array = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_ORDER];
		multiply(ma, mb, array, engine.param.NWORDS_ORDER);
		multiply(array, Montgomery_rprime, array2, engine.param.NWORDS_ORDER);
		multiply(array2, order, array3, engine.param.NWORDS_ORDER);
		num = mp_add(array, array3, array3, 2 * engine.param.NWORDS_ORDER);
		for (ulong num3 = 0uL; num3 < engine.param.NWORDS_ORDER; num3++)
		{
			mc[num3] = array3[engine.param.NWORDS_ORDER + num3];
		}
		num2 = mp_sub(mc, order, mc, engine.param.NWORDS_ORDER);
		ulong num4 = num - num2;
		for (ulong num3 = 0uL; num3 < engine.param.NWORDS_ORDER; num3++)
		{
			array3[num3] = order[num3] & num4;
		}
		mp_add(mc, array3, mc, engine.param.NWORDS_ORDER);
	}

	internal void inv_mod_orderA(ulong[] a, ulong[] c)
	{
		uint num = 0u;
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_ORDER];
		ulong[] array4 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array5 = new ulong[engine.param.NWORDS_ORDER];
		ulong num2 = ulong.MaxValue >> (int)(engine.param.NBITS_ORDER - engine.param.OALICE_BITS);
		array5[engine.param.NWORDS_ORDER - 1] = (ulong)(1L << (int)(64 - (engine.param.NBITS_ORDER - engine.param.OALICE_BITS)));
		array4[0] = 1uL;
		mp_sub(a, array4, array, engine.param.NWORDS_ORDER);
		if ((a[0] & 1) == 0L || is_zero(array, engine.param.NWORDS_ORDER))
		{
			copy_words(a, c, engine.param.NWORDS_ORDER);
			c[engine.param.NWORDS_ORDER - 1] &= num2;
			return;
		}
		mp_sub(array5, array, c, engine.param.NWORDS_ORDER);
		mp_add(c, array4, c, engine.param.NWORDS_ORDER);
		copy_words(array, array2, engine.param.NWORDS_ORDER);
		while ((array2[0] & 1) == 0L)
		{
			num++;
			mp_shiftr1(array2, engine.param.NWORDS_ORDER);
		}
		uint num3 = engine.param.OALICE_BITS / num;
		for (uint num4 = 1u; num4 < num3; num4 <<= 1)
		{
			multiply(array, array, array3, engine.param.NWORDS_ORDER);
			copy_words(array3, array, engine.param.NWORDS_ORDER);
			array[engine.param.NWORDS_ORDER - 1] &= num2;
			mp_add(array, array4, array2, engine.param.NWORDS_ORDER);
			array2[engine.param.NWORDS_ORDER - 1] &= num2;
			multiply(c, array2, array3, engine.param.NWORDS_ORDER);
			copy_words(array3, c, engine.param.NWORDS_ORDER);
			c[engine.param.NWORDS_ORDER - 1] &= num2;
		}
	}

	internal void multiply(ulong[] a, ulong[] b, ulong[] c, uint nwords)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		ulong num3 = 0uL;
		ulong low;
		for (ulong num4 = 0uL; num4 < nwords; num4++)
		{
			for (ulong num5 = 0uL; num5 <= num4; num5++)
			{
				ulong num6 = digit_x_digit(a[num5], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		for (ulong num4 = nwords; num4 < 2 * nwords - 1; num4++)
		{
			for (ulong num5 = num4 - nwords + 1; num5 < nwords; num5++)
			{
				ulong num6 = digit_x_digit(a[num5], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		c[2 * nwords - 1] = num3;
	}

	private bool is_zero_mod_order(ulong[] x)
	{
		for (uint num = 0u; num < engine.param.NWORDS_ORDER; num++)
		{
			if (x[num] != 0L)
			{
				return false;
			}
		}
		return true;
	}

	private bool is_even_mod_order(ulong[] x)
	{
		return (x[0] & 1) == 0;
	}

	private bool is_lt_mod_order(ulong[] x, ulong[] y)
	{
		for (int num = (int)(engine.param.NWORDS_ORDER - 1); num >= 0; num--)
		{
			if (x[num] < y[num])
			{
				return true;
			}
			if (x[num] > y[num])
			{
				return false;
			}
		}
		return false;
	}

	private bool is_zero(ulong[] a, uint nwords)
	{
		for (uint num = 0u; num < nwords; num++)
		{
			if (a[num] != 0L)
			{
				return false;
			}
		}
		return true;
	}

	private uint Montgomery_inversion_mod_order_bingcd_partial(ulong[] a, ulong[] x1, ulong[] order)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array3 = new ulong[engine.param.NWORDS_ORDER];
		copy_words(a, array, engine.param.NWORDS_ORDER);
		copy_words(order, array2, engine.param.NWORDS_ORDER);
		copy_words(array3, x1, engine.param.NWORDS_ORDER);
		x1[0] = 1uL;
		uint num = 0u;
		while (!is_zero_mod_order(array2))
		{
			uint num2 = ++num / Internal.RADIX + 1;
			if (num2 < engine.param.NWORDS_ORDER)
			{
				if (is_even_mod_order(array2))
				{
					mp_shiftr1(array2, engine.param.NWORDS_ORDER);
					mp_shiftl1(x1, num2);
				}
				else if (is_even_mod_order(array))
				{
					mp_shiftr1(array, engine.param.NWORDS_ORDER);
					mp_shiftl1(array3, num2);
				}
				else if (!is_lt_mod_order(array2, array))
				{
					mp_sub(array2, array, array2, engine.param.NWORDS_ORDER);
					mp_shiftr1(array2, engine.param.NWORDS_ORDER);
					mp_add(x1, array3, array3, num2);
					mp_shiftl1(x1, num2);
				}
				else
				{
					mp_sub(array, array2, array, engine.param.NWORDS_ORDER);
					mp_shiftr1(array, engine.param.NWORDS_ORDER);
					mp_add(x1, array3, x1, num2);
					mp_shiftl1(array3, num2);
				}
			}
			else if (is_even_mod_order(array2))
			{
				mp_shiftr1(array2, engine.param.NWORDS_ORDER);
				mp_shiftl1(x1, engine.param.NWORDS_ORDER);
			}
			else if (is_even_mod_order(array))
			{
				mp_shiftr1(array, engine.param.NWORDS_ORDER);
				mp_shiftl1(array3, engine.param.NWORDS_ORDER);
			}
			else if (!is_lt_mod_order(array2, array))
			{
				mp_sub(array2, array, array2, engine.param.NWORDS_ORDER);
				mp_shiftr1(array2, engine.param.NWORDS_ORDER);
				mp_add(x1, array3, array3, engine.param.NWORDS_ORDER);
				mp_shiftl1(x1, engine.param.NWORDS_ORDER);
			}
			else
			{
				mp_sub(array, array2, array, engine.param.NWORDS_ORDER);
				mp_shiftr1(array, engine.param.NWORDS_ORDER);
				mp_add(x1, array3, x1, engine.param.NWORDS_ORDER);
				mp_shiftl1(array3, engine.param.NWORDS_ORDER);
			}
		}
		if (is_lt_mod_order(order, x1))
		{
			mp_sub(x1, order, x1, engine.param.NWORDS_ORDER);
		}
		return num;
	}

	internal void Montgomery_inversion_mod_order_bingcd(ulong[] a, ulong[] c, ulong[] order, ulong[] Montgomery_rprime, ulong[] Montgomery_Rprime)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		if (is_zero(a, engine.param.NWORDS_ORDER))
		{
			copy_words(array2, c, engine.param.NWORDS_ORDER);
			return;
		}
		uint num = Montgomery_inversion_mod_order_bingcd_partial(a, array, order);
		if (num <= engine.param.NBITS_ORDER)
		{
			Montgomery_multiply_mod_order(array, Montgomery_Rprime, array, order, Montgomery_rprime);
			num += engine.param.NBITS_ORDER;
		}
		Montgomery_multiply_mod_order(array, Montgomery_Rprime, array, order, Montgomery_rprime);
		power2_setup(array2, (int)(2 * engine.param.NBITS_ORDER - num), engine.param.NWORDS_ORDER);
		Montgomery_multiply_mod_order(array, array2, c, order, Montgomery_rprime);
	}

	internal void from_Montgomery_mod_order(ulong[] ma, ulong[] c, ulong[] order, ulong[] Montgomery_rprime)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		array[0] = 1uL;
		Montgomery_multiply_mod_order(ma, array, c, order, Montgomery_rprime);
	}

	internal uint mod3(ulong[] a)
	{
		ulong num = 0uL;
		for (int i = 0; i < engine.param.NWORDS_ORDER; i++)
		{
			num += a[i] >> 32;
			num += a[i] & 0xFFFFFFFFu;
		}
		return (uint)(num % 3);
	}

	internal void to_fp2mont(ulong[][] a, ulong[][] mc)
	{
		to_mont(a[0], mc[0]);
		to_mont(a[1], mc[1]);
	}

	private void to_mont(ulong[] a, ulong[] mc)
	{
		fpmul_mont(a, engine.param.Montgomery_R2, mc);
	}

	internal void fpcorrectionPRIME(ulong[] a)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num3 = a[num2] - engine.param.PRIME[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2], engine.param.PRIME[num2]) | (num & is_digit_zero_ct(num3));
			a[num2] = num3 - num;
			num = num4;
		}
		ulong num5 = 0 - num;
		num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			ulong num6 = a[num2] + num;
			a[num2] = (engine.param.PRIME[num2] & num5) + num6;
			num = is_digit_lessthan_ct(num6, num) | is_digit_lessthan_ct(a[num2], num6);
		}
	}

	internal byte cmp_f2elm(ulong[][] x, ulong[][] y)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		ulong[][] array2 = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		byte b = 0;
		fp2copy(x, array);
		fp2copy(y, array2);
		fp2correction(array);
		fp2correction(array2);
		for (int num = (int)(engine.param.NWORDS_FIELD - 1); num >= 0; num--)
		{
			b |= (byte)((array[0][num] ^ array2[0][num]) | (array[1][num] ^ array2[1][num]));
		}
		return (byte)(-b >> 7);
	}

	internal void encode_to_bytes(ulong[] x, byte[] enc, uint encOffset, uint nbytes)
	{
		byte[] array = new byte[(nbytes * 4 + 7) & -8];
		Pack.UInt64_To_LE(x, array, 0);
		Array.Copy(array, 0L, enc, encOffset, nbytes);
	}

	internal void decode_to_digits(byte[] x, uint xOffset, ulong[] dec, uint nbytes, uint ndigits)
	{
		dec[ndigits - 1] = 0uL;
		byte[] array = new byte[(nbytes + 7) & -8];
		Array.Copy(x, xOffset, array, 0L, nbytes);
		Pack.LE_To_UInt64(array, 0, dec);
	}

	internal void fp2_conj(ulong[][] v, ulong[][] r)
	{
		fpcopy(v[0], 0L, r[0]);
		fpcopy(v[1], 0L, r[1]);
		if (!is_felm_zero(r[1]))
		{
			fpnegPRIME(r[1]);
		}
	}

	private void from_mont(ulong[] ma, ulong[] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		array[0] = 1uL;
		fpmul_mont(ma, array, c);
		fpcorrectionPRIME(c);
	}

	private void mp_shiftr1(ulong[] x)
	{
		for (uint num = 0u; num < engine.param.NWORDS_FIELD - 1; num++)
		{
			x[num] = (x[num] >> 1) ^ (x[num + 1] << (int)(Internal.RADIX - 1));
		}
		x[engine.param.NWORDS_FIELD - 1] >>= 1;
	}

	private void mp_shiftr1(ulong[] x, uint nwords)
	{
		for (uint num = 0u; num < nwords - 1; num++)
		{
			x[num] = (x[num] >> 1) ^ (x[num + 1] << (int)(Internal.RADIX - 1));
		}
		x[nwords - 1] >>= 1;
	}

	internal void fp2copy(ulong[][] a, ulong[][] c)
	{
		fpcopy(a[0], 0L, c[0]);
		fpcopy(a[1], 0L, c[1]);
	}

	internal void fp2copy(ulong[][] a, uint aOffset, ulong[][] c)
	{
		fpcopy(a[aOffset], 0L, c[0]);
		fpcopy(a[1 + aOffset], 0L, c[1]);
	}

	internal void fp2copy(ulong[] a, uint aOffset, ulong[][] c)
	{
		fpcopy(a, aOffset, c[0]);
		fpcopy(a, aOffset + engine.param.NWORDS_FIELD, c[1]);
	}

	internal void fpzero(ulong[] a)
	{
		for (uint num = 0u; num < engine.param.NWORDS_FIELD; num++)
		{
			a[num] = 0uL;
		}
	}

	internal void mp2_sub_p2(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		mp_subPRIME_p2(a[0], b[0], c[0]);
		mp_subPRIME_p2(a[1], b[1], c[1]);
	}

	internal void mp_mul(ulong[] a, ulong[] b, ulong[] c, uint nwords)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		ulong num3 = 0uL;
		ulong low;
		for (ulong num4 = 0uL; num4 < nwords; num4++)
		{
			for (ulong num5 = 0uL; num5 <= num4; num5++)
			{
				ulong num6 = digit_x_digit(a[num5], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		for (ulong num4 = nwords; num4 < 2 * nwords - 1; num4++)
		{
			for (ulong num5 = num4 - nwords + 1; num5 < nwords; num5++)
			{
				ulong num6 = digit_x_digit(a[num5], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		c[2 * nwords - 1] = num3;
	}

	internal void mp_mul(ulong[] a, uint aOffset, ulong[] b, ulong[] c, uint nwords)
	{
		ulong num = 0uL;
		ulong num2 = 0uL;
		ulong num3 = 0uL;
		ulong low;
		for (ulong num4 = 0uL; num4 < nwords; num4++)
		{
			for (ulong num5 = 0uL; num5 <= num4; num5++)
			{
				ulong num6 = digit_x_digit(a[num5 + aOffset], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		for (ulong num4 = nwords; num4 < 2 * nwords - 1; num4++)
		{
			for (ulong num5 = num4 - nwords + 1; num5 < nwords; num5++)
			{
				ulong num6 = digit_x_digit(a[num5 + aOffset], b[num4 - num5], out low);
				num3 += low;
				num6 += is_digit_lessthan_ct(num3, low);
				num2 += num6;
				num += is_digit_lessthan_ct(num2, num6);
			}
			c[num4] = num3;
			num3 = num2;
			num2 = num;
			num = 0uL;
		}
		c[2 * nwords - 1] = num3;
	}

	internal void fp2mul_mont(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array5 = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_add(a[0], a[1], array, engine.param.NWORDS_FIELD);
		mp_add(b[0], b[1], array2, engine.param.NWORDS_FIELD);
		mp_mul(a[0], b[0], array3, engine.param.NWORDS_FIELD);
		mp_mul(a[1], b[1], array4, engine.param.NWORDS_FIELD);
		mp_mul(array, array2, array5, engine.param.NWORDS_FIELD);
		mp_dblsubfast(array3, array4, array5);
		mp_subaddfast(array3, array4, array3);
		rdc_mont(array5, c[1]);
		rdc_mont(array3, c[0]);
	}

	internal void fp2mul_mont(ulong[][] a, ulong[][] b, uint bOffset, ulong[][] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array5 = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_add(a[0], a[1], array, engine.param.NWORDS_FIELD);
		mp_add(b[bOffset], b[bOffset + 1], array2, engine.param.NWORDS_FIELD);
		mp_mul(a[0], b[bOffset], array3, engine.param.NWORDS_FIELD);
		mp_mul(a[1], b[bOffset + 1], array4, engine.param.NWORDS_FIELD);
		mp_mul(array, array2, array5, engine.param.NWORDS_FIELD);
		mp_dblsubfast(array3, array4, array5);
		mp_subaddfast(array3, array4, array3);
		rdc_mont(array5, c[1]);
		rdc_mont(array3, c[0]);
	}

	internal void fp2mul_mont(ulong[][] a, ulong[] b, uint bOffset, ulong[][] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array2 = new ulong[engine.param.NWORDS_FIELD];
		ulong[] array3 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array4 = new ulong[2 * engine.param.NWORDS_FIELD];
		ulong[] array5 = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_add(a[0], a[1], array, engine.param.NWORDS_FIELD);
		mp_add(b, bOffset, b, bOffset + engine.param.NWORDS_FIELD, array2, 0u, engine.param.NWORDS_FIELD);
		mp_mul(b, bOffset, a[0], array3, engine.param.NWORDS_FIELD);
		mp_mul(b, bOffset + engine.param.NWORDS_FIELD, a[1], array4, engine.param.NWORDS_FIELD);
		mp_mul(array, array2, array5, engine.param.NWORDS_FIELD);
		mp_dblsubfast(array3, array4, array5);
		mp_subaddfast(array3, array4, array3);
		rdc_mont(array5, c[1]);
		rdc_mont(array3, c[0]);
	}

	private void mp_dblsubfast(ulong[] a, ulong[] b, ulong[] c)
	{
		mp_sub(c, a, c, 2 * engine.param.NWORDS_FIELD);
		mp_sub(c, b, c, 2 * engine.param.NWORDS_FIELD);
	}

	internal ulong mp_sub(ulong[] a, ulong[] b, ulong[] c, uint nwords)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < nwords; num2++)
		{
			ulong num3 = a[num2] - b[num2];
			ulong num4 = is_digit_lessthan_ct(a[num2], b[num2]) | (num & is_digit_zero_ct(num3));
			c[num2] = num3 - num;
			num = num4;
		}
		return num;
	}

	internal bool is_orderelm_lt(ulong[] x, ulong[] y)
	{
		for (int num = (int)(engine.param.NWORDS_ORDER - 1); num >= 0; num--)
		{
			if (x[num] < y[num])
			{
				return true;
			}
			if (x[num] > y[num])
			{
				return false;
			}
		}
		return false;
	}

	private void mp_subaddfast(ulong[] a, ulong[] b, ulong[] c)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		ulong num = 0 - mp_sub(a, b, c, 2 * engine.param.NWORDS_FIELD);
		for (uint num2 = 0u; num2 < engine.param.NWORDS_FIELD; num2++)
		{
			array[num2] = engine.param.PRIME[num2] & num;
		}
		mp_add(c, engine.param.NWORDS_FIELD, array, c, engine.param.NWORDS_FIELD, engine.param.NWORDS_FIELD);
	}

	internal void fpsqr_mont(ulong[] ma, ulong[] mc)
	{
		ulong[] array = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_mul(ma, ma, array, engine.param.NWORDS_FIELD);
		rdc_mont(array, mc);
	}

	private void fpinv_mont(ulong[] a)
	{
		ulong[] array = new ulong[engine.param.NWORDS_FIELD];
		fpcopy(a, 0L, array);
		fpinv_chain_mont(array);
		fpsqr_mont(array, array);
		fpsqr_mont(array, array);
		fpmul_mont(a, array, a);
	}

	internal void fp2inv_mont(ulong[][] a)
	{
		ulong[][] array = SikeUtilities.InitArray(2u, engine.param.NWORDS_FIELD);
		fpsqr_mont(a[0], array[0]);
		fpsqr_mont(a[1], array[1]);
		fpaddPRIME(array[0], array[1], array[0]);
		fpinv_mont(array[0]);
		fpnegPRIME(a[1]);
		fpmul_mont(a[0], array[0], a[0]);
		fpmul_mont(a[1], array[0], a[1]);
	}

	internal void mul3(byte[] a)
	{
		ulong[] array = new ulong[engine.param.NWORDS_ORDER];
		ulong[] array2 = new ulong[engine.param.NWORDS_ORDER];
		decode_to_digits(a, 0u, array, engine.param.SECRETKEY_B_BYTES, engine.param.NWORDS_ORDER);
		mp_add(array, array, array2, engine.param.NWORDS_ORDER);
		mp_add(array, array2, array, engine.param.NWORDS_ORDER);
		encode_to_bytes(array, a, 0u, engine.param.SECRETKEY_B_BYTES);
	}

	internal byte ct_compare(byte[] a, byte[] b, uint len)
	{
		byte b2 = 0;
		for (uint num = 0u; num < len; num++)
		{
			b2 |= (byte)(a[num] ^ b[num]);
		}
		return (byte)(-b2 >> 7);
	}

	internal void ct_cmov(byte[] r, byte[] a, uint len, byte selector)
	{
		for (uint num = 0u; num < len; num++)
		{
			r[num] ^= (byte)(selector & (a[num] ^ r[num]));
		}
	}

	internal void copy_words(ulong[] a, ulong[] c, uint nwords)
	{
		for (uint num = 0u; num < nwords; num++)
		{
			c[num] = a[num];
		}
	}

	internal void fp2shl(ulong[][] a, uint k, ulong[][] c)
	{
		fp2copy(a, c);
		for (uint num = 0u; num < k; num++)
		{
			fp2add(c, c, c);
		}
	}

	internal void copy_words(PointProj a, PointProj c)
	{
		for (uint num = 0u; num < engine.param.NWORDS_FIELD; num++)
		{
			c.X[0][num] = a.X[0][num];
			c.X[1][num] = a.X[1][num];
			c.Z[0][num] = a.Z[0][num];
			c.Z[1][num] = a.Z[1][num];
		}
	}

	internal void Montgomery_neg(ulong[] a, ulong[] order)
	{
		ulong num = 0uL;
		for (ulong num2 = 0uL; num2 < engine.param.NWORDS_ORDER; num2++)
		{
			ulong num3 = order[num2] - a[num2];
			ulong num4 = is_digit_lessthan_ct(order[num2], a[num2]) | (num & is_digit_zero_ct(num3));
			a[num2] = num3 - num;
			num = num4;
		}
	}

	internal void fp2add(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		fpaddPRIME(a[0], b[0], c[0]);
		fpaddPRIME(a[1], b[1], c[1]);
	}

	internal void fp2sub(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		fpsubPRIME(a[0], b[0], c[0]);
		fpsubPRIME(a[1], b[1], c[1]);
	}

	private void mp2_sub_p4(ulong[][] a, ulong[][] b, ulong[][] c)
	{
		mp_subPRIME_p4(a[0], b[0], c[0]);
		mp_subPRIME_p4(a[1], b[1], c[1]);
	}

	internal void fpmul_mont(ulong[] ma, ulong[] mb, ulong[] mc)
	{
		ulong[] array = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_mul(ma, mb, array, engine.param.NWORDS_FIELD);
		rdc_mont(array, mc);
	}

	internal void fpmul_mont(ulong[] ma, uint maOffset, ulong[] mb, ulong[] mc)
	{
		ulong[] array = new ulong[2 * engine.param.NWORDS_FIELD];
		mp_mul(ma, maOffset, mb, array, engine.param.NWORDS_FIELD);
		rdc_mont(array, mc);
	}

	private void fpinv_chain_mont(ulong[] a)
	{
		if (engine.param.NBITS_FIELD == 434)
		{
			ulong[] array = new ulong[engine.param.NWORDS_FIELD];
			ulong[][] array2 = SikeUtilities.InitArray(31u, engine.param.NWORDS_FIELD);
			fpsqr_mont(a, array);
			fpmul_mont(a, array, array2[0]);
			for (uint num = 0u; num <= 29; num++)
			{
				fpmul_mont(array2[num], array, array2[num + 1]);
			}
			fpcopy(a, 0L, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[5], array, array);
			for (uint num = 0u; num < 10; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[14], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[3], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[23], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[13], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[24], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[7], array, array);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[12], array, array);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[30], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[1], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[30], array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[21], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[2], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[19], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[1], array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[24], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[26], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[16], array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[10], array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[6], array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[0], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[20], array, array);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[9], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[25], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[30], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[26], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(a, array, array);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[28], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[6], array, array);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[10], array, array);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array, array);
			}
			fpmul_mont(array2[22], array, array);
			for (uint num2 = 0u; num2 < 35; num2++)
			{
				for (uint num = 0u; num < 6; num++)
				{
					fpsqr_mont(array, array);
				}
				fpmul_mont(array2[30], array, array);
			}
			fpcopy(array, 0L, a);
		}
		if (engine.param.NBITS_FIELD == 503)
		{
			ulong[][] array3 = SikeUtilities.InitArray(15u, engine.param.NWORDS_FIELD);
			ulong[] array4 = new ulong[engine.param.NWORDS_FIELD];
			fpsqr_mont(a, array4);
			fpmul_mont(a, array4, array3[0]);
			for (uint num = 0u; num <= 13; num++)
			{
				fpmul_mont(array3[num], array4, array3[num + 1]);
			}
			fpcopy(a, 0L, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(a, array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[8], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[9], array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[0], array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(a, array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[2], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[8], array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(a, array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[10], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[0], array4, array4);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[10], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[10], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[5], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[2], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[3], array4, array4);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[5], array4, array4);
			for (uint num = 0u; num < 12; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[12], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[8], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[12], array4, array4);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[11], array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[5], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[14], array4, array4);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[14], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[5], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[8], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(a, array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[4], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[6], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[5], array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[7], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(a, array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[0], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[11], array4, array4);
			for (uint num = 0u; num < 5; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[13], array4, array4);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[1], array4, array4);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array4, array4);
			}
			fpmul_mont(array3[10], array4, array4);
			for (uint num2 = 0u; num2 < 49; num2++)
			{
				for (uint num = 0u; num < 5; num++)
				{
					fpsqr_mont(array4, array4);
				}
				fpmul_mont(array3[14], array4, array4);
			}
			fpcopy(array4, 0L, a);
		}
		if (engine.param.NBITS_FIELD == 610)
		{
			ulong[][] array5 = SikeUtilities.InitArray(31u, engine.param.NWORDS_FIELD);
			ulong[] array6 = new ulong[engine.param.NWORDS_FIELD];
			fpsqr_mont(a, array6);
			fpmul_mont(a, array6, array5[0]);
			for (uint num = 0u; num <= 29; num++)
			{
				fpmul_mont(array5[num], array6, array5[num + 1]);
			}
			fpcopy(a, 0L, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[6], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[30], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[25], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[28], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[7], array6, array6);
			for (uint num = 0u; num < 11; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[11], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(a, array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[0], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[3], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[16], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[24], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[28], array6, array6);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[16], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[4], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[3], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[20], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[11], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[14], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[15], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[0], array6, array6);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[15], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[19], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[9], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[5], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[27], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[28], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[29], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[1], array6, array6);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[3], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[2], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[30], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[25], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[28], array6, array6);
			for (uint num = 0u; num < 9; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[22], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[3], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[22], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[7], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[9], array6, array6);
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[4], array6, array6);
			for (uint num = 0u; num < 7; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[20], array6, array6);
			for (uint num = 0u; num < 11; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[10], array6, array6);
			for (uint num = 0u; num < 8; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[26], array6, array6);
			for (uint num = 0u; num < 11; num++)
			{
				fpsqr_mont(array6, array6);
			}
			fpmul_mont(array5[2], array6, array6);
			for (uint num2 = 0u; num2 < 50; num2++)
			{
				for (uint num = 0u; num < 6; num++)
				{
					fpsqr_mont(array6, array6);
				}
				fpmul_mont(array5[30], array6, array6);
			}
			fpcopy(array6, 0L, a);
		}
		if (engine.param.NBITS_FIELD != 751)
		{
			return;
		}
		ulong[][] array7 = SikeUtilities.InitArray(27u, engine.param.NWORDS_FIELD);
		ulong[] array8 = new ulong[engine.param.NWORDS_FIELD];
		fpsqr_mont(a, array8);
		fpmul_mont(a, array8, array7[0]);
		fpmul_mont(array7[0], array8, array7[1]);
		fpmul_mont(array7[1], array8, array7[2]);
		fpmul_mont(array7[2], array8, array7[3]);
		fpmul_mont(array7[3], array8, array7[3]);
		for (uint num = 3u; num <= 8; num++)
		{
			fpmul_mont(array7[num], array8, array7[num + 1]);
		}
		fpmul_mont(array7[9], array8, array7[9]);
		for (uint num = 9u; num <= 20; num++)
		{
			fpmul_mont(array7[num], array8, array7[num + 1]);
		}
		fpmul_mont(array7[21], array8, array7[21]);
		for (uint num = 21u; num <= 24; num++)
		{
			fpmul_mont(array7[num], array8, array7[num + 1]);
		}
		fpmul_mont(array7[25], array8, array7[25]);
		fpmul_mont(array7[25], array8, array7[26]);
		fpcopy(a, 0L, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[20], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[24], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[11], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[8], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[23], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 9; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 10; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[15], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[13], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[26], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[20], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[11], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[10], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[14], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[4], array8, array8);
		for (uint num = 0u; num < 10; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[18], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[1], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[22], array8, array8);
		for (uint num = 0u; num < 10; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[6], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[24], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[9], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[18], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[17], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(a, array8, array8);
		for (uint num = 0u; num < 10; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[16], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[7], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[0], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[12], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[19], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[22], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[25], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[10], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[22], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[18], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[4], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[14], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[13], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[5], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[23], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[21], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[23], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[12], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[9], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[3], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[13], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[17], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[26], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[5], array8, array8);
		for (uint num = 0u; num < 8; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[8], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[2], array8, array8);
		for (uint num = 0u; num < 6; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[11], array8, array8);
		for (uint num = 0u; num < 7; num++)
		{
			fpsqr_mont(array8, array8);
		}
		fpmul_mont(array7[20], array8, array8);
		for (uint num2 = 0u; num2 < 61; num2++)
		{
			for (uint num = 0u; num < 6; num++)
			{
				fpsqr_mont(array8, array8);
			}
			fpmul_mont(array7[26], array8, array8);
		}
		fpcopy(array8, 0L, a);
	}
}
