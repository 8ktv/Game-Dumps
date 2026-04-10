using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
public readonly struct LocalKeyword : IEquatable<LocalKeyword>
{
	internal readonly LocalKeywordSpace m_SpaceInfo;

	internal readonly string m_Name;

	internal readonly uint m_Index;

	public string name => m_Name;

	public bool isDynamic => IsDynamic(this);

	public bool isOverridable => IsOverridable(this);

	public bool isValid => IsValid(m_SpaceInfo, m_Index);

	public ShaderKeywordType type => GetKeywordType(m_SpaceInfo, m_Index);

	[FreeFunction("keywords::IsKeywordDynamic")]
	private static bool IsDynamic(LocalKeyword kw)
	{
		return IsDynamic_Injected(ref kw);
	}

	[FreeFunction("keywords::IsKeywordOverridable")]
	private static bool IsOverridable(LocalKeyword kw)
	{
		return IsOverridable_Injected(ref kw);
	}

	[FreeFunction("ShaderScripting::GetKeywordCount")]
	private static uint GetShaderKeywordCount(Shader shader)
	{
		return GetShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal(shader));
	}

	[FreeFunction("ShaderScripting::GetKeywordIndex")]
	private unsafe static uint GetShaderKeywordIndex(Shader shader, string keyword)
	{
		//The blocks IL_002f are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr shader2 = Object.MarshalledUnityObject.Marshal(shader);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
				}
			}
			return GetShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::GetKeywordCount")]
	private static uint GetComputeShaderKeywordCount(ComputeShader shader)
	{
		return GetComputeShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal(shader));
	}

	[FreeFunction("ShaderScripting::GetKeywordIndex")]
	private unsafe static uint GetComputeShaderKeywordIndex(ComputeShader shader, string keyword)
	{
		//The blocks IL_002f are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr shader2 = Object.MarshalledUnityObject.Marshal(shader);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetComputeShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
				}
			}
			return GetComputeShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::GetKeywordCount")]
	private static uint GetRayTracingShaderKeywordCount(RayTracingShader shader)
	{
		return GetRayTracingShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal(shader));
	}

	[FreeFunction("ShaderScripting::GetKeywordIndex")]
	private unsafe static uint GetRayTracingShaderKeywordIndex(RayTracingShader shader, string keyword)
	{
		//The blocks IL_002f are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr shader2 = Object.MarshalledUnityObject.Marshal(shader);
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(keyword);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetRayTracingShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
				}
			}
			return GetRayTracingShaderKeywordIndex_Injected(shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("keywords::GetKeywordType")]
	private static ShaderKeywordType GetKeywordType(LocalKeywordSpace spaceInfo, uint keyword)
	{
		return GetKeywordType_Injected(ref spaceInfo, keyword);
	}

	[FreeFunction("keywords::IsKeywordValid")]
	private static bool IsValid(LocalKeywordSpace spaceInfo, uint keyword)
	{
		return IsValid_Injected(ref spaceInfo, keyword);
	}

	public LocalKeyword(Shader shader, string name)
	{
		if (shader == null)
		{
			Debug.LogError("Cannot initialize a LocalKeyword with a null Shader.");
		}
		m_SpaceInfo = shader.keywordSpace;
		m_Name = name;
		m_Index = GetShaderKeywordIndex(shader, name);
		if (m_Index >= GetShaderKeywordCount(shader))
		{
			Debug.LogErrorFormat("Local keyword {0} doesn't exist in the shader.", name);
		}
	}

	public LocalKeyword(ComputeShader shader, string name)
	{
		if (shader == null)
		{
			Debug.LogError("Cannot initialize a LocalKeyword with a null ComputeShader.");
		}
		m_SpaceInfo = shader.keywordSpace;
		m_Name = name;
		m_Index = GetComputeShaderKeywordIndex(shader, name);
		if (m_Index >= GetComputeShaderKeywordCount(shader))
		{
			Debug.LogErrorFormat("Local keyword {0} doesn't exist in the compute shader.", name);
		}
	}

	public LocalKeyword(RayTracingShader shader, string name)
	{
		if (shader == null)
		{
			Debug.LogError("Cannot initialize a LocalKeyword with a null RayTracingShader.");
		}
		m_SpaceInfo = shader.keywordSpace;
		m_Name = name;
		m_Index = GetRayTracingShaderKeywordIndex(shader, name);
		if (m_Index >= GetRayTracingShaderKeywordCount(shader))
		{
			Debug.LogErrorFormat("Local keyword {0} doesn't exist in the ray tracing shader.", name);
		}
	}

	public override string ToString()
	{
		return m_Name;
	}

	public override bool Equals(object o)
	{
		return o is LocalKeyword rhs && Equals(rhs);
	}

	public bool Equals(LocalKeyword rhs)
	{
		return m_SpaceInfo == rhs.m_SpaceInfo && m_Index == rhs.m_Index;
	}

	public static bool operator ==(LocalKeyword lhs, LocalKeyword rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(LocalKeyword lhs, LocalKeyword rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return m_Index.GetHashCode() ^ m_SpaceInfo.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsDynamic_Injected([In] ref LocalKeyword kw);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsOverridable_Injected([In] ref LocalKeyword kw);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetShaderKeywordCount_Injected(IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetComputeShaderKeywordCount_Injected(IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetComputeShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetRayTracingShaderKeywordCount_Injected(IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetRayTracingShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShaderKeywordType GetKeywordType_Injected([In] ref LocalKeywordSpace spaceInfo, uint keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Injected([In] ref LocalKeywordSpace spaceInfo, uint keyword);
}
