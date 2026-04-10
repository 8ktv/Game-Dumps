using System;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleRule
{
	[SerializeField]
	private StyleComplexSelector[] m_ComplexSelectors = Array.Empty<StyleComplexSelector>();

	[SerializeField]
	private StyleProperty[] m_Properties = Array.Empty<StyleProperty>();

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[SerializeField]
	internal int line;

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int customPropertiesCount;

	[field: NonSerialized]
	internal StyleSheet styleSheet { get; set; }

	public StyleComplexSelector[] complexSelectors => m_ComplexSelectors;

	public StyleProperty[] properties => m_Properties;

	internal StyleRule(StyleSheet styleSheet)
	{
		this.styleSheet = styleSheet;
	}

	internal void SetSelectors(StyleComplexSelector[] selectors)
	{
		m_ComplexSelectors = selectors;
	}

	internal void SetProperties(StyleProperty[] props)
	{
		m_Properties = props;
	}

	public bool TryAddSelector(string selectorStr, out StyleComplexSelector selector)
	{
		string error;
		return TryAddSelector(selectorStr, out selector, out error);
	}

	public bool TryAddSelector(string selectorStr, out StyleComplexSelector selector, out string error)
	{
		if (!SelectorUtility.ExtractSelectorsAndSpecificityFromString(selectorStr, out var selectors, out var specificity, out error))
		{
			selector = null;
			return false;
		}
		selector = new StyleComplexSelector
		{
			selectors = selectors,
			specificity = specificity
		};
		CollectionExtensions.AddToArray(ref m_ComplexSelectors, selector);
		selector.rule = this;
		styleSheet.RequestRebuild();
		return true;
	}

	public StyleComplexSelector AddSelector(string selectorStr)
	{
		if (!TryAddSelector(selectorStr, out var selector, out var error))
		{
			throw new InvalidOperationException(error);
		}
		return selector;
	}

	public bool RemoveSelector(StyleComplexSelector selector)
	{
		int num = Array.IndexOf(m_ComplexSelectors, selector);
		if (num < 0)
		{
			return false;
		}
		RemoveSelector(num);
		return true;
	}

	public bool RemoveSelector(int index)
	{
		if (index < 0 || index >= m_ComplexSelectors.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		StyleComplexSelector styleComplexSelector = m_ComplexSelectors[index];
		CollectionExtensions.RemoveFromArray(ref m_ComplexSelectors, index);
		styleComplexSelector.ruleIndex = -1;
		styleComplexSelector.rule = null;
		styleComplexSelector.nextInTable = null;
		styleSheet.RequestRebuild();
		return true;
	}

	public StyleProperty AddProperty(string propertyName)
	{
		StyleProperty styleProperty = new StyleProperty
		{
			name = propertyName
		};
		CollectionExtensions.AddToArray(ref m_Properties, styleProperty);
		if (styleProperty.isCustomProperty)
		{
			customPropertiesCount++;
		}
		styleSheet.RequestRebuild();
		return styleProperty;
	}

	public bool RemoveProperty(StyleProperty property)
	{
		int num = Array.IndexOf(m_Properties, property);
		if (num < 0)
		{
			return false;
		}
		RemoveProperty(num);
		return true;
	}

	public bool RemoveProperty(int index)
	{
		if (index < 0 || index >= m_Properties.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		StyleProperty styleProperty = m_Properties[index];
		CollectionExtensions.RemoveFromArray(ref m_Properties, index);
		if (styleProperty.isCustomProperty)
		{
			customPropertiesCount--;
		}
		styleSheet.RequestRebuild();
		return true;
	}

	public StyleProperty FindLastProperty(string propertyName)
	{
		for (int num = properties.Length - 1; num >= 0; num--)
		{
			if (properties[num].name == propertyName)
			{
				return properties[num];
			}
		}
		return null;
	}
}
