using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

[DisallowMultipleComponent]
public class EquipmentSwitcher : NetworkBehaviour
{
	[SerializeField]
	[SyncVar(hook = "OnEquipmentTypeChanged")]
	private EquipmentType equipmentType;

	private PlayerInfo owner;

	private readonly Dictionary<EquipmentType, Equipment> cachedEquipment = new Dictionary<EquipmentType, Equipment>();

	public Action<EquipmentType, EquipmentType> _Mirror_SyncVarHookDelegate_equipmentType;

	public Equipment CurrentEquipment { get; private set; }

	public EquipmentType NetworkequipmentType
	{
		get
		{
			return equipmentType;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref equipmentType, 1uL, _Mirror_SyncVarHookDelegate_equipmentType);
		}
	}

	protected override void OnValidate()
	{
	}

	private void Awake()
	{
		syncDirection = SyncDirection.ClientToServer;
		owner = GetComponentInParent<PlayerInfo>();
	}

	public void OnWillBeDestroyed()
	{
		ApplyUnequip();
		foreach (Equipment value in cachedEquipment.Values)
		{
			EquipmentManager.ReturnEquipment(value);
		}
	}

	public void SetEquipment(EquipmentType type)
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to set their own equipment", base.gameObject);
		}
		else
		{
			NetworkequipmentType = type;
		}
	}

	public Equipment GetEquipment(EquipmentType type)
	{
		return GetOrCreateEquipment(type);
	}

	public void SetEquipmentPreviewLocal(EquipmentType type)
	{
		if (base.netIdentity != null)
		{
			Debug.LogError("This is for local character preview only, don't actually call this on players!!!", base.gameObject);
		}
		else
		{
			OnEquipmentTypeChanged(EquipmentType.None, type);
		}
	}

	private void OnEquipmentTypeChanged(EquipmentType previousType, EquipmentType currentType)
	{
		if (currentType == EquipmentType.None)
		{
			ApplyUnequip();
		}
		else
		{
			ApplyEquip(currentType);
		}
		void ApplyEquip(EquipmentType type)
		{
			ApplyUnequip();
			CurrentEquipment = GetOrCreateEquipment(type);
			CurrentEquipment.gameObject.SetActive(value: true);
			CurrentEquipment.gameObject.SetPlayerShaderIndexOnRenderers(owner);
			CurrentEquipment.gameObject.SetLayerRecursively(GameManager.LayerSettings.ItemsLayer);
		}
	}

	private void ApplyUnequip()
	{
		if (!(CurrentEquipment == null))
		{
			if (!cachedEquipment.TryGetValue(CurrentEquipment.Type, out var value) || value != CurrentEquipment)
			{
				EquipmentManager.ReturnEquipment(CurrentEquipment);
			}
			else
			{
				CurrentEquipment.gameObject.SetActive(value: false);
			}
			CurrentEquipment = null;
		}
	}

	private Equipment GetOrCreateEquipment(EquipmentType type)
	{
		Equipment equipment = GetOrCreate();
		InitializeEquipment(equipment);
		return equipment;
		Equipment GetOrCreate()
		{
			if (cachedEquipment.TryGetValue(type, out var value))
			{
				return value;
			}
			Equipment unusedEquipment = EquipmentManager.GetUnusedEquipment(type);
			cachedEquipment.Add(type, unusedEquipment);
			unusedEquipment.gameObject.SetActive(value: false);
			return unusedEquipment;
		}
	}

	private void InitializeEquipment(Equipment equipment)
	{
		equipment.transform.SetParent(base.transform);
		equipment.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		equipment.transform.localScale = Vector3.one;
	}

	public EquipmentSwitcher()
	{
		_Mirror_SyncVarHookDelegate_equipmentType = OnEquipmentTypeChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_EquipmentType(writer, equipmentType);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_EquipmentType(writer, equipmentType);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref equipmentType, _Mirror_SyncVarHookDelegate_equipmentType, GeneratedNetworkCode._Read_EquipmentType(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref equipmentType, _Mirror_SyncVarHookDelegate_equipmentType, GeneratedNetworkCode._Read_EquipmentType(reader));
		}
	}
}
