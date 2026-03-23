using System;
using System.Collections;
using UnityEngine;

public class FirstToHoleWinsMessageUi : SingletonBehaviour<FirstToHoleWinsMessageUi>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private float minVisibilityDuration;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private bool wasDisplayed;

	private Coroutine fadeRoutine;

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
		CourseManager.CurrentHoleCourseIndexChanged += OnCurrentHoleCourseIndexChanged;
		CourseManager.MatchStateChanged += OnMatchStateChanged;
	}

	private void Start()
	{
		TryDisplay();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CourseManager.CurrentHoleCourseIndexChanged -= OnCurrentHoleCourseIndexChanged;
		CourseManager.MatchStateChanged -= OnMatchStateChanged;
	}

	private void TryDisplay()
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && !wasDisplayed && CourseManager.CurrentHoleCourseIndex == 0 && CourseManager.MatchState == MatchState.TeeOff)
		{
			wasDisplayed = true;
			StartCoroutine(DisplayRoutine());
		}
		IEnumerator DisplayRoutine()
		{
			yield return FadeTo(1f, fadeInDuration, BMath.EaseOut);
			for (float time = 0f; CourseManager.MatchState == MatchState.TeeOff || time < minVisibilityDuration; time += Time.deltaTime)
			{
				yield return null;
			}
			yield return FadeTo(0f, fadeOutDuration, BMath.EaseIn);
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

	private void OnCurrentHoleCourseIndexChanged()
	{
		TryDisplay();
	}

	private void OnMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		TryDisplay();
	}
}
