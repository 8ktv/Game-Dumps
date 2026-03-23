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

	public int fontSize;

	public Color32 color;

	public FontStyles fontStyle;

	public TextFontWeight fontWeight;

	public int linkID;

	public HorizontalAlignment alignment;

	public override string ToString()
	{
		return string.Format("{0}: {1}\n", "color", color) + string.Format("{0}: {1}\n", "fontStyle", fontStyle) + string.Format("{0}: {1}\n", "fontWeight", fontWeight) + string.Format("{0}: {1}\n", "linkID", linkID) + string.Format("{0}: {1}\n", "fontSize", fontSize) + string.Format("{0}: {1}", "fontAsset", fontAsset) + string.Format("{0}: {1}\n", "startIndex", startIndex) + string.Format("{0}: {1}", "length", length);
	}
}
