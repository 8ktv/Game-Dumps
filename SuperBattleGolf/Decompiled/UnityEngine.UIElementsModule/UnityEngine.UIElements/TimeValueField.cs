using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class TimeValueField : TextValueField<TimeValue>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextValueField<TimeValue>.UxmlSerializedData
	{
		[SerializeField]
		private bool showUnitAsDropdown;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		[RegisterUxmlCache]
		public new static void Register()
		{
			TextValueField<TimeValue>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null)
			});
		}

		public override object CreateInstance()
		{
			return new TimeValueField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TimeValueField timeValueField = (TimeValueField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showUnitAsDropdown_UxmlAttributeFlags))
			{
				timeValueField.showUnitAsDropdown = showUnitAsDropdown;
			}
		}
	}

	private class TimeValueInput : TextValueInput
	{
		internal TimeValueField parentTimeValueField { get; set; }

		protected override string allowedCharacters => UINumericFieldsUtils.k_AllowedCharactersForFloat;

		internal TimeValueInput()
		{
			base.formatString = UINumericFieldsUtils.k_DoubleFieldFormatString;
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeValue startValue)
		{
			TimeValue timeValue = StringToValue(base.text);
			timeValue.unit = startValue.unit;
			double num = timeValue.value;
			double num2 = NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
			float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			num += (double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num2;
			num = Mathf.RoundBasedOnMinimumDifference(num, num2);
			timeValue = new TimeValue((float)num, timeValue.unit);
			if (parentTimeValueField.isDelayed)
			{
				parentTimeValueField.text = ValueToString(timeValue);
			}
			else
			{
				parentTimeValueField.value = timeValue;
			}
		}

		protected override string ValueToString(TimeValue v)
		{
			return parentTimeValueField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
		}

		protected override TimeValue StringToValue(string str)
		{
			TimeValue timeValue;
			return TimeValue.TryParseString(str, out timeValue) ? timeValue : parentTimeValueField.value;
		}
	}

	public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

	public new static readonly string ussClassName = "unity-style-field";

	public static readonly string timeValueFieldUssClassName = "unity-time-value-field";

	public new static readonly string inputUssClassName = ussClassName + "__visual-input";

	public static readonly string unitDropdownContainerUssClass = ussClassName + "__options-popup-container";

	public static readonly string unitDropdownUssClass = ussClassName + "__options-popup";

	public static readonly string invisibleUnitDropdownUssClass = unitDropdownUssClass + "--invisible";

	public static readonly string KeywordInitial = "initial";

	public const string UnitSecond = "s";

	public const string UnitMillisecond = "ms";

	private static readonly string[] KLDefaultUnits = new string[2] { "s", "ms" };

	private static readonly string[] AllKeywords = new string[1] { KeywordInitial };

	internal static readonly string s_NoOptionString = "-";

	private bool m_ShowUnitAsDropdown;

	private readonly PopupField<string> m_OptionsPopup;

	private readonly List<string> m_AllOptionsList = new List<string>();

	private TimeValueInput timeValueInput => (TimeValueInput)base.textInputBase;

	[CreateProperty]
	[UxmlAttribute]
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

	public TimeValueField()
		: this(null)
	{
	}

	public TimeValueField(int maxTimeValue)
		: this(null, maxTimeValue)
	{
	}

	public TimeValueField(string label, int maxTimeValue = 1000)
		: base(label, maxTimeValue, (TextValueInput)new TimeValueInput())
	{
		AddToClassList(ussClassName);
		AddToClassList(timeValueFieldUssClassName);
		AddLabelDragger<TimeValue>();
		VisualElement visualElement = new VisualElement();
		visualElement.name = unitDropdownContainerUssClass;
		visualElement.AddToClassList(unitDropdownContainerUssClass);
		m_AllOptionsList.AddRange(KLDefaultUnits);
		m_AllOptionsList.AddRange(AllKeywords);
		m_OptionsPopup = new PopupField<string>(m_AllOptionsList, 0, OnFormatSelectedValue);
		m_OptionsPopup.AddToClassList(unitDropdownUssClass);
		visualElement.Add(m_OptionsPopup);
		timeValueInput.parentTimeValueField = this;
		timeValueInput.AddToClassList(inputUssClassName);
		timeValueInput.delegatesFocus = true;
		Add(visualElement);
		m_OptionsPopup.RegisterValueChangedCallback(OnPopupFieldValueChange);
		UpdateFields();
		showUnitAsDropdown = true;
	}

	public override void SetValueWithoutNotify(TimeValue newValue)
	{
		base.SetValueWithoutNotify(newValue);
		SetOptionsPopupFromValue();
	}

	public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, TimeValue startValue)
	{
		timeValueInput.ApplyInputDeviceDelta(delta, speed, startValue);
	}

	protected override string ValueToString(TimeValue v)
	{
		return showUnitAsDropdown ? v.value.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat) : v.ToString();
	}

	protected override TimeValue StringToValue(string str)
	{
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(str).Trim();
		TimeUnit timeUnit = value.unit;
		if (timeUnit != TimeUnit.Second && timeUnit != TimeUnit.Millisecond)
		{
			timeUnit = TimeUnit.Second;
		}
		ReadOnlySpan<char> readOnlySpan2;
		if (readOnlySpan.EndsWith("ms", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "ms".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			timeUnit = TimeUnit.Millisecond;
		}
		else if (readOnlySpan.EndsWith("s", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "s".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			timeUnit = TimeUnit.Second;
		}
		float num;
		ExpressionEvaluator.Expression expression;
		return UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num, out expression) ? new TimeValue(num, timeUnit) : value;
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

	private static string OnFormatSelectedValue(string value)
	{
		return (Array.IndexOf(AllKeywords, value) < 0) ? value : s_NoOptionString;
	}

	private void SetOptionsPopupFromValue()
	{
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
		TimeValue timeValue = ((newValue == "s") ? new TimeValue(value.value, TimeUnit.Second) : ((!(newValue == "ms")) ? value : new TimeValue(value.value, TimeUnit.Millisecond)));
		if (1 == 0)
		{
		}
		value = timeValue;
		evt.StopImmediatePropagation();
	}
}
