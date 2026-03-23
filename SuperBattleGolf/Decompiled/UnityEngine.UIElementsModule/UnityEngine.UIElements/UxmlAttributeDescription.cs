using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public abstract class UxmlAttributeDescription
{
	public enum Use
	{
		None,
		Optional,
		Prohibited,
		Required
	}

	protected const string xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

	private string[] m_ObsoleteNames;

	public string name { get; set; }

	public IEnumerable<string> obsoleteNames
	{
		get
		{
			return m_ObsoleteNames;
		}
		set
		{
			if (value is string[] array)
			{
				m_ObsoleteNames = array;
			}
			else
			{
				m_ObsoleteNames = value.ToArray();
			}
		}
	}

	public string type { get; protected internal set; }

	public string typeNamespace { get; protected set; }

	public abstract string defaultValueAsString { get; }

	public Use use { get; set; }

	public UxmlTypeRestriction restriction { get; set; }

	protected UxmlAttributeDescription()
	{
		use = Use.Optional;
		restriction = null;
	}

	internal bool TryFindValueInAttributeOverrides(string elementName, CreationContext cc, List<TemplateAsset.AttributeOverride> attributeOverrides, out string value)
	{
		value = null;
		TemplateAsset.AttributeOverride attributeOverride = default(TemplateAsset.AttributeOverride);
		foreach (TemplateAsset.AttributeOverride attributeOverride2 in attributeOverrides)
		{
			if (cc.namesPath == null)
			{
				if (attributeOverride2.m_ElementName != elementName)
				{
					continue;
				}
			}
			else if (!attributeOverride2.NamesPathMatchesElementNamesPath(cc.namesPath))
			{
				continue;
			}
			if (attributeOverride2.m_AttributeName != name)
			{
				if (m_ObsoleteNames == null)
				{
					continue;
				}
				bool flag = false;
				string[] array = m_ObsoleteNames;
				foreach (string text in array)
				{
					if (!(attributeOverride2.m_AttributeName != text))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			if (attributeOverride.m_AttributeName == null)
			{
				attributeOverride = attributeOverride2;
				if (attributeOverride.m_NamesPath == null)
				{
					break;
				}
			}
			else if (attributeOverride.m_NamesPath.Length < attributeOverride2.m_NamesPath.Length)
			{
				attributeOverride = attributeOverride2;
			}
		}
		if (attributeOverride.m_AttributeName != null)
		{
			value = attributeOverride.m_Value;
			return true;
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value)
	{
		VisualTreeAsset sourceAsset;
		return TryGetValueFromBagAsString(bag, cc, out value, out sourceAsset);
	}

	internal bool TryGetAttributeOverrideValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
	{
		bag.TryGetAttributeValue("name", out var value2);
		if (!string.IsNullOrEmpty(value2) && cc.attributeOverrides != null)
		{
			foreach (CreationContext.AttributeOverrideRange attributeOverride in cc.attributeOverrides)
			{
				if (TryFindValueInAttributeOverrides(value2, cc, attributeOverride.attributeOverrides, out value))
				{
					sourceAsset = attributeOverride.sourceAsset;
					return true;
				}
			}
		}
		sourceAsset = null;
		value = null;
		return false;
	}

	internal bool ValidateName()
	{
		if (name == null && (m_ObsoleteNames == null || m_ObsoleteNames.Length == 0))
		{
			Debug.LogError("Attribute description has no name.");
			return false;
		}
		return true;
	}

	internal bool TryGetValueFromBagAsString(IUxmlAttributes bag, CreationContext cc, out string value, out VisualTreeAsset sourceAsset)
	{
		value = null;
		sourceAsset = null;
		if (!ValidateName())
		{
			return false;
		}
		if (TryGetAttributeOverrideValueFromBagAsString(bag, cc, out value, out sourceAsset))
		{
			return true;
		}
		if (name == null)
		{
			for (int i = 0; i < m_ObsoleteNames.Length; i++)
			{
				if (bag.TryGetAttributeValue(m_ObsoleteNames[i], out value))
				{
					sourceAsset = cc.visualTreeAsset;
					return true;
				}
			}
			return false;
		}
		if (!bag.TryGetAttributeValue(name, out value))
		{
			if (m_ObsoleteNames != null)
			{
				for (int j = 0; j < m_ObsoleteNames.Length; j++)
				{
					if (bag.TryGetAttributeValue(m_ObsoleteNames[j], out value))
					{
						sourceAsset = cc.visualTreeAsset;
						if (bag is UxmlAsset uxmlAsset)
						{
							uxmlAsset.RemoveAttribute(m_ObsoleteNames[j]);
							uxmlAsset.SetAttribute(name, value);
						}
						return true;
					}
				}
			}
			return false;
		}
		sourceAsset = cc.visualTreeAsset;
		return true;
	}

	protected bool TryGetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue, ref T value)
	{
		if (TryGetValueFromBagAsString(bag, cc, out var value2))
		{
			if (converterFunc != null)
			{
				value = converterFunc(value2, defaultValue);
			}
			else
			{
				value = defaultValue;
			}
			return true;
		}
		return false;
	}

	protected T GetValueFromBag<T>(IUxmlAttributes bag, CreationContext cc, Func<string, T, T> converterFunc, T defaultValue)
	{
		if (converterFunc == null)
		{
			throw new ArgumentNullException("converterFunc");
		}
		if (TryGetValueFromBagAsString(bag, cc, out var value))
		{
			return converterFunc(value, defaultValue);
		}
		return defaultValue;
	}
}
