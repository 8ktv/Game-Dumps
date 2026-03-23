using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeHeader("Runtime/Export/Scripting/AsyncOperation.bindings.h")]
[NativeHeader("Runtime/Misc/AsyncOperation.h")]
public class AsyncOperation : YieldInstruction
{
	internal static class BindingsMarshaller
	{
		public static AsyncOperation ConvertToManaged(IntPtr ptr)
		{
			return new AsyncOperation(ptr);
		}

		public static IntPtr ConvertToNative(AsyncOperation asyncOperation)
		{
			return asyncOperation.m_Ptr;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.AssetBundleModule" })]
	internal IntPtr m_Ptr;

	private Action<AsyncOperation> m_completeCallback;

	public bool isDone
	{
		[NativeMethod("IsDone")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isDone_Injected(intPtr);
		}
	}

	public float progress
	{
		[NativeMethod("GetProgress")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_progress_Injected(intPtr);
		}
	}

	public int priority
	{
		[NativeMethod("GetPriority")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_priority_Injected(intPtr);
		}
		[NativeMethod("SetPriority")]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_priority_Injected(intPtr, value);
		}
	}

	public bool allowSceneActivation
	{
		[NativeMethod("GetAllowSceneActivation")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowSceneActivation_Injected(intPtr);
		}
		[NativeMethod("SetAllowSceneActivation")]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowSceneActivation_Injected(intPtr, value);
		}
	}

	public event Action<AsyncOperation> completed
	{
		add
		{
			if (isDone)
			{
				value(this);
			}
			else
			{
				m_completeCallback = (Action<AsyncOperation>)Delegate.Combine(m_completeCallback, value);
			}
		}
		remove
		{
			m_completeCallback = (Action<AsyncOperation>)Delegate.Remove(m_completeCallback, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void InternalDestroy(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("AsyncOperationBindings", StaticAccessorType.DoubleColon)]
	[NativeMethod(IsThreadSafe = true)]
	private static extern void InternalSetManagedObject(IntPtr ptr, [Unmarshalled] AsyncOperation self);

	public AsyncOperation()
	{
	}

	protected AsyncOperation(IntPtr ptr)
	{
		if (!(ptr == IntPtr.Zero))
		{
			InternalSetManagedObject(ptr, this);
			m_Ptr = ptr;
		}
	}

	~AsyncOperation()
	{
		InternalDestroy(m_Ptr);
	}

	[RequiredByNativeCode]
	internal void InvokeCompletionEvent()
	{
		if (m_completeCallback != null)
		{
			m_completeCallback(this);
			m_completeCallback = null;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isDone_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_progress_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_priority_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_priority_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowSceneActivation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowSceneActivation_Injected(IntPtr _unity_self, bool value);
}
