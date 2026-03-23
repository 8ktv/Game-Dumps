using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal static class TextGenerationInfo
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern void Destroy(IntPtr ptr);

	[ThreadSafe]
	public static TextRenderingIndices GetTextRenderingIndices(IntPtr ptr, int glyphIndex)
	{
		GetTextRenderingIndices_Injected(ptr, glyphIndex, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetGlyphCount(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextRenderingIndices_Injected(IntPtr ptr, int glyphIndex, out TextRenderingIndices ret);
}
