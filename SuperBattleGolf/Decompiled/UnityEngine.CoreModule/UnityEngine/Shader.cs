using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine;

[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[NativeHeader("Runtime/Shaders/Shader.h")]
[NativeHeader("Runtime/Misc/ResourceManager.h")]
[NativeHeader("Runtime/Shaders/ComputeShader.h")]
[NativeHeader("Runtime/Shaders/ShaderNameRegistry.h")]
[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
[NativeHeader("Runtime/Shaders/GpuPrograms/ShaderVariantCollection.h")]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
public sealed class Shader : Object
{
	[Obsolete("Use Graphics.activeTier instead (UnityUpgradable) -> UnityEngine.Graphics.activeTier", true)]
	public static ShaderHardwareTier globalShaderHardwareTier
	{
		get
		{
			return (ShaderHardwareTier)Graphics.activeTier;
		}
		set
		{
			Graphics.activeTier = (GraphicsTier)value;
		}
	}

	[NativeProperty("MaxChunksRuntimeOverride")]
	public static extern int maximumChunksOverride
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	[NativeProperty("MaximumShaderLOD")]
	public int maximumLOD
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maximumLOD_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maximumLOD_Injected(intPtr, value);
		}
	}

	[NativeProperty("GlobalMaximumShaderLOD")]
	public static extern int globalMaximumLOD
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public bool isSupported
	{
		[NativeMethod("IsSupported")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isSupported_Injected(intPtr);
		}
	}

	public unsafe static string globalRenderPipeline
	{
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				get_globalRenderPipeline_Injected(out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		set
		{
			//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			try
			{
				ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_globalRenderPipeline_Injected(ref managedSpanWrapper);
						return;
					}
				}
				set_globalRenderPipeline_Injected(ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public static GlobalKeyword[] enabledGlobalKeywords => GetEnabledGlobalKeywords();

	public static GlobalKeyword[] globalKeywords => GetAllGlobalKeywords();

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

	public int renderQueue
	{
		[FreeFunction("ShaderScripting::GetRenderQueue", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderQueue_Injected(intPtr);
		}
	}

	internal DisableBatchingType disableBatching
	{
		[FreeFunction("ShaderScripting::GetDisableBatchingType", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_disableBatching_Injected(intPtr);
		}
	}

	public int passCount
	{
		[FreeFunction(Name = "ShaderScripting::GetPassCount", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_passCount_Injected(intPtr);
		}
	}

	public int subshaderCount
	{
		[FreeFunction(Name = "ShaderScripting::GetSubshaderCount", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_subshaderCount_Injected(intPtr);
		}
	}

	public static Shader Find(string name)
	{
		return ResourcesAPI.ActiveAPI.FindShaderByName(name);
	}

	[FreeFunction("GetBuiltinResource<Shader>")]
	internal unsafe static Shader FindBuiltin(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr gcHandlePtr = default(IntPtr);
		Shader result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					gcHandlePtr = FindBuiltin_Injected(ref managedSpanWrapper);
				}
			}
			else
			{
				gcHandlePtr = FindBuiltin_Injected(ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Shader>(gcHandlePtr);
		}
		return result;
	}

	[FreeFunction("ShaderScripting::CreateFromCompiledData")]
	internal unsafe static Shader CreateFromCompiledData(byte[] compiledData, Shader[] dependencies)
	{
		Span<byte> span = new Span<byte>(compiledData);
		Shader result;
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper compiledData2 = new ManagedSpanWrapper(begin, span.Length);
			result = Unmarshal.UnmarshalUnityObject<Shader>(CreateFromCompiledData_Injected(ref compiledData2, dependencies));
		}
		return result;
	}

	[FreeFunction("keywords::GetEnabledGlobalKeywords")]
	internal static GlobalKeyword[] GetEnabledGlobalKeywords()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		GlobalKeyword[] result;
		try
		{
			GetEnabledGlobalKeywords_Injected(out ret);
		}
		finally
		{
			GlobalKeyword[] array = default(GlobalKeyword[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("keywords::GetAllGlobalKeywords")]
	internal static GlobalKeyword[] GetAllGlobalKeywords()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		GlobalKeyword[] result;
		try
		{
			GetAllGlobalKeywords_Injected(out ret);
		}
		finally
		{
			GlobalKeyword[] array = default(GlobalKeyword[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("ShaderScripting::EnableKeyword")]
	public unsafe static void EnableKeyword(string keyword)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					EnableKeyword_Injected(ref managedSpanWrapper);
					return;
				}
			}
			EnableKeyword_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::DisableKeyword")]
	public unsafe static void DisableKeyword(string keyword)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DisableKeyword_Injected(ref managedSpanWrapper);
					return;
				}
			}
			DisableKeyword_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::IsKeywordEnabled")]
	public unsafe static bool IsKeywordEnabled(string keyword)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsKeywordEnabled_Injected(ref managedSpanWrapper);
				}
			}
			return IsKeywordEnabled_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::EnableKeyword")]
	internal static void EnableKeywordFast(GlobalKeyword keyword)
	{
		EnableKeywordFast_Injected(ref keyword);
	}

	[FreeFunction("ShaderScripting::DisableKeyword")]
	internal static void DisableKeywordFast(GlobalKeyword keyword)
	{
		DisableKeywordFast_Injected(ref keyword);
	}

	[FreeFunction("ShaderScripting::SetKeyword")]
	internal static void SetKeywordFast(GlobalKeyword keyword, bool value)
	{
		SetKeywordFast_Injected(ref keyword, value);
	}

	[FreeFunction("ShaderScripting::IsKeywordEnabled")]
	internal static bool IsKeywordEnabledFast(GlobalKeyword keyword)
	{
		return IsKeywordEnabledFast_Injected(ref keyword);
	}

	public static void EnableKeyword(in GlobalKeyword keyword)
	{
		EnableKeywordFast(keyword);
	}

	public static void DisableKeyword(in GlobalKeyword keyword)
	{
		DisableKeywordFast(keyword);
	}

	public static void SetKeyword(in GlobalKeyword keyword, bool value)
	{
		SetKeywordFast(keyword, value);
	}

	public static bool IsKeywordEnabled(in GlobalKeyword keyword)
	{
		return IsKeywordEnabledFast(keyword);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalPropertyCount")]
	internal static extern int GetGlobalPropertyCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalPropertyCount")]
	private static extern int GetGlobalPropertyCountImpl(int propertyType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::ExtractGlobalPropertyNames")]
	private static extern void ExtractGlobalPropertyNamesImpl(int propertyType, [Out] string[] names);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction]
	public static extern void WarmupAllShaders();

	[FreeFunction("ShaderScripting::TagToID")]
	internal unsafe static int TagToID(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return TagToID_Injected(ref managedSpanWrapper);
				}
			}
			return TagToID_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::IDToTag")]
	internal static string IDToTag(int name)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IDToTag_Injected(name, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction(Name = "ShaderScripting::PropertyToID", IsThreadSafe = true)]
	public unsafe static int PropertyToID(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return PropertyToID_Injected(ref managedSpanWrapper);
				}
			}
			return PropertyToID_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe Shader GetDependency(string name)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		IntPtr dependency_Injected = default(IntPtr);
		Shader result;
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
					dependency_Injected = GetDependency_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			else
			{
				dependency_Injected = GetDependency_Injected(intPtr, ref managedSpanWrapper);
			}
		}
		finally
		{
			result = Unmarshal.UnmarshalUnityObject<Shader>(dependency_Injected);
		}
		return result;
	}

	[FreeFunction(Name = "ShaderScripting::GetPassCountInSubshader", HasExplicitThis = true)]
	public int GetPassCountInSubshader(int subshaderIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPassCountInSubshader_Injected(intPtr, subshaderIndex);
	}

	public ShaderTagId FindPassTagValue(int passIndex, ShaderTagId tagName)
	{
		if (passIndex < 0 || passIndex >= passCount)
		{
			throw new ArgumentOutOfRangeException("passIndex");
		}
		int id = Internal_FindPassTagValue(passIndex, tagName.id);
		return new ShaderTagId
		{
			id = id
		};
	}

	public ShaderTagId FindPassTagValue(int subshaderIndex, int passIndex, ShaderTagId tagName)
	{
		if (subshaderIndex < 0 || subshaderIndex >= subshaderCount)
		{
			throw new ArgumentOutOfRangeException("subshaderIndex");
		}
		if (passIndex < 0 || passIndex >= GetPassCountInSubshader(subshaderIndex))
		{
			throw new ArgumentOutOfRangeException("passIndex");
		}
		int id = Internal_FindPassTagValueInSubShader(subshaderIndex, passIndex, tagName.id);
		return new ShaderTagId
		{
			id = id
		};
	}

	public ShaderTagId FindSubshaderTagValue(int subshaderIndex, ShaderTagId tagName)
	{
		if (subshaderIndex < 0 || subshaderIndex >= subshaderCount)
		{
			throw new ArgumentOutOfRangeException($"Invalid subshaderIndex {subshaderIndex}. Value must be in the range [0, {subshaderCount})");
		}
		int id = Internal_FindSubshaderTagValue(subshaderIndex, tagName.id);
		return new ShaderTagId
		{
			id = id
		};
	}

	[FreeFunction(Name = "ShaderScripting::FindPassTagValue", HasExplicitThis = true)]
	private int Internal_FindPassTagValue(int passIndex, int tagName)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_FindPassTagValue_Injected(intPtr, passIndex, tagName);
	}

	[FreeFunction(Name = "ShaderScripting::FindPassTagValue", HasExplicitThis = true)]
	private int Internal_FindPassTagValueInSubShader(int subShaderIndex, int passIndex, int tagName)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_FindPassTagValueInSubShader_Injected(intPtr, subShaderIndex, passIndex, tagName);
	}

	[FreeFunction(Name = "ShaderScripting::FindSubshaderTagValue", HasExplicitThis = true)]
	private int Internal_FindSubshaderTagValue(int subShaderIndex, int tagName)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_FindSubshaderTagValue_Injected(intPtr, subShaderIndex, tagName);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalInt")]
	private static extern void SetGlobalIntImpl(int name, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::SetGlobalFloat")]
	private static extern void SetGlobalFloatImpl(int name, float value);

	[FreeFunction("ShaderScripting::SetGlobalVector")]
	private static void SetGlobalVectorImpl(int name, Vector4 value)
	{
		SetGlobalVectorImpl_Injected(name, ref value);
	}

	[FreeFunction("ShaderScripting::SetGlobalMatrix")]
	private static void SetGlobalMatrixImpl(int name, Matrix4x4 value)
	{
		SetGlobalMatrixImpl_Injected(name, ref value);
	}

	[FreeFunction("ShaderScripting::SetGlobalTexture")]
	private static void SetGlobalTextureImpl(int name, Texture value)
	{
		SetGlobalTextureImpl_Injected(name, MarshalledUnityObject.Marshal(value));
	}

	[FreeFunction("ShaderScripting::SetGlobalRenderTexture")]
	private static void SetGlobalRenderTextureImpl(int name, RenderTexture value, RenderTextureSubElement element)
	{
		SetGlobalRenderTextureImpl_Injected(name, MarshalledUnityObject.Marshal(value), element);
	}

	[FreeFunction("ShaderScripting::SetGlobalBuffer")]
	private static void SetGlobalBufferImpl(int name, ComputeBuffer value)
	{
		SetGlobalBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[FreeFunction("ShaderScripting::SetGlobalBuffer")]
	private static void SetGlobalGraphicsBufferImpl(int name, GraphicsBuffer value)
	{
		SetGlobalGraphicsBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
	}

	[FreeFunction("ShaderScripting::SetGlobalConstantBuffer")]
	private static void SetGlobalConstantBufferImpl(int name, ComputeBuffer value, int offset, int size)
	{
		SetGlobalConstantBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[FreeFunction("ShaderScripting::SetGlobalConstantBuffer")]
	private static void SetGlobalConstantGraphicsBufferImpl(int name, GraphicsBuffer value, int offset, int size)
	{
		SetGlobalConstantGraphicsBufferImpl_Injected(name, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value), offset, size);
	}

	[FreeFunction("ShaderScripting::SetGlobalRayTracingAccelerationStructure")]
	private static void SetGlobalRayTracingAccelerationStructureImpl(int name, RayTracingAccelerationStructure accelerationStructure)
	{
		SetGlobalRayTracingAccelerationStructureImpl_Injected(name, (accelerationStructure == null) ? ((IntPtr)0) : RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalInt")]
	private static extern int GetGlobalIntImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalFloat")]
	private static extern float GetGlobalFloatImpl(int name);

	[FreeFunction("ShaderScripting::GetGlobalVector")]
	private static Vector4 GetGlobalVectorImpl(int name)
	{
		GetGlobalVectorImpl_Injected(name, out var ret);
		return ret;
	}

	[FreeFunction("ShaderScripting::GetGlobalMatrix")]
	private static Matrix4x4 GetGlobalMatrixImpl(int name)
	{
		GetGlobalMatrixImpl_Injected(name, out var ret);
		return ret;
	}

	[FreeFunction("ShaderScripting::GetGlobalTexture")]
	private static Texture GetGlobalTextureImpl(int name)
	{
		return Unmarshal.UnmarshalUnityObject<Texture>(GetGlobalTextureImpl_Injected(name));
	}

	[FreeFunction("ShaderScripting::SetGlobalFloatArray")]
	private unsafe static void SetGlobalFloatArrayImpl(int name, float[] values, int count)
	{
		Span<float> span = new Span<float>(values);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetGlobalFloatArrayImpl_Injected(name, ref values2, count);
		}
	}

	[FreeFunction("ShaderScripting::SetGlobalVectorArray")]
	private unsafe static void SetGlobalVectorArrayImpl(int name, Vector4[] values, int count)
	{
		Span<Vector4> span = new Span<Vector4>(values);
		fixed (Vector4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetGlobalVectorArrayImpl_Injected(name, ref values2, count);
		}
	}

	[FreeFunction("ShaderScripting::SetGlobalMatrixArray")]
	private unsafe static void SetGlobalMatrixArrayImpl(int name, Matrix4x4[] values, int count)
	{
		Span<Matrix4x4> span = new Span<Matrix4x4>(values);
		fixed (Matrix4x4* begin = span)
		{
			ManagedSpanWrapper values2 = new ManagedSpanWrapper(begin, span.Length);
			SetGlobalMatrixArrayImpl_Injected(name, ref values2, count);
		}
	}

	[FreeFunction("ShaderScripting::GetGlobalFloatArray")]
	private static float[] GetGlobalFloatArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		float[] result;
		try
		{
			GetGlobalFloatArrayImpl_Injected(name, out ret);
		}
		finally
		{
			float[] array = default(float[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("ShaderScripting::GetGlobalVectorArray")]
	private static Vector4[] GetGlobalVectorArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Vector4[] result;
		try
		{
			GetGlobalVectorArrayImpl_Injected(name, out ret);
		}
		finally
		{
			Vector4[] array = default(Vector4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction("ShaderScripting::GetGlobalMatrixArray")]
	private static Matrix4x4[] GetGlobalMatrixArrayImpl(int name)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Matrix4x4[] result;
		try
		{
			GetGlobalMatrixArrayImpl_Injected(name, out ret);
		}
		finally
		{
			Matrix4x4[] array = default(Matrix4x4[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalFloatArrayCount")]
	private static extern int GetGlobalFloatArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalVectorArrayCount")]
	private static extern int GetGlobalVectorArrayCountImpl(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalMatrixArrayCount")]
	private static extern int GetGlobalMatrixArrayCountImpl(int name);

	[FreeFunction("ShaderScripting::ExtractGlobalFloatArray")]
	private unsafe static void ExtractGlobalFloatArrayImpl(int name, [Out] float[] val)
	{
		//The blocks IL_001c are reachable both inside and outside the pinned region starting at IL_0005. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			if (val != null)
			{
				fixed (float[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractGlobalFloatArrayImpl_Injected(name, out val2);
					return;
				}
			}
			ExtractGlobalFloatArrayImpl_Injected(name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[FreeFunction("ShaderScripting::ExtractGlobalVectorArray")]
	private unsafe static void ExtractGlobalVectorArrayImpl(int name, [Out] Vector4[] val)
	{
		//The blocks IL_001c are reachable both inside and outside the pinned region starting at IL_0005. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			if (val != null)
			{
				fixed (Vector4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractGlobalVectorArrayImpl_Injected(name, out val2);
					return;
				}
			}
			ExtractGlobalVectorArrayImpl_Injected(name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	[FreeFunction("ShaderScripting::ExtractGlobalMatrixArray")]
	private unsafe static void ExtractGlobalMatrixArrayImpl(int name, [Out] Matrix4x4[] val)
	{
		//The blocks IL_001c are reachable both inside and outside the pinned region starting at IL_0005. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper val2 = default(BlittableArrayWrapper);
		try
		{
			if (val != null)
			{
				fixed (Matrix4x4[] array = val)
				{
					if (array.Length != 0)
					{
						val2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					ExtractGlobalMatrixArrayImpl_Injected(name, out val2);
					return;
				}
			}
			ExtractGlobalMatrixArrayImpl_Injected(name, out val2);
		}
		finally
		{
			val2.Unmarshal(ref array);
		}
	}

	private static void SetGlobalFloatArray(int name, float[] values, int count)
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
		SetGlobalFloatArrayImpl(name, values, count);
	}

	private static void SetGlobalVectorArray(int name, Vector4[] values, int count)
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
		SetGlobalVectorArrayImpl(name, values, count);
	}

	private static void SetGlobalMatrixArray(int name, Matrix4x4[] values, int count)
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
		SetGlobalMatrixArrayImpl(name, values, count);
	}

	private static void ExtractGlobalFloatArray(int name, List<float> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalFloatArrayCountImpl = GetGlobalFloatArrayCountImpl(name);
		if (globalFloatArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalFloatArrayCountImpl);
			ExtractGlobalFloatArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private static void ExtractGlobalVectorArray(int name, List<Vector4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalVectorArrayCountImpl = GetGlobalVectorArrayCountImpl(name);
		if (globalVectorArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalVectorArrayCountImpl);
			ExtractGlobalVectorArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private static void ExtractGlobalMatrixArray(int name, List<Matrix4x4> values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		values.Clear();
		int globalMatrixArrayCountImpl = GetGlobalMatrixArrayCountImpl(name);
		if (globalMatrixArrayCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(values, globalMatrixArrayCountImpl);
			ExtractGlobalMatrixArrayImpl(name, NoAllocHelpers.ExtractArrayFromList(values));
		}
	}

	private static void ExtractGlobalPropertyNames(MaterialPropertyType type, List<string> names)
	{
		if (names == null)
		{
			throw new ArgumentNullException("names");
		}
		names.Clear();
		int globalPropertyCountImpl = GetGlobalPropertyCountImpl((int)type);
		if (globalPropertyCountImpl > 0)
		{
			NoAllocHelpers.EnsureListElemCount(names, globalPropertyCountImpl);
			ExtractGlobalPropertyNamesImpl((int)type, NoAllocHelpers.ExtractArrayFromList(names));
		}
	}

	public static void SetGlobalInt(string name, int value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	public static void SetGlobalInt(int nameID, int value)
	{
		SetGlobalFloatImpl(nameID, value);
	}

	public static void SetGlobalFloat(string name, float value)
	{
		SetGlobalFloatImpl(PropertyToID(name), value);
	}

	public static void SetGlobalFloat(int nameID, float value)
	{
		SetGlobalFloatImpl(nameID, value);
	}

	public static void SetGlobalInteger(string name, int value)
	{
		SetGlobalIntImpl(PropertyToID(name), value);
	}

	public static void SetGlobalInteger(int nameID, int value)
	{
		SetGlobalIntImpl(nameID, value);
	}

	public static void SetGlobalVector(string name, Vector4 value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	public static void SetGlobalVector(int nameID, Vector4 value)
	{
		SetGlobalVectorImpl(nameID, value);
	}

	public static void SetGlobalColor(string name, Color value)
	{
		SetGlobalVectorImpl(PropertyToID(name), value);
	}

	public static void SetGlobalColor(int nameID, Color value)
	{
		SetGlobalVectorImpl(nameID, value);
	}

	public static void SetGlobalMatrix(string name, Matrix4x4 value)
	{
		SetGlobalMatrixImpl(PropertyToID(name), value);
	}

	public static void SetGlobalMatrix(int nameID, Matrix4x4 value)
	{
		SetGlobalMatrixImpl(nameID, value);
	}

	public static void SetGlobalTexture(string name, Texture value)
	{
		SetGlobalTextureImpl(PropertyToID(name), value);
	}

	public static void SetGlobalTexture(int nameID, Texture value)
	{
		SetGlobalTextureImpl(nameID, value);
	}

	public static void SetGlobalTexture(string name, RenderTexture value, RenderTextureSubElement element)
	{
		SetGlobalRenderTextureImpl(PropertyToID(name), value, element);
	}

	public static void SetGlobalTexture(int nameID, RenderTexture value, RenderTextureSubElement element)
	{
		SetGlobalRenderTextureImpl(nameID, value, element);
	}

	public static void SetGlobalBuffer(string name, ComputeBuffer value)
	{
		SetGlobalBufferImpl(PropertyToID(name), value);
	}

	public static void SetGlobalBuffer(int nameID, ComputeBuffer value)
	{
		SetGlobalBufferImpl(nameID, value);
	}

	public static void SetGlobalBuffer(string name, GraphicsBuffer value)
	{
		SetGlobalGraphicsBufferImpl(PropertyToID(name), value);
	}

	public static void SetGlobalBuffer(int nameID, GraphicsBuffer value)
	{
		SetGlobalGraphicsBufferImpl(nameID, value);
	}

	public static void SetGlobalConstantBuffer(string name, ComputeBuffer value, int offset, int size)
	{
		SetGlobalConstantBufferImpl(PropertyToID(name), value, offset, size);
	}

	public static void SetGlobalConstantBuffer(int nameID, ComputeBuffer value, int offset, int size)
	{
		SetGlobalConstantBufferImpl(nameID, value, offset, size);
	}

	public static void SetGlobalConstantBuffer(string name, GraphicsBuffer value, int offset, int size)
	{
		SetGlobalConstantGraphicsBufferImpl(PropertyToID(name), value, offset, size);
	}

	public static void SetGlobalConstantBuffer(int nameID, GraphicsBuffer value, int offset, int size)
	{
		SetGlobalConstantGraphicsBufferImpl(nameID, value, offset, size);
	}

	public static void SetGlobalRayTracingAccelerationStructure(string name, RayTracingAccelerationStructure value)
	{
		SetGlobalRayTracingAccelerationStructureImpl(PropertyToID(name), value);
	}

	public static void SetGlobalRayTracingAccelerationStructure(int nameID, RayTracingAccelerationStructure value)
	{
		SetGlobalRayTracingAccelerationStructureImpl(nameID, value);
	}

	public static void SetGlobalFloatArray(string name, List<float> values)
	{
		SetGlobalFloatArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalFloatArray(int nameID, List<float> values)
	{
		SetGlobalFloatArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalFloatArray(string name, float[] values)
	{
		SetGlobalFloatArray(PropertyToID(name), values, values.Length);
	}

	public static void SetGlobalFloatArray(int nameID, float[] values)
	{
		SetGlobalFloatArray(nameID, values, values.Length);
	}

	public static void SetGlobalVectorArray(string name, List<Vector4> values)
	{
		SetGlobalVectorArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalVectorArray(int nameID, List<Vector4> values)
	{
		SetGlobalVectorArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalVectorArray(string name, Vector4[] values)
	{
		SetGlobalVectorArray(PropertyToID(name), values, values.Length);
	}

	public static void SetGlobalVectorArray(int nameID, Vector4[] values)
	{
		SetGlobalVectorArray(nameID, values, values.Length);
	}

	public static void SetGlobalMatrixArray(string name, List<Matrix4x4> values)
	{
		SetGlobalMatrixArray(PropertyToID(name), NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
	{
		SetGlobalMatrixArray(nameID, NoAllocHelpers.ExtractArrayFromList(values), values.Count);
	}

	public static void SetGlobalMatrixArray(string name, Matrix4x4[] values)
	{
		SetGlobalMatrixArray(PropertyToID(name), values, values.Length);
	}

	public static void SetGlobalMatrixArray(int nameID, Matrix4x4[] values)
	{
		SetGlobalMatrixArray(nameID, values, values.Length);
	}

	public static int GetGlobalInt(string name)
	{
		return (int)GetGlobalFloatImpl(PropertyToID(name));
	}

	public static int GetGlobalInt(int nameID)
	{
		return (int)GetGlobalFloatImpl(nameID);
	}

	public static float GetGlobalFloat(string name)
	{
		return GetGlobalFloatImpl(PropertyToID(name));
	}

	public static float GetGlobalFloat(int nameID)
	{
		return GetGlobalFloatImpl(nameID);
	}

	public static int GetGlobalInteger(string name)
	{
		return GetGlobalIntImpl(PropertyToID(name));
	}

	public static int GetGlobalInteger(int nameID)
	{
		return GetGlobalIntImpl(nameID);
	}

	public static Vector4 GetGlobalVector(string name)
	{
		return GetGlobalVectorImpl(PropertyToID(name));
	}

	public static Vector4 GetGlobalVector(int nameID)
	{
		return GetGlobalVectorImpl(nameID);
	}

	public static Color GetGlobalColor(string name)
	{
		return GetGlobalVectorImpl(PropertyToID(name));
	}

	public static Color GetGlobalColor(int nameID)
	{
		return GetGlobalVectorImpl(nameID);
	}

	public static Matrix4x4 GetGlobalMatrix(string name)
	{
		return GetGlobalMatrixImpl(PropertyToID(name));
	}

	public static Matrix4x4 GetGlobalMatrix(int nameID)
	{
		return GetGlobalMatrixImpl(nameID);
	}

	public static Texture GetGlobalTexture(string name)
	{
		return GetGlobalTextureImpl(PropertyToID(name));
	}

	public static Texture GetGlobalTexture(int nameID)
	{
		return GetGlobalTextureImpl(nameID);
	}

	public static float[] GetGlobalFloatArray(string name)
	{
		return GetGlobalFloatArray(PropertyToID(name));
	}

	public static float[] GetGlobalFloatArray(int nameID)
	{
		return (GetGlobalFloatArrayCountImpl(nameID) != 0) ? GetGlobalFloatArrayImpl(nameID) : null;
	}

	public static Vector4[] GetGlobalVectorArray(string name)
	{
		return GetGlobalVectorArray(PropertyToID(name));
	}

	public static Vector4[] GetGlobalVectorArray(int nameID)
	{
		return (GetGlobalVectorArrayCountImpl(nameID) != 0) ? GetGlobalVectorArrayImpl(nameID) : null;
	}

	public static Matrix4x4[] GetGlobalMatrixArray(string name)
	{
		return GetGlobalMatrixArray(PropertyToID(name));
	}

	public static Matrix4x4[] GetGlobalMatrixArray(int nameID)
	{
		return (GetGlobalMatrixArrayCountImpl(nameID) != 0) ? GetGlobalMatrixArrayImpl(nameID) : null;
	}

	public static void GetGlobalFloatArray(string name, List<float> values)
	{
		ExtractGlobalFloatArray(PropertyToID(name), values);
	}

	public static void GetGlobalFloatArray(int nameID, List<float> values)
	{
		ExtractGlobalFloatArray(nameID, values);
	}

	public static void GetGlobalVectorArray(string name, List<Vector4> values)
	{
		ExtractGlobalVectorArray(PropertyToID(name), values);
	}

	public static void GetGlobalVectorArray(int nameID, List<Vector4> values)
	{
		ExtractGlobalVectorArray(nameID, values);
	}

	public static void GetGlobalMatrixArray(string name, List<Matrix4x4> values)
	{
		ExtractGlobalMatrixArray(PropertyToID(name), values);
	}

	public static void GetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
	{
		ExtractGlobalMatrixArray(nameID, values);
	}

	internal static void GetGlobalPropertyNames(MaterialPropertyType type, List<string> names)
	{
		ExtractGlobalPropertyNames(type, names);
	}

	private Shader()
	{
	}

	[FreeFunction("ShaderScripting::GetPropertyName")]
	private static string GetPropertyName([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			GetPropertyName_Injected(intPtr, propertyIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("ShaderScripting::GetPropertyNameId")]
	private static int GetPropertyNameId([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyNameId_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyType")]
	private static ShaderPropertyType GetPropertyType([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyType_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyDescription")]
	private static string GetPropertyDescription([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			GetPropertyDescription_Injected(intPtr, propertyIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("ShaderScripting::GetPropertyFlags")]
	private static ShaderPropertyFlags GetPropertyFlags([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyFlags_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyAttributes")]
	private static string[] GetPropertyAttributes([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyAttributes_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyDefaultIntValue")]
	private static int GetPropertyDefaultIntValue([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyDefaultIntValue_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyDefaultValue")]
	private static Vector4 GetPropertyDefaultValue([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		GetPropertyDefaultValue_Injected(intPtr, propertyIndex, out var ret);
		return ret;
	}

	[FreeFunction("ShaderScripting::GetPropertyTextureDimension")]
	private static TextureDimension GetPropertyTextureDimension([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		return GetPropertyTextureDimension_Injected(intPtr, propertyIndex);
	}

	[FreeFunction("ShaderScripting::GetPropertyTextureDefaultName")]
	private static string GetPropertyTextureDefaultName([NotNull] Shader shader, int propertyIndex)
	{
		if ((object)shader == null)
		{
			ThrowHelper.ThrowArgumentNullException(shader, "shader");
		}
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(shader);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(shader, "shader");
			}
			GetPropertyTextureDefaultName_Injected(intPtr, propertyIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction("ShaderScripting::FindTextureStack")]
	private static bool FindTextureStackImpl([NotNull] Shader s, int propertyIdx, out string stackName, out int layerIndex)
	{
		if ((object)s == null)
		{
			ThrowHelper.ThrowArgumentNullException(s, "s");
		}
		ManagedSpanWrapper stackName2 = default(ManagedSpanWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(s);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(s, "s");
			}
			return FindTextureStackImpl_Injected(intPtr, propertyIdx, out stackName2, out layerIndex);
		}
		finally
		{
			stackName = OutStringMarshaller.GetStringAndDispose(stackName2);
		}
	}

	private static void CheckPropertyIndex(Shader s, int propertyIndex)
	{
		if (propertyIndex < 0 || propertyIndex >= s.GetPropertyCount())
		{
			throw new ArgumentOutOfRangeException("propertyIndex");
		}
	}

	public int GetPropertyCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPropertyCount_Injected(intPtr);
	}

	public unsafe int FindPropertyIndex(string propertyName)
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
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(propertyName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindPropertyIndex_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return FindPropertyIndex_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public string GetPropertyName(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyName(this, propertyIndex);
	}

	public int GetPropertyNameId(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyNameId(this, propertyIndex);
	}

	public ShaderPropertyType GetPropertyType(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyType(this, propertyIndex);
	}

	public string GetPropertyDescription(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyDescription(this, propertyIndex);
	}

	public ShaderPropertyFlags GetPropertyFlags(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyFlags(this, propertyIndex);
	}

	public string[] GetPropertyAttributes(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		return GetPropertyAttributes(this, propertyIndex);
	}

	public float GetPropertyDefaultFloatValue(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		ShaderPropertyType propertyType = GetPropertyType(propertyIndex);
		if (propertyType != ShaderPropertyType.Float && propertyType != ShaderPropertyType.Range)
		{
			throw new ArgumentException("Property type is not Float or Range.");
		}
		return GetPropertyDefaultValue(this, propertyIndex)[0];
	}

	public Vector4 GetPropertyDefaultVectorValue(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		ShaderPropertyType propertyType = GetPropertyType(propertyIndex);
		if (propertyType != ShaderPropertyType.Color && propertyType != ShaderPropertyType.Vector)
		{
			throw new ArgumentException("Property type is not Color or Vector.");
		}
		return GetPropertyDefaultValue(this, propertyIndex);
	}

	public Vector2 GetPropertyRangeLimits(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		if (GetPropertyType(propertyIndex) != ShaderPropertyType.Range)
		{
			throw new ArgumentException("Property type is not Range.");
		}
		Vector4 propertyDefaultValue = GetPropertyDefaultValue(this, propertyIndex);
		return new Vector2(propertyDefaultValue[1], propertyDefaultValue[2]);
	}

	public int GetPropertyDefaultIntValue(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		if (GetPropertyType(propertyIndex) != ShaderPropertyType.Int)
		{
			throw new ArgumentException("Property type is not Int.");
		}
		return GetPropertyDefaultIntValue(this, propertyIndex);
	}

	public TextureDimension GetPropertyTextureDimension(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		if (GetPropertyType(propertyIndex) != ShaderPropertyType.Texture)
		{
			throw new ArgumentException("Property type is not TexEnv.");
		}
		return GetPropertyTextureDimension(this, propertyIndex);
	}

	public string GetPropertyTextureDefaultName(int propertyIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		ShaderPropertyType propertyType = GetPropertyType(propertyIndex);
		if (propertyType != ShaderPropertyType.Texture)
		{
			throw new ArgumentException("Property type is not Texture.");
		}
		return GetPropertyTextureDefaultName(this, propertyIndex);
	}

	public bool FindTextureStack(int propertyIndex, out string stackName, out int layerIndex)
	{
		CheckPropertyIndex(this, propertyIndex);
		ShaderPropertyType propertyType = GetPropertyType(propertyIndex);
		if (propertyType != ShaderPropertyType.Texture)
		{
			throw new ArgumentException("Property type is not Texture.");
		}
		return FindTextureStackImpl(this, propertyIndex, out stackName, out layerIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindBuiltin_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr CreateFromCompiledData_Injected(ref ManagedSpanWrapper compiledData, Shader[] dependencies);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_maximumLOD_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maximumLOD_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isSupported_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_globalRenderPipeline_Injected(out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_globalRenderPipeline_Injected(ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_keywordSpace_Injected(IntPtr _unity_self, out LocalKeywordSpace ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetEnabledGlobalKeywords_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetAllGlobalKeywords_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableKeyword_Injected(ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableKeyword_Injected(ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordEnabled_Injected(ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableKeywordFast_Injected([In] ref GlobalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableKeywordFast_Injected([In] ref GlobalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetKeywordFast_Injected([In] ref GlobalKeyword keyword, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordEnabledFast_Injected([In] ref GlobalKeyword keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_renderQueue_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern DisableBatchingType get_disableBatching_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int TagToID_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IDToTag_Injected(int name, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int PropertyToID_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDependency_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_passCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_subshaderCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPassCountInSubshader_Injected(IntPtr _unity_self, int subshaderIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_FindPassTagValue_Injected(IntPtr _unity_self, int passIndex, int tagName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_FindPassTagValueInSubShader_Injected(IntPtr _unity_self, int subShaderIndex, int passIndex, int tagName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Internal_FindSubshaderTagValue_Injected(IntPtr _unity_self, int subShaderIndex, int tagName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalVectorImpl_Injected(int name, [In] ref Vector4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalMatrixImpl_Injected(int name, [In] ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalTextureImpl_Injected(int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalRenderTextureImpl_Injected(int name, IntPtr value, RenderTextureSubElement element);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalBufferImpl_Injected(int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalGraphicsBufferImpl_Injected(int name, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalConstantBufferImpl_Injected(int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalConstantGraphicsBufferImpl_Injected(int name, IntPtr value, int offset, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalRayTracingAccelerationStructureImpl_Injected(int name, IntPtr accelerationStructure);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalVectorImpl_Injected(int name, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalMatrixImpl_Injected(int name, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetGlobalTextureImpl_Injected(int name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalFloatArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalVectorArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetGlobalMatrixArrayImpl_Injected(int name, ref ManagedSpanWrapper values, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalFloatArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalVectorArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetGlobalMatrixArrayImpl_Injected(int name, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractGlobalFloatArrayImpl_Injected(int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractGlobalVectorArrayImpl_Injected(int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ExtractGlobalMatrixArrayImpl_Injected(int name, out BlittableArrayWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPropertyName_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPropertyNameId_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShaderPropertyType GetPropertyType_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPropertyDescription_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShaderPropertyFlags GetPropertyFlags_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern string[] GetPropertyAttributes_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPropertyDefaultIntValue_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPropertyDefaultValue_Injected(IntPtr shader, int propertyIndex, out Vector4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextureDimension GetPropertyTextureDimension_Injected(IntPtr shader, int propertyIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPropertyTextureDefaultName_Injected(IntPtr shader, int propertyIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool FindTextureStackImpl_Injected(IntPtr s, int propertyIdx, out ManagedSpanWrapper stackName, out int layerIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetPropertyCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int FindPropertyIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper propertyName);
}
