using System.Collections.Generic;
using UnityEngine;

public class ThrownUsedItemManager : SingletonBehaviour<ThrownUsedItemManager>
{
	[SerializeField]
	private ThrownUsedItem[] prefabs;

	[SerializeField]
	private int maxIndividualPoolSize;

	private static Transform poolParent;

	private static readonly Dictionary<ThrownUsedItemType, ThrownUsedItem> prefabPerType = new Dictionary<ThrownUsedItemType, ThrownUsedItem>();

	private static readonly Dictionary<ThrownUsedItemType, Stack<ThrownUsedItem>> pools = new Dictionary<ThrownUsedItemType, Stack<ThrownUsedItem>>();

	protected override void Awake()
	{
		base.Awake();
		if (prefabPerType.Count <= 0)
		{
			ThrownUsedItem[] array = prefabs;
			foreach (ThrownUsedItem thrownUsedItem in array)
			{
				prefabPerType.Add(thrownUsedItem.Type, thrownUsedItem);
			}
		}
	}

	public static ThrownUsedItem GetUnusedThrownItem(ThrownUsedItemType type)
	{
		if (!SingletonBehaviour<ThrownUsedItemManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<ThrownUsedItemManager>.Instance.GetUnusedThrownItemInternal(type);
	}

	public static void ReturnThrownItem(ThrownUsedItem thrownItem)
	{
		if (SingletonBehaviour<ThrownUsedItemManager>.HasInstance)
		{
			SingletonBehaviour<ThrownUsedItemManager>.Instance.ReturnThrownItemInternal(thrownItem);
		}
	}

	private ThrownUsedItem GetUnusedThrownItemInternal(ThrownUsedItemType type)
	{
		EnsurePoolParentExists();
		if (!pools.TryGetValue(type, out var value))
		{
			value = new Stack<ThrownUsedItem>();
			pools.Add(type, value);
		}
		ThrownUsedItem result = null;
		while (result == null)
		{
			if (!value.TryPop(out result))
			{
				if (!prefabPerType.TryGetValue(type, out var value2))
				{
					Debug.LogError($"Could not find prefab for {type}");
					return null;
				}
				result = Object.Instantiate(value2);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(base.transform);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnThrownItemInternal(ThrownUsedItem thrownItem)
	{
		if (!pools.TryGetValue(thrownItem.Type, out var value))
		{
			value = new Stack<ThrownUsedItem>();
			pools.Add(thrownItem.Type, value);
		}
		if (value.Count >= maxIndividualPoolSize)
		{
			Object.Destroy(thrownItem.gameObject);
			return;
		}
		thrownItem.gameObject.SetActive(value: false);
		thrownItem.transform.SetParent(poolParent);
		value.Push(thrownItem);
	}

	private void EnsurePoolParentExists()
	{
		if (!(poolParent != null))
		{
			GameObject obj = new GameObject("Name tag pool");
			Object.DontDestroyOnLoad(obj);
			poolParent = obj.transform;
		}
	}
}
