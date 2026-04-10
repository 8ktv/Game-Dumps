using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/PolygonCollider2D.h")]
public sealed class PolygonCollider2D : Collider2D
{
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

	public bool autoTiling
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_autoTiling_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_autoTiling_Injected(intPtr, value);
		}
	}

	public unsafe Vector2[] points
	{
		[NativeMethod("GetPoints_Binding")]
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Vector2[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_points_Injected(intPtr, out ret);
			}
			finally
			{
				Vector2[] array = default(Vector2[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		[NativeMethod("SetPoints_Binding")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector2> span = new Span<Vector2>(value);
			fixed (Vector2* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_points_Injected(intPtr, ref value2);
			}
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
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_pathCount_Injected(intPtr, value);
		}
	}

	[NativeMethod("GetPointCount")]
	public int GetTotalPointCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTotalPointCount_Injected(intPtr);
	}

	public Vector2[] GetPath(int index)
	{
		if (index >= pathCount)
		{
			throw new ArgumentOutOfRangeException($"Path {index} does not exist.");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException($"Path {index} does not exist; negative path index is invalid.");
		}
		return GetPath_Internal(index);
	}

	[NativeMethod("GetPath_Binding")]
	private Vector2[] GetPath_Internal(int index)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector2[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetPath_Internal_Injected(intPtr, index, out ret);
		}
		finally
		{
			Vector2[] array = default(Vector2[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	public void SetPath(int index, Vector2[] points)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException($"Negative path index {index} is invalid.");
		}
		SetPath_Internal(index, points);
	}

	[NativeMethod("SetPath_Binding")]
	private unsafe void SetPath_Internal(int index, [NotNull] Vector2[] points)
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
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, span.Length);
			SetPath_Internal_Injected(intPtr, index, ref managedSpanWrapper);
		}
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
		BlittableListWrapper blittableListWrapper = default(BlittableListWrapper);
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
				blittableListWrapper = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetPathList_Internal_Injected(intPtr, index, ref blittableListWrapper);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	public void SetPath(int index, List<Vector2> points)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException($"Negative path index {index} is invalid.");
		}
		SetPathList_Internal(index, points);
	}

	[NativeMethod("SetPathList_Binding")]
	private unsafe void SetPathList_Internal(int index, [NotNull] List<Vector2> points)
	{
		if (points == null)
		{
			ThrowHelper.ThrowArgumentNullException(points, "points");
		}
		List<Vector2> list = default(List<Vector2>);
		BlittableListWrapper blittableListWrapper = default(BlittableListWrapper);
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
				blittableListWrapper = new BlittableListWrapper(arrayWrapper, list.Count);
				SetPathList_Internal_Injected(intPtr, index, ref blittableListWrapper);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	[ExcludeFromDocs]
	public void CreatePrimitive(int sides)
	{
		CreatePrimitive(sides, Vector2.one, Vector2.zero);
	}

	[ExcludeFromDocs]
	public void CreatePrimitive(int sides, Vector2 scale)
	{
		CreatePrimitive(sides, scale, Vector2.zero);
	}

	public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
	{
		if (sides < 3)
		{
			Debug.LogWarning("Cannot create a 2D polygon primitive collider with less than two sides.", this);
		}
		else if (!(scale.x > 0f) || !(scale.y > 0f))
		{
			Debug.LogWarning("Cannot create a 2D polygon primitive collider with an axis scale less than or equal to zero.", this);
		}
		else
		{
			CreatePrimitive_Internal(sides, scale, offset, recreateCollider: true);
		}
	}

	[NativeMethod("CreatePrimitive")]
	private void CreatePrimitive_Internal(int sides, Vector2 scale, Vector2 offset, bool recreateCollider)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CreatePrimitive_Internal_Injected(intPtr, sides, ref scale, ref offset, recreateCollider);
	}

	public bool CreateFromSprite(Sprite sprite, [DefaultValue("0.25f")] float detail = 0.25f, [DefaultValue("200")] byte alphaTolerance = 200, [DefaultValue("true")] bool holeDetection = true, [DefaultValue("true")] bool usePhysicsShapes = true)
	{
		if (sprite == null)
		{
			Debug.LogWarning("Sprite cannot be NULL.", this);
			return false;
		}
		if (detail < 0f || detail > 1f)
		{
			Debug.LogWarning("Detail must be in the range [0, 1].", this);
			return false;
		}
		return CreateFromSprite_Internal(sprite, detail, alphaTolerance, holeDetection, recreateCollider: true, usePhysicsShapes);
	}

	[NativeMethod("CreateFromSprite")]
	private bool CreateFromSprite_Internal([NotNull] Sprite sprite, float detail, byte alphaTolerance, bool holeDetection, bool recreateCollider, bool usePhysicsShapes)
	{
		if ((object)sprite == null)
		{
			ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(sprite);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
		}
		return CreateFromSprite_Internal_Injected(intPtr, intPtr2, detail, alphaTolerance, holeDetection, recreateCollider, usePhysicsShapes);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useDelaunayMesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useDelaunayMesh_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_autoTiling_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_autoTiling_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetTotalPointCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_points_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_points_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pathCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_pathCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPath_Internal_Injected(IntPtr _unity_self, int index, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPath_Internal_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPathList_Internal_Injected(IntPtr _unity_self, int index, ref BlittableListWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreatePrimitive_Internal_Injected(IntPtr _unity_self, int sides, [In] ref Vector2 scale, [In] ref Vector2 offset, bool recreateCollider);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateFromSprite_Internal_Injected(IntPtr _unity_self, IntPtr sprite, float detail, byte alphaTolerance, bool holeDetection, bool recreateCollider, bool usePhysicsShapes);
}
