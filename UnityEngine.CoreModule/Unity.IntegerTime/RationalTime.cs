using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IntegerTime;

[Serializable]
[NativeHeader("Runtime/Input/RationalTime.h")]
public struct RationalTime
{
	[Serializable]
	public struct TicksPerSecond : IEquatable<TicksPerSecond>
	{
		private const uint k_DefaultTicksPerSecond = 141120000u;

		[SerializeField]
		private uint m_Numerator;

		[SerializeField]
		private uint m_Denominator;

		public static readonly TicksPerSecond DefaultTicksPerSecond = new TicksPerSecond(141120000u);

		public static readonly TicksPerSecond TicksPerSecond24 = new TicksPerSecond(24u);

		public static readonly TicksPerSecond TicksPerSecond25 = new TicksPerSecond(25u);

		public static readonly TicksPerSecond TicksPerSecond30 = new TicksPerSecond(30u);

		public static readonly TicksPerSecond TicksPerSecond50 = new TicksPerSecond(50u);

		public static readonly TicksPerSecond TicksPerSecond60 = new TicksPerSecond(60u);

		public static readonly TicksPerSecond TicksPerSecond120 = new TicksPerSecond(120u);

		public static readonly TicksPerSecond TicksPerSecond2397 = new TicksPerSecond(24000u, 1001u);

		public static readonly TicksPerSecond TicksPerSecond2425 = new TicksPerSecond(25000u, 1001u);

		public static readonly TicksPerSecond TicksPerSecond2997 = new TicksPerSecond(30000u, 1001u);

		public static readonly TicksPerSecond TicksPerSecond5994 = new TicksPerSecond(60000u, 1001u);

		public static readonly TicksPerSecond TicksPerSecond11988 = new TicksPerSecond(120000u, 1001u);

		internal static readonly TicksPerSecond DiscreteTimeRate = new TicksPerSecond(141120000u);

		public readonly uint Numerator => m_Numerator;

		public readonly uint Denominator => m_Denominator;

		public readonly bool Valid => IsValid(this);

		public TicksPerSecond(uint num, uint den = 1u)
		{
			m_Numerator = num;
			m_Denominator = den;
			Simplify(ref m_Numerator, ref m_Denominator);
		}

		public readonly bool Equals(TicksPerSecond rhs)
		{
			return m_Numerator == rhs.m_Numerator && m_Denominator == rhs.m_Denominator;
		}

		public override readonly bool Equals(object rhs)
		{
			return rhs is TicksPerSecond rhs2 && Equals(rhs2);
		}

		public override readonly int GetHashCode()
		{
			return HashCode.Combine(m_Numerator, m_Denominator);
		}

		private static void Simplify(ref uint num, ref uint den)
		{
			if (den > 1 && num != 0)
			{
				uint num2 = Gcd(num, den);
				num /= num2;
				den /= num2;
			}
		}

		private static uint Gcd(uint a, uint b)
		{
			while (true)
			{
				if (a == 0)
				{
					return b;
				}
				b %= a;
				if (b == 0)
				{
					break;
				}
				a %= b;
			}
			return a;
		}

		[FreeFunction("IntegerTime::TicksPerSecond::IsValid", IsFreeFunction = true, ThrowsException = false)]
		private static bool IsValid(TicksPerSecond tps)
		{
			return IsValid_Injected(ref tps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValid_Injected([In] ref TicksPerSecond tps);
	}

	[SerializeField]
	private long m_Count;

	[SerializeField]
	private TicksPerSecond m_TicksPerSecond;

	public long Count => m_Count;

	public TicksPerSecond Ticks => m_TicksPerSecond;

	public RationalTime(long count, TicksPerSecond ticks)
	{
		m_Count = count;
		m_TicksPerSecond = ticks;
	}

	[FreeFunction("IntegerTime::RationalTime::FromDouble", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime FromDouble(double t, TicksPerSecond ticksPerSecond)
	{
		FromDouble_Injected(t, ref ticksPerSecond, out var ret);
		return ret;
	}

	public static explicit operator DiscreteTime(RationalTime t)
	{
		return DiscreteTime.FromTicks(t.Convert(TicksPerSecond.DiscreteTimeRate).Count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FromDouble_Injected(double t, [In] ref TicksPerSecond ticksPerSecond, out RationalTime ret);
}
