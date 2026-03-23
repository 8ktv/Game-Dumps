using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[MovedFrom("UnityEngine.Experimental.Rendering")]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[NativeHeader("Runtime/Shaders/RayTracing/RayTracingShader.h")]
[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
public sealed class RayTracingShader : Object
{
	public float maxRecursionDepth
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxRecursionDepth_Injected(intPtr);
		}
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetFloat", HasExplicitThis = true)]
	public void SetFloat(int nameID, float val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloat_Injected(intPtr, nameID, val);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetInt", HasExplicitThis = true)]
	public void SetInt(int nameID, int val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetInt_Injected(intPtr, nameID, val);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetVector", HasExplicitThis = true)]
	public void SetVector(int nameID, Vector4 val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVector_Injected(intPtr, nameID, ref val);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetMatrix", HasExplicitThis = true)]
	public void SetMatrix(int nameID, Matrix4x4 val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMatrix_Injected(intPtr, nameID, ref val);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetFloatArray", HasExplicitThis = true)]
	private unsafe void SetFloatArray(int nameID, float[] values)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(values);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetFloatArray_Injected(intPtr, nameID, ref values2);
		}
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetIntArray", HasExplicitThis = true)]
	private unsafe void SetIntArray(int nameID, int[] values)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<int> span = new Span<int>(values);
		fixed (int* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetIntArray_Injected(intPtr, nameID, ref values2);
		}
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetVectorArray", HasExplicitThis = true)]
	public unsafe void SetVectorArray(int nameID, Vector4[] values)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector4> span = new Span<Vector4>(values);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetVectorArray_Injected(intPtr, nameID, ref values2);
		}
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetMatrixArray", HasExplicitThis = true)]
	public unsafe void SetMatrixArray(int nameID, Matrix4x4[] values)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Matrix4x4> span = new Span<Matrix4x4>(values);
		fixed (Matrix4x4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetMatrixArray_Injected(intPtr, nameID, ref values2);
		}
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetTexture", HasExplicitThis = true, IsFreeFunction = true)]
	public void SetTexture(int nameID, [NotNull] Texture texture)
	{
		if ((object)texture == null)
		{
			ThrowHelper.ThrowArgumentNullException(texture, "texture");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(texture);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(texture, "texture");
		}
		SetTexture_Injected(intPtr, nameID, intPtr2);
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
	public void SetBuffer(int nameID, [NotNull] ComputeBuffer buffer)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		SetBuffer_Injected(intPtr, nameID, intPtr2);
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
	private void SetGraphicsBuffer(int nameID, [NotNull] GraphicsBuffer buffer)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		SetGraphicsBuffer_Injected(intPtr, nameID, intPtr2);
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetBuffer", HasExplicitThis = true, IsFreeFunction = true)]
	private void SetGraphicsBufferHandle(int nameID, GraphicsBufferHandle bufferHandle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGraphicsBufferHandle_Injected(intPtr, nameID, ref bufferHandle);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
	private void SetConstantComputeBuffer(int nameID, [NotNull] ComputeBuffer buffer, int offset, int size)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		SetConstantComputeBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
	}

	[FreeFunction(Name = "RayTracingShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
	private void SetConstantGraphicsBuffer(int nameID, [NotNull] GraphicsBuffer buffer, int offset, int size)
	{
		if (buffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
		}
		SetConstantGraphicsBuffer_Injected(intPtr, nameID, intPtr2, offset, size);
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetAccelerationStructure", HasExplicitThis = true, IsFreeFunction = true)]
	public void SetAccelerationStructure(int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
	{
		if (accelerationStructure == null)
		{
			ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
		}
		SetAccelerationStructure_Injected(intPtr, nameID, intPtr2);
	}

	public unsafe void SetShaderPass(string passName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(passName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetShaderPass_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			SetShaderPass_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeMethod(Name = "RayTracingShaderScripting::SetTextureFromGlobal", HasExplicitThis = true, IsFreeFunction = true)]
	public void SetTextureFromGlobal(int nameID, int globalTextureNameID)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTextureFromGlobal_Injected(intPtr, nameID, globalTextureNameID);
	}

	[NativeMethod(Name = "RayTracingShaderScripting::Dispatch", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public unsafe void Dispatch(string rayGenFunctionName, int width, int height, int depth, Camera camera = null)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenFunctionName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(rayGenFunctionName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Dispatch_Injected(intPtr, ref managedSpanWrapper, width, height, depth, MarshalledUnityObject.Marshal(camera));
					return;
				}
			}
			Dispatch_Injected(intPtr, ref managedSpanWrapper, width, height, depth, MarshalledUnityObject.Marshal(camera));
		}
		finally
		{
		}
	}

	[NativeMethod(Name = "RayTracingShaderScripting::DispatchIndirect", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public unsafe void DispatchIndirect(string rayGenFunctionName, [NotNull] GraphicsBuffer argsBuffer, uint argsOffset = 0u, Camera camera = null)
	{
		//The blocks IL_0048, IL_0054, IL_005f are reachable both inside and outside the pinned region starting at IL_0037. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if (argsBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper rayGenFunctionName2;
			IntPtr intPtr2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenFunctionName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(rayGenFunctionName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					rayGenFunctionName2 = ref managedSpanWrapper;
					intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
					if (intPtr2 == (IntPtr)0)
					{
						ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
					}
					DispatchIndirect_Injected(intPtr, ref rayGenFunctionName2, intPtr2, argsOffset, MarshalledUnityObject.Marshal(camera));
					return;
				}
			}
			rayGenFunctionName2 = ref managedSpanWrapper;
			intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			DispatchIndirect_Injected(intPtr, ref rayGenFunctionName2, intPtr2, argsOffset, MarshalledUnityObject.Marshal(camera));
		}
		finally
		{
		}
	}

	public void SetBuffer(int nameID, GraphicsBuffer buffer)
	{
		SetGraphicsBuffer(nameID, buffer);
	}

	public void SetBuffer(int nameID, GraphicsBufferHandle bufferHandle)
	{
		SetGraphicsBufferHandle(nameID, bufferHandle);
	}

	private RayTracingShader()
	{
	}

	public void SetFloat(string name, float val)
	{
		SetFloat(Shader.PropertyToID(name), val);
	}

	public void SetInt(string name, int val)
	{
		SetInt(Shader.PropertyToID(name), val);
	}

	public void SetVector(string name, Vector4 val)
	{
		SetVector(Shader.PropertyToID(name), val);
	}

	public void SetMatrix(string name, Matrix4x4 val)
	{
		SetMatrix(Shader.PropertyToID(name), val);
	}

	public void SetVectorArray(string name, Vector4[] values)
	{
		SetVectorArray(Shader.PropertyToID(name), values);
	}

	public void SetMatrixArray(string name, Matrix4x4[] values)
	{
		SetMatrixArray(Shader.PropertyToID(name), values);
	}

	public void SetFloats(string name, params float[] values)
	{
		SetFloatArray(Shader.PropertyToID(name), values);
	}

	public void SetFloats(int nameID, params float[] values)
	{
		SetFloatArray(nameID, values);
	}

	public void SetInts(string name, params int[] values)
	{
		SetIntArray(Shader.PropertyToID(name), values);
	}

	public void SetInts(int nameID, params int[] values)
	{
		SetIntArray(nameID, values);
	}

	public void SetBool(string name, bool val)
	{
		SetInt(Shader.PropertyToID(name), val ? 1 : 0);
	}

	public void SetBool(int nameID, bool val)
	{
		SetInt(nameID, val ? 1 : 0);
	}

	public void SetTexture(string name, Texture texture)
	{
		SetTexture(Shader.PropertyToID(name), texture);
	}

	public void SetBuffer(string name, ComputeBuffer buffer)
	{
		SetBuffer(Shader.PropertyToID(name), buffer);
	}

	public void SetBuffer(string name, GraphicsBuffer buffer)
	{
		SetBuffer(Shader.PropertyToID(name), buffer);
	}

	public void SetBuffer(string name, GraphicsBufferHandle bufferHandle)
	{
		SetBuffer(Shader.PropertyToID(name), bufferHandle);
	}

	public void SetConstantBuffer(int nameID, ComputeBuffer buffer, int offset, int size)
	{
		SetConstantComputeBuffer(nameID, buffer, offset, size);
	}

	public void SetConstantBuffer(string name, ComputeBuffer buffer, int offset, int size)
	{
		SetConstantComputeBuffer(Shader.PropertyToID(name), buffer, offset, size);
	}

	public void SetConstantBuffer(int nameID, GraphicsBuffer buffer, int offset, int size)
	{
		SetConstantGraphicsBuffer(nameID, buffer, offset, size);
	}

	public void SetConstantBuffer(string name, GraphicsBuffer buffer, int offset, int size)
	{
		SetConstantGraphicsBuffer(Shader.PropertyToID(name), buffer, offset, size);
	}

	public void SetAccelerationStructure(string name, RayTracingAccelerationStructure accelerationStructure)
	{
		SetAccelerationStructure(Shader.PropertyToID(name), accelerationStructure);
	}

	public void SetTextureFromGlobal(string name, string globalTextureName)
	{
		SetTextureFromGlobal(Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxRecursionDepth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloat_Injected(IntPtr _unity_self, int nameID, float val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetInt_Injected(IntPtr _unity_self, int nameID, int val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVector_Injected(IntPtr _unity_self, int nameID, [In] ref Vector4 val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrix_Injected(IntPtr _unity_self, int nameID, [In] ref Matrix4x4 val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVectorArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrixArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTexture_Injected(IntPtr _unity_self, int nameID, IntPtr texture);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGraphicsBufferHandle_Injected(IntPtr _unity_self, int nameID, [In] ref GraphicsBufferHandle bufferHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantComputeBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAccelerationStructure_Injected(IntPtr _unity_self, int nameID, IntPtr accelerationStructure);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetShaderPass_Injected(IntPtr _unity_self, ref ManagedSpanWrapper passName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureFromGlobal_Injected(IntPtr _unity_self, int nameID, int globalTextureNameID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Dispatch_Injected(IntPtr _unity_self, ref ManagedSpanWrapper rayGenFunctionName, int width, int height, int depth, IntPtr camera);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DispatchIndirect_Injected(IntPtr _unity_self, ref ManagedSpanWrapper rayGenFunctionName, IntPtr argsBuffer, uint argsOffset, IntPtr camera);
}
