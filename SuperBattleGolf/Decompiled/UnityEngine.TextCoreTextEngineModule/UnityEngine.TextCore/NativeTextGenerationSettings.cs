using System;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore;

[NativeHeader("Modules/TextCoreTextEngine/Native/TextGenerationSettings.h")]
[UsedByNativeCode("TextGenerationSettings")]
[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal struct NativeTextGenerationSettings
{
	public IntPtr fontAsset;

	public IntPtr textSettings;

	public string text;

	public int screenWidth;

	public int screenHeight;

	public bool wordWrapEnabled;

	public TextOverflow overflow;

	public LanguageDirection languageDirection;

	public int vertexPadding;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal HorizontalAlignment horizontalAlignment;

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal VerticalAlignment verticalAlignment;

	public int fontSize;

	public bool bestFit;

	public int maxFontSize;

	public int minFontSize;

	public FontStyles fontStyle;

	public TextFontWeight fontWeight;

	public TextSpan[] textSpans;

	public Color32 color;

	public int characterSpacing;

	public int wordSpacing;

	public int paragraphSpacing;

	public PreProcessFlags preProcessFlags;

	public bool hasLink => textSpans != null && Array.Exists(textSpans, (TextSpan span) => span.linkID != -1);

	public static NativeTextGenerationSettings Default => new NativeTextGenerationSettings
	{
		fontStyle = FontStyles.Normal,
		fontWeight = TextFontWeight.Regular,
		color = Color.black
	};

	public readonly TextSpan CreateTextSpan()
	{
		return new TextSpan
		{
			fontAsset = fontAsset,
			fontSize = fontSize,
			color = color,
			fontStyle = fontStyle,
			fontWeight = fontWeight,
			alignment = horizontalAlignment,
			highlightColor = RichTextTagParser.k_HighlightColor,
			highlightPadding = Vector4.zero,
			mspace = 0,
			mspaceUnitType = RichTextTagParser.TagUnitType.Pixels,
			cspace = 0,
			cspaceUnitType = RichTextTagParser.TagUnitType.Pixels,
			spriteColor = color,
			spriteID = EntityId.None,
			spriteScale = 0,
			spriteTint = false,
			margin = 0,
			marginDirection = MarginDirection.Both,
			marginUnitType = RichTextTagParser.TagUnitType.Pixels,
			lineHeight = 0,
			lineHeightUnitType = RichTextTagParser.TagUnitType.Pixels,
			indent = 0,
			indentUnitType = RichTextTagParser.TagUnitType.Pixels,
			vOffset = 0,
			vOffsetUnitType = RichTextTagParser.TagUnitType.Pixels,
			subscriptNestingLevel = 0,
			superscriptNestingLevel = 0,
			linkID = -1
		};
	}

	public string GetTextSpanContent(int spanIndex)
	{
		if (string.IsNullOrEmpty(text))
		{
			throw new InvalidOperationException("The text property is null or empty.");
		}
		if (textSpans == null || spanIndex < 0 || spanIndex >= textSpans.Length)
		{
			throw new ArgumentOutOfRangeException("spanIndex", "Invalid span index.");
		}
		TextSpan textSpan = textSpans[spanIndex];
		if (textSpan.startIndex < 0 || textSpan.startIndex >= text.Length || textSpan.startIndex + textSpan.length > text.Length)
		{
			throw new ArgumentOutOfRangeException("spanIndex", "Invalid startIndex or length for the current text.");
		}
		return text.Substring(textSpan.startIndex, textSpan.length);
	}

	internal NativeTextGenerationSettings(NativeTextGenerationSettings tgs)
	{
		text = tgs.text;
		fontSize = tgs.fontSize;
		bestFit = tgs.bestFit;
		maxFontSize = tgs.maxFontSize;
		minFontSize = tgs.minFontSize;
		screenWidth = tgs.screenWidth;
		screenHeight = tgs.screenHeight;
		wordWrapEnabled = tgs.wordWrapEnabled;
		horizontalAlignment = tgs.horizontalAlignment;
		verticalAlignment = tgs.verticalAlignment;
		color = tgs.color;
		fontAsset = tgs.fontAsset;
		textSettings = tgs.textSettings;
		fontStyle = tgs.fontStyle;
		fontWeight = tgs.fontWeight;
		languageDirection = tgs.languageDirection;
		vertexPadding = tgs.vertexPadding;
		overflow = tgs.overflow;
		textSpans = ((tgs.textSpans != null) ? ((TextSpan[])tgs.textSpans.Clone()) : null);
		characterSpacing = tgs.characterSpacing;
		wordSpacing = tgs.wordSpacing;
		paragraphSpacing = tgs.paragraphSpacing;
		preProcessFlags = tgs.preProcessFlags;
	}

	public override string ToString()
	{
		string text = "null";
		if (textSpans != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			for (int i = 0; i < textSpans.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(textSpans[i].ToString());
			}
			stringBuilder.Append("]");
			text = stringBuilder.ToString();
		}
		return string.Format("{0}: {1}\n", "fontAsset", fontAsset) + string.Format("{0}: {1}\n", "textSettings", textSettings) + "text: " + this.text + "\n" + string.Format("{0}: {1}\n", "screenWidth", screenWidth) + string.Format("{0}: {1}\n", "screenHeight", screenHeight) + string.Format("{0}: {1}\n", "fontSize", fontSize) + string.Format("{0}: {1}\n", "bestFit", bestFit) + string.Format("{0}: {1}\n", "maxFontSize", maxFontSize) + string.Format("{0}: {1}\n", "minFontSize", minFontSize) + string.Format("{0}: {1}\n", "wordWrapEnabled", wordWrapEnabled) + string.Format("{0}: {1}\n", "languageDirection", languageDirection) + string.Format("{0}: {1}\n", "horizontalAlignment", horizontalAlignment) + string.Format("{0}: {1}\n", "verticalAlignment", verticalAlignment) + string.Format("{0}: {1}\n", "color", color) + string.Format("{0}: {1}\n", "fontStyle", fontStyle) + string.Format("{0}: {1}\n", "fontWeight", fontWeight) + string.Format("{0}: {1}\n", "vertexPadding", vertexPadding) + string.Format("{0}: {1}\n", "overflow", overflow) + "textSpans: " + text + "\n" + string.Format("{0}: {1}\n", "characterSpacing", characterSpacing) + string.Format("{0}: {1}\n", "paragraphSpacing", paragraphSpacing) + string.Format("{0}: {1}\n", "wordSpacing", wordSpacing) + string.Format("{0}: {1}\n", "preProcessFlags", preProcessFlags);
	}
}
