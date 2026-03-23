using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Internal;

namespace UnityEngine;

[ExcludeFromDocs]
public struct BackgroundThreadAwaitable : INotifyCompletion
{
	private readonly SynchronizationContext _synchronizationContext;

	private readonly int _mainThreadId;

	public bool IsCompleted => Thread.CurrentThread.ManagedThreadId != _mainThreadId;

	internal BackgroundThreadAwaitable(SynchronizationContext syncContext, int mainThreadId)
	{
		_synchronizationContext = syncContext;
		_mainThreadId = mainThreadId;
	}

	public BackgroundThreadAwaitable GetAwaiter()
	{
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetResult()
	{
	}

	public void OnCompleted(Action continuation)
	{
		Task.Run(continuation);
	}
}
