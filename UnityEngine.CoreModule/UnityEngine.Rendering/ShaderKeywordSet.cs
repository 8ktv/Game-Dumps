#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering;

[UsedByNativeCode]
[NativeHeader("Editor/Src/Graphics/ShaderCompilerData.h")]
public struct ShaderKeywordSet
{
	private IntPtr m_KeywordState;

	private IntPtr m_Shader;

	private IntPtr m_ComputeShader;

	private ulong m_StateIndex;

	[FreeFunction("keywords::IsKeywordEnabled")]
	private static bool IsGlobalKeywordEnabled(ShaderKeywordSet state, uint index)
	{
		return IsGlobalKeywordEnabled_Injected(ref state, index);
	}

	[FreeFunction("keywords::IsKeywordEnabled")]
	private static bool IsKeywordEnabled(ShaderKeywordSet state, LocalKeywordSpace keywordSpace, uint index)
	{
		return IsKeywordEnabled_Injected(ref state, ref keywordSpace, index);
	}

	[FreeFunction("keywords::IsKeywordEnabled")]
	private unsafe static bool IsKeywordNameEnabled(ShaderKeywordSet state, string name)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsKeywordNameEnabled_Injected(ref state, ref managedSpanWrapper);
				}
			}
			return IsKeywordNameEnabled_Injected(ref state, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("keywords::EnableKeyword")]
	private static void EnableGlobalKeyword(ShaderKeywordSet state, uint index)
	{
		EnableGlobalKeyword_Injected(ref state, index);
	}

	[FreeFunction("keywords::EnableKeyword")]
	private unsafe static void EnableKeywordName(ShaderKeywordSet state, string name)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					EnableKeywordName_Injected(ref state, ref managedSpanWrapper);
					return;
				}
			}
			EnableKeywordName_Injected(ref state, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("keywords::DisableKeyword")]
	private static void DisableGlobalKeyword(ShaderKeywordSet state, uint index)
	{
		DisableGlobalKeyword_Injected(ref state, index);
	}

	[FreeFunction("keywords::DisableKeyword")]
	private unsafe static void DisableKeywordName(ShaderKeywordSet state, string name)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DisableKeywordName_Injected(ref state, ref managedSpanWrapper);
					return;
				}
			}
			DisableKeywordName_Injected(ref state, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("keywords::GetEnabledKeywords")]
	private static ShaderKeyword[] GetEnabledKeywords(ShaderKeywordSet state)
	{
		return GetEnabledKeywords_Injected(ref state);
	}

	private void CheckKeywordCompatible(ShaderKeyword keyword)
	{
		if (keyword.m_IsLocal)
		{
			if (m_Shader != IntPtr.Zero)
			{
				Assert.IsTrue(!keyword.m_IsCompute, "Trying to use a keyword that comes from a different shader.");
			}
			else
			{
				Assert.IsTrue(keyword.m_IsCompute, "Trying to use a keyword that comes from a different shader.");
			}
		}
	}

	public bool IsEnabled(ShaderKeyword keyword)
	{
		CheckKeywordCompatible(keyword);
		return IsKeywordNameEnabled(this, keyword.m_Name);
	}

	public bool IsEnabled(GlobalKeyword keyword)
	{
		return IsGlobalKeywordEnabled(this, keyword.m_Index);
	}

	public bool IsEnabled(LocalKeyword keyword)
	{
		return IsKeywordEnabled(this, keyword.m_SpaceInfo, keyword.m_Index);
	}

	public void Enable(ShaderKeyword keyword)
	{
		CheckKeywordCompatible(keyword);
		if (keyword.m_IsLocal || !keyword.IsValid())
		{
			EnableKeywordName(this, keyword.m_Name);
		}
		else
		{
			EnableGlobalKeyword(this, keyword.m_Index);
		}
	}

	public void Disable(ShaderKeyword keyword)
	{
		if (keyword.m_IsLocal || !keyword.IsValid())
		{
			DisableKeywordName(this, keyword.m_Name);
		}
		else
		{
			DisableGlobalKeyword(this, keyword.m_Index);
		}
	}

	public ShaderKeyword[] GetShaderKeywords()
	{
		return GetEnabledKeywords(this);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsGlobalKeywordEnabled_Injected([In] ref ShaderKeywordSet state, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordEnabled_Injected([In] ref ShaderKeywordSet state, [In] ref LocalKeywordSpace keywordSpace, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsKeywordNameEnabled_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableGlobalKeyword_Injected([In] ref ShaderKeywordSet state, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EnableKeywordName_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableGlobalKeyword_Injected([In] ref ShaderKeywordSet state, uint index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DisableKeywordName_Injected([In] ref ShaderKeywordSet state, ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ShaderKeyword[] GetEnabledKeywords_Injected([In] ref ShaderKeywordSet state);
}
