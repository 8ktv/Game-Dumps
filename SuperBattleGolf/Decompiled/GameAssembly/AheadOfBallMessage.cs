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

	[SerializeField]
	private float blinkDelay;

	[SerializeField]
	private float blinkInterval;

	private bool isVisible;

	private float startTime;

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
			startTime = Time.time;
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

	private void Update()
	{
		if (isVisible)
		{
			float num = Time.time - startTime - blinkDelay;
			if (!(num < 0f))
			{
				float num2 = num % blinkInterval / blinkInterval;
				num2 = (num2 - 0.5f) * 2f;
				visibilityController.SetDesiredAlpha(BMath.Abs(num2));
			}
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
