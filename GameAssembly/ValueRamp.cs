using System;
using UnityEngine;

[Serializable]
public struct ValueRamp
{
	private float value;

	private bool increasing;

	public float rampSpeed;

	public AnimationCurve rampCurve;

	public void SetIncreasing(bool increasing)
	{
		this.increasing = increasing;
	}

	public bool Update(float deltaTime)
	{
		if (!increasing)
		{
			if (!(value > 0f))
			{
				return false;
			}
			value -= deltaTime * rampSpeed;
		}
		else if (value < 1f)
		{
			value += deltaTime * rampSpeed;
		}
		value = BMath.Clamp01(value);
		return true;
	}

	public float GetValue()
	{
		return rampCurve.Evaluate(value);
	}

	public void ForceValue(float forcedValue)
	{
		value = forcedValue;
	}
}
