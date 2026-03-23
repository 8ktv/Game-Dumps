using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private ItemSpawnerSettings settings;

	[SerializeField]
	private ItemSpawnerVisuals visuals;

	private SphereCollider pickUpCollider;

	private double itemTakenTimestamp;

	private readonly List<PlayerInventory> playersInRange = new List<PlayerInventory>();

	private readonly List<GolfCartInfo> golfCartsInRange = new List<GolfCartInfo>();

	[SyncVar(hook = "OnHasItemBoxChanged")]
	private bool hasItemBox;

	[SyncVar(hook = "OnVisualsFillChanged")]
	private float visualsFill;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_hasItemBox;

	public Action<float, float> _Mirror_SyncVarHookDelegate_visualsFill;

	public bool NetworkhasItemBox
	{
		get
		{
			return hasItemBox;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref hasItemBox, 1uL, _Mirror_SyncVarHookDelegate_hasItemBox);
		}
	}

	public float NetworkvisualsFill
	{
		get
		{
			return visualsFill;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref visualsFill, 2uL, _Mirror_SyncVarHookDelegate_visualsFill);
		}
	}

	private void Awake()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDestroy()
	{
		BUpdate.DeregisterCallback(this);
	}

	public override void OnStartServer()
	{
		pickUpCollider = base.gameObject.AddComponent<SphereCollider>();
		pickUpCollider.isTrigger = true;
		pickUpCollider.center = base.transform.InverseTransformPoint(visuals.EffectSourcePosition);
		pickUpCollider.radius = settings.PickUpRadius;
		pickUpCollider.enabled = true;
		ServerSpawnItemBox();
	}

	public override void OnStopServer()
	{
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		foreach (PlayerInventory item in playersInRange)
		{
			if (item != null)
			{
				item.PlayerInfo.AsEntity.WillBeDestroyedReferenced -= OnServerPlayerInRangeWillBeDestroyed;
			}
		}
		foreach (GolfCartInfo item2 in golfCartsInRange)
		{
			if (item2 != null)
			{
				item2.AsEntity.WillBeDestroyedReferenced -= OnServerGolfCartInRangeWillBeDestroyed;
			}
		}
	}

	[Server]
	private void OnTriggerEnter(Collider other)
	{
		Entity foundComponent;
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ItemSpawner::OnTriggerEnter(UnityEngine.Collider)' called when server was not active");
		}
		else if (!other.TryGetComponentInParent<Entity>(out foundComponent, includeInactive: true))
		{
			Debug.LogError("Object " + other.name + " with no Entity component entered an item spawner", base.gameObject);
		}
		else if (foundComponent.IsPlayer)
		{
			if (!playersInRange.Contains(foundComponent.PlayerInfo.Inventory))
			{
				playersInRange.Add(foundComponent.PlayerInfo.Inventory);
				foundComponent.WillBeDestroyedReferenced += OnServerPlayerInRangeWillBeDestroyed;
			}
		}
		else if (foundComponent.IsGolfCart)
		{
			if (!golfCartsInRange.Contains(foundComponent.AsGolfCart))
			{
				golfCartsInRange.Add(foundComponent.AsGolfCart);
				foundComponent.WillBeDestroyedReferenced += OnServerGolfCartInRangeWillBeDestroyed;
			}
		}
		else
		{
			Debug.LogError("Invalid item collector detected on " + other.name, base.gameObject);
		}
	}

	[Server]
	private void OnTriggerExit(Collider other)
	{
		Entity foundComponent;
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ItemSpawner::OnTriggerExit(UnityEngine.Collider)' called when server was not active");
		}
		else if (!other.TryGetComponentInParent<Entity>(out foundComponent, includeInactive: true))
		{
			Debug.LogError("Object " + other.name + " with no Entity component exited an item spawner", base.gameObject);
		}
		else if (foundComponent.IsPlayer)
		{
			playersInRange.Remove(foundComponent.PlayerInfo.Inventory);
			foundComponent.WillBeDestroyedReferenced -= OnServerPlayerInRangeWillBeDestroyed;
		}
		else if (foundComponent.IsGolfCart)
		{
			golfCartsInRange.Remove(foundComponent.AsGolfCart);
			foundComponent.WillBeDestroyedReferenced -= OnServerGolfCartInRangeWillBeDestroyed;
		}
		else
		{
			Debug.LogError("Invalid item collector detected on " + other.name, base.gameObject);
		}
	}

	public void OnBUpdate()
	{
		RotateItemBox();
		if (base.isServer)
		{
			if (hasItemBox)
			{
				ServerTryGiveOutItem();
			}
			else
			{
				ServerTrySpawnItemBox();
			}
		}
		void PlayPickUpSoundForAllClients()
		{
			PlayPickUpSoundInternal();
			foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
			{
				if (value != NetworkServer.localConnection)
				{
					RpcPlayPickUpSound(value);
				}
			}
		}
		void RotateItemBox()
		{
			Vector3 eulerAngles = visuals.transform.rotation.eulerAngles;
			eulerAngles.y += settings.ItemBoxRotationPerSecond * Time.deltaTime;
			visuals.transform.rotation = Quaternion.Euler(eulerAngles);
		}
		bool ServerTryGiveItemToRandomGolfCart()
		{
			int num = UnityEngine.Random.Range(0, golfCartsInRange.Count);
			for (int i = 0; i < golfCartsInRange.Count; i++)
			{
				if (TryGiveItemToGolfCart(golfCartsInRange[num]))
				{
					return true;
				}
				num = BMath.Wrap(num + 1, golfCartsInRange.Count);
			}
			return false;
		}
		bool ServerTryGiveItemToRandomPlayer()
		{
			int num = UnityEngine.Random.Range(0, playersInRange.Count);
			for (int i = 0; i < playersInRange.Count; i++)
			{
				if (TryGiveItemToPlayer(playersInRange[num]))
				{
					return true;
				}
				num = BMath.Wrap(num + 1, playersInRange.Count);
			}
			return false;
		}
		void ServerTryGiveOutItem()
		{
			if (playersInRange.Count > 0)
			{
				ServerTryGiveItemToRandomPlayer();
			}
			else if (golfCartsInRange.Count > 0)
			{
				ServerTryGiveItemToRandomGolfCart();
			}
		}
		void ServerTrySpawnItemBox()
		{
			float timeSince = BMath.GetTimeSince(itemTakenTimestamp);
			if (timeSince < settings.RespawnTime)
			{
				float num = timeSince / settings.RespawnTime;
				SetVisualsFill(num);
			}
			else if (visuals.HasBox)
			{
				Debug.LogError("Item spawner attempted to spawn an item box, but it already has one", base.gameObject);
			}
			else
			{
				ServerSpawnItemBox();
			}
		}
		bool TryGiveItemToGolfCart(GolfCartInfo golfCart)
		{
			if (golfCart == null)
			{
				return false;
			}
			for (int i = 0; i < golfCart.passengers.Count; i++)
			{
				PlayerInfo playerInfo = golfCart.passengers[i];
				if (!(playerInfo == null) && TryGiveItemToPlayer(playerInfo.Inventory))
				{
					return true;
				}
			}
			return false;
		}
		bool TryGiveItemToPlayer(PlayerInventory playerInventory)
		{
			if (playerInventory == null)
			{
				return false;
			}
			if (!playerInventory.HasSpaceForItem(out var _))
			{
				return false;
			}
			NetworkhasItemBox = false;
			ItemType randomItemFor = settings.GetRandomItemFor(playerInventory.PlayerInfo);
			if (!GameManager.AllItems.TryGetItemData(randomItemFor, out var itemData))
			{
				Debug.LogError($"Could not find data for item {randomItemFor}");
				return false;
			}
			playerInventory.ServerTryAddItem(randomItemFor, itemData.MaxUses);
			CourseManager.InformPlayerPickedUpItem(playerInventory.PlayerInfo);
			playerInventory.PlayerInfo.RpcInformPickedUpItemFromItemSpawner();
			PlayPickUpSoundForAllClients();
			return true;
		}
	}

	[TargetRpc]
	private void RpcPlayPickUpSound(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void ItemSpawner::RpcPlayPickUpSound(Mirror.NetworkConnectionToClient)", -1116050663, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayPickUpSoundInternal()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.ItemBoxCollectedEvent, visuals.EffectSourcePosition);
	}

	[Server]
	private void ServerSpawnItemBox()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ItemSpawner::ServerSpawnItemBox()' called when server was not active");
		}
		else
		{
			NetworkhasItemBox = true;
		}
	}

	[Server]
	private void SetVisualsFill(float fill)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ItemSpawner::SetVisualsFill(System.Single)' called when server was not active");
		}
		else
		{
			NetworkvisualsFill = fill;
		}
	}

	private void OnHasItemBoxChanged(bool hadItemBox, bool hasItemBox)
	{
		if (hasItemBox == hadItemBox)
		{
			return;
		}
		if (!hasItemBox)
		{
			if (base.isServer)
			{
				itemTakenTimestamp = Time.timeAsDouble;
				SetVisualsFill(0f);
			}
			visuals.SetIsTaken(isTaken: true);
		}
		else
		{
			visuals.SetIsTaken(isTaken: false);
		}
	}

	private void OnVisualsFillChanged(float previousFill, float currentFill)
	{
		visuals.SetFill(visualsFill);
	}

	private void OnServerPlayerInRangeWillBeDestroyed(Entity playerAsEntity)
	{
		playersInRange.Remove(playerAsEntity.PlayerInfo.Inventory);
	}

	private void OnServerGolfCartInRangeWillBeDestroyed(Entity golfCartAsEntity)
	{
		golfCartsInRange.Remove(golfCartAsEntity.AsGolfCart);
	}

	public ItemSpawner()
	{
		_Mirror_SyncVarHookDelegate_hasItemBox = OnHasItemBoxChanged;
		_Mirror_SyncVarHookDelegate_visualsFill = OnVisualsFillChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_RpcPlayPickUpSound__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		PlayPickUpSoundInternal();
	}

	protected static void InvokeUserCode_RpcPlayPickUpSound__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayPickUpSound called on server.");
		}
		else
		{
			((ItemSpawner)obj).UserCode_RpcPlayPickUpSound__NetworkConnectionToClient(null);
		}
	}

	static ItemSpawner()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(ItemSpawner), "System.Void ItemSpawner::RpcPlayPickUpSound(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcPlayPickUpSound__NetworkConnectionToClient);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(hasItemBox);
			writer.WriteFloat(visualsFill);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(hasItemBox);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteFloat(visualsFill);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref hasItemBox, _Mirror_SyncVarHookDelegate_hasItemBox, reader.ReadBool());
			GeneratedSyncVarDeserialize(ref visualsFill, _Mirror_SyncVarHookDelegate_visualsFill, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref hasItemBox, _Mirror_SyncVarHookDelegate_hasItemBox, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref visualsFill, _Mirror_SyncVarHookDelegate_visualsFill, reader.ReadFloat());
		}
	}
}
