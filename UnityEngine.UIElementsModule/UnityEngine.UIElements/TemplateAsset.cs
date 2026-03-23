#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class TemplateAsset : VisualElementAsset
{
	[Serializable]
	public struct AttributeOverride
	{
		public string m_ElementName;

		public string[] m_NamesPath;

		public string m_AttributeName;

		public string m_Value;

		public bool NamesPathMatchesElementNamesPath(IList<string> elementNamesPath)
		{
			if (elementNamesPath == null || m_NamesPath == null || elementNamesPath.Count == 0 || m_NamesPath.Length == 0)
			{
				return false;
			}
			if (m_NamesPath.Length == 1)
			{
				return m_NamesPath[0] == elementNamesPath[elementNamesPath.Count - 1];
			}
			if (m_NamesPath.Length != elementNamesPath.Count)
			{
				return false;
			}
			for (int num = elementNamesPath.Count - 1; num >= 0; num--)
			{
				if (elementNamesPath[num] != m_NamesPath[num])
				{
					return false;
				}
			}
			return true;
		}
	}

	[Serializable]
	public struct UxmlSerializedDataOverride
	{
		public int m_ElementId;

		public List<int> m_ElementIdsPath;

		[SerializeReference]
		public UxmlSerializedData m_SerializedData;
	}

	[SerializeField]
	private string m_TemplateAlias;

	[SerializeField]
	private List<AttributeOverride> m_AttributeOverrides = new List<AttributeOverride>();

	[SerializeField]
	private List<UxmlSerializedDataOverride> m_SerializedDataOverride = new List<UxmlSerializedDataOverride>();

	[SerializeField]
	private List<VisualTreeAsset.SlotUsageEntry> m_SlotUsages;

	public string templateAlias
	{
		get
		{
			return m_TemplateAlias;
		}
		set
		{
			m_TemplateAlias = value;
		}
	}

	public List<AttributeOverride> attributeOverrides
	{
		get
		{
			return m_AttributeOverrides;
		}
		set
		{
			m_AttributeOverrides = value;
		}
	}

	public bool hasAttributeOverride
	{
		get
		{
			List<AttributeOverride> list = m_AttributeOverrides;
			return list != null && list.Count > 0;
		}
	}

	public List<UxmlSerializedDataOverride> serializedDataOverrides
	{
		get
		{
			return m_SerializedDataOverride;
		}
		set
		{
			m_SerializedDataOverride = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal List<VisualTreeAsset.SlotUsageEntry> slotUsages
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_SlotUsages;
		}
		set
		{
			m_SlotUsages = value;
		}
	}

	internal override VisualElement Instantiate(CreationContext cc)
	{
		TemplateContainer templateContainer = (TemplateContainer)base.Instantiate(cc);
		if (templateContainer.templateSource == null)
		{
			templateContainer.templateSource = cc.visualTreeAsset?.ResolveTemplate(templateContainer.templateId);
			if (templateContainer.templateSource == null)
			{
				templateContainer.Add(new Label("Unknown Template: '" + templateContainer.templateId + "'"));
				return templateContainer;
			}
		}
		List<CreationContext.AttributeOverrideRange> value;
		using (CollectionPool<List<CreationContext.AttributeOverrideRange>, CreationContext.AttributeOverrideRange>.Get(out value))
		{
			List<CreationContext.SerializedDataOverrideRange> value2;
			using (CollectionPool<List<CreationContext.SerializedDataOverrideRange>, CreationContext.SerializedDataOverrideRange>.Get(out value2))
			{
				if (cc.attributeOverrides != null)
				{
					value.AddRange(cc.attributeOverrides);
				}
				if (attributeOverrides.Count > 0)
				{
					value.Add(new CreationContext.AttributeOverrideRange(cc.visualTreeAsset, attributeOverrides));
				}
				if (cc.serializedDataOverrides != null)
				{
					value2.AddRange(cc.serializedDataOverrides);
				}
				if (serializedDataOverrides.Count > 0)
				{
					value2.Add(new CreationContext.SerializedDataOverrideRange(cc.visualTreeAsset, serializedDataOverrides, base.id));
				}
				List<int> veaIdsPath = ((cc.veaIdsPath != null) ? new List<int>(cc.veaIdsPath) : new List<int>());
				CreationContext cc2 = new CreationContext(cc.slotInsertionPoints, value, value2, null, null, veaIdsPath, null);
				templateContainer.templateSource.CloneTree(templateContainer, cc2);
				return templateContainer;
			}
		}
	}

	public TemplateAsset(string templateAlias, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
		: base(fullTypeName, xmlNamespace)
	{
		Assert.IsFalse(string.IsNullOrEmpty(templateAlias), "Template alias must not be null or empty");
		m_TemplateAlias = templateAlias;
	}

	public void AddSlotUsage(string slotName, int resId)
	{
		if (m_SlotUsages == null)
		{
			m_SlotUsages = new List<VisualTreeAsset.SlotUsageEntry>();
		}
		m_SlotUsages.Add(new VisualTreeAsset.SlotUsageEntry(slotName, resId));
	}
}
