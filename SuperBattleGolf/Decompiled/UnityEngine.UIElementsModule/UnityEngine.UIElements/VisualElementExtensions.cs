using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnityEngine.UIElements;

public static class VisualElementExtensions
{
	public static void StretchToParentSize(this VisualElement elem)
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		IStyle style = elem.style;
		style.position = Position.Absolute;
		style.left = 0f;
		style.top = 0f;
		style.right = 0f;
		style.bottom = 0f;
	}

	public static void StretchToParentWidth(this VisualElement elem)
	{
		if (elem == null)
		{
			throw new ArgumentNullException("elem");
		}
		IStyle style = elem.style;
		style.position = Position.Absolute;
		style.left = 0f;
		style.right = 0f;
	}

	public static void AddManipulator(this VisualElement ele, IManipulator manipulator)
	{
		if (manipulator != null)
		{
			manipulator.target = ele;
		}
	}

	public static void RemoveManipulator(this VisualElement ele, IManipulator manipulator)
	{
		if (manipulator != null)
		{
			manipulator.target = null;
		}
	}

	public static Vector2 WorldToLocal(this VisualElement ele, Vector2 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.MultiplyMatrix44Point2(ref ele.worldTransformInverse, p);
	}

	internal static Vector3 WorldToLocal3D(this VisualElement ele, Vector3 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return ele.worldTransformInverse.MultiplyPoint3x4(p);
	}

	public static Vector2 LocalToWorld(this VisualElement ele, Vector2 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.MultiplyMatrix44Point2(ref ele.worldTransformRef, p);
	}

	internal static Vector3 LocalToWorld3D(this VisualElement ele, Vector3 p)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return ele.worldTransformRef.MultiplyPoint3x4(p);
	}

	public static Rect WorldToLocal(this VisualElement ele, Rect r)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.CalculateConservativeRect(ref ele.worldTransformInverse, r);
	}

	public static Rect LocalToWorld(this VisualElement ele, Rect r)
	{
		if (ele == null)
		{
			throw new ArgumentNullException("ele");
		}
		return VisualElement.CalculateConservativeRect(ref ele.worldTransformRef, r);
	}

	internal static Ray LocalToWorld([NotNull] this VisualElement ele, Ray r)
	{
		ref Matrix4x4 worldTransformRef = ref ele.worldTransformRef;
		return new Ray(worldTransformRef.MultiplyPoint3x4(r.origin), worldTransformRef.MultiplyVector(r.direction));
	}

	internal static Ray WorldToLocal([NotNull] this VisualElement ele, Ray r)
	{
		ref Matrix4x4 worldTransformInverse = ref ele.worldTransformInverse;
		return new Ray(worldTransformInverse.MultiplyPoint3x4(r.origin), worldTransformInverse.MultiplyVector(r.direction));
	}

	public static Vector2 ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Vector2 point)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		BaseVisualElementPanel elementPanel = src.elementPanel;
		return (elementPanel != null && !elementPanel.isFlat) ? src.ChangeCoordinatesTo_3D(dest, point) : src.ChangeCoordinatesTo_2D(dest, point);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector2 ChangeCoordinatesTo_2D([NotNull] this VisualElement src, [NotNull] VisualElement dest, Vector2 point)
	{
		Vector2 point2 = VisualElement.MultiplyMatrix44Point2(ref src.worldTransformRef, point);
		return VisualElement.MultiplyMatrix44Point2(ref dest.worldTransformInverse, point2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static Vector2 ChangeCoordinatesTo_3D([NotNull] this VisualElement src, [NotNull] VisualElement dest, Vector2 point)
	{
		Vector3 point2 = VisualElement.MultiplyMatrix44Point2ToPoint3(ref src.worldTransformRef, point);
		return VisualElement.MultiplyMatrix44Point3ToPoint2(ref dest.worldTransformInverse, point2);
	}

	public static Rect ChangeCoordinatesTo(this VisualElement src, VisualElement dest, Rect rect)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		if (float.IsNaN(rect.height) || float.IsNaN(rect.width) || float.IsNaN(rect.x) || float.IsNaN(rect.y))
		{
			return new Rect(float.NaN, float.NaN, float.NaN, float.NaN);
		}
		Vector2 point = new Vector2(rect.xMin, rect.yMin);
		Vector2 point2 = new Vector2(rect.xMax, rect.yMax);
		Vector2 point3 = new Vector2(rect.xMax, rect.yMin);
		Vector2 point4 = new Vector2(rect.xMin, rect.yMax);
		BaseVisualElementPanel elementPanel = src.elementPanel;
		Vector2 vector;
		Vector2 vector2;
		Vector2 vector3;
		Vector2 vector4;
		if (elementPanel != null && !elementPanel.isFlat)
		{
			vector = src.ChangeCoordinatesTo_3D(dest, point);
			vector2 = src.ChangeCoordinatesTo_3D(dest, point3);
			vector3 = src.ChangeCoordinatesTo_3D(dest, point4);
			vector4 = src.ChangeCoordinatesTo_3D(dest, point2);
		}
		else
		{
			vector = src.ChangeCoordinatesTo_2D(dest, point);
			vector2 = src.ChangeCoordinatesTo_2D(dest, point3);
			vector3 = src.ChangeCoordinatesTo_2D(dest, point4);
			vector4 = src.ChangeCoordinatesTo_2D(dest, point2);
		}
		Vector2 vector5 = new Vector2(VisualElement.Min(vector.x, vector2.x, vector3.x, vector4.x), VisualElement.Min(vector.y, vector2.y, vector3.y, vector4.y));
		Vector2 vector6 = new Vector2(VisualElement.Max(vector.x, vector2.x, vector3.x, vector4.x), VisualElement.Max(vector.y, vector2.y, vector3.y, vector4.y));
		return new Rect(vector5.x, vector5.y, vector6.x - vector5.x, vector6.y - vector5.y);
	}

	internal static Ray ChangeCoordinatesTo([NotNull] this VisualElement src, [NotNull] VisualElement dest, Ray ray)
	{
		return dest.WorldToLocal(src.LocalToWorld(ray));
	}

	internal static bool IntersectWorldRay([NotNull] this VisualElement ve, Ray worldRay, out float distance, out Vector3 localPoint)
	{
		ref Matrix4x4 worldTransformInverse = ref ve.worldTransformInverse;
		Ray localRay = new Ray(worldTransformInverse.MultiplyPoint3x4(worldRay.origin), worldTransformInverse.MultiplyVector(worldRay.direction));
		bool result = ve.IntersectLocalRay(localRay, out localPoint);
		Vector3 b = ve.worldTransformRef.MultiplyPoint3x4(localPoint);
		distance = Vector3.Distance(worldRay.origin, b);
		return result;
	}

	internal static bool IntersectLocalRay([NotNull] this VisualElement ve, Ray localRay, out Vector3 localPoint)
	{
		float num = (0f - localRay.origin.z) / localRay.direction.z;
		localPoint = localRay.origin + localRay.direction * num;
		if (!(num > 0f))
		{
			return false;
		}
		return ve.rect.Contains(localPoint);
	}

	internal static Ray TransformRay(this Matrix4x4 m, Ray ray)
	{
		return new Ray(m.MultiplyPoint3x4(ray.origin), m.MultiplyVector(ray.direction));
	}
}
