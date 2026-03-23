using System;
using System.Reflection;
using UnityEngine;

public static class GameObjectExtensions
{
	public static Component AddOrGetComponent(this GameObject gameObject, Type type)
	{
		if (type.IsDefined(typeof(DisallowMultipleComponent)) && gameObject.TryGetComponent(type, out var component))
		{
			return component;
		}
		return gameObject.AddComponent(type);
	}

	public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
	{
		if (typeof(T).IsDefined(typeof(DisallowMultipleComponent)) && gameObject.TryGetComponent<T>(out var component))
		{
			return component;
		}
		return gameObject.AddComponent<T>();
	}

	public static void SetLayerRecursively(this GameObject gameObject, int layer)
	{
		gameObject.layer = layer;
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetLayerRecursively(layer);
		}
	}

	public static void SetLayerRecursively(this GameObject gameObject, int layer, int ignoreLayersMask)
	{
		if (((gameObject.layer != 0) ? ((1 << gameObject.layer) & ignoreLayersMask) : ((ignoreLayersMask == 0) ? 1 : 0)) == 0)
		{
			gameObject.layer = layer;
		}
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetLayerRecursively(layer, ignoreLayersMask);
		}
	}

	public static void SetHideFlagsRecursively(this GameObject gameObject, HideFlags hideFlags)
	{
		gameObject.hideFlags = hideFlags;
		foreach (Transform item in gameObject.transform)
		{
			item.gameObject.SetHideFlagsRecursively(hideFlags);
		}
	}

	public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T foundComponent, bool includeInactive)
	{
		foundComponent = gameObject.GetComponentInChildren<T>(includeInactive);
		return foundComponent != null;
	}

	public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] foundComponents, bool includeInactive)
	{
		foundComponents = gameObject.GetComponentsInChildren<T>(includeInactive);
		if (foundComponents != null)
		{
			return foundComponents.Length != 0;
		}
		return false;
	}

	public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T foundComponent, bool includeInactive)
	{
		foundComponent = gameObject.GetComponentInParent<T>(includeInactive);
		return foundComponent != null;
	}

	public static bool TryGetComponentsInParent<T>(this GameObject gameObject, out T[] foundComponents, bool includeInactive)
	{
		foundComponents = gameObject.GetComponentsInParent<T>(includeInactive);
		if (foundComponents != null)
		{
			return foundComponents.Length != 0;
		}
		return false;
	}
}
