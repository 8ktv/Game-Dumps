using System;
using UnityEngine;

public class TutorialManager : SingletonBehaviour<TutorialManager>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private TutorialSettings settings;

	private TutorialObjective activeObjective;

	private TutorialPromptCategory activePromptCategory;

	private TutorialPrompt activePrompt;

	private float activePromptNormalizedProgress;

	private TutorialPromptCategory allowedPromptCategories;

	private bool isFinished;

	public static TutorialSettings Settings
	{
		get
		{
			if (!SingletonBehaviour<TutorialManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<TutorialManager>.Instance.settings;
		}
	}

	public static TutorialObjective ActiveObjective
	{
		get
		{
			if (!SingletonBehaviour<TutorialManager>.HasInstance)
			{
				return TutorialObjective.None;
			}
			return SingletonBehaviour<TutorialManager>.Instance.activeObjective;
		}
	}

	public static bool IsFinished
	{
		get
		{
			if (SingletonBehaviour<TutorialManager>.HasInstance)
			{
				return SingletonBehaviour<TutorialManager>.Instance.isFinished;
			}
			return false;
		}
	}

	public static event Action<TutorialObjective, TutorialObjective> ObjectiveChanged;

	public static event Action IsFinishedChanged;

	private void Start()
	{
		AllowPromptCategoryInternal(TutorialPromptCategory.Basics);
		UpdateActiveObjective(suppressIsFinishedUpdate: true);
		UpdateActivePrompt(suppressIsFinishedUpdate: true);
		UpdateIsFinished();
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			BUpdate.RegisterCallback(this);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
	}

	public void OnLateBUpdate()
	{
		UpdateContinuouslyCheckedCategories();
		UpdatePromptProgress();
		static bool ShouldGreenCategoryBeAllowed()
		{
			if (GameManager.LocalPlayerAsGolfer == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.OwnBall == null)
			{
				return false;
			}
			if (!GameManager.LocalPlayerInfo.LevelBoundsTracker.AuthoritativeIsOnGreen)
			{
				return false;
			}
			if (!GameManager.LocalPlayerAsGolfer.OwnBall.AsEntity.LevelBoundsTracker.AuthoritativeIsOnGreen)
			{
				return false;
			}
			return true;
		}
		void UpdateContinuouslyCheckedCategories()
		{
			if (ShouldGreenCategoryBeAllowed())
			{
				AllowPromptCategoryInternal(TutorialPromptCategory.Green);
			}
			else
			{
				DisallowPromptCategoryInternal(TutorialPromptCategory.Green);
			}
		}
		void UpdatePromptProgress()
		{
			if (CanProgressPrompt())
			{
				switch (activePrompt)
				{
				case TutorialPrompt.LookAround:
				{
					if (CameraModuleController.TryGetOrbitModule(out var orbitModule) && orbitModule.RotationSpeed > 45f)
					{
						SetPromptNormalizedProgress(activePromptNormalizedProgress + Time.deltaTime / settings.LookAroundRequiredDuration);
						if (activePromptNormalizedProgress >= 1f)
						{
							CompletePromptInternal(activePrompt, suppressIsFinishedUpdate: false, forced: false);
						}
					}
					break;
				}
				case TutorialPrompt.Move:
					if (GameManager.LocalPlayerMovement != null && GameManager.LocalPlayerMovement.MoveVectorMagnitude > 0.25f)
					{
						SetPromptNormalizedProgress(activePromptNormalizedProgress + Time.deltaTime / settings.MoveRequiredDuration);
						if (activePromptNormalizedProgress >= 1f)
						{
							CompletePromptInternal(activePrompt, suppressIsFinishedUpdate: false, forced: false);
						}
					}
					break;
				case TutorialPrompt.AimSwing:
					if (GameManager.LocalPlayerAsGolfer != null && GameManager.LocalPlayerAsGolfer.IsAimingSwing && GameManager.LocalPlayerInfo.Input.IsHoldingAimSwing)
					{
						SetPromptNormalizedProgress(activePromptNormalizedProgress + Time.deltaTime / settings.AimSwingRequiredDuration);
						if (activePromptNormalizedProgress >= 1f)
						{
							CompletePromptInternal(activePrompt, suppressIsFinishedUpdate: false, forced: false);
						}
					}
					break;
				case TutorialPrompt.ChargeSwing:
				{
					bool flag2 = GameManager.LocalPlayerAsGolfer != null && GameManager.LocalPlayerAsGolfer.IsChargingSwing && GameManager.LocalPlayerAsGolfer.SwingNormalizedCharge <= 1f;
					SetPromptNormalizedProgress(flag2 ? (GameManager.LocalPlayerAsGolfer.SwingNormalizedCharge / settings.ChargeSwingMinimumSwingNormalizedPower) : 0f);
					break;
				}
				case TutorialPrompt.HomingShot:
				{
					bool flag2 = GameManager.LocalPlayerAsGolfer != null && GameManager.LocalPlayerAsGolfer.IsChargingSwing;
					SetPromptNormalizedProgress(flag2 ? GameManager.LocalPlayerAsGolfer.SwingNormalizedCharge : 0f);
					break;
				}
				case TutorialPrompt.Putt:
				{
					bool flag = GameManager.LocalPlayerAsGolfer != null && GameManager.LocalPlayerAsGolfer.IsChargingSwing && GameManager.LocalPlayerAsGolfer.SwingPitch <= 0f;
					SetPromptNormalizedProgress(flag ? (GameManager.LocalPlayerAsGolfer.SwingNormalizedCharge / settings.PuttMinimumSwingNormalizedPower) : 0f);
					break;
				}
				case TutorialPrompt.ViewScore:
					if (Scoreboard.IsVisible)
					{
						SetPromptNormalizedProgress(activePromptNormalizedProgress + Time.deltaTime / settings.ViewScoreRequiredDuration);
						if (activePromptNormalizedProgress >= 1f)
						{
							CompletePromptInternal(activePrompt, suppressIsFinishedUpdate: false, forced: false);
						}
					}
					break;
				}
			}
		}
	}

	public static void CompleteObjective(TutorialObjective objective)
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.CompleteObjectiveInternal(objective, suppressIsFinishedUpdate: false, forced: false);
		}
	}

	public static void CompletePrompt(TutorialPrompt prompt)
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.CompletePromptInternal(prompt, suppressIsFinishedUpdate: false, forced: false);
		}
	}

	public static void AllowPromptCategory(TutorialPromptCategory category)
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.AllowPromptCategoryInternal(category);
		}
	}

	public static void DisallowPromptCategory(TutorialPromptCategory category)
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.DisallowPromptCategoryInternal(category);
		}
	}

	public static void FinishInstantly()
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.FinishInstantlyInternal();
		}
	}

	public static void ResetTutorial()
	{
		if (SingletonBehaviour<TutorialManager>.HasInstance)
		{
			SingletonBehaviour<TutorialManager>.Instance.ResetTutorialInternal();
		}
	}

	private void CompleteObjectiveInternal(TutorialObjective objective, bool suppressIsFinishedUpdate, bool forced)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance && objective.HasObjective(activeObjective))
		{
			GameSettings.All.TutorialProgress.TryCompleteObjective(activeObjective);
			if (forced)
			{
				GameSettings.All.TutorialProgress.TryCompleteObjective(objective);
			}
			UpdateActiveObjective(suppressIsFinishedUpdate);
			if (!suppressIsFinishedUpdate)
			{
				UpdateIsFinished();
			}
		}
	}

	private void CompletePromptInternal(TutorialPrompt prompt, bool suppressIsFinishedUpdate, bool forced)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance && prompt.HasPrompt(activePrompt) && CanProgressPrompt())
		{
			GameSettings.All.TutorialProgress.TryCompletePrompt(activePrompt);
			if (forced)
			{
				GameSettings.All.TutorialProgress.TryCompletePrompt(prompt);
			}
			UpdateActivePrompt(suppressIsFinishedUpdate);
			if (!suppressIsFinishedUpdate)
			{
				UpdateIsFinished();
			}
		}
	}

	private void AllowPromptCategoryInternal(TutorialPromptCategory category)
	{
		allowedPromptCategories |= category;
		UpdateActivePrompt(suppressIsFinishedUpdate: false);
	}

	private void DisallowPromptCategoryInternal(TutorialPromptCategory category)
	{
		allowedPromptCategories &= ~category;
		UpdateActivePrompt(suppressIsFinishedUpdate: false);
	}

	private void FinishInstantlyInternal()
	{
		CompleteObjectiveInternal((TutorialObjective)(-1), suppressIsFinishedUpdate: true, forced: true);
		CompletePromptInternal((TutorialPrompt)(-1), suppressIsFinishedUpdate: true, forced: true);
		UpdateIsFinished();
	}

	private void ResetTutorialInternal()
	{
		GameSettings.All.TutorialProgress.Clear();
		ResetObjective();
		UpdateActiveObjective(suppressIsFinishedUpdate: true);
		ResetPrompts();
		UpdateActivePrompt(suppressIsFinishedUpdate: true);
		UpdateIsFinished();
		void ResetObjective()
		{
			activeObjective = TutorialObjective.None;
		}
		void ResetPrompts()
		{
			activePromptCategory = TutorialPromptCategory.None;
			activePrompt = TutorialPrompt.None;
		}
	}

	private void UpdateActiveObjective(bool suppressIsFinishedUpdate)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			activeObjective = TutorialObjective.None;
			TutorialObjectiveUi.SetObjective(activeObjective);
			return;
		}
		GameSettings.TutorialProgress tutorialProgress = GameSettings.All.TutorialProgress;
		TutorialObjective tutorialObjective = activeObjective;
		activeObjective = GetCurrentObjective();
		if (activeObjective != tutorialObjective)
		{
			if (!suppressIsFinishedUpdate)
			{
				UpdateIsFinished();
			}
			TutorialObjectiveUi.SetObjective(activeObjective);
			TutorialManager.ObjectiveChanged?.Invoke(tutorialObjective, activeObjective);
		}
		TutorialObjective GetCurrentObjective()
		{
			foreach (TutorialObjective value in Enum.GetValues(typeof(TutorialObjective)))
			{
				if (value != TutorialObjective.None && !tutorialProgress.CompletedObjectives.HasObjective(value))
				{
					return value;
				}
			}
			return TutorialObjective.None;
		}
	}

	private void UpdateActivePrompt(bool suppressIsFinishedUpdate)
	{
		if (!SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			activePromptCategory = TutorialPromptCategory.None;
			activePrompt = TutorialPrompt.None;
			TutorialPromptUi.SetPrompt(activePrompt);
			SetPromptNormalizedProgress(0f);
			return;
		}
		GameSettings.TutorialProgress tutorialProgress = GameSettings.All.TutorialProgress;
		TutorialPrompt tutorialPrompt = activePrompt;
		activePrompt = GetNextPrompt();
		if (activePrompt != tutorialPrompt)
		{
			if (!suppressIsFinishedUpdate)
			{
				UpdateIsFinished();
			}
			TutorialPromptUi.SetPrompt(activePrompt);
			SetPromptNormalizedProgress(0f);
		}
		TutorialPrompt GetNextPrompt()
		{
			if (activePromptCategory == TutorialPromptCategory.None)
			{
				if (!TryGetNextCategory(activePromptCategory, out var nextCategory))
				{
					return TutorialPrompt.None;
				}
				activePromptCategory = nextCategory;
			}
			while (activePromptCategory != TutorialPromptCategory.None)
			{
				if (settings.categorizedPrompts.TryGetValue(activePromptCategory, out var value))
				{
					TutorialPrompt[] array = value;
					foreach (TutorialPrompt tutorialPrompt2 in array)
					{
						if (!tutorialProgress.CompletedPrompts.HasPrompt(tutorialPrompt2))
						{
							return tutorialPrompt2;
						}
					}
				}
				if (!TryGetNextCategory(activePromptCategory, out var nextCategory2))
				{
					activePromptCategory = TutorialPromptCategory.None;
					return TutorialPrompt.None;
				}
				activePromptCategory = nextCategory2;
			}
			activePromptCategory = TutorialPromptCategory.None;
			return TutorialPrompt.None;
		}
		bool TryGetNextCategory(TutorialPromptCategory currentCategory, out TutorialPromptCategory nextCategory)
		{
			if (currentCategory == TutorialPromptCategory.None)
			{
				nextCategory = TutorialPromptCategory.Basics;
			}
			else
			{
				nextCategory = (TutorialPromptCategory)((int)currentCategory << 1);
			}
			if (!settings.categorizedPrompts.TryGetValue(nextCategory, out var value))
			{
				return false;
			}
			if (!allowedPromptCategories.HasCategory(nextCategory))
			{
				bool result = true;
				TutorialPrompt[] array = value;
				foreach (TutorialPrompt promptToCheck in array)
				{
					if (!tutorialProgress.CompletedPrompts.HasPrompt(promptToCheck))
					{
						result = false;
						break;
					}
				}
				return result;
			}
			return true;
		}
	}

	private void SetPromptNormalizedProgress(float normalizedProgress)
	{
		activePromptNormalizedProgress = BMath.Clamp01(normalizedProgress);
		TutorialPromptUi.SetPromptNormalizedProgress(activePromptNormalizedProgress);
	}

	private void UpdateIsFinished()
	{
		bool flag = isFinished;
		isFinished = ShouldBeFinished();
		if (isFinished != flag)
		{
			if (isFinished)
			{
				GameManager.AchievementsManager.Unlock(AchievementId.ReadyForTheBigLeagues);
			}
			TutorialManager.IsFinishedChanged?.Invoke();
		}
		static bool ShouldBeFinished()
		{
			GameSettings.TutorialProgress tutorialProgress = GameSettings.All.TutorialProgress;
			foreach (TutorialObjective value in Enum.GetValues(typeof(TutorialObjective)))
			{
				if (value != TutorialObjective.None && !tutorialProgress.CompletedObjectives.HasObjective(value))
				{
					return false;
				}
			}
			foreach (TutorialPrompt value2 in Enum.GetValues(typeof(TutorialPrompt)))
			{
				if (value2 != TutorialPrompt.None && !tutorialProgress.CompletedPrompts.HasPrompt(value2))
				{
					return false;
				}
			}
			return true;
		}
	}

	private bool CanProgressPrompt()
	{
		return !TutorialPromptUi.IsFadingOut;
	}
}
