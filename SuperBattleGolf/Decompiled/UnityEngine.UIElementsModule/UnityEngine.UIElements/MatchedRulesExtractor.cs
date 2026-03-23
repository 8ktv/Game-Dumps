using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class MatchedRulesExtractor
{
	private static readonly Func<StyleSheet, string> k_defaultGetPath = (StyleSheet ss) => ss.name;

	private Func<StyleSheet, string> m_GetStyleSheetPath;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal HashSet<MatchedRule> selectedElementRules = new HashSet<MatchedRule>(MatchedRule.lineNumberFullPathComparer);

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal HashSet<string> selectedElementStylesheets = new HashSet<string>();

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal List<SelectorMatchRecord> matchRecords = new List<SelectorMatchRecord>();

	public Func<StyleSheet, string> getStyleSheetPath
	{
		get
		{
			return m_GetStyleSheetPath ?? k_defaultGetPath;
		}
		set
		{
			m_GetStyleSheetPath = value;
		}
	}

	public IEnumerable<MatchedRule> GetMatchedRules()
	{
		return selectedElementRules;
	}

	public MatchedRulesExtractor(Func<StyleSheet, string> getAssetPath)
	{
		getStyleSheetPath = getAssetPath;
	}

	private void SetupParents(VisualElement target, StyleMatchingContext matchingContext)
	{
		if (target.hierarchy.parent != null)
		{
			SetupParents(target.hierarchy.parent, matchingContext);
		}
		matchingContext.ancestorFilter.PushElement(target);
		if (target.styleSheetList == null)
		{
			return;
		}
		foreach (StyleSheet styleSheet2 in target.styleSheetList)
		{
			string name;
			if (!(styleSheet2 == null))
			{
				name = getStyleSheetPath(styleSheet2);
				if (string.IsNullOrEmpty(name) || styleSheet2.isDefaultStyleSheet)
				{
					name = styleSheet2.name;
				}
				RecursivePrintStyleSheetNames(styleSheet2);
				selectedElementStylesheets.Add(name);
				matchingContext.AddStyleSheet(styleSheet2);
			}
			void RecursivePrintStyleSheetNames(StyleSheet importedSheet)
			{
				for (int i = 0; i < importedSheet.imports.Length; i++)
				{
					StyleSheet styleSheet = importedSheet.imports[i].styleSheet;
					if (styleSheet != null)
					{
						name = name + "\n(" + styleSheet.name + ")";
						matchingContext.AddStyleSheet(styleSheet);
						RecursivePrintStyleSheetNames(styleSheet);
					}
				}
			}
		}
	}

	public void FindMatchingRules(VisualElement target)
	{
		StyleMatchingContext styleMatchingContext = new StyleMatchingContext(delegate
		{
		})
		{
			currentElement = target
		};
		SetupParents(target, styleMatchingContext);
		matchRecords.Clear();
		StyleSelectorHelper.FindMatches(styleMatchingContext, matchRecords);
		matchRecords.Sort(SelectorMatchRecord.Compare);
		foreach (SelectorMatchRecord matchRecord in matchRecords)
		{
			selectedElementRules.Add(new MatchedRule(matchRecord, getStyleSheetPath(matchRecord.sheet)));
		}
	}

	public void Clear()
	{
		selectedElementRules.Clear();
		selectedElementStylesheets.Clear();
		matchRecords.Clear();
	}
}
