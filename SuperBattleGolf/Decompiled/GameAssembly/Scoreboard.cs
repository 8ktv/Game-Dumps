using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Scoreboard : SingletonBehaviour<Scoreboard>
{
	[SerializeField]
	private ScoreboardEntry entryPrefab;

	[SerializeField]
	private Sprite scoredIcon;

	[SerializeField]
	private Sprite eliminatedIcon;

	[SerializeField]
	private Color[] alternatingBackgroundColors;

	[SerializeField]
	private Color localPlayerColor;

	[SerializeField]
	private Color localPlayerHighlightColor;

	[SerializeField]
	private Color finishedColor;

	[SerializeField]
	private Color finishedHighlightColor;

	[Space]
	[SerializeField]
	private UiVisibilityController visibilityController;

	[SerializeField]
	private Transform entryParent;

	[SerializeField]
	private TMP_Text lobbyNameLabel;

	[SerializeField]
	private TMP_Text spectatorsLabel;

	[SerializeField]
	private TMP_Text courseProgress;

	[SerializeField]
	private TMP_Text holeName;

	[SerializeField]
	private TMP_Text parInfo;

	[SerializeField]
	private GameObject cheatsWarning;

	[Header("Local player stats")]
	[SerializeField]
	private TMP_Text localPlayerScore;

	[SerializeField]
	private Image localPlayerIcon;

	[SerializeField]
	private ScoreboardStat bestHoleScore;

	[SerializeField]
	private ScoreboardStat longestChipIn;

	[SerializeField]
	private ScoreboardStat avgFinishTime;

	[SerializeField]
	private ScoreboardStat itemPickups;

	[SerializeField]
	private ScoreboardStat knockoutRatio;

	[SerializeField]
	private Sprite[] rankMedals;

	private bool wantsToShowExternal;

	private bool isVisible;

	private bool isDirty;

	private Coroutine visibilityCoroutine;

	private readonly List<ScoreboardEntry> entries = new List<ScoreboardEntry>();

	private static bool fakePlayers;

	public static Sprite ScoredIcon
	{
		get
		{
			if (!SingletonBehaviour<Scoreboard>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<Scoreboard>.Instance.scoredIcon;
		}
	}

	public static Sprite EliminatedIcon
	{
		get
		{
			if (!SingletonBehaviour<Scoreboard>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<Scoreboard>.Instance.eliminatedIcon;
		}
	}

	public static bool IsVisible
	{
		get
		{
			if (SingletonBehaviour<Scoreboard>.HasInstance)
			{
				return SingletonBehaviour<Scoreboard>.Instance.isVisible;
			}
			return false;
		}
	}

	public static string HostLocalizedString => Localization.UI.SCOREBOARD_Host;

	public static bool FakePlayers => fakePlayers;

	[CCommand("scoreboardFakePlayers", "", false, false)]
	private static void SetFakePlayers(bool enable)
	{
		fakePlayers = enable;
		if (SingletonBehaviour<Scoreboard>.HasInstance)
		{
			SingletonBehaviour<Scoreboard>.Instance.MarkDirty();
		}
	}

	protected override void Awake()
	{
		base.Awake();
		visibilityController.SetDesiredAlpha(0f);
		CourseManager.ForceDisplayScoreboardChanged += OnCourseManagerForceDisplayScoreboardChanged;
		PlayerId.AnyPlayerGuidChanged += OnAnyPlayerGuidChanged;
	}

	protected override void OnDestroy()
	{
		if (isVisible)
		{
			GameManager.UnhideUiGroup(UiHidingGroup.ScoreboardOpen);
			CourseManager.PlayerStatesChanged -= OnPlayerStatesChanged;
			CourseManager.PlayerPingsChanged -= OnPlayerPingsChanged;
		}
		CourseManager.ForceDisplayScoreboardChanged -= OnCourseManagerForceDisplayScoreboardChanged;
		PlayerId.AnyPlayerGuidChanged -= OnAnyPlayerGuidChanged;
		base.OnDestroy();
	}

	public static void Show()
	{
		if (SingletonBehaviour<Scoreboard>.HasInstance)
		{
			SingletonBehaviour<Scoreboard>.Instance.ShowInternal();
		}
	}

	public static void Hide()
	{
		if (SingletonBehaviour<Scoreboard>.HasInstance)
		{
			SingletonBehaviour<Scoreboard>.Instance.HideInternal();
		}
	}

	private void ShowInternal()
	{
		wantsToShowExternal = true;
		UpdateVisibility();
	}

	private void HideInternal()
	{
		wantsToShowExternal = false;
		UpdateVisibility();
	}

	private void UpdateVisibility()
	{
		bool flag = isVisible;
		isVisible = ShouldBeVisible();
		if (isVisible == flag)
		{
			return;
		}
		if (visibilityCoroutine != null)
		{
			StopCoroutine(visibilityCoroutine);
		}
		if (isVisible)
		{
			Refresh();
			GameManager.HideUiGroup(UiHidingGroup.ScoreboardOpen);
			CourseManager.PlayerStatesChanged += OnPlayerStatesChanged;
			CourseManager.PlayerPingsChanged += OnPlayerPingsChanged;
			localPlayerIcon.sprite = PlayerIconManager.GetPlayerIcon(GameManager.LocalPlayerInfo, PlayerIconManager.IconSize.Large);
			lobbyNameLabel.text = (SingletonNetworkBehaviour<MatchSetupMenu>.HasInstance ? GameManager.RichTextNoParse(MatchSetupMenu.ServerName) : string.Empty);
			courseProgress.text = ((!SingletonBehaviour<DrivingRangeManager>.HasInstance) ? $"({CourseManager.CurrentHoleCourseIndex + 1}/{GameManager.CurrentCourse.Holes.Length})" : string.Empty);
			holeName.text = CourseManager.GetCurrentHoleLocalizedName()?.GetLocalizedString() ?? string.Empty;
			parInfo.text = string.Format(Localization.UI.HOLE_INFO_Par, CourseManager.GetCurrentHolePar().ToString());
			if (RadialMenu.CurrentMode == RadialMenuMode.Emote)
			{
				RadialMenu.Hide();
			}
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(1f, 0.075f, BMath.EaseOut));
			cheatsWarning.SetActive(MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.ConsoleCommands));
		}
		else
		{
			GameManager.UnhideUiGroup(UiHidingGroup.ScoreboardOpen);
			CourseManager.PlayerStatesChanged -= OnPlayerStatesChanged;
			CourseManager.PlayerPingsChanged -= OnPlayerPingsChanged;
			visibilityCoroutine = StartCoroutine(AnimateVisibilityRoutine(0f, 0.075f, BMath.EaseIn));
		}
		IEnumerator AnimateVisibilityRoutine(float targetAlpha, float duration, Func<float, float> Easing)
		{
			float initialAlpha = visibilityController.DesiredAlpha;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				float arg = time / duration;
				visibilityController.SetDesiredAlpha(BMath.Lerp(initialAlpha, targetAlpha, Easing(arg)));
				yield return null;
			}
			visibilityController.SetDesiredAlpha(targetAlpha);
		}
		bool ShouldBeVisible()
		{
			if (wantsToShowExternal)
			{
				return true;
			}
			if (CourseManager.ForceDisplayScoreboard)
			{
				return true;
			}
			return false;
		}
	}

	private async void MarkDirty()
	{
		if (!isDirty)
		{
			isDirty = true;
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
			if (!(this == null))
			{
				Refresh();
				isDirty = false;
			}
		}
	}

	private void Refresh()
	{
		if (!isVisible)
		{
			return;
		}
		List<CourseManager.PlayerState> list = CourseManager.GetSortedPlayerStates();
		if (fakePlayers)
		{
			List<CourseManager.PlayerState> list2 = CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Get();
			list2.AddRange(list);
			int num = 16 - list2.Count;
			for (int i = 0; i < num; i++)
			{
				list2.Add(new CourseManager.PlayerState
				{
					isConnected = true,
					isSpectator = (UnityEngine.Random.Range(0, 2) == 0),
					matchScore = UnityEngine.Random.Range(0, 1000),
					courseScore = UnityEngine.Random.Range(0, 1000),
					matchResolution = (PlayerMatchResolution)UnityEngine.Random.Range(0, 3),
					isRespawning = (UnityEngine.Random.Range(0, 2) == 0),
					wins = UnityEngine.Random.Range(0, 1000),
					courseStrokes = UnityEngine.Random.Range(0, 1000),
					courseKnockouts = UnityEngine.Random.Range(0, 1000),
					finishes = UnityEngine.Random.Range(0, 1000),
					longestChipIn = UnityEngine.Random.Range(100, 200),
					bestHoleScore = (StrokesUnderParType)UnityEngine.Random.Range(0, 7),
					avgFinishTime = UnityEngine.Random.Range(10, 100),
					itemPickups = UnityEngine.Random.Range(0, 100),
					courseKnockedOut = UnityEngine.Random.Range(0, 1000)
				});
			}
			list2.Sort();
			list = list2;
		}
		int num2 = int.MinValue;
		int num3 = int.MaxValue;
		int num4 = int.MinValue;
		int num5 = int.MinValue;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		CourseManager.PlayerState localPlayerState = CourseManager.GetLocalPlayerState();
		float num11 = CalculateKoRatio(localPlayerState);
		for (int j = 0; j < list.Count; j++)
		{
			CourseManager.PlayerState state = list[j];
			if (!SingletonBehaviour<DrivingRangeManager>.HasInstance && state.isSpectator)
			{
				continue;
			}
			if (!state.TryGetStrokesRelativeToPar(out var strokes))
			{
				strokes = int.MaxValue;
			}
			num2 = BMath.Max(state.wins, num2);
			num3 = BMath.Min(strokes, num3);
			num4 = BMath.Max(state.courseKnockouts, num4);
			num5 = BMath.Max(state.finishes, num5);
			if (state.playerGuid != localPlayerState.playerGuid)
			{
				if (state.bestHoleScore > localPlayerState.bestHoleScore)
				{
					num6++;
				}
				if (state.longestChipIn > localPlayerState.longestChipIn)
				{
					num7++;
				}
				if (state.avgFinishTime > float.Epsilon && state.avgFinishTime < localPlayerState.avgFinishTime)
				{
					num8++;
				}
				if (state.itemPickups > localPlayerState.itemPickups)
				{
					num9++;
				}
				float num12 = CalculateKoRatio(state);
				if (num12 > float.Epsilon && num12 > num11)
				{
					num10++;
				}
			}
		}
		localPlayerScore.text = localPlayerState.courseScore.ToString();
		bestHoleScore.Initialize(GetStrokesUnderPar(localPlayerState.bestHoleScore), GetMedal(num6, localPlayerState.bestHoleScore > StrokesUnderParType.None));
		float distanceInCurrentUnitsFloat = GameSettings.All.General.GetDistanceInCurrentUnitsFloat(localPlayerState.longestChipIn);
		string value = ((distanceInCurrentUnitsFloat > float.Epsilon) ? string.Format(GameSettings.All.General.GetLocalizedDistanceUnitName(), $"{distanceInCurrentUnitsFloat:0.0}") : "-");
		longestChipIn.Initialize(value, GetMedal(num7, localPlayerState.longestChipIn > float.Epsilon));
		avgFinishTime.Initialize((localPlayerState.avgFinishTime > float.Epsilon) ? $"{localPlayerState.avgFinishTime:0.0}s" : "-", GetMedal(num8, localPlayerState.avgFinishTime > float.Epsilon));
		itemPickups.Initialize($"{localPlayerState.itemPickups}", GetMedal(num9));
		string value2 = (float.IsFinite(num11) ? $"{num11:0.00}" : ((!(num11 > 0f)) ? "-" : "∞"));
		knockoutRatio.Initialize(value2, GetMedal(num10, num11 > float.Epsilon));
		int num13 = 0;
		int num14 = 0;
		string text = string.Empty;
		for (int k = 0; k < list.Count; k++)
		{
			CourseManager.PlayerState playerState = list[k];
			if (!playerState.isConnected)
			{
				continue;
			}
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance || !playerState.isSpectator)
			{
				int num15 = num13;
				num13++;
				bool num16 = playerState.playerGuid == BNetworkManager.LocalPlayerGuidOnServer;
				ScoreboardEntry scoreboardEntry = ((num15 < entries.Count) ? entries[num15] : CreateNewEntry());
				Color backgroundColor = GetBackgroundColor(k);
				Color statusColor;
				Color stripesColor;
				Color statusHighlightColor;
				if (num16)
				{
					statusColor = localPlayerColor;
					stripesColor = (statusHighlightColor = localPlayerHighlightColor);
				}
				else if (playerState.matchResolution == PlayerMatchResolution.Scored)
				{
					statusColor = finishedColor;
					statusHighlightColor = finishedHighlightColor;
					stripesColor = Color.clear;
				}
				else
				{
					statusColor = (statusHighlightColor = (stripesColor = Color.clear));
				}
				bool showWinsMedal = num2 > 0 && playerState.wins == num2;
				int strokes2;
				bool showStrokesMedal = num3 < int.MaxValue && playerState.TryGetStrokesRelativeToPar(out strokes2) && strokes2 == num3;
				bool showKnockoutMedal = num4 > 0 && playerState.courseKnockouts == num4;
				bool showFinishesMedal = num5 > 0 && playerState.finishes == num5;
				scoreboardEntry.gameObject.SetActive(value: true);
				scoreboardEntry.PopulateWith(playerState, num15 + 1, backgroundColor, statusColor, statusHighlightColor, stripesColor, showWinsMedal, showStrokesMedal, showKnockoutMedal, showFinishesMedal);
			}
			else
			{
				text = text + GameManager.RichTextNoParse(playerState.name) + ", ";
				num14++;
			}
		}
		if (text.Length > 0)
		{
			text = text.Substring(0, text.Length - 2);
			spectatorsLabel.gameObject.SetActive(value: true);
			spectatorsLabel.text = string.Format(Localization.UI.SCOREBOARD_Spectators, "<b>", "</b>", num14.ToString(), text);
		}
		else
		{
			spectatorsLabel.gameObject.SetActive(value: false);
		}
		for (int l = num13; l < entries.Count; l++)
		{
			entries[l].gameObject.SetActive(value: false);
		}
		if (fakePlayers)
		{
			CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Release(list);
		}
		static float CalculateKoRatio(CourseManager.PlayerState playerState2)
		{
			if (playerState2.courseKnockedOut <= 0 && playerState2.courseKnockouts > 0)
			{
				return float.PositiveInfinity;
			}
			if (playerState2.courseKnockedOut <= 0)
			{
				return float.NegativeInfinity;
			}
			return (float)playerState2.courseKnockouts / (float)playerState2.courseKnockedOut;
		}
		ScoreboardEntry CreateNewEntry()
		{
			ScoreboardEntry scoreboardEntry2 = UnityEngine.Object.Instantiate(entryPrefab, entryParent);
			entries.Add(scoreboardEntry2);
			return scoreboardEntry2;
		}
		Color GetBackgroundColor(int entryIndex)
		{
			return alternatingBackgroundColors[entryIndex % alternatingBackgroundColors.Length];
		}
		Sprite GetMedal(int rank, bool condition = true)
		{
			if (!condition || rank >= rankMedals.Length)
			{
				return null;
			}
			return rankMedals[rank];
		}
		static string GetStrokesUnderPar(StrokesUnderParType strokesUnderParType)
		{
			if (strokesUnderParType == StrokesUnderParType.None)
			{
				return "-";
			}
			string text2 = LocalizationManager.GetString(StringTable.UI, $"INFO_FEED_{strokesUnderParType}");
			switch (strokesUnderParType)
			{
			case StrokesUnderParType.Par:
				text2 += " ±0";
				break;
			default:
				text2 += $" -{(int)(strokesUnderParType - 1)}";
				break;
			case StrokesUnderParType.HoleInOne:
				break;
			}
			return text2;
		}
	}

	private void OnPlayerStatesChanged(SyncList<CourseManager.PlayerState>.Operation operation, int itemIndex, CourseManager.PlayerState changedItem)
	{
		MarkDirty();
	}

	private void OnPlayerPingsChanged(SyncIDictionary<ulong, float>.Operation operation, ulong playerGuid, float ping)
	{
		MarkDirty();
	}

	private void OnCourseManagerForceDisplayScoreboardChanged()
	{
		UpdateVisibility();
	}

	private void OnAnyPlayerGuidChanged(PlayerId playerId)
	{
		MarkDirty();
	}
}
