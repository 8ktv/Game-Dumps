using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class LengthField : TextValueField<Length>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextValueField<Length>.UxmlSerializedData
	{
		[SerializeField]
		private bool showUnitAsDropdown;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			TextValueField<Length>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null)
			});
		}

		public override object CreateInstance()
		{
			return new LengthField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			LengthField lengthField = (LengthField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showUnitAsDropdown_UxmlAttributeFlags))
			{
				lengthField.showUnitAsDropdown = showUnitAsDropdown;
			}
		}
	}

	private class LengthInput : TextValueInput
	{
		internal LengthField parentLengthField { get; set; }

		protected override string allowedCharacters => UINumericFieldsUtils.k_AllowedCharactersForFloat;

		internal LengthInput()
		{
			base.formatString = UINumericFieldsUtils.k_DoubleFieldFormatString;
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Length startValue)
		{
			Length length = StringToValue(base.text);
			length.unit = startValue.unit;
			if (length.IsAuto() || length.IsNone())
			{
				length = new Length(0f);
			}
			double num = length.value;
			double num2 = NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
			float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			num += (double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num2;
			num = Mathf.RoundBasedOnMinimumDifference(num, num2);
			length = new Length((float)num, length.unit);
			if (parentLengthField.isDelayed)
			{
				parentLengthField.text = ValueToString(length);
			}
			else
			{
				parentLengthField.value = length;
			}
		}

		protected override string ValueToString(Length v)
		{
			return parentLengthField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
		}

		protected override Length StringToValue(string str)
		{
			return Length.ParseString(str, parentLengthField.value);
		}
	}

	public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

	public new static readonly string ussClassName = "unity-style-field";

	public new static readonly string inputUssClassName = ussClassName + "__visual-input";

	public static readonly string unitDropdownContainerUssClass = ussClassName + "__options-popup-container";

	public static readonly string unitDropdownUssClass = ussClassName + "__options-popup";

	public static readonly string invisibleUnitDropdownUssClass = unitDropdownUssClass + "--invisible";

	public static readonly string KeywordInitial = "initial";

	public static readonly string KeywordAuto = "auto";

	public static readonly string KeywordNone = "none";

	public static readonly string UnitPixel = "px";

	public static readonly string UnitPercent = "%";

	private static readonly string[] KLInitial = new string[1] { KeywordInitial };

	private static readonly string[] KLDefaultUnits = new string[2] { UnitPixel, UnitPercent };

	private static readonly string[] KLAuto = new string[2] { KeywordAuto, KeywordInitial };

	private static readonly string[] KLNone = new string[2] { KeywordNone, KeywordInitial };

	private static readonly string[] AllKeywords = new string[3] { KeywordAuto, KeywordNone, KeywordInitial };

	internal static readonly string s_NoOptionString = "-";

	private bool m_ShowUnitAsDropdown;

	private readonly List<LengthUnit> m_Units = new List<LengthUnit> { LengthUnit.Pixel };

	private readonly PopupField<string> m_OptionsPopup;

	private readonly List<string> m_StyleKeywords = new List<string>();

	private readonly List<string> m_CachedRegularOptionsList = new List<string>();

	private readonly List<string> m_AllOptionsList = new List<string>();

	private readonly StyleMatcher m_StyleMatcher = new StyleMatcher();

	private Expression m_SyntaxTree;

	private LengthInput lengthInput => (LengthInput)base.textInputBase;

	[CreateProperty]
	public bool showUnitAsDropdown
	{
		get
		{
			return m_ShowUnitAsDropdown;
		}
		set
		{
			if (m_ShowUnitAsDropdown != value)
			{
				m_ShowUnitAsDropdown = value;
				UpdateFields();
				NotifyPropertyChanged(in showUnitAsDropdownProperty);
			}
		}
	}

	protected internal PopupField<string> optionsPopup => m_OptionsPopup;

	protected List<string> styleKeywords => m_StyleKeywords;

	public LengthField()
		: this(null)
	{
	}

	public LengthField(int maxLength)
		: this(null, maxLength)
	{
	}

	public LengthField(string label, int maxLength = 1000)
		: base(label, maxLength, (TextValueInput)new LengthInput())
	{
		AddToClassList(ussClassName);
		AddLabelDragger<Length>();
		VisualElement visualElement = new VisualElement();
		visualElement.name = unitDropdownContainerUssClass;
		visualElement.AddToClassList(unitDropdownContainerUssClass);
		m_StyleKeywords.AddRange(KLAuto);
		PopulateAdditionalOptions(m_CachedRegularOptionsList);
		m_AllOptionsList.AddRange(m_CachedRegularOptionsList);
		m_AllOptionsList.AddRange(m_StyleKeywords);
		m_OptionsPopup = new PopupField<string>(m_AllOptionsList, 0, OnFormatSelectedValue);
		m_OptionsPopup.AddToClassList(unitDropdownUssClass);
		visualElement.Add(m_OptionsPopup);
		lengthInput.parentLengthField = this;
		lengthInput.AddToClassList(inputUssClassName);
		lengthInput.delegatesFocus = true;
		Add(visualElement);
		m_OptionsPopup.RegisterValueChangedCallback(OnPopupFieldValueChange);
		UpdateFields();
		showUnitAsDropdown = true;
	}

	public void PopulateStyleKeywords(List<string> keywordList)
	{
		if (m_SyntaxTree == null)
		{
			keywordList.AddRange(KLAuto);
			return;
		}
		bool flag = FindKeywordInExpression(m_SyntaxTree, KeywordAuto);
		bool flag2 = FindKeywordInExpression(m_SyntaxTree, KeywordNone);
		if (flag)
		{
			keywordList.AddRange(KLAuto);
		}
		else if (flag2)
		{
			keywordList.AddRange(KLNone);
		}
		else
		{
			keywordList.AddRange(KLInitial);
		}
	}

	public override void SetValueWithoutNotify(Length newValue)
	{
		if (IsValid(newValue))
		{
			base.SetValueWithoutNotify(newValue);
			SetOptionsPopupFromValue();
		}
	}

	public void SetValidation(StylePropertyValidation validation)
	{
		if (validation is Syntax syntax)
		{
			m_SyntaxTree = Syntax.GetSyntaxTree(syntax);
			UpdateOptionsMenu();
		}
	}

	public void SetValidation(in StylePropertyValidationCollection validation)
	{
		List<Syntax> list;
		using (CollectionPool<List<Syntax>, Syntax>.Get(out list))
		{
			foreach (StylePropertyValidation item2 in validation)
			{
				if (item2 is Syntax item)
				{
					list.Add(item);
				}
			}
			m_SyntaxTree = Syntax.GetSyntaxTree(list);
			UpdateOptionsMenu();
		}
	}

	public void ClearValidation()
	{
		m_SyntaxTree = null;
		UpdateOptionsMenu();
	}

	public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Length startValue)
	{
		lengthInput.ApplyInputDeviceDelta(delta, speed, startValue);
	}

	protected override string ValueToString(Length v)
	{
		if (showUnitAsDropdown && !v.IsAuto() && !v.IsNone())
		{
			return v.value.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}
		return v.ToString();
	}

	protected override Length StringToValue(string str)
	{
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(str).Trim();
		if (MemoryExtensions.Equals(readOnlySpan, KeywordAuto, StringComparison.OrdinalIgnoreCase))
		{
			return Length.Auto();
		}
		if (MemoryExtensions.Equals(readOnlySpan, KeywordNone, StringComparison.OrdinalIgnoreCase))
		{
			return Length.None();
		}
		LengthUnit lengthUnit = value.unit;
		if (lengthUnit != LengthUnit.Percent && lengthUnit != LengthUnit.Pixel)
		{
			lengthUnit = LengthUnit.Pixel;
		}
		ReadOnlySpan<char> readOnlySpan2;
		if (readOnlySpan.EndsWith(UnitPercent, StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = UnitPercent.Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			lengthUnit = LengthUnit.Percent;
		}
		else if (readOnlySpan.EndsWith(UnitPixel, StringComparison.OrdinalIgnoreCase))
		{
			readOnlySpan2 = readOnlySpan;
			int length = UnitPixel.Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			lengthUnit = LengthUnit.Pixel;
		}
		float num;
		ExpressionEvaluator.Expression expression;
		return UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num, out expression) ? new Length(num, lengthUnit) : value;
	}

	internal override bool CanTryParse(string textString)
	{
		double result;
		return double.TryParse(textString, out result);
	}

	private void UpdateFields()
	{
		base.text = ValueToString(value);
		m_OptionsPopup.EnableInClassList(invisibleUnitDropdownUssClass, !showUnitAsDropdown);
	}

	private bool IsValid(Length newValue)
	{
		if (m_SyntaxTree == null)
		{
			return true;
		}
		return m_StyleMatcher.Match(m_SyntaxTree, newValue.ToString()).success;
	}

	protected internal bool Validate(Length previousValue, Length newValue)
	{
		if (!IsValid(newValue))
		{
			value = previousValue;
			return false;
		}
		return true;
	}

	private void UpdateOptionsMenu()
	{
		m_CachedRegularOptionsList.Clear();
		PopulateAdditionalOptions(m_CachedRegularOptionsList);
		m_StyleKeywords.Clear();
		PopulateStyleKeywords(m_StyleKeywords);
		m_AllOptionsList.Clear();
		m_AllOptionsList.AddRange(m_CachedRegularOptionsList);
		m_AllOptionsList.AddRange(m_StyleKeywords);
		m_OptionsPopup.choices = m_AllOptionsList;
		if (!IsValid(value))
		{
			value = GetValidValue();
		}
		SetOptionsPopupFromValue();
	}

	internal void AddOption(string newOption)
	{
		m_OptionsPopup.choices.Add(newOption);
		if (!IsValid(value))
		{
			value = GetValidValue();
		}
		SetOptionsPopupFromValue();
	}

	private Length GetValidValue()
	{
		if (m_Units.Count == 0)
		{
			return Length.Auto();
		}
		if (m_Units.Contains(LengthUnit.Pixel))
		{
			return 0f;
		}
		if (m_Units.Contains(LengthUnit.Percent))
		{
			return Length.Percent(0f);
		}
		return Length.None();
	}

	private static bool FindKeywordInExpression(Expression expression, string keyword)
	{
		if (expression.type == ExpressionType.Keyword && expression.keyword == keyword)
		{
			return true;
		}
		if (expression.subExpressions == null)
		{
			return false;
		}
		Expression[] subExpressions = expression.subExpressions;
		foreach (Expression expression2 in subExpressions)
		{
			if (FindKeywordInExpression(expression2, keyword))
			{
				return true;
			}
		}
		return false;
	}

	private void PopulateAdditionalOptions(List<string> additionalOptions)
	{
		if (m_SyntaxTree == null)
		{
			additionalOptions.AddRange(KLDefaultUnits);
			return;
		}
		bool flag = FindUnitInExpression(m_SyntaxTree, DataType.Length);
		bool flag2 = FindUnitInExpression(m_SyntaxTree, DataType.Percentage);
		m_Units.Clear();
		if (flag)
		{
			m_Units.Add(LengthUnit.Pixel);
		}
		if (flag2)
		{
			m_Units.Add(LengthUnit.Percent);
		}
		foreach (LengthUnit unit in m_Units)
		{
			additionalOptions.Add(unit.ToDisplayString());
		}
	}

	private bool FindUnitInExpression(Expression expression, DataType dataType)
	{
		if (expression.type == ExpressionType.Data && expression.dataType == dataType)
		{
			return true;
		}
		if (expression.subExpressions == null)
		{
			return false;
		}
		Expression[] subExpressions = expression.subExpressions;
		foreach (Expression expression2 in subExpressions)
		{
			if (FindUnitInExpression(expression2, dataType))
			{
				return true;
			}
		}
		return false;
	}

	private static string OnFormatSelectedValue(string value)
	{
		return (Array.IndexOf(AllKeywords, value) < 0) ? value : s_NoOptionString;
	}

	private void SetOptionsPopupFromValue()
	{
		if (value.IsAuto())
		{
			m_OptionsPopup.SetValueWithoutNotify(KeywordAuto);
		}
		else if (value.IsNone())
		{
			m_OptionsPopup.SetValueWithoutNotify(KeywordNone);
		}
		string text = value.unit.ToDisplayString();
		if (string.Compare(text, s_NoOptionString, StringComparison.OrdinalIgnoreCase) != 0)
		{
			m_OptionsPopup.SetValueWithoutNotify(text);
		}
	}

	private void OnPopupFieldValueChange(ChangeEvent<string> evt)
	{
		if (evt.target != optionsPopup)
		{
			evt.StopImmediatePropagation();
			return;
		}
		string newValue = evt.newValue;
		if (1 == 0)
		{
		}
		Length length = newValue switch
		{
			"auto" => Length.Auto(), 
			"none" => Length.None(), 
			"px" => new Length(value.value, LengthUnit.Pixel), 
			"%" => new Length(value.value, LengthUnit.Percent), 
			_ => value, 
		};
		if (1 == 0)
		{
		}
		value = length;
		evt.StopImmediatePropagation();
	}
}
