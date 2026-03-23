using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
	[SerializeField]
	private Image background;

	[SerializeField]
	private Image statusBackground;

	[SerializeField]
	private Image infoBackground;

	[SerializeField]
	private Image statsBackground;

	[SerializeField]
	private Image stripes;

	[SerializeField]
	private Image status;

	[SerializeField]
	private Image respawningSpinner;

	[SerializeField]
	private Image playerIcon;

	[SerializeField]
	private TextMeshProUGUI ranking;

	[SerializeField]
	private new TextMeshProUGUI name;

	[SerializeField]
	private TextMeshProUGUI courseScore;

	[SerializeField]
	private TextMeshProUGUI matchScore;

	[SerializeField]
	private TextMeshProUGUI strokes;

	[SerializeField]
	private GameObject strokesMedal;

	[SerializeField]
	private TextMeshProUGUI knockouts;

	[SerializeField]
	private GameObject knockoutMedal;

	[SerializeField]
	private TextMeshProUGUI wins;

	[SerializeField]
	private GameObject winsMedal;

	[SerializeField]
	private TextMeshProUGUI finishes;

	[SerializeField]
	private GameObject finishesMedal;

	[SerializeField]
	private TextMeshProUGUI ping;

	[SerializeField]
	private Image pingIcon;

	[SerializeField]
	private Sprite[] pingIcons;

	[SerializeField]
	private int pingQualityInterval = 50;

	[SerializeField]
	private GameObject dominatedByLocalPlayerIcon;

	[SerializeField]
	private GameObject dominatesLocalPlayerIcon;

	[SerializeField]
	private GameObject dominationCounter;

	[SerializeField]
	private GameObject dominationsParent;

	[SerializeField]
	private TMP_Text dominationCounterLabel;

	public void PopulateWith(CourseManager.PlayerState playerState, int ranking, Color backgroundColor, Color statusColor, Color statusHighlightColor, Color stripesColor, bool showWinsMedal, bool showStrokesMedal, bool showKnockoutMedal, bool showFinishesMedal)
	{
		UpdateStatus();
		background.color = backgroundColor;
		statusBackground.color = statusHighlightColor;
		infoBackground.color = statusColor;
		Color color = statusColor;
		color.a *= 0.5f;
		statsBackground.color = color;
		stripes.color = stripesColor;
		this.ranking.text = ranking.ToString();
		name.text = GameManager.RichTextNoParse(playerState.name);
		matchScore.text = GetMatchScore();
		courseScore.text = playerState.courseScore.ToString();
		strokes.text = GetStrokesRelativeToPar();
		knockouts.text = playerState.courseKnockouts.ToString();
		wins.text = playerState.wins.ToString();
		finishes.text = playerState.finishes.ToString();
		GetPing(out var text, out var icon);
		ping.text = text;
		pingIcon.sprite = icon;
		playerIcon.sprite = PlayerIconManager.GetPlayerIcon(playerState.playerGuid, PlayerIconManager.IconSize.Medium);
		bool flag = GameManager.LocalPlayerId != null;
		dominatedByLocalPlayerIcon.SetActive(flag && CourseManager.PlayerDominations.Contains(new CourseManager.PlayerPair(GameManager.LocalPlayerId.Guid, playerState.playerGuid)));
		dominatesLocalPlayerIcon.SetActive(flag && CourseManager.PlayerDominations.Contains(new CourseManager.PlayerPair(playerState.playerGuid, GameManager.LocalPlayerId.Guid)));
		if (Scoreboard.FakePlayers && playerState.playerGuid == 0L)
		{
			playerState.dominatingCount = 0;
			bool flag2 = UnityEngine.Random.Range(0, 2) == 0;
			dominatedByLocalPlayerIcon.SetActive(flag2);
			if (!flag2)
			{
				bool flag3 = UnityEngine.Random.Range(0, 2) == 0;
				dominatesLocalPlayerIcon.SetActive(flag3);
				if (flag3)
				{
					playerState.dominatingCount++;
				}
			}
			else
			{
				playerState.dominatingCount++;
			}
			if (UnityEngine.Random.Range(0, 2) == 1)
			{
				playerState.dominatingCount += UnityEngine.Random.Range(0, 15);
			}
		}
		dominationCounter.SetActive(playerState.dominatingCount > 0);
		dominationCounterLabel.text = ((playerState.dominatingCount > 0) ? playerState.dominatingCount.ToString() : string.Empty);
		dominationsParent.SetActive(dominatedByLocalPlayerIcon.activeSelf || dominatesLocalPlayerIcon.activeSelf || dominationCounter.activeSelf);
		winsMedal.SetActive(showWinsMedal);
		wins.fontStyle = (showWinsMedal ? FontStyles.Bold : FontStyles.Normal);
		strokesMedal.SetActive(showStrokesMedal);
		strokes.fontStyle = (showStrokesMedal ? FontStyles.Bold : FontStyles.Normal);
		knockoutMedal.SetActive(showKnockoutMedal);
		knockouts.fontStyle = (showKnockoutMedal ? FontStyles.Bold : FontStyles.Normal);
		finishesMedal.SetActive(showFinishesMedal);
		finishes.fontStyle = (showFinishesMedal ? FontStyles.Bold : FontStyles.Normal);
		int GetIconIndex(int ping)
		{
			return BMath.Min(BMath.FloorToInt(ping) / pingQualityInterval, pingIcons.Length - 1);
		}
		string GetMatchScore()
		{
			if (playerState.matchScore == 0)
			{
				if (!playerState.matchResolution.IsResolved())
				{
					return string.Empty;
				}
				return "+0";
			}
			if (playerState.matchScore > 0)
			{
				return $"+{playerState.matchScore}";
			}
			return playerState.matchScore.ToString();
		}
		void GetPing(out string reference, out Sprite reference2)
		{
			float value;
			if (playerState.isHost)
			{
				reference = Scoreboard.HostLocalizedString;
				reference2 = pingIcons[0];
			}
			else if (!CourseManager.PlayerPingPerGuid.TryGetValue(playerState.playerGuid, out value))
			{
				if (Scoreboard.FakePlayers)
				{
					int num = UnityEngine.Random.Range(20, 300);
					reference2 = pingIcons[GetIconIndex(num)];
					reference = num.ToString();
				}
				else
				{
					reference2 = pingIcons[^1];
					reference = "-";
				}
			}
			else
			{
				int num2 = GetIconIndex(BMath.RoundToInt(value));
				reference2 = pingIcons[num2];
				reference = BMath.RoundToInt(value).ToString();
			}
		}
		string GetStrokesRelativeToPar()
		{
			if (!playerState.TryGetStrokesRelativeToPar(out var num))
			{
				return "-";
			}
			if (num > 0)
			{
				return $"+{num}";
			}
			if (num < 0)
			{
				return num.ToString();
			}
			return "±0";
		}
		bool ShouldShowStatusIcon()
		{
			if (playerState.matchResolution == PlayerMatchResolution.JoinedAsSpectator)
			{
				return false;
			}
			if (playerState.isRespawning)
			{
				return true;
			}
			if (playerState.matchResolution != PlayerMatchResolution.Uninitialized && playerState.matchResolution != PlayerMatchResolution.None)
			{
				return true;
			}
			return false;
		}
		void UpdateStatus()
		{
			if (!ShouldShowStatusIcon())
			{
				status.enabled = false;
				respawningSpinner.enabled = false;
			}
			else
			{
				status.enabled = true;
				if (playerState.isRespawning)
				{
					respawningSpinner.enabled = true;
					status.transform.localScale = Vector3.one * 0.6f;
					status.sprite = Scoreboard.EliminatedIcon;
				}
				else
				{
					respawningSpinner.enabled = false;
					status.transform.localScale = Vector3.one;
					Image image = status;
					image.sprite = playerState.matchResolution switch
					{
						PlayerMatchResolution.Scored => Scoreboard.ScoredIcon, 
						PlayerMatchResolution.Eliminated => Scoreboard.EliminatedIcon, 
						_ => throw new InvalidOperationException($"Match resolution {playerState.matchResolution} has no icon associated with it"), 
					};
				}
			}
		}
	}

	private void Update()
	{
		if (respawningSpinner.enabled)
		{
			respawningSpinner.transform.rotation = Quaternion.Euler(0f, 0f, Time.time * 45f % 360f);
		}
	}
}
