using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace System;

public readonly struct Half : IComparable, IComparable<Half>, IEquatable<Half>
{
	private const NumberStyles DefaultParseStyle = NumberStyles.Float | NumberStyles.AllowThousands;

	internal const ushort SignMask = 32768;

	internal const int SignShift = 15;

	internal const byte ShiftedSignMask = 1;

	internal const ushort BiasedExponentMask = 31744;

	internal const int BiasedExponentShift = 10;

	internal const int BiasedExponentLength = 5;

	internal const byte ShiftedBiasedExponentMask = 31;

	internal const ushort TrailingSignificandMask = 1023;

	internal const byte MinSign = 0;

	internal const byte MaxSign = 1;

	internal const byte MinBiasedExponent = 0;

	internal const byte MaxBiasedExponent = 31;

	internal const byte ExponentBias = 15;

	internal const sbyte MinExponent = -14;

	internal const sbyte MaxExponent = 15;

	internal const ushort MinTrailingSignificand = 0;

	internal const ushort MaxTrailingSignificand = 1023;

	internal const int TrailingSignificandLength = 10;

	internal const int SignificandLength = 11;

	private const ushort PositiveZeroBits = 0;

	private const ushort NegativeZeroBits = 32768;

	private const ushort EpsilonBits = 1;

	private const ushort PositiveInfinityBits = 31744;

	private const ushort NegativeInfinityBits = 64512;

	private const ushort PositiveQNaNBits = 32256;

	private const ushort NegativeQNaNBits = 65024;

	private const ushort MinValueBits = 64511;

	private const ushort MaxValueBits = 31743;

	private const ushort PositiveOneBits = 15360;

	private const ushort NegativeOneBits = 48128;

	private const ushort SmallestNormalBits = 1024;

	private const ushort EBits = 16752;

	private const ushort PiBits = 16968;

	private const ushort TauBits = 17992;

	internal readonly ushort _value;

	public static Half Epsilon => new Half(1);

	public static Half PositiveInfinity => new Half(31744);

	public static Half NegativeInfinity => new Half(64512);

	public static Half NaN => new Half(65024);

	public static Half MinValue => new Half(64511);

	public static Half MaxValue => new Half(31743);

	internal byte BiasedExponent => ExtractBiasedExponentFromBits(_value);

	internal sbyte Exponent => (sbyte)(BiasedExponent - 15);

	internal ushort Significand => (ushort)(TrailingSignificand | ((BiasedExponent != 0) ? 1024 : 0));

	internal ushort TrailingSignificand => ExtractTrailingSignificandFromBits(_value);

	public static Half E => new Half(16752);

	public static Half Pi => new Half(16968);

	public static Half Tau => new Half(17992);

	public static Half NegativeZero => new Half(32768);

	public static Half MultiplicativeIdentity => new Half(15360);

	public static Half One => new Half(15360);

	public static Half Zero => new Half(0);

	public static Half NegativeOne => new Half(48128);

	internal Half(ushort value)
	{
		_value = value;
	}

	private Half(bool sign, ushort exp, ushort sig)
	{
		_value = (ushort)(((sign ? 1 : 0) << 15) + (exp << 10) + sig);
	}

	internal static byte ExtractBiasedExponentFromBits(ushort bits)
	{
		return (byte)((bits >> 10) & 0x1F);
	}

	internal static ushort ExtractTrailingSignificandFromBits(ushort bits)
	{
		return (ushort)(bits & 0x3FF);
	}

	public static bool operator <(Half left, Half right)
	{
		if (IsNaN(left) || IsNaN(right))
		{
			return false;
		}
		bool flag = IsNegative(left);
		if (flag != IsNegative(right))
		{
			if (flag)
			{
				return !AreZero(left, right);
			}
			return false;
		}
		if (left._value != right._value)
		{
			return (left._value < right._value) ^ flag;
		}
		return false;
	}

	public static bool operator >(Half left, Half right)
	{
		return right < left;
	}

	public static bool operator <=(Half left, Half right)
	{
		if (IsNaN(left) || IsNaN(right))
		{
			return false;
		}
		bool flag = IsNegative(left);
		if (flag != IsNegative(right))
		{
			if (!flag)
			{
				return AreZero(left, right);
			}
			return true;
		}
		if (left._value != right._value)
		{
			return (left._value < right._value) ^ flag;
		}
		return true;
	}

	public static bool operator >=(Half left, Half right)
	{
		return right <= left;
	}

	public static bool operator ==(Half left, Half right)
	{
		if (IsNaN(left) || IsNaN(right))
		{
			return false;
		}
		if (left._value != right._value)
		{
			return AreZero(left, right);
		}
		return true;
	}

	public static bool operator !=(Half left, Half right)
	{
		return !(left == right);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFinite(Half value)
	{
		return (~value._value & 0x7C00) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInfinity(Half value)
	{
		return ((ulong)value._value & 0xFFFFFFFFFFFF7FFFuL) == 31744;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(Half value)
	{
		return (long)((ulong)value._value & 0xFFFFFFFFFFFF7FFFuL) > 31744L;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsNaNOrZero(Half value)
	{
		return ((uint)(value._value - 1) & -32769) >= 31744;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegative(Half value)
	{
		return (short)value._value < 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNegativeInfinity(Half value)
	{
		return value._value == 64512;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNormal(Half value)
	{
		return (ushort)(((ulong)value._value & 0xFFFFFFFFFFFF7FFFuL) - 1024) < 30720;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsPositiveInfinity(Half value)
	{
		return value._value == 31744;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSubnormal(Half value)
	{
		return (ushort)(((ulong)value._value & 0xFFFFFFFFFFFF7FFFuL) - 1) < 1023;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsZero(Half value)
	{
		return ((ulong)value._value & 0xFFFFFFFFFFFF7FFFuL) == 0;
	}

	private static bool AreZero(Half left, Half right)
	{
		return ((left._value | right._value) & -32769) == 0;
	}

	public int CompareTo(object obj)
	{
		if (obj is Half other)
		{
			return CompareTo(other);
		}
		if (obj != null)
		{
			throw new ArgumentException("SR.Arg_MustBeHalf");
		}
		return 1;
	}

	public int CompareTo(Half other)
	{
		if (this < other)
		{
			return -1;
		}
		if (this > other)
		{
			return 1;
		}
		if (this == other)
		{
			return 0;
		}
		if (IsNaN(this))
		{
			if (!IsNaN(other))
			{
				return -1;
			}
			return 0;
		}
		return 1;
	}

	public override bool Equals(object obj)
	{
		if (obj is Half other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(Half other)
	{
		if (_value != other._value && !AreZero(this, other))
		{
			if (IsNaN(this))
			{
				return IsNaN(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		uint num = _value;
		if (IsNaNOrZero(this))
		{
			num &= 0x7C00;
		}
		return (int)num;
	}

	public override string ToString()
	{
		return ((float)this).ToString();
	}

	public static explicit operator Half(char value)
	{
		return (Half)(float)(int)value;
	}

	public static explicit operator Half(decimal value)
	{
		return (Half)(float)value;
	}

	public static explicit operator Half(short value)
	{
		return (Half)(float)value;
	}

	public static explicit operator Half(int value)
	{
		return (Half)(float)value;
	}

	public static explicit operator Half(long value)
	{
		return (Half)(float)value;
	}

	public static explicit operator Half(float value)
	{
		return new Half(Mathf.FloatToHalf(value));
	}

	public static explicit operator Half(ushort value)
	{
		return (Half)(float)(int)value;
	}

	public static explicit operator Half(uint value)
	{
		return (Half)(float)value;
	}

	public static explicit operator Half(ulong value)
	{
		return (Half)(float)value;
	}

	public static explicit operator byte(Half value)
	{
		return (byte)(float)value;
	}

	public static explicit operator char(Half value)
	{
		return (char)(float)value;
	}

	public static explicit operator decimal(Half value)
	{
		return (decimal)(float)value;
	}

	public static explicit operator short(Half value)
	{
		return (short)(float)value;
	}

	public static explicit operator int(Half value)
	{
		return (int)(float)value;
	}

	public static explicit operator long(Half value)
	{
		return (long)(float)value;
	}

	public static explicit operator sbyte(Half value)
	{
		return (sbyte)(float)value;
	}

	public static explicit operator ushort(Half value)
	{
		return (ushort)(float)value;
	}

	public static explicit operator uint(Half value)
	{
		return (uint)(float)value;
	}

	public static explicit operator ulong(Half value)
	{
		return (ulong)(float)value;
	}

	public static implicit operator Half(byte value)
	{
		return (Half)(float)(int)value;
	}

	public static implicit operator Half(sbyte value)
	{
		return (Half)(float)value;
	}

	public static explicit operator float(Half value)
	{
		return Mathf.HalfToFloat(value._value);
	}

	internal static Half Negate(Half value)
	{
		if (!IsNaN(value))
		{
			return new Half((ushort)(value._value ^ 0x8000));
		}
		return value;
	}

	public static Half operator +(Half left, Half right)
	{
		return (Half)((float)left + (float)right);
	}

	public static Half operator --(Half value)
	{
		return (Half)((float)value - 1f);
	}

	public static Half operator /(Half left, Half right)
	{
		return (Half)((float)left / (float)right);
	}

	public static Half Exp(Half x)
	{
		return (Half)(float)Math.Exp((float)x);
	}

	public static Half Ceiling(Half x)
	{
		return (Half)(float)Math.Ceiling((float)x);
	}

	public static Half Floor(Half x)
	{
		return (Half)(float)Math.Floor((float)x);
	}

	public static Half Round(Half x)
	{
		return (Half)(float)Math.Round((float)x);
	}

	public static Half Round(Half x, int digits)
	{
		return (Half)(float)Math.Round((float)x, digits);
	}

	public static Half Round(Half x, MidpointRounding mode)
	{
		return (Half)(float)Math.Round((float)x, mode);
	}

	public static Half Round(Half x, int digits, MidpointRounding mode)
	{
		return (Half)(float)Math.Round((float)x, digits, mode);
	}

	public static Half Truncate(Half x)
	{
		return (Half)(float)Math.Truncate((float)x);
	}

	public static Half Atan2(Half y, Half x)
	{
		return (Half)(float)Math.Atan2((float)y, (float)x);
	}

	public static Half Lerp(Half value1, Half value2, Half amount)
	{
		return (Half)Mathf.Lerp((float)value1, (float)value2, (float)amount);
	}

	public static Half Cosh(Half x)
	{
		return (Half)(float)Math.Cosh((float)x);
	}

	public static Half Sinh(Half x)
	{
		return (Half)(float)Math.Sinh((float)x);
	}

	public static Half Tanh(Half x)
	{
		return (Half)(float)Math.Tanh((float)x);
	}

	public static Half operator ++(Half value)
	{
		return (Half)((float)value + 1f);
	}

	public static Half Log(Half x)
	{
		return (Half)(float)Math.Log((float)x);
	}

	public static Half Log(Half x, Half newBase)
	{
		return (Half)(float)Math.Log((float)x, (float)newBase);
	}

	public static Half operator %(Half left, Half right)
	{
		return (Half)((float)left % (float)right);
	}

	public static Half operator *(Half left, Half right)
	{
		return (Half)((float)left * (float)right);
	}

	public static Half Clamp(Half value, Half min, Half max)
	{
		return (Half)Mathf.Clamp((float)value, (float)min, (float)max);
	}

	public static Half CopySign(Half value, Half sign)
	{
		ushort value2 = value._value;
		uint value3 = sign._value;
		return new Half((ushort)(((ulong)value2 & 0xFFFFFFFFFFFF7FFFuL) | (value3 & 0x8000)));
	}

	public static Half Max(Half x, Half y)
	{
		return (Half)Math.Max((float)x, (float)y);
	}

	public static Half MaxNumber(Half x, Half y)
	{
		if (x != y)
		{
			if (!IsNaN(y))
			{
				if (!(y < x))
				{
					return y;
				}
				return x;
			}
			return x;
		}
		if (!IsNegative(y))
		{
			return y;
		}
		return x;
	}

	public static Half Min(Half x, Half y)
	{
		return (Half)Math.Min((float)x, (float)y);
	}

	public static Half MinNumber(Half x, Half y)
	{
		if (x != y)
		{
			if (!IsNaN(y))
			{
				if (!(x < y))
				{
					return y;
				}
				return x;
			}
			return x;
		}
		if (!IsNegative(x))
		{
			return y;
		}
		return x;
	}

	public static int Sign(Half value)
	{
		if (IsNaN(value))
		{
			throw new ArithmeticException("SR.Arithmetic_NaN");
		}
		if (IsZero(value))
		{
			return 0;
		}
		if (IsNegative(value))
		{
			return -1;
		}
		return 1;
	}

	public static Half Abs(Half value)
	{
		return new Half((ushort)(value._value & -32769));
	}

	public static bool IsPositive(Half value)
	{
		return (short)value._value >= 0;
	}

	public static bool IsRealNumber(Half value)
	{
		return value == value;
	}

	public static Half Pow(Half x, Half y)
	{
		return (Half)(float)Math.Pow((float)x, (float)y);
	}

	public static Half Sqrt(Half x)
	{
		return (Half)(float)Math.Sqrt((float)x);
	}

	public static Half operator -(Half left, Half right)
	{
		return (Half)((float)left - (float)right);
	}

	public static Half Acos(Half x)
	{
		return (Half)(float)Math.Acos((float)x);
	}

	public static Half Asin(Half x)
	{
		return (Half)(float)Math.Asin((float)x);
	}

	public static Half Atan(Half x)
	{
		return (Half)(float)Math.Atan((float)x);
	}

	public static Half Cos(Half x)
	{
		return (Half)(float)Math.Cos((float)x);
	}

	public static Half Sin(Half x)
	{
		return (Half)(float)Math.Sin((float)x);
	}

	public static Half Tan(Half x)
	{
		return (Half)(float)Math.Tan((float)x);
	}

	public static Half operator -(Half value)
	{
		return (Half)(0f - (float)value);
	}

	public static Half operator +(Half value)
	{
		return value;
	}
}
