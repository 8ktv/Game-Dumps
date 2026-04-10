using System;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal struct TextSpan
{
	public int startIndex;

	public int length;

	public IntPtr fontAsset;

	public IntPtr gradientAsset;

	public int fontSize;

	public Color32 color;

	public FontStyles fontStyle;

	public TextFontWeight fontWeight;

	public int mspace;

	public RichTextTagParser.TagUnitType mspaceUnitType;

	public int cspace;

	public RichTextTagParser.TagUnitType cspaceUnitType;

	public int linkID;

	public HorizontalAlignment alignment;

	public Color32 highlightColor;

	public Vector4 highlightPadding;

	public GlyphMetrics spriteMetrics;

	public EntityId spriteID;

	public bool spriteTint;

	public int spriteScale;

	public Color32 spriteColor;

	public int margin;

	public MarginDirection marginDirection;

	public RichTextTagParser.TagUnitType marginUnitType;

	public int lineHeight;

	public RichTextTagParser.TagUnitType lineHeightUnitType;

	public int indent;

	public RichTextTagParser.TagUnitType indentUnitType;

	public int vOffset;

	public RichTextTagParser.TagUnitType vOffsetUnitType;

	public sbyte subscriptNestingLevel;

	public sbyte superscriptNestingLevel;

	public override string ToString()
	{
		return string.Format("{0}: {1}\n", "color", color) + string.Format("{0}: {1}\n", "fontStyle", fontStyle) + string.Format("{0}: {1}\n", "fontWeight", fontWeight) + string.Format("{0}: {1}\n", "linkID", linkID) + string.Format("{0}: {1}\n", "fontSize", fontSize) + string.Format("{0}: {1}", "fontAsset", fontAsset) + string.Format("{0}: {1}\n", "gradientAsset", gradientAsset) + string.Format("{0}: {1}\n", "startIndex", startIndex) + string.Format("{0}: {1}", "length", length);
	}
}
