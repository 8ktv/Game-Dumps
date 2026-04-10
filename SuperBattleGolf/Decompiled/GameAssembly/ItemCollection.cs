using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item collection", menuName = "Settings/Gameplay/Item collection")]
public class ItemCollection : ScriptableObject
{
	[DynamicElementName("Type")]
	[SerializeField]
	private ItemData[] items;

	private readonly Dictionary<ItemType, ItemData> allItemData = new Dictionary<ItemType, ItemData>();

	public int Count => items.Length;

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		allItemData.Clear();
		ItemData[] array = items;
		foreach (ItemData itemData in array)
		{
			itemData.Initialize();
			if (itemData.Type == ItemType.None)
			{
				Debug.LogError($"Item with type {ItemType.None} found in an item collection");
			}
			else if (!allItemData.TryAdd(itemData.Type, itemData))
			{
				Debug.LogError($"Attempted to add item of type {itemData.Type} more than once to an item collection");
			}
		}
	}

	public ItemData GetItemAtIndex(int index)
	{
		return items[index];
	}

	public bool TryGetItemData(ItemType itemType, out ItemData itemData)
	{
		return allItemData.TryGetValue(itemType, out itemData);
	}

	public Sprite GetItemIcon(ItemType itemType)
	{
		if (itemType == ItemType.None)
		{
			return null;
		}
		if (!allItemData.TryGetValue(itemType, out var value))
		{
			Debug.LogError($"Could not find data for item {itemType}");
			return null;
		}
		return value.Icon;
	}
}
