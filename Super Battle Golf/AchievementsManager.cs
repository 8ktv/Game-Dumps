public abstract class AchievementsManager
{
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
}
