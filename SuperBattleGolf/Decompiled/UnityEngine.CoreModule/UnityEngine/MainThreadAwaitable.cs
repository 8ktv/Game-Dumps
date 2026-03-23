using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine.Internal;

namespace UnityEngine;

[ExcludeFromDocs]
public struct MainThreadAwaitable : INotifyCompletion
{
	private readonly SynchronizationContext _synchronizationContext;

	private readonly int _mainThreadId;

	public bool IsCompleted => Thread.CurrentThread.ManagedThreadId == _mainThreadId;

	internal MainThreadAwaitable(SynchronizationContext syncContext, int mainThreadId)
	{
		_synchronizationContext = syncContext;
		_mainThreadId = mainThreadId;
	}

	public MainThreadAwaitable GetAwaiter()
	{
		return this;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void GetResult()
	{
	}

	public void OnCompleted(Action continuation)
	{
		_synchronizationContext.Post(DoOnCompleted, continuation);
	}

	private static void DoOnCompleted(object continuation)
	{
		(continuation as Action)?.Invoke();
	}
}
