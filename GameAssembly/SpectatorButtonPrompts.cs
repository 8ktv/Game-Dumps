using System;
using System.Collections;
using UnityEngine;

public class SpectatorButtonPrompts : SingletonBehaviour<SpectatorButtonPrompts>, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private ButtonPrompt cyclePreviousPrompt;

	[SerializeField]
	private ButtonPrompt cycleNextPrompt;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private bool isVisible;

	private bool isUpdateLoopRunning;

	private Coroutine visibilityRoutine;

	protected override void Awake()
	{
		base.Awake();
		cyclePreviousPrompt.Initialize(InputManager.Controls.Spectate.CyclePreviousPlayer, Localization.UI.SPECTATOR_Prompt_Previous_Ref);
		cycleNextPrompt.Initialize(InputManager.Controls.Spectate.CycleNextPlayer, Localization.UI.SPECTATOR_Prompt_Next_Ref);
		visibilityController.SetDesiredAlpha(0f);
		UpdateIsUpdateLoopRunning();
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnLocalPlayerIsSpectatingChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (isUpdateLoopRunning)
		{
			BUpdate.DeregisterCallback(this);
		}
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnLocalPlayerIsSpectatingChanged;
	}

	private void Show()
	{
		FadeTo(1f, fadeInDuration, BMath.EaseOut);
	}

	private void Hide()
	{
		FadeTo(0f, fadeInDuration, BMath.EaseIn);
	}

	public void OnBUpdate()
	{
		bool flag = isVisible;
		isVisible = ShouldBeVisible();
		if (isVisible != flag)
		{
			if (isVisible)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
		static bool ShouldBeVisible()
		{
			if (GameManager.LocalPlayerAsSpectator == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				return false;
			}
			if (!GameManager.LocalPlayerAsSpectator.CanCycleTarget())
			{
				return false;
			}
			return true;
		}
	}

	private void UpdateIsUpdateLoopRunning()
	{
		bool flag = isUpdateLoopRunning;
		isUpdateLoopRunning = ShouldRun();
		if (isUpdateLoopRunning != flag)
		{
			if (isUpdateLoopRunning)
			{
				BUpdate.RegisterCallback(this);
			}
			else
			{
				BUpdate.DeregisterCallback(this);
			}
		}
		static bool ShouldRun()
		{
			if (GameManager.LocalPlayerAsSpectator == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				return false;
			}
			return true;
		}
	}

	private Coroutine FadeTo(float targetAlpha, float duration, Func<float, float> Easing)
	{
		if (visibilityRoutine != null)
		{
			StopCoroutine(visibilityRoutine);
		}
		visibilityRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, Easing));
		return visibilityRoutine;
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

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateIsUpdateLoopRunning();
	}
}
