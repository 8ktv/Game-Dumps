using System;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Unsafe/UTF8StringView.bindings.h")]
internal readonly struct UTF8StringView
{
	public readonly IntPtr utf8Ptr;

	public readonly int utf8Length;

	public unsafe UTF8StringView(ReadOnlySpan<byte> prefixUtf8Span)
	{
		fixed (byte* value = &prefixUtf8Span[0])
		{
			utf8Ptr = new IntPtr(value);
		}
		utf8Length = prefixUtf8Span.Length;
	}

	public UTF8StringView(IntPtr ptr, int lengthUtf8)
	{
		utf8Ptr = ptr;
		utf8Length = lengthUtf8;
	}

	public unsafe UTF8StringView(byte* ptr, int lengthUtf8)
	{
		utf8Ptr = new IntPtr(ptr);
		utf8Length = lengthUtf8;
	}

	public unsafe override string ToString()
	{
		return Encoding.UTF8.GetString((byte*)utf8Ptr.ToPointer(), utf8Length);
	}
}
