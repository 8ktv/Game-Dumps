using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartPrompt : SingletonBehaviour<RestartPrompt>
{
	public Image promptIcon;

	public Image promptIconFill;

	public TMP_Text prefixLabel;

	public TMP_Text suffixLabel;

	private UiVisibilityController visibilityController;

	private bool isVisible;

	private float standstillTimer = float.MinValue;

	private const string separator = "{0}";

	protected override void Awake()
	{
		base.Awake();
		visibilityController = GetComponent<UiVisibilityController>();
		InputManager.SwitchedInputDeviceType += UpdatePrompt;
		LocalizationManager.LanguageChanged += UpdateLabel;
		PlayerGolfer.LocalPlayerAheadOfBallChanged += UpdateLabelIfVisible;
		GolfBall.LocalPlayerBallIsHiddenChanged += UpdateLabelIfVisible;
		UpdatePrompt();
		UpdateLabel();
		visibilityController.SetDesiredAlpha(0f);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		InputManager.SwitchedInputDeviceType -= UpdatePrompt;
		LocalizationManager.LanguageChanged -= UpdateLabel;
		PlayerGolfer.LocalPlayerAheadOfBallChanged -= UpdateLabelIfVisible;
		GolfBall.LocalPlayerBallIsHiddenChanged -= UpdateLabelIfVisible;
	}

	public static bool CanBePressed(PlayerInfo player)
	{
		if (player == null)
		{
			return false;
		}
		if (player.isLocalPlayer && !InputManager.Controls.Gameplay.enabled)
		{
			return false;
		}
		if (player.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (!player.AsSpectator.IsSpectating)
		{
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return CourseManager.MatchState >= MatchState.Ongoing;
			}
			return true;
		}
		return false;
	}

	private bool ShouldBeVisible()
	{
		if (!CanBePressed(GameManager.LocalPlayerInfo))
		{
			return false;
		}
		if (CourseManager.MatchState >= MatchState.Ended)
		{
			return false;
		}
		if (!GameManager.LocalPlayerAsGolfer.IsInitialized)
		{
			return false;
		}
		if (GameManager.LocalPlayerMovement.IsRespawning)
		{
			return false;
		}
		if (InputManager.Controls.Gameplay.Restart.IsPressed() && InputManager.Controls.Gameplay.Restart.GetTimeoutCompletionPercentage() < 1f)
		{
			return true;
		}
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && (WouldReturnToBall() || standstillTimer > GameManager.UiSettings.RestartPromptStandStillShowDelay))
		{
			return true;
		}
		return false;
	}

	private void UpdateLabel()
	{
		string text = ((!WouldReturnToBall()) ? Localization.UI.PROMPT_Restart : Localization.UI.PROMPT_ReturnToBall);
		int num = text.IndexOf("{0}");
		prefixLabel.text = ((num >= 0) ? text.Remove(num) : text);
		suffixLabel.text = ((num >= 0) ? text.Substring(num + "{0}".Length) : text);
	}

	private static bool WouldReturnToBall()
	{
		if (GameManager.LocalPlayerAsGolfer == null || GameManager.LocalPlayerAsGolfer.OwnBall == null)
		{
			return false;
		}
		if (GameManager.LocalPlayerAsGolfer.IsAheadOfBall)
		{
			return GameManager.LocalPlayerAsGolfer.OwnBall.IsStationary;
		}
		return false;
	}

	private void UpdatePrompt()
	{
		Image image = promptIcon;
		Sprite sprite = (promptIconFill.sprite = InputManager.GetInputIcon(InputManager.Controls.Gameplay.Restart));
		image.sprite = sprite;
	}

	private void Update()
	{
		bool flag = ShouldBeVisible();
		if (isVisible != flag)
		{
			if (flag)
			{
				visibilityController.AnimatedDesiredAlpha(1f, GameManager.UiSettings.RestartPromptFadeInTime, (float x) => x);
			}
			else
			{
				visibilityController.AnimatedDesiredAlpha(0f, GameManager.UiSettings.RestartPromptFadeOutTime, (float x) => x);
			}
			isVisible = flag;
			if (isVisible)
			{
				UpdateLabel();
			}
		}
		if (isVisible)
		{
			promptIconFill.fillAmount = (InputManager.Controls.Gameplay.Restart.IsPressed() ? InputManager.Controls.Gameplay.Restart.GetTimeoutCompletionPercentage() : 1f);
		}
		if (GameManager.LocalPlayerMovement != null && CourseManager.MatchState >= MatchState.Ongoing && !PauseMenu.IsPaused)
		{
			if (IsLocalPlayerIdle())
			{
				standstillTimer += Time.deltaTime;
			}
			else
			{
				standstillTimer = 0f;
			}
		}
	}

	private bool IsLocalPlayerIdle()
	{
		if (GameManager.LocalPlayerAsGolfer.IsInitialized && !GameManager.LocalPlayerAsGolfer.IsSwinging && !GameManager.LocalPlayerAsGolfer.IsAimingSwing && GameManager.LocalPlayerMovement.IsGrounded && GameManager.LocalPlayerMovement.MoveVectorMagnitude < float.Epsilon)
		{
			return !GameManager.LocalPlayerMovement.IsRespawning;
		}
		return false;
	}

	private void UpdateLabelIfVisible()
	{
		if (ShouldBeVisible())
		{
			UpdateLabel();
		}
	}
}
