using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class RangeSliderAttribute : PropertyAttribute
{
	public float Min { get; private set; }

	public float Max { get; private set; }

	public RangeSliderAttribute(float min, float max)
	{
		Min = min;
		Max = max;
	}
}
