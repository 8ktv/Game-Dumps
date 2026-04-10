using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Geometry/Intersection.h")]
[NativeHeader("Runtime/Geometry/Ray.h")]
[NativeHeader("Runtime/Math/MathScripting.h")]
[NativeType(Header = "Runtime/Geometry/AABB.h")]
[NativeHeader("Runtime/Geometry/AABB.h")]
[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
[NativeClass("AABB")]
public struct Bounds : IEquatable<Bounds>, IFormattable
{
	private Vector3 m_Center;

	[NativeName("m_Extent")]
	private Vector3 m_Extents;

	public Vector3 center
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Center;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Center = value;
		}
	}

	public Vector3 size
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector3
			{
				x = m_Extents.x * 2f,
				y = m_Extents.y * 2f,
				z = m_Extents.z * 2f
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Extents.x = value.x * 0.5f;
			m_Extents.y = value.y * 0.5f;
			m_Extents.z = value.z * 0.5f;
		}
	}

	public Vector3 extents
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Extents;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Extents = value;
		}
	}

	public Vector3 min
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector3
			{
				x = m_Center.x - m_Extents.x,
				y = m_Center.y - m_Extents.y,
				z = m_Center.z - m_Extents.z
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			SetMinMax(in value, max);
		}
	}

	public Vector3 max
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return new Vector3
			{
				x = m_Center.x + m_Extents.x,
				y = m_Center.y + m_Extents.y,
				z = m_Center.z + m_Extents.z
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			SetMinMax(min, in value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds(Vector3 center, Vector3 size)
	{
		m_Center.x = center.x;
		m_Center.y = center.y;
		m_Center.z = center.z;
		m_Extents.x = size.x * 0.5f;
		m_Extents.y = size.y * 0.5f;
		m_Extents.z = size.z * 0.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Bounds(in Vector3 center, in Vector3 size)
	{
		m_Center.x = center.x;
		m_Center.y = center.y;
		m_Center.z = center.z;
		m_Extents.x = size.x * 0.5f;
		m_Extents.y = size.y * 0.5f;
		m_Extents.z = size.z * 0.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return m_Center.GetHashCode() ^ (m_Extents.GetHashCode() << 2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is Bounds other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Bounds other)
	{
		return m_Center.Equals(in other.m_Center) && m_Extents.Equals(in other.m_Extents);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Bounds other)
	{
		return m_Center.Equals(in other.m_Center) && m_Extents.Equals(in other.m_Extents);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Bounds lhs, Bounds rhs)
	{
		return lhs.m_Center == rhs.m_Center && lhs.m_Extents == rhs.m_Extents;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Bounds lhs, Bounds rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(Vector3 min, Vector3 max)
	{
		m_Extents.x = (max.x - min.x) * 0.5f;
		m_Extents.y = (max.y - min.y) * 0.5f;
		m_Extents.z = (max.z - min.z) * 0.5f;
		m_Center.x = min.x + m_Extents.x;
		m_Center.y = min.y + m_Extents.y;
		m_Center.z = min.z + m_Extents.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMinMax(in Vector3 min, in Vector3 max)
	{
		m_Extents.x = (max.x - min.x) * 0.5f;
		m_Extents.y = (max.y - min.y) * 0.5f;
		m_Extents.z = (max.z - min.z) * 0.5f;
		m_Center.x = min.x + m_Extents.x;
		m_Center.y = min.y + m_Extents.y;
		m_Center.z = min.z + m_Extents.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Encapsulate(Vector3 point)
	{
		this.SetMinMax(in ILSpyHelper_AsRefReadOnly(Vector3.Min(min, in point)), in ILSpyHelper_AsRefReadOnly(Vector3.Max(max, in point)));
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Encapsulate(in Vector3 point)
	{
		this.SetMinMax(in ILSpyHelper_AsRefReadOnly(Vector3.Min(min, in point)), in ILSpyHelper_AsRefReadOnly(Vector3.Max(max, in point)));
		static ref readonly T ILSpyHelper_AsRefReadOnly<T>(in T temp)
		{
			//ILSpy generated this function to help ensure overload resolution can pick the overload using 'in'
			return ref temp;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Encapsulate(Bounds bounds)
	{
		Vector3 point = bounds.min;
		Vector3 point2 = bounds.max;
		Encapsulate(in point);
		Encapsulate(in point2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Encapsulate(in Bounds bounds)
	{
		Vector3 point = bounds.min;
		Vector3 point2 = bounds.max;
		Encapsulate(in point);
		Encapsulate(in point2);
	}

	public void Expand(float amount)
	{
		amount *= 0.5f;
		m_Extents.x += amount;
		m_Extents.y += amount;
		m_Extents.z += amount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Expand(Vector3 amount)
	{
		m_Extents.x += amount.x * 0.5f;
		m_Extents.y += amount.y * 0.5f;
		m_Extents.z += amount.z * 0.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Expand(in Vector3 amount)
	{
		m_Extents.x += amount.x * 0.5f;
		m_Extents.y += amount.y * 0.5f;
		m_Extents.z += amount.z * 0.5f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Intersects(Bounds bounds)
	{
		Vector3 vector = min;
		Vector3 vector2 = max;
		Vector3 vector3 = bounds.min;
		Vector3 vector4 = bounds.max;
		return vector.x <= vector4.x && vector2.x >= vector3.x && vector.y <= vector4.y && vector2.y >= vector3.y && vector.z <= vector4.z && vector2.z >= vector3.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Intersects(in Bounds bounds)
	{
		Vector3 vector = min;
		Vector3 vector2 = max;
		Vector3 vector3 = bounds.min;
		Vector3 vector4 = bounds.max;
		return vector.x <= vector4.x && vector2.x >= vector3.x && vector.y <= vector4.y && vector2.y >= vector3.y && vector.z <= vector4.z && vector2.z >= vector3.z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IntersectRay(Ray ray)
	{
		float dist;
		return IntersectRayAABB(in ray, in this, out dist);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IntersectRay(in Ray ray)
	{
		float dist;
		return IntersectRayAABB(in ray, in this, out dist);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IntersectRay(Ray ray, out float distance)
	{
		return IntersectRayAABB(in ray, in this, out distance);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool IntersectRay(in Ray ray, out float distance)
	{
		return IntersectRayAABB(in ray, in this, out distance);
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
		return $"Center: {m_Center.ToString(format, formatProvider)}, Extents: {m_Extents.ToString(format, formatProvider)}";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("IsInside", IsThreadSafe = true)]
	private readonly extern bool Internal_Contains(in Vector3 point);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(Vector3 point)
	{
		return Internal_Contains(in point);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Contains(in Vector3 point)
	{
		return Internal_Contains(in point);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("BoundsScripting::SqrDistance", HasExplicitThis = true, IsThreadSafe = true)]
	private readonly extern float Internal_SqrDistance(in Vector3 point);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float SqrDistance(Vector3 point)
	{
		return Internal_SqrDistance(in point);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float SqrDistance(in Vector3 point)
	{
		return Internal_SqrDistance(in point);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("IntersectRayAABB", IsThreadSafe = true)]
	private static extern bool IntersectRayAABB(in Ray ray, in Bounds bounds, out float dist);

	[FreeFunction("BoundsScripting::ClosestPoint", HasExplicitThis = true, IsThreadSafe = true)]
	private readonly Vector3 Internal_ClosestPoint(in Vector3 point)
	{
		Internal_ClosestPoint_Injected(ref this, in point, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 ClosestPoint(Vector3 point)
	{
		return Internal_ClosestPoint(in point);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 ClosestPoint(in Vector3 point)
	{
		return Internal_ClosestPoint(in point);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ClosestPoint_Injected(ref Bounds _unity_self, in Vector3 point, out Vector3 ret);
}
