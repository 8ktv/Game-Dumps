using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : SingletonBehaviour<EquipmentManager>
{
	[SerializeField]
	private EquipmentCollection equipmentCollection;

	[SerializeField]
	private int maxIndividualPoolSize;

	private static Transform equipmentPoolParent;

	private static readonly Dictionary<EquipmentType, Stack<Equipment>> equipmentPools = new Dictionary<EquipmentType, Stack<Equipment>>();

	public static Equipment GetUnusedEquipment(EquipmentType type)
	{
		if (!SingletonBehaviour<EquipmentManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<EquipmentManager>.Instance.GetUnusedEquipmentInternal(type);
	}

	public static void ReturnEquipment(Equipment equipment)
	{
		if (SingletonBehaviour<EquipmentManager>.HasInstance)
		{
			SingletonBehaviour<EquipmentManager>.Instance.ReturnEquipmentInternal(equipment);
		}
	}

	private Equipment GetUnusedEquipmentInternal(EquipmentType type)
	{
		EnsurePoolParentExists();
		if (!equipmentPools.TryGetValue(type, out var value))
		{
			value = new Stack<Equipment>();
			equipmentPools.Add(type, value);
		}
		Equipment result = null;
		while (result == null)
		{
			if (!value.TryPop(out result))
			{
				if (!equipmentCollection.TryGetEquipmentSettings(type, out var equipmentSettings) || equipmentSettings.Prefab == null)
				{
					Debug.LogError($"Could not find containing a valid prefab for equipment type {type}");
					return null;
				}
				result = Object.Instantiate(equipmentSettings.Prefab);
			}
		}
		result.gameObject.SetActive(value: true);
		return result;
	}

	private void ReturnEquipmentInternal(Equipment equipment)
	{
		if (!equipmentPools.TryGetValue(equipment.Type, out var value))
		{
			value = new Stack<Equipment>();
			equipmentPools.Add(equipment.Type, value);
		}
		if (value.Count >= maxIndividualPoolSize)
		{
			Object.Destroy(equipment.gameObject);
			return;
		}
		equipment.gameObject.SetActive(value: false);
		equipment.transform.SetParent(equipmentPoolParent);
		value.Push(equipment);
	}

	private void EnsurePoolParentExists()
	{
		if (!(equipmentPoolParent != null))
		{
			GameObject obj = new GameObject("Equipment pool");
			Object.DontDestroyOnLoad(obj);
			equipmentPoolParent = obj.transform;
		}
	}
}
