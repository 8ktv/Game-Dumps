using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mirror;

public abstract class NetworkConnection
{
	public const int LocalConnectionId = 0;

	public bool isAuthenticated;

	public object authenticationData;

	public bool isReady;

	public float lastMessageTime;

	public readonly HashSet<NetworkIdentity> owned = new HashSet<NetworkIdentity>();

	protected Dictionary<int, Batcher> batches = new Dictionary<int, Batcher>();

	public NetworkIdentity identity { get; internal set; }

	public double remoteTimeStamp { get; internal set; }

	internal NetworkConnection()
	{
		lastMessageTime = Time.time;
	}

	protected Batcher GetBatchForChannelId(int channelId)
	{
		if (!batches.TryGetValue(channelId, out var value))
		{
			value = new Batcher(Transport.active.GetBatchThreshold(channelId));
			batches[channelId] = value;
		}
		return value;
	}

	public void Send<T>(T message, int channelId = 0) where T : struct, NetworkMessage
	{
		using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		NetworkMessages.Pack(message, networkWriterPooled);
		int num = NetworkMessages.MaxMessageSize(channelId);
		if (networkWriterPooled.Position > num)
		{
			Debug.LogError($"NetworkConnection.Send: message of type {typeof(T)} with a size of {networkWriterPooled.Position} bytes is larger than the max allowed message size in one batch: {num}.\nThe message was dropped, please make it smaller.");
			return;
		}
		NetworkDiagnostics.OnSend(message, channelId, networkWriterPooled.Position, 1);
		Send(networkWriterPooled.ToArraySegment(), channelId);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal virtual void Send(ArraySegment<byte> segment, int channelId = 0)
	{
		GetBatchForChannelId(channelId).AddMessage(segment, NetworkTime.localTime);
	}

	protected abstract void SendToTransport(ArraySegment<byte> segment, int channelId = 0);

	internal virtual void Update()
	{
		foreach (KeyValuePair<int, Batcher> batch in batches)
		{
			using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
			while (batch.Value.GetBatch(networkWriterPooled))
			{
				ArraySegment<byte> segment = networkWriterPooled.ToArraySegment();
				SendToTransport(segment, batch.Key);
				networkWriterPooled.Position = 0;
			}
		}
	}

	internal virtual bool IsAlive(float timeout)
	{
		return Time.time - lastMessageTime < timeout;
	}

	public abstract void Disconnect();

	public virtual void Cleanup()
	{
		foreach (Batcher value in batches.Values)
		{
			value.Clear();
		}
	}
}
