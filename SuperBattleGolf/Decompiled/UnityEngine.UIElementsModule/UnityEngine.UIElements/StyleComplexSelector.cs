using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleComplexSelector
{
	private struct PseudoStateData
	{
		public readonly PseudoStates state;

		public readonly bool negate;

		public PseudoStateData(PseudoStates state, bool negate)
		{
			this.state = state;
			this.negate = negate;
		}
	}

	[NonSerialized]
	public Hashes ancestorHashes;

	[SerializeField]
	private int m_Specificity;

	[SerializeField]
	private StyleSelector[] m_Selectors = Array.Empty<StyleSelector>();

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int ruleIndex;

	[NonSerialized]
	internal StyleComplexSelector nextInTable;

	[NonSerialized]
	internal int orderInStyleSheet;

	private static Dictionary<string, PseudoStateData> s_PseudoStates;

	private static readonly List<StyleSelectorPart> s_HashList = new List<StyleSelectorPart>();

	public int specificity
	{
		get
		{
			return m_Specificity;
		}
		internal set
		{
			m_Specificity = value;
		}
	}

	[field: NonSerialized]
	public StyleRule rule
	{
		get; [VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set;
	}

	public bool isSimple
	{
		get
		{
			StyleSelector[] array = selectors;
			return array != null && array.Length == 1;
		}
	}

	public StyleSelector[] selectors
	{
		get
		{
			return m_Selectors;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Selectors = value;
		}
	}

	internal StyleComplexSelector()
	{
	}

	public bool TrySetSelectorsFromString(string complexSelectorStr, out string error)
	{
		if (!SelectorUtility.ExtractSelectorsAndSpecificityFromString(complexSelectorStr, out var array, out var num, out error))
		{
			return false;
		}
		selectors = array;
		specificity = num;
		error = null;
		return true;
	}

	internal void CachePseudoStateMasks(StyleSheet styleSheet)
	{
		if (s_PseudoStates == null)
		{
			s_PseudoStates = new Dictionary<string, PseudoStateData>();
			s_PseudoStates["active"] = new PseudoStateData(PseudoStates.Active, negate: false);
			s_PseudoStates["hover"] = new PseudoStateData(PseudoStates.Hover, negate: false);
			s_PseudoStates["checked"] = new PseudoStateData(PseudoStates.Checked, negate: false);
			s_PseudoStates["selected"] = new PseudoStateData(PseudoStates.Checked, negate: false);
			s_PseudoStates["disabled"] = new PseudoStateData(PseudoStates.Disabled, negate: false);
			s_PseudoStates["focus"] = new PseudoStateData(PseudoStates.Focus, negate: false);
			s_PseudoStates["root"] = new PseudoStateData(PseudoStates.Root, negate: false);
			s_PseudoStates["inactive"] = new PseudoStateData(PseudoStates.Active, negate: true);
			s_PseudoStates["enabled"] = new PseudoStateData(PseudoStates.Disabled, negate: true);
		}
		int i = 0;
		for (int num = selectors.Length; i < num; i++)
		{
			StyleSelector styleSelector = selectors[i];
			StyleSelectorPart[] parts = styleSelector.parts;
			PseudoStates pseudoStates = PseudoStates.None;
			PseudoStates pseudoStates2 = PseudoStates.None;
			bool flag = true;
			for (int j = 0; j < styleSelector.parts.Length && flag; j++)
			{
				if (styleSelector.parts[j].type != StyleSelectorType.PseudoClass)
				{
					continue;
				}
				if (s_PseudoStates.TryGetValue(parts[j].value, out var value))
				{
					if (!value.negate)
					{
						pseudoStates |= value.state;
					}
					else
					{
						pseudoStates2 |= value.state;
					}
				}
				else
				{
					Debug.LogWarningFormat(styleSheet, "Unknown pseudo class \"{0}\" in StyleSheet {1}", parts[j].value, styleSheet.name);
					flag = false;
				}
			}
			if (flag)
			{
				styleSelector.pseudoStateMask = (int)pseudoStates;
				styleSelector.negatedPseudoStateMask = (int)pseudoStates2;
			}
			else
			{
				styleSelector.pseudoStateMask = -1;
				styleSelector.negatedPseudoStateMask = -1;
			}
		}
	}

	public override string ToString()
	{
		return string.Format("[{0}]", string.Join(", ", m_Selectors.Select((StyleSelector x) => x.ToString()).ToArray()));
	}

	private static int StyleSelectorPartCompare(StyleSelectorPart x, StyleSelectorPart y)
	{
		if (y.type < x.type)
		{
			return -1;
		}
		if (y.type > x.type)
		{
			return 1;
		}
		return y.value.CompareTo(x.value);
	}

	internal unsafe void CalculateHashes()
	{
		if (isSimple)
		{
			return;
		}
		for (int num = selectors.Length - 2; num > -1; num--)
		{
			s_HashList.AddRange(selectors[num].parts);
		}
		s_HashList.RemoveAll((StyleSelectorPart p) => p.type != StyleSelectorType.Class && p.type != StyleSelectorType.ID && p.type != StyleSelectorType.Type);
		s_HashList.Sort(StyleSelectorPartCompare);
		bool flag = true;
		StyleSelectorType styleSelectorType = StyleSelectorType.Unknown;
		string text = "";
		int num2 = 0;
		int num3 = Math.Min(4, s_HashList.Count);
		for (int num4 = 0; num4 < num3; num4++)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				for (; num2 < s_HashList.Count && s_HashList[num2].type == styleSelectorType && s_HashList[num2].value == text; num2++)
				{
				}
				if (num2 == s_HashList.Count)
				{
					break;
				}
			}
			styleSelectorType = s_HashList[num2].type;
			text = s_HashList[num2].value;
			ancestorHashes.hashes[num4] = text.GetHashCode() * styleSelectorType switch
			{
				StyleSelectorType.ID => 17, 
				StyleSelectorType.Class => 19, 
				_ => 13, 
			};
		}
		s_HashList.Clear();
	}
}
