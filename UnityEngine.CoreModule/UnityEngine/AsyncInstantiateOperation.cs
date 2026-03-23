using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/GameCode/AsyncInstantiate/AsyncInstantiateOperation.h")]
[RequiredByNativeCode]
public class AsyncInstantiateOperation : AsyncOperation
{
	internal new static class BindingsMarshaller
	{
		public static AsyncInstantiateOperation ConvertToManaged(IntPtr ptr)
		{
			return new AsyncInstantiateOperation(ptr, CancellationToken.None);
		}

		public static IntPtr ConvertToNative(AsyncInstantiateOperation obj)
		{
			return obj.m_Ptr;
		}
	}

	internal static CancellationTokenSource s_GlobalCancellation = new CancellationTokenSource();

	internal Object[] m_Result;

	private CancellationToken m_CancellationToken;

	public Object[] Result => m_Result;

	[StaticAccessor("GetAsyncInstantiateManager()", StaticAccessorType.Dot)]
	internal static extern float IntegrationTimeMS
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeMethod("IsWaitingForSceneActivation")]
	public bool IsWaitingForSceneActivation()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsWaitingForSceneActivation_Injected(intPtr);
	}

	[NativeMethod("WaitForCompletion")]
	public void WaitForCompletion()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		WaitForCompletion_Injected(intPtr);
	}

	[NativeMethod("Cancel")]
	public void Cancel()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Cancel_Injected(intPtr);
	}

	public AsyncInstantiateOperation()
		: this(IntPtr.Zero, default(CancellationToken))
	{
	}

	protected AsyncInstantiateOperation(IntPtr ptr, CancellationToken cancellationToken)
		: base(ptr)
	{
		m_CancellationToken = CancellationTokenSource.CreateLinkedTokenSource(s_GlobalCancellation.Token, cancellationToken).Token;
	}

	public static float GetIntegrationTimeMS()
	{
		return IntegrationTimeMS;
	}

	public static void SetIntegrationTimeMS(float integrationTimeMS)
	{
		if (integrationTimeMS <= 0f)
		{
			throw new ArgumentOutOfRangeException("integrationTimeMS", "integrationTimeMS was out of range. Must be greater than zero.");
		}
		IntegrationTimeMS = integrationTimeMS;
	}

	[RequiredByNativeCode(GenerateProxy = true)]
	private bool IsCancellationRequested()
	{
		return m_CancellationToken.IsCancellationRequested;
	}

	internal virtual Object[] CreateResultArray(int size)
	{
		m_Result = new Object[size];
		return m_Result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsWaitingForSceneActivation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WaitForCompletion_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Cancel_Injected(IntPtr _unity_self);
}
public class AsyncInstantiateOperation<T> : AsyncInstantiateOperation
{
	internal new static class BindingsMarshaller
	{
		public static AsyncInstantiateOperation<T> ConvertToManaged(IntPtr ptr)
		{
			return new AsyncInstantiateOperation<T>(ptr, CancellationToken.None);
		}

		public static IntPtr ConvertToNative(AsyncInstantiateOperation<T> obj)
		{
			return obj.m_Ptr;
		}
	}

	[ExcludeFromDocs]
	public struct Awaiter : INotifyCompletion
	{
		private readonly Awaitable _awaitable;

		private readonly AsyncInstantiateOperation<T> _op;

		public bool IsCompleted => _awaitable.IsCompleted;

		public Awaiter(AsyncInstantiateOperation<T> op)
		{
			_awaitable = Awaitable.FromAsyncOperation(op);
			_op = op;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnCompleted(Action continuation)
		{
			_awaitable.SetContinuation(continuation);
		}

		public T[] GetResult()
		{
			_awaitable.GetAwaiter().GetResult();
			return _op.Result;
		}
	}

	public new T[] Result => (T[])(object)m_Result;

	internal AsyncInstantiateOperation(IntPtr ptr, CancellationToken cancellationToken)
		: base(ptr, cancellationToken)
	{
	}

	internal override Object[] CreateResultArray(int size)
	{
		m_Result = (Object[])(object)new T[size];
		return m_Result;
	}

	[ExcludeFromDocs]
	public Awaiter GetAwaiter()
	{
		return new Awaiter(this);
	}
}
