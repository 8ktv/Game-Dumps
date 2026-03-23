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

	private static TextLib s_TextLib;

	internal TextEventHandler m_TextEventHandler;

	protected TextElement m_TextElement;

	internal static readonly float k_MinPadding = 6f;

	private bool wasAdvancedTextEnabledForElement;

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

	internal Vector2 ATGMeasuredSizes { get; set; }

	internal Vector2 ATGRoundedSizes { get; set; }

	public override bool IsPlaceholder => base.useAdvancedText ? m_TextElement.showPlaceholderText : base.IsPlaceholder;

	private void ComputeNativeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
	{
		if (ConvertUssToNativeTextGenerationSettings(fontsize))
		{
			nativeSettings.text = ((textToMeasure.valueLength > 0) ? textToMeasure.CreateString() : "\u200b");
			nativeSettings.screenWidth = (float.IsNaN(width) ? (-1) : ((int)(width * 64f)));
			nativeSettings.screenHeight = (float.IsNaN(height) ? (-1) : ((int)(height * 64f)));
			if (m_TextElement.enableRichText && !string.IsNullOrEmpty(nativeSettings.text))
			{
				RichTextTagParser.CreateTextGenerationSettingsArray(ref nativeSettings, Links, atgHyperlinkColor);
			}
			else
			{
				nativeSettings.textSpans = null;
			}
			pixelPreferedSize = textLib.MeasureText(nativeSettings, IntPtr.Zero);
		}
	}

	public (NativeTextInfo, bool) UpdateNative(bool generateNativeSettings = true)
	{
		if (generateNativeSettings && !ConvertUssToNativeTextGenerationSettings())
		{
			return (default(NativeTextInfo), false);
		}
		if (m_TextElement.enableRichText && !string.IsNullOrEmpty(nativeSettings.text))
		{
			RichTextTagParser.CreateTextGenerationSettingsArray(ref nativeSettings, Links, atgHyperlinkColor);
		}
		else
		{
			nativeSettings.textSpans = null;
		}
		if (nativeSettings.hasLink && textGenerationInfo == IntPtr.Zero)
		{
			textGenerationInfo = TextGenerationInfo.Create();
			if (m_ATGTextEventHandler == null)
			{
				m_ATGTextEventHandler = new ATGTextEventHandler(m_TextElement);
			}
		}
		NativeTextInfo item = textLib.GenerateText(nativeSettings, textGenerationInfo);
		m_IsElided = item.isElided;
		return (item, true);
	}

	public void CacheTextGenerationInfo()
	{
		if (!base.useAdvancedText)
		{
			Debug.LogError("CacheTextGenerationInfo should only be called for ATG.");
		}
		else if (textGenerationInfo == IntPtr.Zero)
		{
			textGenerationInfo = TextGenerationInfo.Create();
		}
	}

	public void ProcessMeshInfos(NativeTextInfo textInfo)
	{
		textLib.ProcessMeshInfos(textInfo, nativeSettings);
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
		if (textGenerationInfo == IntPtr.Zero)
		{
			Debug.LogError("TextGenerationInfo pointer is null.");
			return (RichTextTagParser.TagType.Unknown, null);
		}
		int num = TextLib.FindIntersectingLink(point * GetPixelsPerPoint(), textGenerationInfo);
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

	internal bool ConvertUssToNativeTextGenerationSettings(float? fontsize = null)
	{
		FontAsset fontAsset = TextUtilities.GetFontAsset(m_TextElement);
		if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Static)
		{
			Debug.LogError("Advanced text system cannot render using static font asset " + fontAsset.faceInfo.familyName);
			return false;
		}
		float pixelsPerPoint = GetPixelsPerPoint();
		ComputedStyle computedStyle = m_TextElement.computedStyle;
		nativeSettings.textSettings = TextUtilities.GetTextSettingsFrom(m_TextElement).nativeTextSettings;
		RenderedText renderedText = ((m_TextElement.isElided && !TextLibraryCanElide()) ? new RenderedText(m_TextElement.elidedText) : m_TextElement.renderedText);
		nativeSettings.text = renderedText.CreateString();
		float num = fontsize ?? computedStyle.fontSize.value;
		nativeSettings.fontSize = (int)(num * 64f * pixelsPerPoint);
		nativeSettings.bestFit = computedStyle.unityTextAutoSize.mode == TextAutoSizeMode.BestFit;
		nativeSettings.maxFontSize = (int)(computedStyle.unityTextAutoSize.maxSize.value * 64f * pixelsPerPoint);
		nativeSettings.minFontSize = (int)(computedStyle.unityTextAutoSize.minSize.value * 64f * pixelsPerPoint);
		nativeSettings.wordWrap = computedStyle.whiteSpace.toTextCore(m_TextElement.isInputField);
		nativeSettings.overflow = computedStyle.textOverflow.toTextCore(computedStyle.overflow);
		nativeSettings.horizontalAlignment = TextGeneratorUtilities.GetHorizontalAlignment(computedStyle.unityTextAlign);
		nativeSettings.verticalAlignment = TextGeneratorUtilities.GetVerticalAlignment(computedStyle.unityTextAlign);
		nativeSettings.characterSpacing = (int)(computedStyle.letterSpacing.value * 64f);
		nativeSettings.wordSpacing = (int)(computedStyle.wordSpacing.value * 64f);
		nativeSettings.paragraphSpacing = (int)(computedStyle.unityParagraphSpacing.value * 64f);
		nativeSettings.color = computedStyle.color;
		nativeSettings.fontAsset = fontAsset.nativeFontAsset;
		nativeSettings.languageDirection = m_TextElement.localLanguageDirection.toTextCore();
		nativeSettings.vertexPadding = (int)(GetVertexPadding(fontAsset) * 64f);
		FontStyles fontStyles = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
		nativeSettings.fontStyle = fontStyles & ~FontStyles.Bold;
		nativeSettings.fontWeight = (((fontStyles & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : TextFontWeight.Regular);
		Vector2 vector = m_TextElement.contentRect.size;
		if (Mathf.Abs(vector.x - ATGRoundedSizes.x) < 0.01f && Mathf.Abs(vector.y - ATGRoundedSizes.y) < 0.01f)
		{
			vector = ATGMeasuredSizes;
		}
		else
		{
			ATGRoundedSizes = vector;
			ATGMeasuredSizes = vector;
		}
		nativeSettings.screenWidth = (int)(vector.x * 64f * pixelsPerPoint);
		nativeSettings.screenHeight = (int)(vector.y * 64f * pixelsPerPoint);
		return true;
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
		base.SetDirty();
	}

	public Vector2 ComputeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
	{
		float pixelsPerPoint = GetPixelsPerPoint();
		width = Mathf.Floor(width * pixelsPerPoint);
		height = Mathf.Floor(height * pixelsPerPoint);
		if (TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement))
		{
			ComputeNativeTextSize(in textToMeasure, width, height, fontsize);
		}
		else
		{
			ConvertUssToTextGenerationSettings(populateScreenRect: false, fontsize);
			TextHandle.settings.renderedText = textToMeasure;
			TextHandle.settings.screenRect = new Rect(0f, 0f, width, height);
			UpdatePreferredValues(TextHandle.settings);
		}
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
		RemoveTextInfoFromTemporaryCache();
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
		bool flag = TextUtilities.IsAdvancedTextEnabledForElement(m_TextElement);
		if (wasAdvancedTextEnabledForElement && !flag && textGenerationInfo != IntPtr.Zero)
		{
			TextGenerationInfo.Destroy(textGenerationInfo);
			textGenerationInfo = IntPtr.Zero;
			m_ATGTextEventHandler?.OnDestroy();
			m_ATGTextEventHandler = null;
			m_TextEventHandler = new TextEventHandler(m_TextElement);
		}
		else if (!wasAdvancedTextEnabledForElement && flag)
		{
			TextHandle.s_PermanentCache.RemoveTextInfoFromCache(this);
			TextHandle.s_TemporaryCache.RemoveTextInfoFromCache(this);
			m_TextEventHandler?.OnDestroy();
			m_TextEventHandler = null;
			m_ATGTextEventHandler = new ATGTextEventHandler(m_TextElement);
		}
		wasAdvancedTextEnabledForElement = flag;
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
