using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.LowLevelPhysics;

namespace UnityEngine;

[NativeHeader("Modules/Physics/Collider.h")]
public class Collider : Component
{
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

	public Rigidbody attachedRigidbody
	{
		[NativeMethod("GetRigidbody")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Rigidbody>(get_attachedRigidbody_Injected(intPtr));
		}
	}

	public ArticulationBody attachedArticulationBody
	{
		[NativeMethod("GetArticulationBody")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<ArticulationBody>(get_attachedArticulationBody_Injected(intPtr));
		}
	}

	public bool isTrigger
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isTrigger_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_isTrigger_Injected(intPtr, value);
		}
	}

	public float contactOffset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_contactOffset_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_contactOffset_Injected(intPtr, value);
		}
	}

	public Bounds bounds
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bounds_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public bool hasModifiableContacts
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasModifiableContacts_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_hasModifiableContacts_Injected(intPtr, value);
		}
	}

	public bool providesContacts
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_providesContacts_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_providesContacts_Injected(intPtr, value);
		}
	}

	public int layerOverridePriority
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_layerOverridePriority_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_layerOverridePriority_Injected(intPtr, value);
		}
	}

	public LayerMask excludeLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_excludeLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_excludeLayers_Injected(intPtr, ref value);
		}
	}

	public LayerMask includeLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_includeLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_includeLayers_Injected(intPtr, ref value);
		}
	}

	public GeometryHolder GeometryHolder
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_GeometryHolder_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[NativeMethod("Material")]
	public PhysicsMaterial sharedMaterial
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<PhysicsMaterial>(get_sharedMaterial_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sharedMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public PhysicsMaterial material
	{
		[NativeMethod("GetClonedMaterial")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<PhysicsMaterial>(get_material_Injected(intPtr));
		}
		[NativeMethod("SetMaterial")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_material_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClosestPoint_Injected(intPtr, ref position, out var ret);
		return ret;
	}

	public T GetGeometry<T>() where T : struct, IGeometry
	{
		return GeometryHolder.As<T>();
	}

	private RaycastHit Raycast(Ray ray, float maxDistance, ref bool hasHit)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Raycast_Injected(intPtr, ref ray, maxDistance, ref hasHit, out var ret);
		return ret;
	}

	public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
	{
		bool hasHit = false;
		hitInfo = Raycast(ray, maxDistance, ref hasHit);
		return hasHit;
	}

	[NativeName("ClosestPointOnBounds")]
	private void Internal_ClosestPointOnBounds(Vector3 point, ref Vector3 outPos, ref float distance)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_ClosestPointOnBounds_Injected(intPtr, ref point, ref outPos, ref distance);
	}

	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		float distance = 0f;
		Vector3 outPos = Vector3.zero;
		Internal_ClosestPointOnBounds(position, ref outPos, ref distance);
		return outPos;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enabled_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_attachedRigidbody_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_attachedArticulationBody_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isTrigger_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_isTrigger_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_contactOffset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_contactOffset_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClosestPoint_Injected(IntPtr _unity_self, [In] ref Vector3 position, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasModifiableContacts_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_hasModifiableContacts_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_providesContacts_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_providesContacts_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_layerOverridePriority_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_layerOverridePriority_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_excludeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_excludeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_includeLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_includeLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_GeometryHolder_Injected(IntPtr _unity_self, out GeometryHolder ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMaterial_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_material_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Raycast_Injected(IntPtr _unity_self, [In] ref Ray ray, float maxDistance, ref bool hasHit, out RaycastHit ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_ClosestPointOnBounds_Injected(IntPtr _unity_self, [In] ref Vector3 point, ref Vector3 outPos, ref float distance);
}
