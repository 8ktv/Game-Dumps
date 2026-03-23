using UnityEngine;

public static class AnimationCurveExtensions
{
	public static void NormalizeTime(this AnimationCurve curve)
	{
		if (curve.length >= 2)
		{
			Keyframe[] keys = curve.keys;
			float time = keys[0].time;
			float time2 = keys[^1].time;
			for (int i = 0; i < keys.Length; i++)
			{
				keys[i].time = BMath.InverseLerpClamped(time, time2, keys[i].time);
			}
			curve.keys = keys;
		}
	}

	public static float NormalizeValues(this AnimationCurve curve)
	{
		Keyframe[] keys = curve.keys;
		float num = 0f;
		for (int i = 0; i < keys.Length; i++)
		{
			num = BMath.Max(num, keys[i].value);
		}
		for (int j = 0; j < keys.Length; j++)
		{
			keys[j].value = keys[j].value / num;
		}
		curve.keys = keys;
		return num;
	}
}
