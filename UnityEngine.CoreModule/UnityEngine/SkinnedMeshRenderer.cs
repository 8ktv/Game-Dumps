using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[RequiredByNativeCode]
[NativeHeader("Runtime/Graphics/Mesh/SkinnedMeshRenderer.h")]
public class SkinnedMeshRenderer : Renderer
{
	public SkinQuality quality
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_quality_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_quality_Injected(intPtr, value);
		}
	}

	public bool updateWhenOffscreen
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_updateWhenOffscreen_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_updateWhenOffscreen_Injected(intPtr, value);
		}
	}

	public bool forceMatrixRecalculationPerRender
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_forceMatrixRecalculationPerRender_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_forceMatrixRecalculationPerRender_Injected(intPtr, value);
		}
	}

	public Transform rootBone
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Transform>(get_rootBone_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rootBone_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Transform[] bones
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bones_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bones_Injected(intPtr, value);
		}
	}

	[NativeProperty("Mesh")]
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

	[NativeProperty("SkinnedMeshMotionVectors")]
	public bool skinnedMotionVectors
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_skinnedMotionVectors_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_skinnedMotionVectors_Injected(intPtr, value);
		}
	}

	public GraphicsBuffer.Target vertexBufferTarget
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexBufferTarget_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vertexBufferTarget_Injected(intPtr, value);
		}
	}

	public float GetBlendShapeWeight(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBlendShapeWeight_Injected(intPtr, index);
	}

	public void SetBlendShapeWeight(int index, float value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBlendShapeWeight_Injected(intPtr, index, value);
	}

	public void BakeMesh(Mesh mesh)
	{
		BakeMesh(mesh, useScale: false);
	}

	public void BakeMesh([NotNull] Mesh mesh, bool useScale)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		BakeMesh_Injected(intPtr, intPtr2, useScale);
	}

	public GraphicsBuffer GetVertexBuffer()
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		return GetVertexBufferImpl();
	}

	public GraphicsBuffer GetPreviousVertexBuffer()
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		return GetPreviousVertexBufferImpl();
	}

	[FreeFunction(Name = "SkinnedMeshRendererScripting::GetVertexBufferPtr", HasExplicitThis = true)]
	private GraphicsBuffer GetVertexBufferImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr vertexBufferImpl_Injected = GetVertexBufferImpl_Injected(intPtr);
		return (vertexBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(vertexBufferImpl_Injected);
	}

	[FreeFunction(Name = "SkinnedMeshRendererScripting::GetPreviousVertexBufferPtr", HasExplicitThis = true)]
	private GraphicsBuffer GetPreviousVertexBufferImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr previousVertexBufferImpl_Injected = GetPreviousVertexBufferImpl_Injected(intPtr);
		return (previousVertexBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(previousVertexBufferImpl_Injected);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SkinQuality get_quality_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_quality_Injected(IntPtr _unity_self, SkinQuality value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_updateWhenOffscreen_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_updateWhenOffscreen_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_forceMatrixRecalculationPerRender_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_forceMatrixRecalculationPerRender_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_rootBone_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rootBone_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Transform[] get_bones_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bones_Injected(IntPtr _unity_self, Transform[] value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_sharedMesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sharedMesh_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_skinnedMotionVectors_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_skinnedMotionVectors_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetBlendShapeWeight_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBlendShapeWeight_Injected(IntPtr _unity_self, int index, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, bool useScale);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetVertexBufferImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetPreviousVertexBufferImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsBuffer.Target get_vertexBufferTarget_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vertexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);
}
