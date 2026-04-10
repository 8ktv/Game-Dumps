using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal static class TextGenerationInfo
{
	public static int CurrentGenerationIteration { get; private set; }

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr Create(bool isPermanent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void Destroy(IntPtr ptr);

	public static void OnRepaintEnd()
	{
		CurrentGenerationIteration++;
		DestroyAllTempAllocations();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DestroyAllTempAllocations();

	[ThreadSafe]
	public static TextRenderingIndices GetTextRenderingIndices(IntPtr ptr, int glyphIndex)
	{
		GetTextRenderingIndices_Injected(ptr, glyphIndex, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetGlyphCount(IntPtr ptr);

	public static NativeTextInfo GetTextInfo(IntPtr ptr)
	{
		GetTextInfo_Injected(ptr, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextRenderingIndices_Injected(IntPtr ptr, int glyphIndex, out TextRenderingIndices ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTextInfo_Injected(IntPtr ptr, out NativeTextInfo ret);
}
