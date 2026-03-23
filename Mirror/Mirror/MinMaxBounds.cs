using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public struct MinMaxBounds : IEquatable<Bounds>
{
	public Vector3 min;

	public Vector3 max;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Encapsulate(Vector3 point)
	{
		min = Vector3.Min(min, point);
		max = Vector3.Max(max, point);
	}

	public void Encapsulate(MinMaxBounds bounds)
	{
		Encapsulate(bounds.min);
		Encapsulate(bounds.max);
	}

	public static bool operator ==(MinMaxBounds lhs, Bounds rhs)
	{
		if (lhs.min == rhs.min)
		{
			return lhs.max == rhs.max;
		}
		return false;
	}

	public static bool operator !=(MinMaxBounds lhs, Bounds rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is MinMaxBounds minMaxBounds && min == minMaxBounds.min)
		{
			return max == minMaxBounds.max;
		}
		return false;
	}

	public bool Equals(MinMaxBounds other)
	{
		if (min.Equals(other.min))
		{
			return max.Equals(other.max);
		}
		return false;
	}

	public bool Equals(Bounds other)
	{
		if (min.Equals(other.min))
		{
			return max.Equals(other.max);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(min, max);
	}

	public override string ToString()
	{
		return $"({min}, {max})";
	}
}
