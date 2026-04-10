#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.UIElements.StyleSheets;

internal static class StyleSelectorHelper
{
	private struct SelectorWorkItem
	{
		public StyleSheet.OrderedSelectorType type;

		public string input;

		public SelectorWorkItem(StyleSheet.OrderedSelectorType type, string input)
		{
			this.type = type;
			this.input = input;
		}
	}

	public static MatchResultInfo MatchesSelector(VisualElement element, StyleSelector selector)
	{
		bool flag = true;
		StyleSelectorPart[] parts = selector.parts;
		int num = parts.Length;
		for (int i = 0; i < num && flag; i++)
		{
			switch (parts[i].type)
			{
			case StyleSelectorType.Class:
				flag = element.ClassListContains(parts[i].value);
				break;
			case StyleSelectorType.ID:
				flag = string.Equals(element.name, parts[i].value, StringComparison.Ordinal);
				break;
			case StyleSelectorType.Type:
				flag = string.Equals(element.typeName, parts[i].value, StringComparison.Ordinal);
				break;
			case StyleSelectorType.Predicate:
				flag = parts[i].tempData is UQuery.IVisualPredicateWrapper visualPredicateWrapper && visualPredicateWrapper.Predicate(element);
				break;
			case StyleSelectorType.PseudoClass:
				if (selector.pseudoStateMask == -1 || selector.negatedPseudoStateMask == -1)
				{
					flag = false;
				}
				break;
			default:
				flag = false;
				break;
			case StyleSelectorType.Wildcard:
				break;
			}
		}
		int num2 = 0;
		int num3 = 0;
		bool flag2 = flag;
		if (flag2 && selector.pseudoStateMask != 0)
		{
			flag = ((uint)selector.pseudoStateMask & (uint)element.pseudoStates) == (uint)selector.pseudoStateMask;
			if (flag)
			{
				num3 = selector.pseudoStateMask;
			}
			else
			{
				num2 = selector.pseudoStateMask;
			}
		}
		if (flag2 && selector.negatedPseudoStateMask != 0)
		{
			flag &= ((uint)selector.negatedPseudoStateMask & (uint)(~element.pseudoStates)) == (uint)selector.negatedPseudoStateMask;
			if (flag)
			{
				num2 |= selector.negatedPseudoStateMask;
			}
			else
			{
				num3 |= selector.negatedPseudoStateMask;
			}
		}
		return new MatchResultInfo(flag, (PseudoStates)num2, (PseudoStates)num3);
	}

	public static bool MatchRightToLeft(VisualElement element, StyleComplexSelector complexSelector, Action<VisualElement, MatchResultInfo> processResult)
	{
		VisualElement visualElement = element;
		int num = complexSelector.selectors.Length - 1;
		VisualElement visualElement2 = null;
		int num2 = -1;
		while (num >= 0 && visualElement != null)
		{
			MatchResultInfo arg = MatchesSelector(visualElement, complexSelector.selectors[num]);
			processResult(visualElement, arg);
			if (!arg.success)
			{
				if (num < complexSelector.selectors.Length - 1 && complexSelector.selectors[num + 1].previousRelationship == StyleSelectorRelationship.Descendent)
				{
					visualElement = visualElement.parent;
					continue;
				}
				if (visualElement2 != null)
				{
					visualElement = visualElement2;
					num = num2;
					continue;
				}
				break;
			}
			if (num < complexSelector.selectors.Length - 1 && complexSelector.selectors[num + 1].previousRelationship == StyleSelectorRelationship.Descendent)
			{
				visualElement2 = visualElement.parent;
				num2 = num;
			}
			if (--num < 0)
			{
				return true;
			}
			visualElement = visualElement.parent;
		}
		return false;
	}

	private static void TestSelectorLinkedList(StyleComplexSelector currentComplexSelector, List<SelectorMatchRecord> matchedSelectors, StyleMatchingContext context, ref SelectorMatchRecord record)
	{
		while (currentComplexSelector != null)
		{
			bool flag = true;
			bool flag2 = false;
			if (!currentComplexSelector.isSimple)
			{
				flag = context.ancestorFilter.IsCandidate(currentComplexSelector);
			}
			if (flag)
			{
				flag2 = MatchRightToLeft(context.currentElement, currentComplexSelector, context.processResult);
			}
			if (flag2)
			{
				record.complexSelector = currentComplexSelector;
				matchedSelectors.Add(record);
			}
			currentComplexSelector = currentComplexSelector.nextInTable;
		}
	}

	private static void FastLookup(IDictionary<string, StyleComplexSelector> table, List<SelectorMatchRecord> matchedSelectors, StyleMatchingContext context, string input, ref SelectorMatchRecord record)
	{
		if (table.TryGetValue(input, out var value))
		{
			TestSelectorLinkedList(value, matchedSelectors, context, ref record);
		}
	}

	public static void FindMatches(StyleMatchingContext context, List<SelectorMatchRecord> matchedSelectors)
	{
		VisualElement currentElement = context.currentElement;
		int num = context.styleSheetCount - 1;
		if (currentElement.styleSheetList != null)
		{
			int num2 = currentElement.styleSheetList.Count;
			for (int i = 0; i < currentElement.styleSheetList.Count; i++)
			{
				StyleSheet styleSheet = currentElement.styleSheetList[i];
				if (styleSheet.flattenedRecursiveImports != null)
				{
					num2 += styleSheet.flattenedRecursiveImports.Count;
				}
			}
			num -= num2;
		}
		FindMatches(context, matchedSelectors, num);
	}

	public static void FindMatches(StyleMatchingContext context, List<SelectorMatchRecord> matchedSelectors, int parentSheetIndex)
	{
		Debug.Assert(matchedSelectors.Count == 0);
		Debug.Assert(context.currentElement != null, "context.currentElement != null");
		bool flag = false;
		HashSet<StyleSheet> hashSet = CollectionPool<HashSet<StyleSheet>, StyleSheet>.Get();
		List<SelectorWorkItem> list = CollectionPool<List<SelectorWorkItem>, SelectorWorkItem>.Get();
		try
		{
			VisualElement currentElement = context.currentElement;
			list.Add(new SelectorWorkItem(StyleSheet.OrderedSelectorType.Type, currentElement.typeName));
			if (!string.IsNullOrEmpty(currentElement.name))
			{
				list.Add(new SelectorWorkItem(StyleSheet.OrderedSelectorType.Name, currentElement.name));
			}
			List<string> classesForIteration = currentElement.GetClassesForIteration();
			int count = classesForIteration.Count;
			for (int i = 0; i < count; i++)
			{
				list.Add(new SelectorWorkItem(StyleSheet.OrderedSelectorType.Class, classesForIteration[i]));
			}
			for (int num = context.styleSheetCount - 1; num >= 0; num--)
			{
				StyleSheet styleSheetAt = context.GetStyleSheetAt(num);
				if (hashSet.Add(styleSheetAt))
				{
					styleSheetAt.RebuildIfNecessary();
					if (num > parentSheetIndex)
					{
						currentElement.pseudoStates |= PseudoStates.Root;
						flag = true;
					}
					else
					{
						currentElement.pseudoStates &= ~PseudoStates.Root;
					}
					SelectorMatchRecord record = new SelectorMatchRecord(styleSheetAt, num);
					for (int j = 0; j < list.Count; j++)
					{
						SelectorWorkItem selectorWorkItem = list[j];
						if ((styleSheetAt.nonEmptyTablesMask & (1 << (int)selectorWorkItem.type)) != 0)
						{
							Dictionary<string, StyleComplexSelector> table = styleSheetAt.tables[(int)selectorWorkItem.type];
							FastLookup(table, matchedSelectors, context, selectorWorkItem.input, ref record);
						}
					}
					if (flag)
					{
						TestSelectorLinkedList(styleSheetAt.firstRootSelector, matchedSelectors, context, ref record);
					}
					TestSelectorLinkedList(styleSheetAt.firstWildCardSelector, matchedSelectors, context, ref record);
				}
			}
			if (flag)
			{
				currentElement.pseudoStates &= ~PseudoStates.Root;
			}
		}
		finally
		{
			CollectionPool<List<SelectorWorkItem>, SelectorWorkItem>.Release(list);
			CollectionPool<HashSet<StyleSheet>, StyleSheet>.Release(hashSet);
		}
	}
}
