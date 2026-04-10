using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Math/SphericalHarmonicsL2.h")]
[NativeHeader("Runtime/Shaders/ShaderPropertySheet.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
public sealed class MaterialPropertyBlock
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(MaterialPropertyBlock materialPropertyBlock)
		{
			return materialPropertyBlock.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	public bool isEmpty
	{
		[NativeName("IsEmpty")]
		[ThreadSafe]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isEmpty_Injected(intPtr);
		}
	}

	[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", true)]
	public void AddFloat(string name, float value)
	{
		SetFloat(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetFloat instead (UnityUpgradable) -> SetFloat(*)", true)]
	public void AddFloat(int nameID, float value)
	{
		SetFloat(nameID, value);
	}

	[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", true)]
	public void AddVector(string name, Vector4 value)
	{
		SetVector(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetVector instead (UnityUpgradable) -> SetVector(*)", true)]
	public void AddVector(int nameID, Vector4 value)
	{
		SetVector(nameID, value);
	}

	[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", true)]
	public void AddColor(string name, Color value)
	{
		SetColor(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetColor instead (UnityUpgradable) -> SetColor(*)", true)]
	public void AddColor(int nameID, Color value)
	{
		SetColor(nameID, value);
	}

	[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", true)]
	public void AddMatrix(string name, Matrix4x4 value)
	{
		SetMatrix(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetMatrix instead (UnityUpgradable) -> SetMatrix(*)", true)]
	public void AddMatrix(int nameID, Matrix4x4 value)
	{
		SetMatrix(nameID, value);
	}

	[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", true)]
	public void AddTexture(string name, Texture value)
	{
		SetTexture(Shader.PropertyToID(name), value);
	}

	[Obsolete("Use SetTexture instead (UnityUpgradable) -> SetTexture(*)", true)]
	public void AddTexture(int nameID, Texture value)
	{
		SetTexture(nameID, value);
	}

	[NativeName("GetIntFromScript")]
	[ThreadSafe]
	private int GetIntImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIntImpl_Injected(intPtr, name);
	}

	[ThreadSafe]
	[NativeName("GetFloatFromScript")]
	private float GetFloatImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFloatImpl_Injected(intPtr, name);
	}

	[ThreadSafe]
	[NativeName("GetVectorFromScript")]
	private Vector4 GetVectorImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetVectorImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("GetColorFromScript")]
	[ThreadSafe]
	private Color GetColorImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetColorImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[NativeName("GetMatrixFromScript")]
	[ThreadSafe]
	private Matrix4x4 GetMatrixImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetMatrixImpl_Injected(intPtr, name, out var ret);
		return ret;
	}

	[ThreadSafe]
	[NativeName("GetTextureFromScript")]
	private Texture GetTextureImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Unmarshal.UnmarshalUnityObject<Texture>(GetTextureImpl_Injected(intPtr, name));
	}

	[NativeName("HasPropertyFromScript")]
	private bool HasPropertyImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasPropertyImpl_Injected(intPtr, name);
	}

	[NativeName("HasFloatFromScript")]
	private bool HasFloatImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasFloatImpl_Injected(intPtr, name);
	}

	[NativeName("HasIntegerFromScript")]
	private bool HasIntImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasIntImpl_Injected(intPtr, name);
	}

	[NativeName("HasTextureFromScript")]
	private bool HasTextureImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasTextureImpl_Injected(intPtr, name);
	}

	[NativeName("HasMatrixFromScript")]
	private bool HasMatrixImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasMatrixImpl_Injected(intPtr, name);
	}

	[NativeName("HasVectorFromScript")]
	private bool HasVectorImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasVectorImpl_Injected(intPtr, name);
	}

	[NativeName("HasBufferFromScript")]
	private bool HasBufferImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasBufferImpl_Injected(intPtr, name);
	}

	[NativeName("HasConstantBufferFromScript")]
	private bool HasConstantBufferImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasConstantBufferImpl_Injected(intPtr, name);
	}

	[NativeName("SetIntFromScript")]
	[ThreadSafe]
	private void SetIntImpl(int name, int value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIntImpl_Injected(intPtr, name, value);
	}

	[ThreadSafe]
	[NativeName("SetFloatFromScript")]
	private void SetFloatImpl(int name, float value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetFloatImpl_Injected(intPtr, name, value);
	}

	[NativeName("SetVectorFromScript")]
	[ThreadSafe]
	private void SetVectorImpl(int name, Vector4 value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVectorImpl_Injected(intPtr, name, ref value);
	}

	[ThreadSafe]
	[NativeName("SetColorFromScript")]
	private void SetColorImpl(int name, Color value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetColorImpl_Injected(intPtr, name, ref value);
	}

	[NativeName("SetMatrixFromScript")]
	[ThreadSafe]
	private void SetMatrixImpl(int name, Matrix4x4 value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMatrixImpl_Injected(intPtr, name, ref value);
	}

	[ThreadSafe]
	[NativeName("SetTextureFromScript")]
	private void SetTextureImpl(int name, [NotNull] Texture value)
	{
		if ((object)value == null)
		{
			ThrowHelper.ThrowArgumentNullException(value, "value");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(value);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(value, "value");
		}
		SetTextureImpl_Injected(intPtr, name, intPtr2);
	}

	[ThreadSafe]
	[NativeName("SetRenderTextureFromScript")]
	private void SetRenderTextureImpl(int name, [NotNull] RenderTexture value, RenderTextureSubElement element)
	{
		if ((object)value == null)
		{
			ThrowHelper.ThrowArgumentNullException(value, "value");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(value);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(value, "value");
		}
		SetRenderTextureImpl_Injected(intPtr, name, intPtr2, element);
	}

	[NativeName("SetBufferFromScript")]
	[ThreadSafe]
	private void SetBufferImpl(int name, ComputeBuffer value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[NativeName("SetBufferFromScript")]
	[ThreadSafe]
	private void SetGraphicsBufferImpl(int name, GraphicsBuffer value)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[ThreadSafe]
	[NativeName("SetConstantBufferFromScript")]
	private void SetConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetConstantBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[ThreadSafe]
	[NativeName("SetConstantBufferFromScript")]
	private void SetConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetConstantGraphicsBufferImpl_Injected(intPtr, name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[NativeName("SetFloatArrayFromScript")]
	[ThreadSafe]
	private unsafe void SetFloatArrayImpl(int name, float[] values, int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(values);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetFloatArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[NativeName("SetVectorArrayFromScript")]
	[ThreadSafe]
	private unsafe void SetVectorArrayImpl(int name, Vector4[] values, int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector4> span = new Span<Vector4>(values);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetVectorArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[NativeName("SetMatrixArrayFromScript")]
	[ThreadSafe]
	private unsafe void SetMatrixArrayImpl(int name, Matrix4x4[] values, int count)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Matrix4x4> span = new Span<Matrix4x4>(values);
		fixed (Matrix4x4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetMatrixArrayImpl_Injected(intPtr, name, ref values2, count);
		}
	}

	[NativeName("GetFloatArrayFromScript")]
	[ThreadSafe]
	private float[] GetFloatArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		float[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetFloatArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ThreadSafe]
	[NativeName("GetVectorArrayFromScript")]
	private Vector4[] GetVectorArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector4[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetVectorArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			Vector4[] array = default(Vector4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[ThreadSafe]
	[NativeName("GetMatrixArrayFromScript")]
	private Matrix4x4[] GetMatrixArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Matrix4x4[] result;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetMatrixArrayImpl_Injected(intPtr, name, out ret);
		}
		finally
		{
			Matrix4x4[] array = default(Matrix4x4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[NativeName("GetFloatArrayCountFromScript")]
	[ThreadSafe]
	private int GetFloatArrayCountImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetFloatArrayCountImpl_Injected(intPtr, name);
	}

	[NativeName("GetVectorArrayCountFromScript")]
	[ThreadSafe]
	private int GetVectorArrayCountImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVectorArrayCountImpl_Injected(intPtr, name);
	}

	[ThreadSafe]
	[NativeName("GetMatrixArrayCountFromScript")]
	private int GetMatrixArrayCountImpl(int name)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMatrixArrayCountImpl_Injected(intPtr, name);
	}

	[NativeName("ExtractFloatArrayFromScript")]
	[ThreadSafe]
	private unsafe void ExtractFloatArrayImpl(int name, [Out] float[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (float[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractFloatArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractFloatArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[NativeName("ExtractVectorArrayFromScript")]
	[ThreadSafe]
	private unsafe void ExtractVectorArrayImpl(int name, [Out] Vector4[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (Vector4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractVectorArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractVectorArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[NativeName("ExtractMatrixArrayFromScript")]
	[ThreadSafe]
	private unsafe void ExtractMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
	{
		//The blocks IL_002c are reachable both inside and outside the pinned region starting at IL_0015. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (val != null)
			{
				fixed (Matrix4x4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractMatrixArrayImpl_Injected(intPtr, name, out val2);
					return;
				}
			}
			ExtractMatrixArrayImpl_Injected(intPtr, name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[ThreadSafe]
	[FreeFunction("ConvertAndCopySHCoefficientArraysToPropertySheetFromScript")]
	internal unsafe static void Internal_CopySHCoefficientArraysFrom(MaterialPropertyBlock properties, SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
	{
		IntPtr properties2 = ((properties == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(properties));
		Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(lightProbes);
		fixed (SphericalHarmonicsL2* begin = span)
		{
			ManagedSpanWrapper lightProbes2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_CopySHCoefficientArraysFrom_Injected(properties2, ref lightProbes2, sourceStart, destStart, count);
		}
	}

	[FreeFunction("CopyProbeOcclusionArrayToPropertySheetFromScript")]
	[ThreadSafe]
	internal unsafe static void Internal_CopyProbeOcclusionArrayFrom(MaterialPropertyBlock properties, Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
	{
		IntPtr properties2 = ((properties == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(properties));
		Span<Vector4> span = new Span<Vector4>(occlusionProbes);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper occlusionProbes2 = new ManagedSpanWrapper(begin, span.Length);
			Internal_CopyProbeOcclusionArrayFrom_Injected(properties2, ref occlusionProbes2, sourceStart, destStart, count);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Create", IsFreeFunction = true)]
	private static extern IntPtr CreateImpl();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "MaterialPropertyBlockScripting::Destroy", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void DestroyImpl(IntPtr mpb);

	[ThreadSafe]
	private void Clear(bool keepMemory)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Clear_Injected(intPtr, keepMemory);
	}

	public void Clear()
	{
		Clear(keepMemory: true);
	}

	private void SetFloatArray(int name, float[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetFloatArrayImpl(name, values, count);
	}

	private void SetVectorArray(int name, Vector4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetVectorArrayImpl(name, values, count);
	}

	private void SetMatrixArray(int name, Matrix4x4[] values, int count)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (values.Length == 0)
		{
			throw new ArgumentException("Zero-sized array is not allowed.");
		}
		if (values.Length < count)
		{
			throw new ArgumentException("array has less elements than passed count.");
		}
		SetMatrixArrayImpl(name, values, count);
	}

	private void ExtractFloatArray(int name, List<float> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int floatArrayCountImpl = GetFloatArrayCountImpl(name);
		if (floatArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, floatArrayCountImpl);
			ExtractFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractVectorArray(int name, List<Vector4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int vectorArrayCountImpl = GetVectorArrayCountImpl(name);
		if (vectorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, vectorArrayCountImpl);
			ExtractVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private void ExtractMatrixArray(int name, List<Matrix4x4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int matrixArrayCountImpl = GetMatrixArrayCountImpl(name);
		if (matrixArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, matrixArrayCountImpl);
			ExtractMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	public MaterialPropertyBlock()
	{
		m_Ptr = CreateImpl();
	}

	~MaterialPropertyBlock()
	{
		Dispose();
	}

	private void Dispose()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			DestroyImpl(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	public void SetInt(string name, int value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetInt(int nameID, int value)
	{
		SetFloatImpl(nameID, value);
	}

	public void SetFloat(string name, float value)
	{
		SetFloatImpl(Shader.PropertyToID(name), value);
	}

	public void SetFloat(int nameID, float value)
	{
		SetFloatImpl(nameID, value);
	}

	public void SetInteger(string name, int value)
	{
		SetIntImpl(Shader.PropertyToID(name), value);
	}

	public void SetInteger(int nameID, int value)
	{
		SetIntImpl(nameID, value);
	}

	public void SetVector(string name, Vector4 value)
	{
		SetVectorImpl(Shader.PropertyToID(name), value);
	}

	public void SetVector(int nameID, Vector4 value)
	{
		SetVectorImpl(nameID, value);
	}

	public void SetColor(string name, Color value)
	{
		SetColorImpl(Shader.PropertyToID(name), value);
	}

	public void SetColor(int nameID, Color value)
	{
		SetColorImpl(nameID, value);
	}

	public void SetMatrix(string name, Matrix4x4 value)
	{
		SetMatrixImpl(Shader.PropertyToID(name), value);
	}

	public void SetMatrix(int nameID, Matrix4x4 value)
	{
		SetMatrixImpl(nameID, value);
	}

	public void SetBuffer(string name, ComputeBuffer value)
	{
		SetBufferImpl(Shader.PropertyToID(name), value);
	}

	public void SetBuffer(int nameID, ComputeBuffer value)
	{
		SetBufferImpl(nameID, value);
	}

	public void SetBuffer(string name, GraphicsBuffer value)
	{
		SetGraphicsBufferImpl(Shader.PropertyToID(name), value);
	}

	public void SetBuffer(int nameID, GraphicsBuffer value)
	{
		SetGraphicsBufferImpl(nameID, value);
	}

	public void SetTexture(string name, Texture value)
	{
		SetTextureImpl(Shader.PropertyToID(name), value);
	}

	public void SetTexture(int nameID, Texture value)
	{
		SetTextureImpl(nameID, value);
	}

	public void SetTexture(string name, RenderTexture value, RenderTextureSubElement element)
	{
		SetRenderTextureImpl(Shader.PropertyToID(name), value, element);
	}

	public void SetTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
	{
		SetRenderTextureImpl(nameID, value, element);
	}

	public void SetConstantBuffer(string name, ComputeBuffer value, int offset, int size)
	{
		SetConstantBufferImpl(Shader.PropertyToID(name), value, offset, size);
	}

	public void SetConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
	{
		SetConstantBufferImpl(nameID, value, offset, size);
	}

	public void SetConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
	{
		SetConstantGraphicsBufferImpl(Shader.PropertyToID(name), value, offset, size);
	}

	public void SetConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
	{
		SetConstantGraphicsBufferImpl(nameID, value, offset, size);
	}

	public void SetFloatArray(string name, List<float> values)
	{
		SetFloatArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetFloatArray(int nameID, List<float> values)
	{
		SetFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetFloatArray(string name, float[] values)
	{
		SetFloatArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetFloatArray(int nameID, float[] values)
	{
		SetFloatArray(nameID, values, values.Length);
	}

	public void SetVectorArray(string name, List<Vector4> values)
	{
		SetVectorArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetVectorArray(int nameID, List<Vector4> values)
	{
		SetVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetVectorArray(string name, Vector4[] values)
	{
		SetVectorArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetVectorArray(int nameID, Vector4[] values)
	{
		SetVectorArray(nameID, values, values.Length);
	}

	public void SetMatrixArray(string name, List<Matrix4x4> values)
	{
		SetMatrixArray(Shader.PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetMatrixArray(int nameID, List<Matrix4x4> values)
	{
		SetMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public void SetMatrixArray(string name, Matrix4x4[] values)
	{
		SetMatrixArray(Shader.PropertyToID(name), values, values.Length);
	}

	public void SetMatrixArray(int nameID, Matrix4x4[] values)
	{
		SetMatrixArray(nameID, values, values.Length);
	}

	public bool HasProperty(string name)
	{
		return HasPropertyImpl(Shader.PropertyToID(name));
	}

	public bool HasProperty(int nameID)
	{
		return HasPropertyImpl(nameID);
	}

	public bool HasInt(string name)
	{
		return HasFloatImpl(Shader.PropertyToID(name));
	}

	public bool HasInt(int nameID)
	{
		return HasFloatImpl(nameID);
	}

	public bool HasFloat(string name)
	{
		return HasFloatImpl(Shader.PropertyToID(name));
	}

	public bool HasFloat(int nameID)
	{
		return HasFloatImpl(nameID);
	}

	public bool HasInteger(string name)
	{
		return HasIntImpl(Shader.PropertyToID(name));
	}

	public bool HasInteger(int nameID)
	{
		return HasIntImpl(nameID);
	}

	public bool HasTexture(string name)
	{
		return HasTextureImpl(Shader.PropertyToID(name));
	}

	public bool HasTexture(int nameID)
	{
		return HasTextureImpl(nameID);
	}

	public bool HasMatrix(string name)
	{
		return HasMatrixImpl(Shader.PropertyToID(name));
	}

	public bool HasMatrix(int nameID)
	{
		return HasMatrixImpl(nameID);
	}

	public bool HasVector(string name)
	{
		return HasVectorImpl(Shader.PropertyToID(name));
	}

	public bool HasVector(int nameID)
	{
		return HasVectorImpl(nameID);
	}

	public bool HasColor(string name)
	{
		return HasVectorImpl(Shader.PropertyToID(name));
	}

	public bool HasColor(int nameID)
	{
		return HasVectorImpl(nameID);
	}

	public bool HasBuffer(string name)
	{
		return HasBufferImpl(Shader.PropertyToID(name));
	}

	public bool HasBuffer(int nameID)
	{
		return HasBufferImpl(nameID);
	}

	public bool HasConstantBuffer(string name)
	{
		return HasConstantBufferImpl(Shader.PropertyToID(name));
	}

	public bool HasConstantBuffer(int nameID)
	{
		return HasConstantBufferImpl(nameID);
	}

	public float GetFloat(string name)
	{
		return GetFloatImpl(Shader.PropertyToID(name));
	}

	public float GetFloat(int nameID)
	{
		return GetFloatImpl(nameID);
	}

	public int GetInt(string name)
	{
		return (int)GetFloatImpl(Shader.PropertyToID(name));
	}

	public int GetInt(int nameID)
	{
		return (int)GetFloatImpl(nameID);
	}

	public int GetInteger(string name)
	{
		return GetIntImpl(Shader.PropertyToID(name));
	}

	public int GetInteger(int nameID)
	{
		return GetIntImpl(nameID);
	}

	public Vector4 GetVector(string name)
	{
		return GetVectorImpl(Shader.PropertyToID(name));
	}

	public Vector4 GetVector(int nameID)
	{
		return GetVectorImpl(nameID);
	}

	public Color GetColor(string name)
	{
		return GetColorImpl(Shader.PropertyToID(name));
	}

	public Color GetColor(int nameID)
	{
		return GetColorImpl(nameID);
	}

	public Matrix4x4 GetMatrix(string name)
	{
		return GetMatrixImpl(Shader.PropertyToID(name));
	}

	public Matrix4x4 GetMatrix(int nameID)
	{
		return GetMatrixImpl(nameID);
	}

	public Texture GetTexture(string name)
	{
		return GetTextureImpl(Shader.PropertyToID(name));
	}

	public Texture GetTexture(int nameID)
	{
		return GetTextureImpl(nameID);
	}

	public float[] GetFloatArray(string name)
	{
		return GetFloatArray(Shader.PropertyToID(name));
	}

	public float[] GetFloatArray(int nameID)
	{
		return (GetFloatArrayCountImpl(nameID) != 0) ? GetFloatArrayImpl(nameID) : null;
	}

	public Vector4[] GetVectorArray(string name)
	{
		return GetVectorArray(Shader.PropertyToID(name));
	}

	public Vector4[] GetVectorArray(int nameID)
	{
		return (GetVectorArrayCountImpl(nameID) != 0) ? GetVectorArrayImpl(nameID) : null;
	}

	public Matrix4x4[] GetMatrixArray(string name)
	{
		return GetMatrixArray(Shader.PropertyToID(name));
	}

	public Matrix4x4[] GetMatrixArray(int nameID)
	{
		return (GetMatrixArrayCountImpl(nameID) != 0) ? GetMatrixArrayImpl(nameID) : null;
	}

	public void GetFloatArray(string name, List<float> values)
	{
		ExtractFloatArray(Shader.PropertyToID(name), values);
	}

	public void GetFloatArray(int nameID, List<float> values)
	{
		ExtractFloatArray(nameID, values);
	}

	public void GetVectorArray(string name, List<Vector4> values)
	{
		ExtractVectorArray(Shader.PropertyToID(name), values);
	}

	public void GetVectorArray(int nameID, List<Vector4> values)
	{
		ExtractVectorArray(nameID, values);
	}

	public void GetMatrixArray(string name, List<Matrix4x4> values)
	{
		ExtractMatrixArray(Shader.PropertyToID(name), values);
	}

	public void GetMatrixArray(int nameID, List<Matrix4x4> values)
	{
		ExtractMatrixArray(nameID, values);
	}

	public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromList(lightProbes), 0, 0, lightProbes.Count);
	}

	public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		CopySHCoefficientArraysFrom(lightProbes, 0, 0, lightProbes.Length);
	}

	public void CopySHCoefficientArraysFrom(List<SphericalHarmonicsL2> lightProbes, int sourceStart, int destStart, int count)
	{
		CopySHCoefficientArraysFrom(NoAllocHelpers.ExtractArrayFromList(lightProbes), sourceStart, destStart, count);
	}

	public void CopySHCoefficientArraysFrom(SphericalHarmonicsL2[] lightProbes, int sourceStart, int destStart, int count)
	{
		if (lightProbes == null)
		{
			throw new ArgumentNullException("lightProbes");
		}
		if (sourceStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
		}
		if (destStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
		}
		if (lightProbes.Length < sourceStart + count)
		{
			throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
		}
		Internal_CopySHCoefficientArraysFrom(this, lightProbes, sourceStart, destStart, count);
	}

	public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromList(occlusionProbes), 0, 0, occlusionProbes.Count);
	}

	public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		CopyProbeOcclusionArrayFrom(occlusionProbes, 0, 0, occlusionProbes.Length);
	}

	public void CopyProbeOcclusionArrayFrom(List<Vector4> occlusionProbes, int sourceStart, int destStart, int count)
	{
		CopyProbeOcclusionArrayFrom(NoAllocHelpers.ExtractArrayFromList(occlusionProbes), sourceStart, destStart, count);
	}

	public void CopyProbeOcclusionArrayFrom(Vector4[] occlusionProbes, int sourceStart, int destStart, int count)
	{
		if (occlusionProbes == null)
		{
			throw new ArgumentNullException("occlusionProbes");
		}
		if (sourceStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument sourceStart must not be negative.");
		}
		if (destStart < 0)
		{
			throw new ArgumentOutOfRangeException("sourceStart", "Argument destStart must not be negative.");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Argument count must not be negative.");
		}
		if (occlusionProbes.Length < sourceStart + count)
		{
			throw new ArgumentOutOfRangeException("The specified source start index or count is out of the range.");
		}
		Internal_CopyProbeOcclusionArrayFrom(this, occlusionProbes, sourceStart, destStart, count);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetIntImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetFloatImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVectorImpl_Injected(IntPtr _unity_self, int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetColorImpl_Injected(IntPtr _unity_self, int name, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMatrixImpl_Injected(IntPtr _unity_self, int name, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetTextureImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasPropertyImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasFloatImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasIntImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasTextureImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasMatrixImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasVectorImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasBufferImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasConstantBufferImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntImpl_Injected(IntPtr _unity_self, int name, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatImpl_Injected(IntPtr _unity_self, int name, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVectorImpl_Injected(IntPtr _unity_self, int name, [In] ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetColorImpl_Injected(IntPtr _unity_self, int name, [In] ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrixImpl_Injected(IntPtr _unity_self, int name, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetRenderTextureImpl_Injected(IntPtr _unity_self, int name, IntPtr value, RenderTextureSubElement element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetConstantGraphicsBufferImpl_Injected(IntPtr _unity_self, int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetFloatArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVectorArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetFloatArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVectorArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMatrixArrayCountImpl_Injected(IntPtr _unity_self, int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractFloatArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractVectorArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractMatrixArrayImpl_Injected(IntPtr _unity_self, int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CopySHCoefficientArraysFrom_Injected(IntPtr properties, ref ManagedSpanWrapper lightProbes, int sourceStart, int destStart, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CopyProbeOcclusionArrayFrom_Injected(IntPtr properties, ref ManagedSpanWrapper occlusionProbes, int sourceStart, int destStart, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isEmpty_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Clear_Injected(IntPtr _unity_self, bool keepMemory);
}
