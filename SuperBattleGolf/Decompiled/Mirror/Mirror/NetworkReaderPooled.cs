using System;

namespace Mirror;

public sealed class NetworkReaderPooled : NetworkReader, IDisposable
{
	internal NetworkReaderPooled(byte[] bytes)
		: base(bytes)
	{
	}

	internal NetworkReaderPooled(ArraySegment<byte> segment)
		: base(segment)
	{
	}

	public void Dispose()
	{
		NetworkReaderPool.Return(this);
	}
}
