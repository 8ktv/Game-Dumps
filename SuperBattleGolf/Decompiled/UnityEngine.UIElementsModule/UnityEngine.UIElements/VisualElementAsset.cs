using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class VisualElementAsset : UxmlAsset, ISerializationCallbackReceiver
{
	[SerializeField]
	private string m_Name = string.Empty;

	[SerializeField]
	private int m_RuleIndex = -1;

	[SerializeField]
	private string m_Text = string.Empty;

	[SerializeField]
	private PickingMode m_PickingMode = PickingMode.Position;

	[SerializeField]
	private string[] m_Classes;

	[SerializeField]
	private List<string> m_StylesheetPaths;

	[SerializeField]
	private List<StyleSheet> m_Stylesheets;

	[SerializeReference]
	internal UxmlSerializedData m_SerializedData;

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
		set
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

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (!string.IsNullOrEmpty(m_Name) && !m_Properties.Contains("name"))
		{
			SetAttribute("name", m_Name);
		}
		if (!string.IsNullOrEmpty(m_Text) && !m_Properties.Contains("text"))
		{
			SetAttribute("text", m_Text);
		}
		if (m_PickingMode != PickingMode.Position && !m_Properties.Contains("picking-mode") && !m_Properties.Contains("pickingMode"))
		{
			SetAttribute("picking-mode", m_PickingMode.ToString());
		}
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

	public override string ToString()
	{
		return $"{m_Name}({base.fullTypeName})({base.id})";
	}
}
