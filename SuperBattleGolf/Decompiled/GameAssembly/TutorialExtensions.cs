public static class TutorialExtensions
{
	public static bool HasObjective(this TutorialObjective objective, TutorialObjective objectiveToCheck)
	{
		return (objective & objectiveToCheck) == objectiveToCheck;
	}

	public static bool HasCategory(this TutorialPromptCategory category, TutorialPromptCategory categoryToCheck)
	{
		return (category & categoryToCheck) == categoryToCheck;
	}

	public static bool HasPrompt(this TutorialPrompt prompt, TutorialPrompt promptToCheck)
	{
		return (prompt & promptToCheck) == promptToCheck;
	}
}
