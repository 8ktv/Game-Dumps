using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Billboard/BillboardAsset.h")]
[NativeHeader("Runtime/Export/Graphics/BillboardRenderer.bindings.h")]
public sealed class BillboardAsset : Object
{
	public float width
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_width_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_width_Injected(intPtr, value);
		}
	}

	public float height
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_height_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_height_Injected(intPtr, value);
		}
	}

	public float bottom
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bottom_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bottom_Injected(intPtr, value);
		}
	}

	public int imageCount
	{
		[NativeMethod("GetNumImages")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_imageCount_Injected(intPtr);
		}
	}

	public int vertexCount
	{
		[NativeMethod("GetNumVertices")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexCount_Injected(intPtr);
		}
	}

	public int indexCount
	{
		[NativeMethod("GetNumIndices")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_indexCount_Injected(intPtr);
		}
	}

	public Material material
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_material_Injected(intPtr));
		}
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

	public BillboardAsset()
	{
		Internal_Create(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "BillboardRenderer_Bindings::Internal_Create")]
	private static extern void Internal_Create([Writable] BillboardAsset obj);

	public void GetImageTexCoords(List<Vector4> imageTexCoords)
	{
		if (imageTexCoords == null)
		{
			throw new ArgumentNullException("imageTexCoords");
		}
		GetImageTexCoordsInternal(imageTexCoords);
	}

	[NativeMethod("GetBillboardDataReadonly().GetImageTexCoords")]
	public Vector4[] GetImageTexCoords()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector4[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetImageTexCoords_Injected(intPtr, out ret);
		}
		finally
		{
			Vector4[] array = default(Vector4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::GetImageTexCoordsInternal", HasExplicitThis = true)]
	internal void GetImageTexCoordsInternal(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetImageTexCoordsInternal_Injected(intPtr, list);
	}

	public void SetImageTexCoords(List<Vector4> imageTexCoords)
	{
		if (imageTexCoords == null)
		{
			throw new ArgumentNullException("imageTexCoords");
		}
		SetImageTexCoordsInternalList(imageTexCoords);
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetImageTexCoords", HasExplicitThis = true)]
	public unsafe void SetImageTexCoords([NotNull] Vector4[] imageTexCoords)
	{
		if (imageTexCoords == null)
		{
			ThrowHelper.ThrowArgumentNullException(imageTexCoords, "imageTexCoords");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector4> span = new Span<Vector4>(imageTexCoords);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper imageTexCoords2 = new ManagedSpanWrapper(begin, span.Length);
			SetImageTexCoords_Injected(intPtr, ref imageTexCoords2);
		}
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetImageTexCoordsInternalList", HasExplicitThis = true)]
	internal void SetImageTexCoordsInternalList(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetImageTexCoordsInternalList_Injected(intPtr, list);
	}

	public void GetVertices(List<Vector2> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices");
		}
		GetVerticesInternal(vertices);
	}

	[NativeMethod("GetBillboardDataReadonly().GetVertices")]
	public Vector2[] GetVertices()
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
			GetVertices_Injected(intPtr, out ret);
		}
		finally
		{
			Vector2[] array = default(Vector2[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::GetVerticesInternal", HasExplicitThis = true)]
	internal void GetVerticesInternal(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetVerticesInternal_Injected(intPtr, list);
	}

	public void SetVertices(List<Vector2> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices");
		}
		SetVerticesInternalList(vertices);
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetVertices", HasExplicitThis = true)]
	public unsafe void SetVertices([NotNull] Vector2[] vertices)
	{
		if (vertices == null)
		{
			ThrowHelper.ThrowArgumentNullException(vertices, "vertices");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector2> span = new Span<Vector2>(vertices);
		fixed (Vector2* begin = span)
		{
			ManagedSpanWrapper vertices2 = new ManagedSpanWrapper(begin, span.Length);
			SetVertices_Injected(intPtr, ref vertices2);
		}
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetVerticesInternalList", HasExplicitThis = true)]
	internal void SetVerticesInternalList(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVerticesInternalList_Injected(intPtr, list);
	}

	public void GetIndices(List<ushort> indices)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		GetIndicesInternal(indices);
	}

	[NativeMethod("GetBillboardDataReadonly().GetIndices")]
	public ushort[] GetIndices()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		ushort[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetIndices_Injected(intPtr, out ret);
		}
		finally
		{
			ushort[] array = default(ushort[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::GetIndicesInternal", HasExplicitThis = true)]
	internal void GetIndicesInternal(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetIndicesInternal_Injected(intPtr, list);
	}

	public void SetIndices(List<ushort> indices)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices");
		}
		SetIndicesInternalList(indices);
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetIndices", HasExplicitThis = true)]
	public unsafe void SetIndices([NotNull] ushort[] indices)
	{
		if (indices == null)
		{
			ThrowHelper.ThrowArgumentNullException(indices, "indices");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<ushort> span = new Span<ushort>(indices);
		fixed (ushort* begin = span)
		{
			ManagedSpanWrapper indices2 = new ManagedSpanWrapper(begin, span.Length);
			SetIndices_Injected(intPtr, ref indices2);
		}
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::SetIndicesInternalList", HasExplicitThis = true)]
	internal void SetIndicesInternalList(object list)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIndicesInternalList_Injected(intPtr, list);
	}

	[FreeFunction(Name = "BillboardRenderer_Bindings::MakeMaterialProperties", HasExplicitThis = true)]
	internal void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MakeMaterialProperties_Injected(intPtr, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties), MarshalledUnityObject.Marshal(camera));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_width_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_width_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_height_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_height_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_bottom_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bottom_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_imageCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_vertexCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_indexCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_material_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetImageTexCoords_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetImageTexCoordsInternal_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetImageTexCoords_Injected(IntPtr _unity_self, ref ManagedSpanWrapper imageTexCoords);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetImageTexCoordsInternalList_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVertices_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVerticesInternal_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVertices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper vertices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVerticesInternalList_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIndices_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIndicesInternal_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIndicesInternalList_Injected(IntPtr _unity_self, object list);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MakeMaterialProperties_Injected(IntPtr _unity_self, IntPtr properties, IntPtr camera);
}
