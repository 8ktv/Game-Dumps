using DG.Tweening.Core.Easing;
using UnityEngine;

namespace DG.Tweening;

public static class DOVirtual
{
	public static Tweener Float(float from, float to, float duration, TweenCallback<float> onVirtualUpdate)
	{
		return DOTween.To(() => from, delegate(float x)
		{
			from = x;
		}, to, duration).OnUpdate(delegate
		{
			onVirtualUpdate(from);
		});
	}

	public static Tweener Int(int from, int to, float duration, TweenCallback<int> onVirtualUpdate)
	{
		return DOTween.To(() => from, delegate(int x)
		{
			from = x;
		}, to, duration).OnUpdate(delegate
		{
			onVirtualUpdate(from);
		});
	}

	public static Tweener Vector2(Vector2 from, Vector2 to, float duration, TweenCallback<Vector2> onVirtualUpdate)
	{
		return DOTween.To(() => from, delegate(Vector2 x)
		{
			from = x;
		}, to, duration).OnUpdate(delegate
		{
			onVirtualUpdate(from);
		});
	}

	public static Tweener Vector3(Vector3 from, Vector3 to, float duration, TweenCallback<Vector3> onVirtualUpdate)
	{
		return DOTween.To(() => from, delegate(Vector3 x)
		{
			from = x;
		}, to, duration).OnUpdate(delegate
		{
			onVirtualUpdate(from);
		});
	}

	public static Tweener Color(Color from, Color to, float duration, TweenCallback<Color> onVirtualUpdate)
	{
		return DOTween.To(() => from, delegate(Color x)
		{
			from = x;
		}, to, duration).OnUpdate(delegate
		{
			onVirtualUpdate(from);
		});
	}

	public static float EasedValue(float from, float to, float lifetimePercentage, Ease easeType)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, DOTween.defaultEaseOvershootOrAmplitude, DOTween.defaultEasePeriod);
	}

	public static float EasedValue(float from, float to, float lifetimePercentage, Ease easeType, float overshoot)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, overshoot, DOTween.defaultEasePeriod);
	}

	public static float EasedValue(float from, float to, float lifetimePercentage, Ease easeType, float amplitude, float period)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, amplitude, period);
	}

	public static float EasedValue(float from, float to, float lifetimePercentage, AnimationCurve easeCurve)
	{
		return from + (to - from) * EaseManager.Evaluate(Ease.INTERNAL_Custom, new EaseCurve(easeCurve).Evaluate, lifetimePercentage, 1f, DOTween.defaultEaseOvershootOrAmplitude, DOTween.defaultEasePeriod);
	}

	public static Vector3 EasedValue(Vector3 from, Vector3 to, float lifetimePercentage, Ease easeType)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, DOTween.defaultEaseOvershootOrAmplitude, DOTween.defaultEasePeriod);
	}

	public static Vector3 EasedValue(Vector3 from, Vector3 to, float lifetimePercentage, Ease easeType, float overshoot)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, overshoot, DOTween.defaultEasePeriod);
	}

	public static Vector3 EasedValue(Vector3 from, Vector3 to, float lifetimePercentage, Ease easeType, float amplitude, float period)
	{
		return from + (to - from) * EaseManager.Evaluate(easeType, null, lifetimePercentage, 1f, amplitude, period);
	}

	public static Vector3 EasedValue(Vector3 from, Vector3 to, float lifetimePercentage, AnimationCurve easeCurve)
	{
		return from + (to - from) * EaseManager.Evaluate(Ease.INTERNAL_Custom, new EaseCurve(easeCurve).Evaluate, lifetimePercentage, 1f, DOTween.defaultEaseOvershootOrAmplitude, DOTween.defaultEasePeriod);
	}

	public static Tween DelayedCall(float delay, TweenCallback callback, bool ignoreTimeScale = true)
	{
		return DOTween.Sequence().AppendInterval(delay).OnStepComplete(callback)
			.SetUpdate(UpdateType.Normal, ignoreTimeScale)
			.SetAutoKill(autoKillOnCompletion: true);
	}
}
