using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Graphics/Mesh/MeshFilter.h")]
public sealed class MeshFilter : Component
{
	public Mesh sharedMesh
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(get_sharedMesh_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sharedMesh_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Mesh mesh
	{
		[NativeName("GetInstantiatedMeshFromScript")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(get_mesh_Injected(intPtr));
		}
		[NativeName("SetInstantiatedMesh")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mesh_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	[RequiredByNativeCode]
	private void DontStripMeshFilter()
	{
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMesh_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_mesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mesh_Injected(IntPtr _unity_self, IntPtr value);
}
