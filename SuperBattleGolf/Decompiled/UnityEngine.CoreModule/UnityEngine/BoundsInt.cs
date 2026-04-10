using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct BoundsInt : IEquatable<BoundsInt>, IFormattable
{
	public struct PositionEnumerator : IEnumerator<Vector3Int>, IEnumerator, IDisposable
	{
		private readonly Vector3Int _min;

		private readonly Vector3Int _max;

		private Vector3Int _current;

		public readonly Vector3Int Current
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
		public PositionEnumerator(in Vector3Int min, in Vector3Int max)
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
			if (_current.z >= _max.z || _current.y >= _max.y)
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
					_current.y = _min.y;
					_current.z++;
					if (_current.z >= _max.z)
					{
						return false;
					}
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

	private Vector3Int m_Position;

	private Vector3Int m_Size;

	public int x
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Position.x;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Position.x = value;
		}
	}

	public int y
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Position.y;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Position.y = value;
		}
	}

	public int z
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Position.z;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Position.z = value;
		}
	}

	public readonly Vector3 center
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Vector3
			{
				x = (float)m_Position.x + (float)m_Size.x * 0.5f,
				y = (float)m_Position.y + (float)m_Size.y * 0.5f,
				z = (float)m_Position.z + (float)m_Size.z * 0.5f
			};
		}
	}

	public Vector3Int min
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector3Int(xMin, yMin, zMin);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			xMin = value.x;
			yMin = value.y;
			zMin = value.z;
		}
	}

	public Vector3Int max
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector3Int(xMax, yMax, zMax);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			xMax = value.x;
			yMax = value.y;
			zMax = value.z;
		}
	}

	public int xMin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Min(m_Position.x, m_Position.x + m_Size.x);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = xMax;
			m_Position.x = value;
			m_Size.x = num - m_Position.x;
		}
	}

	public int yMin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Min(m_Position.y, m_Position.y + m_Size.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = yMax;
			m_Position.y = value;
			m_Size.y = num - m_Position.y;
		}
	}

	public int zMin
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Min(m_Position.z, m_Position.z + m_Size.z);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			int num = zMax;
			m_Position.z = value;
			m_Size.z = num - m_Position.z;
		}
	}

	public int xMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Max(m_Position.x, m_Position.x + m_Size.x);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Size.x = value - m_Position.x;
		}
	}

	public int yMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Max(m_Position.y, m_Position.y + m_Size.y);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Size.y = value - m_Position.y;
		}
	}

	public int zMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return Math.Max(m_Position.z, m_Position.z + m_Size.z);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Size.z = value - m_Position.z;
		}
	}

	public Vector3Int position
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Position;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Position = value;
		}
	}

	public Vector3Int size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Size;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Size = value;
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
	public BoundsInt(int xMin, int yMin, int zMin, int sizeX, int sizeY, int sizeZ)
	{
		m_Position = new Vector3Int(xMin, yMin, zMin);
		m_Size = new Vector3Int(sizeX, sizeY, sizeZ);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BoundsInt(Vector3Int position, Vector3Int size)
	{
		m_Position = position;
		m_Size = size;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BoundsInt(in Vector3Int position, in Vector3Int size)
	{
		m_Position = position;
		m_Size = size;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(Vector3Int minPosition, Vector3Int maxPosition)
	{
		xMin = minPosition.x;
		yMin = minPosition.y;
		zMin = minPosition.z;
		xMax = maxPosition.x;
		yMax = maxPosition.y;
		zMax = maxPosition.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(in Vector3Int minPosition, in Vector3Int maxPosition)
	{
		xMin = minPosition.x;
		yMin = minPosition.y;
		zMin = minPosition.z;
		xMax = maxPosition.x;
		yMax = maxPosition.y;
		zMax = maxPosition.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClampToBounds(BoundsInt bounds)
	{
		m_Position.x = Math.Max(Math.Min(bounds.xMax, m_Position.x), bounds.xMin);
		m_Position.y = Math.Max(Math.Min(bounds.yMax, m_Position.y), bounds.yMin);
		m_Position.z = Math.Max(Math.Min(bounds.zMax, m_Position.z), bounds.zMin);
		m_Size.x = Math.Min(bounds.xMax - m_Position.x, m_Size.x);
		m_Size.y = Math.Min(bounds.yMax - m_Position.y, m_Size.y);
		m_Size.z = Math.Min(bounds.zMax - m_Position.z, m_Size.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClampToBounds(in BoundsInt bounds)
	{
		m_Position.x = Math.Max(Math.Min(bounds.xMax, m_Position.x), bounds.xMin);
		m_Position.y = Math.Max(Math.Min(bounds.yMax, m_Position.y), bounds.yMin);
		m_Position.z = Math.Max(Math.Min(bounds.zMax, m_Position.z), bounds.zMin);
		m_Size.x = Math.Min(bounds.xMax - m_Position.x, m_Size.x);
		m_Size.y = Math.Min(bounds.yMax - m_Position.y, m_Size.y);
		m_Size.z = Math.Min(bounds.zMax - m_Position.z, m_Size.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(Vector3Int position)
	{
		return position.x >= xMin && position.y >= yMin && position.z >= zMin && position.x < xMax && position.y < yMax && position.z < zMax;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(in Vector3Int position)
	{
		return position.x >= xMin && position.y >= yMin && position.z >= zMin && position.x < xMax && position.y < yMax && position.z < zMax;
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
		return $"Position: {m_Position.ToString(format, formatProvider)}, Size: {m_Size.ToString(format, formatProvider)}";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(BoundsInt lhs, BoundsInt rhs)
	{
		return lhs.m_Position == rhs.m_Position && lhs.m_Size == rhs.m_Size;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(BoundsInt lhs, BoundsInt rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is BoundsInt other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(BoundsInt other)
	{
		return m_Position.Equals(in other.m_Position) && m_Size.Equals(in other.m_Size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in BoundsInt other)
	{
		return m_Position.Equals(in other.m_Position) && m_Size.Equals(in other.m_Size);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return m_Position.GetHashCode() ^ (m_Size.GetHashCode() << 2);
	}
}
