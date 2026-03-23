using System;
using System.Collections.Generic;

namespace Mirror;

public class LocalConnectionToClient : NetworkConnectionToClient
{
	internal LocalConnectionToServer connectionToServer;

	internal readonly Queue<NetworkWriterPooled> queue = new Queue<NetworkWriterPooled>();

	public LocalConnectionToClient()
		: base(0)
	{
	}

	internal override void Send(ArraySegment<byte> segment, int channelId = 0)
	{
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteBytes(segment.Array, segment.Offset, segment.Count);
		connectionToServer.queue.Enqueue(networkWriterPooled);
	}

	internal override bool IsAlive(float timeout)
	{
		return true;
	}

	protected override void UpdatePing()
	{
	}

	internal override void Update()
	{
		base.Update();
		while (queue.Count > 0)
		{
			NetworkWriterPooled networkWriterPooled = queue.Dequeue();
			ArraySegment<byte> message = networkWriterPooled.ToArraySegment();
			Batcher batchForChannelId = GetBatchForChannelId(0);
			batchForChannelId.AddMessage(message, NetworkTime.localTime);
			using (NetworkWriterPooled networkWriterPooled2 = NetworkWriterPool.Get())
			{
				if (batchForChannelId.GetBatch(networkWriterPooled2))
				{
					NetworkServer.OnTransportData(connectionId, networkWriterPooled2.ToArraySegment(), 0);
				}
			}
			NetworkWriterPool.Return(networkWriterPooled);
		}
	}

	internal void DisconnectInternal()
	{
		isReady = false;
		RemoveFromObservingsObservers();
	}

	public override void Disconnect()
	{
		DisconnectInternal();
		connectionToServer.DisconnectInternal();
	}
}
