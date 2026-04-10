using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/CompositeCollider2D.h")]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class CompositeCollider2D : Collider2D
{
	public enum GeometryType
	{
		Outlines,
		Polygons
	}

	public enum GenerationType
	{
		Synchronous,
		Manual
	}

	public GeometryType geometryType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_geometryType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_geometryType_Injected(intPtr, value);
		}
	}

	public GenerationType generationType
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_generationType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_generationType_Injected(intPtr, value);
		}
	}

	public bool useDelaunayMesh
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useDelaunayMesh_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useDelaunayMesh_Injected(intPtr, value);
		}
	}

	public float vertexDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vertexDistance_Injected(intPtr, value);
		}
	}

	public float edgeRadius
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_edgeRadius_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_edgeRadius_Injected(intPtr, value);
		}
	}

	public float offsetDistance
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_offsetDistance_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_offsetDistance_Injected(intPtr, value);
		}
	}

	public int pathCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pathCount_Injected(intPtr);
		}
	}

	public int pointCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pointCount_Injected(intPtr);
		}
	}

	public void GenerateGeometry()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GenerateGeometry_Injected(intPtr);
	}

	[NativeMethod("GetCompositedColliders_Binding")]
	public int GetCompositedColliders([NotNull] List<Collider2D> colliders)
	{
		if (colliders == null)
		{
			ThrowHelper.ThrowArgumentNullException(colliders, "colliders");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetCompositedColliders_Injected(intPtr, colliders);
	}

	public int GetPathPointCount(int index)
	{
		int num = pathCount - 1;
		if (index < 0 || index > num)
		{
			throw new ArgumentOutOfRangeException("index", $"Path index {index} must be in the range of 0 to {num}.");
		}
		return GetPathPointCount_Internal(index);
	}

	[NativeMethod("GetPathPointCount_Binding")]
	private int GetPathPointCount_Internal(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPathPointCount_Internal_Injected(intPtr, index);
	}

	public int GetPath(int index, Vector2[] points)
	{
		if (index < 0 || index >= pathCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Path index {index} must be in the range of 0 to {pathCount - 1}.");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		return GetPathArray_Internal(index, points);
	}

	[NativeMethod("GetPathArray_Binding")]
	private unsafe int GetPathArray_Internal(int index, [NotNull] Vector2[] points)
	{
		if (points == null)
		{
			ThrowHelper.ThrowArgumentNullException(points, "points");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector2> span = new Span<Vector2>(points);
		int pathArray_Internal_Injected;
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper points2 = new ManagedSpanWrapper(begin, span.Length);
			pathArray_Internal_Injected = GetPathArray_Internal_Injected(intPtr, index, ref points2);
		}
		return pathArray_Internal_Injected;
	}

	public int GetPath(int index, List<Vector2> points)
	{
		if (index < 0 || index >= pathCount)
		{
			throw new ArgumentOutOfRangeException("index", $"Path index {index} must be in the range of 0 to {pathCount - 1}.");
		}
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		return GetPathList_Internal(index, points);
	}

	[NativeMethod("GetPathList_Binding")]
	private unsafe int GetPathList_Internal(int index, [NotNull] List<Vector2> points)
	{
		if (points == null)
		{
			ThrowHelper.ThrowArgumentNullException(points, "points");
		}
		List<Vector2> list = default(List<Vector2>);
		BlittableListWrapper points2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = points;
			fixed (Vector2[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				points2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetPathList_Internal_Injected(intPtr, index, ref points2);
			}
		}
		finally
		{
			points2.Unmarshal(list);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GeometryType get_geometryType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_geometryType_Injected(IntPtr _unity_self, GeometryType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GenerationType get_generationType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_generationType_Injected(IntPtr _unity_self, GenerationType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useDelaunayMesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useDelaunayMesh_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_vertexDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vertexDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_edgeRadius_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_edgeRadius_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_offsetDistance_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_offsetDistance_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GenerateGeometry_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCompositedColliders_Injected(IntPtr _unity_self, List<Collider2D> colliders);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPathPointCount_Internal_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pathCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pointCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPathArray_Internal_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);
}
