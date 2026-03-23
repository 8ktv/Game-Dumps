using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class OutOfBoundsMessage : SingletonBehaviour<OutOfBoundsMessage>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private TextMeshProUGUI message;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private int displayedTime = -1;

	private bool wantsToShow;

	private bool isTemporarilyForceHidden;

	private Coroutine forceHideTemporarilyRoutine;

	private bool isVisible;

	private Coroutine fadeRoutine;

	private static string UnformattedMessage => Localization.UI.OUT_OF_BOUNDS_Message;

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
		LocalizationManager.LanguageChanged += OnLocalizationLanguageChanged;
	}

	protected override void OnDestroy()
	{
		LocalizationManager.LanguageChanged -= OnLocalizationLanguageChanged;
		base.OnDestroy();
	}

	public static void Show()
	{
		if (SingletonBehaviour<OutOfBoundsMessage>.HasInstance)
		{
			SingletonBehaviour<OutOfBoundsMessage>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonBehaviour<OutOfBoundsMessage>.HasInstance)
		{
			SingletonBehaviour<OutOfBoundsMessage>.Instance.HideInternal();
		}
	}

	public static void ForceHideTemporarily(float duration)
	{
		if (SingletonBehaviour<OutOfBoundsMessage>.HasInstance)
		{
			SingletonBehaviour<OutOfBoundsMessage>.Instance.ForceHideTemporarilyInternal(duration);
		}
	}

	public static void SetRemainingTime(float time)
	{
		if (SingletonBehaviour<OutOfBoundsMessage>.HasInstance)
		{
			SingletonBehaviour<OutOfBoundsMessage>.Instance.SetRemainingTimeInternal(time);
		}
	}

	private void ShowInternal()
	{
		wantsToShow = true;
		UpdateVisibility();
	}

	private void HideInternal()
	{
		wantsToShow = false;
		UpdateVisibility();
	}

	private void ForceHideTemporarilyInternal(float duration)
	{
		if (forceHideTemporarilyRoutine != null)
		{
			StopCoroutine(forceHideTemporarilyRoutine);
		}
		forceHideTemporarilyRoutine = StartCoroutine(ForceHideTemporarilyRoutine(duration));
		IEnumerator ForceHideTemporarilyRoutine(float seconds)
		{
			isTemporarilyForceHidden = true;
			UpdateVisibility();
			yield return new WaitForSeconds(seconds);
			isTemporarilyForceHidden = false;
			UpdateVisibility();
		}
	}

	private void SetRemainingTimeInternal(float time)
	{
		int num = displayedTime;
		displayedTime = BMath.CeilToInt(BMath.Max(0f, time));
		if (displayedTime != num)
		{
			UpdateMessageInternal();
		}
	}

	private void UpdateVisibility()
	{
		bool flag = isVisible;
		isVisible = wantsToShow && !isTemporarilyForceHidden;
		if (isVisible != flag)
		{
			if (isVisible)
			{
				FadeTo(1f, fadeInDuration, BMath.EaseOut);
			}
			else
			{
				FadeTo(0f, fadeInDuration, BMath.EaseIn);
			}
		}
		IEnumerator FadeRoutine(float targetAlpha, float duration, Func<float, float> Easing)
		{
			float initialAlpha = visibilityController.DesiredAlpha;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				float arg = time / duration;
				float t = Easing(arg);
				visibilityController.SetDesiredAlpha(BMath.Lerp(initialAlpha, targetAlpha, t));
				yield return null;
			}
			visibilityController.SetDesiredAlpha(targetAlpha);
		}
		Coroutine FadeTo(float targetAlpha, float duration, Func<float, float> Easing)
		{
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, Easing));
			return fadeRoutine;
		}
	}

	private void UpdateMessageInternal()
	{
		message.text = string.Format(UnformattedMessage, displayedTime);
	}

	private void OnLocalizationLanguageChanged()
	{
		UpdateMessageInternal();
	}
}
