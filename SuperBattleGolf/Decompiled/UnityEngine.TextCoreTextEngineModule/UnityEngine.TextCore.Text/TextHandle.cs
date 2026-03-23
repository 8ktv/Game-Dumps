using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[DebuggerDisplay("{settings.text}")]
[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal class TextHandle
{
	internal readonly record struct GlyphMetricsForOverlay
	{
		public readonly bool isVisible;

		public readonly float origin;

		public readonly float xAdvance;

		public readonly float ascentline;

		public readonly float baseline;

		public readonly float descentline;

		public readonly Vector3 topLeft;

		public readonly Vector3 bottomLeft;

		public readonly Vector3 topRight;

		public readonly Vector3 bottomRight;

		public readonly float scale;

		public readonly int lineNumber;

		public readonly float fontCapLine;

		public readonly float fontMeanLine;

		public GlyphMetricsForOverlay(ref TextElementInfo textElementInfo, float pixelPerPoint)
		{
			float num = 1f / pixelPerPoint;
			isVisible = textElementInfo.isVisible;
			origin = textElementInfo.origin * num;
			xAdvance = textElementInfo.xAdvance * num;
			ascentline = textElementInfo.ascender * num;
			baseline = textElementInfo.baseLine * num;
			descentline = textElementInfo.descender * num;
			topLeft = textElementInfo.topLeft * num;
			bottomLeft = textElementInfo.bottomLeft * num;
			topRight = textElementInfo.topRight * num;
			bottomRight = textElementInfo.bottomRight * num;
			scale = textElementInfo.scale;
			lineNumber = textElementInfo.lineNumber;
			fontCapLine = textElementInfo.fontAsset.faceInfo.capLine * num;
			fontMeanLine = textElementInfo.fontAsset.faceInfo.meanLine * num;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static TextHandleTemporaryCache s_TemporaryCache = new TextHandleTemporaryCache();

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static TextHandlePermanentCache s_PermanentCache = new TextHandlePermanentCache();

	private static TextGenerationSettings[] s_Settings;

	private static TextGenerator[] s_Generators;

	private static TextInfo[] s_TextInfosCommon;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal NativeTextGenerationSettings nativeSettings = NativeTextGenerationSettings.Default;

	protected Vector2 pixelPreferedSize;

	private Rect m_ScreenRect;

	private float m_LineHeightDefault;

	private bool m_IsPlaceholder;

	protected bool m_IsElided;

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal IntPtr textGenerationInfo = IntPtr.Zero;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal int m_PreviousGenerationSettingsHash;

	protected bool isDirty;

	internal static TextGenerationSettings[] settingsArray
	{
		get
		{
			if (s_Settings == null)
			{
				InitArray(ref s_Settings, () => new TextGenerationSettings());
			}
			return s_Settings;
		}
	}

	internal static TextGenerator[] generators
	{
		get
		{
			if (s_Generators == null)
			{
				InitArray(ref s_Generators, () => new TextGenerator());
			}
			return s_Generators;
		}
	}

	internal static TextInfo[] textInfosCommon
	{
		get
		{
			if (s_TextInfosCommon == null)
			{
				InitArray(ref s_TextInfosCommon, () => new TextInfo());
			}
			return s_TextInfosCommon;
		}
	}

	internal static TextInfo textInfoCommon => textInfosCommon[JobsUtility.ThreadIndex];

	private static TextGenerator generator => generators[JobsUtility.ThreadIndex];

	internal static TextGenerationSettings settings
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		get
		{
			return settingsArray[JobsUtility.ThreadIndex];
		}
	}

	internal Vector2 preferredSize
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		get
		{
			return PixelsToPoints(pixelPreferedSize);
		}
	}

	internal LinkedListNode<TextInfo> TextInfoNode { get; set; }

	internal bool IsCachedPermanent { get; set; }

	internal bool IsCachedTemporary { get; set; }

	internal bool useAdvancedText
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		get
		{
			return IsAdvancedTextEnabledForElement();
		}
	}

	internal int characterCount
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		get
		{
			return useAdvancedText ? nativeSettings.text.Length : textInfo.characterCount;
		}
	}

	internal TextInfo textInfo
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		get
		{
			if (TextInfoNode == null)
			{
				return textInfoCommon;
			}
			return TextInfoNode.Value;
		}
	}

	public virtual bool IsPlaceholder => m_IsPlaceholder;

	~TextHandle()
	{
		RemoveTextInfoFromTemporaryCache();
		RemoveTextInfoFromPermanentCache();
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal static void InitThreadArrays()
	{
		if (s_Settings == null || s_Generators == null || s_TextInfosCommon == null)
		{
			InitArray(ref s_Settings, () => new TextGenerationSettings());
			InitArray(ref s_Generators, () => new TextGenerator());
			InitArray(ref s_TextInfosCommon, () => new TextInfo());
		}
	}

	private static void InitArray<T>(ref T[] array, Func<T> createInstance)
	{
		if (array == null)
		{
			array = new T[JobsUtility.ThreadIndexCount];
			for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
			{
				array[i] = createInstance();
			}
		}
	}

	protected float PointsToPixels(float point)
	{
		return point * GetPixelsPerPoint();
	}

	protected float PixelsToPoints(float pixel)
	{
		return pixel / GetPixelsPerPoint();
	}

	protected Vector2 PointsToPixels(Vector2 point)
	{
		return point * GetPixelsPerPoint();
	}

	protected Vector2 PixelsToPoints(Vector2 pixel)
	{
		return pixel / GetPixelsPerPoint();
	}

	protected virtual float GetPixelsPerPoint()
	{
		return 1f;
	}

	public virtual void AddToPermanentCacheAndGenerateMesh()
	{
		if (useAdvancedText)
		{
			throw new InvalidOperationException("Method is virtual and should be overriden in ATGTextHanle, the only valid handle for ATG");
		}
		s_PermanentCache.AddTextInfoToCache(this);
	}

	public void AddTextInfoToTemporaryCache(int hashCode)
	{
		if (!useAdvancedText)
		{
			s_TemporaryCache.AddTextInfoToCache(this, hashCode);
		}
	}

	public void RemoveTextInfoFromTemporaryCache()
	{
		s_TemporaryCache.RemoveTextInfoFromCache(this);
	}

	public void RemoveTextInfoFromPermanentCache()
	{
		if (textGenerationInfo != IntPtr.Zero)
		{
			TextGenerationInfo.Destroy(textGenerationInfo);
			textGenerationInfo = IntPtr.Zero;
		}
		else
		{
			s_PermanentCache.RemoveTextInfoFromCache(this);
		}
	}

	public static void UpdateCurrentFrame()
	{
		s_TemporaryCache.UpdateCurrentFrame();
	}

	internal bool IsTextInfoAllocated()
	{
		return textInfo != null;
	}

	public virtual void SetDirty()
	{
		isDirty = true;
	}

	public bool IsDirty(int hashCode)
	{
		if (m_PreviousGenerationSettingsHash == hashCode && !isDirty && (IsCachedTemporary || IsCachedPermanent))
		{
			return false;
		}
		return true;
	}

	public float ComputeTextWidth(TextGenerationSettings tgs)
	{
		UpdatePreferredValues(tgs);
		return preferredSize.x;
	}

	public float ComputeTextHeight(TextGenerationSettings tgs)
	{
		UpdatePreferredValues(tgs);
		return preferredSize.y;
	}

	protected void UpdatePreferredValues(TextGenerationSettings tgs)
	{
		pixelPreferedSize = generator.GetPreferredValues(tgs, textInfoCommon);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal TextInfo Update()
	{
		return UpdateWithHash(settings.GetHashCode());
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal TextInfo UpdateWithHash(int hashCode)
	{
		m_ScreenRect = settings.screenRect;
		m_LineHeightDefault = GetLineHeightDefault(settings);
		m_IsPlaceholder = settings.isPlaceholder;
		if (!IsDirty(hashCode))
		{
			return textInfo;
		}
		if (settings.fontAsset == null)
		{
			Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
			return textInfo;
		}
		generator.GenerateText(settings, textInfo);
		m_PreviousGenerationSettingsHash = hashCode;
		isDirty = false;
		m_IsElided = generator.isTextTruncated;
		return textInfo;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal bool PrepareFontAsset()
	{
		if (settings.fontAsset == null)
		{
			return false;
		}
		if (!IsDirty(settings.GetHashCode()))
		{
			return true;
		}
		return generator.PrepareFontAsset(settings);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal void UpdatePreferredSize()
	{
		if (textInfo.characterCount > 0)
		{
			float num = float.MinValue;
			float num2 = textInfo.textElementInfo[textInfo.characterCount - 1].descender;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < textInfo.lineCount; i++)
			{
				LineInfo lineInfo = textInfo.lineInfo[i];
				num = Mathf.Max(num, textInfo.textElementInfo[lineInfo.firstVisibleCharacterIndex].ascender);
				num2 = Mathf.Min(num2, textInfo.textElementInfo[lineInfo.firstVisibleCharacterIndex].descender);
				num3 = (settings.isIMGUI ? Mathf.Max(num3, lineInfo.length) : Mathf.Max(num3, lineInfo.lineExtents.max.x - lineInfo.lineExtents.min.x));
			}
			num4 = num - num2;
			num3 = (float)(int)(num3 * 100f + 1f) / 100f;
			num4 = (float)(int)(num4 * 100f + 1f) / 100f;
			pixelPreferedSize = new Vector2(num3, num4);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static float ConvertPixelUnitsToTextCoreRelativeUnits(float fontSize, FontAsset fontAsset)
	{
		float num = 1f / (float)fontAsset.atlasPadding;
		float num2 = fontAsset.faceInfo.pointSize / fontSize;
		return num * num2;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	internal static float GetLineHeightDefault(TextGenerationSettings settings)
	{
		if (settings != null && settings.fontAsset != null)
		{
			return settings.fontAsset.faceInfo.lineHeight / settings.fontAsset.faceInfo.pointSize * (float)settings.fontSize;
		}
		return 0f;
	}

	public virtual Vector2 GetCursorPositionFromStringIndexUsingCharacterHeight(int index, bool inverseYAxis = true)
	{
		AddToPermanentCacheAndGenerateMesh();
		Vector2 pixel = (useAdvancedText ? TextSelectionService.GetCursorPositionFromLogicalIndex(textGenerationInfo, index) : textInfo.GetCursorPositionFromStringIndexUsingCharacterHeight(index, m_ScreenRect, m_LineHeightDefault, inverseYAxis));
		return PixelsToPoints(pixel);
	}

	public Vector2 GetCursorPositionFromStringIndexUsingLineHeight(int index, bool useXAdvance = false, bool inverseYAxis = true)
	{
		AddToPermanentCacheAndGenerateMesh();
		Vector2 pixel = (useAdvancedText ? TextSelectionService.GetCursorPositionFromLogicalIndex(textGenerationInfo, index) : textInfo.GetCursorPositionFromStringIndexUsingLineHeight(index, m_ScreenRect, m_LineHeightDefault, useXAdvance, inverseYAxis));
		return PixelsToPoints(pixel);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal Rect[] GetHighlightRectangles(int cursorIndex, int selectIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use GetHighlightRectangles while using Standard Text");
			return new Rect[0];
		}
		Rect[] highlightRectangles = TextSelectionService.GetHighlightRectangles(textGenerationInfo, cursorIndex, selectIndex);
		float num = 1f / GetPixelsPerPoint();
		for (int i = 0; i < highlightRectangles.Length; i++)
		{
			highlightRectangles[i].x *= num;
			highlightRectangles[i].y *= num;
			highlightRectangles[i].width *= num;
			highlightRectangles[i].height *= num;
		}
		return highlightRectangles;
	}

	public int GetCursorIndexFromPosition(Vector2 position, bool inverseYAxis = true)
	{
		position = PointsToPixels(position);
		return useAdvancedText ? TextSelectionService.GetCursorLogicalIndexFromPosition(textGenerationInfo, position) : textInfo.GetCursorIndexFromPosition(position, m_ScreenRect, inverseYAxis);
	}

	public int LineDownCharacterPosition(int originalLogicalPos)
	{
		return useAdvancedText ? TextSelectionService.LineDownCharacterPosition(textGenerationInfo, originalLogicalPos) : textInfo.LineDownCharacterPosition(originalLogicalPos);
	}

	public int LineUpCharacterPosition(int originalLogicalPos)
	{
		return useAdvancedText ? TextSelectionService.LineUpCharacterPosition(textGenerationInfo, originalLogicalPos) : textInfo.LineUpCharacterPosition(originalLogicalPos);
	}

	public int FindWordIndex(int cursorIndex)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use FindWordIndex while using Advanced Text");
			return 0;
		}
		return textInfo.FindWordIndex(cursorIndex);
	}

	public int FindNearestLine(Vector2 position)
	{
		position = PointsToPixels(position);
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use FindNearestLine while using Advanced Text");
			return 0;
		}
		return textInfo.FindNearestLine(position);
	}

	public int FindNearestCharacterOnLine(Vector2 position, int line, bool visibleOnly)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use FindNearestCharacterOnLine while using Advanced Text");
			return 0;
		}
		position = PointsToPixels(position);
		return textInfo.FindNearestCharacterOnLine(position, line, visibleOnly);
	}

	public int FindIntersectingLink(Vector3 position, bool inverseYAxis = true)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use FindIntersectingLink while using Advanced Text");
			return 0;
		}
		position = PointsToPixels(position);
		return textInfo.FindIntersectingLink(position, m_ScreenRect, inverseYAxis);
	}

	public int GetCorrespondingStringIndex(int index)
	{
		return useAdvancedText ? index : textInfo.GetCorrespondingStringIndex(index);
	}

	public int GetCorrespondingCodePointIndex(int stringIndex)
	{
		return useAdvancedText ? stringIndex : textInfo.GetCorrespondingCodePointIndex(stringIndex);
	}

	public LineInfo GetLineInfoFromCharacterIndex(int index)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use GetLineInfoFromCharacterIndex while using Advanced Text");
			return default(LineInfo);
		}
		return textInfo.GetLineInfoFromCharacterIndex(index);
	}

	public int GetLineNumber(int index)
	{
		return useAdvancedText ? TextSelectionService.GetLineNumber(textGenerationInfo, index) : textInfo.GetLineNumber(index);
	}

	public float GetLineHeight(int lineNumber)
	{
		return PixelsToPoints(useAdvancedText ? TextSelectionService.GetLineHeight(textGenerationInfo, lineNumber) : textInfo.GetLineHeight(lineNumber));
	}

	public float GetLineHeightFromCharacterIndex(int index)
	{
		return PixelsToPoints(useAdvancedText ? TextSelectionService.GetCharacterHeightFromIndex(textGenerationInfo, index) : textInfo.GetLineHeightFromCharacterIndex(index));
	}

	public float GetCharacterHeightFromIndex(int index)
	{
		return PixelsToPoints(useAdvancedText ? TextSelectionService.GetCharacterHeightFromIndex(textGenerationInfo, index) : textInfo.GetCharacterHeightFromIndex(index));
	}

	public string Substring(int startIndex, int length)
	{
		return useAdvancedText ? TextSelectionService.Substring(textGenerationInfo, startIndex, startIndex + length) : textInfo.Substring(startIndex, length);
	}

	public int PreviousCodePointIndex(int currentIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use PreviousCodePointIndex while using Standard Text");
			return 0;
		}
		return TextSelectionService.PreviousCodePointIndex(textGenerationInfo, currentIndex);
	}

	public int NextCodePointIndex(int currentIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use NextCodePointIndex while using Standard Text");
			return 0;
		}
		return TextSelectionService.NextCodePointIndex(textGenerationInfo, currentIndex);
	}

	public int GetStartOfNextWord(int currentIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use GetStartOfNextWord while using Standard Text");
			return 0;
		}
		return TextSelectionService.GetStartOfNextWord(textGenerationInfo, currentIndex);
	}

	public int GetEndOfPreviousWord(int currentIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use GetEndOfPreviousWord while using Standard Text");
			return 0;
		}
		return TextSelectionService.GetEndOfPreviousWord(textGenerationInfo, currentIndex);
	}

	public int GetFirstCharacterIndexOnLine(int currentIndex)
	{
		if (!useAdvancedText)
		{
			return GetLineInfoFromCharacterIndex(currentIndex).firstCharacterIndex;
		}
		return TextSelectionService.GetFirstCharacterIndexOnLine(textGenerationInfo, currentIndex);
	}

	public int GetLastCharacterIndexOnLine(int currentIndex)
	{
		if (!useAdvancedText)
		{
			return GetLineInfoFromCharacterIndex(currentIndex).lastCharacterIndex;
		}
		return TextSelectionService.GetLastCharacterIndexOnLine(textGenerationInfo, currentIndex);
	}

	public int IndexOf(char value, int startIndex)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use IndexOf while using Advanced Text");
			return 0;
		}
		return textInfo.IndexOf(value, startIndex);
	}

	public int LastIndexOf(char value, int startIndex)
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use LastIndexOf while using Advanced Text");
			return 0;
		}
		return textInfo.LastIndexOf(value, startIndex);
	}

	public void SelectCurrentWord(int index, ref int cursorIndex, ref int selectIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectCurrentWord while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectCurrentWord(textGenerationInfo, index, ref cursorIndex, ref selectIndex);
		}
	}

	public void SelectCurrentParagraph(ref int cursorIndex, ref int selectIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectCurrentParagraph while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectCurrentParagraph(textGenerationInfo, ref cursorIndex, ref selectIndex);
		}
	}

	public void SelectToPreviousParagraph(ref int cursorIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectToPreviousParagraph while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectToPreviousParagraph(textGenerationInfo, ref cursorIndex);
		}
	}

	public void SelectToNextParagraph(ref int cursorIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectToNextParagraph while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectToNextParagraph(textGenerationInfo, ref cursorIndex);
		}
	}

	public void SelectToStartOfParagraph(ref int cursorIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectToStartOfParagraph while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectToStartOfParagraph(textGenerationInfo, ref cursorIndex);
		}
	}

	public void SelectToEndOfParagraph(ref int cursorIndex)
	{
		if (!useAdvancedText)
		{
			Debug.LogError("Cannot use SelectToEndOfParagraph while using Standard Text");
		}
		else
		{
			TextSelectionService.SelectToEndOfParagraph(textGenerationInfo, ref cursorIndex);
		}
	}

	internal virtual bool IsAdvancedTextEnabledForElement()
	{
		return false;
	}

	internal int GetTextElementCount()
	{
		if (useAdvancedText)
		{
			Debug.LogError("Cannot use GetTextElementCount while using Advanced Text");
			return 0;
		}
		return textInfo.textElementInfo.Length;
	}

	internal GlyphMetricsForOverlay GetScaledCharacterMetrics(int i)
	{
		if (useAdvancedText)
		{
			throw new InvalidOperationException("Cannot use GetScaledCharacterMetrics while using Advanced Text");
		}
		return new GlyphMetricsForOverlay(ref textInfo.textElementInfo[i], GetPixelsPerPoint());
	}
}
