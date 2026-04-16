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

	private bool isShown;

	private ulong requesterGuid;

	private ulong targetGuid;

	private string requesterName = string.Empty;

	private string targetName = string.Empty;

	private float voteFillWidth;

	private VoteKickManager.Vote lastVote;

	public static bool IsShown
	{
		get
		{
			if (SingletonBehaviour<VoteKickUi>.HasInstance)
			{
				return SingletonBehaviour<VoteKickUi>.Instance.isShown;
			}
			return false;
		}
	}

	public static ulong RequesterGuid
	{
		get
		{
			if (!SingletonBehaviour<VoteKickUi>.HasInstance)
			{
				return 0uL;
			}
			return SingletonBehaviour<VoteKickUi>.Instance.requesterGuid;
		}
	}

	public static ulong TargetGuid
	{
		get
		{
			if (!SingletonBehaviour<VoteKickUi>.HasInstance)
			{
				return 0uL;
			}
			return SingletonBehaviour<VoteKickUi>.Instance.targetGuid;
		}
	}

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
			targetGuid = target.PlayerId.Guid;
			requesterGuid = requester.PlayerId.Guid;
			targetName = target.PlayerId.PlayerNameNoRichText;
			requesterName = requester.PlayerId.PlayerNameNoRichText;
			isShown = true;
			lastVote = VoteKickManager.Vote.NotVoted;
			window.SetActive(value: true);
			yesSelect.SetActive(value: false);
			noSelect.SetActive(value: false);
			UpdateLocalizedLabels();
			UpdateValuesInternal();
		}
	}

	private void HideInternal()
	{
		targetGuid = 0uL;
		requesterGuid = 0uL;
		targetName = string.Empty;
		requesterName = string.Empty;
		isShown = false;
		window.SetActive(value: false);
	}

	private void UpdateValuesInternal()
	{
		if (isShown)
		{
			int yesVotes = VoteKickManager.YesVotes;
			int noVotes = VoteKickManager.NoVotes;
			int totalVoterCount = VoteKickManager.TotalVoterCount;
			yesCountLabel.text = yesVotes.ToString();
			noCountLabel.text = noVotes.ToString();
			SetFill(yesFill, (float)yesVotes / (float)totalVoterCount);
			SetFill(noFill, (float)noVotes / (float)totalVoterCount);
			timerFill.fillAmount = VoteKickManager.NormalizedProgress;
			VoteKickManager.Vote vote = VoteKickManager.GetVote(GameManager.LocalPlayerInfo);
			if (lastVote != vote)
			{
				yesSelect.SetActive(vote == VoteKickManager.Vote.Yes);
				noSelect.SetActive(vote == VoteKickManager.Vote.No);
				lastVote = vote;
				UpdateLocalizedLabels();
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
			if (lastVote != VoteKickManager.Vote.NotVoted)
			{
				yesLabel.text = "<voffset=-9px><allcaps>" + Localization.UI.MISC_Yes + "</allcaps></voffset>";
				noLabel.text = "<voffset=-9px><allcaps>" + Localization.UI.MISC_No + "</allcaps></voffset>";
				return;
			}
			string text = ((!InputManager.UsingGamepad) ? keyboardIconSize.ToString() : gamepadIconSize.ToString());
			yesLabel.text = string.Format(Localization.UI.VOTEKICK_Yes, "<size=" + text + ">" + InputManager.GetInputIconRichTextTag(InputManager.Controls.Vote.Yes) + "</size>");
			noLabel.text = string.Format(Localization.UI.VOTEKICK_No, "<size=" + text + ">" + InputManager.GetInputIconRichTextTag(InputManager.Controls.Vote.No) + "</size>");
		}
	}
}
