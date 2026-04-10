using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Mono/AsyncOperationAwaitable.h")]
[NativeHeader("Runtime/Mono/Awaitable.h")]
[AsyncMethodBuilder(typeof(AwaitableAsyncMethodBuilder))]
[NativeHeader("Runtime/Mono/DelayedCallAwaitable.h")]
public class Awaitable : IEnumerator
{
	internal enum AwaiterCompletionThreadAffinity
	{
		None,
		MainThread,
		BackgroundThread
	}

	[ExcludeFromDocs]
	public struct AwaitableAsyncMethodBuilder
	{
		private interface IStateMachineBox : IDisposable
		{
			Awaitable ResultingCoroutine { get; set; }

			Action MoveNext { get; }
		}

		private class StateMachineBox<TStateMachine> : IStateMachineBox, IDisposable where TStateMachine : IAsyncStateMachine
		{
			private static readonly ThreadLocal<ObjectPool<StateMachineBox<TStateMachine>>> _pool = new ThreadLocal<ObjectPool<StateMachineBox<TStateMachine>>>(() => new ObjectPool<StateMachineBox<TStateMachine>>(() => new StateMachineBox<TStateMachine>(), null, null, null, collectionCheck: false));

			public TStateMachine StateMachine { get; set; }

			public Action MoveNext { get; }

			public Awaitable ResultingCoroutine { get; set; }

			public static StateMachineBox<TStateMachine> GetOne()
			{
				return _pool.Value.Get();
			}

			public void Dispose()
			{
				StateMachine = default(TStateMachine);
				ResultingCoroutine = null;
				_pool.Value.Release(this);
			}

			private void DoMoveNext()
			{
				StateMachine.MoveNext();
			}

			public StateMachineBox()
			{
				MoveNext = DoMoveNext;
			}
		}

		private IStateMachineBox _stateMachineBox;

		private Awaitable _resultingCoroutine;

		public Awaitable Task
		{
			get
			{
				if (_resultingCoroutine != null)
				{
					return _resultingCoroutine;
				}
				if (_stateMachineBox != null)
				{
					IStateMachineBox stateMachineBox = _stateMachineBox;
					Awaitable obj = stateMachineBox.ResultingCoroutine ?? (stateMachineBox.ResultingCoroutine = NewManagedAwaitable());
					Awaitable result = obj;
					_resultingCoroutine = obj;
					return result;
				}
				return _resultingCoroutine = NewManagedAwaitable();
			}
		}

		private IStateMachineBox EnsureStateMachineBox<TStateMachine>() where TStateMachine : IAsyncStateMachine
		{
			if (_stateMachineBox != null)
			{
				return _stateMachineBox;
			}
			_stateMachineBox = StateMachineBox<TStateMachine>.GetOne();
			_stateMachineBox.ResultingCoroutine = _resultingCoroutine;
			return _stateMachineBox;
		}

		public static AwaitableAsyncMethodBuilder Create()
		{
			return default(AwaitableAsyncMethodBuilder);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			StateMachineBox<TStateMachine> stateMachineBox2 = (StateMachineBox<TStateMachine>)stateMachineBox;
			Task.CompletionThreadAffinity = ((Thread.CurrentThread.ManagedThreadId == _mainThreadId) ? AwaiterCompletionThreadAffinity.MainThread : AwaiterCompletionThreadAffinity.BackgroundThread);
			stateMachineBox2.StateMachine = stateMachine;
			stateMachineBox2.MoveNext();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			StateMachineBox<TStateMachine> stateMachineBox2 = (StateMachineBox<TStateMachine>)stateMachineBox;
			stateMachineBox2.StateMachine = stateMachine;
			Action moveNext = stateMachineBox.MoveNext;
			awaiter.OnCompleted(moveNext);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			StateMachineBox<TStateMachine> stateMachineBox2 = (StateMachineBox<TStateMachine>)stateMachineBox;
			stateMachineBox2.StateMachine = stateMachine;
			Action moveNext = stateMachineBox.MoveNext;
			awaiter.UnsafeOnCompleted(moveNext);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetException(Exception e)
		{
			Task.RaiseManagedCompletion(e);
			_stateMachineBox.Dispose();
			_stateMachineBox = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetResult()
		{
			Task.RaiseManagedCompletion();
			_stateMachineBox.Dispose();
			_stateMachineBox = null;
		}
	}

	[ExcludeFromDocs]
	public struct AwaitableAsyncMethodBuilder<T>
	{
		private interface IStateMachineBox : IDisposable
		{
			Awaitable<T> ResultingCoroutine { get; set; }

			Action MoveNext { get; }
		}

		private class StateMachineBox<TStateMachine> : IStateMachineBox, IDisposable where TStateMachine : IAsyncStateMachine
		{
			private static readonly ThreadLocal<ObjectPool<StateMachineBox<TStateMachine>>> _pool = new ThreadLocal<ObjectPool<StateMachineBox<TStateMachine>>>(() => new ObjectPool<StateMachineBox<TStateMachine>>(() => new StateMachineBox<TStateMachine>(), null, null, null, collectionCheck: false));

			public TStateMachine StateMachine { get; set; }

			public Action MoveNext { get; }

			public Awaitable<T> ResultingCoroutine { get; set; }

			public static StateMachineBox<TStateMachine> GetOne()
			{
				return _pool.Value.Get();
			}

			public void Dispose()
			{
				StateMachine = default(TStateMachine);
				ResultingCoroutine = null;
				_pool.Value.Release(this);
			}

			private void DoMoveNext()
			{
				StateMachine.MoveNext();
			}

			public StateMachineBox()
			{
				MoveNext = DoMoveNext;
			}
		}

		private IStateMachineBox _stateMachineBox;

		private Awaitable<T> _resultingCoroutine;

		public Awaitable<T> Task
		{
			get
			{
				if (_resultingCoroutine != null)
				{
					return _resultingCoroutine;
				}
				if (_stateMachineBox != null)
				{
					IStateMachineBox stateMachineBox = _stateMachineBox;
					Awaitable<T> obj = stateMachineBox.ResultingCoroutine ?? (stateMachineBox.ResultingCoroutine = Awaitable<T>.GetManaged());
					Awaitable<T> result = obj;
					_resultingCoroutine = obj;
					return result;
				}
				return _resultingCoroutine = Awaitable<T>.GetManaged();
			}
		}

		private IStateMachineBox EnsureStateMachineBox<TStateMachine>() where TStateMachine : IAsyncStateMachine
		{
			if (_stateMachineBox != null)
			{
				return _stateMachineBox;
			}
			_stateMachineBox = StateMachineBox<TStateMachine>.GetOne();
			_stateMachineBox.ResultingCoroutine = _resultingCoroutine;
			return _stateMachineBox;
		}

		public static AwaitableAsyncMethodBuilder<T> Create()
		{
			return default(AwaitableAsyncMethodBuilder<T>);
		}

		public void SetResult(T value)
		{
			Task.SetResultAndRaiseContinuation(value);
			_stateMachineBox.Dispose();
			_stateMachineBox = null;
		}

		public void SetException(Exception e)
		{
			Task.SetExceptionAndRaiseContinuation(e);
			_stateMachineBox.Dispose();
			_stateMachineBox = null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			StateMachineBox<TStateMachine> stateMachineBox2 = (StateMachineBox<TStateMachine>)stateMachineBox;
			Task.CompletionThreadAffinity = ((Thread.CurrentThread.ManagedThreadId == _mainThreadId) ? AwaiterCompletionThreadAffinity.MainThread : AwaiterCompletionThreadAffinity.BackgroundThread);
			stateMachineBox2.StateMachine = stateMachine;
			stateMachineBox2.MoveNext();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			StateMachineBox<TStateMachine> stateMachineBox2 = (StateMachineBox<TStateMachine>)stateMachineBox;
			stateMachineBox2.StateMachine = stateMachine;
			Action moveNext = stateMachineBox.MoveNext;
			awaiter.OnCompleted(moveNext);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
		{
			IStateMachineBox stateMachineBox = EnsureStateMachineBox<TStateMachine>();
			((StateMachineBox<TStateMachine>)stateMachineBox).StateMachine = stateMachine;
			Action moveNext = stateMachineBox.MoveNext;
			awaiter.UnsafeOnCompleted(moveNext);
		}
	}

	[ExcludeFromDocs]
	public struct Awaiter : INotifyCompletion
	{
		private readonly Awaitable _awaited;

		public bool IsCompleted => _awaited.IsCompleted;

		internal Awaiter(Awaitable awaited)
		{
			_awaited = awaited;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnCompleted(Action continuation)
		{
			_awaited.SetContinuation(continuation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetResult()
		{
			_awaited.PropagateExceptionAndRelease();
		}
	}

	private readonly struct AwaitableHandle
	{
		private readonly IntPtr _handle;

		public static AwaitableHandle ManagedHandle = new AwaitableHandle(new IntPtr(-1));

		public static AwaitableHandle NullHandle = new AwaitableHandle(IntPtr.Zero);

		public bool IsNull => _handle == IntPtr.Zero;

		public bool IsManaged => _handle == ManagedHandle._handle;

		public AwaitableHandle(IntPtr handle)
		{
			_handle = handle;
		}

		public static implicit operator IntPtr(AwaitableHandle handle)
		{
			return handle._handle;
		}

		public static implicit operator AwaitableHandle(IntPtr handle)
		{
			return new AwaitableHandle(handle);
		}
	}

	private struct AwaitableAndFrameIndex
	{
		public Awaitable Awaitable { get; }

		public int FrameIndex { get; }

		public AwaitableAndFrameIndex(Awaitable awaitable, int frameIndex)
		{
			Awaitable = awaitable;
			FrameIndex = frameIndex;
		}
	}

	private class DoubleBufferedAwaitableList
	{
		private List<AwaitableAndFrameIndex> _awaitables = new List<AwaitableAndFrameIndex>();

		private List<AwaitableAndFrameIndex> _scratch = new List<AwaitableAndFrameIndex>();

		public void SwapAndComplete()
		{
			List<AwaitableAndFrameIndex> scratch = _scratch;
			List<AwaitableAndFrameIndex> awaitables = _awaitables;
			_awaitables = scratch;
			_scratch = awaitables;
			try
			{
				foreach (AwaitableAndFrameIndex item in awaitables)
				{
					if (!item.Awaitable.IsDettachedOrCompleted)
					{
						if (Time.frameCount >= item.FrameIndex || item.FrameIndex == -1)
						{
							item.Awaitable.RaiseManagedCompletion();
						}
						else
						{
							scratch.Add(item);
						}
					}
				}
			}
			finally
			{
				awaitables.Clear();
			}
		}

		public void Add(Awaitable item, int frameIndex)
		{
			_awaitables.Add(new AwaitableAndFrameIndex(item, frameIndex));
		}

		public void Remove(Awaitable item)
		{
			_awaitables.RemoveAll((AwaitableAndFrameIndex x) => x.Awaitable == item);
		}

		public void Clear()
		{
			_awaitables.Clear();
		}
	}

	private SpinLock _spinLock = default(SpinLock);

	private static readonly ThreadLocal<ObjectPool<Awaitable>> _pool = new ThreadLocal<ObjectPool<Awaitable>>(() => new ObjectPool<Awaitable>(() => new Awaitable(), null, null, null, collectionCheck: false));

	private AwaitableHandle _handle;

	private ExceptionDispatchInfo _exceptionToRethrow;

	private bool _managedAwaitableDone;

	private AwaiterCompletionThreadAffinity _completionThreadAffinity;

	private Action _continuation;

	private CancellationTokenRegistration? _cancelTokenRegistration;

	private DoubleBufferedAwaitableList _managedCompletionQueue;

	private static bool _nextFrameAndEndOfFrameWiredUp = false;

	private static CancellationTokenRegistration _nextFrameAndEndOfFrameWiredUpCTRegistration = default(CancellationTokenRegistration);

	private static readonly DoubleBufferedAwaitableList _nextFrameAwaitables = new DoubleBufferedAwaitableList();

	private static readonly DoubleBufferedAwaitableList _endOfFrameAwaitables = new DoubleBufferedAwaitableList();

	private static SynchronizationContext _synchronizationContext;

	private static int _mainThreadId;

	private bool IsCompletedNoLock
	{
		get
		{
			CheckPointerValidity();
			if (_handle.IsManaged)
			{
				return _managedAwaitableDone && MatchCompletionThreadAffinity(_completionThreadAffinity);
			}
			return IsNativeAwaitableCompleted(_handle) != 0;
		}
	}

	private bool IsLogicallyCompletedNoLock
	{
		get
		{
			CheckPointerValidity();
			if (_handle.IsManaged)
			{
				return _managedAwaitableDone;
			}
			return IsNativeAwaitableCompleted(_handle) != 0;
		}
	}

	public bool IsCompleted
	{
		get
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);
				return IsCompletedNoLock;
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}
	}

	internal bool IsDettachedOrCompleted
	{
		get
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);
				if (_handle.IsNull)
				{
					return true;
				}
				CheckPointerValidity();
				if (_handle.IsManaged)
				{
					return _managedAwaitableDone;
				}
				return IsNativeAwaitableCompleted(_handle) != 0;
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}
	}

	internal AwaiterCompletionThreadAffinity CompletionThreadAffinity
	{
		get
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);
				return _completionThreadAffinity;
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}
		set
		{
			bool lockTaken = false;
			try
			{
				_spinLock.Enter(ref lockTaken);
				_completionThreadAffinity = value;
			}
			finally
			{
				if (lockTaken)
				{
					_spinLock.Exit();
				}
			}
		}
	}

	object IEnumerator.Current => null;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Awaitable FromAsyncOperation(AsyncOperation op, CancellationToken cancellationToken = default(CancellationToken))
	{
		cancellationToken.ThrowIfCancellationRequested();
		IntPtr nativeHandle = FromAsyncOperationInternal(op.m_Ptr);
		return FromNativeAwaitableHandle(nativeHandle, cancellationToken);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::FromAsyncOperation", ThrowsException = true)]
	private static extern IntPtr FromAsyncOperationInternal(IntPtr asyncOperation);

	[ExcludeFromDocs]
	public Awaiter GetAwaiter()
	{
		return new Awaiter(this);
	}

	[RequiredByNativeCode(GenerateProxy = true)]
	private void SetExceptionFromNative(Exception ex)
	{
		bool lockTaken = false;
		try
		{
			_spinLock.Enter(ref lockTaken);
			_exceptionToRethrow = ExceptionDispatchInfo.Capture(ex);
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
	}

	[RequiredByNativeCode(GenerateProxy = true)]
	private void RunContinuation()
	{
		Action action = null;
		bool lockTaken = false;
		try
		{
			_spinLock.Enter(ref lockTaken);
			action = _continuation;
			_continuation = null;
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
		action?.Invoke();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::AttachManagedWrapper", IsThreadSafe = true)]
	private static extern void AttachManagedGCHandleToNativeAwaitable(IntPtr nativeAwaitable, UIntPtr gcHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::Release", IsThreadSafe = true)]
	private static extern void ReleaseNativeAwaitable(IntPtr nativeAwaitable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::Cancel", IsThreadSafe = true)]
	private static extern void CancelNativeAwaitable(IntPtr nativeAwaitable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::IsCompleted", IsThreadSafe = true)]
	private static extern int IsNativeAwaitableCompleted(IntPtr nativeAwaitable);

	private Awaitable()
	{
	}

	internal static Awaitable NewManagedAwaitable()
	{
		Awaitable awaitable = _pool.Value.Get();
		awaitable._handle = AwaitableHandle.ManagedHandle;
		return awaitable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static Awaitable FromNativeAwaitableHandle(IntPtr nativeHandle, CancellationToken cancellationToken)
	{
		Awaitable awaitable = _pool.Value.Get();
		awaitable._handle = nativeHandle;
		AttachManagedGCHandleToNativeAwaitable(nativeHandle, (UIntPtr)(void*)GCHandle.ToIntPtr(GCHandle.Alloc(awaitable)));
		if (cancellationToken.CanBeCanceled)
		{
			WireupCancellation(awaitable, cancellationToken);
		}
		return awaitable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void WireupCancellation(Awaitable awaitable, CancellationToken cancellationToken)
	{
		if (awaitable == null)
		{
			throw new ArgumentNullException("awaitable");
		}
		bool lockTaken = false;
		try
		{
			awaitable._spinLock.Enter(ref lockTaken);
			using (ExecutionContext.SuppressFlow())
			{
				awaitable._cancelTokenRegistration = cancellationToken.Register(delegate(object coroutine)
				{
					((Awaitable)coroutine).Cancel();
				}, awaitable, useSynchronizationContext: false);
			}
		}
		finally
		{
			if (lockTaken)
			{
				awaitable._spinLock.Exit();
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool MatchCompletionThreadAffinity(AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity)
	{
		if (1 == 0)
		{
		}
		bool result = awaiterCompletionThreadAffinity switch
		{
			AwaiterCompletionThreadAffinity.MainThread => Thread.CurrentThread.ManagedThreadId == _mainThreadId, 
			AwaiterCompletionThreadAffinity.BackgroundThread => Thread.CurrentThread.ManagedThreadId != _mainThreadId, 
			_ => true, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void RaiseManagedCompletion(Exception exception)
	{
		Action action = null;
		bool lockTaken = false;
		AwaiterCompletionThreadAffinity completionThreadAffinity;
		try
		{
			_spinLock.Enter(ref lockTaken);
			if (exception != null)
			{
				_exceptionToRethrow = ExceptionDispatchInfo.Capture(exception);
			}
			_managedAwaitableDone = true;
			action = _continuation;
			completionThreadAffinity = _completionThreadAffinity;
			_continuation = null;
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
		if (action != null)
		{
			RunOrScheduleContinuation(completionThreadAffinity, action);
		}
	}

	private void RunOrScheduleContinuation(AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity, Action continuation)
	{
		if (MatchCompletionThreadAffinity(awaiterCompletionThreadAffinity))
		{
			continuation();
			return;
		}
		switch (awaiterCompletionThreadAffinity)
		{
		case AwaiterCompletionThreadAffinity.MainThread:
			_synchronizationContext.Post(DoRunContinuationOnSynchonizationContext, continuation);
			break;
		case AwaiterCompletionThreadAffinity.BackgroundThread:
			Task.Run(continuation);
			break;
		}
	}

	private static void DoRunContinuationOnSynchonizationContext(object continuation)
	{
		(continuation as Action)?.Invoke();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void RaiseManagedCompletion()
	{
		Action action = null;
		bool lockTaken = false;
		AwaiterCompletionThreadAffinity completionThreadAffinity;
		try
		{
			_spinLock.Enter(ref lockTaken);
			_managedAwaitableDone = true;
			completionThreadAffinity = _completionThreadAffinity;
			action = _continuation;
			_continuation = null;
			_managedCompletionQueue = null;
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
		if (action != null)
		{
			RunOrScheduleContinuation(completionThreadAffinity, action);
		}
	}

	internal void PropagateExceptionAndRelease()
	{
		bool lockTaken = false;
		try
		{
			_spinLock.Enter(ref lockTaken);
			CheckPointerValidity();
			if (_cancelTokenRegistration.HasValue)
			{
				_cancelTokenRegistration.Value.Dispose();
				_cancelTokenRegistration = null;
			}
			_managedAwaitableDone = false;
			_completionThreadAffinity = AwaiterCompletionThreadAffinity.None;
			AwaitableHandle handle = _handle;
			_handle = AwaitableHandle.NullHandle;
			ExceptionDispatchInfo exceptionToRethrow = _exceptionToRethrow;
			_exceptionToRethrow = null;
			_managedCompletionQueue = null;
			_continuation = null;
			if (!handle.IsManaged && !handle.IsNull)
			{
				ReleaseNativeAwaitable(handle);
			}
			_pool.Value.Release(this);
			exceptionToRethrow?.Throw();
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
	}

	public void Cancel()
	{
		AwaitableHandle awaitableHandle = CheckPointerValidity();
		if (awaitableHandle.IsManaged)
		{
			_managedCompletionQueue?.Remove(this);
			RaiseManagedCompletion(new OperationCanceledException());
		}
		else
		{
			CancelNativeAwaitable(awaitableHandle);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private AwaitableHandle CheckPointerValidity()
	{
		AwaitableHandle handle = _handle;
		if (handle.IsNull)
		{
			throw new InvalidOperationException("Awaitable is in detached state");
		}
		return handle;
	}

	internal void SetContinuation(Action continuation)
	{
		bool flag = false;
		bool lockTaken = false;
		AwaiterCompletionThreadAffinity awaiterCompletionThreadAffinity = AwaiterCompletionThreadAffinity.None;
		try
		{
			_spinLock.Enter(ref lockTaken);
			if (IsLogicallyCompletedNoLock)
			{
				flag = true;
				awaiterCompletionThreadAffinity = _completionThreadAffinity;
			}
			else
			{
				_continuation = continuation;
			}
		}
		finally
		{
			if (lockTaken)
			{
				_spinLock.Exit();
			}
		}
		if (flag)
		{
			RunOrScheduleContinuation(awaiterCompletionThreadAffinity, continuation);
		}
	}

	bool IEnumerator.MoveNext()
	{
		if (IsCompleted)
		{
			PropagateExceptionAndRelease();
			return false;
		}
		return true;
	}

	void IEnumerator.Reset()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ThrowIfNotMainThread()
	{
		if (Thread.CurrentThread.ManagedThreadId != _mainThreadId)
		{
			throw new InvalidOperationException("This operation can only be performed on the main thread.");
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Awaitable NextFrameAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfNotMainThread();
		cancellationToken.ThrowIfCancellationRequested();
		EnsureDelayedCallWiredUp();
		Awaitable awaitable = NewManagedAwaitable();
		_nextFrameAwaitables.Add(awaitable, Time.frameCount + 1);
		awaitable._managedCompletionQueue = _nextFrameAwaitables;
		if (cancellationToken.CanBeCanceled)
		{
			WireupCancellation(awaitable, cancellationToken);
		}
		return awaitable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Awaitable WaitForSecondsAsync(float seconds, CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfNotMainThread();
		cancellationToken.ThrowIfCancellationRequested();
		IntPtr nativeHandle = WaitForScondsInternal(seconds);
		return FromNativeAwaitableHandle(nativeHandle, cancellationToken);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Awaitable FixedUpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfNotMainThread();
		cancellationToken.ThrowIfCancellationRequested();
		IntPtr nativeHandle = FixedUpdateInternal();
		return FromNativeAwaitableHandle(nativeHandle, cancellationToken);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Awaitable EndOfFrameAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		ThrowIfNotMainThread();
		cancellationToken.ThrowIfCancellationRequested();
		EnsureDelayedCallWiredUp();
		Awaitable awaitable = NewManagedAwaitable();
		_endOfFrameAwaitables.Add(awaitable, -1);
		awaitable._managedCompletionQueue = _endOfFrameAwaitables;
		if (cancellationToken.CanBeCanceled)
		{
			WireupCancellation(awaitable, cancellationToken);
		}
		return awaitable;
	}

	private static void EnsureDelayedCallWiredUp()
	{
		if (!_nextFrameAndEndOfFrameWiredUp)
		{
			_nextFrameAndEndOfFrameWiredUp = true;
			WireupNextFrameAndEndOfFrameCallbacks();
			_nextFrameAndEndOfFrameWiredUpCTRegistration = Application.exitCancellationToken.Register(OnDelayedCallManagerCleared);
		}
	}

	[RequiredByNativeCode]
	private static void OnDelayedCallManagerCleared()
	{
		_nextFrameAndEndOfFrameWiredUp = false;
		_nextFrameAwaitables.Clear();
		_endOfFrameAwaitables.Clear();
	}

	[RequiredByNativeCode]
	private static void OnUpdate()
	{
		_nextFrameAwaitables.SwapAndComplete();
	}

	[RequiredByNativeCode]
	private static void OnEndOfFrame()
	{
		_endOfFrameAwaitables.SwapAndComplete();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::NextFrameAwaitable")]
	private static extern IntPtr NextFrameInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::WaitForSecondsAwaitable")]
	private static extern IntPtr WaitForScondsInternal(float seconds);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::FixedUpdateAwaitable")]
	private static extern IntPtr FixedUpdateInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::EndOfFrameAwaitable")]
	private static extern IntPtr EndOfFrameInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("Scripting::Awaitables::WireupNextFrameAndEndOfFrameCallbacks")]
	private static extern void WireupNextFrameAndEndOfFrameCallbacks();

	internal static void SetSynchronizationContext(UnitySynchronizationContext synchronizationContext)
	{
		_synchronizationContext = synchronizationContext;
		_mainThreadId = synchronizationContext.MainThreadId;
	}

	public static MainThreadAwaitable MainThreadAsync()
	{
		return new MainThreadAwaitable(_synchronizationContext, _mainThreadId);
	}

	public static BackgroundThreadAwaitable BackgroundThreadAsync()
	{
		return new BackgroundThreadAwaitable(_synchronizationContext, _mainThreadId);
	}
}
[AsyncMethodBuilder(typeof(Awaitable.AwaitableAsyncMethodBuilder<>))]
public class Awaitable<T>
{
	[ExcludeFromDocs]
	public struct Awaiter : INotifyCompletion
	{
		private readonly Awaitable<T> _coroutine;

		public bool IsCompleted => _coroutine._awaitable.IsCompleted;

		public Awaiter(Awaitable<T> coroutine)
		{
			_coroutine = coroutine;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnCompleted(Action continuation)
		{
			_coroutine.ContinueWith(continuation);
		}

		public T GetResult()
		{
			return _coroutine.GetResult();
		}
	}

	private static readonly ThreadLocal<ObjectPool<Awaitable<T>>> _pool = new ThreadLocal<ObjectPool<Awaitable<T>>>(() => new ObjectPool<Awaitable<T>>(() => new Awaitable<T>(), null, null, null, collectionCheck: false));

	private Awaitable _awaitable;

	private T _result;

	internal Awaitable.AwaiterCompletionThreadAffinity CompletionThreadAffinity
	{
		get
		{
			return _awaitable.CompletionThreadAffinity;
		}
		set
		{
			_awaitable.CompletionThreadAffinity = value;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ContinueWith(Action continuation)
	{
		_awaitable.SetContinuation(continuation);
	}

	private T GetResult()
	{
		try
		{
			_awaitable.PropagateExceptionAndRelease();
			return _result;
		}
		finally
		{
			_awaitable = null;
			_result = default(T);
			_pool.Value.Release(this);
		}
	}

	internal void SetResultAndRaiseContinuation(T result)
	{
		_result = result;
		_awaitable.RaiseManagedCompletion();
	}

	internal void SetExceptionAndRaiseContinuation(Exception exception)
	{
		_awaitable.RaiseManagedCompletion(exception);
	}

	public void Cancel()
	{
		_awaitable.Cancel();
	}

	private Awaitable()
	{
	}

	internal static Awaitable<T> GetManaged()
	{
		Awaitable awaitable = Awaitable.NewManagedAwaitable();
		Awaitable<T> awaitable2 = _pool.Value.Get();
		awaitable2._awaitable = awaitable;
		return awaitable2;
	}

	[ExcludeFromDocs]
	public Awaiter GetAwaiter()
	{
		return new Awaiter(this);
	}
}
