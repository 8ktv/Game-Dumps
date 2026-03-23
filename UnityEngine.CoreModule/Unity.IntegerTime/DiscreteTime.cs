using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.IntegerTime;

[Serializable]
public struct DiscreteTime : IEquatable<DiscreteTime>, IFormattable, IComparable<DiscreteTime>
{
	[StructLayout(LayoutKind.Explicit)]
	private struct LongDoubleUnion
	{
		[FieldOffset(0)]
		public long longValue;

		[FieldOffset(0)]
		public double doubleValue;
	}

	[SerializeField]
	public long Value;

	public static readonly DiscreteTime Zero = default(DiscreteTime);

	public static readonly DiscreteTime MinValue = new DiscreteTime(long.MinValue, 0);

	public static readonly DiscreteTime MaxValue = new DiscreteTime(long.MaxValue, 0);

	private const int Pow2Exp = 9;

	private const uint Pow2Tps = 512u;

	private const uint NonPow2Tps = 275625u;

	private static readonly int TicksPerSecondBits = (int)Mathf.Ceil(Mathf.Log(141120000f, 2f));

	private static readonly int NonPow2TpsBits = (int)Mathf.Ceil(Mathf.Log(275625f, 2f));

	public const uint TicksPerSecond = 141120000u;

	public const double Tick = 7.0861678004535145E-09;

	public const long MaxValueSeconds = 65358361939L;

	public const long MinValueSeconds = -65358361939L;

	public const uint Tick5Fps = 28224000u;

	public const uint Tick10Fps = 14112000u;

	public const uint Tick12Fps = 11760000u;

	public const uint Tick15Fps = 9408000u;

	public const uint Tick2397Fps = 5885880u;

	public const uint Tick24Fps = 5880000u;

	public const uint Tick25Fps = 5644800u;

	public const uint Tick2997Fps = 4708704u;

	public const uint Tick30Fps = 4704000u;

	public const uint Tick48Fps = 2940000u;

	public const uint Tick50Fps = 2822400u;

	public const uint Tick5995Fps = 2354352u;

	public const uint Tick60Fps = 2352000u;

	public const uint Tick90Fps = 1568000u;

	public const uint Tick11988Fps = 1177176u;

	public const uint Tick120Fps = 1176000u;

	public const uint Tick240Fps = 588000u;

	public const uint Tick1000Fps = 141120u;

	public const uint Tick8Khz = 17640u;

	public const uint Tick16Khz = 8820u;

	public const uint Tick22Khz = 6400u;

	public const uint Tick44Khz = 3200u;

	public const uint Tick48Khz = 2940u;

	public const uint Tick88Khz = 1600u;

	public const uint Tick96Khz = 1470u;

	public const uint Tick192Khz = 735u;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiscreteTime(DiscreteTime x)
	{
		Value = x.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiscreteTime(float v)
	{
		Value = (long)Math.Round((double)v * 141120000.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiscreteTime(double v)
	{
		Value = (long)Math.Round(v * 141120000.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiscreteTime(long v)
	{
		Value = v * 141120000;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiscreteTime(int v)
	{
		Value = (long)v * 141120000L;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private DiscreteTime(long v, int _)
	{
		Value = v;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime FromTicks(long v)
	{
		return new DiscreteTime(v, 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator DiscreteTime(float v)
	{
		return new DiscreteTime(v);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator DiscreteTime(double v)
	{
		return new DiscreteTime(v);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator float(DiscreteTime d)
	{
		return (float)((double)d.Value / 141120000.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator double(DiscreteTime d)
	{
		return (double)d.Value / 141120000.0;
	}

	public static implicit operator RationalTime(DiscreteTime t)
	{
		return new RationalTime(t.Value, RationalTime.TicksPerSecond.DiscreteTimeRate);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value == rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value != rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value < rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value > rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator <=(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value <= rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator >=(DiscreteTime lhs, DiscreteTime rhs)
	{
		return lhs.Value >= rhs.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator +(DiscreteTime lhs, DiscreteTime rhs)
	{
		return FromTicks(lhs.Value + rhs.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator -(DiscreteTime lhs, DiscreteTime rhs)
	{
		return FromTicks(lhs.Value - rhs.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator *(DiscreteTime lhs, long rhs)
	{
		return FromTicks(lhs.Value * rhs);
	}

	public static DiscreteTime operator *(DiscreteTime lhs, double s)
	{
		double i;
		double num = Modf(s, out i);
		long num2 = lhs.Value * (long)i;
		if (Math.Abs(num) >= 7.0861678004535145E-09)
		{
			int num3 = Lzcnt(Math.Abs(lhs.Value)) - 1;
			long num4 = 1 << num3;
			if (num3 >= TicksPerSecondBits)
			{
				num4 = 141120000L;
			}
			else if (num3 >= NonPow2TpsBits)
			{
				num4 = (uint)(275625 << num3 - NonPow2TpsBits);
			}
			long num5 = (long)Math.Round((double)num4 / num);
			num2 += lhs.Value * num4 / num5;
		}
		return FromTicks(num2);
	}

	private static double Modf(double x, out double i)
	{
		i = Math.Truncate(x);
		return x - i;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static int Lzcnt(long x)
	{
		if (x == 0)
		{
			return 64;
		}
		uint num = (uint)(x >> 32);
		uint num2 = (uint)((num != 0) ? num : x);
		int num3 = ((num != 0) ? 1054 : 1086);
		LongDoubleUnion longDoubleUnion = default(LongDoubleUnion);
		longDoubleUnion.doubleValue = 0.0;
		longDoubleUnion.longValue = 4841369599423283200L + (long)num2;
		longDoubleUnion.doubleValue -= 4503599627370496.0;
		return num3 - (int)(longDoubleUnion.longValue >> 52);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator *(DiscreteTime lhs, float s)
	{
		return lhs * (double)s;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator /(DiscreteTime lhs, double s)
	{
		return lhs * (1.0 / s);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator /(DiscreteTime lhs, long s)
	{
		return FromTicks(lhs.Value / s);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator %(DiscreteTime lhs, DiscreteTime rhs)
	{
		return FromTicks(lhs.Value % rhs.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static DiscreteTime operator -(DiscreteTime lhs)
	{
		return FromTicks(-lhs.Value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(DiscreteTime rhs)
	{
		return Value == rhs.Value;
	}

	public override readonly bool Equals(object o)
	{
		return Equals((DiscreteTime)o);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public override readonly string ToString()
	{
		return ((double)this).ToString();
	}

	public readonly string ToString(string format, IFormatProvider formatProvider)
	{
		return ((double)this).ToString(format, formatProvider);
	}

	public readonly int CompareTo(DiscreteTime other)
	{
		return Value.CompareTo(other.Value);
	}
}
