using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetLODGroupManager()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/Graphics/LOD/LODGroup.h")]
[NativeHeader("Runtime/Graphics/LOD/LODGroupManager.h")]
[NativeHeader("Runtime/Graphics/LOD/LODUtility.h")]
public class LODGroup : Component
{
	public Vector3 localReferencePoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localReferencePoint_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_localReferencePoint_Injected(intPtr, ref value);
		}
	}

	public float size
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_size_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_size_Injected(intPtr, value);
		}
	}

	public int lodCount
	{
		[NativeMethod("GetLODCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lodCount_Injected(intPtr);
		}
	}

	public bool lastLODBillboard
	{
		[NativeMethod("GetLastLODIsBillboard")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lastLODBillboard_Injected(intPtr);
		}
		[NativeMethod("SetLastLODIsBillboard")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lastLODBillboard_Injected(intPtr, value);
		}
	}

	public LODFadeMode fadeMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fadeMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fadeMode_Injected(intPtr, value);
		}
	}

	public bool animateCrossFading
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_animateCrossFading_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_animateCrossFading_Injected(intPtr, value);
		}
	}

	public bool enabled
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enabled_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enabled_Injected(intPtr, value);
		}
	}

	[StaticAccessor("GetLODGroupManager()")]
	public static extern float crossFadeAnimationDuration
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	internal Vector3 worldReferencePoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_worldReferencePoint_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[FreeFunction("UpdateLODGroupBoundingBox", HasExplicitThis = true)]
	public void RecalculateBounds()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateBounds_Injected(intPtr);
	}

	[FreeFunction("GetLODs_Binding", HasExplicitThis = true)]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public LOD[] GetLODs()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLODs_Injected(intPtr);
	}

	[Obsolete("Use SetLODs instead.")]
	public void SetLODS(LOD[] lods)
	{
		SetLODs(lods);
	}

	[FreeFunction("SetLODs_Binding", HasExplicitThis = true)]
	public void SetLODs([UnityMarshalAs(NativeType.ScriptingObjectPtr)] LOD[] lods)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLODs_Injected(intPtr, lods);
	}

	[FreeFunction("ForceLODLevel", HasExplicitThis = true)]
	public void ForceLOD(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ForceLOD_Injected(intPtr, index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localReferencePoint_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_localReferencePoint_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_size_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_size_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_lodCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_lastLODBillboard_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lastLODBillboard_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LODFadeMode get_fadeMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fadeMode_Injected(IntPtr _unity_self, LODFadeMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_animateCrossFading_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_animateCrossFading_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enabled_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateBounds_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LOD[] GetLODs_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLODs_Injected(IntPtr _unity_self, LOD[] lods);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ForceLOD_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_worldReferencePoint_Injected(IntPtr _unity_self, out Vector3 ret);
}
