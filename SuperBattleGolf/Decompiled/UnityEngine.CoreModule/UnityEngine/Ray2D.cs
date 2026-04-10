using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityEngine;

public struct Ray2D : IFormattable
{
	private Vector2 m_Origin;

	private Vector2 m_Direction;

	public Vector2 origin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Origin;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Origin = value;
		}
	}

	public Vector2 direction
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Direction;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Direction = value;
			m_Direction.Normalize();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Ray2D(Vector2 origin, Vector2 direction)
	{
		m_Origin = origin;
		m_Direction = direction;
		m_Direction.Normalize();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Ray2D(in Vector2 origin, in Vector2 direction)
	{
		m_Origin = origin;
		m_Direction = direction;
		m_Direction.Normalize();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector2 GetPoint(float distance)
	{
		return new Vector2
		{
			x = m_Origin.x + m_Direction.x * distance,
			y = m_Origin.y + m_Direction.y * distance
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly string ToString()
	{
		return ToString(null, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string format)
	{
		return ToString(format, null);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly string ToString(string format, IFormatProvider formatProvider)
	{
		if (string.IsNullOrEmpty(format))
		{
			format = "F2";
		}
		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}
		return $"Origin: {m_Origin.ToString(format, formatProvider)}, Dir: {m_Direction.ToString(format, formatProvider)}";
	}
}
