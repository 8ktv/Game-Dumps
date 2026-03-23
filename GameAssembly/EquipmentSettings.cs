using System;
using UnityEngine;

[Serializable]
public class EquipmentSettings
{
	[field: SerializeField]
	public EquipmentType Type { get; private set; }

	[field: SerializeField]
	public Equipment Prefab { get; private set; }
}
