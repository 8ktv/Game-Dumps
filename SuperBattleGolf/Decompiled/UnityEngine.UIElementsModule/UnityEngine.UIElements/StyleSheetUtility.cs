#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class StyleSheetUtility
{
	private static readonly Dictionary<string, string> SpecialEnumToStringCases = new Dictionary<string, string> { { "no-wrap", "nowrap" } };

	private static readonly Dictionary<string, string> SpecialStringToEnumCases = new Dictionary<string, string>
	{
		{ "nowrap", "NoWrap" },
		{ "sdf", "SDF" },
		{ "uv", "UV" }
	};

	public static StyleSheet CreateInstanceWithHideFlags()
	{
		StyleSheet styleSheet = ScriptableObject.CreateInstance<StyleSheet>();
		styleSheet.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset;
		return styleSheet;
	}

	public static Dimension ToDimension(this Length length)
	{
		if (length.IsAuto() || length.IsNone())
		{
			throw new InvalidCastException($"Can't convert a Length to a Dimension because it contains the '{length}' keyword.");
		}
		return new Dimension(length.value, length.unit.ToDimensionUnit());
	}

	public static Dimension.Unit ToDimensionUnit(this LengthUnit unit)
	{
		if (1 == 0)
		{
		}
		Dimension.Unit result = unit switch
		{
			LengthUnit.Pixel => Dimension.Unit.Pixel, 
			LengthUnit.Percent => Dimension.Unit.Percent, 
			_ => throw new InvalidCastException($"Can't convert a LengthUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'px' or '%', but was {unit}"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static Dimension ToDimension(this Angle angle)
	{
		if (angle.IsNone())
		{
			throw new InvalidCastException($"Can't convert a Rotate to a Dimension because it contains the '{angle}' keyword.");
		}
		return new Dimension(angle.value, angle.unit.ToDimensionUnit());
	}

	public static Dimension.Unit ToDimensionUnit(this AngleUnit unit)
	{
		if (1 == 0)
		{
		}
		Dimension.Unit result = unit switch
		{
			AngleUnit.Degree => Dimension.Unit.Degree, 
			AngleUnit.Gradian => Dimension.Unit.Gradian, 
			AngleUnit.Radian => Dimension.Unit.Radian, 
			AngleUnit.Turn => Dimension.Unit.Turn, 
			_ => throw new InvalidCastException($"Can't convert a AngleUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 'deg', 'grad', 'rad' or 'turn', but was {unit}"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static Dimension ToDimension(this TimeValue timeValue)
	{
		return new Dimension(timeValue.value, timeValue.unit.ToDimensionUnit());
	}

	public static Dimension.Unit ToDimensionUnit(this TimeUnit unit)
	{
		if (1 == 0)
		{
		}
		Dimension.Unit result = unit switch
		{
			TimeUnit.Second => Dimension.Unit.Second, 
			TimeUnit.Millisecond => Dimension.Unit.Millisecond, 
			_ => throw new InvalidCastException($"Can't convert a TimeUnit to a Dimension.Unit because it does not contain a valid keyword. Expected 's' or 'ms', but was {unit}"), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static StyleValueKeyword ToStyleValueKeyword(this StyleKeyword keyword)
	{
		if (1 == 0)
		{
		}
		StyleValueKeyword result = keyword switch
		{
			StyleKeyword.Auto => StyleValueKeyword.Auto, 
			StyleKeyword.None => StyleValueKeyword.None, 
			StyleKeyword.Initial => StyleValueKeyword.Initial, 
			_ => throw new InvalidCastException($"Can't convert a StyleKeyword to a StyleValueKeyword because it does not contain a valid keyword. Expected 'auto', 'none' or 'initial', but was {keyword}."), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static void TransferStylePropertyHandles(StyleSheet fromStyleSheet, StyleProperty fromStyleProperty, StyleSheet toStyleSheet, StyleProperty toStyleProperty)
	{
		Assert.IsNotNull(fromStyleSheet);
		Assert.IsNotNull(toStyleSheet);
		Assert.IsNotNull(fromStyleProperty);
		Assert.IsNotNull(toStyleProperty);
		Assert.IsFalse(fromStyleProperty == toStyleProperty, "Cannot transfer a StyleProperty unto itself.");
		List<StyleValueHandle> value;
		using (CollectionPool<List<StyleValueHandle>, StyleValueHandle>.Get(out value))
		{
			value.AddRange(toStyleProperty.values);
			StyleValueHandle[] values = fromStyleProperty.values;
			for (int i = 0; i < values.Length; i++)
			{
				StyleValueHandle handle = values[i];
				StyleValueType valueType = handle.valueType;
				if (1 == 0)
				{
				}
				int num = valueType switch
				{
					StyleValueType.Float => toStyleSheet.AddValue(fromStyleSheet.ReadFloat(handle)), 
					StyleValueType.Dimension => toStyleSheet.AddValue(fromStyleSheet.ReadDimension(handle)), 
					StyleValueType.Enum => toStyleSheet.AddValue(fromStyleSheet.ReadEnum(handle)), 
					StyleValueType.String => toStyleSheet.AddValue(fromStyleSheet.ReadString(handle)), 
					StyleValueType.Color => toStyleSheet.AddValue(fromStyleSheet.ReadColor(handle)), 
					StyleValueType.AssetReference => toStyleSheet.AddValue(fromStyleSheet.ReadAssetReference(handle)), 
					StyleValueType.ResourcePath => toStyleSheet.AddValue(fromStyleSheet.ReadResourcePath(handle)), 
					StyleValueType.Variable => toStyleSheet.AddValue(fromStyleSheet.ReadVariable(handle)), 
					StyleValueType.Keyword => toStyleSheet.AddValue(fromStyleSheet.ReadKeyword(handle)), 
					StyleValueType.CommaSeparator => handle.valueIndex, 
					StyleValueType.Function => toStyleSheet.AddValue(fromStyleSheet.ReadFunction(handle)), 
					StyleValueType.ScalableImage => toStyleSheet.AddValue(fromStyleSheet.ReadScalableImage(handle)), 
					StyleValueType.MissingAssetReference => toStyleSheet.AddValue(fromStyleSheet.ReadMissingAssetReferenceUrl(handle)), 
					StyleValueType.Invalid => handle.valueIndex, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				if (1 == 0)
				{
				}
				int valueIndex = num;
				value.Add(new StyleValueHandle(valueIndex, valueType));
			}
			toStyleProperty.requireVariableResolve |= fromStyleProperty.requireVariableResolve;
			toStyleProperty.values = value.ToArray();
		}
	}

	public static string GetEnumExportString(Enum value)
	{
		return ConvertCamelToDash(value.ToString());
	}

	public static string ConvertCamelToDash(string camel)
	{
		string text = Regex.Replace(Regex.Replace(camel, "(\\P{Ll})(\\P{Ll}\\p{Ll})", "$1-$2"), "(\\p{Ll})(\\P{Ll})", "$1-$2");
		string text2 = text.ToLowerInvariant();
		return SpecialEnumToStringCases.GetValueOrDefault(text2, text2);
	}

	public static string ConvertDashToHungarian(string dash)
	{
		return ConvertDashToUpperNoSpace(dash, firstCase: true, addSpace: false);
	}

	public static string ConvertDashToCamel(string dash)
	{
		return ConvertDashToUpperNoSpace(dash, firstCase: false, addSpace: false);
	}

	public static string ConvertDashToHuman(string dash)
	{
		return ConvertDashToUpperNoSpace(dash, firstCase: true, addSpace: true);
	}

	public static string ConvertDashToUpperNoSpace(string dash, bool firstCase, bool addSpace)
	{
		if (SpecialStringToEnumCases.TryGetValue(dash, out var value))
		{
			return value;
		}
		StringBuilder stringBuilder = GenericPool<StringBuilder>.Get();
		try
		{
			bool flag = firstCase;
			foreach (char c in dash)
			{
				if (c == '-')
				{
					if (addSpace)
					{
						stringBuilder.Append(' ');
					}
					flag = true;
				}
				else if (flag)
				{
					stringBuilder.Append(char.ToUpper(c, CultureInfo.InvariantCulture));
					flag = false;
				}
				else
				{
					stringBuilder.Append(char.ToLowerInvariant(c));
				}
			}
			return stringBuilder.ToString();
		}
		finally
		{
			GenericPool<StringBuilder>.Release(stringBuilder.Clear());
		}
	}

	public static string GetDimensionUnitExportString(Dimension.Unit unit)
	{
		if (1 == 0)
		{
		}
		string result = unit switch
		{
			Dimension.Unit.Pixel => "px", 
			Dimension.Unit.Percent => "%", 
			Dimension.Unit.Second => "s", 
			Dimension.Unit.Millisecond => "ms", 
			Dimension.Unit.Degree => "deg", 
			Dimension.Unit.Gradian => "grad", 
			Dimension.Unit.Radian => "rad", 
			Dimension.Unit.Turn => "turn", 
			Dimension.Unit.Unitless => string.Empty, 
			_ => throw new ArgumentOutOfRangeException("unit", unit, null), 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static void GetValueOffsets(StyleSheet styleSheet, Span<StyleValueHandle> handles, List<int> offsets)
	{
		offsets.Clear();
		if (handles.Length == 0)
		{
			return;
		}
		offsets.Add(0);
		int num = 0;
		while (true)
		{
			num = GetNextValueOffset(styleSheet, handles, num);
			if (num >= 0 && num < handles.Length)
			{
				offsets.Add(num);
				continue;
			}
			break;
		}
	}

	internal static int GetNextValueOffset(StyleSheet styleSheet, Span<StyleValueHandle> handles, int index)
	{
		if (index < 0 || index >= handles.Length)
		{
			return -1;
		}
		int num = index;
		StyleValueHandle styleValueHandle = handles[index];
		switch (styleValueHandle.valueType)
		{
		case StyleValueType.Keyword:
		case StyleValueType.Float:
		case StyleValueType.Dimension:
		case StyleValueType.Color:
		case StyleValueType.ResourcePath:
		case StyleValueType.AssetReference:
		case StyleValueType.Enum:
		case StyleValueType.Variable:
		case StyleValueType.String:
		case StyleValueType.ScalableImage:
		case StyleValueType.MissingAssetReference:
			return num + 1 + OffsetByComma(handles, num + 1);
		case StyleValueType.Function:
		{
			StyleValueHandle handle = handles[++index];
			float num2 = styleSheet.ReadFloat(handle);
			int num3 = ++index;
			for (int i = num3; (float)i < (float)num3 + num2; i++)
			{
				if (handles[i].valueType == StyleValueType.Function)
				{
					int nextValueOffset = GetNextValueOffset(styleSheet, handles, i);
					if (nextValueOffset <= 0)
					{
						return -1;
					}
					index = nextValueOffset;
				}
				else
				{
					index++;
				}
			}
			return index + OffsetByComma(handles, index);
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
		static int OffsetByComma(Span<StyleValueHandle> span, int next)
		{
			return (next < span.Length && span[next].valueType == StyleValueType.CommaSeparator) ? 1 : 0;
		}
	}
}
