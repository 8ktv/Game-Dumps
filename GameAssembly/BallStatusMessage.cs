using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BallStatusMessage : SingletonBehaviour<BallStatusMessage>
{
	private enum Status
	{
		None,
		Returned
	}

	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private TextMeshProUGUI message;

	[SerializeField]
	private float returnedMessageDuration;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private Status displayedStatus;

	private Coroutine fadeRoutine;

	private static string ReturnedMessage => Localization.UI.BALL_ReturnedFromOutOfBounds;

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

	public static void SetReturned()
	{
		if (SingletonBehaviour<BallStatusMessage>.HasInstance)
		{
			SingletonBehaviour<BallStatusMessage>.Instance.SetReturnedInternal();
		}
	}

	public static void Clear(bool forced)
	{
		if (SingletonBehaviour<BallStatusMessage>.HasInstance)
		{
			SingletonBehaviour<BallStatusMessage>.Instance.ClearInternal(forced);
		}
	}

	private void SetReturnedInternal()
	{
		SetDisplayedStatus(Status.Returned);
	}

	private void ClearInternal(bool forced)
	{
		if (forced || displayedStatus != Status.Returned)
		{
			SetDisplayedStatus(Status.None);
		}
	}

	private void SetDisplayedStatus(Status status, bool suppressMessageUpdate = false)
	{
		if (CourseManager.MatchState == MatchState.Ended && displayedStatus != Status.None)
		{
			ClearInternal(forced: true);
			return;
		}
		Status status2 = displayedStatus;
		displayedStatus = status;
		if (displayedStatus != status2)
		{
			if (!suppressMessageUpdate)
			{
				UpdateMessage();
			}
			if (status == Status.Returned)
			{
				ClearReturnedMessageDelayed();
			}
		}
		async void ClearReturnedMessageDelayed()
		{
			for (float time = 0f; time < returnedMessageDuration; time += Time.deltaTime)
			{
				await UniTask.Yield();
				if (this == null || displayedStatus != Status.Returned)
				{
					return;
				}
			}
			ClearInternal(forced: true);
		}
	}

	private void UpdateMessage()
	{
		if (displayedStatus == Status.None)
		{
			Hide();
			return;
		}
		Show();
		TextMeshProUGUI textMeshProUGUI = message;
		string text = ((displayedStatus != Status.Returned) ? string.Empty : ReturnedMessage);
		textMeshProUGUI.text = text;
	}

	private void Show()
	{
		FadeTo(1f, fadeInDuration, BMath.EaseOut);
	}

	private void Hide()
	{
		FadeTo(0f, fadeInDuration, BMath.EaseIn);
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

	private void OnLocalizationLanguageChanged()
	{
		UpdateMessage();
	}
}
