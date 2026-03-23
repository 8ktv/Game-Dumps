using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Shaders/Keywords/KeywordSpaceScriptBindings.h")]
public readonly struct LocalKeywordSpace : IEquatable<LocalKeywordSpace>
{
	private readonly IntPtr m_KeywordSpace;

	public LocalKeyword[] keywords => GetKeywords();

	public string[] keywordNames => GetKeywordNames();

	public uint keywordCount => GetKeywordCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("keywords::GetKeywords", HasExplicitThis = true)]
	private extern LocalKeyword[] GetKeywords();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("keywords::GetKeywordNames", HasExplicitThis = true)]
	private extern string[] GetKeywordNames();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("keywords::GetKeywordCount", HasExplicitThis = true)]
	private extern uint GetKeywordCount();

	[FreeFunction("keywords::GetKeyword", HasExplicitThis = true)]
	private unsafe LocalKeyword GetKeyword(string name)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		LocalKeyword ret = default(LocalKeyword);
		LocalKeyword result;
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetKeyword_Injected(ref this, ref managedSpanWrapper, out ret);
				}
			}
			else
			{
				GetKeyword_Injected(ref this, ref managedSpanWrapper, out ret);
			}
		}
		finally
		{
			result = ret;
		}
		return result;
	}

	public LocalKeyword FindKeyword(string name)
	{
		return GetKeyword(name);
	}

	public override bool Equals(object o)
	{
		return o is LocalKeywordSpace rhs && Equals(rhs);
	}

	public bool Equals(LocalKeywordSpace rhs)
	{
		return m_KeywordSpace == rhs.m_KeywordSpace;
	}

	public static bool operator ==(LocalKeywordSpace lhs, LocalKeywordSpace rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(LocalKeywordSpace lhs, LocalKeywordSpace rhs)
	{
		return !(lhs == rhs);
	}

	public override int GetHashCode()
	{
		return m_KeywordSpace.GetHashCode();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetKeyword_Injected(ref LocalKeywordSpace _unity_self, ref ManagedSpanWrapper name, out LocalKeyword ret);
}
