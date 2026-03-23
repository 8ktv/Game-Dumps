using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct SelectorMatchRecord : IEquatable<SelectorMatchRecord>
{
	public StyleSheet sheet;

	public int styleSheetIndexInStack;

	public StyleComplexSelector complexSelector;

	public SelectorMatchRecord(StyleSheet sheet, int styleSheetIndexInStack)
	{
		this = default(SelectorMatchRecord);
		this.sheet = sheet;
		this.styleSheetIndexInStack = styleSheetIndexInStack;
	}

	public static int Compare(SelectorMatchRecord a, SelectorMatchRecord b)
	{
		if (a.sheet.isDefaultStyleSheet != b.sheet.isDefaultStyleSheet)
		{
			return (!a.sheet.isDefaultStyleSheet) ? 1 : (-1);
		}
		int num = a.complexSelector.specificity.CompareTo(b.complexSelector.specificity);
		if (num == 0)
		{
			num = a.styleSheetIndexInStack.CompareTo(b.styleSheetIndexInStack);
		}
		if (num == 0)
		{
			num = a.complexSelector.orderInStyleSheet.CompareTo(b.complexSelector.orderInStyleSheet);
		}
		return num;
	}

	public bool Equals(SelectorMatchRecord other)
	{
		return object.Equals(sheet, other.sheet) && styleSheetIndexInStack == other.styleSheetIndexInStack && object.Equals(complexSelector, other.complexSelector);
	}

	public override bool Equals(object obj)
	{
		return obj is SelectorMatchRecord other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(sheet, styleSheetIndexInStack, complexSelector);
	}
}
