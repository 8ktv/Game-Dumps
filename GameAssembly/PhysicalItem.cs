using FMODUnity;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.Localization;

public class PhysicalItem : NetworkBehaviour, IInteractable
{
	[SerializeField]
	private ItemType itemType;

	private bool isPickupSuppressed;

	private AntiCheatPerPlayerRateChecker serverGiveToPlayerCommandRateLimiter;

	public int RemainingUses { get; private set; }

	public Entity AsEntity { get; private set; }

	public LocalizedString InteractString => LocalizationManager.GetLocalizedString(StringTable.Data, "ITEM_" + itemType);

	public bool IsInteractionEnabled => !isPickupSuppressed;

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer obj in componentsInChildren)
		{
			obj.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat("_CameraFadeNearDistance", 0f);
			materialPropertyBlock.SetFloat("_CameraFadeFarDistance", 0f);
			obj.SetPropertyBlock(materialPropertyBlock);
		}
	}

	public override void OnStartServer()
	{
		serverGiveToPlayerCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " give to player", 0.025f, 10, 20, 0.05f);
	}

	public void Initialize(int remainingUses)
	{
		RemainingUses = remainingUses;
	}

	public void LocalPlayerInteract()
	{
		CmdGiveToPlayer(GameManager.LocalPlayerInventory);
	}

	[Command(requiresAuthority = false)]
	private void CmdGiveToPlayer(PlayerInventory player, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdGiveToPlayer__PlayerInventory__NetworkConnectionToClient(player, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(player);
		SendCommandInternal("System.Void PhysicalItem::CmdGiveToPlayer(PlayerInventory,Mirror.NetworkConnectionToClient)", 602422712, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	public void SetIsPickupSuppressed(bool issuppressed)
	{
		isPickupSuppressed = issuppressed;
	}

	[Server]
	public void ServerPlayWaterSplashForAllClients(Vector3 worldPosition)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PhysicalItem::ServerPlayWaterSplashForAllClients(UnityEngine.Vector3)' called when server was not active");
			return;
		}
		PlayWaterSplashInternal(worldPosition);
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection)
			{
				RpcPlayWaterSplash(value, worldPosition);
			}
		}
	}

	[TargetRpc]
	private void RpcPlayWaterSplash(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(worldPosition);
		SendTargetRPCInternal(connection, "System.Void PhysicalItem::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", -1557213870, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayWaterSplashInternal(Vector3 worldPosition)
	{
		VfxManager.PlayPooledVfxLocalOnly(VfxType.WaterImpactSmall, worldPosition, Quaternion.identity);
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.ItemWaterSplash, worldPosition);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdGiveToPlayer__PlayerInventory__NetworkConnectionToClient(PlayerInventory player, NetworkConnectionToClient sender)
	{
		if (!serverGiveToPlayerCommandRateLimiter.RegisterHit(sender) || isPickupSuppressed || !player.HasSpaceForItem(out var _) || !player.ServerTryAddItem(itemType, RemainingUses))
		{
			return;
		}
		if (itemType == ItemType.Landmine)
		{
			Landmine component = GetComponent<Landmine>();
			if (component.IsArmed && component.Owner != player)
			{
				player.PlayerInfo.RpcInformDisarmedLandmine();
			}
		}
		NetworkServer.Destroy(base.gameObject);
	}

	protected static void InvokeUserCode_CmdGiveToPlayer__PlayerInventory__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdGiveToPlayer called on client.");
		}
		else
		{
			((PhysicalItem)obj).UserCode_CmdGiveToPlayer__PlayerInventory__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInventory>(), senderConnection);
		}
	}

	protected void UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkConnectionToClient connection, Vector3 worldPosition)
	{
		PlayWaterSplashInternal(worldPosition);
	}

	protected static void InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayWaterSplash called on server.");
		}
		else
		{
			((PhysicalItem)obj).UserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3(null, reader.ReadVector3());
		}
	}

	static PhysicalItem()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PhysicalItem), "System.Void PhysicalItem::CmdGiveToPlayer(PlayerInventory,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdGiveToPlayer__PlayerInventory__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(PhysicalItem), "System.Void PhysicalItem::RpcPlayWaterSplash(Mirror.NetworkConnectionToClient,UnityEngine.Vector3)", InvokeUserCode_RpcPlayWaterSplash__NetworkConnectionToClient__Vector3);
	}
}
