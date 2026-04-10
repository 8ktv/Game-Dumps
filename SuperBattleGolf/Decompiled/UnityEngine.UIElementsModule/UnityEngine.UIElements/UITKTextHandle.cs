#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements;

internal class UITKTextHandle : TextHandle
{
	internal ATGTextEventHandler m_ATGTextEventHandler;

	private List<(int, RichTextTagParser.TagType, string)> m_Links;

	internal Color atgHyperlinkColor = Color.blue;

	private bool uvsAreGenerated = false;

	private static TextLib s_TextLib;

	internal TextEventHandler m_TextEventHandler;

	protected TextElement m_TextElement;

	internal static readonly float k_MinPadding = 6f;

	private List<(int, RichTextTagParser.TagType, string)> Links => m_Links ?? (m_Links = new List<(int, RichTextTagParser.TagType, string)>());

	protected internal TextLib textLib
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			InitTextLib();
			return s_TextLib;
		}
	}

	internal float LastPixelPerPoint { get; set; }

	internal float? MeasuredWidth { get; set; }

	internal float RoundedWidth { get; set; }

	internal float? ATGMeasuredWidth { get; set; }

	internal float ATGRoundedWidth { get; set; }

	public override bool IsPlaceholder => base.useAdvancedText ? m_TextElement.showPlaceholderText : base.IsPlaceholder;

	private void ComputeNativeTextSize(in string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
	{
		if (ConvertUssToNativeTextGenerationSettings(textToMeasure, fontsize))
		{
			if (string.IsNullOrEmpty(nativeSettings.text) && m_TextElement.isInputField)
			{
				nativeSettings.text = "\u200b";
			}
			if (widthMode == VisualElement.MeasureMode.Undefined || float.IsNaN(width) || float.IsNegative(width))
			{
				nativeSettings.screenWidth = -1;
			}
			else
			{
				nativeSettings.screenWidth = (int)(width * 64f);
			}
			if (heightMode == VisualElement.MeasureMode.Undefined || float.IsNaN(height) || float.IsNegative(height))
			{
				nativeSettings.screenHeight = -1;
			}
			else
			{
				nativeSettings.screenHeight = (int)(height * 64f);
			}
			if (base.textGenerationInfo == IntPtr.Zero)
			{
				base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
			}
			pixelPreferedSize = textLib.MeasureText(nativeSettings, base.textGenerationInfo);
		}
	}

	public (NativeTextInfo, bool) UpdateNative(bool generateNativeSettings = true)
	{
		if (generateNativeSettings && !ConvertUssToNativeTextGenerationSettings())
		{
			return (default(NativeTextInfo), false);
		}
		if (nativeSettings.hasLink)
		{
			m_TextElement.uitkTextHandle.CacheTextGenerationInfo();
			if (m_ATGTextEventHandler == null)
			{
				m_ATGTextEventHandler = new ATGTextEventHandler(m_TextElement);
			}
		}
		if (base.textGenerationInfo == IntPtr.Zero)
		{
			base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
		}
		bool wasCached = false;
		NativeTextInfo item = textLib.GenerateText(nativeSettings, base.textGenerationInfo, ref wasCached);
		if (!wasCached)
		{
			uvsAreGenerated = false;
		}
		m_IsElided = item.isElided;
		return (item, true);
	}

	public void CacheTextGenerationInfo()
	{
		if (!base.useAdvancedText)
		{
			Debug.LogError("CacheTextGenerationInfo should only be called for ATG.");
		}
		else if (!m_TextHandleFlags.HasFlag(TextHandleFlags.IsCachedPermanentATG))
		{
			if (base.textGenerationInfo != IntPtr.Zero)
			{
				TextGenerationInfo.Destroy(base.textGenerationInfo);
				base.textGenerationInfo = IntPtr.Zero;
			}
			base.IsCachedPermanentATG = true;
			base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
		}
	}

	public void ShapeText()
	{
		if (ConvertUssToNativeTextGenerationSettings())
		{
			if (base.textGenerationInfo == IntPtr.Zero)
			{
				base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
			}
			textLib.ShapeText(nativeSettings, base.textGenerationInfo);
		}
	}

	public void ProcessMeshInfos(NativeTextInfo textInfo, ref List<List<List<int>>> textElementIndicesByMesh, ref List<bool> hasMultipleColorsByMesh)
	{
		textLib.ProcessMeshInfos(textInfo, nativeSettings, ref textElementIndicesByMesh, ref hasMultipleColorsByMesh, uvsAreGenerated);
		uvsAreGenerated = true;
	}

	public bool HasMissingGlyphs(NativeTextInfo textInfo, ref Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset)
	{
		return textLib.HasMissingGlyphs(textInfo, ref missingGlyphsPerFontAsset);
	}

	private (bool, bool) hasLinkAndHyperlink()
	{
		bool flag = false;
		bool flag2 = false;
		if (m_Links != null)
		{
			foreach (var link in Links)
			{
				RichTextTagParser.TagType item = link.Item2;
				flag = flag || item == RichTextTagParser.TagType.Link;
				flag2 = flag2 || item == RichTextTagParser.TagType.Hyperlink;
				if (flag && flag2)
				{
					break;
				}
			}
		}
		return (flag, flag2);
	}

	internal (RichTextTagParser.TagType, string) ATGFindIntersectingLink(Vector2 point)
	{
		Debug.Assert(base.useAdvancedText);
		if (base.textGenerationInfo == IntPtr.Zero)
		{
			Debug.LogError("TextGenerationInfo pointer is null.");
			return (RichTextTagParser.TagType.Unknown, null);
		}
		int num = TextLib.FindIntersectingLink(point * GetPixelsPerPoint(), base.textGenerationInfo);
		if (num == -1)
		{
			return (RichTextTagParser.TagType.Unknown, null);
		}
		return (m_Links[num].Item2, m_Links[num].Item3);
	}

	internal void UpdateATGTextEventHandler()
	{
		if (m_ATGTextEventHandler != null)
		{
			var (flag, flag2) = hasLinkAndHyperlink();
			if (flag)
			{
				m_ATGTextEventHandler.RegisterLinkTagCallbacks();
			}
			else
			{
				m_ATGTextEventHandler.UnRegisterLinkTagCallbacks();
			}
			if (flag2)
			{
				m_ATGTextEventHandler.RegisterHyperlinkCallbacks();
			}
			else
			{
				m_ATGTextEventHandler.UnRegisterHyperlinkCallbacks();
			}
		}
	}

	internal void EnsureIsReadyForJobs()
	{
		InitTextLib();
		FontAsset fontAsset = TextUtilities.GetFontAsset(m_TextElement);
		if (!(fontAsset == null))
		{
			TextUtilities.GetTextSettingsFrom(m_TextElement).UpdateNativeTextSettings();
			fontAsset.EnsureNativeFontAssetIsCreated();
		}
	}

	internal bool ConvertUssToNativeTextGenerationSettings(string? textToMeasure = null, float? fontsize = null)
	{
		float pixelsPerPoint = GetPixelsPerPoint();
		ComputedStyle computedStyle = m_TextElement.computedStyle;
		nativeSettings.preProcessFlags = PreProcessFlags.None;
		nativeSettings.text = ((m_TextElement.isElided && !TextLibraryCanElide()) ? m_TextElement.elidedText : m_TextElement.renderedTextString);
		if (textToMeasure != null)
		{
			nativeSettings.text = textToMeasure;
		}
		if (nativeSettings.text == null)
		{
			nativeSettings.text = "";
		}
		float num = (fontsize ?? computedStyle.fontSize.value) * pixelsPerPoint;
		nativeSettings.fontSize = (int)Math.Round(num * 64f, MidpointRounding.AwayFromZero);
		nativeSettings.bestFit = computedStyle.unityTextAutoSize.mode == TextAutoSizeMode.BestFit;
		nativeSettings.maxFontSize = (int)(computedStyle.unityTextAutoSize.maxSize.value * 64f * pixelsPerPoint);
		nativeSettings.minFontSize = (int)(computedStyle.unityTextAutoSize.minSize.value * 64f * pixelsPerPoint);
		nativeSettings.wordWrapEnabled = computedStyle.whiteSpace == WhiteSpace.Normal || computedStyle.whiteSpace == WhiteSpace.PreWrap;
		if (!m_TextElement.isInputField && (computedStyle.whiteSpace == WhiteSpace.NoWrap || computedStyle.whiteSpace == WhiteSpace.Normal))
		{
			nativeSettings.preProcessFlags |= PreProcessFlags.CollapseWhiteSpaces;
		}
		if (m_TextElement.parseEscapeSequences)
		{
			nativeSettings.preProcessFlags |= PreProcessFlags.ParseEscapeSequences;
		}
		nativeSettings.overflow = computedStyle.textOverflow.toTextCore(computedStyle.overflow, computedStyle.unityTextOverflowPosition);
		nativeSettings.horizontalAlignment = TextGeneratorUtilities.GetHorizontalAlignment(computedStyle.unityTextAlign);
		nativeSettings.verticalAlignment = TextGeneratorUtilities.GetVerticalAlignment(computedStyle.unityTextAlign);
		nativeSettings.characterSpacing = (int)(computedStyle.letterSpacing.value * 64f);
		nativeSettings.wordSpacing = (int)(computedStyle.wordSpacing.value * 64f);
		nativeSettings.paragraphSpacing = (int)(computedStyle.unityParagraphSpacing.value * 64f);
		nativeSettings.color = computedStyle.color;
		ref Color32 color = ref nativeSettings.color;
		color *= m_TextElement.playModeTintColor;
		nativeSettings.languageDirection = m_TextElement.localLanguageDirection.toTextCore();
		FontStyles fontStyles = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
		nativeSettings.fontStyle = fontStyles & ~FontStyles.Bold;
		nativeSettings.fontWeight = (((fontStyles & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : TextFontWeight.Regular);
		Vector2 size = m_TextElement.contentRect.size;
		if (ATGMeasuredWidth.HasValue && Mathf.Abs(size.x - ATGRoundedWidth) < 0.01f && LastPixelPerPoint == pixelsPerPoint)
		{
			size.x = ATGMeasuredWidth.Value;
		}
		else
		{
			ATGRoundedWidth = size.x;
			ATGMeasuredWidth = null;
		}
		nativeSettings.screenWidth = Mathf.RoundToInt(size.x * 64f * pixelsPerPoint);
		nativeSettings.screenHeight = Mathf.RoundToInt(size.y * 64f * pixelsPerPoint);
		FontAsset fontAsset = TextUtilities.GetFontAsset(m_TextElement);
		if (fontAsset == null)
		{
			return false;
		}
		if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Static)
		{
			Debug.LogError("Advanced text system cannot render using static font asset " + fontAsset.faceInfo.familyName);
			return false;
		}
		nativeSettings.vertexPadding = (int)(GetVertexPadding(fontAsset) * 64f);
		nativeSettings.fontAsset = fontAsset.nativeFontAsset;
		if (fontAsset.nativeFontAsset == IntPtr.Zero)
		{
			return false;
		}
		nativeSettings.textSettings = TextUtilities.GetTextSettingsFrom(m_TextElement).nativeTextSettings;
		if (m_TextElement.enableRichText && RichTextTagParser.MayNeedParsing(nativeSettings.text))
		{
			TextPreprocessor.PreProcessString(ref nativeSettings.text, nativeSettings.preProcessFlags, TextUtilities.GetTextSettingsFrom(m_TextElement));
			nativeSettings.preProcessFlags = PreProcessFlags.None;
			RichTextTagParser.CreateTextGenerationSettingsArray(ref nativeSettings, Links, atgHyperlinkColor, GetPixelsPerPoint(), TextUtilities.GetTextSettingsFrom(m_TextElement));
		}
		else
		{
			nativeSettings.textSpans = null;
		}
		return true;
	}

	internal void EnsureFontAssetsAreCreatedOnTheMainThread()
	{
		FontAsset fontAsset = TextUtilities.GetFontAsset(m_TextElement);
		fontAsset.EnsureNativeFontAssetIsCreated();
	}

	private TextAsset GetICUAsset()
	{
		if (m_TextElement.panel == null)
		{
			throw new InvalidOperationException("Text cannot be processed on elements not in a panel");
		}
		TextAsset iCUDataAsset = ((PanelSettings)((RuntimePanel)m_TextElement.panel).ownerObject).m_ICUDataAsset;
		if (iCUDataAsset != null)
		{
			return iCUDataAsset;
		}
		iCUDataAsset = GetICUAssetStaticFalback();
		if (iCUDataAsset != null)
		{
			return iCUDataAsset;
		}
		Debug.LogError("ICU Data not available. The data should be automatically assigned to the PanelSettings in the editor if the advanced text option is enable in the project settings. It will not be present on PanelSettings created at runtime, so make sure the build contains at least one PanelSettings asset");
		return null;
	}

	internal static TextAsset GetICUAssetStaticFalback()
	{
		TextAsset[] array = Resources.FindObjectsOfTypeAll<TextAsset>();
		foreach (TextAsset textAsset in array)
		{
			if (textAsset.name == "icudt73l")
			{
				return textAsset;
			}
		}
		return null;
	}

	protected internal void InitTextLib()
	{
		if (s_TextLib == null)
		{
			s_TextLib = new TextLib(GetICUAsset().bytes);
		}
	}

	public override void RemoveFromPermanentCacheATG()
	{
		if (base.IsCachedPermanentATG)
		{
			m_ATGTextEventHandler?.UnRegisterHyperlinkCallbacks();
		}
		base.RemoveFromPermanentCacheATG();
	}

	public UITKTextHandle(TextElement te)
	{
		m_TextElement = te;
		m_TextEventHandler = new TextEventHandler(te);
	}

	protected override float GetPixelsPerPoint()
	{
		return m_TextElement?.scaledPixelsPerPoint ?? 1f;
	}

	public override void SetDirty()
	{
		MeasuredWidth = null;
		ATGMeasuredWidth = null;
		base.SetDirty();
	}

	public Vector2 ComputeTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
	{
		if (!TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement))
		{
			return ComputeTextSize(new RenderedText(textToMeasure), width, height, fontsize);
		}
		float pixelsPerPoint = GetPixelsPerPoint();
		width = Mathf.Floor(width * pixelsPerPoint);
		height = Mathf.Floor(height * pixelsPerPoint);
		ComputeNativeTextSize(in textToMeasure, width, widthMode, height, heightMode, fontsize);
		return base.preferredSize;
	}

	public Vector2 ComputeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
	{
		if (TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement))
		{
			return Vector2.zero;
		}
		float pixelsPerPoint = GetPixelsPerPoint();
		width = Mathf.Floor(width * pixelsPerPoint);
		height = Mathf.Floor(height * pixelsPerPoint);
		ConvertUssToTextGenerationSettings(populateScreenRect: false, fontsize);
		TextHandle.settings.renderedText = textToMeasure;
		TextHandle.settings.screenRect = new Rect(0f, 0f, width, height);
		UpdatePreferredValues(TextHandle.settings);
		return base.preferredSize;
	}

	public void ComputeSettingsAndUpdate()
	{
		if (base.useAdvancedText)
		{
			UpdateNative();
			UpdateATGTextEventHandler();
			return;
		}
		UpdateMesh();
		HandleATag();
		HandleLinkTag();
		HandleLinkAndATagCallbacks();
	}

	public void HandleATag()
	{
		m_TextEventHandler?.HandleATag();
	}

	public void HandleLinkTag()
	{
		m_TextEventHandler?.HandleLinkTag();
	}

	public void HandleLinkAndATagCallbacks()
	{
		m_TextEventHandler?.HandleLinkAndATagCallbacks();
	}

	public void UpdateMesh()
	{
		ConvertUssToTextGenerationSettings(populateScreenRect: true);
		int hashCode = TextHandle.settings.GetHashCode();
		if (m_PreviousGenerationSettingsHash == hashCode && !isDirty)
		{
			AddTextInfoToTemporaryCache(hashCode);
			return;
		}
		RemoveFromTemporaryCache();
		UpdateWithHash(hashCode);
	}

	public override void AddToPermanentCacheAndGenerateMesh()
	{
		if (base.useAdvancedText)
		{
			CacheTextGenerationInfo();
			UpdateNative();
			UpdateATGTextEventHandler();
		}
		else if (ConvertUssToTextGenerationSettings(populateScreenRect: true))
		{
			base.AddToPermanentCacheAndGenerateMesh();
		}
		ReleaseResourcesIfPossible();
	}

	private TextOverflowMode GetTextOverflowMode()
	{
		ComputedStyle computedStyle = m_TextElement.computedStyle;
		if (computedStyle.textOverflow == TextOverflow.Clip)
		{
			return TextOverflowMode.Masking;
		}
		if (computedStyle.textOverflow != TextOverflow.Ellipsis)
		{
			return TextOverflowMode.Overflow;
		}
		if (!TextLibraryCanElide())
		{
			return TextOverflowMode.Masking;
		}
		if (computedStyle.overflow == OverflowInternal.Hidden)
		{
			return TextOverflowMode.Ellipsis;
		}
		return TextOverflowMode.Overflow;
	}

	internal virtual bool ConvertUssToTextGenerationSettings(bool populateScreenRect, float? fontsize = null)
	{
		ComputedStyle computedStyle = m_TextElement.computedStyle;
		UnityEngine.TextCore.Text.TextGenerationSettings textGenerationSettings = TextHandle.settings;
		if (computedStyle.unityTextAutoSize != TextAutoSize.None())
		{
			Debug.LogWarning("TextAutoSize is not supported with the Standard TextGenerator. Please use Advanced Text Generation instead.");
		}
		textGenerationSettings.text = string.Empty;
		textGenerationSettings.isIMGUI = false;
		textGenerationSettings.textSettings = TextUtilities.GetTextSettingsFrom(m_TextElement);
		if (textGenerationSettings.textSettings == null)
		{
			return false;
		}
		textGenerationSettings.fontAsset = TextUtilities.GetFontAsset(m_TextElement);
		if (textGenerationSettings.fontAsset == null)
		{
			return false;
		}
		textGenerationSettings.extraPadding = GetVertexPadding(textGenerationSettings.fontAsset);
		textGenerationSettings.renderedText = ((m_TextElement.isElided && !TextLibraryCanElide()) ? new RenderedText(m_TextElement.elidedText) : m_TextElement.renderedText);
		textGenerationSettings.isPlaceholder = m_TextElement.showPlaceholderText;
		float pixelsPerPoint = GetPixelsPerPoint();
		float num = fontsize ?? computedStyle.fontSize.value;
		textGenerationSettings.fontSize = (int)Math.Round(num * pixelsPerPoint, MidpointRounding.AwayFromZero);
		textGenerationSettings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
		textGenerationSettings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(computedStyle.unityTextAlign);
		textGenerationSettings.textWrappingMode = computedStyle.whiteSpace.toTextWrappingMode(m_TextElement.isInputField && !m_TextElement.edition.multiline);
		textGenerationSettings.richText = m_TextElement.enableRichText;
		textGenerationSettings.overflowMode = GetTextOverflowMode();
		textGenerationSettings.characterSpacing = computedStyle.letterSpacing.value;
		textGenerationSettings.wordSpacing = computedStyle.wordSpacing.value;
		textGenerationSettings.paragraphSpacing = computedStyle.unityParagraphSpacing.value;
		textGenerationSettings.color = computedStyle.color;
		textGenerationSettings.color *= m_TextElement.playModeTintColor;
		textGenerationSettings.shouldConvertToLinearSpace = false;
		textGenerationSettings.parseControlCharacters = m_TextElement.parseEscapeSequences;
		textGenerationSettings.isRightToLeft = m_TextElement.localLanguageDirection == LanguageDirection.RTL;
		textGenerationSettings.emojiFallbackSupport = m_TextElement.emojiFallbackSupport;
		TextHandle.settings.pixelsPerPoint = pixelsPerPoint;
		if (populateScreenRect)
		{
			Vector2 size = m_TextElement.contentRect.size;
			if (MeasuredWidth.HasValue && Mathf.Abs(size.x - RoundedWidth) < 0.01f && LastPixelPerPoint == pixelsPerPoint)
			{
				size.x = MeasuredWidth.Value;
			}
			else
			{
				RoundedWidth = size.x;
				MeasuredWidth = null;
				LastPixelPerPoint = pixelsPerPoint;
			}
			size.x *= pixelsPerPoint;
			size.y *= pixelsPerPoint;
			if (textGenerationSettings.fontAsset.IsBitmap())
			{
				size.x = Mathf.Round(size.x);
				size.y = Mathf.Round(size.y);
			}
			textGenerationSettings.screenRect = new Rect(Vector2.zero, size);
		}
		return true;
	}

	internal bool TextLibraryCanElide()
	{
		return m_TextElement.computedStyle.unityTextOverflowPosition == TextOverflowPosition.End;
	}

	internal float GetVertexPadding(FontAsset fontAsset)
	{
		ComputedStyle computedStyle = m_TextElement.computedStyle;
		float num = computedStyle.unityTextOutlineWidth / 2f;
		float num2 = Mathf.Abs(computedStyle.textShadow.offset.x);
		float num3 = Mathf.Abs(computedStyle.textShadow.offset.y);
		float num4 = Mathf.Abs(computedStyle.textShadow.blurRadius);
		if (num <= 0f && num2 <= 0f && num3 <= 0f && num4 <= 0f)
		{
			return k_MinPadding;
		}
		float a = Mathf.Max(num2 + num4, num);
		float b = Mathf.Max(num3 + num4, num);
		float num5 = Mathf.Max(a, b) + k_MinPadding;
		float num6 = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
		int num7 = fontAsset.atlasPadding + 1;
		return Mathf.Min(num5 * num6 * (float)num7, num7);
	}

	internal override bool IsAdvancedTextEnabledForElement()
	{
		return TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement);
	}

	internal void ReleaseResourcesIfPossible()
	{
		if (!TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement))
		{
			RemoveFromPermanentCacheATG();
			if (m_ATGTextEventHandler != null)
			{
				m_ATGTextEventHandler?.OnDestroy();
				m_ATGTextEventHandler = null;
			}
			if (m_TextEventHandler == null)
			{
				m_TextEventHandler = new TextEventHandler(m_TextElement);
			}
			return;
		}
		if (base.IsCachedPermanentTextCore)
		{
			RemoveFromPermanentCacheTextCore();
		}
		if (base.IsCachedTemporary)
		{
			RemoveFromTemporaryCache();
		}
		if (m_TextEventHandler != null)
		{
			m_TextEventHandler?.OnDestroy();
			m_TextEventHandler = null;
		}
	}

	public bool IsElided()
	{
		if (string.IsNullOrEmpty(m_TextElement.text))
		{
			return true;
		}
		return m_IsElided;
	}
}
