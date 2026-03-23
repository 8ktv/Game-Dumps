using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine;

public static class AsyncOperationAwaitableExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[ExcludeFromDocs]
	public static Awaitable.Awaiter GetAwaiter(this AsyncOperation op)
	{
		return Awaitable.FromAsyncOperation(op).GetAwaiter();
	}
}
