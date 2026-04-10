using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
public struct ShaderKeyword
{
	internal string m_Name;

	internal uint m_Index;

	internal bool m_IsLocal;

	internal bool m_IsCompute;

	internal bool m_IsValid;

	public string name => m_Name;

	public int index => (int)m_Index;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetGlobalKeywordCount")]
	internal static extern uint GetGlobalKeywordCount();

	[FreeFunction("ShaderScripting::GetGlobalKeywordIndex")]
	internal unsafe static uint GetGlobalKeywordIndex(string keyword)
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
					return GetGlobalKeywordIndex_Injected(ref managedSpanWrapper);
				}
			}
			return GetGlobalKeywordIndex_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::GetKeywordCount")]
	internal static uint GetKeywordCount(Shader shader)
	{
		return GetKeywordCount_Injected(Object.MarshalledUnityObject.Marshal(shader));
	}

	[FreeFunction("ShaderScripting::GetKeywordIndex")]
	internal unsafe static uint GetKeywordIndex(Shader shader, string keyword)
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
					return GetKeywordIndex_Injected(shader2, ref managedSpanWrapper);
				}
			}
			return GetKeywordIndex_Injected(shader2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("ShaderScripting::GetKeywordCount")]
	internal static uint GetComputeShaderKeywordCount(ComputeShader shader)
	{
		return GetComputeShaderKeywordCount_Injected(Object.MarshalledUnityObject.Marshal(shader));
	}

	[FreeFunction("ShaderScripting::GetKeywordIndex")]
	internal unsafe static uint GetComputeShaderKeywordIndex(ComputeShader shader, string keyword)
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

	[FreeFunction("ShaderScripting::CreateGlobalKeyword")]
	internal unsafe static void CreateGlobalKeyword(string keyword)
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
					CreateGlobalKeyword_Injected(ref managedSpanWrapper);
					return;
				}
			}
			CreateGlobalKeyword_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("ShaderScripting::GetKeywordType")]
	internal static extern ShaderKeywordType GetGlobalShaderKeywordType(uint keyword);

	public static ShaderKeywordType GetGlobalKeywordType(ShaderKeyword index)
	{
		if (index.IsValid() && !index.m_IsLocal)
		{
			return GetGlobalShaderKeywordType(index.m_Index);
		}
		return ShaderKeywordType.UserDefined;
	}

	public ShaderKeyword(string keywordName)
	{
		m_Name = keywordName;
		m_Index = GetGlobalKeywordIndex(keywordName);
		if (m_Index >= GetGlobalKeywordCount())
		{
			CreateGlobalKeyword(keywordName);
			m_Index = GetGlobalKeywordIndex(keywordName);
		}
		m_IsValid = true;
		m_IsLocal = false;
		m_IsCompute = false;
	}

	public ShaderKeyword(Shader shader, string keywordName)
	{
		m_Name = keywordName;
		m_Index = GetKeywordIndex(shader, keywordName);
		m_IsValid = m_Index < GetKeywordCount(shader);
		m_IsLocal = true;
		m_IsCompute = false;
	}

	public ShaderKeyword(ComputeShader shader, string keywordName)
	{
		m_Name = keywordName;
		m_Index = GetComputeShaderKeywordIndex(shader, keywordName);
		m_IsValid = m_Index < GetComputeShaderKeywordCount(shader);
		m_IsLocal = true;
		m_IsCompute = true;
	}

	public static bool IsKeywordLocal(ShaderKeyword keyword)
	{
		return keyword.m_IsLocal;
	}

	public bool IsValid()
	{
		return m_IsValid;
	}

	public bool IsValid(ComputeShader shader)
	{
		return m_IsValid;
	}

	public bool IsValid(Shader shader)
	{
		return m_IsValid;
	}

	public override string ToString()
	{
		return m_Name;
	}

	[Obsolete("GetKeywordType is deprecated. Only global keywords can have a type. This method always returns ShaderKeywordType.UserDefined.")]
	public static ShaderKeywordType GetKeywordType(Shader shader, ShaderKeyword index)
	{
		return ShaderKeywordType.UserDefined;
	}

	[Obsolete("GetKeywordType is deprecated. Only global keywords can have a type. This method always returns ShaderKeywordType.UserDefined.")]
	public static ShaderKeywordType GetKeywordType(ComputeShader shader, ShaderKeyword index)
	{
		return ShaderKeywordType.UserDefined;
	}

	[Obsolete("GetGlobalKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
	public static string GetGlobalKeywordName(ShaderKeyword index)
	{
		return "";
	}

	[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
	public static string GetKeywordName(Shader shader, ShaderKeyword index)
	{
		return "";
	}

	[Obsolete("GetKeywordName is deprecated. Use the ShaderKeyword.name property instead.", true)]
	public static string GetKeywordName(ComputeShader shader, ShaderKeyword index)
	{
		return "";
	}

	[Obsolete("GetKeywordType is deprecated. Use ShaderKeyword.GetGlobalKeywordType instead.", true)]
	public ShaderKeywordType GetKeywordType()
	{
		return ShaderKeywordType.None;
	}

	[Obsolete("GetKeywordName is deprecated. Use ShaderKeyword.name instead.", true)]
	public string GetKeywordName()
	{
		return "";
	}

	[Obsolete("GetName() has been deprecated. Use ShaderKeyword.name instead.", true)]
	public string GetName()
	{
		return "";
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetGlobalKeywordIndex_Injected(ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetKeywordCount_Injected(IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetComputeShaderKeywordCount_Injected(IntPtr shader);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetComputeShaderKeywordIndex_Injected(IntPtr shader, ref ManagedSpanWrapper keyword);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CreateGlobalKeyword_Injected(ref ManagedSpanWrapper keyword);
}
