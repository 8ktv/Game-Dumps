using System;

namespace Mirror;

public sealed class NetworkWriterPooled : NetworkWriter, IDisposable
{
	public void Dispose()
	{
		NetworkWriterPool.Return(this);
	}
}
