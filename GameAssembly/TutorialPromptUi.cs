using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class TutorialPromptUi : SingletonBehaviour<TutorialPromptUi>
{
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private VerticalLayoutGroup verticalLayout;

	[SerializeField]
	private LocalizeStringEvent titleLocalizeStringEvent;

	[SerializeField]
	private TextMeshProUGUI contentText;

	[SerializeField]
	private RectTransform progressBarBackground;

	[SerializeField]
	private Image progressBar;

	[SerializeField]
	private int noProgressLayoutBottomPadding;

	[SerializeField]
	private float strikedThroughDuration;

	[SerializeField]
	private float fadeOutDuration;

	[SerializeField]
	private float fadeInDuration;

	private TutorialPrompt displayedPrompt;

	private float desiredDisplayedProgress;

	private int defaultLayoutBottomPadding;

	private bool isProgressBarHidden;

	private bool isFadingOut;

	private Coroutine fadeToRoutine;

	private Coroutine updatePromptAnimatedRoutine;

	public static bool IsFadingOut
	{
		get
		{
			if (SingletonBehaviour<TutorialPromptUi>.HasInstance)
			{
				return SingletonBehaviour<TutorialPromptUi>.Instance.isFadingOut;
			}
			return false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		defaultLayoutBottomPadding = verticalLayout.padding.bottom;
		visibilityController.SetDesiredAlpha(0f);
		LocalizationManager.LanguageChanged += OnLanguageChanged;
		InputManager.SwitchedInputDeviceType += OnSwitchedInputDeviceType;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		InputManager.SwitchedInputDeviceType -= OnSwitchedInputDeviceType;
	}

	public static void SetPrompt(TutorialPrompt prompt)
	{
		if (SingletonBehaviour<TutorialPromptUi>.HasInstance)
		{
			SingletonBehaviour<TutorialPromptUi>.Instance.SetPromptInternal(prompt);
		}
	}

	public static void SetPromptNormalizedProgress(float normalizedProgress)
	{
		if (SingletonBehaviour<TutorialPromptUi>.HasInstance)
		{
			SingletonBehaviour<TutorialPromptUi>.Instance.SetPromptNormalizedProgressInternal(normalizedProgress);
		}
	}

	private void SetPromptInternal(TutorialPrompt prompt)
	{
		if (prompt != displayedPrompt)
		{
			TutorialPrompt num = displayedPrompt;
			displayedPrompt = prompt;
			if (num == TutorialPrompt.None)
			{
				UpdatePromptText();
				UpdateIsProgressBarHidden();
				FadeTo(1f, fadeInDuration, BMath.EaseOut);
			}
			else if (displayedPrompt == TutorialPrompt.None)
			{
				StrikeThroughContentText();
				FadeTo(0f, fadeOutDuration, BMath.EaseOut);
			}
			else
			{
				UpdatePromptAnimated();
			}
		}
		void StrikeThroughContentText()
		{
			contentText.text = "<s>" + contentText.text + "</s>";
		}
		void UpdatePromptAnimated()
		{
			if (updatePromptAnimatedRoutine != null)
			{
				StopCoroutine(updatePromptAnimatedRoutine);
			}
			updatePromptAnimatedRoutine = StartCoroutine(UpdatePromptAnimatedRoutine());
		}
		IEnumerator UpdatePromptAnimatedRoutine()
		{
			ApplyIsProgressBarHidden();
			isFadingOut = true;
			StrikeThroughContentText();
			yield return new WaitForSeconds(strikedThroughDuration);
			yield return FadeTo(0f, fadeOutDuration, BMath.EaseOut);
			isFadingOut = false;
			ApplyProgress();
			UpdatePromptText();
			UpdateIsProgressBarHidden();
			yield return FadeTo(1f, fadeInDuration, BMath.EaseOut);
		}
		void UpdatePromptText()
		{
			UpdateTitleText();
			UpdateContentText();
		}
		void UpdateTitleText()
		{
			titleLocalizeStringEvent.StringReference = LocalizationManager.GetLocalizedString(StringTable.UI, $"TUTORIAL_PROMPT_{prompt}_Title");
		}
	}

	private void SetPromptNormalizedProgressInternal(float normalizedProgress)
	{
		desiredDisplayedProgress = normalizedProgress;
		if (!isFadingOut)
		{
			ApplyProgress();
		}
	}

	private void UpdateContentText()
	{
		contentText.text = GetContentText(displayedPrompt);
		static string GetContentText(TutorialPrompt prompt)
		{
			return prompt switch
			{
				TutorialPrompt.LookAround => string.Format(Localization.UI.TUTORIAL_PROMPT_LookAround_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Camera.Look), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.Move => string.Format(Localization.UI.TUTORIAL_PROMPT_Move_Content, InputManager.GetCompositeInputIconRichTextTags(InputManager.Controls.Gameplay.Move, string.Empty, canMerge: true), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.Jump => string.Format(Localization.UI.TUTORIAL_PROMPT_Jump_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Jump), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.Dive => string.Format(Localization.UI.TUTORIAL_PROMPT_Dive_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Dive), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.Interact => string.Format(Localization.UI.TUTORIAL_PROMPT_Interact_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Interact), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.AimSwing => string.Format(Localization.UI.TUTORIAL_PROMPT_AimSwing_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Aim), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.AdjustAngle => string.Format(InputManager.UsingKeyboard ? Localization.UI.TUTORIAL_PROMPT_AdjustAngle_Content_Keyboard : Localization.UI.TUTORIAL_PROMPT_AdjustAngle_Content_Controller, InputManager.UsingKeyboard ? InputManager.GetCompositeInputIconRichTextTags(InputManager.Controls.Gameplay.Pitch, string.Empty, canMerge: true) : InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.CycleSwingAngle), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.OptimalAngle => string.Format(InputManager.UsingKeyboard ? Localization.UI.TUTORIAL_PROMPT_OptimalAngle_Content_Keyboard : Localization.UI.TUTORIAL_PROMPT_OptimalAngle_Content_Controller, InputManager.UsingKeyboard ? InputManager.GetCompositeInputIconRichTextTags(InputManager.Controls.Gameplay.Pitch, string.Empty, canMerge: true) : InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.CycleSwingAngle), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.ChargeSwing => string.Format(Localization.UI.TUTORIAL_PROMPT_ChargeSwing_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Swing), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.HomingShot => string.Format(Localization.UI.TUTORIAL_PROMPT_HomingShot_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Swing), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.CancelSwing => string.Format(Localization.UI.TUTORIAL_PROMPT_CancelSwing_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.Cancel), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.SelectItem => string.Format(InputManager.UsingKeyboard ? Localization.UI.TUTORIAL_PROMPT_SelectItem_Content_Keyboard : Localization.UI.TUTORIAL_PROMPT_SelectItem_Content_Controller, InputManager.UsingKeyboard ? GetInventoryHotkeyIconRichTextTags() : GetInventoryCycleIconRichTextTags(), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.DropItem => string.Format(Localization.UI.TUTORIAL_PROMPT_DropItem_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.DropItem), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.Putt => string.Format(InputManager.UsingKeyboard ? Localization.UI.TUTORIAL_PROMPT_Putt_Content_Keyboard : Localization.UI.TUTORIAL_PROMPT_Putt_Content_Controller, InputManager.UsingKeyboard ? InputManager.GetCompositeInputIconRichTextTags(InputManager.Controls.Gameplay.Pitch, string.Empty, canMerge: true) : InputManager.GetInputIconRichTextTag(InputManager.Controls.Gameplay.CycleSwingAngle), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				TutorialPrompt.ViewScore => string.Format(Localization.UI.TUTORIAL_PROMPT_ViewScore_Content, InputManager.GetInputIconRichTextTag(InputManager.Controls.Ingame.ShowScoreboard), GameManager.UiSettings.TextHighlightStartTag, GameManager.UiSettings.TextColorEndTag), 
				_ => null, 
			};
		}
		static string GetInventoryCycleIconRichTextTags()
		{
			return InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.CycleLeft) + InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.CycleRight);
		}
		static string GetInventoryHotkeyIconRichTextTags()
		{
			return InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.Hotkey1) + InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.Hotkey2) + InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.Hotkey3) + InputManager.GetInputIconRichTextTag(InputManager.Controls.Hotkeys.Hotkey4);
		}
	}

	private void UpdateIsProgressBarHidden()
	{
		bool flag = isProgressBarHidden;
		isProgressBarHidden = ShouldBeHidden();
		if (isProgressBarHidden != flag)
		{
			if (isProgressBarHidden)
			{
				progressBarBackground.gameObject.SetActive(value: false);
				verticalLayout.padding.bottom = noProgressLayoutBottomPadding;
			}
			else
			{
				progressBarBackground.gameObject.SetActive(value: true);
				verticalLayout.padding.bottom = defaultLayoutBottomPadding;
			}
		}
		bool ShouldBeHidden()
		{
			return (displayedPrompt & TutorialPrompt.HasProgress) == 0;
		}
	}

	private void ApplyIsProgressBarHidden()
	{
		if (isProgressBarHidden)
		{
			progressBarBackground.gameObject.SetActive(value: false);
			verticalLayout.padding.bottom = noProgressLayoutBottomPadding;
		}
		else
		{
			progressBarBackground.gameObject.SetActive(value: true);
			verticalLayout.padding.bottom = defaultLayoutBottomPadding;
		}
		LayoutRebuilder.MarkLayoutForRebuild(verticalLayout.transform as RectTransform);
		LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayout.transform as RectTransform);
		Canvas.ForceUpdateCanvases();
	}

	private void ApplyProgress()
	{
		progressBar.fillAmount = desiredDisplayedProgress;
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
				if (num <= 0f)
				{
					isFadingOut = true;
				}
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
				isFadingOut = false;
				ApplyProgress();
			}
		}
	}

	private void OnLanguageChanged()
	{
		UpdateContentText();
	}

	private void OnSwitchedInputDeviceType()
	{
		UpdateContentText();
	}
}
