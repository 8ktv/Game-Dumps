using System;

public abstract class AchievementsManager
{
	public event Action<AchievementId> AchievementUnlocked;

	public abstract void Initialize();

	public abstract void Unlock(AchievementId id);

	public abstract void SetProgress(AchievementId id, int value, bool canLower);

	public abstract void IncrementProgress(AchievementId id, int amount);

	public abstract bool IsUnlocked(AchievementId id);

	public abstract int GetProgress(AchievementId id);

	public abstract void IndicateProgressOnMultipleOf(AchievementId id, int amount);

	public abstract void ResetAchievement(AchievementId id);

	public abstract void ResetAllAchievements();

	protected virtual bool IsAchievementProgressAllowed()
	{
		return !MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.ConsoleCommands);
	}

	protected void OnAchievementUnlocked(AchievementId id)
	{
		this.AchievementUnlocked?.Invoke(id);
	}
}
