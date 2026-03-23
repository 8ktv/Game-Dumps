using System.Collections.Generic;
using UnityEngine;

public class DummyAchievementManager : AchievementsManager
{
	private readonly HashSet<AchievementId> unlockedAchievements = new HashSet<AchievementId>();

	private readonly Dictionary<AchievementId, int> achievementProgress = new Dictionary<AchievementId, int>();

	public override void Initialize()
	{
		Debug.Log("Dummy achievement manager is initialized");
	}

	public override void Unlock(AchievementId id)
	{
		if (!IsAchievementProgressAllowed())
		{
			Debug.Log($"Unlocking achievement {id} was denied");
		}
		else if (unlockedAchievements.Add(id))
		{
			Debug.Log($"Unlocked achievement {id}");
		}
	}

	public override void SetProgress(AchievementId id, int value, bool canLower)
	{
		if (!IsAchievementProgressAllowed())
		{
			Debug.Log($"Setting achievement {id} progress to {value} was denied");
			return;
		}
		if (!GameManager.Achievements.achievementsById.TryGetValue(id, out var value2))
		{
			Debug.LogError($"Achievement data for {id} was not found");
			return;
		}
		if (!value2.HasProgressRequirement)
		{
			Debug.LogError($"Attempted to set achievement {id} progress to {value}, but it has no progress requirement");
			return;
		}
		if (!achievementProgress.TryGetValue(id, out var value3))
		{
			value3 = 0;
		}
		if (!canLower && value3 > value)
		{
			Debug.LogError($"Attempted to set achievement {id} progress to {value}, but it's lower than the current progress of {value3}");
			return;
		}
		achievementProgress[id] = value3;
		Debug.Log($"Set achievement {id} progress to {value}/{value2.RequiredProgress}");
		if (value >= value2.RequiredProgress)
		{
			Unlock(id);
		}
	}

	public override void IncrementProgress(AchievementId id, int amount)
	{
		if (!IsAchievementProgressAllowed())
		{
			Debug.Log($"Incrementing achievement {id} progress by {amount} was denied");
			return;
		}
		if (!achievementProgress.TryGetValue(id, out var value))
		{
			value = 0;
		}
		SetProgress(id, value + amount, canLower: false);
	}

	public override bool IsUnlocked(AchievementId id)
	{
		return unlockedAchievements.Contains(id);
	}

	public override int GetProgress(AchievementId id)
	{
		if (!achievementProgress.TryGetValue(id, out var value))
		{
			return 0;
		}
		return value;
	}

	public override void IndicateProgressOnMultipleOf(AchievementId id, int amount)
	{
	}

	public override void ResetAchievement(AchievementId id)
	{
		unlockedAchievements.Remove(id);
		achievementProgress.Remove(id);
	}

	public override void ResetAllAchievements()
	{
		unlockedAchievements.Clear();
		achievementProgress.Clear();
		Debug.Log("Achievements reset");
	}
}
