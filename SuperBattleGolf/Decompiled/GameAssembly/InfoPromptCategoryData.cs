using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial prompt category settings", menuName = "Settings/Tutorial/Prompt category")]
public class InfoPromptCategoryData : ScriptableObject
{
	[field: SerializeField]
	public TutorialPromptCategory Category { get; private set; }

	[field: SerializeField]
	[field: ElementName("Prompt")]
	public TutorialPrompt[] Prompts { get; private set; }
}
