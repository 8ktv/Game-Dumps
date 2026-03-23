using System;
using System.Collections.Generic;
using Mirror.RemoteCalls;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Transform (Unreliable)")]
public class NetworkTransformUnreliable : NetworkTransformBase
{
	private uint sendIntervalCounter;

	private double lastSendIntervalTime = double.MinValue;

	[Header("Additional Settings")]
	[Tooltip("How much time, as a multiple of send interval, has passed before clearing buffers.\nA larger buffer means more delay, but results in smoother movement.\nExample: 1 for faster responses minimal smoothing, 5 covers bad pings but has noticable delay, 3 is recommended for balanced results.")]
	public float bufferResetMultiplier = 3f;

	[Header("Sensitivity")]
	[Tooltip("Sensitivity of changes needed before an updated state is sent over the network")]
	public float positionSensitivity = 0.01f;

	public float rotationSensitivity = 0.01f;

	public float scaleSensitivity = 0.01f;

	protected TransformSnapshot lastSnapshot;

	protected Changed cachedChangedComparison;

	protected bool hasSentUnchangedPosition;

	protected override void Configure()
	{
		base.Configure();
		syncMethod = SyncMethod.Reliable;
	}

	private void Update()
	{
		if (updateMethod == UpdateMethod.Update)
		{
			DoUpdate();
		}
	}

	private void FixedUpdate()
	{
		if (updateMethod == UpdateMethod.FixedUpdate)
		{
			DoUpdate();
		}
		if (pendingSnapshot.HasValue)
		{
			Apply(pendingSnapshot.Value, pendingSnapshot.Value);
			pendingSnapshot = null;
		}
	}

	private void LateUpdate()
	{
		if (updateMethod == UpdateMethod.LateUpdate)
		{
			DoUpdate();
		}
		if (base.isServer)
		{
			UpdateServerBroadcast();
		}
		else if (base.isClient && base.IsClientWithAuthority)
		{
			UpdateClientBroadcast();
		}
	}

	private void DoUpdate()
	{
		if (base.isServer)
		{
			UpdateServerInterpolation();
		}
		else if (base.isClient && !base.IsClientWithAuthority)
		{
			UpdateClientInterpolation();
		}
	}

	protected virtual void CheckLastSendTime()
	{
		if (sendIntervalCounter >= base.sendIntervalMultiplier)
		{
			sendIntervalCounter = 0u;
		}
		if (AccurateInterval.Elapsed(NetworkTime.localTime, NetworkServer.sendInterval, ref lastSendIntervalTime))
		{
			sendIntervalCounter++;
		}
	}

	private void UpdateServerBroadcast()
	{
		CheckLastSendTime();
		if (sendIntervalCounter != base.sendIntervalMultiplier || (syncDirection != SyncDirection.ServerToClient && !base.IsClientWithAuthority))
		{
			return;
		}
		TransformSnapshot transformSnapshot = Construct();
		cachedChangedComparison = CompareChangedSnapshots(transformSnapshot);
		if ((cachedChangedComparison != Changed.None && cachedChangedComparison != Changed.CompressRot) || !hasSentUnchangedPosition || !onlySyncOnChange)
		{
			SyncData syncData = new SyncData(cachedChangedComparison, transformSnapshot);
			RpcServerToClientSync(syncData);
			if (cachedChangedComparison == Changed.None || cachedChangedComparison == Changed.CompressRot)
			{
				hasSentUnchangedPosition = true;
				return;
			}
			hasSentUnchangedPosition = false;
			UpdateLastSentSnapshot(cachedChangedComparison, transformSnapshot);
		}
	}

	private void UpdateServerInterpolation()
	{
		if (syncDirection == SyncDirection.ClientToServer && base.connectionToClient != null && !base.isOwned && serverSnapshots.Count != 0)
		{
			SnapshotInterpolation.StepInterpolation(serverSnapshots, base.connectionToClient.remoteTimeline, out var fromSnapshot, out var toSnapshot, out var t);
			TransformSnapshot transformSnapshot = TransformSnapshot.Interpolate(fromSnapshot, toSnapshot, t);
			if (updateMethod == UpdateMethod.FixedUpdate)
			{
				pendingSnapshot = transformSnapshot;
			}
			else
			{
				Apply(transformSnapshot, toSnapshot);
			}
		}
	}

	private void UpdateClientBroadcast()
	{
		if (!NetworkClient.ready)
		{
			return;
		}
		CheckLastSendTime();
		if (sendIntervalCounter != base.sendIntervalMultiplier)
		{
			return;
		}
		TransformSnapshot transformSnapshot = Construct();
		cachedChangedComparison = CompareChangedSnapshots(transformSnapshot);
		if ((cachedChangedComparison != Changed.None && cachedChangedComparison != Changed.CompressRot) || !hasSentUnchangedPosition || !onlySyncOnChange)
		{
			SyncData syncData = new SyncData(cachedChangedComparison, transformSnapshot);
			CmdClientToServerSync(syncData);
			if (cachedChangedComparison == Changed.None || cachedChangedComparison == Changed.CompressRot)
			{
				hasSentUnchangedPosition = true;
				return;
			}
			hasSentUnchangedPosition = false;
			UpdateLastSentSnapshot(cachedChangedComparison, transformSnapshot);
		}
	}

	private void UpdateClientInterpolation()
	{
		if (clientSnapshots.Count != 0)
		{
			SnapshotInterpolation.StepInterpolation(clientSnapshots, NetworkTime.time, out var fromSnapshot, out var toSnapshot, out var t);
			TransformSnapshot transformSnapshot = TransformSnapshot.Interpolate(fromSnapshot, toSnapshot, t);
			if (updateMethod == UpdateMethod.FixedUpdate)
			{
				pendingSnapshot = transformSnapshot;
			}
			else
			{
				Apply(transformSnapshot, toSnapshot);
			}
		}
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		if (initialState)
		{
			if (syncPosition)
			{
				writer.WriteVector3(GetPosition());
			}
			if (syncRotation)
			{
				writer.WriteQuaternion(GetRotation());
			}
			if (syncScale)
			{
				writer.WriteVector3(GetScale());
			}
		}
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			if (syncPosition)
			{
				SetPosition(reader.ReadVector3());
			}
			if (syncRotation)
			{
				SetRotation(reader.ReadQuaternion());
			}
			if (syncScale)
			{
				SetScale(reader.ReadVector3());
			}
		}
	}

	protected virtual void UpdateLastSentSnapshot(Changed change, TransformSnapshot currentSnapshot)
	{
		if (change == Changed.None || change == Changed.CompressRot)
		{
			return;
		}
		if ((int)(change & Changed.PosX) > 0)
		{
			lastSnapshot.position.x = currentSnapshot.position.x;
		}
		if ((int)(change & Changed.PosY) > 0)
		{
			lastSnapshot.position.y = currentSnapshot.position.y;
		}
		if ((int)(change & Changed.PosZ) > 0)
		{
			lastSnapshot.position.z = currentSnapshot.position.z;
		}
		if (compressRotation)
		{
			if ((int)(change & Changed.Rot) > 0)
			{
				lastSnapshot.rotation = currentSnapshot.rotation;
			}
		}
		else
		{
			Vector3 euler = default(Vector3);
			euler.x = (((int)(change & Changed.RotX) > 0) ? currentSnapshot.rotation.eulerAngles.x : lastSnapshot.rotation.eulerAngles.x);
			euler.y = (((int)(change & Changed.RotY) > 0) ? currentSnapshot.rotation.eulerAngles.y : lastSnapshot.rotation.eulerAngles.y);
			euler.z = (((int)(change & Changed.RotZ) > 0) ? currentSnapshot.rotation.eulerAngles.z : lastSnapshot.rotation.eulerAngles.z);
			lastSnapshot.rotation = Quaternion.Euler(euler);
		}
		if ((int)(change & Changed.Scale) > 0)
		{
			lastSnapshot.scale = currentSnapshot.scale;
		}
	}

	protected virtual Changed CompareChangedSnapshots(TransformSnapshot currentSnapshot)
	{
		Changed changed = Changed.None;
		if (syncPosition && Vector3.SqrMagnitude(lastSnapshot.position - currentSnapshot.position) > positionSensitivity * positionSensitivity)
		{
			if (Mathf.Abs(lastSnapshot.position.x - currentSnapshot.position.x) > positionSensitivity)
			{
				changed |= Changed.PosX;
			}
			if (Mathf.Abs(lastSnapshot.position.y - currentSnapshot.position.y) > positionSensitivity)
			{
				changed |= Changed.PosY;
			}
			if (Mathf.Abs(lastSnapshot.position.z - currentSnapshot.position.z) > positionSensitivity)
			{
				changed |= Changed.PosZ;
			}
		}
		if (syncRotation)
		{
			if (compressRotation)
			{
				if (Quaternion.Angle(lastSnapshot.rotation, currentSnapshot.rotation) > rotationSensitivity)
				{
					changed |= Changed.CompressRot;
					changed |= Changed.Rot;
				}
				else
				{
					changed |= Changed.CompressRot;
				}
			}
			else
			{
				if (Mathf.Abs(lastSnapshot.rotation.eulerAngles.x - currentSnapshot.rotation.eulerAngles.x) > rotationSensitivity)
				{
					changed |= Changed.RotX;
				}
				if (Mathf.Abs(lastSnapshot.rotation.eulerAngles.y - currentSnapshot.rotation.eulerAngles.y) > rotationSensitivity)
				{
					changed |= Changed.RotY;
				}
				if (Mathf.Abs(lastSnapshot.rotation.eulerAngles.z - currentSnapshot.rotation.eulerAngles.z) > rotationSensitivity)
				{
					changed |= Changed.RotZ;
				}
			}
		}
		if (syncScale && Vector3.SqrMagnitude(lastSnapshot.scale - currentSnapshot.scale) > scaleSensitivity * scaleSensitivity)
		{
			changed |= Changed.Scale;
		}
		return changed;
	}

	[Command(channel = 1)]
	private void CmdClientToServerSync(SyncData syncData)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdClientToServerSync__SyncData(syncData);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteSyncData(syncData);
		SendCommandInternal("System.Void Mirror.NetworkTransformUnreliable::CmdClientToServerSync(Mirror.SyncData)", -2082940711, writer, 1);
		NetworkWriterPool.Return(writer);
	}

	protected virtual void OnClientToServerSync(SyncData syncData)
	{
		if (syncDirection != SyncDirection.ClientToServer || serverSnapshots.Count >= base.connectionToClient.snapshotBufferSizeLimit)
		{
			return;
		}
		double remoteTimeStamp = base.connectionToClient.remoteTimeStamp;
		if (onlySyncOnChange)
		{
			double num = bufferResetMultiplier * (float)base.sendIntervalMultiplier * NetworkClient.sendInterval;
			if (serverSnapshots.Count > 0 && serverSnapshots.Values[serverSnapshots.Count - 1].remoteTime + num < remoteTimeStamp)
			{
				ResetState();
			}
		}
		UpdateSyncData(ref syncData, serverSnapshots);
		AddSnapshot(serverSnapshots, base.connectionToClient.remoteTimeStamp + base.timeStampAdjustment + base.offset, syncData.position, syncData.quatRotation, syncData.scale);
	}

	[ClientRpc(channel = 1)]
	private void RpcServerToClientSync(SyncData syncData)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteSyncData(syncData);
		SendRPCInternal("System.Void Mirror.NetworkTransformUnreliable::RpcServerToClientSync(Mirror.SyncData)", -1891602648, writer, 1, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	protected virtual void OnServerToClientSync(SyncData syncData)
	{
		if (base.isServer || base.IsClientWithAuthority)
		{
			return;
		}
		double remoteTimeStamp = NetworkClient.connection.remoteTimeStamp;
		if (onlySyncOnChange)
		{
			double num = bufferResetMultiplier * (float)base.sendIntervalMultiplier * NetworkServer.sendInterval;
			if (clientSnapshots.Count > 0 && clientSnapshots.Values[clientSnapshots.Count - 1].remoteTime + num < remoteTimeStamp)
			{
				ResetState();
			}
		}
		UpdateSyncData(ref syncData, clientSnapshots);
		AddSnapshot(clientSnapshots, NetworkClient.connection.remoteTimeStamp + base.timeStampAdjustment + base.offset, syncData.position, syncData.quatRotation, syncData.scale);
	}

	protected virtual void UpdateSyncData(ref SyncData syncData, SortedList<double, TransformSnapshot> snapshots)
	{
		if (syncData.changedDataByte == Changed.None || syncData.changedDataByte == Changed.CompressRot)
		{
			syncData.position = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position : GetPosition());
			syncData.quatRotation = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation : GetRotation());
			syncData.scale = ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].scale : GetScale());
			return;
		}
		syncData.position.x = (((int)(syncData.changedDataByte & Changed.PosX) > 0) ? syncData.position.x : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position.x : GetPosition().x));
		syncData.position.y = (((int)(syncData.changedDataByte & Changed.PosY) > 0) ? syncData.position.y : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position.y : GetPosition().y));
		syncData.position.z = (((int)(syncData.changedDataByte & Changed.PosZ) > 0) ? syncData.position.z : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].position.z : GetPosition().z));
		if ((syncData.changedDataByte & Changed.CompressRot) == 0)
		{
			syncData.vecRotation.x = (((int)(syncData.changedDataByte & Changed.RotX) > 0) ? syncData.vecRotation.x : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation.eulerAngles.x : GetRotation().eulerAngles.x));
			syncData.vecRotation.y = (((int)(syncData.changedDataByte & Changed.RotY) > 0) ? syncData.vecRotation.y : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation.eulerAngles.y : GetRotation().eulerAngles.y));
			syncData.vecRotation.z = (((int)(syncData.changedDataByte & Changed.RotZ) > 0) ? syncData.vecRotation.z : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation.eulerAngles.z : GetRotation().eulerAngles.z));
			syncData.quatRotation = Quaternion.Euler(syncData.vecRotation);
		}
		else
		{
			syncData.quatRotation = (((int)(syncData.changedDataByte & Changed.Rot) > 0) ? syncData.quatRotation : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].rotation : GetRotation()));
		}
		syncData.scale = (((int)(syncData.changedDataByte & Changed.Scale) > 0) ? syncData.scale : ((snapshots.Count > 0) ? snapshots.Values[snapshots.Count - 1].scale : GetScale()));
	}

	protected virtual void DeconstructSyncData(ArraySegment<byte> receivedPayload, out byte? changedFlagData, out Vector3? position, out Quaternion? rotation, out Vector3? scale)
	{
		using NetworkReaderPooled networkReaderPooled = NetworkReaderPool.Get(receivedPayload);
		SyncData syncData = networkReaderPooled.Read<SyncData>();
		changedFlagData = (byte)syncData.changedDataByte;
		position = syncData.position;
		rotation = syncData.quatRotation;
		scale = syncData.scale;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdClientToServerSync__SyncData(SyncData syncData)
	{
		OnClientToServerSync(syncData);
		if (syncDirection == SyncDirection.ClientToServer)
		{
			RpcServerToClientSync(syncData);
		}
	}

	protected static void InvokeUserCode_CmdClientToServerSync__SyncData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdClientToServerSync called on client.");
		}
		else
		{
			((NetworkTransformUnreliable)obj).UserCode_CmdClientToServerSync__SyncData(reader.ReadSyncData());
		}
	}

	protected void UserCode_RpcServerToClientSync__SyncData(SyncData syncData)
	{
		OnServerToClientSync(syncData);
	}

	protected static void InvokeUserCode_RpcServerToClientSync__SyncData(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcServerToClientSync called on server.");
		}
		else
		{
			((NetworkTransformUnreliable)obj).UserCode_RpcServerToClientSync__SyncData(reader.ReadSyncData());
		}
	}

	static NetworkTransformUnreliable()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NetworkTransformUnreliable), "System.Void Mirror.NetworkTransformUnreliable::CmdClientToServerSync(Mirror.SyncData)", InvokeUserCode_CmdClientToServerSync__SyncData, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(NetworkTransformUnreliable), "System.Void Mirror.NetworkTransformUnreliable::RpcServerToClientSync(Mirror.SyncData)", InvokeUserCode_RpcServerToClientSync__SyncData);
	}
}
