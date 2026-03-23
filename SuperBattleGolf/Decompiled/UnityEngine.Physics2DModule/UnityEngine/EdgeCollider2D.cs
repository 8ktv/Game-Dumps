using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Physics2D/Public/EdgeCollider2D.h")]
public sealed class EdgeCollider2D : Collider2D
{
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

	public int edgeCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_edgeCount_Injected(intPtr);
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

	public unsafe Vector2[] points
	{
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

	public bool useAdjacentStartPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useAdjacentStartPoint_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useAdjacentStartPoint_Injected(intPtr, value);
		}
	}

	public bool useAdjacentEndPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useAdjacentEndPoint_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useAdjacentEndPoint_Injected(intPtr, value);
		}
	}

	public Vector2 adjacentStartPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_adjacentStartPoint_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_adjacentStartPoint_Injected(intPtr, ref value);
		}
	}

	public Vector2 adjacentEndPoint
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_adjacentEndPoint_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_adjacentEndPoint_Injected(intPtr, ref value);
		}
	}

	public void Reset()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Reset_Injected(intPtr);
	}

	[NativeMethod("GetPoints_Binding")]
	public unsafe int GetPoints([NotNull] List<Vector2> points)
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
				return GetPoints_Injected(intPtr, ref blittableListWrapper);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	[NativeMethod("SetPoints_Binding")]
	public unsafe bool SetPoints([NotNull] List<Vector2> points)
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
				return SetPoints_Injected(intPtr, ref blittableListWrapper);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Reset_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_edgeRadius_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_edgeRadius_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_edgeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_pointCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_points_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_points_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPoints_Injected(IntPtr _unity_self, ref BlittableListWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool SetPoints_Injected(IntPtr _unity_self, ref BlittableListWrapper points);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useAdjacentStartPoint_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useAdjacentStartPoint_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useAdjacentEndPoint_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useAdjacentEndPoint_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_adjacentStartPoint_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_adjacentStartPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_adjacentEndPoint_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_adjacentEndPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);
}
