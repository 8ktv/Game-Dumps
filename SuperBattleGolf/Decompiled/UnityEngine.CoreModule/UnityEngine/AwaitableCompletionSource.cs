using System;
using System.Threading;

namespace UnityEngine;

public class AwaitableCompletionSource
{
	private volatile int _state;

	public Awaitable Awaitable { get; private set; } = Awaitable.NewManagedAwaitable();

	public void SetResult()
	{
		if (!TrySetResult())
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	public void SetCanceled()
	{
		if (!TrySetCanceled())
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	public void SetException(Exception exception)
	{
		if (!TrySetException(exception))
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	private bool CheckAndAcquireCompletionState()
	{
		return Interlocked.CompareExchange(ref _state, 1, 0) == 0;
	}

	public bool TrySetResult()
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.RaiseManagedCompletion();
		return true;
	}

	public bool TrySetCanceled()
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.Cancel();
		return true;
	}

	public bool TrySetException(Exception exception)
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.RaiseManagedCompletion(exception);
		return true;
	}

	public void Reset()
	{
		Awaitable = Awaitable.NewManagedAwaitable();
		_state = 0;
	}
}
public class AwaitableCompletionSource<T>
{
	private volatile int _state;

	public Awaitable<T> Awaitable { get; private set; } = Awaitable<T>.GetManaged();

	public void SetResult(in T value)
	{
		if (!TrySetResult(in value))
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	public void SetCanceled()
	{
		if (!TrySetCanceled())
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	public void SetException(Exception exception)
	{
		if (!TrySetException(exception))
		{
			throw new InvalidOperationException("Can't raise completion of the same Awaitable twice");
		}
	}

	private bool CheckAndAcquireCompletionState()
	{
		return Interlocked.CompareExchange(ref _state, 1, 0) == 0;
	}

	public bool TrySetResult(in T value)
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.SetResultAndRaiseContinuation(value);
		return true;
	}

	public bool TrySetCanceled()
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.Cancel();
		return true;
	}

	public bool TrySetException(Exception exception)
	{
		if (!CheckAndAcquireCompletionState())
		{
			return false;
		}
		Awaitable.SetExceptionAndRaiseContinuation(exception);
		return true;
	}

	public void Reset()
	{
		Awaitable = Awaitable<T>.GetManaged();
		_state = 0;
	}
}
