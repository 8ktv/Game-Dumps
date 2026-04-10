using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class SteamAchievementManager : AchievementsManager
{
	private const float minStoreStatsTime = 60f;

	private readonly Dictionary<AchievementId, Achievement> achievementsById = new Dictionary<AchievementId, Achievement>();

	private bool isAttemptingToStoreStats;

	private double lastStoreStatsTimestamp = double.MinValue;

	[CVar("steamAchievementsVerbose", "", "", false, true)]
	private static bool steamAchievementsVerbose;

	public override void Initialize()
	{
		foreach (Achievement achievement in SteamUserStats.Achievements)
		{
			if (!GameManager.Achievements.achievementsBySteamApiName.TryGetValue(achievement.Identifier, out var value))
			{
				Debug.LogError("Steam achievement with API name " + achievement.Identifier + " was not found");
			}
			else
			{
				achievementsById.Add(value.Id, achievement);
			}
		}
		GameManager.LocalPlayerDeregistered += OnLocalPlayerDeregistered;
		GameManager.IsSteamOverlayActiveChanged += OnIsSteamOverlayActiveChanged;
		GameManager.ApplicationLostFocus += OnApplicationLostFocus;
		SteamUserStats.OnUserStatsStored += OnUserStatsStored;
		SteamUserStats.OnAchievementProgress += OnAchievementProgress;
		Debug.Log("Steam achievement manager is initialized");
	}

	public override void Unlock(AchievementId id)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Attempting to unlock achievement {id}");
		}
		if (!IsAchievementProgressAllowed())
		{
			return;
		}
		if (!achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Steam achievement {id} was not found");
			return;
		}
		bool state = value.State;
		bool flag = value.Trigger(apply: false);
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Trigged achievement {value.Name} unlock locally with result {flag}");
		}
		if (!state)
		{
			EnsureStatsStored(skipTimer: true);
		}
	}

	public override void SetProgress(AchievementId id, int value, bool canLower)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Attempting to set achievement {id} progress to {value} (can lower: {canLower})");
		}
		if (!IsAchievementProgressAllowed())
		{
			return;
		}
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value2))
		{
			Debug.LogError($"Achievement data for {id} was not found");
			return;
		}
		if (!value2.HasProgressRequirement)
		{
			Debug.LogError($"Attempted to set progress for achievement {id}, it has no progress requirement");
			return;
		}
		int statInt = SteamUserStats.GetStatInt(value2.SteamProgressStatApiName);
		if (canLower || statInt <= value)
		{
			bool flag = SteamUserStats.SetStat(value2.SteamProgressStatApiName, value);
			if (steamAchievementsVerbose)
			{
				Debug.Log($"[STEAM] Set achievement {value2.Id} progress to {value}/{value2.RequiredProgress} locally with result {flag}");
			}
			bool skipTimer = statInt < value2.RequiredProgress && value >= value2.RequiredProgress;
			EnsureStatsStored(skipTimer);
		}
	}

	public override void IncrementProgress(AchievementId id, int amount)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Attempting to increment achievement {id} progress by {amount}");
		}
		if (!IsAchievementProgressAllowed())
		{
			return;
		}
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Achievement data for {id} was not found");
			return;
		}
		if (!value.HasProgressRequirement)
		{
			Debug.LogError($"Attempted to increment progress for achievement {id}, but it has no progress requirement");
			return;
		}
		int statInt = SteamUserStats.GetStatInt(value.SteamProgressStatApiName);
		int num = statInt + amount;
		bool flag = SteamUserStats.AddStat(value.SteamProgressStatApiName, amount);
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Progressed achievement {value.Id} by {amount} to {num}/{value.RequiredProgress} locally with result {flag}");
		}
		bool skipTimer = statInt < value.RequiredProgress && num >= value.RequiredProgress;
		EnsureStatsStored(skipTimer);
	}

	public override bool IsUnlocked(AchievementId id)
	{
		if (!achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Steam achievement {id} was not found");
			return false;
		}
		return value.State;
	}

	public override int GetProgress(AchievementId id)
	{
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Achievement data for {id} was not found");
			return 0;
		}
		if (!value.HasProgressRequirement)
		{
			return 0;
		}
		return SteamUserStats.GetStatInt(value.SteamProgressStatApiName);
	}

	public override void IndicateProgressOnMultipleOf(AchievementId id, int amount)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Attempting to indicate progress of achievement {id} (multiple of {amount})");
		}
		if (!IsAchievementProgressAllowed())
		{
			return;
		}
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Achievement data for {id} was not found");
		}
		else
		{
			if (!value.HasProgressRequirement)
			{
				return;
			}
			int statInt = SteamUserStats.GetStatInt(value.SteamProgressStatApiName);
			if (statInt < value.RequiredProgress && statInt > 0 && statInt % amount == 0)
			{
				bool flag = SteamUserStats.IndicateAchievementProgress(value.SteamApiName, statInt, value.RequiredProgress);
				if (steamAchievementsVerbose)
				{
					Debug.Log($"[STEAM] Indicated achievement {value.Id} progress {statInt}/{value.RequiredProgress} locally with result {flag}");
				}
			}
		}
	}

	public override void ResetAchievement(AchievementId id)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Attempting to reset achievement {id}");
		}
		if (!achievementsById.TryGetValue(id, out var value))
		{
			Debug.LogError($"Steam achievement {id} was not found");
			return;
		}
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value2))
		{
			Debug.LogError($"Achievement data for {id} was not found");
			return;
		}
		bool flag = value.Clear();
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Cleared achievement {value.Name} locally with result {flag}");
		}
		if (value2.HasProgressRequirement)
		{
			flag = SteamUserStats.SetStat(value2.SteamProgressStatApiName, 0);
			if (steamAchievementsVerbose)
			{
				Debug.Log($"[STEAM] Cleared achievement {value.Name} progress locally with result {flag}");
			}
		}
		EnsureStatsStored(skipTimer: true);
	}

	public override void ResetAllAchievements()
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log("[STEAM] Attempting to reset all achievements");
		}
		bool flag = SteamUserStats.ResetAll(includeAchievements: true);
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Reset all achievements locally with result {flag}");
		}
		EnsureStatsStored(skipTimer: true);
	}

	private async void EnsureStatsStored(bool skipTimer)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log("[STEAM] Ensuring stats stored");
		}
		if (skipTimer)
		{
			lastStoreStatsTimestamp = double.MinValue;
		}
		if (isAttemptingToStoreStats)
		{
			return;
		}
		isAttemptingToStoreStats = true;
		if (steamAchievementsVerbose)
		{
			Debug.Log("[STEAM] Attempting to send stats");
		}
		try
		{
			while (BMath.GetTimeSince(lastStoreStatsTimestamp) < 60f)
			{
				await UniTask.Yield();
			}
			bool flag = false;
			int attemptCount = 0;
			while (!flag)
			{
				await UniTask.Yield();
				flag = SteamUserStats.StoreStats();
				attemptCount++;
			}
			if (steamAchievementsVerbose)
			{
				Debug.Log($"[STEAM] Stats sent successfully after {attemptCount} attempts");
			}
			lastStoreStatsTimestamp = Time.timeAsDouble;
		}
		finally
		{
			isAttemptingToStoreStats = false;
		}
	}

	protected override bool IsAchievementProgressAllowed()
	{
		bool num = base.IsAchievementProgressAllowed();
		if (!num && steamAchievementsVerbose)
		{
			Debug.Log("[STEAM] Achievement progress was disallowed");
		}
		return num;
	}

	private void OnLocalPlayerDeregistered()
	{
		EnsureStatsStored(skipTimer: true);
	}

	private void OnIsSteamOverlayActiveChanged()
	{
		if (GameManager.IsSteamOverlayActive)
		{
			EnsureStatsStored(skipTimer: false);
		}
	}

	private void OnApplicationLostFocus()
	{
		EnsureStatsStored(skipTimer: false);
	}

	private void OnUserStatsStored(Result result)
	{
		if (steamAchievementsVerbose)
		{
			Debug.Log($"[STEAM] Stats stored with result {result}");
		}
	}

	private void OnAchievementProgress(Achievement achievement, int currentProgress, int maxProgress)
	{
		bool flag = currentProgress == 0 && maxProgress == 0;
		if (steamAchievementsVerbose)
		{
			if (flag)
			{
				Debug.Log("[STEAM] Achievement " + achievement.Name + " unlocked");
			}
			else
			{
				Debug.Log($"[STEAM] Achievement {achievement.Name} progressed to {currentProgress}/{maxProgress}");
			}
		}
		if (flag)
		{
			if (!GameManager.Achievements.achievementsBySteamApiName.TryGetValue(achievement.Identifier, out var value))
			{
				Debug.LogError("Failed to retrieve achievment " + achievement.Identifier);
			}
			else
			{
				OnAchievementUnlocked(value.Id);
			}
		}
	}
}
