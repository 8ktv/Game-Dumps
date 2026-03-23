using UnityEngine;

public static class ComponentExtensions
{
	public static bool TryGetComponentInChildren<T>(this Component component, out T foundComponent, bool includeInactive)
	{
		foundComponent = component.GetComponentInChildren<T>(includeInactive);
		return foundComponent != null;
	}

	public static bool TryGetComponentsInChildren<T>(this Component component, out T[] foundComponents, bool includeInactive)
	{
		foundComponents = component.GetComponentsInChildren<T>(includeInactive);
		if (foundComponents != null)
		{
			return foundComponents.Length != 0;
		}
		return false;
	}

	public static bool TryGetComponentInParent<T>(this Component component, out T foundComponent, bool includeInactive)
	{
		foundComponent = component.GetComponentInParent<T>(includeInactive);
		return foundComponent != null;
	}

	public static bool TryGetComponentsInParent<T>(this Component component, out T[] foundComponents, bool includeInactive)
	{
		foundComponents = component.GetComponentsInParent<T>(includeInactive);
		if (foundComponents != null)
		{
			return foundComponents.Length != 0;
		}
		return false;
	}

	public static bool TryGetClosestPointOnAllActiveColliders(this Component component, Vector3 worldPosition, out Vector3 closestPoint, out float distanceSquared, int layerMask = -1)
	{
		closestPoint = default(Vector3);
		distanceSquared = float.MaxValue;
		Collider[] componentsInChildren = component.GetComponentsInChildren<Collider>(includeInactive: false);
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.enabled && ((1 << collider.gameObject.layer) & layerMask) != 0 && !(collider is WheelCollider))
			{
				Vector3 vector = collider.ClosestPoint(worldPosition);
				float sqrMagnitude = (vector - worldPosition).sqrMagnitude;
				if (sqrMagnitude < distanceSquared)
				{
					closestPoint = vector;
					distanceSquared = sqrMagnitude;
				}
			}
		}
		return distanceSquared < float.MaxValue;
	}
}
