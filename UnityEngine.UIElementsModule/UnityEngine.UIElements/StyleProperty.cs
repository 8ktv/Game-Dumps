using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleProperty
{
	[SerializeField]
	private string m_Name;

	[SerializeField]
	private int m_Line;

	[SerializeField]
	private StyleValueHandle[] m_Values = Array.Empty<StyleValueHandle>();

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool isCustomProperty;

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool requireVariableResolve;

	public string name
	{
		get
		{
			return m_Name;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Name = value;
		}
	}

	public int line
	{
		get
		{
			return m_Line;
		}
		internal set
		{
			m_Line = value;
		}
	}

	public StyleValueHandle[] values
	{
		get
		{
			return m_Values;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			m_Values = value;
		}
	}

	internal int handleCount
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			StyleValueHandle[] array = m_Values;
			return (array != null) ? array.Length : 0;
		}
	}

	public bool ContainsVariable()
	{
		StyleValueHandle[] array = values;
		foreach (StyleValueHandle styleValueHandle in array)
		{
			if (styleValueHandle.IsVarFunction())
			{
				return true;
			}
		}
		return false;
	}

	public bool HasValue()
	{
		return handleCount != 0;
	}

	public void ClearValue()
	{
		m_Values = Array.Empty<StyleValueHandle>();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetKeyword(StyleSheet styleSheet, StyleValueKeyword value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteKeyword(ref m_Values[0], value);
	}

	public bool TryGetKeyword(StyleSheet styleSheet, out StyleValueKeyword value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadKeyword(m_Values[0], out value);
		}
		value = StyleValueKeyword.Inherit;
		return false;
	}

	public void SetFloat(StyleSheet styleSheet, float value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteFloat(ref m_Values[0], value);
	}

	public bool TryGetFloat(StyleSheet styleSheet, out float value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadFloat(m_Values[0], out value);
		}
		value = 0f;
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetDimension(StyleSheet styleSheet, Dimension value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteDimension(ref m_Values[0], value);
	}

	public bool TryGetDimension(StyleSheet styleSheet, out Dimension value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadDimension(m_Values[0], out value);
		}
		value = default(Dimension);
		return false;
	}

	public void SetColor(StyleSheet styleSheet, Color value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteColor(ref m_Values[0], value);
	}

	public bool TryGetColor(StyleSheet styleSheet, out Color value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadColor(m_Values[0], out value);
		}
		value = default(Color);
		return false;
	}

	public void SetString(StyleSheet styleSheet, string value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteString(ref values[0], value);
	}

	public bool TryGetString(StyleSheet styleSheet, out string value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadString(m_Values[0], out value);
		}
		value = null;
		return false;
	}

	public void SetEnum(StyleSheet styleSheet, Enum value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteEnum(ref m_Values[0], value);
	}

	public void SetEnum<TEnum>(StyleSheet styleSheet, TEnum value) where TEnum : struct, Enum
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteEnum(ref m_Values[0], value);
	}

	public bool TryGetEnumString(StyleSheet styleSheet, out string value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadEnum(m_Values[0], out value);
		}
		value = null;
		return false;
	}

	public bool TryGetEnum<TEnum>(StyleSheet styleSheet, out TEnum value) where TEnum : struct, Enum
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadEnum(m_Values[0], out value);
		}
		value = default(TEnum);
		return false;
	}

	public void SetVariableReference(StyleSheet styleSheet, string variableName)
	{
		SetSize(ref m_Values, 3);
		styleSheet.WriteFunction(ref m_Values[0], StyleValueFunction.Var);
		styleSheet.WriteFloat(ref m_Values[1], 1f);
		styleSheet.WriteVariable(ref m_Values[2], variableName);
	}

	public bool TryGetVariableReference(StyleSheet styleSheet, out string variableName)
	{
		if (handleCount == 3 && styleSheet.TryReadFunction(m_Values[0], out var value) && value == StyleValueFunction.Var && styleSheet.TryReadFloat(m_Values[1], out var value2) && (int)value2 == 1)
		{
			return styleSheet.TryReadVariable(m_Values[2], out variableName);
		}
		variableName = null;
		return false;
	}

	public void SetResourcePath(StyleSheet styleSheet, string value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteResourcePath(ref m_Values[0], value);
	}

	public bool TryGetResourcePath(StyleSheet styleSheet, out string value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadResourcePath(m_Values[0], out value);
		}
		value = null;
		return false;
	}

	public void SetAssetReference(StyleSheet styleSheet, Object value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteAssetReference(ref m_Values[0], value);
	}

	public bool TryGetAssetReference(StyleSheet styleSheet, out Object value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadAssetReference(m_Values[0], out value);
		}
		value = null;
		return false;
	}

	public bool TryGetAssetReference<TObject>(StyleSheet styleSheet, out TObject value) where TObject : Object
	{
		if (TryGetAssetReference(styleSheet, out var value2) && value2 is TObject val)
		{
			value = val;
			return true;
		}
		value = null;
		return false;
	}

	public void SetMissingAssetReferenceUrl(StyleSheet styleSheet, string value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteMissingAssetReferenceUrl(ref m_Values[0], value);
	}

	public bool TryGetMissingAssetReferenceUrl(StyleSheet styleSheet, out string value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadMissingAssetReferenceUrl(m_Values[0], out value);
		}
		value = null;
		return false;
	}

	public void SetScalableImage(StyleSheet styleSheet, ScalableImage value)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteScalableImage(ref m_Values[0], value);
	}

	public bool TryGetScalableImage(StyleSheet styleSheet, out ScalableImage value)
	{
		if (handleCount == 1)
		{
			return styleSheet.TryReadScalableImage(m_Values[0], out value);
		}
		value = default(ScalableImage);
		return false;
	}

	public void SetKeyword(StyleSheet styleSheet, StyleKeyword value)
	{
		SetKeyword(styleSheet, value.ToStyleValueKeyword());
	}

	public bool TryGetKeyword(StyleSheet styleSheet, out StyleKeyword value)
	{
		if (handleCount == 1)
		{
			return TryReadSetKeyword(styleSheet, ref m_Values[0], out value);
		}
		value = StyleKeyword.Undefined;
		return false;
	}

	public void SetBackgroundRepeat(StyleSheet styleSheet, BackgroundRepeat value)
	{
		SetSize(ref m_Values, 2);
		styleSheet.WriteEnum(ref values[0], value.x);
		styleSheet.WriteEnum(ref values[1], value.y);
	}

	public bool TryGetBackgroundRepeat(StyleSheet styleSheet, out BackgroundRepeat value)
	{
		int num = handleCount;
		if (num <= 0 || num > 2)
		{
			value = default(BackgroundRepeat);
			return false;
		}
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((handleCount > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadBackgroundRepeat(handleCount, val, val2);
		return true;
	}

	public void SetBackgroundSize(StyleSheet styleSheet, BackgroundSize value)
	{
		switch (value.sizeType)
		{
		case BackgroundSizeType.Length:
			SetSize(ref m_Values, 2);
			styleSheet.WriteLength(ref values[0], value.x);
			styleSheet.WriteLength(ref values[1], value.y);
			break;
		case BackgroundSizeType.Cover:
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref values[0], StyleValueKeyword.Cover);
			break;
		case BackgroundSizeType.Contain:
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref values[0], StyleValueKeyword.Contain);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool TryGetBackgroundSize(StyleSheet styleSheet, out BackgroundSize value)
	{
		int num = handleCount;
		if (num <= 0 || num > 2)
		{
			value = default(BackgroundSize);
			return false;
		}
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((handleCount > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadBackgroundSize(handleCount, val, val2);
		return true;
	}

	public void SetBackgroundPosition(StyleSheet styleSheet, BackgroundPosition value)
	{
		if (value.keyword == BackgroundPositionKeyword.Center)
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteEnum(ref values[0], value.keyword);
		}
		else
		{
			SetSize(ref m_Values, 2);
			styleSheet.WriteEnum(ref values[0], value.keyword);
			styleSheet.WriteDimension(ref values[1], value.offset.ToDimension());
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryGetBackgroundPosition(StyleSheet styleSheet, out BackgroundPosition value, BackgroundPosition.Axis axis)
	{
		int num = handleCount;
		if (num <= 0 || num > 2)
		{
			value = default(BackgroundPosition);
			return false;
		}
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((handleCount > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadBackgroundPosition(handleCount, val, val2, (axis != BackgroundPosition.Axis.Horizontal) ? BackgroundPositionKeyword.Top : BackgroundPositionKeyword.Left);
		return true;
	}

	public void SetInt(StyleSheet styleSheet, int value)
	{
		SetFloat(styleSheet, value);
	}

	public bool TryGetInt(StyleSheet styleSheet, out int value)
	{
		if (TryGetFloat(styleSheet, out var value2))
		{
			value = (int)value2;
			return true;
		}
		value = 0;
		return false;
	}

	public void SetLength(StyleSheet styleSheet, Length value)
	{
		if (value.IsAuto())
		{
			SetKeyword(styleSheet, StyleValueKeyword.Auto);
		}
		else if (value.IsNone())
		{
			SetKeyword(styleSheet, StyleValueKeyword.None);
		}
		else
		{
			SetDimension(styleSheet, value.ToDimension());
		}
	}

	public bool TryGetLength(StyleSheet styleSheet, out Length value)
	{
		if (handleCount != 1)
		{
			value = default(Length);
			return false;
		}
		if (styleSheet.TryReadKeyword(m_Values[0], out var value2))
		{
			switch (value2)
			{
			case StyleValueKeyword.Initial:
				value = default(Length);
				return true;
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
		if (styleSheet.TryReadDimension(m_Values[0], out var value3) && value3.IsLength())
		{
			value = value3.ToLength();
			return true;
		}
		value = default(Length);
		return false;
	}

	public void SetTranslate(StyleSheet styleSheet, Translate value)
	{
		if (value.IsNone())
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref m_Values[0], StyleValueKeyword.None);
		}
		else if (value.z == 0f)
		{
			SetSize(ref m_Values, 2);
			styleSheet.WriteDimension(ref m_Values[0], value.x.ToDimension());
			styleSheet.WriteDimension(ref m_Values[1], value.y.ToDimension());
		}
		else
		{
			SetSize(ref m_Values, 3);
			styleSheet.WriteDimension(ref m_Values[0], value.x.ToDimension());
			styleSheet.WriteDimension(ref m_Values[1], value.y.ToDimension());
			styleSheet.WriteDimension(ref m_Values[2], new Length(value.z).ToDimension());
		}
	}

	public bool TryGetTranslate(StyleSheet styleSheet, out Translate value)
	{
		int num = handleCount;
		if (num <= 0 || num > 3)
		{
			value = default(Translate);
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val3 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadTranslate(num2, val, val2, val3);
		return true;
	}

	public void SetRotate(StyleSheet styleSheet, Rotate value)
	{
		if (value.IsNone())
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref values[0], StyleValueKeyword.None);
			return;
		}
		if (value.axis == Vector3.forward)
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteAngle(ref values[0], value.angle);
			return;
		}
		SetSize(ref m_Values, 4);
		Vector3 axis = value.axis;
		styleSheet.WriteFloat(ref values[0], axis.x);
		styleSheet.WriteFloat(ref values[1], axis.y);
		styleSheet.WriteFloat(ref values[2], axis.z);
		styleSheet.WriteAngle(ref values[3], value.angle);
	}

	public bool TryGetRotate(StyleSheet styleSheet, out Rotate value)
	{
		int num = handleCount;
		if (num <= 0 || num > 4)
		{
			value = default(Rotate);
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val3 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val4 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[3],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadRotate(num2, val, val2, val3, val4);
		return true;
	}

	public void SetScale(StyleSheet styleSheet, Scale value)
	{
		if (value.IsNone())
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref values[0], StyleValueKeyword.None);
		}
		else if (Mathf.Approximately(value.value.z, 1f))
		{
			SetSize(ref m_Values, 2);
			styleSheet.WriteFloat(ref values[0], value.value.x);
			styleSheet.WriteFloat(ref values[1], value.value.y);
		}
		else
		{
			SetSize(ref m_Values, 3);
			styleSheet.WriteFloat(ref values[0], value.value.x);
			styleSheet.WriteFloat(ref values[1], value.value.y);
			styleSheet.WriteFloat(ref values[2], value.value.z);
		}
	}

	public bool TryGetScale(StyleSheet styleSheet, out Scale value)
	{
		int num = handleCount;
		if (num <= 0 || num > 3)
		{
			value = default(Scale);
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val3 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadScale(num2, val, val2, val3);
		return true;
	}

	public void SetTextShadow(StyleSheet styleSheet, TextShadow value)
	{
		SetSize(ref m_Values, 4);
		styleSheet.WriteDimension(ref values[0], new Dimension
		{
			value = value.offset.x,
			unit = Dimension.Unit.Pixel
		});
		styleSheet.WriteDimension(ref values[1], new Dimension
		{
			value = value.offset.y,
			unit = Dimension.Unit.Pixel
		});
		styleSheet.WriteDimension(ref values[2], new Dimension
		{
			value = value.blurRadius,
			unit = Dimension.Unit.Pixel
		});
		styleSheet.WriteColor(ref values[3], value.color);
	}

	public bool TryGetTextShadow(StyleSheet styleSheet, out TextShadow value)
	{
		int num = handleCount;
		if (num <= 0 || num > 4)
		{
			value = default(TextShadow);
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val3 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val4 = ((num2 > 3) ? new StylePropertyValue
		{
			handle = values[3],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadTextShadow(num2, val, val2, val3, val4);
		return true;
	}

	public void SetTextAutoSize(StyleSheet styleSheet, TextAutoSize value)
	{
		if (value.mode == TextAutoSizeMode.None)
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteEnum(ref m_Values[0], value.mode);
			return;
		}
		SetSize(ref m_Values, 3);
		styleSheet.WriteEnum(ref m_Values[0], value.mode);
		styleSheet.WriteDimension(ref values[1], new Dimension
		{
			value = value.minSize.value,
			unit = Dimension.Unit.Pixel
		});
		styleSheet.WriteDimension(ref values[2], new Dimension
		{
			value = value.maxSize.value,
			unit = Dimension.Unit.Pixel
		});
	}

	public bool TryGetTextAutoSize(StyleSheet styleSheet, out TextAutoSize value)
	{
		int num = handleCount;
		if (num <= 0 || num > 3)
		{
			value = TextAutoSize.None();
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue val3 = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadTextAutoSize(num2, val, val2, val3);
		return true;
	}

	public void SetTransformOrigin(StyleSheet styleSheet, TransformOrigin value)
	{
		TransformOriginOffset? transformOriginOffset = GetTransformOriginOffset(value.x, horizontal: true);
		TransformOriginOffset? transformOriginOffset2 = GetTransformOriginOffset(value.y, horizontal: false);
		bool flag = value.z != 0f;
		if (!flag)
		{
			if (transformOriginOffset2.HasValue && transformOriginOffset2 == TransformOriginOffset.Center)
			{
				SetSize(ref m_Values, 1);
				if (transformOriginOffset.HasValue)
				{
					styleSheet.WriteEnum(ref m_Values[0], transformOriginOffset.Value);
				}
				else
				{
					styleSheet.WriteDimension(ref m_Values[0], value.x.ToDimension());
				}
				return;
			}
			if (transformOriginOffset.HasValue && transformOriginOffset2.HasValue && transformOriginOffset.Value == TransformOriginOffset.Center)
			{
				SetSize(ref m_Values, 1);
				styleSheet.WriteEnum(ref m_Values[0], transformOriginOffset2.Value);
				return;
			}
		}
		SetSize(ref m_Values, 2 + (flag ? 1 : 0));
		if (transformOriginOffset.HasValue)
		{
			styleSheet.WriteEnum(ref m_Values[0], transformOriginOffset.Value);
		}
		else
		{
			styleSheet.WriteDimension(ref m_Values[0], value.x.ToDimension());
		}
		if (transformOriginOffset2.HasValue)
		{
			styleSheet.WriteEnum(ref m_Values[1], transformOriginOffset2.Value);
		}
		else
		{
			styleSheet.WriteDimension(ref m_Values[1], value.y.ToDimension());
		}
		if (flag)
		{
			styleSheet.WriteDimension(ref m_Values[2], new Dimension(value.z, Dimension.Unit.Pixel));
		}
	}

	public bool TryGetTransformOrigin(StyleSheet styleSheet, out TransformOrigin value)
	{
		int num = handleCount;
		if (num <= 0 || num > 3)
		{
			value = default(TransformOrigin);
			return false;
		}
		int num2 = handleCount;
		StylePropertyValue val = new StylePropertyValue
		{
			handle = values[0],
			sheet = styleSheet
		};
		StylePropertyValue val2 = ((num2 > 1) ? new StylePropertyValue
		{
			handle = values[1],
			sheet = styleSheet
		} : default(StylePropertyValue));
		StylePropertyValue zVvalue = ((num2 > 2) ? new StylePropertyValue
		{
			handle = values[2],
			sheet = styleSheet
		} : default(StylePropertyValue));
		value = StylePropertyReader.ReadTransformOrigin(num2, val, val2, zVvalue);
		return true;
	}

	public void SetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
	{
		SetSize(ref m_Values, value.Count * 2 - 1);
		for (int i = 0; i < value.Count; i++)
		{
			int num = i * 2;
			styleSheet.WriteDimension(ref values[num], value[i].ToDimension());
			if (i < value.Count - 1)
			{
				styleSheet.WriteCommaSeparator(ref values[num + 1]);
			}
		}
	}

	public bool TryGetTimeValue(StyleSheet styleSheet, out List<TimeValue> value)
	{
		if (ContainsVariable())
		{
			value = null;
			return false;
		}
		value = new List<TimeValue>();
		return TryGetTimeValue(styleSheet, value);
	}

	public bool TryGetTimeValue(StyleSheet styleSheet, List<TimeValue> value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		value.Clear();
		if (ContainsVariable())
		{
			return false;
		}
		for (int i = 0; i < m_Values.Length; i += 2)
		{
			int num = i + 1;
			if (!styleSheet.TryReadTimeValue(m_Values[i], out var value2) || (num < m_Values.Length && values[num].valueType != StyleValueType.CommaSeparator))
			{
				value.Clear();
				return false;
			}
			value.Add(value2);
		}
		return true;
	}

	public void SetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
	{
		SetSize(ref m_Values, value.Count * 2 - 1);
		for (int i = 0; i < value.Count; i++)
		{
			int num = i * 2;
			styleSheet.WriteStylePropertyName(ref values[num], value[i]);
			if (i < value.Count - 1)
			{
				styleSheet.WriteCommaSeparator(ref values[num + 1]);
			}
		}
	}

	public bool TryGetStylePropertyName(StyleSheet styleSheet, out List<StylePropertyName> value)
	{
		if (ContainsVariable())
		{
			value = null;
			return false;
		}
		value = new List<StylePropertyName>();
		return TryGetStylePropertyName(styleSheet, value);
	}

	public bool TryGetStylePropertyName(StyleSheet styleSheet, List<StylePropertyName> value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		value.Clear();
		if (ContainsVariable())
		{
			return false;
		}
		for (int i = 0; i < m_Values.Length; i += 2)
		{
			int num = i + 1;
			if (!styleSheet.TryReadStylePropertyName(m_Values[i], out var value2) || (num < m_Values.Length && values[num].valueType != StyleValueType.CommaSeparator))
			{
				value.Clear();
				return false;
			}
			value.Add(value2);
		}
		return true;
	}

	public void SetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
	{
		SetSize(ref m_Values, value.Count * 2 - 1);
		for (int i = 0; i < value.Count; i++)
		{
			int num = i * 2;
			styleSheet.WriteEnum(ref values[num], value[i].mode);
			if (i < value.Count - 1)
			{
				styleSheet.WriteCommaSeparator(ref values[num + 1]);
			}
		}
	}

	public bool TryGetEasingFunction(StyleSheet styleSheet, out List<EasingFunction> value)
	{
		if (ContainsVariable())
		{
			value = null;
			return false;
		}
		value = new List<EasingFunction>();
		return TryGetEasingFunction(styleSheet, value);
	}

	public bool TryGetEasingFunction(StyleSheet styleSheet, List<EasingFunction> value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		value.Clear();
		if (ContainsVariable())
		{
			return false;
		}
		for (int i = 0; i < m_Values.Length; i += 2)
		{
			int num = i + 1;
			if (!styleSheet.TryReadEnum(m_Values[i], out EasingMode value2) || (num < m_Values.Length && values[num].valueType != StyleValueType.CommaSeparator))
			{
				value.Clear();
				return false;
			}
			value.Add(new EasingFunction(value2));
		}
		return true;
	}

	private static void SetSize(ref StyleValueHandle[] store, int size)
	{
		StyleValueHandle[] obj = store;
		if (obj == null || obj.Length != size)
		{
			store = new StyleValueHandle[size];
		}
	}

	internal static bool TryReadSetKeyword(StyleSheet styleSheet, ref StyleValueHandle handle, out StyleKeyword value)
	{
		if (handle.valueType == StyleValueType.Keyword)
		{
			switch ((StyleValueKeyword)handle.valueIndex)
			{
			case StyleValueKeyword.Initial:
				value = StyleKeyword.Initial;
				return true;
			case StyleValueKeyword.Auto:
				value = StyleKeyword.Auto;
				return true;
			case StyleValueKeyword.None:
				value = StyleKeyword.None;
				return true;
			}
		}
		value = StyleKeyword.Undefined;
		return false;
	}

	private static TransformOriginOffset? GetTransformOriginOffset(Length dim, bool horizontal)
	{
		TransformOriginOffset? result = null;
		if (Mathf.Approximately(dim.value, 0f))
		{
			result = (horizontal ? TransformOriginOffset.Left : TransformOriginOffset.Top);
		}
		else if (dim.unit == LengthUnit.Percent)
		{
			if (Mathf.Approximately(dim.value, 50f))
			{
				result = TransformOriginOffset.Center;
			}
			else if (Mathf.Approximately(dim.value, 100f))
			{
				result = (horizontal ? TransformOriginOffset.Right : TransformOriginOffset.Bottom);
			}
		}
		return result;
	}
}
