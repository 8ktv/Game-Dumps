using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
	public static bool TryFind(this Transform transform, string childName, out Transform child)
	{
		child = transform.Find(childName);
		return child != null;
	}

	public static bool TryFindRecursive(this Transform transform, string relativePath, out Transform child)
	{
		child = transform.FindRecursive(relativePath);
		return child != null;
	}

	public static Transform FindRecursive(this Transform transform, string relativePath)
	{
		if (transform == null)
		{
			return null;
		}
		Transform transform2 = transform.Find(relativePath);
		if (transform2 != null)
		{
			return transform2;
		}
		Transform transform3 = transform;
		string[] array = relativePath.Split('/');
		foreach (string n in array)
		{
			transform3 = transform3.Find(n);
			if (transform3 == null)
			{
				return null;
			}
		}
		return transform3;
	}

	public static Transform FindOrCreateChild(this Transform transform, string relativePath)
	{
		Transform result = transform.Find(relativePath);
		if (transform.Find(relativePath) != null)
		{
			return result;
		}
		Transform transform2 = transform;
		string[] array = relativePath.Split('/');
		foreach (string childName in array)
		{
			transform2 = transform2.FindOrCreateDirectChild(childName);
		}
		return transform2;
	}

	private static Transform FindOrCreateDirectChild(this Transform transform, string childName)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == childName)
			{
				return child;
			}
		}
		Transform transform2 = new GameObject(childName).transform;
		transform2.SetParent(transform);
		transform2.localPosition = Vector3.zero;
		transform2.localRotation = Quaternion.identity;
		return transform2;
	}

	public static Transform[] GetOrderedTransformsInChildrenContaining<T>(this Transform transform)
	{
		List<Transform> list = new List<Transform>();
		if (transform.TryGetComponent<T>(out var _))
		{
			list.Add(transform);
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			list.AddRange(transform.GetChild(i).GetOrderedTransformsInChildrenContaining<T>());
		}
		return list.ToArray();
	}

	public static string GetFullPath(this Transform transform)
	{
		string text = transform.name;
		Transform parent = transform.parent;
		while (parent != null)
		{
			text = parent.name + "/" + text;
			parent = parent.parent;
		}
		return text;
	}

	public static Bounds TransformBounds(this Transform transform, Bounds localBounds)
	{
		Vector3 center = transform.TransformPoint(localBounds.center);
		Vector3 extents = localBounds.extents;
		Vector3 vector = transform.TransformVector(extents.x, 0f, 0f);
		Vector3 vector2 = transform.TransformVector(0f, extents.y, 0f);
		Vector3 vector3 = transform.TransformVector(0f, 0f, extents.z);
		extents.x = BMath.Abs(vector.x) + BMath.Abs(vector2.x) + BMath.Abs(vector3.x);
		extents.y = BMath.Abs(vector.y) + BMath.Abs(vector2.y) + BMath.Abs(vector3.y);
		extents.z = BMath.Abs(vector.z) + BMath.Abs(vector2.z) + BMath.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}

	public static Bounds InverseTransformBounds(this Transform transform, Bounds worldBounds)
	{
		Vector3 center = transform.InverseTransformPoint(worldBounds.center);
		Vector3 extents = worldBounds.extents;
		Vector3 vector = transform.InverseTransformVector(extents.x, 0f, 0f);
		Vector3 vector2 = transform.InverseTransformVector(0f, extents.y, 0f);
		Vector3 vector3 = transform.InverseTransformVector(0f, 0f, extents.z);
		extents.x = BMath.Abs(vector.x) + BMath.Abs(vector2.x) + BMath.Abs(vector3.x);
		extents.y = BMath.Abs(vector.y) + BMath.Abs(vector2.y) + BMath.Abs(vector3.y);
		extents.z = BMath.Abs(vector.z) + BMath.Abs(vector2.z) + BMath.Abs(vector3.z);
		return new Bounds
		{
			center = center,
			extents = extents
		};
	}

	public static Bounds GetLocalCompoundBounds(this Transform transform, bool includeTriggers = false, int layerMask = -1)
	{
		Bounds result = default(Bounds);
		bool flag = true;
		Collider[] componentsInChildren = transform.GetComponentsInChildren<Collider>(includeInactive: true);
		foreach (Collider collider in componentsInChildren)
		{
			if ((includeTriggers || !collider.isTrigger) && (layerMask == -1 || (layerMask & (1 << collider.gameObject.layer)) != 0))
			{
				Bounds localBounds = collider.GetLocalBounds();
				localBounds.size = transform.InverseTransformDirection(collider.transform.TransformDirection(localBounds.size));
				localBounds.center = transform.InverseTransformPoint(collider.transform.TransformPoint(localBounds.center));
				if (flag)
				{
					result = localBounds;
					flag = false;
				}
				else
				{
					result.Encapsulate(localBounds);
				}
			}
		}
		return result;
	}

	public static bool IsRotated90DegreesYaw(this Transform transform)
	{
		return (transform.rotation.eulerAngles.y % 180f).Approximately(90f, 1f);
	}

	public static T GetClosestInstance<T>(this Transform transform, HashSet<T> instances) where T : MonoBehaviour
	{
		if (instances == null)
		{
			return null;
		}
		T result = null;
		float num = float.PositiveInfinity;
		foreach (T instance in instances)
		{
			if (!(transform == instance.transform))
			{
				float sqrMagnitude = (transform.position - instance.transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = instance;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	public static void SetPositionX(this Transform transform, float value)
	{
		Vector3 position = transform.position;
		position.x = value;
		transform.position = position;
	}

	public static void SetPositionY(this Transform transform, float value)
	{
		Vector3 position = transform.position;
		position.y = value;
		transform.position = position;
	}

	public static void SetPositionZ(this Transform transform, float value)
	{
		Vector3 position = transform.position;
		position.z = value;
		transform.position = position;
	}

	public static void SetLocalPositionX(this Transform transform, float value)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.x = value;
		transform.localPosition = localPosition;
	}

	public static void SetLocalPositionY(this Transform transform, float value)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.y = value;
		transform.localPosition = localPosition;
	}

	public static void SetLocalPositionZ(this Transform transform, float value)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.z = value;
		transform.localPosition = localPosition;
	}

	public static void SetEulerAnglesX(this Transform transform, float value)
	{
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.x = value;
		transform.eulerAngles = eulerAngles;
	}

	public static void SetEulerAnglesY(this Transform transform, float value)
	{
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.y = value;
		transform.eulerAngles = eulerAngles;
	}

	public static void SetEulerAnglesZ(this Transform transform, float value)
	{
		Vector3 eulerAngles = transform.eulerAngles;
		eulerAngles.z = value;
		transform.eulerAngles = eulerAngles;
	}

	public static void SetLocalScale(this Transform transform, float value)
	{
		transform.localScale = Vector3.one * value;
	}

	public static void SetLocalScaleX(this Transform transform, float value)
	{
		transform.localScale = new Vector3(value, 1f, 1f);
	}

	public static void SetLocalScaleY(this Transform transform, float value)
	{
		transform.localScale = new Vector3(1f, value, 1f);
	}

	public static void SetLocalScaleZ(this Transform transform, float value)
	{
		transform.localScale = new Vector3(1f, 1f, value);
	}
}
