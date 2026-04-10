using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoleProgressBarUi : SingletonBehaviour<HoleProgressBarUi>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private HoleProgressBarPlayerEntryUi playerEntryPrefab;

	[SerializeField]
	private int maxPlayerEntryPoolSize;

	[SerializeField]
	private RectTransform playerEntryParent;

	[SerializeField]
	private TextMeshProUGUI strokesLabel;

	[SerializeField]
	private float strokesFadeInDuration;

	[SerializeField]
	private float strokesFadeOutDuration;

	[SerializeField]
	private float strokesVisibilityDuration;

	private readonly Dictionary<PlayerInfo, HoleProgressBarPlayerEntryUi> activePlayerEntries = new Dictionary<PlayerInfo, HoleProgressBarPlayerEntryUi>();

	private readonly List<HoleProgressBarPlayerEntryUi> sortedActivePlayerEntires = new List<HoleProgressBarPlayerEntryUi>();

	private static Transform playerEntryPoolParent;

	private static readonly Stack<HoleProgressBarPlayerEntryUi> playerEntryPool = new Stack<HoleProgressBarPlayerEntryUi>();

	private int displayedStrokes = -1;

	private Coroutine strokesVisibilityRoutine;

	private Coroutine strokesFadeRoutine;

	protected override void Awake()
	{
		base.Awake();
		GameManager.LocalPlayerRegistered += OnLocalPlayerRegistered;
		GameManager.LocalPlayerDeregistered += OnLocalPlayerDeregistered;
		GameManager.RemotePlayerRegistered += OnRemotePlayerRegistered;
		GameManager.RemotePlayerDeregistered += OnRemotePlayerDeregistered;
		PlayerGolfer.AnyPlayerMatchResolutionChanged += OnAnyPlayerMatchResolutionChanged;
		PlayerSpectator.AnyPlayerIsSpectatingChanged += OnAnyPlayerIsSpectatingChanged;
		BNetworkManager.WillChangeScene += OnWillChangeScene;
		LocalizationManager.LanguageChanged += OnLanguageChanged;
	}

	private void Start()
	{
		BUpdate.RegisterCallback(this);
		SetStrokes(0);
		strokesLabel.alpha = 0f;
		if (GameManager.LocalPlayerInfo != null)
		{
			AddPlayerEntry(GameManager.LocalPlayerInfo);
		}
		foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
		{
			AddPlayerEntry(remotePlayer);
		}
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			UpdateStrokesText();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		GameManager.LocalPlayerRegistered -= OnLocalPlayerRegistered;
		GameManager.LocalPlayerDeregistered -= OnLocalPlayerDeregistered;
		GameManager.RemotePlayerRegistered -= OnRemotePlayerRegistered;
		GameManager.RemotePlayerDeregistered -= OnRemotePlayerDeregistered;
		PlayerGolfer.AnyPlayerMatchResolutionChanged -= OnAnyPlayerMatchResolutionChanged;
		PlayerSpectator.AnyPlayerIsSpectatingChanged -= OnAnyPlayerIsSpectatingChanged;
		BNetworkManager.WillChangeScene -= OnWillChangeScene;
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
	}

	public static void UpdateStrokes()
	{
		if (SingletonBehaviour<HoleProgressBarUi>.HasInstance)
		{
			SingletonBehaviour<HoleProgressBarUi>.Instance.UpdateStrokesInternal();
		}
	}

	public static void IncrementStrokes()
	{
		if (SingletonBehaviour<HoleProgressBarUi>.HasInstance)
		{
			SingletonBehaviour<HoleProgressBarUi>.Instance.IncrementStrokesInternal();
		}
	}

	private void UpdateStrokesInternal()
	{
		SetStrokes(GetStrokes());
		static int GetStrokes()
		{
			if (!CourseManager.TryGetPlayerState(GameManager.GetViewedOrLocalPlayer(), out var state))
			{
				return 0;
			}
			return state.matchStrokes;
		}
	}

	private void SetStrokes(int strokes)
	{
		if (strokes != displayedStrokes)
		{
			displayedStrokes = strokes;
			UpdateStrokesText();
			if (strokesVisibilityRoutine != null)
			{
				StopCoroutine(strokesVisibilityRoutine);
			}
			if (strokesLabel.alpha < 1f && displayedStrokes > 0)
			{
				strokesVisibilityRoutine = StartCoroutine(ShowStrokesRoutine(1f));
			}
		}
		IEnumerator FadeRoutine(float targetAlpha, float duration, Func<float, float> Easing)
		{
			float initialAlpha = strokesLabel.alpha;
			for (float time = 0f; time < duration; time += Time.deltaTime)
			{
				float arg = time / duration;
				float t = Easing(arg);
				strokesLabel.alpha = BMath.Lerp(initialAlpha, targetAlpha, t);
				yield return null;
			}
			strokesLabel.alpha = targetAlpha;
		}
		Coroutine FadeTo(float targetAlpha, float duration, Func<float, float> Easing)
		{
			if (strokesFadeRoutine != null)
			{
				StopCoroutine(strokesFadeRoutine);
			}
			strokesFadeRoutine = StartCoroutine(FadeRoutine(targetAlpha, duration, Easing));
			return strokesFadeRoutine;
		}
		IEnumerator ShowStrokesRoutine(float targetAlpha)
		{
			yield return FadeTo(targetAlpha, strokesFadeInDuration, BMath.EaseOut);
		}
	}

	private void IncrementStrokesInternal()
	{
		SetStrokes(displayedStrokes + 1);
	}

	public void OnLateBUpdate()
	{
		foreach (HoleProgressBarPlayerEntryUi value in activePlayerEntries.Values)
		{
			value.OnLateUpdate();
		}
		sortedActivePlayerEntires.Sort(CompareEntries);
		for (int i = 0; i < sortedActivePlayerEntires.Count; i++)
		{
			HoleProgressBarPlayerEntryUi holeProgressBarPlayerEntryUi = sortedActivePlayerEntires[i];
			if (!(holeProgressBarPlayerEntryUi == null))
			{
				holeProgressBarPlayerEntryUi.transform.SetSiblingIndex(i);
			}
		}
		static int CompareEntries(HoleProgressBarPlayerEntryUi a, HoleProgressBarPlayerEntryUi b)
		{
			if (a.IsLocalPlayer)
			{
				return 1;
			}
			if (b.IsLocalPlayer)
			{
				return -1;
			}
			return a.NormalizedProgress.CompareTo(b.NormalizedProgress);
		}
	}

	private void AddPlayerEntry(PlayerInfo player)
	{
		if (!activePlayerEntries.ContainsKey(player))
		{
			HoleProgressBarPlayerEntryUi holeProgressBarPlayerEntryUi = GetUnusedPlayerEntry();
			holeProgressBarPlayerEntryUi.Initialize(player);
			activePlayerEntries.Add(player, holeProgressBarPlayerEntryUi);
			sortedActivePlayerEntires.Add(holeProgressBarPlayerEntryUi);
		}
		HoleProgressBarPlayerEntryUi GetUnusedPlayerEntry()
		{
			EnsurePoolParentExists();
			HoleProgressBarPlayerEntryUi result = null;
			while (result == null)
			{
				if (!playerEntryPool.TryPop(out result))
				{
					result = UnityEngine.Object.Instantiate(playerEntryPrefab);
				}
			}
			result.gameObject.SetActive(value: true);
			result.transform.SetParent(playerEntryParent);
			result.transform.localScale = Vector3.one;
			return result;
		}
	}

	private void RemovePlayerEntry(PlayerInfo player)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown && !(player == null) && activePlayerEntries.TryGetValue(player, out var value))
		{
			sortedActivePlayerEntires.Remove(value);
			UnityEngine.Object.Destroy(value);
			activePlayerEntries.Remove(player);
			ReturnPlayerEntryToPool(value);
		}
	}

	private void ReturnPlayerEntryToPool(HoleProgressBarPlayerEntryUi entry)
	{
		if (playerEntryPool.Count >= maxPlayerEntryPoolSize)
		{
			UnityEngine.Object.Destroy(entry.gameObject);
		}
		else if (!(entry == null))
		{
			entry.OnReturnedToPool();
			entry.gameObject.SetActive(value: false);
			entry.transform.SetParent(playerEntryPoolParent);
			playerEntryPool.Push(entry);
		}
	}

	private void EnsurePoolParentExists()
	{
		if (!(playerEntryPoolParent != null))
		{
			GameObject obj = new GameObject("Name tag pool");
			UnityEngine.Object.DontDestroyOnLoad(obj);
			playerEntryPoolParent = obj.transform;
		}
	}

	private void UpdateStrokesText()
	{
		strokesLabel.text = string.Format(Localization.UI.HOLE_INFO_Strokes, $"<size=42>{displayedStrokes}</size>");
	}

	private void UpdatePlayerRegistration(PlayerInfo player)
	{
		if (!(player == null))
		{
			if (ShouldBeRegistered())
			{
				AddPlayerEntry(player);
			}
			else
			{
				RemovePlayerEntry(player);
			}
		}
		bool ShouldBeRegistered()
		{
			if (player != GameManager.LocalPlayerInfo && !GameManager.RemotePlayers.Contains(player))
			{
				return false;
			}
			if (player.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (player.AsSpectator.IsSpectating)
			{
				return false;
			}
			return true;
		}
	}

	private void OnLocalPlayerRegistered()
	{
		UpdatePlayerRegistration(GameManager.LocalPlayerInfo);
	}

	private void OnLocalPlayerDeregistered()
	{
		UpdatePlayerRegistration(GameManager.LocalPlayerInfo);
	}

	private void OnRemotePlayerRegistered(PlayerInfo player)
	{
		UpdatePlayerRegistration(player);
	}

	private void OnRemotePlayerDeregistered(PlayerInfo player)
	{
		UpdatePlayerRegistration(player);
	}

	private void OnAnyPlayerMatchResolutionChanged(PlayerGolfer player, PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		UpdatePlayerRegistration(player.PlayerInfo);
	}

	private void OnAnyPlayerIsSpectatingChanged(PlayerSpectator player)
	{
		UpdatePlayerRegistration(player.PlayerInfo);
	}

	private void OnWillChangeScene()
	{
		foreach (HoleProgressBarPlayerEntryUi value in activePlayerEntries.Values)
		{
			ReturnPlayerEntryToPool(value);
		}
	}

	private void OnLanguageChanged()
	{
		UpdateStrokesText();
	}
}
