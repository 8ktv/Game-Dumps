using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics/MeshCollider.h")]
[RequireComponent(typeof(Transform))]
[NativeHeader("Runtime/Graphics/Mesh/Mesh.h")]
public class MeshCollider : Collider
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

	public bool convex
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_convex_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_convex_Injected(intPtr, value);
		}
	}

	public MeshColliderCookingOptions cookingOptions
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cookingOptions_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cookingOptions_Injected(intPtr, value);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMesh_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_convex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_convex_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MeshColliderCookingOptions get_cookingOptions_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cookingOptions_Injected(IntPtr _unity_self, MeshColliderCookingOptions value);
}
