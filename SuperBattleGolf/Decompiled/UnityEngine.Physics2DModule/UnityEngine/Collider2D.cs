using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode(Optional = true)]
[NativeHeader("Modules/Physics2D/Public/Collider2D.h")]
[RequireComponent(typeof(Transform))]
public class Collider2D : Behaviour
{
	public enum CompositeOperation
	{
		None,
		Merge,
		Intersect,
		Difference,
		Flip
	}

	public float density
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_density_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_density_Injected(intPtr, value);
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

	public bool usedByEffector
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_usedByEffector_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_usedByEffector_Injected(intPtr, value);
		}
	}

	public CompositeOperation compositeOperation
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_compositeOperation_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_compositeOperation_Injected(intPtr, value);
		}
	}

	public int compositeOrder
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_compositeOrder_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_compositeOrder_Injected(intPtr, value);
		}
	}

	public CompositeCollider2D composite
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<CompositeCollider2D>(get_composite_Injected(intPtr));
		}
	}

	public Vector2 offset
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_offset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_offset_Injected(intPtr, ref value);
		}
	}

	public Rigidbody2D attachedRigidbody
	{
		[NativeMethod("GetAttachedRigidbody_Binding")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Rigidbody2D>(get_attachedRigidbody_Injected(intPtr));
		}
	}

	public Matrix4x4 localToWorldMatrix
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_localToWorldMatrix_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public int shapeCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shapeCount_Injected(intPtr);
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

	public ColliderErrorState2D errorState
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_errorState_Injected(intPtr);
		}
	}

	public bool compositeCapable
	{
		[NativeMethod("GetCompositeCapable_Binding")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_compositeCapable_Injected(intPtr);
		}
	}

	public PhysicsMaterial2D sharedMaterial
	{
		[NativeMethod("GetMaterial")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<PhysicsMaterial2D>(get_sharedMaterial_Injected(intPtr));
		}
		[NativeMethod("SetMaterial")]
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

	public LayerMask forceSendLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_forceSendLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceSendLayers_Injected(intPtr, ref value);
		}
	}

	public LayerMask forceReceiveLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_forceReceiveLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceReceiveLayers_Injected(intPtr, ref value);
		}
	}

	public LayerMask contactCaptureLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_contactCaptureLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_contactCaptureLayers_Injected(intPtr, ref value);
		}
	}

	public LayerMask callbackLayers
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_callbackLayers_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_callbackLayers_Injected(intPtr, ref value);
		}
	}

	public float friction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_friction_Injected(intPtr);
		}
	}

	public float bounciness
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bounciness_Injected(intPtr);
		}
	}

	public PhysicsMaterialCombine2D frictionCombine
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_frictionCombine_Injected(intPtr);
		}
	}

	public PhysicsMaterialCombine2D bounceCombine
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bounceCombine_Injected(intPtr);
		}
	}

	public LayerMask contactMask
	{
		[NativeMethod("GetContactMask_Binding")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_contactMask_Injected(intPtr, out var ret);
			return ret;
		}
	}

	[ExcludeFromDocs]
	[Obsolete("usedByComposite has been deprecated. Please use compositeOperation.", false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool usedByComposite
	{
		get
		{
			return compositeOperation != CompositeOperation.None;
		}
		set
		{
			compositeOperation = (value ? CompositeOperation.Merge : CompositeOperation.None);
		}
	}

	[ExcludeFromDocs]
	public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation)
	{
		return CreateMesh(useBodyPosition, useBodyRotation, useDelaunay: true);
	}

	[NativeMethod("CreateMesh_Binding")]
	public Mesh CreateMesh(bool useBodyPosition, bool useBodyRotation, [UnityEngine.Internal.DefaultValue("true")] bool useDelaunay = true)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Mesh>(CreateMesh_Injected(intPtr, useBodyPosition, useBodyRotation, useDelaunay));
	}

	[NativeMethod("GetShapeHash_Binding")]
	public uint GetShapeHash()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetShapeHash_Injected(intPtr);
	}

	public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup)
	{
		return GetShapes_Internal(ref physicsShapeGroup.m_GroupState, 0, shapeCount);
	}

	public int GetShapes(PhysicsShapeGroup2D physicsShapeGroup, int shapeIndex, [UnityEngine.Internal.DefaultValue("1")] int shapeCount = 1)
	{
		int num = this.shapeCount;
		if (shapeIndex < 0 || shapeIndex >= num || shapeCount < 1 || shapeIndex + shapeCount > num)
		{
			throw new ArgumentOutOfRangeException($"Cannot get shape range from {shapeIndex} to {shapeIndex + shapeCount - 1} as Collider2D only has {num} shape(s).");
		}
		return GetShapes_Internal(ref physicsShapeGroup.m_GroupState, shapeIndex, shapeCount);
	}

	[NativeMethod("GetShapes_Binding")]
	private int GetShapes_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetShapes_Internal_Injected(intPtr, ref physicsShapeGroupState, shapeIndex, shapeCount);
	}

	[NativeMethod("GetShapeBounds_Binding")]
	public unsafe Bounds GetShapeBounds(List<Bounds> bounds, bool useRadii, bool useWorldSpace)
	{
		//The blocks IL_0041 are reachable both inside and outside the pinned region starting at IL_001d. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<Bounds> list = default(List<Bounds>);
		BlittableListWrapper blittableListWrapper = default(BlittableListWrapper);
		Bounds ret = default(Bounds);
		Bounds result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = bounds;
			if (list != null)
			{
				fixed (Bounds[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					blittableListWrapper = new BlittableListWrapper(arrayWrapper, list.Count);
					GetShapeBounds_Injected(intPtr, ref blittableListWrapper, useRadii, useWorldSpace, out ret);
				}
			}
			else
			{
				GetShapeBounds_Injected(intPtr, ref blittableListWrapper, useRadii, useWorldSpace, out ret);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
			result = ret;
		}
		return result;
	}

	[NativeMethod("CanContact_Binding")]
	public bool CanContact([NotNull] Collider2D collider)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return CanContact_Injected(intPtr, intPtr2);
	}

	public bool IsTouching([NotNull] Collider2D collider)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouching_Injected(intPtr, intPtr2);
	}

	public bool IsTouching(Collider2D collider, ContactFilter2D contactFilter)
	{
		return IsTouching_OtherColliderWithFilter(collider, contactFilter);
	}

	[NativeMethod("IsTouching")]
	private bool IsTouching_OtherColliderWithFilter([NotNull] Collider2D collider, ContactFilter2D contactFilter)
	{
		if ((object)collider == null)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(collider);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(collider, "collider");
		}
		return IsTouching_OtherColliderWithFilter_Injected(intPtr, intPtr2, ref contactFilter);
	}

	public bool IsTouching(ContactFilter2D contactFilter)
	{
		return IsTouching_AnyColliderWithFilter(contactFilter);
	}

	[NativeMethod("IsTouching")]
	private bool IsTouching_AnyColliderWithFilter(ContactFilter2D contactFilter)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsTouching_AnyColliderWithFilter_Injected(intPtr, ref contactFilter);
	}

	[ExcludeFromDocs]
	public bool IsTouchingLayers()
	{
		return IsTouchingLayers(-1);
	}

	public bool IsTouchingLayers([UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsTouchingLayers_Injected(intPtr, layerMask);
	}

	public bool OverlapPoint(Vector2 point)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return OverlapPoint_Injected(intPtr, ref point);
	}

	public int Overlap(ContactFilter2D contactFilter, Collider2D[] results)
	{
		return PhysicsScene2D.OverlapCollider(this, contactFilter, results);
	}

	public int Overlap(List<Collider2D> results)
	{
		return PhysicsScene2D.OverlapCollider(this, results);
	}

	public int Overlap(ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return PhysicsScene2D.OverlapCollider(this, contactFilter, results);
	}

	public int Overlap(Vector2 position, float angle, List<Collider2D> results)
	{
		if ((bool)attachedRigidbody)
		{
			return PhysicsScene2D.OverlapCollider(position, angle, this, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	public int Overlap(Vector2 position, float angle, ContactFilter2D contactFilter, List<Collider2D> results)
	{
		if ((bool)attachedRigidbody)
		{
			return PhysicsScene2D.OverlapCollider(position, angle, this, contactFilter, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Overlap at a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = default(ContactFilter2D);
		contactFilter.useTriggers = Physics2D.queriesHitTriggers;
		contactFilter.SetLayerMask(contactMask);
		return CastArray_Internal(direction, float.PositiveInfinity, contactFilter, ignoreSiblingColliders: true, results);
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = default(ContactFilter2D);
		contactFilter.useTriggers = Physics2D.queriesHitTriggers;
		contactFilter.SetLayerMask(contactMask);
		return CastArray_Internal(direction, distance, contactFilter, ignoreSiblingColliders: true, results);
	}

	public int Cast(Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders)
	{
		ContactFilter2D contactFilter = default(ContactFilter2D);
		contactFilter.useTriggers = Physics2D.queriesHitTriggers;
		contactFilter.SetLayerMask(contactMask);
		return CastArray_Internal(direction, distance, contactFilter, ignoreSiblingColliders, results);
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return CastArray_Internal(direction, float.PositiveInfinity, contactFilter, ignoreSiblingColliders: true, results);
	}

	[ExcludeFromDocs]
	public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, float distance)
	{
		return CastArray_Internal(direction, distance, contactFilter, ignoreSiblingColliders: true, results);
	}

	public int Cast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders)
	{
		return CastArray_Internal(direction, distance, contactFilter, ignoreSiblingColliders, results);
	}

	public int Cast(Vector2 direction, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders = true)
	{
		return CastList_Internal(direction, distance, ignoreSiblingColliders, results);
	}

	public int Cast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders = true)
	{
		return CastListFiltered_Internal(direction, distance, contactFilter, ignoreSiblingColliders, results);
	}

	public int Cast(Vector2 position, float angle, Vector2 direction, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders = true)
	{
		if ((bool)attachedRigidbody)
		{
			return CastFrom_Internal(position, angle, direction, distance, ignoreSiblingColliders, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Cast from a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	public int Cast(Vector2 position, float angle, Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity, [UnityEngine.Internal.DefaultValue("true")] bool ignoreSiblingColliders = true)
	{
		if ((bool)attachedRigidbody)
		{
			return CastFromFiltered_Internal(position, angle, direction, distance, contactFilter, ignoreSiblingColliders, results);
		}
		throw new InvalidOperationException("Cannot perform a Collider Cast from a specific position and angle if the Collider is not attached to a Rigidbody2D.");
	}

	[NativeMethod("CastArray_Binding")]
	private unsafe int CastArray_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = CastArray_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ignoreSiblingColliders, ref results2);
		}
		return result;
	}

	[NativeMethod("CastList_Binding")]
	private unsafe int CastList_Internal(Vector2 direction, float distance, bool ignoreSiblingColliders, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastList_Internal_Injected(intPtr, ref direction, distance, ignoreSiblingColliders, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastListFiltered_Binding")]
	private unsafe int CastListFiltered_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastListFiltered_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ignoreSiblingColliders, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastFrom_Binding")]
	private unsafe int CastFrom_Internal(Vector2 position, float angle, Vector2 direction, float distance, bool ignoreSiblingColliders, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastFrom_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ignoreSiblingColliders, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[NativeMethod("CastFromFiltered_Binding")]
	private unsafe int CastFromFiltered_Internal(Vector2 position, float angle, Vector2 direction, float distance, ContactFilter2D contactFilter, bool ignoreSiblingColliders, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return CastFromFiltered_Internal_Injected(intPtr, ref position, angle, ref direction, distance, ref contactFilter, ignoreSiblingColliders, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	[ExcludeFromDocs]
	public int Raycast(Vector2 direction, RaycastHit2D[] results)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastArray_Internal(direction, float.PositiveInfinity, contactFilter, results);
	}

	[ExcludeFromDocs]
	public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(-1, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastArray_Internal(direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, float.NegativeInfinity, float.PositiveInfinity);
		return RaycastArray_Internal(direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public int Raycast(Vector2 direction, RaycastHit2D[] results, float distance, int layerMask, float minDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, float.PositiveInfinity);
		return RaycastArray_Internal(direction, distance, contactFilter, results);
	}

	public int Raycast(Vector2 direction, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask, [UnityEngine.Internal.DefaultValue("-Mathf.Infinity")] float minDepth, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float maxDepth)
	{
		ContactFilter2D contactFilter = ContactFilter2D.CreateLegacyFilter(layerMask, minDepth, maxDepth);
		return RaycastArray_Internal(direction, distance, contactFilter, results);
	}

	[ExcludeFromDocs]
	public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results)
	{
		return RaycastArray_Internal(direction, float.PositiveInfinity, contactFilter, results);
	}

	public int Raycast(Vector2 direction, ContactFilter2D contactFilter, RaycastHit2D[] results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance)
	{
		return RaycastArray_Internal(direction, distance, contactFilter, results);
	}

	[NativeMethod("RaycastArray_Binding")]
	private unsafe int RaycastArray_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] RaycastHit2D[] results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<RaycastHit2D> span = new Span<RaycastHit2D>(results);
		int result;
		fixed (RaycastHit2D* begin = span)
		{
			ManagedSpanWrapper results2 = new ManagedSpanWrapper(begin, span.Length);
			result = RaycastArray_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref results2);
		}
		return result;
	}

	public int Raycast(Vector2 direction, ContactFilter2D contactFilter, List<RaycastHit2D> results, [UnityEngine.Internal.DefaultValue("Mathf.Infinity")] float distance = float.PositiveInfinity)
	{
		return RaycastList_Internal(direction, distance, contactFilter, results);
	}

	[NativeMethod("RaycastList_Binding")]
	private unsafe int RaycastList_Internal(Vector2 direction, float distance, ContactFilter2D contactFilter, [NotNull] List<RaycastHit2D> results)
	{
		if (results == null)
		{
			ThrowHelper.ThrowArgumentNullException(results, "results");
		}
		List<RaycastHit2D> list = default(List<RaycastHit2D>);
		BlittableListWrapper results2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = results;
			fixed (RaycastHit2D[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				results2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return RaycastList_Internal_Injected(intPtr, ref direction, distance, ref contactFilter, ref results2);
			}
		}
		finally
		{
			results2.Unmarshal(list);
		}
	}

	public ColliderDistance2D Distance(Collider2D collider)
	{
		return Physics2D.Distance(this, collider);
	}

	public ColliderDistance2D Distance(Vector2 thisPosition, float thisAngle, Collider2D collider, Vector2 position, float angle)
	{
		return Physics2D.Distance(this, thisPosition, thisAngle, collider, position, angle);
	}

	public Vector2 ClosestPoint(Vector2 position)
	{
		return Physics2D.ClosestPoint(position, this);
	}

	public int GetContacts(ContactPoint2D[] contacts)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
	}

	public int GetContacts(List<ContactPoint2D> contacts)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, contacts);
	}

	public int GetContacts(ContactFilter2D contactFilter, ContactPoint2D[] contacts)
	{
		return Physics2D.GetContacts(this, contactFilter, contacts);
	}

	public int GetContacts(ContactFilter2D contactFilter, List<ContactPoint2D> contacts)
	{
		return Physics2D.GetContacts(this, contactFilter, contacts);
	}

	public int GetContacts(Collider2D[] colliders)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
	}

	public int GetContacts(List<Collider2D> colliders)
	{
		return Physics2D.GetContacts(this, ContactFilter2D.noFilter, colliders);
	}

	public int GetContacts(ContactFilter2D contactFilter, Collider2D[] colliders)
	{
		return Physics2D.GetContacts(this, contactFilter, colliders);
	}

	public int GetContacts(ContactFilter2D contactFilter, List<Collider2D> colliders)
	{
		return Physics2D.GetContacts(this, contactFilter, colliders);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCollider has been deprecated. Please use Overlap. (UnityUpgradable) -> Overlap(*)", false)]
	[ExcludeFromDocs]
	public int OverlapCollider(ContactFilter2D contactFilter, Collider2D[] results)
	{
		return Overlap(contactFilter, results);
	}

	[ExcludeFromDocs]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("OverlapCollider has been deprecated. Please use Overlap. (UnityUpgradable) -> Overlap(*)", false)]
	public int OverlapCollider(ContactFilter2D contactFilter, List<Collider2D> results)
	{
		return Overlap(contactFilter, results);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_density_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_density_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isTrigger_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_isTrigger_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_usedByEffector_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_usedByEffector_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern CompositeOperation get_compositeOperation_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_compositeOperation_Injected(IntPtr _unity_self, CompositeOperation value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_compositeOrder_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_compositeOrder_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_composite_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_offset_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_offset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_attachedRigidbody_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_localToWorldMatrix_Injected(IntPtr _unity_self, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_shapeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateMesh_Injected(IntPtr _unity_self, bool useBodyPosition, bool useBodyRotation, [UnityEngine.Internal.DefaultValue("true")] bool useDelaunay);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetShapeHash_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetShapes_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetShapeBounds_Injected(IntPtr _unity_self, ref BlittableListWrapper bounds, bool useRadii, bool useWorldSpace, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ColliderErrorState2D get_errorState_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_compositeCapable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMaterial_Injected(IntPtr _unity_self, IntPtr value);

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
	private static extern void get_forceSendLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceSendLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_forceReceiveLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceReceiveLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_contactCaptureLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_contactCaptureLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_callbackLayers_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_callbackLayers_Injected(IntPtr _unity_self, [In] ref LayerMask value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_friction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_bounciness_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsMaterialCombine2D get_frictionCombine_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PhysicsMaterialCombine2D get_bounceCombine_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_contactMask_Injected(IntPtr _unity_self, out LayerMask ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CanContact_Injected(IntPtr _unity_self, IntPtr collider);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_Injected(IntPtr _unity_self, IntPtr collider);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_OtherColliderWithFilter_Injected(IntPtr _unity_self, IntPtr collider, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouching_AnyColliderWithFilter_Injected(IntPtr _unity_self, [In] ref ContactFilter2D contactFilter);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTouchingLayers_Injected(IntPtr _unity_self, [UnityEngine.Internal.DefaultValue("Physics2D.AllLayers")] int layerMask);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool OverlapPoint_Injected(IntPtr _unity_self, [In] ref Vector2 point);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, bool ignoreSiblingColliders, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastListFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFrom_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, bool ignoreSiblingColliders, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int CastFromFiltered_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 position, float angle, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, bool ignoreSiblingColliders, ref BlittableListWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastArray_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref ManagedSpanWrapper results);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int RaycastList_Internal_Injected(IntPtr _unity_self, [In] ref Vector2 direction, float distance, [In] ref ContactFilter2D contactFilter, ref BlittableListWrapper results);
}
