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
		if (messageData is InfoFeed.GenericMessageData genericMessageData)
		{
			if (fromInitialization)
			{
				RefreshGenericMessage(genericMessageData);
			}
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
		else
		{
			Debug.LogError($"Invalid info feed message data type: {messageData.GetType()}", base.gameObject);
		}
		void OnRefreshedMessage()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}
		void RefreshChipInMessage(InfoFeed.ChipInMessageData chipInMessageData2)
		{
			string text = string.Format(Localization.UI.INFO_FEED_ChipIn, BMath.FloorToInt(chipInMessageData2.distance));
			string text2 = InfoFeed.ColorizePlayerName(chipInMessageData2.playerName, InfoFeed.Player0Color) + " " + text;
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text2;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshDominatingMessage(InfoFeed.DominatingMessageData dominatingMessageData2)
		{
			string text = string.Format(GameManager.UiSettings.ApplyColorTag(Localization.UI.DOMINATION_IsDominating, TextHighlight.Red), dominatingMessageData2.dominatingPlayerName, dominatingMessageData2.dominatedPlayerName);
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshFinishedHoleMessage(InfoFeed.FinishedHoleMessageData finishedHoleMessageData2)
		{
			string arg = InfoFeed.ColorizePlayerName(finishedHoleMessageData2.playerName, InfoFeed.Player0Color);
			string text = ((finishedHoleMessageData2.displayPlacement > 8) ? string.Format(LocalizationManager.GetString(StringTable.UI, "INFO_FEED_FinishedHigh"), arg, finishedHoleMessageData2.displayPlacement) : string.Format(LocalizationManager.GetString(StringTable.UI, $"INFO_FEED_Finished{finishedHoleMessageData2.displayPlacement}"), arg));
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshGenericMessage(InfoFeed.GenericMessageData genericMessageData2)
		{
			bool flag = !string.IsNullOrEmpty(genericMessageData2.preIconText);
			preIconText.gameObject.SetActive(flag);
			if (flag)
			{
				preIconText.text = genericMessageData2.preIconText;
			}
			bool flag2 = genericMessageData2.icon1 != InfoFeedIconSettings.Type.None;
			icon1.gameObject.SetActive(flag2);
			if (flag2)
			{
				if (InfoFeed.IconSettings.TryGetIcon(genericMessageData2.icon1, out var icon))
				{
					SetIcon(icon1, icon);
				}
				else
				{
					Debug.LogError($"Attempted to display an info feed message icon of type {genericMessageData2.icon1}, but it doesn't exist", base.gameObject);
				}
			}
			bool flag3 = genericMessageData2.icon2 != InfoFeedIconSettings.Type.None;
			this.icon2.gameObject.SetActive(flag3);
			if (flag3)
			{
				if (InfoFeed.IconSettings.TryGetIcon(genericMessageData2.icon2, out var icon2))
				{
					SetIcon(this.icon2, icon2);
				}
				else
				{
					Debug.LogError($"Attempted to display an info feed message icon of type {genericMessageData2.icon2}, but it doesn't exist", base.gameObject);
				}
			}
			bool flag4 = !string.IsNullOrEmpty(genericMessageData2.postIconText);
			postIconText.gameObject.SetActive(flag4);
			if (flag4)
			{
				postIconText.text = genericMessageData2.postIconText;
			}
			OnRefreshedMessage();
		}
		void RefreshRevengeMessage(InfoFeed.RevengeMessageData revengeMessageData2)
		{
			string text = string.Format(GameManager.UiSettings.ApplyColorTag(Localization.UI.DOMINATION_GotRevenge, TextHighlight.Red), revengeMessageData2.previouslyDominatedPlayerName, revengeMessageData2.previouslyDominatingPlayerName);
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshScoredOnDrivingRangeMessage(InfoFeed.ScoredOnDrivingRangeMessageData scoredOnDrivingRangeMessageData2)
		{
			string text = string.Format(LocalizationManager.GetString(StringTable.UI, "INFO_FEED_ScoredOnDrivingRange"), GameManager.RichTextNoParse(scoredOnDrivingRangeMessageData2.playerName));
			preIconText.gameObject.SetActive(value: true);
			preIconText.text = text;
			icon1.gameObject.SetActive(value: false);
			icon2.gameObject.SetActive(value: false);
			postIconText.gameObject.SetActive(value: false);
			OnRefreshedMessage();
		}
		void RefreshSpeedrunMessage(InfoFeed.SpeedrunMessageData speedrunMessageData2)
		{
			string arg = speedrunMessageData2.time.ToString("0.0");
			string text = string.Format(Localization.UI.INFO_FEED_Speedrun, arg);
			string text2 = InfoFeed.ColorizePlayerName(speedrunMessageData2.playerName, InfoFeed.Player0Color) + " " + text;
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
			string text2 = InfoFeed.ColorizePlayerName(strokesMessageData2.playerName, InfoFeed.Player0Color) + " " + text;
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
