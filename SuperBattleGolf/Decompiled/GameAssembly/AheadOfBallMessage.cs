using System;
using System.Collections;
using UnityEngine;

public class AheadOfBallMessage : SingletonBehaviour<AheadOfBallMessage>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private bool isVisible;

	private Coroutine fadeRoutine;

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
	}

	public static void Show()
	{
		if (SingletonBehaviour<AheadOfBallMessage>.HasInstance)
		{
			SingletonBehaviour<AheadOfBallMessage>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonBehaviour<AheadOfBallMessage>.HasInstance)
		{
			SingletonBehaviour<AheadOfBallMessage>.Instance.HideInternal();
		}
	}

	private void ShowInternal()
	{
		if (!isVisible)
		{
			isVisible = true;
			FadeTo(1f, fadeInDuration, BMath.EaseOut);
		}
	}

	private void HideInternal()
	{
		if (isVisible)
		{
			isVisible = false;
			FadeTo(0f, fadeInDuration, BMath.EaseIn);
		}
	}

	private Coroutine FadeTo(float targetAlpha, float duration, Func<float, float> Easing)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, Easing));
		return fadeRoutine;
		IEnumerator FadeRoutine(float num2, float num, Func<float, float> func)
		{
			float initialAlpha = visibilityController.DesiredAlpha;
			for (float time = 0f; time < num; time += Time.deltaTime)
			{
				float arg = time / num;
				float t = func(arg);
				visibilityController.SetDesiredAlpha(BMath.Lerp(initialAlpha, num2, t));
				yield return null;
			}
			visibilityController.SetDesiredAlpha(num2);
		}
	}
}
