using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
public sealed class ComputeShader : Object
{
	public LocalKeywordSpace keywordSpace
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_keywordSpace_Injected(intPtr, out var ret);
			return ret;
		}
	}

	public string[] shaderKeywords
	{
		get
		{
			return GetShaderKeywords();
		}
		set
		{
			SetShaderKeywords(value);
		}
	}

	public LocalKeyword[] enabledKeywords
	{
		get
		{
			return GetEnabledKeywords();
		}
		set
		{
			SetEnabledKeywords(value);
		}
	}

	[RequiredByNativeCode]
	[NativeMethod(Name = "ComputeShaderScripting::FindKernel", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public unsafe int FindKernel(string name)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindKernel_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return FindKernel_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "ComputeShaderScripting::HasKernel", HasExplicitThis = true)]
	public unsafe bool HasKernel(string name)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return HasKernel_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return HasKernel_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetValue<float>", HasExplicitThis = true)]
	public void SetFloat(int nameID, float val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloat_Injected(intPtr, nameID, val);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetValue<int>", HasExplicitThis = true)]
	public void SetInt(int nameID, int val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetInt_Injected(intPtr, nameID, val);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetValue<Vector4f>", HasExplicitThis = true)]
	public void SetVector(int nameID, Vector4 val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVector_Injected(intPtr, nameID, ref val);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetValue<Matrix4x4f>", HasExplicitThis = true)]
	public void SetMatrix(int nameID, Matrix4x4 val)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMatrix_Injected(intPtr, nameID, ref val);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetArray<float>", HasExplicitThis = true)]
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

	[FreeFunction(Name = "ComputeShaderScripting::SetArray<int>", HasExplicitThis = true)]
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

	[FreeFunction(Name = "ComputeShaderScripting::SetArray<Vector4f>", HasExplicitThis = true)]
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

	[FreeFunction(Name = "ComputeShaderScripting::SetArray<Matrix4x4f>", HasExplicitThis = true)]
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

	[NativeMethod(Name = "ComputeShaderScripting::SetTexture", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public void SetTexture(int kernelIndex, int nameID, [NotNull] Texture texture, int mipLevel)
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
		SetTexture_Injected(intPtr, kernelIndex, nameID, intPtr2, mipLevel);
	}

	[NativeMethod(Name = "ComputeShaderScripting::SetRenderTexture", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	private void SetRenderTexture(int kernelIndex, int nameID, [NotNull] RenderTexture texture, int mipLevel, RenderTextureSubElement element)
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
		SetRenderTexture_Injected(intPtr, kernelIndex, nameID, intPtr2, mipLevel, element);
	}

	[NativeMethod(Name = "ComputeShaderScripting::SetTextureFromGlobal", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public void SetTextureFromGlobal(int kernelIndex, int nameID, int globalTextureNameID)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTextureFromGlobal_Injected(intPtr, kernelIndex, nameID, globalTextureNameID);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetBuffer", HasExplicitThis = true)]
	private void Internal_SetBuffer(int kernelIndex, int nameID, [NotNull] ComputeBuffer buffer)
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
		Internal_SetBuffer_Injected(intPtr, kernelIndex, nameID, intPtr2);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetBuffer", HasExplicitThis = true)]
	private void Internal_SetGraphicsBuffer(int kernelIndex, int nameID, [NotNull] GraphicsBuffer buffer)
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
		Internal_SetGraphicsBuffer_Injected(intPtr, kernelIndex, nameID, intPtr2);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetRayTracingAccelerationStructure", HasExplicitThis = true)]
	private void Internal_SetRayTracingAccelerationStructure(int kernelIndex, int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
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
		Internal_SetRayTracingAccelerationStructure_Injected(intPtr, kernelIndex, nameID, intPtr2);
	}

	public void SetRayTracingAccelerationStructure(int kernelIndex, int nameID, RayTracingAccelerationStructure accelerationStructure)
	{
		Internal_SetRayTracingAccelerationStructure(kernelIndex, nameID, accelerationStructure);
	}

	public void SetBuffer(int kernelIndex, int nameID, ComputeBuffer buffer)
	{
		Internal_SetBuffer(kernelIndex, nameID, buffer);
	}

	public void SetBuffer(int kernelIndex, int nameID, GraphicsBuffer buffer)
	{
		Internal_SetGraphicsBuffer(kernelIndex, nameID, buffer);
	}

	[FreeFunction(Name = "ComputeShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
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

	[FreeFunction(Name = "ComputeShaderScripting::SetConstantBuffer", HasExplicitThis = true)]
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

	[NativeMethod(Name = "ComputeShaderScripting::GetKernelThreadGroupSizes", HasExplicitThis = true, IsFreeFunction = true, ThrowsException = true)]
	public void GetKernelThreadGroupSizes(int kernelIndex, out uint x, out uint y, out uint z)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetKernelThreadGroupSizes_Injected(intPtr, kernelIndex, out x, out y, out z);
	}

	[NativeName("DispatchComputeShader")]
	public void Dispatch(int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Dispatch_Injected(intPtr, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
	}

	[FreeFunction(Name = "ComputeShaderScripting::DispatchIndirect", HasExplicitThis = true)]
	private void Internal_DispatchIndirect(int kernelIndex, [NotNull] ComputeBuffer argsBuffer, uint argsOffset)
	{
		if (argsBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		Internal_DispatchIndirect_Injected(intPtr, kernelIndex, intPtr2, argsOffset);
	}

	[FreeFunction(Name = "ComputeShaderScripting::DispatchIndirect", HasExplicitThis = true)]
	private void Internal_DispatchIndirectGraphicsBuffer(int kernelIndex, [NotNull] GraphicsBuffer argsBuffer, uint argsOffset)
	{
		if (argsBuffer == null)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
		}
		Internal_DispatchIndirectGraphicsBuffer_Injected(intPtr, kernelIndex, intPtr2, argsOffset);
	}

	[FreeFunction("ComputeShaderScripting::EnableKeyword", HasExplicitThis = true)]
	public unsafe void EnableKeyword(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			EnableKeyword_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ComputeShaderScripting::DisableKeyword", HasExplicitThis = true)]
	public unsafe void DisableKeyword(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
					return;
				}
			}
			DisableKeyword_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ComputeShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
	public unsafe bool IsKeywordEnabled(string keyword)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return IsKeywordEnabled_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ComputeShaderScripting::EnableKeyword", HasExplicitThis = true)]
	private void EnableLocalKeyword(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EnableLocalKeyword_Injected(intPtr, ref keyword);
	}

	[FreeFunction("ComputeShaderScripting::DisableKeyword", HasExplicitThis = true)]
	private void DisableLocalKeyword(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		DisableLocalKeyword_Injected(intPtr, ref keyword);
	}

	[FreeFunction("ComputeShaderScripting::SetKeyword", HasExplicitThis = true)]
	private void SetLocalKeyword(LocalKeyword keyword, bool value)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLocalKeyword_Injected(intPtr, ref keyword, value);
	}

	[FreeFunction("ComputeShaderScripting::IsKeywordEnabled", HasExplicitThis = true)]
	private bool IsLocalKeywordEnabled(LocalKeyword keyword)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsLocalKeywordEnabled_Injected(intPtr, ref keyword);
	}

	public void EnableKeyword(in LocalKeyword keyword)
	{
		EnableLocalKeyword(keyword);
	}

	public void DisableKeyword(in LocalKeyword keyword)
	{
		DisableLocalKeyword(keyword);
	}

	public void SetKeyword(in LocalKeyword keyword, bool value)
	{
		SetLocalKeyword(keyword, value);
	}

	public bool IsKeywordEnabled(in LocalKeyword keyword)
	{
		return IsLocalKeywordEnabled(keyword);
	}

	[FreeFunction("ComputeShaderScripting::IsSupported", HasExplicitThis = true)]
	public bool IsSupported(int kernelIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsSupported_Injected(intPtr, kernelIndex);
	}

	[FreeFunction("ComputeShaderScripting::GetShaderKeywords", HasExplicitThis = true)]
	private string[] GetShaderKeywords()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetShaderKeywords_Injected(intPtr);
	}

	[FreeFunction("ComputeShaderScripting::SetShaderKeywords", HasExplicitThis = true)]
	private void SetShaderKeywords(string[] names)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetShaderKeywords_Injected(intPtr, names);
	}

	[FreeFunction("ComputeShaderScripting::GetEnabledKeywords", HasExplicitThis = true)]
	private LocalKeyword[] GetEnabledKeywords()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetEnabledKeywords_Injected(intPtr);
	}

	[FreeFunction("ComputeShaderScripting::SetEnabledKeywords", HasExplicitThis = true)]
	private void SetEnabledKeywords(LocalKeyword[] keywords)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetEnabledKeywords_Injected(intPtr, keywords);
	}

	private ComputeShader()
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

	public void SetTexture(int kernelIndex, int nameID, Texture texture)
	{
		SetTexture(kernelIndex, nameID, texture, 0);
	}

	public void SetTexture(int kernelIndex, string name, Texture texture)
	{
		SetTexture(kernelIndex, Shader.PropertyToID(name), texture, 0);
	}

	public void SetTexture(int kernelIndex, string name, Texture texture, int mipLevel)
	{
		SetTexture(kernelIndex, Shader.PropertyToID(name), texture, mipLevel);
	}

	public void SetTexture(int kernelIndex, int nameID, RenderTexture texture, int mipLevel, RenderTextureSubElement element)
	{
		SetRenderTexture(kernelIndex, nameID, texture, mipLevel, element);
	}

	public void SetTexture(int kernelIndex, string name, RenderTexture texture, int mipLevel, RenderTextureSubElement element)
	{
		SetRenderTexture(kernelIndex, Shader.PropertyToID(name), texture, mipLevel, element);
	}

	public void SetTextureFromGlobal(int kernelIndex, string name, string globalTextureName)
	{
		SetTextureFromGlobal(kernelIndex, Shader.PropertyToID(name), Shader.PropertyToID(globalTextureName));
	}

	public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer)
	{
		SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
	}

	public void SetBuffer(int kernelIndex, string name, GraphicsBuffer buffer)
	{
		SetBuffer(kernelIndex, Shader.PropertyToID(name), buffer);
	}

	public void SetRayTracingAccelerationStructure(int kernelIndex, string name, RayTracingAccelerationStructure accelerationStructure)
	{
		SetRayTracingAccelerationStructure(kernelIndex, Shader.PropertyToID(name), accelerationStructure);
	}

	public void SetConstantBuffer(int nameID, ComputeBuffer buffer, int offset, int size)
	{
		SetConstantComputeBuffer(nameID, buffer, offset, size);
	}

	public void SetConstantBuffer(string name, ComputeBuffer buffer, int offset, int size)
	{
		SetConstantBuffer(Shader.PropertyToID(name), buffer, offset, size);
	}

	public void SetConstantBuffer(int nameID, GraphicsBuffer buffer, int offset, int size)
	{
		SetConstantGraphicsBuffer(nameID, buffer, offset, size);
	}

	public void SetConstantBuffer(string name, GraphicsBuffer buffer, int offset, int size)
	{
		SetConstantBuffer(Shader.PropertyToID(name), buffer, offset, size);
	}

	public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
	{
		if (argsBuffer == null)
		{
			throw new ArgumentNullException("argsBuffer");
		}
		if (argsBuffer.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("argsBuffer");
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		Internal_DispatchIndirect(kernelIndex, argsBuffer, argsOffset);
	}

	[ExcludeFromDocs]
	public void DispatchIndirect(int kernelIndex, ComputeBuffer argsBuffer)
	{
		DispatchIndirect(kernelIndex, argsBuffer, 0u);
	}

	public void DispatchIndirect(int kernelIndex, GraphicsBuffer argsBuffer, [DefaultValue("0")] uint argsOffset)
	{
		if (argsBuffer == null)
		{
			throw new ArgumentNullException("argsBuffer");
		}
		if (argsBuffer.m_Ptr == IntPtr.Zero)
		{
			throw new ObjectDisposedException("argsBuffer");
		}
		if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer)
		{
			throw new InvalidOperationException("Indirect argument buffers are not supported.");
		}
		Internal_DispatchIndirectGraphicsBuffer(kernelIndex, argsBuffer, argsOffset);
	}

	[ExcludeFromDocs]
	public void DispatchIndirect(int kernelIndex, GraphicsBuffer argsBuffer)
	{
		DispatchIndirect(kernelIndex, argsBuffer, 0u);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int FindKernel_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasKernel_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

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
	private static extern void SetTexture_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr texture, int mipLevel);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRenderTexture_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr texture, int mipLevel, RenderTextureSubElement element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureFromGlobal_Injected(IntPtr _unity_self, int kernelIndex, int nameID, int globalTextureNameID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetBuffer_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetGraphicsBuffer_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr buffer);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetRayTracingAccelerationStructure_Injected(IntPtr _unity_self, int kernelIndex, int nameID, IntPtr accelerationStructure);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantComputeBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantGraphicsBuffer_Injected(IntPtr _unity_self, int nameID, IntPtr buffer, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetKernelThreadGroupSizes_Injected(IntPtr _unity_self, int kernelIndex, out uint x, out uint y, out uint z);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Dispatch_Injected(IntPtr _unity_self, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DispatchIndirect_Injected(IntPtr _unity_self, int kernelIndex, IntPtr argsBuffer, uint argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DispatchIndirectGraphicsBuffer_Injected(IntPtr _unity_self, int kernelIndex, IntPtr argsBuffer, uint argsOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_keywordSpace_Injected(IntPtr _unity_self, out LocalKeywordSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordEnabled_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLocalKeyword_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsLocalKeywordEnabled_Injected(IntPtr _unity_self, [In] ref LocalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsSupported_Injected(IntPtr _unity_self, int kernelIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetShaderKeywords_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetShaderKeywords_Injected(IntPtr _unity_self, string[] names);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern LocalKeyword[] GetEnabledKeywords_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetEnabledKeywords_Injected(IntPtr _unity_self, LocalKeyword[] keywords);
}
