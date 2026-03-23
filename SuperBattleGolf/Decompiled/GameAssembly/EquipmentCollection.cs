using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment collection", menuName = "Settings/Gameplay/Equipment collection")]
public class EquipmentCollection : ScriptableObject
{
	[SerializeField]
	[DynamicElementName("Type")]
	private EquipmentSettings[] equipment;

	private Dictionary<EquipmentType, EquipmentSettings> equipmentDictionary = new Dictionary<EquipmentType, EquipmentSettings>();

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	public bool TryGetEquipmentSettings(EquipmentType type, out EquipmentSettings equipmentSettings)
	{
		return equipmentDictionary.TryGetValue(type, out equipmentSettings);
	}

	private void Initialize()
	{
		equipmentDictionary.Clear();
		EquipmentSettings[] array = equipment;
		foreach (EquipmentSettings equipmentSettings in array)
		{
			equipmentDictionary.Add(equipmentSettings.Type, equipmentSettings);
		}
	}
}
