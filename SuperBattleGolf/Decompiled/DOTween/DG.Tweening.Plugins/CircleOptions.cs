using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening.Plugins;

public struct CircleOptions : IPlugOptions
{
	public float endValueDegrees;

	public bool relativeCenter;

	public bool snapping;

	internal Vector2 center;

	internal float radius;

	internal float startValueDegrees;

	internal bool initialized;

	public void Reset()
	{
		initialized = false;
		startValueDegrees = (endValueDegrees = 0f);
		relativeCenter = false;
		snapping = false;
	}

	public void Initialize(Vector2 startValue, Vector2 endValue)
	{
		initialized = true;
		center = endValue;
		if (relativeCenter)
		{
			center = startValue + center;
		}
		radius = Vector2.Distance(center, startValue);
		Vector2 vector = startValue - center;
		startValueDegrees = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
	}
}
