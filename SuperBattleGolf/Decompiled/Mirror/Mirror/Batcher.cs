using System;
using System.Collections.Generic;

namespace Mirror;

public class Batcher
{
	private readonly int threshold;

	public const int TimestampSize = 8;

	private readonly Queue<NetworkWriterPooled> batches = new Queue<NetworkWriterPooled>();

	private NetworkWriterPooled batch;

	private double batchTimestamp;

	public static int MessageHeaderSize(int messageSize)
	{
		return Compression.VarUIntSize((ulong)messageSize);
	}

	public static int MaxMessageOverhead(int messageSize)
	{
		return 8 + MessageHeaderSize(messageSize);
	}

	public Batcher(int threshold)
	{
		this.threshold = threshold;
	}

	public void AddMessage(ArraySegment<byte> message, double timeStamp)
	{
		if (batch != null && batchTimestamp != timeStamp)
		{
			batches.Enqueue(batch);
			batch = null;
			batchTimestamp = 0.0;
		}
		int num = Compression.VarUIntSize((ulong)message.Count) + message.Count;
		if (batch != null && batch.Position + num > threshold)
		{
			batches.Enqueue(batch);
			batch = null;
			batchTimestamp = 0.0;
		}
		if (batch == null)
		{
			batch = NetworkWriterPool.Get();
			batch.WriteDouble(timeStamp);
			batchTimestamp = timeStamp;
		}
		Compression.CompressVarUInt(batch, (ulong)message.Count);
		batch.WriteBytes(message.Array, message.Offset, message.Count);
	}

	private static void CopyAndReturn(NetworkWriterPooled batch, NetworkWriter writer)
	{
		if (writer.Position != 0)
		{
			throw new ArgumentException("GetBatch needs a fresh writer!");
		}
		ArraySegment<byte> arraySegment = batch.ToArraySegment();
		writer.WriteBytes(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		NetworkWriterPool.Return(batch);
	}

	public bool GetBatch(NetworkWriter writer)
	{
		if (batches.TryDequeue(out var result))
		{
			CopyAndReturn(result, writer);
			return true;
		}
		if (batch != null)
		{
			CopyAndReturn(batch, writer);
			batch = null;
			return true;
		}
		return false;
	}

	public void Clear()
	{
		if (batch != null)
		{
			NetworkWriterPool.Return(batch);
			batch = null;
			batchTimestamp = 0.0;
		}
		foreach (NetworkWriterPooled batch in batches)
		{
			NetworkWriterPool.Return(batch);
		}
		batches.Clear();
	}
}
