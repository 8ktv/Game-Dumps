using System;
using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class TutorialObjectiveUi : SingletonBehaviour<TutorialObjectiveUi>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private TextMeshProUGUI objectiveText;

	[SerializeField]
	private LocalizeStringEvent objectiveLocalizeStringEvent;

	[SerializeField]
	private float strikedThroughDuration;

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float textFadeOutDuration;

	[SerializeField]
	private float textFadeInDuration;

	private TutorialObjective displayedObjective;

	private Coroutine fadeToRoutine;

	private Coroutine fadeTextToRoutine;

	private Coroutine updateObjectiveAnimatedRoutine;

	public static TutorialObjective DisplayedObjective
	{
		get
		{
			if (!SingletonBehaviour<TutorialObjectiveUi>.HasInstance)
			{
				return TutorialObjective.None;
			}
			return SingletonBehaviour<TutorialObjectiveUi>.Instance.displayedObjective;
		}
	}

	public static event Action ObjectiveTextUpdated;

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
	}

	public static void SetObjective(TutorialObjective objective)
	{
		if (SingletonBehaviour<TutorialObjectiveUi>.HasInstance)
		{
			SingletonBehaviour<TutorialObjectiveUi>.Instance.SetObjectiveInternal(objective);
		}
	}

	private void SetObjectiveInternal(TutorialObjective objective)
	{
		if (objective != displayedObjective)
		{
			TutorialObjective num = displayedObjective;
			displayedObjective = objective;
			if (num == TutorialObjective.None)
			{
				UpdateObjectiveText();
				FadeTo(1f, fadeInDuration, BMath.EaseOut);
				FadeTextTo(1f, textFadeInDuration, BMath.EaseOut);
			}
			else if (displayedObjective == TutorialObjective.None)
			{
				StrikeThroughObjectiveText();
				FadeTo(0f, fadeOutDuration, BMath.EaseIn);
			}
			else
			{
				UpdateObjectiveAnimated();
			}
		}
		void StrikeThroughObjectiveText()
		{
			objectiveText.text = "<s>" + objectiveText.text + "</s>";
		}
		void UpdateObjectiveAnimated()
		{
			if (updateObjectiveAnimatedRoutine != null)
			{
				StopCoroutine(updateObjectiveAnimatedRoutine);
			}
			updateObjectiveAnimatedRoutine = StartCoroutine(UpdateObjectiveAnimatedRoutine());
		}
		IEnumerator UpdateObjectiveAnimatedRoutine()
		{
			StrikeThroughObjectiveText();
			yield return new WaitForSeconds(strikedThroughDuration);
			yield return FadeTextTo(0f, textFadeOutDuration, BMath.EaseIn);
			UpdateObjectiveText();
			yield return FadeTextTo(1f, textFadeInDuration, BMath.EaseOut);
		}
		void UpdateObjectiveText()
		{
			LocalizedString localizedString = ((displayedObjective != TutorialObjective.StartMatch) ? LocalizationManager.GetLocalizedString(StringTable.UI, $"TUTORIAL_OBJECTIVE_{displayedObjective}") : (NetworkServer.active ? Localization.UI.TUTORIAL_OBJECTIVE_StartMatchHost_Ref : Localization.UI.TUTORIAL_OBJECTIVE_StartMatchNonHost_Ref));
			LocalizedString localizedString2 = localizedString;
			localizedString2.GetLocalizedString();
			objectiveLocalizeStringEvent.StringReference = localizedString2;
			TutorialObjectiveUi.ObjectiveTextUpdated?.Invoke();
		}
	}

	private Coroutine FadeTo(float targetAlpha, float duration, Func<float, float> Easing)
	{
		if (fadeToRoutine != null)
		{
			StopCoroutine(fadeToRoutine);
		}
		fadeToRoutine = StartCoroutine(FadeToRoutine(targetAlpha, duration, Easing));
		return fadeToRoutine;
		IEnumerator FadeToRoutine(float num, float num2, Func<float, float> func)
		{
			if (visibilityController.DesiredAlpha != num)
			{
				float initialAlpha = visibilityController.DesiredAlpha;
				for (float time = 0f; time < num2; time += Time.deltaTime)
				{
					float arg = time / num2;
					float t = func(arg);
					float desiredAlpha = BMath.Lerp(initialAlpha, num, t);
					visibilityController.SetDesiredAlpha(desiredAlpha);
					yield return null;
				}
				visibilityController.SetDesiredAlpha(num);
			}
		}
	}

	private Coroutine FadeTextTo(float targetAlpha, float duration, Func<float, float> Easing)
	{
		if (fadeTextToRoutine != null)
		{
			StopCoroutine(fadeTextToRoutine);
		}
		fadeTextToRoutine = StartCoroutine(FadeToRoutine(targetAlpha, duration, Easing));
		return fadeTextToRoutine;
		IEnumerator FadeToRoutine(float num, float num2, Func<float, float> func)
		{
			if (objectiveText.alpha != num)
			{
				float initialAlpha = objectiveText.alpha;
				for (float time = 0f; time < num2; time += Time.deltaTime)
				{
					float arg = time / num2;
					float t = func(arg);
					float alpha = BMath.Lerp(initialAlpha, num, t);
					objectiveText.alpha = alpha;
					yield return null;
				}
				objectiveText.alpha = num;
			}
		}
	}
}
