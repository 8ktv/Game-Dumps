using System;
using System.Collections.Generic;

namespace Mirror;

public class Unbatcher
{
	private readonly Queue<NetworkWriterPooled> batches = new Queue<NetworkWriterPooled>();

	private readonly NetworkReader reader = new NetworkReader(new byte[0]);

	private double readerRemoteTimeStamp;

	public int BatchesCount => batches.Count;

	private void StartReadingBatch(NetworkWriterPooled batch)
	{
		reader.SetBuffer(batch.ToArraySegment());
		readerRemoteTimeStamp = reader.ReadDouble();
	}

	public bool AddBatch(ArraySegment<byte> batch)
	{
		if (batch.Count < 8)
		{
			return false;
		}
		NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
		networkWriterPooled.WriteBytes(batch.Array, batch.Offset, batch.Count);
		if (batches.Count == 0)
		{
			StartReadingBatch(networkWriterPooled);
		}
		batches.Enqueue(networkWriterPooled);
		return true;
	}

	public bool GetNextMessage(out ArraySegment<byte> message, out double remoteTimeStamp)
	{
		message = default(ArraySegment<byte>);
		remoteTimeStamp = 0.0;
		if (batches.Count == 0)
		{
			return false;
		}
		if (reader.Capacity == 0)
		{
			return false;
		}
		if (reader.Remaining == 0)
		{
			NetworkWriterPool.Return(batches.Dequeue());
			if (batches.Count <= 0)
			{
				return false;
			}
			NetworkWriterPooled batch = batches.Peek();
			StartReadingBatch(batch);
		}
		remoteTimeStamp = readerRemoteTimeStamp;
		if (reader.Remaining == 0)
		{
			return false;
		}
		int num = (int)Compression.DecompressVarUInt(reader);
		if (reader.Remaining < num)
		{
			return false;
		}
		message = reader.ReadBytesSegment(num);
		return true;
	}
}
