using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Runtime/Export/Camera/CullingGroup.bindings.h")]
public class CullingGroup : IDisposable
{
	public delegate void StateChanged(CullingGroupEvent sphere);

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(CullingGroup cullingGroup)
		{
			return cullingGroup.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	private StateChanged m_OnStateChanged = null;

	public StateChanged onStateChanged
	{
		get
		{
			return m_OnStateChanged;
		}
		set
		{
			m_OnStateChanged = value;
		}
	}

	public bool enabled
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enabled_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enabled_Injected(intPtr, value);
		}
	}

	public Camera targetCamera
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Camera>(get_targetCamera_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_targetCamera_Injected(intPtr, Object.MarshalledUnityObject.Marshal(value));
		}
	}

	public CullingGroup()
	{
		m_Ptr = Init(this);
	}

	~CullingGroup()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			FinalizerFailure();
		}
	}

	[FreeFunction("CullingGroup_Bindings::Dispose", HasExplicitThis = true)]
	private void DisposeInternal()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DisposeInternal_Injected(intPtr);
	}

	public void Dispose()
	{
		DisposeInternal();
		m_Ptr = IntPtr.Zero;
	}

	public void SetBoundingSpheres([UnityMarshalAs(NativeType.ScriptingObjectPtr)] BoundingSphere[] array)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBoundingSpheres_Injected(intPtr, array);
	}

	public void SetBoundingSphereCount(int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBoundingSphereCount_Injected(intPtr, count);
	}

	public void EraseSwapBack(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EraseSwapBack_Injected(intPtr, index);
	}

	public static void EraseSwapBack<T>(int index, T[] myArray, ref int size)
	{
		size--;
		myArray[index] = myArray[size];
	}

	public int QueryIndices(bool visible, int[] result, int firstIndex)
	{
		return QueryIndices(visible, -1, CullingQueryOptions.IgnoreDistance, result, firstIndex);
	}

	public int QueryIndices(int distanceIndex, int[] result, int firstIndex)
	{
		return QueryIndices(visible: false, distanceIndex, CullingQueryOptions.IgnoreVisibility, result, firstIndex);
	}

	public int QueryIndices(bool visible, int distanceIndex, int[] result, int firstIndex)
	{
		return QueryIndices(visible, distanceIndex, CullingQueryOptions.Normal, result, firstIndex);
	}

	[NativeThrows]
	[FreeFunction("CullingGroup_Bindings::QueryIndices", HasExplicitThis = true)]
	private unsafe int QueryIndices(bool visible, int distanceIndex, CullingQueryOptions options, int[] result, int firstIndex)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = new Span<int>(result);
		int result3;
		fixed (int* begin = span)
		{
			ManagedSpanWrapper result2 = new ManagedSpanWrapper(begin, span.Length);
			result3 = QueryIndices_Injected(intPtr, visible, distanceIndex, options, ref result2, firstIndex);
		}
		return result3;
	}

	[NativeThrows]
	[FreeFunction("CullingGroup_Bindings::IsVisible", HasExplicitThis = true)]
	public bool IsVisible(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsVisible_Injected(intPtr, index);
	}

	[NativeThrows]
	[FreeFunction("CullingGroup_Bindings::GetDistance", HasExplicitThis = true)]
	public int GetDistance(int index)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDistance_Injected(intPtr, index);
	}

	[FreeFunction("CullingGroup_Bindings::SetBoundingDistances", HasExplicitThis = true)]
	public unsafe void SetBoundingDistances(float[] distances)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(distances);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper distances2 = new ManagedSpanWrapper(begin, span.Length);
			SetBoundingDistances_Injected(intPtr, ref distances2);
		}
	}

	[FreeFunction("CullingGroup_Bindings::SetDistanceReferencePoint", HasExplicitThis = true)]
	private void SetDistanceReferencePoint_InternalVector3(Vector3 point)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDistanceReferencePoint_InternalVector3_Injected(intPtr, ref point);
	}

	[NativeMethod("SetDistanceReferenceTransform")]
	private void SetDistanceReferencePoint_InternalTransform(Transform transform)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetDistanceReferencePoint_InternalTransform_Injected(intPtr, Object.MarshalledUnityObject.Marshal(transform));
	}

	public void SetDistanceReferencePoint(Vector3 point)
	{
		SetDistanceReferencePoint_InternalVector3(point);
	}

	public void SetDistanceReferencePoint(Transform transform)
	{
		SetDistanceReferencePoint_InternalTransform(transform);
	}

	[RequiredByNativeCode]
	[SecuritySafeCritical]
	private unsafe static void SendEvents(CullingGroup cullingGroup, IntPtr eventsPtr, int count)
	{
		CullingGroupEvent* ptr = (CullingGroupEvent*)eventsPtr.ToPointer();
		if (cullingGroup.m_OnStateChanged != null)
		{
			for (int i = 0; i < count; i++)
			{
				cullingGroup.m_OnStateChanged(ptr[i]);
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("CullingGroup_Bindings::Init")]
	private static extern IntPtr Init(object scripting);

	[FreeFunction("CullingGroup_Bindings::FinalizerFailure", HasExplicitThis = true, IsThreadSafe = true)]
	private void FinalizerFailure()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		FinalizerFailure_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisposeInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enabled_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_targetCamera_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_targetCamera_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoundingSpheres_Injected(IntPtr _unity_self, BoundingSphere[] array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoundingSphereCount_Injected(IntPtr _unity_self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EraseSwapBack_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int QueryIndices_Injected(IntPtr _unity_self, bool visible, int distanceIndex, CullingQueryOptions options, ref ManagedSpanWrapper result, int firstIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsVisible_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDistance_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoundingDistances_Injected(IntPtr _unity_self, ref ManagedSpanWrapper distances);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDistanceReferencePoint_InternalVector3_Injected(IntPtr _unity_self, [In] ref Vector3 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDistanceReferencePoint_InternalTransform_Injected(IntPtr _unity_self, IntPtr transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FinalizerFailure_Injected(IntPtr _unity_self);
}
