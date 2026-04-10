using System;
using System.Runtime.InteropServices;
using FMODUnity;
using Mirror;
using UnityEngine;

public class BreakableIce : NetworkBehaviour
{
	[SerializeField]
	private Entity asEntity;

	[SerializeField]
	private MeshRenderer renderer;

	[SerializeField]
	private BreakableIceState initialState = BreakableIceState.Pristine;

	[SyncVar(hook = "OnStateChanged")]
	private BreakableIceState state;

	public Action<BreakableIceState, BreakableIceState> _Mirror_SyncVarHookDelegate_state;

	public BreakableIceState Networkstate
	{
		get
		{
			return state;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref state, 1uL, _Mirror_SyncVarHookDelegate_state);
		}
	}

	private void OnDestroy()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.IceBreakableFloorBreak, out var particleSystem))
			{
				Debug.LogError("Failed to get breakable ice break VFX");
				return;
			}
			if (!particleSystem.TryGetComponent<IceBreakableFloorVfx>(out var component))
			{
				Debug.LogError("Breakable ice break VFX doesn't have IceBreakableFloorVfx component");
				particleSystem.ReturnToPool();
				return;
			}
			particleSystem.transform.position = base.transform.position;
			component.SetData(this);
			particleSystem.Play();
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.BreakableIceBreakingEvent, base.transform.position);
		}
	}

	public override void OnStartServer()
	{
		Networkstate = initialState;
		asEntity.AsHittable.WasHitByItem += OnServerWasHitByItem;
	}

	public override void OnStopServer()
	{
		asEntity.AsHittable.WasHitByItem -= OnServerWasHitByItem;
	}

	public override void OnStartClient()
	{
		UpdateMaterial();
	}

	private void UpdateMaterial()
	{
		Material material = ((state != BreakableIceState.Cracked) ? GameManager.HazardSettings.BreakableIcePristineMaterial : GameManager.HazardSettings.BreakableIceCrackedMaterial);
		Material sharedMaterial = material;
		renderer.sharedMaterial = sharedMaterial;
	}

	[Server]
	private void ServerRegisterHit()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void BreakableIce::ServerRegisterHit()' called when server was not active");
		}
		else if (state == BreakableIceState.Pristine)
		{
			Networkstate = BreakableIceState.Cracked;
		}
		else if (state == BreakableIceState.Cracked && UnityEngine.Random.value <= GameManager.HazardSettings.BreakableIceHitBreakChance)
		{
			asEntity.DestroyEntity();
		}
	}

	private void OnServerWasHitByItem(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 direction, float distance, bool isReflected)
	{
		if (!GameManager.AllItems.TryGetItemData(itemType, out var itemData))
		{
			Debug.LogError($"Could not find data for item {itemType}");
		}
		else if (itemData.CanBreakBreakableIce)
		{
			ServerRegisterHit();
		}
	}

	private void OnStateChanged(BreakableIceState previousState, BreakableIceState currentState)
	{
		UpdateMaterial();
		if (previousState != BreakableIceState.Uninitialized && state == BreakableIceState.Cracked)
		{
			PlayCrackedEffects();
		}
		void PlayCrackedEffects()
		{
			IceBreakableFloorVfx component;
			if (!VfxPersistentData.TryGetPooledVfx(VfxType.IceBreakableFloorCrack, out var particleSystem))
			{
				Debug.LogError("Failed to get breakable ice break VFX");
			}
			else if (!particleSystem.TryGetComponent<IceBreakableFloorVfx>(out component))
			{
				Debug.LogError("Breakable ice break VFX doesn't have IceBreakableFloorVfx component");
				particleSystem.ReturnToPool();
			}
			else
			{
				particleSystem.transform.position = base.transform.position;
				component.SetData(this);
				particleSystem.Play();
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.BreakableIceCrackingEvent, base.transform.position);
			}
		}
	}

	public BreakableIce()
	{
		_Mirror_SyncVarHookDelegate_state = OnStateChanged;
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
			GeneratedNetworkCode._Write_BreakableIceState(writer, state);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_BreakableIceState(writer, state);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_BreakableIceState(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref state, _Mirror_SyncVarHookDelegate_state, GeneratedNetworkCode._Read_BreakableIceState(reader));
		}
	}
}
