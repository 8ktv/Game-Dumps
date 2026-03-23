using UnityEngine;

public static class InstantiationUtility
{
	public static GameObject InstantiateInactive(GameObject original, Transform parent = null)
	{
		if (!original.activeInHierarchy)
		{
			return Object.Instantiate(original);
		}
		GameObject gameObject = new GameObject();
		gameObject.SetActive(value: false);
		GameObject gameObject2 = Object.Instantiate(original, gameObject.transform);
		gameObject2.SetActive(value: false);
		gameObject2.transform.SetParent(parent);
		Object.Destroy(gameObject);
		return gameObject2;
	}

	public static GameObject InstantiateInactive(GameObject original, Vector3 position, Quaternion rotation, Transform parent = null)
	{
		if (!original.activeInHierarchy)
		{
			return Object.Instantiate(original);
		}
		GameObject gameObject = new GameObject();
		gameObject.SetActive(value: false);
		GameObject gameObject2 = Object.Instantiate(original, position, rotation, gameObject.transform);
		gameObject2.SetActive(value: false);
		gameObject2.transform.SetParent(parent);
		Object.Destroy(gameObject);
		return gameObject2;
	}
}
