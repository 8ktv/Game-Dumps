using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "Unity.UIElements.PlayModeTests" })]
[NativeHeader("Modules/TextCoreTextEngine/Native/TextSelectionService.h")]
internal class TextSelectionService
{
	[NativeMethod(Name = "TextSelectionService::Substring")]
	internal static string Substring(IntPtr textGenerationInfo, int startIndex, int endIndex)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			Substring_Injected(textGenerationInfo, startIndex, endIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectCurrentWord")]
	internal static extern void SelectCurrentWord(IntPtr textGenerationInfo, int currentIndex, ref int startIndex, ref int endIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::PreviousCodePointIndex")]
	internal static extern int PreviousCodePointIndex(IntPtr textGenerationInfo, int currentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::NextCodePointIndex")]
	internal static extern int NextCodePointIndex(IntPtr textGenerationInfo, int currentIndex);

	[NativeMethod(Name = "TextSelectionService::GetCursorLogicalIndexFromPosition")]
	internal static int GetCursorLogicalIndexFromPosition(IntPtr textGenerationInfo, Vector2 position)
	{
		return GetCursorLogicalIndexFromPosition_Injected(textGenerationInfo, ref position);
	}

	[NativeMethod(Name = "TextSelectionService::GetCursorPositionFromLogicalIndex")]
	internal static Vector2 GetCursorPositionFromLogicalIndex(IntPtr textGenerationInfo, int logicalIndex)
	{
		GetCursorPositionFromLogicalIndex_Injected(textGenerationInfo, logicalIndex, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::LineUpCharacterPosition")]
	internal static extern int LineUpCharacterPosition(IntPtr textGenerationInfo, int originalPos);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::LineDownCharacterPosition")]
	internal static extern int LineDownCharacterPosition(IntPtr textGenerationInfo, int originalPos);

	[NativeMethod(Name = "TextSelectionService::GetHighlightRectangles")]
	internal static Rect[] GetHighlightRectangles(IntPtr textGenerationInfo, int cursorIndex, int selectIndex)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		Rect[] result;
		try
		{
			GetHighlightRectangles_Injected(textGenerationInfo, cursorIndex, selectIndex, out ret);
		}
		finally
		{
			Rect[] array = default(Rect[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetCharacterHeightFromIndex")]
	internal static extern float GetCharacterHeightFromIndex(IntPtr textGenerationInfo, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetStartOfNextWord")]
	internal static extern int GetStartOfNextWord(IntPtr textGenerationInfo, int currentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetEndOfPreviousWord")]
	internal static extern int GetEndOfPreviousWord(IntPtr textGenerationInfo, int currentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetFirstCharacterIndexOnLine")]
	internal static extern int GetFirstCharacterIndexOnLine(IntPtr textGenerationInfo, int currentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetLastCharacterIndexOnLine")]
	internal static extern int GetLastCharacterIndexOnLine(IntPtr textGenerationInfo, int currentIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetLineHeight")]
	internal static extern float GetLineHeight(IntPtr textGenerationInfo, int lineIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::GetLineNumberFromLogicalIndex")]
	internal static extern int GetLineNumber(IntPtr textGenerationInfo, int logicalIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectToPreviousParagraph")]
	internal static extern void SelectToPreviousParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectToStartOfParagraph")]
	internal static extern void SelectToStartOfParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectToEndOfParagraph")]
	internal static extern void SelectToEndOfParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectToNextParagraph")]
	internal static extern void SelectToNextParagraph(IntPtr textGenerationInfo, ref int cursorIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "TextSelectionService::SelectCurrentParagraph")]
	internal static extern void SelectCurrentParagraph(IntPtr textGenerationInfo, ref int cursorIndex, ref int selectIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Substring_Injected(IntPtr textGenerationInfo, int startIndex, int endIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCursorLogicalIndexFromPosition_Injected(IntPtr textGenerationInfo, [In] ref Vector2 position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetCursorPositionFromLogicalIndex_Injected(IntPtr textGenerationInfo, int logicalIndex, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetHighlightRectangles_Injected(IntPtr textGenerationInfo, int cursorIndex, int selectIndex, out BlittableArrayWrapper ret);
}
