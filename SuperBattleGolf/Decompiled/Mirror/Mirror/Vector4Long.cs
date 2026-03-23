using System;
using System.Runtime.CompilerServices;

namespace Mirror;

public struct Vector4Long
{
	public long x;

	public long y;

	public long z;

	public long w;

	public static readonly Vector4Long zero = new Vector4Long(0L, 0L, 0L, 0L);

	public static readonly Vector4Long one = new Vector4Long(1L, 1L, 1L, 1L);

	public long this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return index switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				3 => w, 
				_ => throw new IndexOutOfRangeException($"Vector4Long[{index}] out of range."), 
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (index)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			case 3:
				w = value;
				break;
			default:
				throw new IndexOutOfRangeException($"Vector4Long[{index}] out of range.");
			}
		}
	}

	public Vector4Long(long x, long y, long z, long w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4Long operator +(Vector4Long a, Vector4Long b)
	{
		return new Vector4Long(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4Long operator -(Vector4Long a, Vector4Long b)
	{
		return new Vector4Long(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4Long operator -(Vector4Long v)
	{
		return new Vector4Long(-v.x, -v.y, -v.z, -v.w);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4Long operator *(Vector4Long a, long n)
	{
		return new Vector4Long(a.x * n, a.y * n, a.z * n, a.w * n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4Long operator *(long n, Vector4Long a)
	{
		return new Vector4Long(a.x * n, a.y * n, a.z * n, a.w * n);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Vector4Long a, Vector4Long b)
	{
		if (a.x == b.x && a.y == b.y && a.z == b.z)
		{
			return a.w == b.w;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Vector4Long a, Vector4Long b)
	{
		return !(a == b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		return $"({x} {y} {z} {w})";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Vector4Long other)
	{
		if (x == other.x && y == other.y && z == other.z)
		{
			return w == other.w;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object other)
	{
		if (other is Vector4Long other2)
		{
			return Equals(other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return HashCode.Combine(x, y, z, w);
	}
}
