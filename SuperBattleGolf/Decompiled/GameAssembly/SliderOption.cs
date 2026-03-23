using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderOption : MonoBehaviour
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private TextMeshProUGUI sliderLabel;

	private Action onChanged;

	private bool supressSfx = true;

	private float lastSfx;

	public Slider Slider => slider;

	public float value
	{
		get
		{
			return slider.value;
		}
		set
		{
			SetValue(value);
		}
	}

	public float valueWithoutNotify
	{
		set
		{
			slider.SetValueWithoutNotify(value);
		}
	}

	public Navigation navigation
	{
		get
		{
			return slider.navigation;
		}
		set
		{
			slider.navigation = value;
		}
	}

	public void Initialize(Action onChanged, float value)
	{
		supressSfx = true;
		this.onChanged = onChanged;
		SetValue(value);
		supressSfx = false;
	}

	public void SetValue(float value)
	{
		supressSfx = true;
		slider.value = value;
		onChanged?.Invoke();
		supressSfx = false;
	}

	public void SetValueText(string text)
	{
		sliderLabel.text = text;
	}

	public void SetLimits(float min, float max)
	{
		slider.minValue = min;
		slider.maxValue = max;
	}

	public float SetSensitivityValue(float minSensitivity, float maxSensitivity, bool snapOnKeyboard = true, bool snapOnGamepad = false)
	{
		if (InputManager.UsingKeyboard ? snapOnKeyboard : snapOnGamepad)
		{
			SnapToMiddle();
		}
		float num = RemapSliderValueToValueMiddleLinear(slider.value, minSensitivity, maxSensitivity, 1f);
		SetValueText($"{num:0.0}x");
		return num;
	}

	public float SetPercentageValue(float maxValue = 1f, bool force1ToMiddle = false, bool snapOnKeyboard = true, bool snapOnGamepad = false)
	{
		if (InputManager.UsingKeyboard ? snapOnKeyboard : snapOnGamepad)
		{
			SnapToMiddle();
		}
		float num = slider.value;
		if (force1ToMiddle)
		{
			num = RemapSliderValueToValueMiddleLinear(num, 0f, maxValue, 1f);
		}
		SetValueText(GetPercentageString(num));
		return num;
	}

	private float SnapToMiddle(float tolerance = 0.05f)
	{
		return valueWithoutNotify = Snap(value, 0.5f, tolerance);
	}

	private static float Snap(float value, float to, float tolerance)
	{
		if (value.Approximately(to, tolerance))
		{
			return to;
		}
		return value;
	}

	private static float RemapSliderValueToValueMiddleLinear(float slideValue, float min, float max, float middle)
	{
		if (slideValue < 0.5f)
		{
			return BMath.Remap(0f, 0.5f, min, middle, slideValue);
		}
		return BMath.Remap(0.5f, 1f, middle, max, slideValue);
	}

	public static float RemapValueSliderValueMiddleLinear(float value, float min, float max, float middle)
	{
		if (value < middle)
		{
			return BMath.Remap(min, middle, 0f, 0.5f, value);
		}
		return BMath.Remap(middle, max, 0.5f, 1f, value);
	}

	private static string GetPercentageString(float normalizedValue)
	{
		return BMath.RoundToInt(normalizedValue * 100f) + "%";
	}

	public void OnValueChanged()
	{
		onChanged?.Invoke();
		if (!supressSfx && Time.time - lastSfx > 0.1f)
		{
			RuntimeManager.PlayOneShot(GameManager.AudioSettings.SliderOnValueChange);
			lastSfx = Time.time;
		}
	}
}
