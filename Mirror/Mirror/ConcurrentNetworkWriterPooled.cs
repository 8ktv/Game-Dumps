using System;

namespace Mirror;

public sealed class ConcurrentNetworkWriterPooled : NetworkWriter, IDisposable
{
	public void Dispose()
	{
		ConcurrentNetworkWriterPool.Return(this);
	}
}
