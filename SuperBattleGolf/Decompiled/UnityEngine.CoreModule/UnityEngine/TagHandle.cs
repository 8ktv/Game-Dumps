using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
[NativeHeader("Runtime/BaseClasses/TagManager.h")]
public struct TagHandle
{
	private uint _tagIndex;

	public static TagHandle GetExistingTag(string tagName)
	{
		return new TagHandle
		{
			_tagIndex = ExtractTagThrowing(tagName)
		};
	}

	public override string ToString()
	{
		return TagToString(_tagIndex);
	}

	[NativeHeader("Runtime/Export/Scripting/GameObject.bindings.h")]
	[FreeFunction]
	[NativeThrows]
	private unsafe static uint ExtractTagThrowing(string tagName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tagName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tagName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ExtractTagThrowing_Injected(ref managedSpanWrapper);
				}
			}
			return ExtractTagThrowing_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private static string TagToString(uint tagIndex)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			TagToString_Injected(tagIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint ExtractTagThrowing_Injected(ref ManagedSpanWrapper tagName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TagToString_Injected(uint tagIndex, out ManagedSpanWrapper ret);
}
