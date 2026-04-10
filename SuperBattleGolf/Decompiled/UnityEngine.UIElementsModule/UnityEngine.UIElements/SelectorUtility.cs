using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class SelectorUtility
{
	private const string k_DescendantSymbol = ">";

	public static bool ExtractSelectorsAndSpecificityFromString(string complexSelectorStr, out StyleSelector[] selectors, out int specificity, out string error)
	{
		selectors = null;
		specificity = -1;
		string[] array = complexSelectorStr.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		int selectorSpecificity = CSSSpec.GetSelectorSpecificity(complexSelectorStr);
		if (selectorSpecificity == 0)
		{
			error = "Selector '" + complexSelectorStr + "' is invalid: failed to calculate selector specificity.";
			return false;
		}
		List<StyleSelector> list = new List<StyleSelector>();
		StyleSelectorRelationship previousRelationship = StyleSelectorRelationship.None;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text == ">")
			{
				previousRelationship = StyleSelectorRelationship.Child;
				continue;
			}
			if (!CSSSpec.ParseSelector(text, out var parts))
			{
				error = "Selector '" + complexSelectorStr + "' is invalid: the selector could not be parsed.";
				return false;
			}
			for (int j = 0; j < parts.Length; j++)
			{
				StyleSelectorPart styleSelectorPart = parts[j];
				switch (styleSelectorPart.type)
				{
				case StyleSelectorType.Unknown:
					error = "Selector '" + complexSelectorStr + "' is invalid: the selector contains unknown parts.";
					return false;
				case StyleSelectorType.RecursivePseudoClass:
					error = "Selector '" + complexSelectorStr + "' is invalid: the selector contains recursive parts.";
					return false;
				}
			}
			StyleSelector item = new StyleSelector
			{
				parts = parts,
				previousRelationship = previousRelationship
			};
			list.Add(item);
			previousRelationship = StyleSelectorRelationship.Descendent;
		}
		selectors = list.ToArray();
		specificity = selectorSpecificity;
		error = null;
		return true;
	}

	public static bool CompareSelectors(StyleComplexSelector lhs, StyleComplexSelector rhs)
	{
		if (lhs.isSimple != rhs.isSimple || lhs.specificity != rhs.specificity || lhs.selectors.Length != rhs.selectors.Length)
		{
			return false;
		}
		for (int i = 0; i < lhs.selectors.Length; i++)
		{
			StyleSelector styleSelector = lhs.selectors[i];
			StyleSelector styleSelector2 = rhs.selectors[i];
			if (styleSelector.parts.Length != styleSelector2.parts.Length)
			{
				return false;
			}
			if (styleSelector.previousRelationship != styleSelector2.previousRelationship)
			{
				return false;
			}
			for (int j = 0; j < styleSelector.parts.Length; j++)
			{
				if (!EqualityComparer<StyleSelectorPart>.Default.Equals(styleSelector.parts[j], styleSelector2.parts[j]))
				{
					return false;
				}
			}
		}
		return true;
	}
}
