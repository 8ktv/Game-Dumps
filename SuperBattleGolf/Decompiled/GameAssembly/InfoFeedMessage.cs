using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoFeedMessage : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private TextMeshProUGUI preIconText;

	[SerializeField]
	private Image icon1;

	[SerializeField]
	private Image icon2;

	[SerializeField]
	private TextMeshProUGUI postIconText;

	[SerializeField]
	private GameObject stripes;

	private RectTransform rectTransform;

	private float fullHeight;

	private InfoFeed.IMessageData messageData;

	private float timeSinceAppeared;

	public bool IsFadingOut { get; private set; }

	private void Awake()
	{
		rectTransform = base.transform as RectTransform;
		fullHeight = rectTransform.sizeDelta.y;
	}

	public void Initialize(InfoFeed.IMessageData messageData)
	{
		this.messageData = messageData;
		timeSinceAppeared = 0f;
		canvasGroup.alpha = 1f;
		base.transform.SetAsFirstSibling();
		stripes.SetActive(value: false);
		RefreshMessage(fromInitialization: true);
		SlideIn();
	}

	public void OnUpdate()
	{
		if (!IsFadingOut)
		{
			timeSinceAppeared += Time.deltaTime;
			if (timeSinceAppeared >= InfoFeed.MessageDuration)
			{
				FadeOut();
			}
		}
	}

	public void RefreshMessage()
	{
		RefreshMessage(fromInitialization: false);
	}

	private void RefreshMessage(bool fromInitialization)
	{
		if (messageData is InfoFeed.EliminationMessaqeData eliminationMessageData)
		{
			RefreshEliminationMessage(eliminationMessageData);
		}
		else if (messageData is InfoFeed.SelfEliminationMessageData eliminationMessageData2)
		{
			RefreshSelfEliminationMessage(eliminationMessageData2);
		}
		else if (messageData is InfoFeed.KnockoutMessageData eliminationMessageData3)
		{
			RefreshKnockoutMessage(eliminationMessageData3);
		}
		else if (messageData is InfoFeed.SelfKnockoutMessageData eliminationMessageData4)
		{
			RefreshSelfKnockoutMessage(eliminationMessageData4);
		}
		else if (messageData is InfoFeed.FinishedHoleMessageData finishedHoleMessageData)
		{
			RefreshFinishedHoleMessage(finishedHoleMessageData);
		}
		else if (messageData is InfoFeed.ScoredOnDrivingRangeMessageData scoredOnDrivingRangeMessageData)
		{
			RefreshScoredOnDrivingRangeMessage(scoredOnDrivingRangeMessageData);
		}
		else if (messageData is InfoFeed.StrokesMessageData strokesMessageData)
		{
			RefreshStrokesMessage(strokesMessageData);
		}
		else if (messageData is InfoFeed.ChipInMessageData chipInMessageData)
		{
			RefreshChipInMessage(chipInMessageData);
		}
		else if (messageData is InfoFeed.SpeedrunMessageData speedrunMessageData)
		{
			RefreshSpeedrunMessage(speedrunMessageData);
		}
		else if (messageData is InfoFeed.DominatingMessageData dominatingMessageData)
		{
			RefreshDominatingMessage(dominatingMessageData);
		}
		else if (messageData is InfoFeed.RevengeMessageData revengeMessageData)
		{
			RefreshRevengeMessage(revengeMessageData);
		}
		else if (messageData is InfoFeed.ComebackMessageData comebackMessageData)
		{
			RefreshComebackMessage(comebackMessageData);
		}
		else
		{
			Debug.LogError($"Invalid info feed message data type: {messageData.GetType()}", base.gameObject);
		}
		static string GetCurrentLocalizedChippedInString()
		{
			return GameSettings.All.General.DistanceUnits switch
			{
				GameSettings.GeneralSettings.DistanceUnit.Meters => Localization.UI.INFO_FEED_ChipIn, 
				GameSettings.GeneralSettings.DistanceUnit.Yards => Localization.UI.INFO_FEED_ChipIn_Yards, 
				_ => Localization.UI.INFO_FEED_ChipIn, 
			};
		}
		void OnRefreshedMessage()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
		void RefreshChipInMessage(InfoFeed.ChipInMessageData chipInMessageData2)
		{
			string text = string.Format(GetCurrentLocalizedChippedInString(), GameSettings.All.General.GetDistanceInCurrentUnits(chipInMessageData2.distance));
			string text2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(chipInMessageData2.playerGuid), InfoFeed.Player0Color) + " " + text;
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text2;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshComebackMessage(InfoFeed.ComebackMessageData comebackMessageData2)
		{
			string text = string.Format(Localization.UI.INFO_FEED_Comeback, comebackMessageData2.comebackBonus);
			string text2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(comebackMessageData2.playerGuid), InfoFeed.Player0Color) + " " + text;
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text2;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshDominatingMessage(InfoFeed.DominatingMessageData dominatingMessageData2)
		{
			string format = GameManager.UiSettings.ApplyColorTag(Localization.UI.DOMINATION_IsDominating, TextHighlight.Red);
			string arg = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(dominatingMessageData2.dominatedPlayerGuid), InfoFeed.Player1Color);
			string arg2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(dominatingMessageData2.dominatingPlayerGuid), InfoFeed.Player0Color);
			string text = string.Format(format, arg2, arg);
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshEliminationMessage(InfoFeed.EliminationMessaqeData eliminationMessaqeData)
		{
			if (fromInitialization)
			{
				string preIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(eliminationMessaqeData.responsiblePlayer), InfoFeed.Player0Color);
				string postIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(eliminationMessaqeData.knockedOutPlayer), InfoFeed.Player1Color);
				RefreshGenericMessage(preIconTextStr, postIconTextStr, eliminationMessaqeData.icon, InfoFeedIconType.Elimination);
			}
		}
		void RefreshFinishedHoleMessage(InfoFeed.FinishedHoleMessageData finishedHoleMessageData2)
		{
			string arg = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(finishedHoleMessageData2.playerGuid), InfoFeed.Player0Color);
			string text = ((finishedHoleMessageData2.displayPlacement > 8) ? string.Format(LocalizationManager.GetString(StringTable.UI, "INFO_FEED_FinishedHigh"), arg, finishedHoleMessageData2.displayPlacement) : string.Format(LocalizationManager.GetString(StringTable.UI, $"INFO_FEED_Finished{finishedHoleMessageData2.displayPlacement}"), arg));
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshGenericMessage(string preIconTextStr, string postIconTextStr, InfoFeedIconType iconType1, InfoFeedIconType iconType2)
		{
			bool flag = !string.IsNullOrEmpty(preIconTextStr);
			preIconText.gameObject.SetActive(flag);
			if (flag)
			{
				preIconText.text = preIconTextStr;
			}
			bool flag2 = iconType1 != InfoFeedIconType.None;
			icon1.gameObject.SetActive(flag2);
			if (flag2)
			{
				if (InfoFeed.IconSettings.TryGetIcon(iconType1, out var icon))
				{
					SetIcon(icon1, icon);
				}
				else
				{
					Debug.LogError($"Attempted to display an info feed message icon of type {iconType1}, but it doesn't exist", base.gameObject);
				}
			}
			bool flag3 = iconType2 != InfoFeedIconType.None;
			this.icon2.gameObject.SetActive(flag3);
			if (flag3)
			{
				if (InfoFeed.IconSettings.TryGetIcon(iconType2, out var icon2))
				{
					SetIcon(this.icon2, icon2);
				}
				else
				{
					Debug.LogError($"Attempted to display an info feed message icon of type {iconType2}, but it doesn't exist", base.gameObject);
				}
			}
			bool flag4 = !string.IsNullOrEmpty(postIconTextStr);
			postIconText.gameObject.SetActive(flag4);
			if (flag4)
			{
				postIconText.text = postIconTextStr;
			}
			OnRefreshedMessage();
		}
		void RefreshKnockoutMessage(InfoFeed.KnockoutMessageData knockoutMessageData)
		{
			if (fromInitialization)
			{
				string preIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(knockoutMessageData.responsiblePlayer), InfoFeed.Player0Color);
				string postIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(knockoutMessageData.knockedOutPlayer), InfoFeed.Player1Color);
				RefreshGenericMessage(preIconTextStr, postIconTextStr, knockoutMessageData.icon, InfoFeedIconType.None);
			}
		}
		void RefreshRevengeMessage(InfoFeed.RevengeMessageData revengeMessageData2)
		{
			string format = GameManager.UiSettings.ApplyColorTag(Localization.UI.DOMINATION_GotRevenge, TextHighlight.Red);
			string arg = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(revengeMessageData2.previouslyDominatedPlayerGuid), InfoFeed.Player0Color);
			string arg2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(revengeMessageData2.previouslyDominatingPlayerGuid), InfoFeed.Player1Color);
			string text = string.Format(format, arg, arg2);
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshScoredOnDrivingRangeMessage(InfoFeed.ScoredOnDrivingRangeMessageData scoredOnDrivingRangeMessageData2)
		{
			string text = string.Format(LocalizationManager.GetString(StringTable.UI, "INFO_FEED_ScoredOnDrivingRange"), GameManager.RichTextNoParse(CourseManager.GetPlayerName(scoredOnDrivingRangeMessageData2.playerGuid)));
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshSelfEliminationMessage(InfoFeed.SelfEliminationMessageData selfEliminationMessageData)
		{
			if (fromInitialization)
			{
				string preIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(selfEliminationMessageData.playerGuid), InfoFeed.Player1Color);
				RefreshGenericMessage(preIconTextStr, string.Empty, selfEliminationMessageData.icon, InfoFeedIconType.Elimination);
			}
		}
		void RefreshSelfKnockoutMessage(InfoFeed.SelfKnockoutMessageData selfKnockoutMessageData)
		{
			if (fromInitialization)
			{
				string preIconTextStr = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(selfKnockoutMessageData.playerGuid), InfoFeed.Player1Color);
				RefreshGenericMessage(preIconTextStr, string.Empty, selfKnockoutMessageData.icon, InfoFeedIconType.None);
			}
		}
		void RefreshSpeedrunMessage(InfoFeed.SpeedrunMessageData speedrunMessageData2)
		{
			string arg = speedrunMessageData2.time.ToString("0.0");
			string text = string.Format(Localization.UI.INFO_FEED_Speedrun, arg);
			string text2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(speedrunMessageData2.playerGuid), InfoFeed.Player0Color) + " " + text;
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text2;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshStrokesMessage(InfoFeed.StrokesMessageData strokesMessageData2)
		{
			string text = LocalizationManager.GetString(StringTable.UI, $"INFO_FEED_{strokesMessageData2.strokesUnderParType}");
			string text2 = InfoFeed.ColorizePlayerName(CourseManager.GetPlayerName(strokesMessageData2.playerGuid), InfoFeed.Player0Color) + " " + text;
			if (strokesMessageData2.strokesUnderParType != StrokesUnderParType.HoleInOne)
			{
				text2 += string.Format(" ({0}{1})", (strokesMessageData2.strokesUnderPar == 0) ? string.Empty : "-", strokesMessageData2.strokesUnderPar);
			}
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text2;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		static void SetIcon(Image image, Sprite icon)
		{
			image.sprite = icon;
			if (image.TryGetComponent<LayoutElement>(out var component))
			{
				float num = icon.rect.width / icon.rect.height;
				component.preferredWidth = component.preferredHeight * num;
			}
		}
	}

	private async void SlideIn()
	{
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0f);
		float time = 0f;
		while (time < InfoFeed.MessageSlideInDuration)
		{
			await UniTask.Yield();
			if (this == null)
			{
				return;
			}
			time += Time.deltaTime;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, BMath.Lerp(0f, fullHeight, BMath.EaseOut(BMath.EaseOut(time / InfoFeed.MessageSlideInDuration))));
		}
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, fullHeight);
	}

	private async void FadeOut()
	{
		IsFadingOut = true;
		stripes.gameObject.SetActive(value: true);
		float time = 0f;
		while (time < InfoFeed.MessageFadeOutDuration)
		{
			await UniTask.Yield();
			if (this == null)
			{
				return;
			}
			time += Time.deltaTime;
			canvasGroup.alpha = BMath.Lerp(1f, 0f, BMath.EaseIn(time / InfoFeed.MessageFadeOutDuration));
		}
		InfoFeed.InformMessageDisappeared(this);
		IsFadingOut = false;
	}
}
