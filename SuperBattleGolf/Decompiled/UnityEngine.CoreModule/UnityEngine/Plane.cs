using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
public struct Plane : IEquatable<Plane>, IFormattable
{
	internal const int size = 16;

	private Vector3 m_Normal;

	private float m_Distance;

	public Vector3 normal
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Normal;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Normal = value;
		}
	}

	public float distance
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		readonly get
		{
			return m_Distance;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			m_Distance = value;
		}
	}

	public readonly Plane flipped
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Plane(-m_Normal, 0f - m_Distance);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(Vector3 inNormal, Vector3 inPoint)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in inPoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(in Vector3 inNormal, in Vector3 inPoint)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in inPoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(Vector3 inNormal, float d)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = d;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(in Vector3 inNormal, float d)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = d;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 lhs = default(Vector3);
		lhs.x = b.x - a.x;
		lhs.y = b.y - a.y;
		lhs.z = b.z - a.z;
		Vector3 rhs = default(Vector3);
		rhs.x = c.x - a.x;
		rhs.y = c.y - a.y;
		rhs.z = c.z - a.z;
		Vector3 vector = Vector3.Cross(in lhs, in rhs);
		m_Normal.x = vector.x;
		m_Normal.y = vector.y;
		m_Normal.z = vector.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Plane(in Vector3 a, in Vector3 b, in Vector3 c)
	{
		Vector3 lhs = default(Vector3);
		lhs.x = b.x - a.x;
		lhs.y = b.y - a.y;
		lhs.z = b.z - a.z;
		Vector3 rhs = default(Vector3);
		rhs.x = c.x - a.x;
		rhs.y = c.y - a.y;
		rhs.z = c.z - a.z;
		Vector3 vector = Vector3.Cross(in lhs, in rhs);
		m_Normal.x = vector.x;
		m_Normal.y = vector.y;
		m_Normal.z = vector.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in inPoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetNormalAndPosition(in Vector3 inNormal, in Vector3 inPoint)
	{
		m_Normal.x = inNormal.x;
		m_Normal.y = inNormal.y;
		m_Normal.z = inNormal.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in inPoint);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 lhs = default(Vector3);
		lhs.x = b.x - a.x;
		lhs.y = b.y - a.y;
		lhs.z = b.z - a.z;
		Vector3 rhs = default(Vector3);
		rhs.x = c.x - a.x;
		rhs.y = c.y - a.y;
		rhs.z = c.z - a.z;
		Vector3 vector = Vector3.Cross(in lhs, in rhs);
		m_Normal.x = vector.x;
		m_Normal.y = vector.y;
		m_Normal.z = vector.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set3Points(in Vector3 a, in Vector3 b, in Vector3 c)
	{
		Vector3 lhs = default(Vector3);
		lhs.x = b.x - a.x;
		lhs.y = b.y - a.y;
		lhs.z = b.z - a.z;
		Vector3 rhs = default(Vector3);
		rhs.x = c.x - a.x;
		rhs.y = c.y - a.y;
		rhs.z = c.z - a.z;
		Vector3 vector = Vector3.Cross(in lhs, in rhs);
		m_Normal.x = vector.x;
		m_Normal.y = vector.y;
		m_Normal.z = vector.z;
		m_Normal.Normalize();
		m_Distance = 0f - Vector3.Dot(in m_Normal, in a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Flip()
	{
		m_Normal.x = 0f - m_Normal.x;
		m_Normal.y = 0f - m_Normal.y;
		m_Normal.z = 0f - m_Normal.z;
		m_Distance = 0f - m_Distance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Translate(Vector3 translation)
	{
		m_Distance += Vector3.Dot(in m_Normal, in translation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Translate(in Vector3 translation)
	{
		m_Distance += Vector3.Dot(in m_Normal, in translation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane Translate(Plane plane, Vector3 translation)
	{
		return new Plane(in plane.m_Normal, plane.m_Distance + Vector3.Dot(in plane.m_Normal, in translation));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Plane Translate(in Plane plane, in Vector3 translation)
	{
		return new Plane(in plane.m_Normal, plane.m_Distance + Vector3.Dot(in plane.m_Normal, in translation));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 ClosestPointOnPlane(Vector3 point)
	{
		float num = Vector3.Dot(in m_Normal, in point) + m_Distance;
		Vector3 result = default(Vector3);
		result.x = point.x - m_Normal.x * num;
		result.y = point.y - m_Normal.y * num;
		result.z = point.z - m_Normal.z * num;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly Vector3 ClosestPointOnPlane(in Vector3 point)
	{
		float num = Vector3.Dot(in m_Normal, in point) + m_Distance;
		Vector3 result = default(Vector3);
		result.x = point.x - m_Normal.x * num;
		result.y = point.y - m_Normal.y * num;
		result.z = point.z - m_Normal.z * num;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float GetDistanceToPoint(Vector3 point)
	{
		return Vector3.Dot(in m_Normal, in point) + m_Distance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly float GetDistanceToPoint(in Vector3 point)
	{
		return Vector3.Dot(in m_Normal, in point) + m_Distance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetSide(Vector3 point)
	{
		return Vector3.Dot(in m_Normal, in point) + m_Distance > 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool GetSide(in Vector3 point)
	{
		return Vector3.Dot(in m_Normal, in point) + m_Distance > 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool SameSide(Vector3 inPt0, Vector3 inPt1)
	{
		float distanceToPoint = GetDistanceToPoint(in inPt0);
		float distanceToPoint2 = GetDistanceToPoint(in inPt1);
		return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool SameSide(in Vector3 inPt0, in Vector3 inPt1)
	{
		float distanceToPoint = GetDistanceToPoint(in inPt0);
		float distanceToPoint2 = GetDistanceToPoint(in inPt1);
		return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Raycast(Ray ray, out float enter)
	{
		float num = Vector3.Dot(ray.direction, in m_Normal);
		float num2 = 0f - Vector3.Dot(ray.origin, in m_Normal) - m_Distance;
		if (Mathf.Approximately(num, 0f))
		{
			enter = 0f;
			return false;
		}
		enter = num2 / num;
		return enter > 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Raycast(in Ray ray, out float enter)
	{
		float num = Vector3.Dot(ray.direction, in m_Normal);
		float num2 = 0f - Vector3.Dot(ray.origin, in m_Normal) - m_Distance;
		if (Mathf.Approximately(num, 0f))
		{
			enter = 0f;
			return false;
		}
		enter = num2 / num;
		return enter > 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Plane lhs, Plane rhs)
	{
		return lhs.m_Normal == rhs.m_Normal && lhs.m_Distance == rhs.m_Distance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Plane lhs, Plane rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly bool Equals(object other)
	{
		if (other is Plane other2)
		{
			return Equals(in other2);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(Plane other)
	{
		return this == other;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(in Plane other)
	{
		return this == other;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode()
	{
		return m_Distance.GetHashCode() ^ (m_Normal.GetHashCode() << 2);
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
		return $"(normal:{m_Normal.ToString(format, formatProvider)}, distance:{m_Distance.ToString(format, formatProvider)})";
	}
}
