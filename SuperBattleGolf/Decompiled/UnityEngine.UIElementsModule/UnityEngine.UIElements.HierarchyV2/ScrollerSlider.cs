using System;

namespace UnityEngine.UIElements.HierarchyV2;

internal class ScrollerSlider : BaseSlider<double>
{
	public ScrollerSlider(double start, double end, SliderDirection direction, float pageSize)
		: base((string)null, start, end, direction, pageSize)
	{
	}

	internal override double SliderLerpUnclamped(double a, double b, float interpolant)
	{
		double num = a + (b - a) * (double)interpolant;
		double num2 = Math.Abs((base.highValue - base.lowValue) / (double)(base.dragContainer.resolvedStyle.width - base.dragElement.resolvedStyle.width));
		int digits = ((num2 == 0.0) ? Math.Clamp((int)(5.0 - Math.Log10(Math.Abs(num2))), 0, 15) : ((int)Math.Clamp(0.0 - Math.Floor(Math.Log10(Math.Abs(num2))), 0.0, 15.0)));
		return Math.Round(num, digits, MidpointRounding.AwayFromZero);
	}

	internal override float SliderNormalizeValue(double currentValue, double lowerValue, double higherValue)
	{
		double num = higherValue - lowerValue;
		if (Math.Abs(num) < 1E-05)
		{
			return 1f;
		}
		double num2 = (currentValue - lowerValue) / num;
		return (float)Math.Clamp(num2, 0.0, 1.0);
	}

	internal override double SliderRange()
	{
		return Math.Abs(base.highValue - base.lowValue);
	}

	internal override double ParseStringToValue(string previousValue, string newValue)
	{
		if (UINumericFieldsUtils.TryConvertStringToDouble(newValue, previousValue, out var result, out var _))
		{
			return result;
		}
		return 0.0;
	}

	private static double GetClosestPowerOfTen(double positiveNumber)
	{
		if (positiveNumber <= 0.0)
		{
			return 1.0;
		}
		return Math.Pow(10.0, Math.Round(Math.Log10(positiveNumber)));
	}

	private static double RoundToMultipleOf(double value, double roundingValue)
	{
		if (roundingValue == 0.0)
		{
			return value;
		}
		return Math.Round(value / roundingValue) * roundingValue;
	}

	internal override void ComputeValueFromKey(SliderKey sliderKey, bool isShift)
	{
		switch (sliderKey)
		{
		case SliderKey.None:
			return;
		case SliderKey.Lowest:
			value = base.lowValue;
			return;
		case SliderKey.Highest:
			value = base.highValue;
			return;
		}
		bool flag = sliderKey == SliderKey.LowerPage || sliderKey == SliderKey.HigherPage;
		double num = GetClosestPowerOfTen(Math.Abs((base.highValue - base.lowValue) * 0.009999999776482582));
		if (flag)
		{
			num *= (double)pageSize;
		}
		else if (isShift)
		{
			num *= 10.0;
		}
		if (sliderKey == SliderKey.Lower || sliderKey == SliderKey.LowerPage)
		{
			num = 0.0 - num;
		}
		value = RoundToMultipleOf(value + num * 0.5001, Math.Abs(num));
	}
}
