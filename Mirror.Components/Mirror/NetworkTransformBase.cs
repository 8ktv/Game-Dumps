using System;
using System.Collections.Generic;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

public abstract class NetworkTransformBase : NetworkBehaviour
{
	internal TransformSnapshot? pendingSnapshot;

	[Header("Target")]
	[Tooltip("The Transform component to sync. May be on on this GameObject, or on a child.")]
	public Transform target;

	[Header("Base Settings")]
	[Tooltip("Select which Update method to use.\nSelect FixedUpdate for non-kinematic rigidbodies.")]
	public UpdateMethod updateMethod;

	public readonly SortedList<double, TransformSnapshot> clientSnapshots = new SortedList<double, TransformSnapshot>(16);

	public readonly SortedList<double, TransformSnapshot> serverSnapshots = new SortedList<double, TransformSnapshot>(16);

	[Header("Selective Sync\nDon't change these at Runtime")]
	public bool syncPosition = true;

	public bool syncRotation = true;

	public bool syncScale;

	[Header("Bandwidth Savings")]
	[Tooltip("When true, changes are not sent unless greater than sensitivity values below.")]
	public bool onlySyncOnChange = true;

	[Tooltip("Apply smallest-three quaternion compression. This is lossy, you can disable it if the small rotation inaccuracies are noticeable in your project.")]
	public bool compressRotation = true;

	[Header("Interpolation")]
	[Tooltip("Set to false to have a snap-like effect on position movement.")]
	public bool interpolatePosition = true;

	[Tooltip("Set to false to have a snap-like effect on rotations.")]
	public bool interpolateRotation = true;

	[Tooltip("Set to false to remove scale smoothing. Example use-case: Instant flipping of sprites that use -X and +X for direction.")]
	public bool interpolateScale = true;

	[Header("Coordinate Space")]
	[Tooltip("Local by default. World may be better when changing hierarchy, or non-NetworkTransforms root position/rotation/scale values.")]
	public CoordinateSpace coordinateSpace;

	[Header("Timeline Offset")]
	[Tooltip("Add a small timeline offset to account for decoupled arrival of NetworkTime and NetworkTransform snapshots.\nfixes: https://github.com/MirrorNetworking/Mirror/issues/3427")]
	public bool timelineOffset = true;

	[Header("Debug")]
	public bool showGizmos;

	public bool showOverlay;

	public Color overlayColor = new Color(0f, 0f, 0f, 0.5f);

	protected bool IsClientWithAuthority
	{
		get
		{
			if (base.isClient)
			{
				return base.authority;
			}
			return false;
		}
	}

	public uint sendIntervalMultiplier
	{
		get
		{
			if (syncInterval > 0f)
			{
				float num = syncInterval / NetworkServer.sendInterval;
				if (!(num > 1f))
				{
					return 1u;
				}
				return (uint)Mathf.RoundToInt(num);
			}
			return 1u;
		}
	}

	protected double timeStampAdjustment => NetworkServer.sendInterval * (float)(sendIntervalMultiplier - 1);

	protected double offset => timelineOffset ? (NetworkServer.sendInterval * (float)sendIntervalMultiplier) : 0f;

	public Vector3 velocity { get; private set; }

	public Vector3 angularVelocity { get; private set; }

	public event Action WillTeleport;

	public event Action Teleported;

	protected override void OnValidate()
	{
		if (!Application.isPlaying)
		{
			base.OnValidate();
			Configure();
		}
	}

	internal override bool showSyncMethod()
	{
		return false;
	}

	protected virtual void Configure()
	{
		if (target == null)
		{
			target = base.transform;
		}
		if (coordinateSpace == CoordinateSpace.World)
		{
			syncScale = false;
		}
	}

	protected virtual void Awake()
	{
		Configure();
	}

	protected virtual Vector3 GetPosition()
	{
		if (coordinateSpace != CoordinateSpace.Local)
		{
			return target.position;
		}
		return target.localPosition;
	}

	protected virtual Quaternion GetRotation()
	{
		if (coordinateSpace != CoordinateSpace.Local)
		{
			return target.rotation;
		}
		return target.localRotation;
	}

	protected virtual Vector3 GetScale()
	{
		if (coordinateSpace != CoordinateSpace.Local)
		{
			return target.lossyScale;
		}
		return target.localScale;
	}

	protected virtual void SetPosition(Vector3 position)
	{
		if (coordinateSpace == CoordinateSpace.Local)
		{
			target.localPosition = position;
		}
		else
		{
			target.position = position;
		}
	}

	protected virtual void SetRotation(Quaternion rotation)
	{
		if (coordinateSpace == CoordinateSpace.Local)
		{
			target.localRotation = rotation;
		}
		else
		{
			target.rotation = rotation;
		}
	}

	protected virtual void SetScale(Vector3 scale)
	{
		if (coordinateSpace == CoordinateSpace.Local)
		{
			target.localScale = scale;
		}
	}

	protected virtual TransformSnapshot Construct()
	{
		return new TransformSnapshot(NetworkTime.localTime, 0.0, GetPosition(), GetRotation(), GetScale());
	}

	protected void AddSnapshot(SortedList<double, TransformSnapshot> snapshots, double timeStamp, Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (!position.HasValue)
		{
			position = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position : GetPosition());
		}
		if (!rotation.HasValue)
		{
			rotation = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation : GetRotation());
		}
		if (!scale.HasValue)
		{
			scale = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].scale : GetScale());
		}
		SnapshotInterpolation.InsertIfNotExists(snapshots, NetworkClient.snapshotSettings.bufferLimit, new TransformSnapshot(timeStamp, NetworkTime.localTime, position.Value, rotation.Value, scale.Value));
	}

	protected virtual void Apply(TransformSnapshot interpolated, TransformSnapshot endGoal)
	{
		if (!base.isOwned && Time.deltaTime > 0f)
		{
			velocity = (interpolated.position - base.transform.localPosition) / Time.deltaTime;
			angularVelocity = (interpolated.rotation.eulerAngles - base.transform.localRotation.eulerAngles) / Time.deltaTime;
		}
		if (syncPosition)
		{
			SetPosition(interpolatePosition ? interpolated.position : endGoal.position);
		}
		if (syncRotation)
		{
			SetRotation(interpolateRotation ? interpolated.rotation : endGoal.rotation);
		}
		if (syncScale)
		{
			SetScale(interpolateScale ? interpolated.scale : endGoal.scale);
		}
	}

	[Command]
	public void CmdTeleport(Vector3 destination)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdTeleport__Vector3(destination);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		SendCommandInternal("System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3)", 2109034929, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[Command]
	public void CmdTeleport(Vector3 destination, Quaternion rotation)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdTeleport__Vector3__Quaternion(destination, rotation);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		writer.WriteQuaternion(rotation);
		SendCommandInternal("System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", 1176796404, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcTeleport(Vector3 destination)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3)", -1933368736, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	public void RpcTeleport(Vector3 destination, Quaternion rotation)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVector3(destination);
		writer.WriteQuaternion(rotation);
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", -1675599861, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[Server]
	public void ServerTeleport(Vector3 destination, Quaternion rotation)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void Mirror.NetworkTransformBase::ServerTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)' called when server was not active");
			return;
		}
		OnTeleport(destination, rotation);
		RpcTeleport(destination, rotation);
	}

	[ClientRpc]
	private void RpcResetState()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendRPCInternal("System.Void Mirror.NetworkTransformBase::RpcResetState()", 752432316, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	public void TeleportLocalOnly(Vector3 destination)
	{
		OnTeleport(destination);
	}

	public void TeleportLocalOnly(Vector3 destination, Quaternion rotation)
	{
		OnTeleport(destination, rotation);
	}

	protected virtual void OnTeleport(Vector3 destination)
	{
		BeforeTeleport();
		target.position = destination;
		ResetState();
		AfterTeleport();
	}

	protected virtual void OnTeleport(Vector3 destination, Quaternion rotation)
	{
		BeforeTeleport();
		target.position = destination;
		target.rotation = rotation;
		ResetState();
		AfterTeleport();
	}

	protected void BeforeTeleport()
	{
		this.WillTeleport?.Invoke();
	}

	protected void AfterTeleport()
	{
		this.Teleported?.Invoke();
	}

	public virtual void ResetState()
	{
		serverSnapshots.Clear();
		clientSnapshots.Clear();
		Physics.SyncTransforms();
	}

	public virtual void Reset()
	{
		ResetState();
		syncDirection = SyncDirection.ClientToServer;
		syncInterval = 0.05f;
	}

	protected virtual void OnEnable()
	{
		ResetState();
		if (NetworkServer.active)
		{
			NetworkIdentity.clientAuthorityCallback += OnClientAuthorityChanged;
		}
	}

	protected virtual void OnDisable()
	{
		ResetState();
		if (NetworkServer.active)
		{
			NetworkIdentity.clientAuthorityCallback -= OnClientAuthorityChanged;
		}
	}

	[ServerCallback]
	private void OnClientAuthorityChanged(NetworkConnectionToClient conn, NetworkIdentity identity, bool authorityState)
	{
		if (NetworkServer.active && !(identity != base.netIdentity) && syncDirection == SyncDirection.ClientToServer)
		{
			ResetState();
			RpcResetState();
		}
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdTeleport__Vector3(Vector3 destination)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			OnTeleport(destination);
			RpcTeleport(destination);
		}
	}

	protected static void InvokeUserCode_CmdTeleport__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTeleport called on client.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_CmdTeleport__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_CmdTeleport__Vector3__Quaternion(Vector3 destination, Quaternion rotation)
	{
		if (syncDirection == SyncDirection.ClientToServer)
		{
			OnTeleport(destination, rotation);
			RpcTeleport(destination, rotation);
		}
	}

	protected static void InvokeUserCode_CmdTeleport__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdTeleport called on client.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_CmdTeleport__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_RpcTeleport__Vector3(Vector3 destination)
	{
		OnTeleport(destination);
	}

	protected static void InvokeUserCode_RpcTeleport__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcTeleport called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcTeleport__Vector3(reader.ReadVector3());
		}
	}

	protected void UserCode_RpcTeleport__Vector3__Quaternion(Vector3 destination, Quaternion rotation)
	{
		OnTeleport(destination, rotation);
	}

	protected static void InvokeUserCode_RpcTeleport__Vector3__Quaternion(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcTeleport called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcTeleport__Vector3__Quaternion(reader.ReadVector3(), reader.ReadQuaternion());
		}
	}

	protected void UserCode_RpcResetState()
	{
		ResetState();
	}

	protected static void InvokeUserCode_RpcResetState(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcResetState called on server.");
		}
		else
		{
			((NetworkTransformBase)obj).UserCode_RpcResetState();
		}
	}

	static NetworkTransformBase()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3)", InvokeUserCode_CmdTeleport__Vector3, requiresAuthority: true);
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::CmdTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_CmdTeleport__Vector3__Quaternion, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3)", InvokeUserCode_RpcTeleport__Vector3);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcTeleport(UnityEngine.Vector3,UnityEngine.Quaternion)", InvokeUserCode_RpcTeleport__Vector3__Quaternion);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformBase), "System.Void Mirror.NetworkTransformBase::RpcResetState()", InvokeUserCode_RpcResetState);
	}
}
