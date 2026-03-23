using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerCustomizationBuilding : SingletonNetworkBehaviour<PlayerCustomizationBuilding>
{
	[SerializeField]
	private Transform exitPosition;

	[SerializeField]
	private float exitPositionMaxOffset;

	[SerializeField]
	private PlayerCustomizeBuildingVfx vfx;

	private AntiCheatPerPlayerRateChecker serverDoorOpenCommandRateLimiter;

	public override void OnStartServer()
	{
		serverDoorOpenCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Door open", vfx.OpenDoorsCooldown * 0.5f, 5, 10, vfx.OpenDoorsCooldown * 2f);
	}

	public static void InformLocalPlayerEntered()
	{
		if (SingletonNetworkBehaviour<PlayerCustomizationBuilding>.HasInstance)
		{
			SingletonNetworkBehaviour<PlayerCustomizationBuilding>.Instance.InformLocalPlayerEnteredInternal();
		}
	}

	public static void InformLocalPlayerExited()
	{
		if (SingletonNetworkBehaviour<PlayerCustomizationBuilding>.HasInstance)
		{
			SingletonNetworkBehaviour<PlayerCustomizationBuilding>.Instance.InformLocalPlayerExitedInternal();
		}
	}

	private void InformLocalPlayerEnteredInternal()
	{
		OpenDoorsForAllClients();
	}

	private void InformLocalPlayerExitedInternal()
	{
		SetLocalPlayerToExitPosition();
		OpenDoorsForAllClients();
	}

	private void SetLocalPlayerToExitPosition()
	{
		if (!(GameManager.LocalPlayerMovement == null))
		{
			Vector3 position = exitPosition.position;
			position += (Random.insideUnitCircle * exitPositionMaxOffset).AsHorizontal3();
			GameManager.LocalPlayerMovement.Teleport(position, exitPosition.rotation, resetState: true);
		}
	}

	private void OpenDoorsForAllClients()
	{
		OpenDoorsInternal();
		CmdOpenDoorsForAllClients();
	}

	[Command]
	private void CmdOpenDoorsForAllClients(NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdOpenDoorsForAllClients__NetworkConnectionToClient(sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendCommandInternal("System.Void PlayerCustomizationBuilding::CmdOpenDoorsForAllClients(Mirror.NetworkConnectionToClient)", -312061665, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcOpenDoors(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void PlayerCustomizationBuilding::RpcOpenDoors(Mirror.NetworkConnectionToClient)", -183823574, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OpenDoorsInternal()
	{
		vfx.OpenDoors();
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdOpenDoorsForAllClients__NetworkConnectionToClient(NetworkConnectionToClient sender)
	{
		if (!serverDoorOpenCommandRateLimiter.RegisterHit(sender))
		{
			return;
		}
		if (sender != null && sender != NetworkServer.localConnection)
		{
			OpenDoorsInternal();
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcOpenDoors(value);
			}
		}
	}

	protected static void InvokeUserCode_CmdOpenDoorsForAllClients__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdOpenDoorsForAllClients called on client.");
		}
		else
		{
			((PlayerCustomizationBuilding)obj).UserCode_CmdOpenDoorsForAllClients__NetworkConnectionToClient(senderConnection);
		}
	}

	protected void UserCode_RpcOpenDoors__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		OpenDoorsInternal();
	}

	protected static void InvokeUserCode_RpcOpenDoors__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcOpenDoors called on server.");
		}
		else
		{
			((PlayerCustomizationBuilding)obj).UserCode_RpcOpenDoors__NetworkConnectionToClient(null);
		}
	}

	static PlayerCustomizationBuilding()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerCustomizationBuilding), "System.Void PlayerCustomizationBuilding::CmdOpenDoorsForAllClients(Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdOpenDoorsForAllClients__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerCustomizationBuilding), "System.Void PlayerCustomizationBuilding::RpcOpenDoors(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcOpenDoors__NetworkConnectionToClient);
	}
}
