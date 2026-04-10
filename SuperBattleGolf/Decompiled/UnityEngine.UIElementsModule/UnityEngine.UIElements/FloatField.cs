using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.UIElements;

[MovedFrom(true, "UnityEditor.UIElements", "UnityEditor.UIElementsModule", null)]
public class FloatField : TextValueField<float>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextValueField<float>.UxmlSerializedData
	{
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			TextValueField<float>.UxmlSerializedData.Register();
		}

		public override object CreateInstance()
		{
			return new FloatField();
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<FloatField, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : TextValueFieldTraits<float, UxmlFloatAttributeDescription>
	{
	}

	private class FloatInput : TextValueInput
	{
		private FloatField parentFloatField => (FloatField)base.parent;

		protected override string allowedCharacters => parentFloatField.supportExpressions ? UINumericFieldsUtils.k_AllowedCharactersForFloat : UINumericFieldsUtils.k_AllowedCharactersForFloat_NoExpressions;

		internal FloatInput()
		{
			base.formatString = UINumericFieldsUtils.k_FloatFieldFormatString;
		}

		public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, float startValue)
		{
			double num = NumericFieldDraggerUtility.CalculateFloatDragSensitivity(startValue);
			float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
			double num2 = StringToValue(base.text);
			num2 += (double)NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * num;
			num2 = Mathf.RoundBasedOnMinimumDifference(num2, num);
			if (parentFloatField.isDelayed)
			{
				base.text = ValueToString(Mathf.ClampToFloat(num2));
			}
			else
			{
				parentFloatField.value = Mathf.ClampToFloat(num2);
			}
		}

		protected override string ValueToString(float v)
		{
			return v.ToString(base.formatString);
		}

		protected override float StringToValue(string str)
		{
			return parentFloatField.StringToValue(str);
		}
	}

	public new static readonly string ussClassName = "unity-float-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	private FloatInput floatInput => (FloatInput)base.textInputBase;

	protected override string ValueToString(float v)
	{
		return v.ToString(base.formatString, CultureInfo.InvariantCulture.NumberFormat);
	}

	protected override float StringToValue(string str)
	{
		float num;
		ExpressionEvaluator.Expression expression;
		bool flag = UINumericFieldsUtils.TryConvertStringToFloat(str, base.textInputBase.originalText, out num, out expression);
		expressionEvaluated?.Invoke(expression);
		return flag ? num : base.rawValue;
	}

	internal override void UpdateValueFromText()
	{
		string text = ValueToString(base.rawValue);
		if (text != base.text)
		{
			base.UpdateValueFromText();
		}
	}

	public FloatField()
		: this(null)
	{
	}

	public FloatField(int maxLength)
		: this(null, maxLength)
	{
	}

	public FloatField(string label, int maxLength = 1000)
		: base(label, maxLength, (TextValueInput)new FloatInput())
	{
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		AddLabelDragger<float>();
	}

	internal override bool CanTryParse(string textString)
	{
		float result;
		return float.TryParse(textString, out result);
	}

	public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, float startValue)
	{
		floatInput.ApplyInputDeviceDelta(delta, speed, startValue);
	}
}
