using System;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class LocalSpectatorCameraFollower : NetworkBehaviour
{
	public GameObject model;

	[SyncVar(hook = "OnTargetChange")]
	public Transform target;

	[SyncVar(hook = "OnOwnerChange")]
	public PlayerInfo owner;

	public float smoothing = 16f;

	public float targetChangedSpeed = 16f;

	public float maxTargetChangeDuration = 4f;

	public AnimationCurve targetChangeCurve;

	private bool wasOwned;

	private bool visible;

	private Vector3 currentOffset;

	private Vector3 targetOffset;

	private int lastReceiveFrame;

	private float lastSend;

	private float lastTargetChange;

	private float targetChangeDuration;

	private Vector3 lastTargetPosition;

	private NameTagUi nameTag;

	private float sendPeriod;

	private AntiCheatPerPlayerRateChecker serverUpdateCommandRateLimiter;

	protected NetworkBehaviourSyncVar ___ownerNetId;

	public Action<Transform, Transform> _Mirror_SyncVarHookDelegate_target;

	public Action<PlayerInfo, PlayerInfo> _Mirror_SyncVarHookDelegate_owner;

	public Transform Networktarget
	{
		get
		{
			return target;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref target, 1uL, _Mirror_SyncVarHookDelegate_target);
		}
	}

	public PlayerInfo Networkowner
	{
		get
		{
			return GetSyncVarNetworkBehaviour(___ownerNetId, ref owner);
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter_NetworkBehaviour(value, ref owner, 2uL, _Mirror_SyncVarHookDelegate_owner, ref ___ownerNetId);
		}
	}

	private void Awake()
	{
		sendPeriod = 1f / (float)NetworkManager.singleton.sendRate;
	}

	public override void OnStartServer()
	{
		serverUpdateCommandRateLimiter = new AntiCheatPerPlayerRateChecker("Spectator camera update", sendPeriod * 0.5f, NetworkManager.singleton.sendRate * 5, NetworkManager.singleton.sendRate * 15, sendPeriod * 2f, 25);
	}

	public override void OnStartClient()
	{
		model.SetActive(value: false);
		visible = false;
		if (base.isOwned && !(GameManager.LocalPlayerInfo == null))
		{
			PlayerSpectator.LocalPlayerSetSpectatingTarget += OnLocalPlayerSetSpectatingTarget;
			Networktarget = GameManager.LocalPlayerInfo.AsSpectator.Target;
			Networkowner = GameManager.LocalPlayerInfo;
			wasOwned = true;
		}
	}

	private void OnDestroy()
	{
		if (wasOwned)
		{
			PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnLocalPlayerSetSpectatingTarget;
		}
		if (nameTag != null)
		{
			NameTagManager.ReturnNameTag(nameTag);
			nameTag = null;
		}
	}

	public void OnLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		Networktarget = ((GameManager.LocalPlayerInfo != null) ? GameManager.LocalPlayerInfo.AsSpectator.Target : null);
	}

	[Command(channel = 1)]
	public void CmdUpdateState(Vector3 offset, int frame, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdUpdateState__Vector3__Int32__NetworkConnectionToClient(offset, frame, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(offset);
		writer.WriteVarInt(frame);
		SendCommandInternal("System.Void LocalSpectatorCameraFollower::CmdUpdateState(UnityEngine.Vector3,System.Int32,Mirror.NetworkConnectionToClient)", 619253773, writer, 1);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc(channel = 1)]
	public void RpcUpdateState(Vector3 offset, int frame)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(offset);
		writer.WriteVarInt(frame);
		SendRPCInternal("System.Void LocalSpectatorCameraFollower::RpcUpdateState(UnityEngine.Vector3,System.Int32)", 1721101469, writer, 1, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	private void LateUpdate()
	{
		if (target == null)
		{
			return;
		}
		Vector3 position = target.position;
		if (base.isOwned)
		{
			if (Time.time - lastSend > sendPeriod)
			{
				Vector3 offset = position - GameManager.Camera.transform.position;
				CmdUpdateState(offset, Time.frameCount);
				lastSend = Time.time;
			}
			return;
		}
		float num = Time.time - lastTargetChange;
		Vector3 vector = ((!(num < targetChangeDuration)) ? position : Vector3.Lerp(lastTargetPosition, position, targetChangeCurve.Evaluate(num / targetChangeDuration)));
		currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothing);
		base.transform.position = vector - currentOffset;
		base.transform.LookAt(position);
		bool flag = CameraModuleController.CurrentModuleType != CameraModuleType.Overview;
		if (visible != flag)
		{
			visible = flag;
			model.SetActive(visible);
		}
	}

	private void OnTargetChange(Transform prev, Transform curr)
	{
		if (!base.isOwned && !(curr == null))
		{
			lastTargetChange = Time.time;
			lastTargetPosition = ((prev == null) ? curr.position : (base.transform.position + currentOffset));
			targetChangeDuration = BMath.Min(maxTargetChangeDuration, Vector3.Distance(lastTargetPosition, curr.position) / targetChangedSpeed);
		}
	}

	private void OnOwnerChange(PlayerInfo prev, PlayerInfo owner)
	{
		owner.VoiceChat.voiceNetworker.SetTargetTransform(base.transform);
		if (!base.isOwned)
		{
			if (nameTag != null)
			{
				NameTagManager.ReturnNameTag(nameTag);
			}
			nameTag = NameTagManager.GetUnusedNameTag();
			nameTag.Initialize(NameTagManager.SpectatorNameTagSettings, base.transform, GameManager.UiSettings.SpectatorNameTagLocalOffset, GameManager.UiSettings.SpectatorNameTagWorldOffset, owner.PlayerId.PlayerNameNoRichText, owner, nameTagIsPlayer: false);
		}
	}

	public LocalSpectatorCameraFollower()
	{
		_Mirror_SyncVarHookDelegate_target = OnTargetChange;
		_Mirror_SyncVarHookDelegate_owner = OnOwnerChange;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdUpdateState__Vector3__Int32__NetworkConnectionToClient(Vector3 offset, int frame, NetworkConnectionToClient sender)
	{
		if (serverUpdateCommandRateLimiter.RegisterHit(sender))
		{
			RpcUpdateState(offset, frame);
		}
	}

	protected static void InvokeUserCode_CmdUpdateState__Vector3__Int32__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdUpdateState called on client.");
		}
		else
		{
			((LocalSpectatorCameraFollower)obj).UserCode_CmdUpdateState__Vector3__Int32__NetworkConnectionToClient(reader.ReadVector3(), reader.ReadVarInt(), senderConnection);
		}
	}

	protected void UserCode_RpcUpdateState__Vector3__Int32(Vector3 offset, int frame)
	{
		if (!base.isOwned && frame > lastReceiveFrame)
		{
			lastReceiveFrame = frame;
			targetOffset = offset;
		}
	}

	protected static void InvokeUserCode_RpcUpdateState__Vector3__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcUpdateState called on server.");
		}
		else
		{
			((LocalSpectatorCameraFollower)obj).UserCode_RpcUpdateState__Vector3__Int32(reader.ReadVector3(), reader.ReadVarInt());
		}
	}

	static LocalSpectatorCameraFollower()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(LocalSpectatorCameraFollower), "System.Void LocalSpectatorCameraFollower::CmdUpdateState(UnityEngine.Vector3,System.Int32,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdUpdateState__Vector3__Int32__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(LocalSpectatorCameraFollower), "System.Void LocalSpectatorCameraFollower::RpcUpdateState(UnityEngine.Vector3,System.Int32)", InvokeUserCode_RpcUpdateState__Vector3__Int32);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteTransform(target);
			writer.WriteNetworkBehaviour(Networkowner);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteTransform(target);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteNetworkBehaviour(Networkowner);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref target, _Mirror_SyncVarHookDelegate_target, reader.ReadTransform());
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, _Mirror_SyncVarHookDelegate_owner, reader, ref ___ownerNetId);
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref target, _Mirror_SyncVarHookDelegate_target, reader.ReadTransform());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize_NetworkBehaviour(ref owner, _Mirror_SyncVarHookDelegate_owner, reader, ref ___ownerNetId);
		}
	}
}
