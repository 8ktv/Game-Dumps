using System;
using System.Collections;
using UnityEngine;

public class HudManager : SingletonBehaviour<HudManager>
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	private Coroutine visibilityCoroutine;

	public static void Show(bool instant)
	{
		if (SingletonBehaviour<HudManager>.HasInstance)
		{
			SingletonBehaviour<HudManager>.Instance.ShowInternal(instant);
		}
	}

	public static void Hide(bool instant)
	{
		if (SingletonBehaviour<HudManager>.HasInstance)
		{
			SingletonBehaviour<HudManager>.Instance.HideInternal(instant);
		}
	}

	private void ShowInternal(bool instant)
	{
		if (visibilityCoroutine != null)
		{
			StopCoroutine(visibilityCoroutine);
		}
		if (instant)
		{
			canvasGroup.alpha = 1f;
		}
		else
		{
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(1f, 0.1f, BMath.EaseOut));
		}
	}

	private void HideInternal(bool instant)
	{
		if (visibilityCoroutine != null)
		{
			StopCoroutine(visibilityCoroutine);
		}
		if (instant)
		{
			canvasGroup.alpha = 0f;
		}
		else
		{
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(0f, 0.1f, BMath.EaseIn));
		}
	}

	private IEnumerator AnimateVisibilityRoutine(float targetAlpha, float duration, Func<float, float> Easing)
	{
		float initialAlpha = canvasGroup.alpha;
		for (float time = 0f; time < duration; time += Time.deltaTime)
		{
			float arg = time / duration;
			canvasGroup.alpha = BMath.Lerp(initialAlpha, targetAlpha, Easing(arg));
			yield return null;
		}
		canvasGroup.alpha = targetAlpha;
	}
}
