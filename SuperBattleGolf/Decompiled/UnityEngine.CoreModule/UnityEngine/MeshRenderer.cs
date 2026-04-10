using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/Mesh/MeshRenderer.h")]
public class MeshRenderer : Renderer
{
	public Mesh additionalVertexStreams
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(get_additionalVertexStreams_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_additionalVertexStreams_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public Mesh enlightenVertexStream
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(get_enlightenVertexStream_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enlightenVertexStream_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public int subMeshStartIndex
	{
		[NativeName("GetSubMeshStartIndex")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_subMeshStartIndex_Injected(intPtr);
		}
	}

	[RequiredByNativeCode]
	private void DontStripMeshRenderer()
	{
	}

	[FreeFunction(Name = "MeshRendererScripting::SetShaderUserValue", HasExplicitThis = true)]
	internal void Internal_SetShaderUserValueUInt(uint v)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_SetShaderUserValueUInt_Injected(intPtr, v);
	}

	public void SetShaderUserValue(uint v)
	{
		Internal_SetShaderUserValueUInt(v);
	}

	[FreeFunction(Name = "MeshRendererScripting::GetShaderUserValue", HasExplicitThis = true)]
	internal uint Internal_GetShaderUserValueUInt()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_GetShaderUserValueUInt_Injected(intPtr);
	}

	public uint GetShaderUserValue()
	{
		return Internal_GetShaderUserValueUInt();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_additionalVertexStreams_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_additionalVertexStreams_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_enlightenVertexStream_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enlightenVertexStream_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_subMeshStartIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetShaderUserValueUInt_Injected(IntPtr _unity_self, uint v);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint Internal_GetShaderUserValueUInt_Injected(IntPtr _unity_self);
}
