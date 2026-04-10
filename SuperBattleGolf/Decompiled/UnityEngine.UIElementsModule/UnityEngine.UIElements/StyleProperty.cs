using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.Layout;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class StyleProperty
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal readonly struct Manipulator
	{
		private readonly ref struct ValueSpan
		{
			public readonly int start;

			public readonly int length;

			public ValueSpan(int start, int length)
			{
				this.start = start;
				this.length = length;
			}
		}

		private readonly StyleSheet m_StyleSheet;

		private readonly StyleProperty m_Property;

		internal Manipulator(StyleSheet styleSheet, StyleProperty property)
		{
			m_StyleSheet = styleSheet;
			m_Property = property;
		}

		public int GetValueCount()
		{
			List<int> value;
			using (CollectionPool<List<int>, int>.Get(out value))
			{
				StyleSheetUtility.GetValueOffsets(m_StyleSheet, m_Property.values, value);
				return value.Count;
			}
		}

		public void AddKeyword(StyleValueKeyword value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteKeyword(ref handle, value);
			AddHandle(handle);
		}

		public void SetKeyword(int index, StyleValueKeyword value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteKeyword(ref m_Property.values[span.start], value);
		}

		public void InsertKeyword(int index, StyleValueKeyword value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteKeyword(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetKeyword(int index, out StyleValueKeyword value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = StyleValueKeyword.Inherit;
				return false;
			}
			return m_StyleSheet.TryReadKeyword(m_Property.values[valueSpan.start], out value);
		}

		public void AddFloat(float value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteFloat(ref handle, value);
			AddHandle(handle);
		}

		public void SetFloat(int index, float value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteFloat(ref m_Property.values[span.start], value);
		}

		public void InsertFloat(int index, float value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteFloat(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetFloat(int index, out float value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = 0f;
				return false;
			}
			return m_StyleSheet.TryReadFloat(m_Property.values[valueSpan.start], out value);
		}

		public void AddDimension(Dimension value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteDimension(ref handle, value);
			AddHandle(handle);
		}

		public void SetDimension(int index, Dimension value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteDimension(ref m_Property.values[span.start], value);
		}

		public void InsertDimension(int index, Dimension value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteDimension(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetDimension(int index, out Dimension value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(Dimension);
				return false;
			}
			return m_StyleSheet.TryReadDimension(m_Property.values[valueSpan.start], out value);
		}

		public void AddColor(Color value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteColor(ref handle, value);
			AddHandle(handle);
		}

		public void SetColor(int index, Color value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteColor(ref m_Property.values[span.start], value);
		}

		public void InsertColor(int index, Color value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteColor(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetColor(int index, out Color value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(Color);
				return false;
			}
			return m_StyleSheet.TryReadColor(m_Property.values[valueSpan.start], out value);
		}

		public void AddString(string value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteString(ref handle, value);
			AddHandle(handle);
		}

		public void SetString(int index, string value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteString(ref m_Property.values[span.start], value);
		}

		public void InsertString(int index, string value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteString(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetString(int index, out string value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadString(m_Property.values[valueSpan.start], out value);
		}

		public void AddEnum(Enum value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteEnum(ref handle, value);
			AddHandle(handle);
		}

		public void AddEnum<TEnum>(TEnum value) where TEnum : struct, Enum
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteEnum(ref handle, value);
			AddHandle(handle);
		}

		public void AddEnum(string value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteEnumAsString(ref handle, value);
			AddHandle(handle);
		}

		public void SetEnum(int index, Enum value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[span.start], value);
		}

		public void SetEnum<TEnum>(int index, TEnum value) where TEnum : struct, Enum
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[span.start], value);
		}

		public void SetEnum(int index, string value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteEnumAsString(ref m_Property.values[span.start], value);
		}

		public void InsertEnum(int index, Enum value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[valueSpan.start], value);
		}

		public void InsertEnum<TEnum>(int index, TEnum value) where TEnum : struct, Enum
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[valueSpan.start], value);
		}

		public void InsertEnum(int index, string value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteEnumAsString(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetEnum<TEnum>(int index, out TEnum value) where TEnum : struct, Enum
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(TEnum);
				return false;
			}
			return m_StyleSheet.TryReadEnum(m_Property.values[valueSpan.start], out value);
		}

		public bool TryGetEnum(int index, out string value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadEnum(m_Property.values[valueSpan.start], out value);
		}

		public void AddVariableReference(string value)
		{
			int num = m_Property.values.Length;
			Insert(m_Property, m_Property.values.Length, 3);
			m_StyleSheet.WriteFunction(ref m_Property.values[num], StyleValueFunction.Var);
			m_StyleSheet.WriteFloat(ref m_Property.values[num + 1], 1f);
			m_StyleSheet.WriteVariable(ref m_Property.values[num + 2], value);
			m_Property.requireVariableResolve = true;
		}

		public void SetVariableReference(int index, string value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 3);
			m_StyleSheet.WriteFunction(ref m_Property.values[span.start], StyleValueFunction.Var);
			m_StyleSheet.WriteFloat(ref m_Property.values[span.start + 1], 1f);
			m_StyleSheet.WriteVariable(ref m_Property.values[span.start + 2], value);
			m_Property.requireVariableResolve = true;
		}

		public void InsertVariableReference(int index, string value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 3);
			m_StyleSheet.WriteFunction(ref m_Property.values[valueSpan.start], StyleValueFunction.Var);
			m_StyleSheet.WriteFloat(ref m_Property.values[valueSpan.start + 1], 1f);
			m_StyleSheet.WriteVariable(ref m_Property.values[valueSpan.start + 2], value);
			m_Property.requireVariableResolve = true;
		}

		public bool TryGetVariableReference(int index, out string value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length >= 3 && m_Property.values[valueSpan.start].valueType == StyleValueType.Function && m_Property.values[valueSpan.start + 1].valueType == StyleValueType.Float && m_Property.values[valueSpan.start + 2].valueType == StyleValueType.Variable)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadVariable(m_Property.values[valueSpan.start + 2], out value);
		}

		public void AddResourcePath(string value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteResourcePath(ref handle, value);
			AddHandle(handle);
		}

		public void SetResourcePath(int index, string value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteResourcePath(ref m_Property.values[span.start], value);
		}

		public void InsertResourcePath(int index, string value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteResourcePath(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetResourcePath(int index, out string value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadResourcePath(m_Property.values[valueSpan.start], out value);
		}

		public void AddAssetReference(Object value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteAssetReference(ref handle, value);
			AddHandle(handle);
		}

		public void SetAssetReference(int index, Object value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteAssetReference(ref m_Property.values[span.start], value);
		}

		public void InsertAssetReference(int index, Object value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteAssetReference(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetAssetReference(int index, out Object value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadAssetReference(m_Property.values[valueSpan.start], out value);
		}

		public bool TryGetAssetReference<TObject>(int index, out TObject value) where TObject : Object
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			if (m_StyleSheet.TryReadAssetReference(m_Property.values[valueSpan.start], out var value2) && value2 is TObject val)
			{
				value = val;
				return true;
			}
			value = null;
			return false;
		}

		public void AddMissingAssetReferenceUrl(string value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteMissingAssetReferenceUrl(ref handle, value);
			AddHandle(handle);
		}

		public void SetMissingAssetReferenceUrl(int index, string value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteMissingAssetReferenceUrl(ref m_Property.values[span.start], value);
		}

		public void InsertMissingAssetReferenceUrl(int index, string value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteMissingAssetReferenceUrl(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetMissingAssetReferenceUrl(int index, out string value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = null;
				return false;
			}
			return m_StyleSheet.TryReadMissingAssetReferenceUrl(m_Property.values[valueSpan.start], out value);
		}

		public void AddScalableImage(ScalableImage value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteScalableImage(ref handle, value);
			AddHandle(handle);
		}

		public void SetScalableImage(int index, ScalableImage value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteScalableImage(ref m_Property.values[span.start], value);
		}

		public void InsertScalableImage(int index, ScalableImage value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteScalableImage(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetScalableImage(int index, out ScalableImage value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(ScalableImage);
				return false;
			}
			return m_StyleSheet.TryReadScalableImage(m_Property.values[valueSpan.start], out value);
		}

		public void AddAngle(Angle value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteAngle(ref handle, value);
			AddHandle(handle);
		}

		public void SetAngle(int index, Angle value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteAngle(ref m_Property.values[span.start], value);
		}

		public void InsertAngle(int index, Angle value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteAngle(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetAngle(int index, out Angle value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(Angle);
				return false;
			}
			return m_StyleSheet.TryReadAngle(m_Property.values[valueSpan.start], out value);
		}

		public void AddKeyword(StyleKeyword value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteKeyword(ref handle, value.ToStyleValueKeyword());
			AddHandle(handle);
		}

		public void SetKeyword(int index, StyleKeyword value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteKeyword(ref m_Property.values[span.start], value.ToStyleValueKeyword());
		}

		public void InsertKeyword(int index, StyleKeyword value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteKeyword(ref m_Property.values[valueSpan.start], value.ToStyleValueKeyword());
		}

		public bool TryGetKeyword(int index, out StyleKeyword value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = StyleKeyword.Undefined;
				return false;
			}
			return TryReadKeyword(m_StyleSheet, ref m_Property.values[valueSpan.start], out value);
		}

		public void AddInt(int value)
		{
			AddFloat(value);
		}

		public void SetInt(int index, int value)
		{
			SetFloat(index, value);
		}

		public void InsertInt(int index, int value)
		{
			InsertFloat(index, value);
		}

		public bool TryGetInt(int index, out int value)
		{
			if (TryGetFloat(index, out var value2))
			{
				value = (int)value2;
				return true;
			}
			value = 0;
			return false;
		}

		public void AddLength(Length value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteLength(ref handle, value);
			AddHandle(handle);
		}

		public void SetLength(int index, Length value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteLength(ref m_Property.values[span.start], value);
		}

		public void InsertLength(int index, Length value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteLength(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetLength(int index, out Length value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(Length);
				return false;
			}
			return m_StyleSheet.TryReadLength(m_Property.values[valueSpan.start], out value);
		}

		public void AddTimeValue(TimeValue value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteTimeValue(ref handle, value);
			AddHandle(handle);
		}

		public void SetTimeValue(int index, TimeValue value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteTimeValue(ref m_Property.values[span.start], value);
		}

		public void InsertTimeValue(int index, TimeValue value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteTimeValue(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetTimeValue(int index, out TimeValue value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(TimeValue);
				return false;
			}
			return m_StyleSheet.TryReadTimeValue(m_Property.values[valueSpan.start], out value);
		}

		public void AddStylePropertyName(StylePropertyName value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteStylePropertyName(ref handle, value);
			AddHandle(handle);
		}

		public void SetStylePropertyName(int index, StylePropertyName value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteStylePropertyName(ref m_Property.values[span.start], value);
		}

		public void InsertStylePropertyName(int index, StylePropertyName value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteStylePropertyName(ref m_Property.values[valueSpan.start], value);
		}

		public bool TryGetStylePropertyName(int index, out StylePropertyName value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(StylePropertyName);
				return false;
			}
			return m_StyleSheet.TryReadStylePropertyName(m_Property.values[valueSpan.start], out value);
		}

		public void AddEasingFunction(EasingFunction value)
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteEnum(ref handle, value.mode);
			AddHandle(handle);
		}

		public void SetEasingFunction(int index, EasingFunction value)
		{
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[span.start], value.mode);
		}

		public void InsertEasingFunction(int index, EasingFunction value)
		{
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, 1);
			m_StyleSheet.WriteEnum(ref m_Property.values[valueSpan.start], value.mode);
		}

		public bool TryGetEasingFunction(int index, out EasingFunction value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length != 1)
			{
				value = default(EasingFunction);
				return false;
			}
			if (m_StyleSheet.TryReadEnum(m_Property.values[valueSpan.start], out EasingMode value2))
			{
				value = new EasingFunction(value2);
				return true;
			}
			value = default(EasingFunction);
			return false;
		}

		public void AddFilterFunction(FilterFunction value)
		{
			bool flag = value.type == FilterFunctionType.Custom && (bool)value.customDefinition;
			int num = value.parameterCount + (flag ? 1 : 0);
			int count = num + 2;
			int num2 = m_Property.values.Length;
			Insert(m_Property, m_Property.values.Length, count);
			m_StyleSheet.WriteFunction(ref m_Property.values[num2], ToStyleValueFunction(value.type));
			m_StyleSheet.WriteFloat(ref m_Property.values[num2 + 1], num);
			int num3 = num2 + 2;
			if (flag)
			{
				m_StyleSheet.WriteAssetReference(ref m_Property.values[num3], value.customDefinition);
				num3++;
			}
			int num4 = 0;
			while (num4 < value.parameterCount)
			{
				FilterParameter parameter = value.GetParameter(num4);
				switch (parameter.type)
				{
				case FilterParameterType.Float:
					m_StyleSheet.WriteFloat(ref m_Property.values[num3], parameter.floatValue);
					break;
				case FilterParameterType.Color:
					m_StyleSheet.WriteColor(ref m_Property.values[num3], parameter.colorValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				num4++;
				num3++;
			}
		}

		public void SetFilterFunction(int index, FilterFunction value)
		{
			bool flag = value.type == FilterFunctionType.Custom && (bool)value.customDefinition;
			int num = value.parameterCount + (flag ? 1 : 0);
			int count = num + 2;
			ValueSpan span = GetValueSpan(index);
			ResizeValue(ref span, count);
			m_StyleSheet.WriteFunction(ref m_Property.values[span.start], ToStyleValueFunction(value.type));
			m_StyleSheet.WriteFloat(ref m_Property.values[span.start + 1], num);
			int num2 = span.start + 2;
			if (flag)
			{
				m_StyleSheet.WriteAssetReference(ref m_Property.values[num2], value.customDefinition);
				num2++;
			}
			int num3 = 0;
			while (num3 < value.parameterCount)
			{
				FilterParameter parameter = value.GetParameter(num3);
				switch (parameter.type)
				{
				case FilterParameterType.Float:
					m_StyleSheet.WriteFloat(ref m_Property.values[num2], parameter.floatValue);
					break;
				case FilterParameterType.Color:
					m_StyleSheet.WriteColor(ref m_Property.values[num2], parameter.colorValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				num3++;
				num2++;
			}
		}

		public void InsertFilterFunction(int index, FilterFunction value)
		{
			bool flag = value.type == FilterFunctionType.Custom && (bool)value.customDefinition;
			int num = value.parameterCount + (flag ? 1 : 0);
			int count = num + 2;
			ValueSpan valueSpan = GetValueSpan(index, insertMode: true);
			Insert(m_Property, valueSpan.start, count);
			m_StyleSheet.WriteFunction(ref m_Property.values[valueSpan.start], ToStyleValueFunction(value.type));
			m_StyleSheet.WriteFloat(ref m_Property.values[valueSpan.start + 1], num);
			int num2 = valueSpan.start + 2;
			if (flag)
			{
				m_StyleSheet.WriteAssetReference(ref m_Property.values[num2], value.customDefinition);
				num2++;
			}
			int num3 = 0;
			while (num3 < value.parameterCount)
			{
				FilterParameter parameter = value.GetParameter(num3);
				switch (parameter.type)
				{
				case FilterParameterType.Float:
					m_StyleSheet.WriteFloat(ref m_Property.values[num2], parameter.floatValue);
					break;
				case FilterParameterType.Color:
					m_StyleSheet.WriteColor(ref m_Property.values[num2], parameter.colorValue);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				num3++;
				num2++;
			}
		}

		public bool TryGetFilterFunction(int index, out FilterFunction value)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			if (valueSpan.length <= 1)
			{
				value = default(FilterFunction);
				return false;
			}
			if (!m_StyleSheet.TryReadFunction(m_Property.values[valueSpan.start], out var value2) || !m_StyleSheet.TryReadFloat(m_Property.values[valueSpan.start + 1], out var value3))
			{
				value = default(FilterFunction);
				return false;
			}
			int num = (int)value3;
			FixedBuffer4<FilterParameter> parameters = default(FixedBuffer4<FilterParameter>);
			int num2 = valueSpan.start + 2;
			FilterFunctionType filterFunctionType = ToFilterFunctionType(value2);
			FilterFunctionDefinition customDefinition = null;
			bool flag = false;
			if (filterFunctionType == FilterFunctionType.Custom && num > 0)
			{
				string value5;
				if (m_StyleSheet.TryReadAssetReference(m_Property.values[num2], out var value4))
				{
					customDefinition = (FilterFunctionDefinition)value4;
					flag = true;
				}
				else if (m_StyleSheet.TryReadResourcePath(m_Property.values[num2], out value5))
				{
					customDefinition = (FilterFunctionDefinition)Panel.LoadResource(value5, typeof(Object), 1f);
					flag = true;
				}
				num--;
			}
			int num3 = 0;
			while (num3 < num)
			{
				float value7;
				if (m_StyleSheet.TryReadColor(m_Property.values[num2], out var value6))
				{
					parameters[num3] = new FilterParameter
					{
						type = FilterParameterType.Color,
						colorValue = value6
					};
				}
				else if (m_StyleSheet.TryReadFloat(m_Property.values[num2], out value7))
				{
					parameters[num3] = new FilterParameter
					{
						type = FilterParameterType.Float,
						floatValue = value7
					};
				}
				else if (m_Property.values[num2].valueType != StyleValueType.CommaSeparator)
				{
					Debug.LogError($"Unexpected value type {m_Property.values[num2].valueType} in filter function argument");
				}
				num3++;
				num2++;
			}
			value = (flag ? new FilterFunction(customDefinition, parameters, num) : new FilterFunction(filterFunctionType, parameters, num));
			return true;
		}

		private void WriteMaterialPropertyValue(MaterialPropertyValue value, int index)
		{
			switch (value.type)
			{
			case MaterialPropertyValueType.Float:
				m_StyleSheet.WriteFloat(ref m_Property.values[index++], value.GetFloat());
				break;
			case MaterialPropertyValueType.Vector:
			{
				Vector4 vector = value.GetVector();
				m_StyleSheet.WriteFloat(ref m_Property.values[index++], vector.x);
				m_StyleSheet.WriteFloat(ref m_Property.values[index++], vector.y);
				m_StyleSheet.WriteFloat(ref m_Property.values[index++], vector.z);
				m_StyleSheet.WriteFloat(ref m_Property.values[index++], vector.w);
				break;
			}
			case MaterialPropertyValueType.Color:
				m_StyleSheet.WriteColor(ref m_Property.values[index++], value.GetColor());
				break;
			case MaterialPropertyValueType.Texture:
				m_StyleSheet.WriteAssetReference(ref m_Property.values[index++], value.textureValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool TryGetMaterialPropertyValue(int index, out MaterialPropertyValue value)
		{
			value = default(MaterialPropertyValue);
			ValueSpan valueSpan = GetValueSpan(index + 1);
			if (valueSpan.length < 4)
			{
				return false;
			}
			if (!m_StyleSheet.TryReadFunction(m_Property.values[valueSpan.start], out var value2) || value2 != StyleValueFunction.MaterialProperty)
			{
				return false;
			}
			if (!m_StyleSheet.TryReadFloat(m_Property.values[valueSpan.start + 1], out var value3))
			{
				return false;
			}
			int num = (int)value3;
			if (!m_StyleSheet.TryReadString(m_Property.values[valueSpan.start + 2], out var value4))
			{
				return false;
			}
			int num2 = valueSpan.start + 3;
			int num3 = num - 1;
			if (num3 == 1 && m_StyleSheet.TryReadFloat(m_Property.values[num2], out var value5))
			{
				value = new MaterialPropertyValue
				{
					name = value4,
					type = MaterialPropertyValueType.Float,
					packedValue = new Vector4(value5, 0f, 0f, 0f)
				};
				return true;
			}
			if (num3 == 1 && m_StyleSheet.TryReadColor(m_Property.values[num2], out var value6))
			{
				value = new MaterialPropertyValue
				{
					name = value4,
					type = MaterialPropertyValueType.Color,
					packedValue = new Vector4(value6.r, value6.g, value6.b, value6.a)
				};
				return true;
			}
			if (num3 == 4 && m_StyleSheet.TryReadFloat(m_Property.values[num2], out var value7) && m_StyleSheet.TryReadFloat(m_Property.values[num2 + 1], out var value8) && m_StyleSheet.TryReadFloat(m_Property.values[num2 + 2], out var value9) && m_StyleSheet.TryReadFloat(m_Property.values[num2 + 3], out var value10))
			{
				value = new MaterialPropertyValue
				{
					name = value4,
					type = MaterialPropertyValueType.Vector,
					packedValue = new Vector4(value7, value8, value9, value10)
				};
				return true;
			}
			if (num3 == 1 && m_StyleSheet.TryReadAssetReference(m_Property.values[num2], out var value11))
			{
				value = new MaterialPropertyValue
				{
					name = value4,
					type = MaterialPropertyValueType.Texture,
					textureValue = (value11 as Texture)
				};
				return true;
			}
			return false;
		}

		public void AddMaterialPropertyValue(MaterialPropertyValue value)
		{
			int num = m_Property.values.Length;
			int num2 = 1 + ArgumentCountForMaterialPropertyValueType(value.type);
			Insert(m_Property, m_Property.values.Length, 2 + num2);
			m_StyleSheet.WriteFunction(ref m_Property.values[num], StyleValueFunction.MaterialProperty);
			m_StyleSheet.WriteFloat(ref m_Property.values[num + 1], num2);
			m_StyleSheet.WriteString(ref m_Property.values[num + 2], value.name);
			WriteMaterialPropertyValue(value, num + 3);
		}

		public void SetMaterialPropertyValue(int index, MaterialPropertyValue value)
		{
			int num = ArgumentCountForMaterialPropertyValueType(value.type) + 1;
			int count = 2 + num;
			ValueSpan span = GetValueSpan(index + 1);
			ResizeValue(ref span, count);
			m_StyleSheet.WriteFunction(ref m_Property.values[span.start], StyleValueFunction.MaterialProperty);
			m_StyleSheet.WriteFloat(ref m_Property.values[span.start + 1], num);
			m_StyleSheet.WriteString(ref m_Property.values[span.start + 2], value.name);
			WriteMaterialPropertyValue(value, span.start + 3);
		}

		public void InsertMaterialPropertyValue(int index, MaterialPropertyValue value)
		{
			int num = ArgumentCountForMaterialPropertyValueType(value.type) + 1;
			int count = 2 + num;
			ValueSpan valueSpan = GetValueSpan(index + 1, insertMode: true);
			Insert(m_Property, valueSpan.start, count);
			m_StyleSheet.WriteFunction(ref m_Property.values[valueSpan.start], StyleValueFunction.MaterialProperty);
			m_StyleSheet.WriteFloat(ref m_Property.values[valueSpan.start + 1], num);
			m_StyleSheet.WriteString(ref m_Property.values[valueSpan.start + 2], value.name);
			WriteMaterialPropertyValue(value, valueSpan.start + 3);
		}

		public void AddCommaSeparator()
		{
			StyleValueHandle handle = default(StyleValueHandle);
			m_StyleSheet.WriteCommaSeparator(ref handle);
			AddHandle(handle);
		}

		public void RemoveValue(int index)
		{
			ValueSpan valueSpan = GetValueSpan(index);
			int num = valueSpan.start;
			int num2 = valueSpan.length;
			if (IsCommaSeparator(m_Property.values, valueSpan.start + valueSpan.length))
			{
				num2++;
			}
			else if (IsCommaSeparator(m_Property.values, valueSpan.start - 1))
			{
				num--;
				num2++;
			}
			Remove(m_Property, num, num2);
		}

		private ValueSpan GetValueSpan(int index, bool insertMode = false)
		{
			List<int> value;
			using (CollectionPool<List<int>, int>.Get(out value))
			{
				StyleSheetUtility.GetValueOffsets(m_StyleSheet, m_Property.values, value);
				if (index < 0 || index > value.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index == value.Count)
				{
					if (insertMode)
					{
						return new ValueSpan(m_Property.values.Length, 0);
					}
					throw new ArgumentOutOfRangeException("index");
				}
				int num = value[index];
				int num2 = ((index == value.Count - 1) ? m_Property.values.Length : value[index + 1]);
				int num3 = ((num2 < m_Property.values.Length && m_Property.values[num2 - 1].valueType == StyleValueType.CommaSeparator) ? 1 : 0);
				return new ValueSpan(num, num2 - num - num3);
			}
		}

		private void ResizeValue(ref ValueSpan span, int count)
		{
			if (span.length != count)
			{
				if (count > span.length)
				{
					int count2 = count - span.length;
					Insert(m_Property, span.start, count2);
				}
				else
				{
					int count3 = span.length - count;
					Remove(m_Property, span.start, count3);
				}
			}
		}

		private void AddHandle(StyleValueHandle handle)
		{
			Insert(m_Property, m_Property.values.Length, 1);
			m_Property.values[^1] = handle;
		}

		private static void Insert(StyleProperty property, int index, int count)
		{
			StyleValueHandle[] values = property.values;
			property.values = new StyleValueHandle[values.Length + count];
			Array.Copy(values, 0, property.values, 0, index);
			Array.Copy(values, index, property.values, index + count, values.Length - index);
		}

		private static void Remove(StyleProperty property, int index, int count)
		{
			StyleValueHandle[] values = property.values;
			property.values = new StyleValueHandle[values.Length - count];
			Array.Copy(values, 0, property.values, 0, index);
			Array.Copy(values, index + count, property.values, index, values.Length - (index + count));
		}

		private static bool IsCommaSeparator(StyleValueHandle[] array, int index)
		{
			if (index < 0 || index >= array.Length)
			{
				return false;
			}
			return array[index].valueType == StyleValueType.CommaSeparator;
		}
	}

	[SerializeField]
	private StylePropertyId m_Id;

	[SerializeField]
	private string m_CustomName;

	[SerializeField]
	private int m_Line;

	[SerializeField]
	private StyleValueHandle[] m_Values = Array.Empty<StyleValueHandle>();

	[NonSerialized]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool requireVariableResolve;

	internal StylePropertyId id => m_Id;

	public string name
	{
		get
		{
			StylePropertyId stylePropertyId = id;
			StylePropertyId stylePropertyId2 = stylePropertyId;
			if ((uint)(stylePropertyId2 - -1) <= 1u)
			{
				return m_CustomName;
			}
			return StylePropertyUtil.stylePropertyIdToPropertyName[id];
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal set
		{
			CacheId(value);
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

	internal bool isCustomProperty
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return id == StylePropertyId.Custom;
		}
	}

	internal StyleProperty()
	{
	}

	internal void CacheId(string value)
	{
		m_Id = StylePropertyId.Unknown;
		m_CustomName = value;
		StylePropertyId value2;
		if (string.IsNullOrEmpty(value))
		{
			m_Id = StylePropertyId.Unknown;
		}
		else if (StringUtils.StartsWith(value, "--"))
		{
			m_Id = StylePropertyId.Custom;
		}
		else if (StylePropertyUtil.propertyNameToStylePropertyId.TryGetValue(value, out value2))
		{
			m_Id = value2;
			m_CustomName = null;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetEnumAsString(StyleSheet styleSheet, string enumStr)
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteEnumAsString(ref m_Values[0], enumStr);
		requireVariableResolve = false;
	}

	public void SetEnum<TEnum>(StyleSheet styleSheet, TEnum value) where TEnum : struct, Enum
	{
		SetSize(ref m_Values, 1);
		styleSheet.WriteEnum(ref m_Values[0], value);
		requireVariableResolve = false;
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
		requireVariableResolve = true;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
	}

	public bool TryGetKeyword(StyleSheet styleSheet, out StyleKeyword value)
	{
		if (handleCount == 1)
		{
			return TryReadKeyword(styleSheet, ref m_Values[0], out value);
		}
		value = StyleKeyword.Undefined;
		return false;
	}

	public void SetBackgroundRepeat(StyleSheet styleSheet, BackgroundRepeat value)
	{
		SetSize(ref m_Values, 2);
		styleSheet.WriteEnum(ref values[0], value.x);
		styleSheet.WriteEnum(ref values[1], value.y);
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
			requireVariableResolve = false;
		}
		else
		{
			SetSize(ref m_Values, 2);
			styleSheet.WriteEnum(ref values[0], value.keyword);
			styleSheet.WriteDimension(ref values[1], value.offset.ToDimension());
			requireVariableResolve = false;
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
			requireVariableResolve = false;
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

	public void SetRatio(StyleSheet styleSheet, Ratio value)
	{
		if (value.IsAuto())
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref m_Values[0], StyleValueKeyword.Auto);
		}
		else
		{
			SetFloat(styleSheet, value.value);
		}
	}

	public bool TryGetRatio(StyleSheet styleSheet, out Ratio value)
	{
		if (TryGetFloat(styleSheet, out var value2) && value2 != 0f)
		{
			value = value2;
			return true;
		}
		value = Ratio.Auto();
		return false;
	}

	public void SetRotate(StyleSheet styleSheet, Rotate value)
	{
		if (value.IsNone())
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteKeyword(ref values[0], StyleValueKeyword.None);
			requireVariableResolve = false;
			return;
		}
		if (value.axis == Vector3.forward)
		{
			SetSize(ref m_Values, 1);
			styleSheet.WriteAngle(ref values[0], value.angle);
			requireVariableResolve = false;
			return;
		}
		SetSize(ref m_Values, 4);
		Vector3 axis = value.axis;
		styleSheet.WriteFloat(ref values[0], axis.x);
		styleSheet.WriteFloat(ref values[1], axis.y);
		styleSheet.WriteFloat(ref values[2], axis.z);
		styleSheet.WriteAngle(ref values[3], value.angle);
		requireVariableResolve = false;
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
			requireVariableResolve = false;
		}
		else if (Mathf.Approximately(value.value.z, 1f))
		{
			SetSize(ref m_Values, 2);
			styleSheet.WriteFloat(ref values[0], value.value.x);
			styleSheet.WriteFloat(ref values[1], value.value.y);
			requireVariableResolve = false;
		}
		else
		{
			SetSize(ref m_Values, 3);
			styleSheet.WriteFloat(ref values[0], value.value.x);
			styleSheet.WriteFloat(ref values[1], value.value.y);
			styleSheet.WriteFloat(ref values[2], value.value.z);
			requireVariableResolve = false;
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
		requireVariableResolve = false;
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
				requireVariableResolve = false;
				return;
			}
			if (transformOriginOffset.HasValue && transformOriginOffset2.HasValue && transformOriginOffset.Value == TransformOriginOffset.Center)
			{
				SetSize(ref m_Values, 1);
				styleSheet.WriteEnum(ref m_Values[0], transformOriginOffset2.Value);
				requireVariableResolve = false;
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
		requireVariableResolve = false;
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

	internal static int ArgumentCountForMaterialPropertyValueType(MaterialPropertyValueType type)
	{
		switch (type)
		{
		case MaterialPropertyValueType.Float:
		case MaterialPropertyValueType.Color:
		case MaterialPropertyValueType.Texture:
			return 1;
		case MaterialPropertyValueType.Vector:
			return 4;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void SetMaterialDefinition(StyleSheet styleSheet, MaterialDefinition value)
	{
		int num = 1;
		if (value.propertyValues != null)
		{
			foreach (MaterialPropertyValue propertyValue in value.propertyValues)
			{
				num += ArgumentCountForMaterialPropertyValueType(propertyValue.type) + 3;
			}
		}
		SetSize(ref m_Values, num);
		styleSheet.WriteAssetReference(ref values[0], value.material);
		if (value.propertyValues == null)
		{
			return;
		}
		int num2 = 1;
		foreach (MaterialPropertyValue propertyValue2 in value.propertyValues)
		{
			styleSheet.WriteFunction(ref values[num2++], StyleValueFunction.MaterialProperty);
			styleSheet.WriteFloat(ref values[num2++], ArgumentCountForMaterialPropertyValueType(propertyValue2.type) + 1);
			styleSheet.WriteString(ref values[num2++], propertyValue2.name);
			switch (propertyValue2.type)
			{
			case MaterialPropertyValueType.Color:
			{
				Color color = value.GetColor(propertyValue2.name);
				styleSheet.WriteColor(ref values[num2++], color);
				break;
			}
			case MaterialPropertyValueType.Float:
			{
				float value2 = value.GetFloat(propertyValue2.name);
				styleSheet.WriteFloat(ref values[num2++], value2);
				break;
			}
			case MaterialPropertyValueType.Vector:
			{
				Vector4 vector = value.GetVector(propertyValue2.name);
				styleSheet.WriteFloat(ref values[num2++], vector.x);
				styleSheet.WriteFloat(ref values[num2++], vector.y);
				styleSheet.WriteFloat(ref values[num2++], vector.z);
				styleSheet.WriteFloat(ref values[num2++], vector.w);
				break;
			}
			case MaterialPropertyValueType.Texture:
			{
				Texture texture = value.GetTexture(propertyValue2.name);
				styleSheet.WriteAssetReference(ref values[num2++], texture);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	public bool TryGetMaterialDefinition(StyleSheet styleSheet, out MaterialDefinition value)
	{
		value = default(MaterialDefinition);
		if (handleCount < 1)
		{
			return false;
		}
		if (!styleSheet.TryReadAssetReference(values[0], out var value2))
		{
			if (!styleSheet.TryReadResourcePath(values[0], out var value3))
			{
				return false;
			}
			value2 = (Material)Panel.LoadResource(value3, typeof(Material), 1f);
		}
		List<MaterialPropertyValue> list = new List<MaterialPropertyValue>();
		int num = 1;
		while (num < values.Length)
		{
			StyleValueFunction valueIndex = (StyleValueFunction)values[num++].valueIndex;
			if (valueIndex != StyleValueFunction.MaterialProperty)
			{
				break;
			}
			if (!styleSheet.TryReadFloat(values[num++], out var value4))
			{
				return false;
			}
			int num2 = (int)value4;
			if (!styleSheet.TryReadString(values[num++], out var value5))
			{
				return false;
			}
			StyleValueType valueType = values[num].valueType;
			switch (valueType)
			{
			case StyleValueType.Float:
			{
				int num3 = num2 - 1;
				Vector4 zero = Vector4.zero;
				int num4 = 0;
				while (num4 < num3)
				{
					if (!styleSheet.TryReadFloat(values[num++], out var value9))
					{
						return false;
					}
					zero[num4++] = value9;
				}
				MaterialPropertyValueType type = ((num3 > 1) ? MaterialPropertyValueType.Vector : MaterialPropertyValueType.Float);
				list.Add(new MaterialPropertyValue
				{
					name = value5,
					type = type,
					packedValue = zero
				});
				break;
			}
			default:
				if (valueType != StyleValueType.Enum)
				{
					if (valueType == StyleValueType.AssetReference || valueType == StyleValueType.ResourcePath || valueType == StyleValueType.MissingAssetReference)
					{
						Object value6 = null;
						if (valueType != StyleValueType.MissingAssetReference)
						{
							if (values[num].valueType == StyleValueType.AssetReference)
							{
								if (!styleSheet.TryReadAssetReference(values[num++], out value6))
								{
									return false;
								}
							}
							else if (values[num].valueType == StyleValueType.ResourcePath)
							{
								if (!styleSheet.TryReadResourcePath(values[num++], out var value7))
								{
									return false;
								}
								value6 = (Texture)Panel.LoadResource(value7, typeof(Texture), 1f);
							}
						}
						else
						{
							num++;
						}
						list.Add(new MaterialPropertyValue
						{
							name = value5,
							type = MaterialPropertyValueType.Texture,
							textureValue = (value6 as Texture)
						});
						break;
					}
					Debug.LogError($"Unexpected value type {valueType} in material property argument");
					return false;
				}
				goto case StyleValueType.Color;
			case StyleValueType.Color:
			{
				if (!styleSheet.TryReadColor(values[num++], out var value8))
				{
					return false;
				}
				list.Add(new MaterialPropertyValue
				{
					name = value5,
					type = MaterialPropertyValueType.Color,
					packedValue = new Vector4(value8.r, value8.g, value8.b, value8.a)
				});
				break;
			}
			}
		}
		value = new MaterialDefinition
		{
			material = (value2 as Material),
			propertyValues = list
		};
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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
		requireVariableResolve = false;
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

	public void SetFilter(StyleSheet styleSheet, List<FilterFunction> filterFunctions)
	{
		int num = 0;
		foreach (FilterFunction filterFunction in filterFunctions)
		{
			num += GetNumberOfValuesForFilterFunction(filterFunction);
		}
		SetSize(ref m_Values, num);
		int num2 = 0;
		foreach (FilterFunction filterFunction2 in filterFunctions)
		{
			styleSheet.WriteFunction(ref values[num2++], ToStyleValueFunction(filterFunction2.type));
			int num3 = filterFunction2.parameterCount;
			if (filterFunction2.customDefinition != null)
			{
				num3++;
			}
			styleSheet.WriteFloat(ref values[num2++], num3);
			if (filterFunction2.customDefinition != null)
			{
				styleSheet.WriteAssetReference(ref values[num2++], filterFunction2.customDefinition);
			}
			for (int i = 0; i < filterFunction2.parameterCount; i++)
			{
				FilterParameter parameter = filterFunction2.GetParameter(i);
				if (parameter.type == FilterParameterType.Float)
				{
					styleSheet.WriteFloat(ref values[num2++], parameter.floatValue);
				}
				else if (parameter.type == FilterParameterType.Color)
				{
					styleSheet.WriteColor(ref values[num2++], parameter.colorValue);
				}
			}
		}
	}

	public bool TryGetFilter(StyleSheet styleSheet, out List<FilterFunction> value)
	{
		if (ContainsVariable())
		{
			value = null;
			return false;
		}
		value = new List<FilterFunction>();
		return TryGetFilter(styleSheet, value);
	}

	public bool TryGetFilter(StyleSheet styleSheet, List<FilterFunction> value)
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
		int num = 0;
		while (num < m_Values.Length)
		{
			if (!styleSheet.TryReadFunction(m_Values[num++], out var value2))
			{
				value.Clear();
				return false;
			}
			if (!styleSheet.TryReadFloat(m_Values[num++], out var value3))
			{
				value.Clear();
				return false;
			}
			int num2 = (int)value3;
			FilterFunctionDefinition filterFunctionDefinition = null;
			if (value2 == StyleValueFunction.CustomFilter && num2 > 0)
			{
				if (!styleSheet.TryReadAssetReference(m_Values[num++], out var value4))
				{
					value.Clear();
					return false;
				}
				filterFunctionDefinition = value4 as FilterFunctionDefinition;
				if (filterFunctionDefinition == null)
				{
					value.Clear();
					return false;
				}
				num2--;
				if (filterFunctionDefinition.parameters.Length != num2)
				{
					value.Clear();
					return false;
				}
			}
			FixedBuffer4<FilterParameter> parameters = default(FixedBuffer4<FilterParameter>);
			for (int i = 0; i < num2; i++)
			{
				StyleValueHandle handle = m_Values[num++];
				if (styleSheet.TryReadDimension(handle, out var value5))
				{
					parameters[i] = new FilterParameter(ConvertDimensionToFilterFloat(value5));
					continue;
				}
				if (styleSheet.TryReadFloat(handle, out var value6))
				{
					parameters[i] = new FilterParameter(value6);
					continue;
				}
				if (styleSheet.TryReadColor(handle, out var value7))
				{
					parameters[i] = new FilterParameter(value7);
					continue;
				}
				value.Clear();
				return false;
			}
			if (value2 == StyleValueFunction.CustomFilter)
			{
				value.Add(new FilterFunction(filterFunctionDefinition, parameters, num2));
			}
			else
			{
				value.Add(new FilterFunction(ToFilterFunctionType(value2), parameters, num2));
			}
		}
		return true;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static int GetNumberOfValuesForFilterFunction(FilterFunction ff)
	{
		int num = 0;
		FilterFunctionDefinition definition = ff.GetDefinition();
		int num2 = (((object)definition != null) ? definition.parameters.Length : 0);
		if (ff.customDefinition != null)
		{
			num++;
		}
		return num + (num2 + 2);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static FilterFunctionType ToFilterFunctionType(StyleValueFunction function)
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
			StyleValueFunction.FilterContrast => FilterFunctionType.Contrast, 
			StyleValueFunction.FilterHueRotate => FilterFunctionType.HueRotate, 
			_ => FilterFunctionType.None, 
		};
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static StyleValueFunction ToStyleValueFunction(FilterFunctionType type)
	{
		return type switch
		{
			FilterFunctionType.None => StyleValueFunction.NoneFilter, 
			FilterFunctionType.Tint => StyleValueFunction.FilterTint, 
			FilterFunctionType.Opacity => StyleValueFunction.FilterOpacity, 
			FilterFunctionType.Invert => StyleValueFunction.FilterInvert, 
			FilterFunctionType.Grayscale => StyleValueFunction.FilterGrayscale, 
			FilterFunctionType.Sepia => StyleValueFunction.FilterSepia, 
			FilterFunctionType.Blur => StyleValueFunction.FilterBlur, 
			FilterFunctionType.Contrast => StyleValueFunction.FilterContrast, 
			FilterFunctionType.HueRotate => StyleValueFunction.FilterHueRotate, 
			_ => StyleValueFunction.CustomFilter, 
		};
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static float ConvertDimensionToFilterFloat(Dimension dim)
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

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static Dimension ConvertFilterFloatToDimension(float value, Dimension.Unit unit)
	{
		switch (unit)
		{
		case Dimension.Unit.Percent:
			value *= 100f;
			break;
		case Dimension.Unit.Degree:
			value *= 57.29578f;
			break;
		case Dimension.Unit.Turn:
			value /= MathF.PI * 2f;
			break;
		case Dimension.Unit.Gradian:
			value /= MathF.PI / 200f;
			break;
		case Dimension.Unit.Millisecond:
			value *= 1000f;
			break;
		}
		return new Dimension(value, unit);
	}

	private static void SetSize(ref StyleValueHandle[] store, int size)
	{
		StyleValueHandle[] obj = store;
		if (obj == null || obj.Length != size)
		{
			store = new StyleValueHandle[size];
		}
	}

	internal static bool TryReadKeyword(StyleSheet styleSheet, ref StyleValueHandle handle, out StyleKeyword value)
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

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Manipulator GetManipulator(StyleSheet styleSheet)
	{
		return new Manipulator(styleSheet, this);
	}
}
