using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct RectInt : IEquatable<RectInt>, IFormattable
{
	public struct PositionEnumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
	{
		private readonly Vector2Int _min;

		private readonly Vector2Int _max;

		private Vector2Int _current;

		public readonly Vector2Int Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return _current;
			}
		}

		readonly object IEnumerator.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Current;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PositionEnumerator(in Vector2Int min, in Vector2Int max)
		{
			_min = min;
			_max = max;
			_current = _min;
			_current.x--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PositionEnumerator GetEnumerator()
		{
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_current.y >= _max.y)
			{
				return false;
			}
			_current.x++;
			if (_current.x >= _max.x)
			{
				_current.x = _min.x;
				if (_current.x >= _max.x)
				{
					return false;
				}
				_current.y++;
				if (_current.y >= _max.y)
				{
					return false;
				}
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_current = _min;
			_current.x--;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IDisposable.Dispose()
		{
		}
	}

	private int m_XMin;

	private int m_YMin;

	private int m_Width;

	private int m_Height;

	private static readonly RectInt kZero = new RectInt(0, 0, 0, 0);

	public int x
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_XMin;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_XMin = value;
		}
	}

	public int y
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_YMin;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_YMin = value;
		}
	}

	public readonly Vector2 center
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Vector2
			{
				x = (float)m_XMin + (float)m_Width * 0.5f,
				y = (float)m_YMin + (float)m_Height * 0.5f
			};
		}
	}

	public Vector2Int min
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector2Int(xMin, yMin);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			xMin = value.x;
			yMin = value.y;
		}
	}

	public Vector2Int max
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector2Int(xMax, yMax);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			xMax = value.x;
			yMax = value.y;
		}
	}

	public int width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Width;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Width = value;
		}
	}

	public int height
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Height;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Height = value;
		}
	}

	public int xMin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Mathf.Min(m_XMin, m_XMin + m_Width);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = xMax;
			m_XMin = value;
			m_Width = num - m_XMin;
		}
	}

	public int yMin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Mathf.Min(m_YMin, m_YMin + m_Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = yMax;
			m_YMin = value;
			m_Height = num - m_YMin;
		}
	}

	public int xMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Mathf.Max(m_XMin, m_XMin + m_Width);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Width = value - m_XMin;
		}
	}

	public int yMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Mathf.Max(m_YMin, m_YMin + m_Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Height = value - m_YMin;
		}
	}

	public Vector2Int position
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector2Int(m_XMin, m_YMin);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_XMin = value.x;
			m_YMin = value.y;
		}
	}

	public Vector2Int size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector2Int(m_Width, m_Height);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Width = value.x;
			m_Height = value.y;
		}
	}

	public static RectInt zero
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return kZero;
		}
	}

	public readonly PositionEnumerator allPositionsWithin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new PositionEnumerator(min, max);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(Vector2Int minPosition, Vector2Int maxPosition)
	{
		min = minPosition;
		max = maxPosition;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(in Vector2Int minPosition, in Vector2Int maxPosition)
	{
		min = minPosition;
		max = maxPosition;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectInt(int xMin, int yMin, int width, int height)
	{
		m_XMin = xMin;
		m_YMin = yMin;
		m_Width = width;
		m_Height = height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectInt(Vector2Int position, Vector2Int size)
	{
		m_XMin = position.x;
		m_YMin = position.y;
		m_Width = size.x;
		m_Height = size.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RectInt(in Vector2Int position, in Vector2Int size)
	{
		m_XMin = position.x;
		m_YMin = position.y;
		m_Width = size.x;
		m_Height = size.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClampToBounds(RectInt bounds)
	{
		int val = bounds.xMin;
		int num = bounds.xMax;
		int val2 = bounds.yMin;
		int num2 = bounds.yMax;
		m_XMin = Math.Max(Math.Min(num, m_XMin), val);
		m_YMin = Math.Max(Math.Min(num2, m_YMin), val2);
		m_Width = Math.Min(num - m_XMin, m_Width);
		m_Height = Math.Min(num2 - m_YMin, m_Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClampToBounds(in RectInt bounds)
	{
		int val = bounds.xMin;
		int num = bounds.xMax;
		int val2 = bounds.yMin;
		int num2 = bounds.yMax;
		m_XMin = Math.Max(Math.Min(num, m_XMin), val);
		m_YMin = Math.Max(Math.Min(num2, m_YMin), val2);
		m_Width = Math.Min(num - m_XMin, m_Width);
		m_Height = Math.Min(num2 - m_YMin, m_Height);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Vector2Int position)
	{
		int num = position.x;
		int num2 = position.y;
		return num >= xMin && num2 >= yMin && num < xMax && num2 < yMax;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(in Vector2Int position)
	{
		int num = position.x;
		int num2 = position.y;
		return num >= xMin && num2 >= yMin && num < xMax && num2 < yMax;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Overlaps(RectInt other)
	{
		return other.xMin < xMax && other.xMax > xMin && other.yMin < yMax && other.yMax > yMin;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Overlaps(in RectInt other)
	{
		return other.xMin < xMax && other.xMax > xMin && other.yMin < yMax && other.yMax > yMin;
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
		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}
		return $"(x:{x.ToString(format, formatProvider)}, y:{y.ToString(format, formatProvider)}, width:{width.ToString(format, formatProvider)}, height:{height.ToString(format, formatProvider)})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(RectInt lhs, RectInt rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(RectInt lhs, RectInt rhs)
	{
		return lhs.m_XMin == rhs.m_XMin && lhs.m_YMin == rhs.m_YMin && lhs.m_Width == rhs.m_Width && lhs.m_Height == rhs.m_Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		int hashCode = m_XMin.GetHashCode();
		int hashCode2 = m_YMin.GetHashCode();
		int hashCode3 = m_Width.GetHashCode();
		int hashCode4 = m_Height.GetHashCode();
		return hashCode ^ (hashCode2 << 4) ^ (hashCode2 >> 28) ^ (hashCode3 >> 4) ^ (hashCode3 << 28) ^ (hashCode4 >> 4) ^ (hashCode4 << 28);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is RectInt other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(RectInt other)
	{
		return m_XMin == other.m_XMin && m_YMin == other.m_YMin && m_Width == other.m_Width && m_Height == other.m_Height;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in RectInt other)
	{
		return m_XMin == other.m_XMin && m_YMin == other.m_YMin && m_Width == other.m_Width && m_Height == other.m_Height;
	}
}
