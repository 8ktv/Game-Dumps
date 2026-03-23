using System;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class LevelBoundsTrackerNetworker : NetworkBehaviour
{
	[SerializeField]
	private LevelBoundsTracker tracker;

	[SyncVar(hook = "OnBoundsStateChanged")]
	private BoundsState boundsState;

	[SyncVar(hook = "OnIsOnGreenChanged")]
	private bool isOnGreen;

	private AntiCheatPerPlayerRateChecker serverInformTeleportedIntoBoundsCommandRateLimiter;

	public Action<BoundsState, BoundsState> _Mirror_SyncVarHookDelegate_boundsState;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isOnGreen;

	public BoundsState BoundsState => boundsState;

	public bool IsOnGreen => isOnGreen;

	public BoundsState NetworkboundsState
	{
		get
		{
			return boundsState;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref boundsState, 1uL, _Mirror_SyncVarHookDelegate_boundsState);
		}
	}

	public bool NetworkisOnGreen
	{
		get
		{
			return isOnGreen;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isOnGreen, 2uL, _Mirror_SyncVarHookDelegate_isOnGreen);
		}
	}

	public event Action<BoundsState, BoundsState> BoundsStateChanged;

	public event Action IsOnGreenChanged;

	public override void OnStartServer()
	{
		serverInformTeleportedIntoBoundsCommandRateLimiter = new AntiCheatPerPlayerRateChecker(base.name + " inform teleported into bounds", 0.5f, 5, 10, 2f);
	}

	public void SetBoundsState(BoundsState boundsState)
	{
		NetworkboundsState = boundsState;
	}

	public void SetIsOnGreen(bool isOnGreen)
	{
		NetworkisOnGreen = isOnGreen;
	}

	[Command]
	public void CmdInformTeleportedIntoBounds(Vector3 position, Quaternion rotation, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdInformTeleportedIntoBounds__Vector3__Quaternion__NetworkConnectionToClient(position, rotation, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		SendCommandInternal("System.Void LevelBoundsTrackerNetworker::CmdInformTeleportedIntoBounds(UnityEngine.Vector3,UnityEngine.Quaternion,Mirror.NetworkConnectionToClient)", 824407694, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	public void RpcReturnToBounds(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(position);
		writer.WriteQuaternion(rotation);
		SendTargetRPCInternal(connection, "System.Void LevelBoundsTrackerNetworker::RpcReturnToBounds(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion)", -462674941, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void OnBoundsStateChanged(BoundsState previousState, BoundsState currentState)
	{
		this.BoundsStateChanged?.Invoke(previousState, currentState);
	}

	private void OnIsOnGreenChanged(bool wasOnGreen, bool isOnGreen)
	{
		this.IsOnGreenChanged?.Invoke();
	}

	public LevelBoundsTrackerNetworker()
	{
		_Mirror_SyncVarHookDelegate_boundsState = OnBoundsStateChanged;
		_Mirror_SyncVarHookDelegate_isOnGreen = OnIsOnGreenChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdInformTeleportedIntoBounds__Vector3__Quaternion__NetworkConnectionToClient(Vector3 position, Quaternion rotation, NetworkConnectionToClient sender)
	{
		if (serverInformTeleportedIntoBoundsCommandRateLimiter.RegisterHit(sender))
		{
			tracker.InformTeleportedIntoBounds(position, rotation);
		}
	}

	protected static void InvokeUserCode_CmdInformTeleportedIntoBounds__Vector3__Quaternion__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdInformTeleportedIntoBounds called on client.");
		}
		else
		{
			((LevelBoundsTrackerNetworker)obj).UserCode_CmdInformTeleportedIntoBounds__Vector3__Quaternion__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadQuaternion(), senderConnection);
		}
	}

	protected void UserCode_RpcReturnToBounds__NetworkConnectionToClient__Vector3__Quaternion(NetworkConnectionToClient connection, Vector3 position, Quaternion rotation)
	{
		tracker.ReturnToBoundsInternal(position, rotation);
	}

	protected static void InvokeUserCode_RpcReturnToBounds__NetworkConnectionToClient__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcReturnToBounds called on server.");
		}
		else
		{
			((LevelBoundsTrackerNetworker)obj).UserCode_RpcReturnToBounds__NetworkConnectionToClient__Vector3__Quaternion(null, reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	static LevelBoundsTrackerNetworker()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(LevelBoundsTrackerNetworker), "System.Void LevelBoundsTrackerNetworker::CmdInformTeleportedIntoBounds(UnityEngine.Vector3,UnityEngine.Quaternion,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdInformTeleportedIntoBounds__Vector3__Quaternion__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(LevelBoundsTrackerNetworker), "System.Void LevelBoundsTrackerNetworker::RpcReturnToBounds(Mirror.NetworkConnectionToClient,UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_RpcReturnToBounds__NetworkConnectionToClient__Vector3__Quaternion);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_BoundsState(writer, boundsState);
			writer.WriteBool(isOnGreen);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_BoundsState(writer, boundsState);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isOnGreen);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref boundsState, _Mirror_SyncVarHookDelegate_boundsState, GeneratedNetworkCode._Read_BoundsState(reader));
			GeneratedSyncVarDeserialize(ref isOnGreen, _Mirror_SyncVarHookDelegate_isOnGreen, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref boundsState, _Mirror_SyncVarHookDelegate_boundsState, GeneratedNetworkCode._Read_BoundsState(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isOnGreen, _Mirror_SyncVarHookDelegate_isOnGreen, reader.ReadBool());
		}
	}
}
