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
internal class AngleField : TextValueField<Angle>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextValueField<Angle>.UxmlSerializedData
	{
		[SerializeField]
		private bool showUnitAsDropdown;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags showUnitAsDropdown_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			TextValueField<Angle>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("showUnitAsDropdown", "show-unit-as-dropdown", null)
			});
		}

		public override object CreateInstance()
		{
			return new AngleField();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			AngleField angleField = (AngleField)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(showUnitAsDropdown_UxmlAttributeFlags))
			{
				angleField.showUnitAsDropdown = showUnitAsDropdown;
			}
		}
	}

	private class AngleInput : TextValueInput
	{
		internal AngleField parentAngleField { get; set; }

		protected override string allowedCharacters => UINumericFieldsUtils.k_AllowedCharactersForFloat;

		internal AngleInput()
		{
			base.formatString = UINumericFieldsUtils.k_DoubleFieldFormatString;
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Angle startValue)
		{
			Angle angle = StringToValue(base.text);
			angle.unit = startValue.unit;
			if (angle.IsNone())
			{
				angle = new Angle(0f);
			}
			double num = angle.value;
			double num2 = NumericFieldDraggerUtility.CalculateIntDragSensitivity((long)startValue.value);
			float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			num += (double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num2;
			num = Mathf.RoundBasedOnMinimumDifference(num, num2);
			angle = new Angle((float)num, angle.unit);
			if (parentAngleField.isDelayed)
			{
				parentAngleField.text = ValueToString(angle);
			}
			else
			{
				parentAngleField.value = angle;
			}
		}

		protected override string ValueToString(Angle v)
		{
			return parentAngleField.showUnitAsDropdown ? v.value.ToString(CultureInfo.InvariantCulture) : v.ToString();
		}

		protected override Angle StringToValue(string str)
		{
			Angle angle;
			return Angle.TryParseString(str, out angle) ? angle : parentAngleField.value;
		}
	}

	public static readonly BindingId showUnitAsDropdownProperty = "showUnitAsDropdown";

	public new static readonly string ussClassName = "unity-style-field";

	public static readonly string angleFieldUssClassName = "unity-angle-field";

	public new static readonly string inputUssClassName = ussClassName + "__visual-input";

	public static readonly string unitDropdownContainerUssClass = ussClassName + "__options-popup-container";

	public static readonly string unitDropdownUssClass = ussClassName + "__options-popup";

	public static readonly string invisibleUnitDropdownUssClass = unitDropdownUssClass + "--invisible";

	public static readonly string KeywordInitial = "initial";

	public static readonly string KeywordNone = "none";

	public const string UnitDegree = "deg";

	public const string UnitGrad = "grad";

	public const string UnitRad = "rad";

	public const string UnitTurn = "turn";

	private static readonly string[] KLDefaultUnits = new string[4] { "deg", "grad", "rad", "turn" };

	private static readonly string[] AllKeywords = new string[2] { KeywordNone, KeywordInitial };

	internal static readonly string s_NoOptionString = "-";

	private bool m_ShowUnitAsDropdown;

	private readonly PopupField<string> m_OptionsPopup;

	private readonly List<string> m_AllOptionsList = new List<string>();

	private AngleInput angleInput => (AngleInput)base.textInputBase;

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

	public AngleField()
		: this(null)
	{
	}

	public AngleField(int maxAngle)
		: this(null, maxAngle)
	{
	}

	public AngleField(string label, int maxAngle = 1000)
		: base(label, maxAngle, (TextValueInput)new AngleInput())
	{
		AddToClassList(ussClassName);
		AddToClassList(angleFieldUssClassName);
		AddLabelDragger<Angle>();
		VisualElement visualElement = new VisualElement();
		visualElement.name = unitDropdownContainerUssClass;
		visualElement.AddToClassList(unitDropdownContainerUssClass);
		m_AllOptionsList.AddRange(KLDefaultUnits);
		m_AllOptionsList.AddRange(AllKeywords);
		m_OptionsPopup = new PopupField<string>(m_AllOptionsList, 0, OnFormatSelectedValue);
		m_OptionsPopup.AddToClassList(unitDropdownUssClass);
		visualElement.Add(m_OptionsPopup);
		angleInput.parentAngleField = this;
		angleInput.AddToClassList(inputUssClassName);
		angleInput.delegatesFocus = true;
		Add(visualElement);
		m_OptionsPopup.RegisterValueChangedCallback(OnPopupFieldValueChange);
		UpdateFields();
		showUnitAsDropdown = true;
	}

	public override void SetValueWithoutNotify(Angle newValue)
	{
		base.SetValueWithoutNotify(newValue);
		SetOptionsPopupFromValue();
	}

	public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, Angle startValue)
	{
		angleInput.ApplyInputDeviceDelta(delta, speed, startValue);
	}

	protected override string ValueToString(Angle v)
	{
		if (showUnitAsDropdown && !v.IsNone())
		{
			return v.value.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
		}
		return v.ToString();
	}

	protected override Angle StringToValue(string str)
	{
		ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(str).Trim();
		if (MemoryExtensions.Equals(readOnlySpan, KeywordNone, StringComparison.OrdinalIgnoreCase))
		{
			return Angle.None();
		}
		AngleUnit angleUnit = value.unit;
		if (angleUnit != AngleUnit.Degree && angleUnit != AngleUnit.Gradian && angleUnit != AngleUnit.Radian && angleUnit != AngleUnit.Turn)
		{
			angleUnit = AngleUnit.Degree;
		}
		ReadOnlySpan<char> readOnlySpan2;
		if (readOnlySpan.EndsWith("deg", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "deg".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			angleUnit = AngleUnit.Degree;
		}
		else if (readOnlySpan.EndsWith("grad", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "grad".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			angleUnit = AngleUnit.Gradian;
		}
		else if (readOnlySpan.EndsWith("rad", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "rad".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			angleUnit = AngleUnit.Radian;
		}
		else if (readOnlySpan.EndsWith("turn", StringComparison.Ordinal))
		{
			readOnlySpan2 = readOnlySpan;
			int length = "turn".Length;
			readOnlySpan = readOnlySpan2.Slice(0, readOnlySpan2.Length - length);
			angleUnit = AngleUnit.Turn;
		}
		float num;
		ExpressionEvaluator.Expression expression;
		return UINumericFieldsUtils.TryConvertStringToFloat(readOnlySpan.ToString(), base.textInputBase.originalText, out num, out expression) ? new Angle(num, angleUnit) : value;
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
		if (value.IsNone())
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
		Angle angle = newValue switch
		{
			"deg" => new Angle(value.value, AngleUnit.Degree), 
			"grad" => new Angle(value.value, AngleUnit.Gradian), 
			"rad" => new Angle(value.value, AngleUnit.Radian), 
			"turn" => new Angle(value.value, AngleUnit.Turn), 
			_ => value, 
		};
		if (1 == 0)
		{
		}
		value = angle;
		evt.StopImmediatePropagation();
	}
}
