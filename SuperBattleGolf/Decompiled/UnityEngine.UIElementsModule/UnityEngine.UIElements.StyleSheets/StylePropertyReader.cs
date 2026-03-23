using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements.StyleSheets;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StylePropertyReader
{
	internal delegate int GetCursorIdFunction(StyleSheet sheet, StyleValueHandle handle);

	internal static GetCursorIdFunction getCursorIdFunc;

	private List<StylePropertyValue> m_Values = new List<StylePropertyValue>();

	private List<int> m_ValueCount = new List<int>();

	private StyleVariableResolver m_Resolver = new StyleVariableResolver();

	private StyleSheet m_Sheet;

	private StyleProperty[] m_Properties;

	private StylePropertyId[] m_PropertyIds;

	private int m_CurrentPropertyIndex;

	private int m_CurrentValueIndex { get; set; }

	public StyleProperty property { get; private set; }

	public StylePropertyId propertyId { get; private set; }

	public int valueCount { get; private set; }

	public float dpiScaling { get; private set; }

	public void SetContext(StyleSheet sheet, StyleComplexSelector selector, StyleVariableContext varContext, float dpiScaling = 1f)
	{
		m_Sheet = sheet;
		m_Properties = selector.rule.properties;
		m_PropertyIds = StyleSheetCache.GetPropertyIds(sheet, selector.ruleIndex);
		m_Resolver.variableContext = varContext;
		this.dpiScaling = dpiScaling;
		LoadProperties();
	}

	public void SetInlineContext(StyleSheet sheet, StyleProperty[] properties, StylePropertyId[] propertyIds, float dpiScaling = 1f)
	{
		m_Sheet = sheet;
		m_Properties = properties;
		m_PropertyIds = propertyIds;
		this.dpiScaling = dpiScaling;
		LoadProperties();
	}

	public StylePropertyId MoveNextProperty()
	{
		m_CurrentPropertyIndex++;
		m_CurrentValueIndex += valueCount;
		SetCurrentProperty();
		return propertyId;
	}

	public StylePropertyValue GetValue(int index)
	{
		return m_Values[m_CurrentValueIndex + index];
	}

	public StyleValueType GetValueType(int index)
	{
		return m_Values[m_CurrentValueIndex + index].handle.valueType;
	}

	public bool IsValueType(int index, StyleValueType type)
	{
		return m_Values[m_CurrentValueIndex + index].handle.valueType == type;
	}

	public bool IsKeyword(int index, StyleValueKeyword keyword)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return stylePropertyValue.handle.valueType == StyleValueType.Keyword && stylePropertyValue.handle.valueIndex == (int)keyword;
	}

	public string ReadAsString(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
	}

	public Length ReadLength(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		if (stylePropertyValue.handle.valueType == StyleValueType.Keyword)
		{
			return (StyleValueKeyword)stylePropertyValue.handle.valueIndex switch
			{
				StyleValueKeyword.Auto => Length.Auto(), 
				StyleValueKeyword.None => Length.None(), 
				_ => default(Length), 
			};
		}
		return stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToLength();
	}

	public TimeValue ReadTimeValue(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToTime();
	}

	public Translate ReadTranslate(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		return ReadTranslate(valueCount, val, val2, val3);
	}

	public TransformOrigin ReadTransformOrigin(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue zVvalue = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		return ReadTransformOrigin(valueCount, val, val2, zVvalue);
	}

	public Rotate ReadRotate(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		StylePropertyValue val4 = ((valueCount > 3) ? m_Values[m_CurrentValueIndex + index + 3] : default(StylePropertyValue));
		return ReadRotate(valueCount, val, val2, val3, val4);
	}

	public Scale ReadScale(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		return ReadScale(valueCount, val, val2, val3);
	}

	public float ReadFloat(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle);
	}

	public int ReadInt(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return (int)stylePropertyValue.sheet.ReadFloat(stylePropertyValue.handle);
	}

	public Color ReadColor(int index)
	{
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		return stylePropertyValue.sheet.ReadColor(stylePropertyValue.handle);
	}

	public int ReadEnum(StyleEnumType enumType, int index)
	{
		string text = null;
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		StyleValueHandle handle = stylePropertyValue.handle;
		if (handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword svk = stylePropertyValue.sheet.ReadKeyword(handle);
			text = svk.ToUssString();
		}
		else
		{
			text = stylePropertyValue.sheet.ReadEnum(handle);
		}
		StylePropertyUtil.TryGetEnumIntValue(enumType, text, out var intValue);
		return intValue;
	}

	public Object ReadAsset(int index)
	{
		Object result = null;
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		switch (stylePropertyValue.handle.valueType)
		{
		case StyleValueType.ResourcePath:
		{
			string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
			if (!string.IsNullOrEmpty(text))
			{
				result = Panel.LoadResource(text, typeof(Object), dpiScaling);
			}
			break;
		}
		case StyleValueType.AssetReference:
			result = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle);
			break;
		}
		return result;
	}

	public FontDefinition ReadFontDefinition(int index)
	{
		FontAsset fontAsset = null;
		Font font = null;
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		switch (stylePropertyValue.handle.valueType)
		{
		case StyleValueType.ResourcePath:
		{
			string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
			if (!string.IsNullOrEmpty(text))
			{
				font = Panel.LoadResource(text, typeof(Font), dpiScaling) as Font;
				if (font == null)
				{
					fontAsset = Panel.LoadResource(text, typeof(FontAsset), dpiScaling) as FontAsset;
				}
			}
			if (fontAsset == null && font == null)
			{
				Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Font not found for path: {0}", text));
			}
			break;
		}
		case StyleValueType.AssetReference:
			font = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Font;
			if (font == null)
			{
				fontAsset = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as FontAsset;
			}
			break;
		case StyleValueType.Keyword:
			if (stylePropertyValue.handle.valueIndex != 6)
			{
				Debug.LogWarning("Invalid keyword for font " + (StyleValueKeyword)stylePropertyValue.handle.valueIndex/*cast due to .constrained prefix*/);
			}
			break;
		default:
			Debug.LogWarning("Invalid value for font " + stylePropertyValue.handle.valueType);
			break;
		}
		if (font != null)
		{
			return FontDefinition.FromFont(font);
		}
		if (fontAsset != null)
		{
			return FontDefinition.FromSDFFont(fontAsset);
		}
		return default(FontDefinition);
	}

	public Font ReadFont(int index)
	{
		Font font = null;
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		switch (stylePropertyValue.handle.valueType)
		{
		case StyleValueType.ResourcePath:
		{
			string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
			if (!string.IsNullOrEmpty(text))
			{
				font = Panel.LoadResource(text, typeof(Font), dpiScaling) as Font;
			}
			if (font == null)
			{
				Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Font not found for path: {0}", text));
			}
			break;
		}
		case StyleValueType.AssetReference:
			font = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Font;
			break;
		case StyleValueType.Keyword:
			if (stylePropertyValue.handle.valueIndex != 6)
			{
				Debug.LogWarning("Invalid keyword for font " + (StyleValueKeyword)stylePropertyValue.handle.valueIndex/*cast due to .constrained prefix*/);
			}
			break;
		default:
			Debug.LogWarning("Invalid value for font " + stylePropertyValue.handle.valueType);
			break;
		}
		return font;
	}

	public Material ReadMaterial(int index)
	{
		Material material = null;
		StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
		switch (stylePropertyValue.handle.valueType)
		{
		case StyleValueType.ResourcePath:
		{
			string text = stylePropertyValue.sheet.ReadResourcePath(stylePropertyValue.handle);
			if (!string.IsNullOrEmpty(text))
			{
				material = Panel.LoadResource(text, typeof(Material), dpiScaling) as Material;
			}
			if (material == null)
			{
				Debug.LogWarning(string.Format(CultureInfo.InvariantCulture, "Material not found for path: {0}", text));
			}
			break;
		}
		case StyleValueType.AssetReference:
			material = stylePropertyValue.sheet.ReadAssetReference(stylePropertyValue.handle) as Material;
			break;
		case StyleValueType.Keyword:
			if (stylePropertyValue.handle.valueIndex != 6)
			{
				Debug.LogWarning("Invalid keyword for material " + (StyleValueKeyword)stylePropertyValue.handle.valueIndex/*cast due to .constrained prefix*/);
			}
			break;
		default:
			Debug.LogWarning("Invalid value for material " + stylePropertyValue.handle.valueType);
			break;
		}
		return material;
	}

	public Background ReadBackground(int index)
	{
		ImageSource source = default(ImageSource);
		StylePropertyValue propertyValue = m_Values[m_CurrentValueIndex + index];
		if (propertyValue.handle.valueType == StyleValueType.Keyword)
		{
			if (propertyValue.handle.valueIndex != 6)
			{
				Debug.LogWarning("Invalid keyword for image source " + (StyleValueKeyword)propertyValue.handle.valueIndex/*cast due to .constrained prefix*/);
			}
		}
		else if (TryGetImageSourceFromValue(propertyValue, dpiScaling, out source))
		{
		}
		if (source.texture != null)
		{
			return Background.FromTexture2D(source.texture);
		}
		if (source.sprite != null)
		{
			return Background.FromSprite(source.sprite);
		}
		if (source.vectorImage != null)
		{
			return Background.FromVectorImage(source.vectorImage);
		}
		if (source.renderTexture != null)
		{
			return Background.FromRenderTexture(source.renderTexture);
		}
		return default(Background);
	}

	public Cursor ReadCursor(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		return ReadCursor(valueCount, val, val2, val3, dpiScaling);
	}

	public TextShadow ReadTextShadow(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		StylePropertyValue val4 = ((valueCount > 3) ? m_Values[m_CurrentValueIndex + index + 3] : default(StylePropertyValue));
		return ReadTextShadow(valueCount, val, val2, val3, val4);
	}

	public TextAutoSize ReadTextAutoSize(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		StylePropertyValue val3 = ((valueCount > 2) ? m_Values[m_CurrentValueIndex + index + 2] : default(StylePropertyValue));
		return ReadTextAutoSize(valueCount, val, val2, val3);
	}

	public BackgroundPosition ReadBackgroundPositionX(int index)
	{
		return ReadBackgroundPosition(index, BackgroundPositionKeyword.Left);
	}

	public BackgroundPosition ReadBackgroundPositionY(int index)
	{
		return ReadBackgroundPosition(index, BackgroundPositionKeyword.Top);
	}

	private BackgroundPosition ReadBackgroundPosition(int index, BackgroundPositionKeyword keyword)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		return ReadBackgroundPosition(valueCount, val, val2, keyword);
	}

	public BackgroundRepeat ReadBackgroundRepeat(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		return ReadBackgroundRepeat(valueCount, val, val2);
	}

	public BackgroundSize ReadBackgroundSize(int index)
	{
		StylePropertyValue val = m_Values[m_CurrentValueIndex + index];
		StylePropertyValue val2 = ((valueCount > 1) ? m_Values[m_CurrentValueIndex + index + 1] : default(StylePropertyValue));
		return ReadBackgroundSize(valueCount, val, val2);
	}

	public void ReadListEasingFunction(List<EasingFunction> list, int index)
	{
		list.Clear();
		do
		{
			StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
			StyleValueHandle handle = stylePropertyValue.handle;
			if (handle.valueType == StyleValueType.Enum)
			{
				string value = stylePropertyValue.sheet.ReadEnum(handle);
				StylePropertyUtil.TryGetEnumIntValue(StyleEnumType.EasingMode, value, out var intValue);
				list.Add(new EasingFunction((EasingMode)intValue));
				index++;
			}
			if (index < valueCount && m_Values[m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator)
			{
				index++;
			}
		}
		while (index < valueCount);
	}

	public void ReadListTimeValue(List<TimeValue> list, int index)
	{
		list.Clear();
		do
		{
			StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
			TimeValue item = stylePropertyValue.sheet.ReadDimension(stylePropertyValue.handle).ToTime();
			list.Add(item);
			index++;
			if (index < valueCount && m_Values[m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator)
			{
				index++;
			}
		}
		while (index < valueCount);
	}

	private FilterFunctionType ToFilterFunctionType(StyleValueFunction function)
	{
		return function switch
		{
			StyleValueFunction.CustomFilter => FilterFunctionType.Custom, 
			StyleValueFunction.FilterTint => FilterFunctionType.Tint, 
			StyleValueFunction.FilterOpacity => FilterFunctionType.Opacity, 
			StyleValueFunction.FilterInvert => FilterFunctionType.Invert, 
			StyleValueFunction.FilterGrayscale => FilterFunctionType.Grayscale, 
			StyleValueFunction.FilterSepia => FilterFunctionType.Sepia, 
			StyleValueFunction.FilterBlur => FilterFunctionType.Blur, 
			_ => FilterFunctionType.None, 
		};
	}

	public void ReadListFilterFunction(List<FilterFunction> list, int index)
	{
		list.Clear();
		do
		{
			StyleValueFunction valueIndex = (StyleValueFunction)GetValue(index++).handle.valueIndex;
			int num = ReadInt(index++);
			bool flag = false;
			FilterFunctionDefinition customDefinition = null;
			if (valueIndex == StyleValueFunction.CustomFilter && num > 0)
			{
				flag = true;
				customDefinition = ReadAsset(index++) as FilterFunctionDefinition;
				num--;
			}
			FixedBuffer4<FilterParameter> parameters = default(FixedBuffer4<FilterParameter>);
			for (int i = 0; i < num; i++)
			{
				StyleValueType valueType = GetValueType(index);
				if (valueType == StyleValueType.Color || valueType == StyleValueType.Enum)
				{
					Color colorValue = ReadColor(index++);
					parameters[i] = new FilterParameter
					{
						type = FilterParameterType.Color,
						colorValue = colorValue
					};
				}
				else if (valueType == StyleValueType.Dimension || valueType == StyleValueType.Float)
				{
					StylePropertyValue value = GetValue(index++);
					Dimension dim = value.sheet.ReadDimension(value.handle);
					parameters[i] = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = ConvertDimensionToFilterFloat(dim)
					};
				}
				else if (valueType != StyleValueType.CommaSeparator)
				{
					Debug.LogError($"Unexpected value type {valueType} in filter function argument");
				}
			}
			if (flag)
			{
				list.Add(new FilterFunction(customDefinition, parameters, num));
			}
			else
			{
				list.Add(new FilterFunction(ToFilterFunctionType(valueIndex), parameters, num));
			}
		}
		while (index < valueCount);
	}

	private float ConvertDimensionToFilterFloat(Dimension dim)
	{
		return dim.unit switch
		{
			Dimension.Unit.Percent => dim.value * 0.01f, 
			Dimension.Unit.Degree => dim.value * (MathF.PI / 180f), 
			Dimension.Unit.Turn => dim.value * MathF.PI * 2f, 
			Dimension.Unit.Gradian => dim.value * MathF.PI / 200f, 
			Dimension.Unit.Millisecond => dim.value * 0.001f, 
			_ => dim.value, 
		};
	}

	public void ReadListStylePropertyName(List<StylePropertyName> list, int index)
	{
		list.Clear();
		do
		{
			StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
			StylePropertyName item = stylePropertyValue.sheet.ReadStylePropertyName(stylePropertyValue.handle);
			list.Add(item);
			index++;
			if (index < valueCount && m_Values[m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator)
			{
				index++;
			}
		}
		while (index < valueCount);
	}

	public void ReadListString(List<string> list, int index)
	{
		list.Clear();
		do
		{
			StylePropertyValue stylePropertyValue = m_Values[m_CurrentValueIndex + index];
			string item = stylePropertyValue.sheet.ReadAsString(stylePropertyValue.handle);
			list.Add(item);
			index++;
			if (index < valueCount && m_Values[m_CurrentValueIndex + index].handle.valueType == StyleValueType.CommaSeparator)
			{
				index++;
			}
		}
		while (index < valueCount);
	}

	private void LoadProperties()
	{
		m_CurrentPropertyIndex = 0;
		m_CurrentValueIndex = 0;
		m_Values.Clear();
		m_ValueCount.Clear();
		StyleProperty[] properties = m_Properties;
		foreach (StyleProperty styleProperty in properties)
		{
			int num = 0;
			bool flag = true;
			if (styleProperty.requireVariableResolve)
			{
				m_Resolver.Init(styleProperty, m_Sheet, styleProperty.values);
				for (int j = 0; j < styleProperty.values.Length && flag; j++)
				{
					StyleValueHandle handle = styleProperty.values[j];
					if (handle.IsVarFunction())
					{
						flag = m_Resolver.ResolveVarFunction(ref j);
					}
					else
					{
						m_Resolver.AddValue(handle);
					}
				}
				if (flag && m_Resolver.ValidateResolvedValues())
				{
					m_Values.AddRange(m_Resolver.resolvedValues);
					num += m_Resolver.resolvedValues.Count;
				}
				else
				{
					StyleValueHandle handle2 = new StyleValueHandle
					{
						valueType = StyleValueType.Keyword,
						valueIndex = 3
					};
					m_Values.Add(new StylePropertyValue
					{
						sheet = m_Sheet,
						handle = handle2
					});
					num++;
				}
			}
			else
			{
				num = styleProperty.values.Length;
				for (int k = 0; k < num; k++)
				{
					m_Values.Add(new StylePropertyValue
					{
						sheet = m_Sheet,
						handle = styleProperty.values[k]
					});
				}
			}
			m_ValueCount.Add(num);
		}
		SetCurrentProperty();
	}

	private void SetCurrentProperty()
	{
		if (m_CurrentPropertyIndex < m_PropertyIds.Length)
		{
			property = m_Properties[m_CurrentPropertyIndex];
			propertyId = m_PropertyIds[m_CurrentPropertyIndex];
			valueCount = m_ValueCount[m_CurrentPropertyIndex];
		}
		else
		{
			property = null;
			propertyId = StylePropertyId.Unknown;
			valueCount = 0;
		}
	}

	public static TransformOrigin ReadTransformOrigin(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue zVvalue)
	{
		Length x = Length.Percent(50f);
		Length y = Length.Percent(50f);
		float z = 0f;
		switch (valCount)
		{
		case 1:
		{
			bool isVertical;
			bool isHorizontal;
			Length length = ReadTransformOriginEnum(val1, out isVertical, out isHorizontal);
			if (isHorizontal)
			{
				x = length;
			}
			else
			{
				y = length;
			}
			break;
		}
		case 2:
		{
			bool isVertical2;
			bool isHorizontal2;
			Length length2 = ReadTransformOriginEnum(val1, out isVertical2, out isHorizontal2);
			bool isVertical3;
			bool isHorizontal3;
			Length length3 = ReadTransformOriginEnum(val2, out isVertical3, out isHorizontal3);
			if (!isHorizontal2 || !isVertical3)
			{
				if (isHorizontal3 && isVertical2)
				{
					x = length3;
					y = length2;
				}
			}
			else
			{
				x = length2;
				y = length3;
			}
			break;
		}
		case 3:
			if (zVvalue.handle.valueType == StyleValueType.Dimension || zVvalue.handle.valueType == StyleValueType.Float)
			{
				z = zVvalue.sheet.ReadDimension(zVvalue.handle).value;
			}
			goto case 2;
		}
		return new TransformOrigin(x, y, z);
	}

	private static Length ReadTransformOriginEnum(StylePropertyValue value, out bool isVertical, out bool isHorizontal)
	{
		if (value.handle.valueType == StyleValueType.Enum)
		{
			switch ((TransformOriginOffset)ReadEnum(StyleEnumType.TransformOriginOffset, value))
			{
			case TransformOriginOffset.Left:
				isVertical = false;
				isHorizontal = true;
				return Length.Percent(0f);
			case TransformOriginOffset.Top:
				isVertical = true;
				isHorizontal = false;
				return Length.Percent(0f);
			case TransformOriginOffset.Center:
				isVertical = true;
				isHorizontal = true;
				return Length.Percent(50f);
			case TransformOriginOffset.Right:
				isVertical = false;
				isHorizontal = true;
				return Length.Percent(100f);
			case TransformOriginOffset.Bottom:
				isVertical = true;
				isHorizontal = false;
				return Length.Percent(100f);
			}
		}
		else if (value.handle.valueType == StyleValueType.Dimension || value.handle.valueType == StyleValueType.Float)
		{
			isVertical = true;
			isHorizontal = true;
			return value.sheet.ReadDimension(value.handle).ToLength();
		}
		isVertical = false;
		isHorizontal = false;
		return Length.Percent(50f);
	}

	public static Translate ReadTranslate(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
	{
		if (val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6)
		{
			return Translate.None();
		}
		Length x = 0f;
		Length y = 0f;
		float z = 0f;
		switch (valCount)
		{
		case 1:
			if (val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float)
			{
				x = val1.sheet.ReadDimension(val1.handle).ToLength();
				y = val1.sheet.ReadDimension(val1.handle).ToLength();
			}
			break;
		case 2:
			if (val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float)
			{
				x = val1.sheet.ReadDimension(val1.handle).ToLength();
			}
			if (val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float)
			{
				y = val2.sheet.ReadDimension(val2.handle).ToLength();
			}
			break;
		case 3:
			if (val3.handle.valueType == StyleValueType.Dimension || val3.handle.valueType == StyleValueType.Float)
			{
				z = val3.sheet.ReadDimension(val3.handle).value;
			}
			goto case 2;
		}
		return new Translate(x, y, z);
	}

	public static Scale ReadScale(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
	{
		if (val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6)
		{
			return Scale.None();
		}
		Vector3 one = Vector3.one;
		switch (valCount)
		{
		case 1:
			if (val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float)
			{
				one.x = val1.sheet.ReadFloat(val1.handle);
				one.y = one.x;
			}
			break;
		case 2:
			if (val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float)
			{
				one.x = val1.sheet.ReadFloat(val1.handle);
			}
			if (val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float)
			{
				one.y = val2.sheet.ReadFloat(val2.handle);
			}
			break;
		case 3:
			if (val3.handle.valueType == StyleValueType.Dimension || val3.handle.valueType == StyleValueType.Float)
			{
				one.z = val3.sheet.ReadFloat(val3.handle);
			}
			goto case 2;
		}
		return new Scale(one);
	}

	public static Rotate ReadRotate(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, StylePropertyValue val4)
	{
		if (val1.handle.valueType == StyleValueType.Keyword && val1.handle.valueIndex == 6)
		{
			return Rotate.None();
		}
		Rotate result = Rotate.Initial();
		switch (valCount)
		{
		case 1:
			if (val1.handle.valueType == StyleValueType.Dimension)
			{
				result.angle = ReadAngle(val1);
			}
			break;
		case 2:
			result.angle = ReadAngle(val2);
			switch ((Axis)ReadEnum(StyleEnumType.Axis, val1))
			{
			case Axis.X:
				result.axis = new Vector3(1f, 0f, 0f);
				break;
			case Axis.Y:
				result.axis = new Vector3(0f, 1f, 0f);
				break;
			case Axis.Z:
				result.axis = new Vector3(0f, 0f, 1f);
				break;
			}
			break;
		case 4:
			result.angle = ReadAngle(val4);
			result.axis = new Vector3(val1.sheet.ReadFloat(val1.handle), val1.sheet.ReadFloat(val2.handle), val1.sheet.ReadFloat(val3.handle));
			break;
		}
		return result;
	}

	private static bool TryReadEnum(StyleEnumType enumType, StylePropertyValue value, out int intValue)
	{
		string text = null;
		StyleValueHandle handle = value.handle;
		if (handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword svk = value.sheet.ReadKeyword(handle);
			text = svk.ToUssString();
		}
		else
		{
			text = value.sheet.ReadEnum(handle);
		}
		return StylePropertyUtil.TryGetEnumIntValue(enumType, text, out intValue);
	}

	private static int ReadEnum(StyleEnumType enumType, StylePropertyValue value)
	{
		string text = null;
		StyleValueHandle handle = value.handle;
		if (handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword svk = value.sheet.ReadKeyword(handle);
			text = svk.ToUssString();
		}
		else
		{
			text = value.sheet.ReadEnum(handle);
		}
		StylePropertyUtil.TryGetEnumIntValue(enumType, text, out var intValue);
		return intValue;
	}

	public static Angle ReadAngle(StylePropertyValue value)
	{
		if (value.handle.valueType == StyleValueType.Keyword)
		{
			StyleValueKeyword valueIndex = (StyleValueKeyword)value.handle.valueIndex;
			StyleValueKeyword styleValueKeyword = valueIndex;
			StyleValueKeyword styleValueKeyword2 = styleValueKeyword;
			if (styleValueKeyword2 == StyleValueKeyword.None)
			{
				return Angle.None();
			}
			return default(Angle);
		}
		return value.sheet.ReadDimension(value.handle).ToAngle();
	}

	public static BackgroundPosition ReadBackgroundPosition(int valCount, StylePropertyValue val1, StylePropertyValue val2, BackgroundPositionKeyword keyword)
	{
		switch (valCount)
		{
		case 1:
			if (val1.handle.valueType == StyleValueType.Enum)
			{
				return new BackgroundPosition((BackgroundPositionKeyword)ReadEnum(StyleEnumType.BackgroundPositionKeyword, val1));
			}
			if (val1.handle.valueType == StyleValueType.Dimension || val1.handle.valueType == StyleValueType.Float)
			{
				return new BackgroundPosition(keyword, val1.sheet.ReadDimension(val1.handle).ToLength());
			}
			break;
		case 2:
			if (val1.handle.valueType == StyleValueType.Enum && (val2.handle.valueType == StyleValueType.Dimension || val2.handle.valueType == StyleValueType.Float))
			{
				return new BackgroundPosition((BackgroundPositionKeyword)ReadEnum(StyleEnumType.BackgroundPositionKeyword, val1), val1.sheet.ReadDimension(val2.handle).ToLength());
			}
			break;
		}
		return default(BackgroundPosition);
	}

	public static BackgroundRepeat ReadBackgroundRepeat(int valCount, StylePropertyValue val1, StylePropertyValue val2)
	{
		BackgroundRepeat result = default(BackgroundRepeat);
		if (valCount == 1)
		{
			if (TryReadEnum(StyleEnumType.RepeatXY, val1, out var intValue))
			{
				switch (intValue)
				{
				case 0:
					result.x = Repeat.Repeat;
					result.y = Repeat.NoRepeat;
					break;
				case 1:
					result.x = Repeat.NoRepeat;
					result.y = Repeat.Repeat;
					break;
				}
			}
			else
			{
				result.x = (Repeat)ReadEnum(StyleEnumType.Repeat, val1);
				result.y = result.x;
			}
		}
		else
		{
			result.x = (Repeat)ReadEnum(StyleEnumType.Repeat, val1);
			result.y = (Repeat)ReadEnum(StyleEnumType.Repeat, val2);
		}
		return result;
	}

	public static BackgroundSize ReadBackgroundSize(int valCount, StylePropertyValue val1, StylePropertyValue val2)
	{
		BackgroundSize result = default(BackgroundSize);
		switch (valCount)
		{
		case 1:
			if (val1.handle.valueType == StyleValueType.Keyword)
			{
				if (val1.handle.valueIndex == 2)
				{
					result.x = Length.Auto();
					result.y = Length.Auto();
				}
				else if (val1.handle.valueIndex == 7)
				{
					result.sizeType = BackgroundSizeType.Cover;
				}
				else if (val1.handle.valueIndex == 8)
				{
					result.sizeType = BackgroundSizeType.Contain;
				}
			}
			else if (val1.handle.valueType == StyleValueType.Enum)
			{
				result.sizeType = (BackgroundSizeType)ReadEnum(StyleEnumType.BackgroundSizeType, val1);
			}
			else if (val1.handle.valueType == StyleValueType.Dimension)
			{
				result.x = val1.sheet.ReadDimension(val1.handle).ToLength();
				result.y = Length.Auto();
			}
			break;
		case 2:
			if (val1.handle.valueType == StyleValueType.Keyword)
			{
				if (val1.handle.valueIndex == 2)
				{
					result.x = Length.Auto();
				}
			}
			else if (val1.handle.valueType == StyleValueType.Dimension)
			{
				result.x = val1.sheet.ReadDimension(val1.handle).ToLength();
			}
			if (val2.handle.valueType == StyleValueType.Keyword)
			{
				if (val2.handle.valueIndex == 2)
				{
					result.y = Length.Auto();
				}
			}
			else if (val2.handle.valueType == StyleValueType.Dimension)
			{
				result.y = val2.sheet.ReadDimension(val2.handle).ToLength();
			}
			break;
		}
		return result;
	}

	public static TextShadow ReadTextShadow(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, StylePropertyValue val4)
	{
		if (valCount < 2)
		{
			return default(TextShadow);
		}
		StyleValueType valueType = val1.handle.valueType;
		bool flag = valueType == StyleValueType.Color || valueType == StyleValueType.Enum;
		TextShadow result = new TextShadow
		{
			color = Color.clear,
			offset = Vector2.zero,
			blurRadius = 0f
		};
		if (valCount < 4)
		{
			switch (valCount)
			{
			case 2:
			{
				Vector2 offset2 = result.offset;
				if (val1.sheet.TryReadDimension(val1.handle, out var value4))
				{
					offset2.x = value4.value;
				}
				if (val2.sheet.TryReadDimension(val2.handle, out var value5))
				{
					offset2.y = value5.value;
				}
				result.offset = offset2;
				break;
			}
			case 3:
			{
				StylePropertyValue stylePropertyValue = (flag ? val1 : val3);
				StylePropertyValue stylePropertyValue2 = (flag ? val2 : val1);
				StylePropertyValue stylePropertyValue3 = (flag ? val3 : val2);
				if (stylePropertyValue.sheet.TryReadColor(stylePropertyValue.handle, out var value))
				{
					result.color = value;
				}
				Vector2 offset = default(Vector2);
				if (stylePropertyValue2.sheet.TryReadDimension(stylePropertyValue2.handle, out var value2))
				{
					offset.x = value2.value;
				}
				if (stylePropertyValue3.sheet.TryReadDimension(stylePropertyValue3.handle, out var value3))
				{
					offset.y = value3.value;
				}
				result.offset = offset;
				break;
			}
			}
		}
		else
		{
			StylePropertyValue stylePropertyValue4 = (flag ? val1 : val4);
			StylePropertyValue stylePropertyValue5 = (flag ? val2 : val1);
			StylePropertyValue stylePropertyValue6 = (flag ? val3 : val2);
			StylePropertyValue stylePropertyValue7 = (flag ? val4 : val3);
			if (stylePropertyValue4.sheet.TryReadColor(stylePropertyValue4.handle, out var value6))
			{
				result.color = value6;
			}
			Vector2 offset3 = default(Vector2);
			if (stylePropertyValue5.sheet.TryReadDimension(stylePropertyValue5.handle, out var value7))
			{
				offset3.x = value7.value;
			}
			if (stylePropertyValue6.sheet.TryReadDimension(stylePropertyValue6.handle, out var value8))
			{
				offset3.y = value8.value;
			}
			result.offset = offset3;
			if (stylePropertyValue7.sheet.TryReadDimension(stylePropertyValue7.handle, out var value9))
			{
				result.blurRadius = value9.value;
			}
		}
		return result;
	}

	public static TextAutoSize ReadTextAutoSize(int valCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3)
	{
		switch (valCount)
		{
		case 0:
		case 1:
		case 2:
			return TextAutoSize.None();
		case 3:
		{
			if (val2.handle.valueType == StyleValueType.Keyword && val2.handle.valueIndex == 6)
			{
				return TextAutoSize.None();
			}
			if (val1.handle.valueType != StyleValueType.Enum)
			{
				break;
			}
			TextAutoSizeMode textAutoSizeMode = (TextAutoSizeMode)ReadEnum(StyleEnumType.TextAutoSizeMode, val1);
			switch (textAutoSizeMode)
			{
			case TextAutoSizeMode.None:
				return TextAutoSize.None();
			case TextAutoSizeMode.BestFit:
			{
				TextAutoSize result = new TextAutoSize
				{
					mode = textAutoSizeMode
				};
				if (val2.sheet.TryReadDimension(val2.handle, out var value))
				{
					result.minSize = value.ToLength();
				}
				if (val3.sheet.TryReadDimension(val3.handle, out var value2))
				{
					result.maxSize = value2.ToLength();
				}
				return result;
			}
			}
			break;
		}
		}
		return TextAutoSize.None();
	}

	internal static Cursor ReadCursor(int valueCount, StylePropertyValue val1, StylePropertyValue val2, StylePropertyValue val3, float dpiScaling = 1f)
	{
		Cursor result = default(Cursor);
		StyleValueType valueType = val1.handle.valueType;
		if (valueType == StyleValueType.ResourcePath || valueType == StyleValueType.AssetReference || valueType == StyleValueType.ScalableImage || valueType == StyleValueType.MissingAssetReference)
		{
			ImageSource source = default(ImageSource);
			if (TryGetImageSourceFromValue(val1, dpiScaling, out source))
			{
				result.texture = source.texture;
				if (valueCount >= 3)
				{
					if (val2.handle.valueType != StyleValueType.Float || val3.handle.valueType != StyleValueType.Float)
					{
						Debug.LogWarning("USS 'cursor' property requires two integers for the hot spot value.");
					}
					else
					{
						result.hotspot = new Vector2(val2.sheet.ReadFloat(val2.handle), val3.sheet.ReadFloat(val3.handle));
					}
				}
			}
		}
		else if (getCursorIdFunc != null)
		{
			result.defaultCursorId = getCursorIdFunc(val1.sheet, val1.handle);
		}
		return result;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static bool TryGetImageSourceFromValue(StylePropertyValue propertyValue, float dpiScaling, out ImageSource source)
	{
		source = default(ImageSource);
		switch (propertyValue.handle.valueType)
		{
		case StyleValueType.ResourcePath:
		{
			string text = propertyValue.sheet.ReadResourcePath(propertyValue.handle);
			if (!string.IsNullOrEmpty(text))
			{
				source.sprite = Panel.LoadResource(text, typeof(Sprite), dpiScaling) as Sprite;
				if (source.IsNull())
				{
					source.texture = Panel.LoadResource(text, typeof(Texture2D), dpiScaling) as Texture2D;
				}
				if (source.IsNull())
				{
					source.vectorImage = Panel.LoadResource(text, typeof(VectorImage), dpiScaling) as VectorImage;
				}
				if (source.IsNull())
				{
					source.renderTexture = Panel.LoadResource(text, typeof(RenderTexture), dpiScaling) as RenderTexture;
				}
			}
			if (source.IsNull())
			{
				Debug.LogWarning($"Image not found for path: {text}");
				return false;
			}
			break;
		}
		case StyleValueType.AssetReference:
		{
			Object obj = propertyValue.sheet.ReadAssetReference(propertyValue.handle);
			source.texture = obj as Texture2D;
			source.sprite = obj as Sprite;
			source.vectorImage = obj as VectorImage;
			source.renderTexture = obj as RenderTexture;
			if (source.IsNull())
			{
				Debug.LogWarning("Invalid image specified");
				return false;
			}
			break;
		}
		case StyleValueType.MissingAssetReference:
			return false;
		case StyleValueType.ScalableImage:
		{
			ScalableImage scalableImage = propertyValue.sheet.ReadScalableImage(propertyValue.handle);
			if (scalableImage.normalImage == null && scalableImage.highResolutionImage == null)
			{
				Debug.LogWarning("Invalid scalable image specified");
				return false;
			}
			source.texture = scalableImage.normalImage;
			if (!Mathf.Approximately(dpiScaling % 1f, 0f))
			{
				source.texture.filterMode = FilterMode.Bilinear;
			}
			break;
		}
		default:
			Debug.LogWarning("Invalid value for image texture " + propertyValue.handle.valueType);
			return false;
		}
		return true;
	}
}
