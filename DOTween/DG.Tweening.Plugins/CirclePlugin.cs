using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Core;
using UnityEngine;

namespace DG.Tweening.Plugins;

public class CirclePlugin : ABSTweenPlugin<Vector2, Vector2, CircleOptions>
{
	public override void Reset(TweenerCore<Vector2, Vector2, CircleOptions> t)
	{
	}

	public override void SetFrom(TweenerCore<Vector2, Vector2, CircleOptions> t, bool isRelative)
	{
		if (!t.plugOptions.initialized)
		{
			t.startValue = t.getter();
			t.plugOptions.Initialize(t.startValue, t.endValue);
		}
		float endValueDegrees = t.plugOptions.endValueDegrees;
		t.plugOptions.endValueDegrees = t.plugOptions.startValueDegrees;
		t.plugOptions.startValueDegrees = (isRelative ? (t.plugOptions.endValueDegrees + endValueDegrees) : endValueDegrees);
		t.startValue = GetPositionOnCircle(t.plugOptions, t.plugOptions.startValueDegrees);
		t.setter(t.startValue);
	}

	public override void SetFrom(TweenerCore<Vector2, Vector2, CircleOptions> t, Vector2 fromValue, bool setImmediately, bool isRelative)
	{
		if (!t.plugOptions.initialized)
		{
			t.startValue = t.getter();
			t.plugOptions.Initialize(t.startValue, t.endValue);
		}
		float num = fromValue.x;
		if (isRelative)
		{
			float startValueDegrees = t.plugOptions.startValueDegrees;
			t.plugOptions.endValueDegrees += startValueDegrees;
			num += startValueDegrees;
		}
		t.plugOptions.startValueDegrees = num;
		t.startValue = GetPositionOnCircle(t.plugOptions, num);
		if (setImmediately)
		{
			t.setter(t.startValue);
		}
	}

	public static ABSTweenPlugin<Vector2, Vector2, CircleOptions> Get()
	{
		return PluginsManager.GetCustomPlugin<CirclePlugin, Vector2, Vector2, CircleOptions>();
	}

	public override Vector2 ConvertToStartValue(TweenerCore<Vector2, Vector2, CircleOptions> t, Vector2 value)
	{
		return value;
	}

	public override void SetRelativeEndValue(TweenerCore<Vector2, Vector2, CircleOptions> t)
	{
		if (!t.plugOptions.initialized)
		{
			t.plugOptions.Initialize(t.startValue, t.endValue);
		}
		t.plugOptions.endValueDegrees += t.plugOptions.startValueDegrees;
	}

	public override void SetChangeValue(TweenerCore<Vector2, Vector2, CircleOptions> t)
	{
		if (!t.plugOptions.initialized)
		{
			t.plugOptions.Initialize(t.startValue, t.endValue);
		}
		t.changeValue = new Vector2(t.plugOptions.endValueDegrees - t.plugOptions.startValueDegrees, 0f);
	}

	public override float GetSpeedBasedDuration(CircleOptions options, float unitsXSecond, Vector2 changeValue)
	{
		return changeValue.x / unitsXSecond;
	}

	public override void EvaluateAndApply(CircleOptions options, Tween t, bool isRelative, DOGetter<Vector2> getter, DOSetter<Vector2> setter, float elapsed, Vector2 startValue, Vector2 changeValue, float duration, bool usingInversePosition, int newCompletedSteps, UpdateNotice updateNotice)
	{
		float num = options.startValueDegrees;
		if (t.loopType == LoopType.Incremental)
		{
			num += changeValue.x * (float)(t.isComplete ? (t.completedLoops - 1) : t.completedLoops);
		}
		if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
		{
			num += changeValue.x * (float)((t.loopType != LoopType.Incremental) ? 1 : t.loops) * (float)(t.sequenceParent.isComplete ? (t.sequenceParent.completedLoops - 1) : t.sequenceParent.completedLoops);
		}
		float num2 = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
		setter(GetPositionOnCircle(options, num + changeValue.x * num2));
	}

	public Vector2 GetPositionOnCircle(CircleOptions options, float degrees)
	{
		Vector2 pointOnCircle = DOTweenUtils.GetPointOnCircle(options.center, options.radius, degrees);
		if (options.snapping)
		{
			pointOnCircle.x = Mathf.Round(pointOnCircle.x);
			pointOnCircle.y = Mathf.Round(pointOnCircle.y);
		}
		return pointOnCircle;
	}
}
