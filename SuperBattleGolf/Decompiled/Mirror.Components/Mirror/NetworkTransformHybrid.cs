using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/Network Transform (Hybrid)")]
public class NetworkTransformHybrid : NetworkTransformBase
{
	[Header("Additional Settings")]
	[Tooltip("If we only sync on change, then we need to correct old snapshots if more time than sendInterval * multiplier has elapsed.\n\nOtherwise the first move will always start interpolating from the last move sequence's time, which will make it stutter when starting every time.")]
	public float onlySyncOnChangeCorrectionMultiplier = 2f;

	[Header("Rotation")]
	[Tooltip("Sensitivity of changes needed before an updated state is sent over the network")]
	public float rotationSensitivity = 0.01f;

	[Header("Precision")]
	[Tooltip("Position is rounded in order to drastically minimize bandwidth.\n\nFor example, a precision of 0.01 rounds to a centimeter. In other words, sub-centimeter movements aren't synced until they eventually exceeded an actual centimeter.\n\nDepending on how important the object is, a precision of 0.01-0.10 (1-10 cm) is recommended.\n\nFor example, even a 1cm precision combined with delta compression cuts the Benchmark demo's bandwidth in half, compared to sending every tiny change.")]
	[Range(0.0001f, 1f)]
	public float positionPrecision = 0.01f;

	[Range(0.0001f, 1f)]
	public float rotationPrecision = 0.001f;

	[Range(0.0001f, 1f)]
	public float scalePrecision = 0.01f;

	[Header("Debug")]
	public bool debugDraw;

	protected Vector3Long lastSerializedPosition = Vector3Long.zero;

	protected Vector3Long lastDeserializedPosition = Vector3Long.zero;

	protected Vector4Long lastSerializedRotation = Vector4Long.zero;

	protected Vector4Long lastDeserializedRotation = Vector4Long.zero;

	protected Vector3Long lastSerializedScale = Vector3Long.zero;

	protected Vector3Long lastDeserializedScale = Vector3Long.zero;

	protected TransformSnapshot last;

	protected override void Configure()
	{
		base.Configure();
		syncMethod = SyncMethod.Hybrid;
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
		if (pendingSnapshot.HasValue && !base.IsClientWithAuthority)
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
		if ((base.isServer || (base.IsClientWithAuthority && NetworkClient.ready)) && (!onlySyncOnChange || Changed(Construct())))
		{
			SetDirty();
		}
	}

	private void DoUpdate()
	{
		if (base.isServer)
		{
			UpdateServer();
		}
		else if (base.isClient)
		{
			UpdateClient();
		}
	}

	protected virtual void UpdateServer()
	{
		if (syncDirection == SyncDirection.ClientToServer && base.connectionToClient != null && !base.isOwned && serverSnapshots.Count > 0)
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

	protected virtual void UpdateClient()
	{
		if (!base.IsClientWithAuthority && clientSnapshots.Count > 0)
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
			if (debugDraw)
			{
				Debug.DrawLine(fromSnapshot.position, toSnapshot.position, Color.white, 10f);
				Debug.DrawLine(transformSnapshot.position, transformSnapshot.position + Vector3.up, Color.white, 10f);
			}
		}
	}

	protected virtual bool Changed(TransformSnapshot current)
	{
		if (!QuantizedChanged(last.position, current.position, positionPrecision) && !(Quaternion.Angle(last.rotation, current.rotation) > rotationSensitivity))
		{
			return QuantizedChanged(last.scale, current.scale, scalePrecision);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected bool QuantizedChanged(Vector3 u, Vector3 v, float precision)
	{
		Compression.ScaleToLong(u, precision, out var quantized);
		Compression.ScaleToLong(v, precision, out var quantized2);
		return quantized != quantized2;
	}

	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		TransformSnapshot transformSnapshot = Construct();
		if (initialState)
		{
			_ = writer.Position;
			if (syncPosition)
			{
				writer.WriteVector3(transformSnapshot.position);
			}
			if (syncRotation)
			{
				writer.WriteQuaternion(transformSnapshot.rotation);
			}
			if (syncScale)
			{
				writer.WriteVector3(transformSnapshot.scale);
			}
			if (syncPosition)
			{
				Compression.ScaleToLong(transformSnapshot.position, positionPrecision, out lastSerializedPosition);
			}
			if (syncRotation && !compressRotation)
			{
				Compression.ScaleToLong(transformSnapshot.rotation, rotationPrecision, out lastSerializedRotation);
			}
			if (syncScale)
			{
				Compression.ScaleToLong(transformSnapshot.scale, scalePrecision, out lastSerializedScale);
			}
			last = transformSnapshot;
			return;
		}
		_ = writer.Position;
		if (syncPosition)
		{
			Compression.ScaleToLong(transformSnapshot.position, positionPrecision, out var quantized);
			DeltaCompression.Compress(writer, lastSerializedPosition, quantized);
		}
		if (syncRotation)
		{
			if (compressRotation)
			{
				writer.WriteUInt(Compression.CompressQuaternion(transformSnapshot.rotation));
			}
			else
			{
				Compression.ScaleToLong(transformSnapshot.rotation, rotationPrecision, out var quantized2);
				DeltaCompression.Compress(writer, lastSerializedRotation, quantized2);
			}
		}
		if (syncScale)
		{
			Compression.ScaleToLong(transformSnapshot.scale, scalePrecision, out var quantized3);
			DeltaCompression.Compress(writer, lastSerializedScale, quantized3);
		}
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		Vector3? position = null;
		Quaternion? rotation = null;
		Vector3? scale = null;
		if (initialState)
		{
			if (syncPosition)
			{
				position = reader.ReadVector3();
				if (debugDraw)
				{
					Debug.DrawLine(position.Value, position.Value + Vector3.up, Color.green, 10f);
				}
			}
			if (syncRotation)
			{
				rotation = reader.ReadQuaternion();
			}
			if (syncScale)
			{
				scale = reader.ReadVector3();
			}
			if (syncPosition)
			{
				Compression.ScaleToLong(position.Value, positionPrecision, out lastDeserializedPosition);
			}
			if (syncRotation && !compressRotation)
			{
				Compression.ScaleToLong(rotation.Value, rotationPrecision, out lastDeserializedRotation);
			}
			if (syncScale)
			{
				Compression.ScaleToLong(scale.Value, scalePrecision, out lastDeserializedScale);
			}
			if (base.isServer)
			{
				OnClientToServerSync(position, rotation, scale);
			}
			else if (base.isClient)
			{
				OnServerToClientSync(position, rotation, scale);
			}
			return;
		}
		if (syncPosition)
		{
			Vector3Long value = DeltaCompression.Decompress(reader, lastDeserializedPosition);
			position = Compression.ScaleToFloat(value, positionPrecision);
			if (debugDraw)
			{
				Debug.DrawLine(position.Value, position.Value + Vector3.up, Color.yellow, 10f);
			}
		}
		if (syncRotation)
		{
			if (compressRotation)
			{
				rotation = Compression.DecompressQuaternion(reader.ReadUInt());
			}
			else
			{
				Vector4Long value2 = DeltaCompression.Decompress(reader, lastDeserializedRotation);
				rotation = Compression.ScaleToFloat(value2, rotationPrecision);
			}
		}
		if (syncScale)
		{
			Vector3Long value3 = DeltaCompression.Decompress(reader, lastDeserializedScale);
			scale = Compression.ScaleToFloat(value3, scalePrecision);
		}
		if (base.isServer)
		{
			OnClientToServerSync(position, rotation, scale);
		}
		else if (base.isClient)
		{
			OnServerToClientSync(position, rotation, scale);
		}
	}

	protected virtual void OnClientToServerSync(Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (syncDirection == SyncDirection.ClientToServer && serverSnapshots.Count < base.connectionToClient.snapshotBufferSizeLimit)
		{
			if (onlySyncOnChange && NeedsCorrection(serverSnapshots, base.connectionToClient.remoteTimeStamp, NetworkServer.sendInterval * (float)base.sendIntervalMultiplier, onlySyncOnChangeCorrectionMultiplier))
			{
				RewriteHistory(serverSnapshots, base.connectionToClient.remoteTimeStamp, NetworkTime.localTime, NetworkServer.sendInterval * (float)base.sendIntervalMultiplier, GetPosition(), GetRotation(), GetScale());
			}
			AddSnapshot(serverSnapshots, base.connectionToClient.remoteTimeStamp + base.timeStampAdjustment + base.offset, position, rotation, scale);
		}
	}

	protected virtual void OnServerToClientSync(Vector3? position, Quaternion? rotation, Vector3? scale)
	{
		if (!base.IsClientWithAuthority)
		{
			if (onlySyncOnChange && NeedsCorrection(clientSnapshots, NetworkClient.connection.remoteTimeStamp, NetworkClient.sendInterval * (float)base.sendIntervalMultiplier, onlySyncOnChangeCorrectionMultiplier))
			{
				RewriteHistory(clientSnapshots, NetworkClient.connection.remoteTimeStamp, NetworkTime.localTime, NetworkClient.sendInterval * (float)base.sendIntervalMultiplier, GetPosition(), GetRotation(), GetScale());
			}
			AddSnapshot(clientSnapshots, NetworkClient.connection.remoteTimeStamp + base.timeStampAdjustment + base.offset, position, rotation, scale);
		}
	}

	private static bool NeedsCorrection(SortedList<double, TransformSnapshot> snapshots, double remoteTimestamp, double bufferTime, double toleranceMultiplier)
	{
		if (snapshots.Count == 1)
		{
			return remoteTimestamp - snapshots.Keys[0] >= bufferTime * toleranceMultiplier;
		}
		return false;
	}

	private static void RewriteHistory(SortedList<double, TransformSnapshot> snapshots, double remoteTimeStamp, double localTime, double sendInterval, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		snapshots.Clear();
		SnapshotInterpolation.InsertIfNotExists(snapshots, NetworkClient.snapshotSettings.bufferLimit, new TransformSnapshot(remoteTimeStamp - sendInterval, localTime - sendInterval, position, rotation, scale));
	}

	protected override void OnTeleport(Vector3 destination)
	{
		target.position = destination;
		base.ResetState();
	}

	public override void ResetState()
	{
		base.ResetState();
		lastSerializedPosition = Vector3Long.zero;
		lastDeserializedPosition = Vector3Long.zero;
		lastSerializedRotation = Vector4Long.zero;
		lastDeserializedRotation = Vector4Long.zero;
		lastSerializedScale = Vector3Long.zero;
		lastDeserializedScale = Vector3Long.zero;
		last = new TransformSnapshot(0.0, 0.0, Vector3.zero, Quaternion.identity, Vector3.zero);
	}

	public override bool Weaved()
	{
		return true;
	}
}
