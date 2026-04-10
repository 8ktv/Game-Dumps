using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule", "UnityEditor.UIToolkitAuthoringModule" })]
internal class VisualElementAsset : UxmlAsset
{
	internal const string k_LostInlineStyles = "VisualElementAsset previously had inline styles that were lost.";

	[SerializeField]
	private int m_RuleIndex = -1;

	[SerializeField]
	private string[] m_Classes = Array.Empty<string>();

	[SerializeField]
	private List<string> m_StylesheetPaths;

	[SerializeField]
	private List<StyleSheet> m_Stylesheets;

	[SerializeReference]
	private UxmlSerializedData m_SerializedData;

	[SerializeField]
	private bool m_SkipClone;

	public int ruleIndex
	{
		get
		{
			return m_RuleIndex;
		}
		set
		{
			m_RuleIndex = value;
		}
	}

	public string[] classes
	{
		get
		{
			return m_Classes;
		}
		internal set
		{
			m_Classes = value;
		}
	}

	public List<string> stylesheetPaths
	{
		get
		{
			return m_StylesheetPaths ?? (m_StylesheetPaths = new List<string>());
		}
		set
		{
			m_StylesheetPaths = value;
		}
	}

	public bool hasStylesheetPaths => m_StylesheetPaths != null;

	public List<StyleSheet> stylesheets
	{
		get
		{
			return m_Stylesheets ?? (m_Stylesheets = new List<StyleSheet>());
		}
		set
		{
			m_Stylesheets = value;
		}
	}

	public bool hasStylesheets => m_Stylesheets != null;

	public UxmlSerializedData serializedData
	{
		get
		{
			return m_SerializedData;
		}
		set
		{
			m_SerializedData = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool skipClone
	{
		get
		{
			return m_SkipClone;
		}
		set
		{
			m_SkipClone = value;
		}
	}

	public VisualElementAsset(string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		: base(fullTypeName, xmlNamespace)
	{
	}

	private static bool IdsPathMatchesAttributeOverrideIdsPath(List<int> idsPath, List<int> attributeOverrideIdsPath, int templateId)
	{
		if (idsPath == null || attributeOverrideIdsPath == null || idsPath.Count == 0 || attributeOverrideIdsPath.Count == 0)
		{
			return false;
		}
		int num = idsPath.IndexOf(templateId);
		if (idsPath.Count != attributeOverrideIdsPath.Count + num + 1)
		{
			return false;
		}
		for (int num2 = idsPath.Count - 1; num2 > num; num2--)
		{
			if (idsPath[num2] != attributeOverrideIdsPath[num2 - num - 1])
			{
				return false;
			}
		}
		return true;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal virtual VisualElement Instantiate(CreationContext cc)
	{
		VisualElement visualElement = (VisualElement)serializedData.CreateInstance();
		serializedData.Deserialize(visualElement);
		if (cc.hasOverrides)
		{
			cc.veaIdsPath.Add(base.id);
			for (int num = cc.serializedDataOverrides.Count - 1; num >= 0; num--)
			{
				foreach (TemplateAsset.UxmlSerializedDataOverride attributeOverride in cc.serializedDataOverrides[num].attributeOverrides)
				{
					if (attributeOverride.m_ElementId == base.id && IdsPathMatchesAttributeOverrideIdsPath(cc.veaIdsPath, attributeOverride.m_ElementIdsPath, cc.serializedDataOverrides[num].templateId))
					{
						attributeOverride.m_SerializedData.Deserialize(visualElement);
					}
				}
			}
			cc.veaIdsPath.Remove(base.id);
		}
		if (hasStylesheetPaths)
		{
			for (int i = 0; i < stylesheetPaths.Count; i++)
			{
				visualElement.AddStyleSheetPath(stylesheetPaths[i]);
			}
		}
		if (hasStylesheets)
		{
			for (int j = 0; j < stylesheets.Count; j++)
			{
				if (stylesheets[j] != null)
				{
					visualElement.styleSheets.Add(stylesheets[j]);
				}
			}
		}
		if (classes != null)
		{
			for (int k = 0; k < classes.Length; k++)
			{
				visualElement.AddToClassList(classes[k]);
			}
		}
		return visualElement;
	}

	internal override bool Accepts(UxmlAsset asset, out string errorMessage)
	{
		bool flag = !asset.isRoot;
		errorMessage = ((!flag) ? "[UI Toolkit] Cannot add a root UXML asset as a children of a UXML asset." : null);
		return flag;
	}

	public override string ToString()
	{
		string value;
		return TryGetAttributeValue("name", out value) ? $"{value}({base.fullTypeName})({base.id})" : $"({base.fullTypeName})({base.id})";
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void AddStyleSheet(StyleSheet styleSheet)
	{
		if (!(styleSheet == null) && !stylesheets.Contains(styleSheet))
		{
			stylesheets.Add(styleSheet);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	public void AddStyleSheets(IEnumerable<StyleSheet> styleSheets)
	{
		foreach (StyleSheet styleSheet in styleSheets)
		{
			AddStyleSheet(styleSheet);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RemoveStyleSheet(StyleSheet styleSheet)
	{
		stylesheets.RemoveAll((StyleSheet s) => s == styleSheet);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void AddStyleClass(string className)
	{
		if (m_Classes == null)
		{
			m_Classes = Array.Empty<string>();
		}
		if (Array.IndexOf(m_Classes, className) == -1)
		{
			Unity.Collections.CollectionExtensions.AddToArray(ref m_Classes, className);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RemoveStyleClass(string className)
	{
		if (m_Classes != null)
		{
			Unity.Collections.CollectionExtensions.RemoveFromArray(ref m_Classes, className);
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	public void ClearStyleSheets()
	{
		stylesheets.Clear();
	}

	private protected override void OnVisualTreeAssetChanged(VisualTreeAsset previousVta, VisualTreeAsset newVta)
	{
		base.OnVisualTreeAssetChanged(previousVta, newVta);
		if (ruleIndex >= 0)
		{
			if (!previousVta)
			{
				ruleIndex = -1;
				Debug.LogWarning("VisualElementAsset previously had inline styles that were lost.");
			}
			else if ((bool)newVta)
			{
				VisualTreeAsset.SwallowStyleRule(previousVta, newVta, this);
			}
		}
	}
}
