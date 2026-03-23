using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/CustomCollider2D.h")]
public sealed class CustomCollider2D : Collider2D
{
	[NativeMethod("CustomShapeCount_Binding")]
	public int customShapeCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_customShapeCount_Injected(intPtr);
		}
	}

	[NativeMethod("CustomVertexCount_Binding")]
	public int customVertexCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_customVertexCount_Injected(intPtr);
		}
	}

	public int GetCustomShapes(PhysicsShapeGroup2D physicsShapeGroup)
	{
		int num = customShapeCount;
		if (num > 0)
		{
			return GetCustomShapes_Internal(ref physicsShapeGroup.m_GroupState, 0, num);
		}
		physicsShapeGroup.Clear();
		return 0;
	}

	public int GetCustomShapes(PhysicsShapeGroup2D physicsShapeGroup, int shapeIndex, [DefaultValue("1")] int shapeCount = 1)
	{
		int num = customShapeCount;
		if (shapeIndex < 0 || shapeIndex >= num || shapeCount < 1 || shapeIndex + shapeCount > num)
		{
			throw new ArgumentOutOfRangeException($"Cannot get shape range from {shapeIndex} to {shapeIndex + shapeCount - 1} as CustomCollider2D only has {num} shape(s).");
		}
		return GetCustomShapes_Internal(ref physicsShapeGroup.m_GroupState, shapeIndex, shapeCount);
	}

	[NativeMethod("GetCustomShapes_Binding")]
	private int GetCustomShapes_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCustomShapes_Internal_Injected(intPtr, ref physicsShapeGroupState, shapeIndex, shapeCount);
	}

	public unsafe int GetCustomShapes(NativeArray<PhysicsShape2D> shapes, NativeArray<Vector2> vertices)
	{
		if (!shapes.IsCreated || shapes.Length != customShapeCount)
		{
			throw new ArgumentException($"Cannot get custom shapes as the native shapes array length must be identical to the current custom shape count of {customShapeCount}.", "shapes");
		}
		if (!vertices.IsCreated || vertices.Length != customVertexCount)
		{
			throw new ArgumentException($"Cannot get custom shapes as the native vertices array length must be identical to the current custom vertex count of {customVertexCount}.", "vertices");
		}
		return GetCustomShapesNative_Internal((IntPtr)shapes.GetUnsafeReadOnlyPtr(), shapes.Length, (IntPtr)vertices.GetUnsafeReadOnlyPtr(), vertices.Length);
	}

	[NativeMethod("GetCustomShapesAllNative_Binding")]
	private int GetCustomShapesNative_Internal(IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCustomShapesNative_Internal_Injected(intPtr, shapesPtr, shapeCount, verticesPtr, vertexCount);
	}

	public void SetCustomShapes(PhysicsShapeGroup2D physicsShapeGroup)
	{
		if (physicsShapeGroup.shapeCount > 0)
		{
			SetCustomShapesAll_Internal(ref physicsShapeGroup.m_GroupState);
		}
		else
		{
			ClearCustomShapes();
		}
	}

	[NativeMethod("SetCustomShapesAll_Binding")]
	private void SetCustomShapesAll_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetCustomShapesAll_Internal_Injected(intPtr, ref physicsShapeGroupState);
	}

	public unsafe void SetCustomShapes(NativeArray<PhysicsShape2D> shapes, NativeArray<Vector2> vertices)
	{
		if (!shapes.IsCreated || shapes.Length == 0)
		{
			throw new ArgumentException("Cannot set custom shapes as the native shapes array is empty.", "shapes");
		}
		if (!vertices.IsCreated || vertices.Length == 0)
		{
			throw new ArgumentException("Cannot set custom shapes as the native vertices array is empty.", "vertices");
		}
		SetCustomShapesNative_Internal((IntPtr)shapes.GetUnsafeReadOnlyPtr(), shapes.Length, (IntPtr)vertices.GetUnsafeReadOnlyPtr(), vertices.Length);
	}

	[NativeMethod("SetCustomShapesAllNative_Binding", ThrowsException = true)]
	private void SetCustomShapesNative_Internal(IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetCustomShapesNative_Internal_Injected(intPtr, shapesPtr, shapeCount, verticesPtr, vertexCount);
	}

	public void SetCustomShape(PhysicsShapeGroup2D physicsShapeGroup, int srcShapeIndex, int dstShapeIndex)
	{
		if (srcShapeIndex < 0 || srcShapeIndex >= physicsShapeGroup.shapeCount)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at {srcShapeIndex} as the shape group only has {physicsShapeGroup.shapeCount} shape(s).");
		}
		PhysicsShape2D shape = physicsShapeGroup.GetShape(srcShapeIndex);
		if (shape.vertexStartIndex < 0 || shape.vertexStartIndex >= physicsShapeGroup.vertexCount || shape.vertexCount < 1 || shape.vertexStartIndex + shape.vertexCount > physicsShapeGroup.vertexCount)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at {srcShapeIndex} as its shape indices are out of the available vertices ranges.");
		}
		if (dstShapeIndex < 0 || dstShapeIndex >= customShapeCount)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at destination {dstShapeIndex} as CustomCollider2D only has {customShapeCount} custom shape(s). The destination index must be within the existing shape range.");
		}
		SetCustomShape_Internal(ref physicsShapeGroup.m_GroupState, srcShapeIndex, dstShapeIndex);
	}

	[NativeMethod("SetCustomShape_Binding")]
	private void SetCustomShape_Internal(ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int srcShapeIndex, int dstShapeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetCustomShape_Internal_Injected(intPtr, ref physicsShapeGroupState, srcShapeIndex, dstShapeIndex);
	}

	public unsafe void SetCustomShape(NativeArray<PhysicsShape2D> shapes, NativeArray<Vector2> vertices, int srcShapeIndex, int dstShapeIndex)
	{
		if (!shapes.IsCreated || shapes.Length == 0)
		{
			throw new ArgumentException("Cannot set custom shapes as the native shapes array is empty.", "shapes");
		}
		if (!vertices.IsCreated || vertices.Length == 0)
		{
			throw new ArgumentException("Cannot set custom shapes as the native vertices array is empty.", "vertices");
		}
		if (srcShapeIndex < 0 || srcShapeIndex >= shapes.Length)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at {srcShapeIndex} as the shape native array only has {shapes.Length} shape(s).");
		}
		PhysicsShape2D physicsShape2D = shapes[srcShapeIndex];
		if (physicsShape2D.vertexStartIndex < 0 || physicsShape2D.vertexStartIndex >= vertices.Length || physicsShape2D.vertexCount < 1 || physicsShape2D.vertexStartIndex + physicsShape2D.vertexCount > vertices.Length)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at {srcShapeIndex} as its shape indices are out of the available vertices ranges.");
		}
		if (dstShapeIndex < 0 || dstShapeIndex >= customShapeCount)
		{
			throw new ArgumentOutOfRangeException($"Cannot set custom shape at destination {dstShapeIndex} as CustomCollider2D only has {customShapeCount} custom shape(s). The destination index must be within the existing shape range.");
		}
		SetCustomShapeNative_Internal((IntPtr)shapes.GetUnsafeReadOnlyPtr(), shapes.Length, (IntPtr)vertices.GetUnsafeReadOnlyPtr(), vertices.Length, srcShapeIndex, dstShapeIndex);
	}

	[NativeMethod("SetCustomShapeNative_Binding", ThrowsException = true)]
	private void SetCustomShapeNative_Internal(IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount, int srcShapeIndex, int dstShapeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetCustomShapeNative_Internal_Injected(intPtr, shapesPtr, shapeCount, verticesPtr, vertexCount, srcShapeIndex, dstShapeIndex);
	}

	public void ClearCustomShapes(int shapeIndex, int shapeCount)
	{
		int num = customShapeCount;
		if (shapeIndex < 0 || shapeIndex >= num)
		{
			throw new ArgumentOutOfRangeException($"Cannot clear custom shape(s) at index {shapeIndex} as the CustomCollider2D only has {num} shape(s).");
		}
		if (shapeIndex + shapeCount < 0 || shapeIndex + shapeCount > customShapeCount)
		{
			throw new ArgumentOutOfRangeException($"Cannot clear custom shape(s) in the range (index {shapeIndex}, count {shapeCount}) as this range is outside range of the existing {customShapeCount} shape(s).");
		}
		ClearCustomShapes_Internal(shapeIndex, shapeCount);
	}

	[NativeMethod("ClearCustomShapes_Binding")]
	private void ClearCustomShapes_Internal(int shapeIndex, int shapeCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearCustomShapes_Internal_Injected(intPtr, shapeIndex, shapeCount);
	}

	[NativeMethod("ClearCustomShapes_Binding")]
	public void ClearCustomShapes()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearCustomShapes_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_customShapeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_customVertexCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCustomShapes_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int shapeIndex, int shapeCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCustomShapesNative_Internal_Injected(IntPtr _unity_self, IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomShapesAll_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomShapesNative_Internal_Injected(IntPtr _unity_self, IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomShape_Internal_Injected(IntPtr _unity_self, ref PhysicsShapeGroup2D.GroupState physicsShapeGroupState, int srcShapeIndex, int dstShapeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomShapeNative_Internal_Injected(IntPtr _unity_self, IntPtr shapesPtr, int shapeCount, IntPtr verticesPtr, int vertexCount, int srcShapeIndex, int dstShapeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearCustomShapes_Internal_Injected(IntPtr _unity_self, int shapeIndex, int shapeCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearCustomShapes_Injected(IntPtr _unity_self);
}
