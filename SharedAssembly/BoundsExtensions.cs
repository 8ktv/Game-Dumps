using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtensions
{
	public static Vector3[] GetWorldCorners(this Bounds bounds)
	{
		return new Vector3[8]
		{
			bounds.center + bounds.extents,
			bounds.center + new Vector3(bounds.extents.x, 0f - bounds.extents.y, 0f - bounds.extents.z),
			bounds.center + new Vector3(bounds.extents.x, 0f - bounds.extents.y, bounds.extents.z),
			bounds.center + new Vector3(0f - bounds.extents.x, 0f - bounds.extents.y, bounds.extents.z),
			bounds.center - bounds.extents,
			bounds.center - new Vector3(bounds.extents.x, 0f - bounds.extents.y, 0f - bounds.extents.z),
			bounds.center - new Vector3(bounds.extents.x, 0f - bounds.extents.y, bounds.extents.z),
			bounds.center - new Vector3(0f - bounds.extents.x, 0f - bounds.extents.y, bounds.extents.y)
		};
	}

	public static IEnumerable<Vector3> GetLocalCorners(this Bounds bounds)
	{
		yield return bounds.min;
		yield return bounds.max;
		yield return bounds.min + bounds.size.x * Vector3.right;
		yield return bounds.max + bounds.size.x * Vector3.left;
		yield return bounds.max + bounds.size.y * Vector3.down;
		yield return bounds.min + bounds.size.y * Vector3.up;
		yield return bounds.min + bounds.size.z * Vector3.forward;
		yield return bounds.max + bounds.size.z * Vector3.back;
	}

	public static Rect GetScreenRect(this Bounds bounds, Camera camera)
	{
		Rect result = default(Rect);
		bool flag = true;
		Vector3[] worldCorners = bounds.GetWorldCorners();
		foreach (Vector3 position in worldCorners)
		{
			Vector3 vector = camera.WorldToScreenPoint(position);
			if (flag)
			{
				result = new Rect(vector, Vector2.zero);
				flag = false;
			}
			else
			{
				result.min = Vector3.Min(result.min, vector);
				result.max = Vector3.Max(result.max, vector);
			}
		}
		return result;
	}

	public static Bounds MultiplyBounds(this Matrix4x4 matrix, Bounds localBounds)
	{
		Vector3 center = matrix.MultiplyPoint(localBounds.center);
		Vector3 extents = localBounds.extents;
		Vector3 vector = matrix.MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 vector2 = matrix.MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 vector3 = matrix.MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = BMath.Abs(vector.x) + BMath.Abs(vector2.x) + BMath.Abs(vector3.x);
		extents.y = BMath.Abs(vector.y) + BMath.Abs(vector2.y) + BMath.Abs(vector3.y);
		extents.z = BMath.Abs(vector.z) + BMath.Abs(vector2.z) + BMath.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}

	public static bool Overlaps(this Bounds bounds, Bounds otherBounds)
	{
		if (bounds.max.x >= otherBounds.min.x && bounds.min.x <= otherBounds.max.x && bounds.max.y >= otherBounds.min.y && bounds.min.y <= otherBounds.max.y && bounds.max.z >= otherBounds.min.z)
		{
			return bounds.min.z <= otherBounds.max.z;
		}
		return false;
	}

	public static bool OverlapsHorizontally(this Bounds bounds, Bounds otherBounds)
	{
		if (bounds.max.x >= otherBounds.min.x && bounds.min.x <= otherBounds.max.x && bounds.max.z >= otherBounds.min.z)
		{
			return bounds.min.z <= otherBounds.max.z;
		}
		return false;
	}

	public static bool Contains(this Bounds bounds, Bounds otherBounds)
	{
		if (bounds.min.x <= otherBounds.min.x && bounds.max.x >= otherBounds.max.x && bounds.min.y <= otherBounds.min.y && bounds.max.y >= otherBounds.max.y && bounds.min.z <= otherBounds.min.z)
		{
			return bounds.max.z >= otherBounds.max.z;
		}
		return false;
	}

	public static bool ContainsHorizontally(this Bounds bounds, Bounds otherBounds)
	{
		if (bounds.min.x <= otherBounds.min.x && bounds.max.x >= otherBounds.max.x && bounds.min.z <= otherBounds.min.z)
		{
			return bounds.max.z >= otherBounds.max.z;
		}
		return false;
	}

	public static bool TryGetOverlap(this Bounds bounds, Bounds otherBounds, out Bounds overlap)
	{
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		if (bounds.min.x > otherBounds.max.x || bounds.max.x < otherBounds.min.x)
		{
			overlap = default(Bounds);
			return false;
		}
		vector.x = BMath.Max(bounds.min.x, otherBounds.min.x);
		vector2.x = BMath.Min(bounds.max.x, otherBounds.max.x);
		if (bounds.min.y > otherBounds.max.y || bounds.max.y < otherBounds.min.y)
		{
			overlap = default(Bounds);
			return false;
		}
		vector.y = BMath.Max(bounds.min.y, otherBounds.min.y);
		vector2.y = BMath.Min(bounds.max.y, otherBounds.max.y);
		if (bounds.min.z > otherBounds.max.z || bounds.max.z < otherBounds.min.z)
		{
			overlap = default(Bounds);
			return false;
		}
		vector.z = BMath.Max(bounds.min.z, otherBounds.min.z);
		vector2.z = BMath.Min(bounds.max.z, otherBounds.max.z);
		overlap = new Bounds((vector + vector2) / 2f, vector2 - vector);
		return true;
	}

	public static float GetHorizontalDistanceSquared(this Bounds bounds, Vector3 point)
	{
		Vector3 vector = BMath.Abs(point - bounds.center);
		float num = BMath.Max(0f, vector.x - bounds.extents.x);
		float num2 = BMath.Max(0f, vector.z - bounds.extents.z);
		return num * num + num2 * num2;
	}

	public static Vector3 ClosestPointOnSurface(this Bounds bounds, Vector3 point, bool ignoreBottom = false)
	{
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector3 zero = Vector3.zero;
		Vector3Int zero2 = Vector3Int.zero;
		ref float x = ref zero.x;
		ref Vector3Int reference = ref zero2;
		(float, int) tuple = SignedDistance(point.x, min.x, max.x);
		x = tuple.Item1;
		int num = (reference.x = tuple.Item2);
		ref float y = ref zero.y;
		reference = ref zero2;
		tuple = SignedDistance(point.y, min.y, max.y);
		y = tuple.Item1;
		num = (reference.y = tuple.Item2);
		ref float z = ref zero.z;
		reference = ref zero2;
		tuple = SignedDistance(point.z, min.z, max.z);
		z = tuple.Item1;
		num = (reference.z = tuple.Item2);
		Vector3 result = point;
		bool flag = false;
		if (zero.x > 0f)
		{
			flag = true;
			Vector3 vector = new Vector3(zero2.x, 0f, 0f);
			result -= zero.x * vector;
		}
		if (zero.y > 0f && (!ignoreBottom || zero2.y > 0))
		{
			flag = true;
			Vector3 vector2 = new Vector3(0f, zero2.y, 0f);
			result -= zero.y * vector2;
		}
		if (zero.z > 0f)
		{
			flag = true;
			Vector3 vector3 = new Vector3(0f, 0f, zero2.z);
			result -= zero.z * vector3;
		}
		if (!flag)
		{
			BMath.Min(0f - zero.x, (ignoreBottom && zero2.y < 0) ? float.MaxValue : (0f - zero.y), 0f - zero.z, out var index);
			Vector3 zero3 = Vector3.zero;
			zero3[index] = (0f - zero[index]) * (float)zero2[index];
			result += zero3;
		}
		return result;
		static (float signedDistance, int side) SignedDistance(float value, float num2, float num3)
		{
			if (value < num2)
			{
				return (signedDistance: num2 - value, side: -1);
			}
			if (value > num3)
			{
				return (signedDistance: value - num3, side: 1);
			}
			float num4 = num2 - value;
			float num5 = value - num3;
			if (num4 > num5)
			{
				return (signedDistance: num4, side: -1);
			}
			return (signedDistance: num5, side: 1);
		}
	}
}
