using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item pool", menuName = "Settings/Gameplay/Item pool")]
public class ItemPool : ScriptableObject
{
	[Serializable]
	public struct ItemSpawnChance
	{
		public ItemType item;

		[Min(0f)]
		public float spawnChanceWeight;
	}

	[SerializeField]
	[DynamicElementName("item")]
	private ItemSpawnChance[] spawnChances;

	[SerializeField]
	[HideInInspector]
	private float totalSpawnChanceWeight;

	public ItemSpawnChance[] SpawnChances => spawnChances;

	public float TotalSpawnChanceWeight => totalSpawnChanceWeight;

	private void OnValidate()
	{
		UpdateTotalWeight();
	}

	public float GetSpawnChanceWeight(ItemType itemType)
	{
		ItemSpawnChance[] array = spawnChances;
		for (int i = 0; i < array.Length; i++)
		{
			ItemSpawnChance itemSpawnChance = array[i];
			if (itemType == itemSpawnChance.item)
			{
				return itemSpawnChance.spawnChanceWeight;
			}
		}
		return 0f;
	}

	public void UpdateTotalWeight()
	{
		totalSpawnChanceWeight = 0f;
		ItemSpawnChance[] array = spawnChances;
		for (int i = 0; i < array.Length; i++)
		{
			ItemSpawnChance itemSpawnChance = array[i];
			if (itemSpawnChance.item == ItemType.None)
			{
				Debug.LogError($"Item with type {ItemType.None} found in an item spawner's pool");
			}
			else
			{
				totalSpawnChanceWeight += itemSpawnChance.spawnChanceWeight;
			}
		}
	}

	public ItemType GetWeightedRandomItem()
	{
		float num = UnityEngine.Random.value * totalSpawnChanceWeight;
		float num2 = 0f;
		for (int i = 0; i < spawnChances.Length - 1; i++)
		{
			ItemSpawnChance itemSpawnChance = spawnChances[i];
			num2 += itemSpawnChance.spawnChanceWeight;
			if (num < num2)
			{
				return itemSpawnChance.item;
			}
		}
		return spawnChances[^1].item;
	}

	public bool ContainsItemType(ItemType itemType)
	{
		ItemSpawnChance[] array = spawnChances;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].item == itemType)
			{
				return true;
			}
		}
		return false;
	}
}
