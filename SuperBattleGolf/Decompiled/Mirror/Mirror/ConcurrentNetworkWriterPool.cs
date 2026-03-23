using System.Runtime.CompilerServices;

namespace Mirror;

public static class ConcurrentNetworkWriterPool
{
	public const int InitialCapacity = 1000;

	private static readonly ConcurrentPool<ConcurrentNetworkWriterPooled> pool = new ConcurrentPool<ConcurrentNetworkWriterPooled>(() => new ConcurrentNetworkWriterPooled(), 1000);

	public static int Count => pool.Count;

	public static ConcurrentNetworkWriterPooled Get()
	{
		ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = pool.Get();
		concurrentNetworkWriterPooled.Position = 0;
		return concurrentNetworkWriterPooled;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Return(ConcurrentNetworkWriterPooled writer)
	{
		pool.Return(writer);
	}
}
