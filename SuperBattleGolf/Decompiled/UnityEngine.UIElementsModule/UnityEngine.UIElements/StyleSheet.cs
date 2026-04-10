using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[Serializable]
[HelpURL("UIE-USS")]
public class StyleSheet : ScriptableObject
{
	[Flags]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal enum RebuildOptions
	{
		None = 0,
		Synchronous = 1
	}

	[Serializable]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct ImportStruct
	{
		public StyleSheet styleSheet;

		public string[] mediaQueries;
	}

	internal enum OrderedSelectorType
	{
		None = -1,
		Name,
		Type,
		Class,
		Length
	}

	[NonSerialized]
	private bool m_RequiresRebuild = true;

	[SerializeField]
	private bool m_ImportedWithErrors;

	[SerializeField]
	private bool m_ImportedWithWarnings;

	[SerializeField]
	private StyleRule[] m_Rules = Array.Empty<StyleRule>();

	[SerializeField]
	internal float[] floats = Array.Empty<float>();

	[SerializeField]
	internal Dimension[] dimensions = Array.Empty<Dimension>();

	[SerializeField]
	internal Color[] colors = Array.Empty<Color>();

	[SerializeField]
	internal string[] strings = Array.Empty<string>();

	[SerializeField]
	internal Object[] assets = Array.Empty<Object>();

	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal ImportStruct[] imports = Array.Empty<ImportStruct>();

	[SerializeField]
	private List<StyleSheet> m_FlattenedImportedStyleSheets = new List<StyleSheet>();

	[SerializeField]
	private int m_ContentHash;

	[SerializeField]
	internal ScalableImage[] scalableImages = Array.Empty<ScalableImage>();

	[NonSerialized]
	internal Dictionary<string, StyleComplexSelector>[] m_Tables;

	[NonSerialized]
	internal int nonEmptyTablesMask;

	[NonSerialized]
	internal StyleComplexSelector firstRootSelector;

	[NonSerialized]
	internal StyleComplexSelector firstWildCardSelector;

	[NonSerialized]
	private bool m_IsDefaultStyleSheet;

	public bool importedWithErrors
	{
		get
		{
			return m_ImportedWithErrors;
		}
		internal set
		{
			m_ImportedWithErrors = value;
		}
	}

	public bool importedWithWarnings
	{
		get
		{
			return m_ImportedWithWarnings;
		}
		internal set
		{
			m_ImportedWithWarnings = value;
		}
	}

	internal StyleRule[] rules
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_Rules;
		}
	}

	internal List<StyleSheet> flattenedRecursiveImports
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_FlattenedImportedStyleSheets;
		}
	}

	public int contentHash
	{
		get
		{
			return m_ContentHash;
		}
		set
		{
			m_ContentHash = value;
		}
	}

	internal Dictionary<string, StyleComplexSelector>[] tables
	{
		get
		{
			Dictionary<string, StyleComplexSelector>[] array = m_Tables;
			if (array == null)
			{
				Dictionary<string, StyleComplexSelector>[] obj = new Dictionary<string, StyleComplexSelector>[3]
				{
					new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal),
					new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal),
					new Dictionary<string, StyleComplexSelector>(StringComparer.Ordinal)
				};
				Dictionary<string, StyleComplexSelector>[] array2 = obj;
				m_Tables = obj;
				array = array2;
			}
			return array;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool isDefaultStyleSheet
	{
		get
		{
			return m_IsDefaultStyleSheet;
		}
		set
		{
			m_IsDefaultStyleSheet = value;
			if (flattenedRecursiveImports == null)
			{
				return;
			}
			foreach (StyleSheet flattenedRecursiveImport in flattenedRecursiveImports)
			{
				flattenedRecursiveImport.isDefaultStyleSheet = value;
			}
		}
	}

	private bool TryCheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle, out T value)
	{
		if (handle.valueType != type || handle.valueIndex < 0 || handle.valueIndex >= list.Length)
		{
			value = default(T);
			return false;
		}
		value = list[handle.valueIndex];
		return true;
	}

	private T CheckAccess<T>(T[] list, StyleValueType type, StyleValueHandle handle)
	{
		T result = default(T);
		if (handle.valueType != type)
		{
			Debug.LogErrorFormat(this, "Trying to read value of type {0} while reading a value of type {1}", type, handle.valueType);
		}
		else
		{
			if (list != null && handle.valueIndex >= 0 && handle.valueIndex < list.Length)
			{
				return list[handle.valueIndex];
			}
			Debug.LogError("Accessing invalid property", this);
		}
		return result;
	}

	internal virtual void OnEnable()
	{
		SetupReferences();
	}

	internal void FlattenImportedStyleSheetsRecursive()
	{
		m_FlattenedImportedStyleSheets = new List<StyleSheet>();
		FlattenImportedStyleSheetsRecursive(this);
	}

	private void FlattenImportedStyleSheetsRecursive(StyleSheet sheet)
	{
		if (sheet.imports == null)
		{
			return;
		}
		for (int i = 0; i < sheet.imports.Length; i++)
		{
			StyleSheet styleSheet = sheet.imports[i].styleSheet;
			if (!(styleSheet == null))
			{
				styleSheet.isDefaultStyleSheet = isDefaultStyleSheet;
				FlattenImportedStyleSheetsRecursive(styleSheet);
				m_FlattenedImportedStyleSheets.Add(styleSheet);
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleRule AddRule()
	{
		return AddRuleAtIndex(-1);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleRule AddRuleAtIndex(int index)
	{
		return AddRuleAtIndex(index, null);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleRule AddRule(string selector)
	{
		return AddRuleAtIndex(-1, selector);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleRule AddRuleAtIndex(int index, string selector)
	{
		if (index == -1)
		{
			index = rules.Length;
		}
		StyleRule styleRule = new StyleRule(this);
		if (!string.IsNullOrEmpty(selector))
		{
			styleRule.AddSelector(selector);
		}
		InsertValueInArray(ref m_Rules, index, styleRule);
		RequestRebuild();
		return styleRule;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool RemoveRule(StyleRule rule)
	{
		if (rule.styleSheet != this)
		{
			return false;
		}
		int num = Array.IndexOf(m_Rules, rule);
		if (num < 0)
		{
			return false;
		}
		RemoveRule(num);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RemoveRule(int ruleIndex)
	{
		if (ruleIndex < 0 || ruleIndex >= m_Rules.Length)
		{
			throw new ArgumentOutOfRangeException("ruleIndex");
		}
		StyleRule styleRule = rules[ruleIndex];
		Unity.Collections.CollectionExtensions.RemoveFromArray(ref m_Rules, ruleIndex);
		styleRule.styleSheet = null;
		RequestRebuild();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetRules(StyleRule[] newRules)
	{
		m_Rules = newRules;
		SetupReferences();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RequestRebuild(RebuildOptions options = RebuildOptions.None)
	{
		m_RequiresRebuild = true;
		MarkAsChanged();
		if ((options & RebuildOptions.Synchronous) == RebuildOptions.Synchronous)
		{
			RebuildIfNecessary();
		}
	}

	internal void RebuildIfNecessary()
	{
		if (m_RequiresRebuild)
		{
			SetupReferences();
		}
	}

	internal void SetupReferences()
	{
		if (tables != null)
		{
			tables[0].Clear();
			tables[1].Clear();
			tables[2].Clear();
		}
		nonEmptyTablesMask = 0;
		firstRootSelector = null;
		firstWildCardSelector = null;
		if (rules == null || rules.Length == 0)
		{
			m_RequiresRebuild = false;
			return;
		}
		int num = 0;
		for (int i = 0; i < rules.Length; i++)
		{
			StyleRule styleRule = rules[i];
			styleRule.styleSheet = this;
			if (styleRule.complexSelectors == null)
			{
				continue;
			}
			StyleComplexSelector[] complexSelectors = styleRule.complexSelectors;
			foreach (StyleComplexSelector styleComplexSelector in complexSelectors)
			{
				styleComplexSelector.rule = styleRule;
				styleComplexSelector.ruleIndex = i;
				styleComplexSelector.nextInTable = null;
				styleComplexSelector.CachePseudoStateMasks(this);
				styleComplexSelector.CalculateHashes();
				styleComplexSelector.orderInStyleSheet = num++;
				StyleSelector styleSelector = styleComplexSelector.selectors[^1];
				StyleSelectorPart styleSelectorPart = styleSelector.parts[0];
				string value = styleSelectorPart.value;
				OrderedSelectorType orderedSelectorType = OrderedSelectorType.None;
				switch (styleSelectorPart.type)
				{
				case StyleSelectorType.Class:
					orderedSelectorType = OrderedSelectorType.Class;
					break;
				case StyleSelectorType.ID:
					orderedSelectorType = OrderedSelectorType.Name;
					break;
				case StyleSelectorType.Type:
					value = styleSelectorPart.value;
					orderedSelectorType = OrderedSelectorType.Type;
					break;
				case StyleSelectorType.Wildcard:
					if (firstWildCardSelector != null)
					{
						styleComplexSelector.nextInTable = firstWildCardSelector;
					}
					firstWildCardSelector = styleComplexSelector;
					break;
				case StyleSelectorType.PseudoClass:
					if ((styleSelector.pseudoStateMask & 0x80) != 0)
					{
						if (firstRootSelector != null)
						{
							styleComplexSelector.nextInTable = firstRootSelector;
						}
						firstRootSelector = styleComplexSelector;
					}
					else
					{
						if (firstWildCardSelector != null)
						{
							styleComplexSelector.nextInTable = firstWildCardSelector;
						}
						firstWildCardSelector = styleComplexSelector;
					}
					break;
				default:
					Debug.LogError($"Invalid first part type {styleSelectorPart.type}", this);
					break;
				}
				if (orderedSelectorType != OrderedSelectorType.None)
				{
					Dictionary<string, StyleComplexSelector> dictionary = tables[(int)orderedSelectorType];
					if (dictionary.TryGetValue(value, out var value2))
					{
						styleComplexSelector.nextInTable = value2;
					}
					nonEmptyTablesMask |= 1 << (int)orderedSelectorType;
					dictionary[value] = styleComplexSelector;
				}
			}
			styleRule.customPropertiesCount = 0;
			StyleProperty[] properties = styleRule.properties;
			foreach (StyleProperty styleProperty in properties)
			{
				if (styleProperty.isCustomProperty)
				{
					styleRule.customPropertiesCount++;
				}
				StyleValueHandle[] values = styleProperty.values;
				foreach (StyleValueHandle styleValueHandle in values)
				{
					if (styleValueHandle.IsVarFunction())
					{
						styleProperty.requireVariableResolve = true;
						break;
					}
				}
			}
		}
		m_RequiresRebuild = false;
	}

	private int AddValueToArray<T>(ref T[] array, T value)
	{
		Unity.Collections.CollectionExtensions.AddToArray(ref array, value);
		MarkAsChanged();
		return array.Length - 1;
	}

	private int InsertValueInArray<T>(ref T[] array, int index, T value)
	{
		Unity.Collections.CollectionExtensions.InsertIntoArray(ref array, index, value);
		MarkAsChanged();
		return index;
	}

	internal int AddValue(StyleValueKeyword keyword)
	{
		MarkAsChanged();
		return (int)keyword;
	}

	internal int AddValue(StyleValueFunction function)
	{
		MarkAsChanged();
		return (int)function;
	}

	internal int AddValue(float value)
	{
		return AddValueToArray(ref floats, value);
	}

	internal int AddValue(Dimension value)
	{
		return AddValueToArray(ref dimensions, value);
	}

	internal int AddValue(Color value)
	{
		return AddValueToArray(ref colors, value);
	}

	internal int AddValue(ScalableImage value)
	{
		return AddValueToArray(ref scalableImages, value);
	}

	internal int AddValue(string value)
	{
		return AddValueToArray(ref strings, value);
	}

	internal int AddValue(Object value)
	{
		return AddValueToArray(ref assets, value);
	}

	internal int AddValue(Enum value)
	{
		string enumExportString = StyleSheetUtility.GetEnumExportString(value);
		return AddValueToArray(ref strings, enumExportString);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleValueKeyword ReadKeyword(StyleValueHandle handle)
	{
		return (StyleValueKeyword)handle.valueIndex;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryReadKeyword(StyleValueHandle handle, out StyleValueKeyword value)
	{
		value = (StyleValueKeyword)handle.valueIndex;
		return handle.valueType == StyleValueType.Keyword;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal float ReadFloat(StyleValueHandle handle)
	{
		if (handle.valueType == StyleValueType.Dimension)
		{
			return CheckAccess(dimensions, StyleValueType.Dimension, handle).value;
		}
		return CheckAccess(floats, StyleValueType.Float, handle);
	}

	internal bool TryReadFloat(StyleValueHandle handle, out float value)
	{
		if (TryCheckAccess(floats, StyleValueType.Float, handle, out value))
		{
			return true;
		}
		Dimension value2;
		bool result = TryCheckAccess(dimensions, StyleValueType.Float, handle, out value2);
		value = value2.value;
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Dimension ReadDimension(StyleValueHandle handle)
	{
		if (handle.valueType == StyleValueType.Float)
		{
			float value = CheckAccess(floats, StyleValueType.Float, handle);
			return new Dimension(value, Dimension.Unit.Unitless);
		}
		return CheckAccess(dimensions, StyleValueType.Dimension, handle);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryReadDimension(StyleValueHandle handle, out Dimension value)
	{
		if (TryCheckAccess(dimensions, StyleValueType.Dimension, handle, out value))
		{
			return true;
		}
		float value2;
		bool result = TryCheckAccess(floats, StyleValueType.Float, handle, out value2);
		value = new Dimension(value2, Dimension.Unit.Unitless);
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Color ReadColor(StyleValueHandle handle)
	{
		if (handle.valueType == StyleValueType.Enum)
		{
			string text = ReadEnum(handle);
			StyleSheetColor.TryGetColor(text.ToLowerInvariant(), out var color);
			return color;
		}
		return CheckAccess(colors, StyleValueType.Color, handle);
	}

	internal bool TryReadColor(StyleValueHandle handle, out Color value)
	{
		if (TryCheckAccess(colors, StyleValueType.Color, handle, out value))
		{
			return true;
		}
		if (TryCheckAccess(strings, StyleValueType.Enum, handle, out var value2))
		{
			return StyleSheetColor.TryGetColor(value2.ToLowerInvariant(), out value);
		}
		value = default(Color);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadString(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.String, handle);
	}

	internal bool TryReadString(StyleValueHandle handle, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.String, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadEnum(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.Enum, handle);
	}

	internal bool TryReadEnum(StyleValueHandle handle, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.Enum, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal TEnum ReadEnum<TEnum>(StyleValueHandle handle) where TEnum : struct, Enum
	{
		string dash = ReadEnum(handle);
		TEnum result;
		return Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(dash), out result) ? result : default(TEnum);
	}

	internal bool TryReadEnum<TEnum>(StyleValueHandle handle, out TEnum value) where TEnum : struct, Enum
	{
		if (TryReadEnum(handle, out var value2) && Enum.TryParse<TEnum>(StyleSheetUtility.ConvertDashToHungarian(value2), out value))
		{
			return true;
		}
		value = default(TEnum);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadVariable(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.Variable, handle);
	}

	internal bool TryReadVariable(StyleValueHandle handle, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.Variable, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadResourcePath(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.ResourcePath, handle);
	}

	internal bool TryReadResourcePath(StyleValueHandle handle, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.ResourcePath, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Object ReadAssetReference(StyleValueHandle handle)
	{
		return CheckAccess(assets, StyleValueType.AssetReference, handle);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadMissingAssetReferenceUrl(StyleValueHandle handle)
	{
		return CheckAccess(strings, StyleValueType.MissingAssetReference, handle);
	}

	internal bool TryReadMissingAssetReferenceUrl(StyleValueHandle handle, out string value)
	{
		return TryCheckAccess(strings, StyleValueType.MissingAssetReference, handle, out value);
	}

	internal bool TryReadAssetReference(StyleValueHandle handle, out Object value)
	{
		return TryCheckAccess(assets, StyleValueType.AssetReference, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleValueFunction ReadFunction(StyleValueHandle handle)
	{
		return (StyleValueFunction)handle.valueIndex;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryReadFunction(StyleValueHandle handle, out StyleValueFunction value)
	{
		value = (StyleValueFunction)handle.valueIndex;
		return handle.valueType == StyleValueType.Function;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal string ReadFunctionName(StyleValueHandle handle)
	{
		if (handle.valueType != StyleValueType.Function)
		{
			Debug.LogErrorFormat(this, $"Trying to read value of type {StyleValueType.Function} while reading a value of type {handle.valueType}");
			return string.Empty;
		}
		StyleValueFunction valueIndex = (StyleValueFunction)handle.valueIndex;
		return valueIndex.ToUssString();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal ScalableImage ReadScalableImage(StyleValueHandle handle)
	{
		return CheckAccess(scalableImages, StyleValueType.ScalableImage, handle);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryReadScalableImage(StyleValueHandle handle, out ScalableImage value)
	{
		return TryCheckAccess(scalableImages, StyleValueType.ScalableImage, handle, out value);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StylePropertyName ReadStylePropertyName(StyleValueHandle handle)
	{
		return new StylePropertyName(CheckAccess(strings, StyleValueType.Enum, handle));
	}

	internal bool TryReadStylePropertyName(StyleValueHandle handle, out StylePropertyName value)
	{
		if (TryCheckAccess(strings, StyleValueType.Enum, handle, out var value2))
		{
			value = new StylePropertyName(value2);
			return true;
		}
		value = default(StylePropertyName);
		return false;
	}

	internal Length ReadLength(StyleValueHandle handle)
	{
		if (handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword styleValueKeyword = ReadKeyword(handle);
			if (1 == 0)
			{
			}
			Length result = styleValueKeyword switch
			{
				StyleValueKeyword.Auto => Length.Auto(), 
				StyleValueKeyword.None => Length.None(), 
				_ => default(Length), 
			};
			if (1 == 0)
			{
			}
			return result;
		}
		Dimension dimension = ReadDimension(handle);
		return dimension.IsLength() ? dimension.ToLength() : default(Length);
	}

	internal bool TryReadLength(StyleValueHandle handle, out Length value)
	{
		if (TryReadKeyword(handle, out var value2))
		{
			switch (value2)
			{
			case StyleValueKeyword.Auto:
				value = Length.Auto();
				return true;
			case StyleValueKeyword.None:
				value = Length.None();
				return true;
			default:
				value = default(Length);
				return false;
			}
		}
		if (TryReadDimension(handle, out var value3) && value3.IsLength())
		{
			value = value3.ToLength();
			return true;
		}
		value = default(Length);
		return false;
	}

	internal Angle ReadAngle(StyleValueHandle handle)
	{
		if (handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword styleValueKeyword = ReadKeyword(handle);
			if (1 == 0)
			{
			}
			Angle result = ((styleValueKeyword != StyleValueKeyword.None) ? default(Angle) : Angle.None());
			if (1 == 0)
			{
			}
			return result;
		}
		Dimension dimension = ReadDimension(handle);
		return dimension.IsAngle() ? dimension.ToAngle() : default(Angle);
	}

	internal bool TryReadAngle(StyleValueHandle handle, out Angle value)
	{
		if (TryReadKeyword(handle, out var value2))
		{
			StyleValueKeyword styleValueKeyword = value2;
			StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
			if (styleValueKeyword2 == StyleValueKeyword.None)
			{
				value = Angle.None();
				return true;
			}
			value = default(Angle);
			return false;
		}
		if (TryReadDimension(handle, out var value3) && value3.IsAngle())
		{
			value = value3.ToAngle();
			return true;
		}
		value = default(Angle);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal TimeValue ReadTimeValue(StyleValueHandle handle)
	{
		Dimension dimension = ReadDimension(handle);
		return dimension.IsTimeValue() ? dimension.ToTime() : default(TimeValue);
	}

	internal bool TryReadTimeValue(StyleValueHandle handle, out TimeValue value)
	{
		if (TryReadDimension(handle, out var value2) && value2.IsTimeValue())
		{
			value = value2.ToTime();
			return true;
		}
		value = default(TimeValue);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteKeyword(ref StyleValueHandle handle, StyleValueKeyword value)
	{
		handle.valueType = StyleValueType.Keyword;
		handle.valueIndex = (int)value;
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteFloat(ref StyleValueHandle handle, float value)
	{
		if (handle.valueType == StyleValueType.Float)
		{
			floats[handle.valueIndex] = value;
		}
		else
		{
			int valueIndex = AddValue(value);
			handle.valueType = StyleValueType.Float;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteDimension(ref StyleValueHandle handle, Dimension dimension)
	{
		if (handle.valueType == StyleValueType.Dimension)
		{
			dimensions[handle.valueIndex] = dimension;
		}
		else
		{
			int valueIndex = AddValue(dimension);
			handle.valueType = StyleValueType.Dimension;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteColor(ref StyleValueHandle handle, Color color)
	{
		if (handle.valueType == StyleValueType.Color)
		{
			colors[handle.valueIndex] = color;
		}
		else
		{
			int valueIndex = AddValue(color);
			handle.valueType = StyleValueType.Color;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteString(ref StyleValueHandle handle, string value)
	{
		if (handle.valueType == StyleValueType.String)
		{
			strings[handle.valueIndex] = value;
		}
		else
		{
			int valueIndex = AddValue(value);
			handle.valueType = StyleValueType.String;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteEnum<TEnum>(ref StyleValueHandle handle, TEnum value) where TEnum : Enum
	{
		string enumExportString = StyleSheetUtility.GetEnumExportString(value);
		WriteEnumAsString(ref handle, enumExportString);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteEnumAsString(ref StyleValueHandle handle, string valueStr)
	{
		if (handle.valueType == StyleValueType.Enum)
		{
			strings[handle.valueIndex] = valueStr;
		}
		else
		{
			int valueIndex = AddValue(valueStr);
			handle.valueType = StyleValueType.Enum;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteVariable(ref StyleValueHandle handle, string variableName)
	{
		if (handle.valueType == StyleValueType.Variable)
		{
			strings[handle.valueIndex] = variableName;
		}
		else
		{
			int valueIndex = AddValue(variableName);
			handle.valueType = StyleValueType.Variable;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteResourcePath(ref StyleValueHandle handle, string resourcePath)
	{
		if (handle.valueType == StyleValueType.ResourcePath)
		{
			strings[handle.valueIndex] = resourcePath;
		}
		else
		{
			int valueIndex = AddValue(resourcePath);
			handle.valueType = StyleValueType.ResourcePath;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteAssetReference(ref StyleValueHandle handle, Object value)
	{
		if (handle.valueType == StyleValueType.AssetReference)
		{
			assets[handle.valueIndex] = value;
		}
		else
		{
			int valueIndex = AddValue(value);
			handle.valueType = StyleValueType.AssetReference;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteMissingAssetReferenceUrl(ref StyleValueHandle handle, string assetReference)
	{
		if (handle.valueType == StyleValueType.MissingAssetReference)
		{
			strings[handle.valueIndex] = assetReference;
		}
		else
		{
			int valueIndex = AddValue(assetReference);
			handle.valueType = StyleValueType.MissingAssetReference;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteFunction(ref StyleValueHandle handle, StyleValueFunction function)
	{
		handle.valueType = StyleValueType.Function;
		handle.valueIndex = (int)function;
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteScalableImage(ref StyleValueHandle handle, ScalableImage scalableImage)
	{
		if (handle.valueType == StyleValueType.ScalableImage)
		{
			scalableImages[handle.valueIndex] = scalableImage;
		}
		else
		{
			int valueIndex = AddValue(scalableImage);
			handle.valueType = StyleValueType.ScalableImage;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteStylePropertyName(ref StyleValueHandle handle, StylePropertyName propertyName)
	{
		string text = ((propertyName.id != StylePropertyId.Unknown) ? propertyName.ToString() : "ignored");
		if (handle.valueType == StyleValueType.Enum)
		{
			strings[handle.valueIndex] = text;
		}
		else
		{
			int valueIndex = AddValue(text);
			handle.valueType = StyleValueType.Enum;
			handle.valueIndex = valueIndex;
		}
		MarkAsChanged();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void WriteCommaSeparator(ref StyleValueHandle handle)
	{
		handle.valueIndex = 0;
		handle.valueType = StyleValueType.CommaSeparator;
		MarkAsChanged();
	}

	internal void WriteLength(ref StyleValueHandle handle, Length value)
	{
		if (value.IsAuto())
		{
			WriteKeyword(ref handle, StyleValueKeyword.Auto);
		}
		else if (value.IsNone())
		{
			WriteKeyword(ref handle, StyleValueKeyword.None);
		}
		else
		{
			WriteDimension(ref handle, value.ToDimension());
		}
	}

	internal void WriteAngle(ref StyleValueHandle handle, Angle value)
	{
		if (value.IsNone())
		{
			WriteKeyword(ref handle, StyleValueKeyword.None);
		}
		else
		{
			WriteDimension(ref handle, value.ToDimension());
		}
	}

	internal void WriteTimeValue(ref StyleValueHandle handle, TimeValue value)
	{
		WriteDimension(ref handle, value.ToDimension());
	}

	private void MarkAsChanged()
	{
		if (rules == null || rules.Length == 0)
		{
			contentHash = 0;
		}
		else
		{
			contentHash = Random.Range(1, int.MaxValue);
		}
		UIElementsUtility.MarkStyleSheetAsChanged(this);
	}
}
