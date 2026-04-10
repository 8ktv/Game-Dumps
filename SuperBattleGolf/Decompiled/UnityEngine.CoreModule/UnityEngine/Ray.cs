using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace UnityEngine;

public struct Ray : IFormattable
{
	private Vector3 m_Origin;

	private Vector3 m_Direction;

	public Vector3 origin
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

	public Vector3 direction
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
	public Ray(Vector3 origin, Vector3 direction)
	{
		m_Origin = origin;
		m_Direction = direction;
		m_Direction.Normalize();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Ray(in Vector3 origin, in Vector3 direction)
	{
		m_Origin = origin;
		m_Direction = direction;
		m_Direction.Normalize();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 GetPoint(float distance)
	{
		return new Vector3
		{
			x = m_Origin.x + m_Direction.x * distance,
			y = m_Origin.y + m_Direction.y * distance,
			z = m_Origin.z + m_Direction.z * distance
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
