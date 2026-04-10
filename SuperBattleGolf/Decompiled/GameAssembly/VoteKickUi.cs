using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VoteKickUi : SingletonBehaviour<VoteKickUi>
{
	public TextMeshProUGUI requesterLabel;

	public TextMeshProUGUI kickLabel;

	public TextMeshProUGUI yesLabel;

	public TextMeshProUGUI yesCountLabel;

	public GameObject yesSelect;

	public TextMeshProUGUI noLabel;

	public TextMeshProUGUI noCountLabel;

	public GameObject noSelect;

	public RectTransform yesFill;

	public RectTransform noFill;

	public Image timerFill;

	public GameObject window;

	public int gamepadIconSize = 42;

	public int keyboardIconSize = 48;

	private bool active;

	private bool hasVoted;

	private string requesterName = string.Empty;

	private string targetName = string.Empty;

	private float voteFillWidth;

	private void Start()
	{
		voteFillWidth = yesFill.sizeDelta.x;
		window.SetActive(value: false);
		LocalizationManager.LanguageChanged += UpdateLocalizedLabels;
		InputManager.SwitchedInputDeviceType += UpdateLocalizedLabels;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		LocalizationManager.LanguageChanged -= UpdateLocalizedLabels;
		InputManager.SwitchedInputDeviceType -= UpdateLocalizedLabels;
	}

	public static void Show(PlayerInfo target, PlayerInfo requester)
	{
		if (SingletonBehaviour<VoteKickUi>.HasInstance)
		{
			SingletonBehaviour<VoteKickUi>.Instance.ShowInternal(target, requester);
		}
	}

	public static void Hide()
	{
		if (SingletonBehaviour<VoteKickUi>.HasInstance)
		{
			SingletonBehaviour<VoteKickUi>.Instance.HideInternal();
		}
	}

	public static void UpdateValues()
	{
		if (SingletonBehaviour<VoteKickUi>.HasInstance)
		{
			SingletonBehaviour<VoteKickUi>.Instance.UpdateValuesInternal();
		}
	}

	private void ShowInternal(PlayerInfo target, PlayerInfo requester)
	{
		if (!(target == null) && !(requester == null))
		{
			targetName = target.PlayerId.PlayerNameNoRichText;
			requesterName = requester.PlayerId.PlayerNameNoRichText;
			active = true;
			hasVoted = false;
			window.SetActive(value: true);
			yesSelect.SetActive(value: false);
			noSelect.SetActive(value: false);
			UpdateLocalizedLabels();
			UpdateValuesInternal();
		}
	}

	private void HideInternal()
	{
		targetName = string.Empty;
		requesterName = string.Empty;
		active = false;
		window.SetActive(value: false);
	}

	private void UpdateValuesInternal()
	{
		if (active)
		{
			int yesVotes = VoteKickManager.YesVotes;
			int noVotes = VoteKickManager.NoVotes;
			int totalVoterCount = VoteKickManager.TotalVoterCount;
			yesCountLabel.text = yesVotes.ToString();
			noCountLabel.text = noVotes.ToString();
			SetFill(yesFill, (float)yesVotes / (float)totalVoterCount);
			SetFill(noFill, (float)noVotes / (float)totalVoterCount);
			timerFill.fillAmount = VoteKickManager.NormalizedProgress;
			bool flag = hasVoted;
			VoteKickManager.Vote vote = VoteKickManager.GetVote(GameManager.LocalPlayerInfo);
			hasVoted = vote > VoteKickManager.Vote.NotVoted;
			if (hasVoted != flag)
			{
				UpdateLocalizedLabels();
				yesSelect.SetActive(vote == VoteKickManager.Vote.Yes);
				noSelect.SetActive(vote == VoteKickManager.Vote.No);
			}
		}
		void SetFill(RectTransform rect, float factor)
		{
			Vector2 sizeDelta = rect.sizeDelta;
			sizeDelta.x = voteFillWidth * factor;
			rect.sizeDelta = sizeDelta;
		}
	}

	private void UpdateLocalizedLabels()
	{
		if (!string.IsNullOrWhiteSpace(targetName) && !string.IsNullOrWhiteSpace(requesterName))
		{
			requesterLabel.text = string.Format(Localization.UI.VOTEKICK_StartedBy, requesterName);
			kickLabel.text = "<pos=28px>" + string.Format(Localization.UI.VOTEKICK_Label, "<nobr>" + GameManager.UiSettings.ApplyColorTag(targetName, TextHighlight.Regular) + "</nobr>");
			if (hasVoted)
			{
				yesLabel.text = "<allcaps>" + Localization.UI.MISC_Yes + "</allcaps>";
				noLabel.text = "<allcaps>" + Localization.UI.MISC_No + "</allcaps>";
				return;
			}
			string text = ((!InputManager.UsingGamepad) ? keyboardIconSize.ToString() : gamepadIconSize.ToString());
			yesLabel.text = string.Format(Localization.UI.VOTEKICK_Yes, "<size=" + text + ">" + InputManager.GetInputIconRichTextTag(InputManager.Controls.Vote.Yes) + "</size>");
			noLabel.text = string.Format(Localization.UI.VOTEKICK_No, "<size=" + text + ">" + InputManager.GetInputIconRichTextTag(InputManager.Controls.Vote.No) + "</size>");
		}
	}
}
