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
		visibilityController.AnimatedDesiredAlpha(1f, fadeInDuration, BMath.EaseOut);
	}

	private void Hide()
	{
		visibilityController.AnimatedDesiredAlpha(0f, fadeInDuration, BMath.EaseIn);
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
		if (isUpdateLoopRunning == flag)
		{
			return;
		}
		if (isUpdateLoopRunning)
		{
			BUpdate.RegisterCallback(this);
			return;
		}
		BUpdate.DeregisterCallback(this);
		if (isVisible)
		{
			Hide();
			isVisible = false;
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

	private void OnLocalPlayerIsSpectatingChanged()
	{
		UpdateIsUpdateLoopRunning();
	}
}
