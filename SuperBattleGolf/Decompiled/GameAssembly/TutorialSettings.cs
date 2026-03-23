using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial settings", menuName = "Settings/Tutorial/General")]
public class TutorialSettings : ScriptableObject
{
	[SerializeField]
	[DynamicElementName("Category")]
	private InfoPromptCategoryData[] Categories;

	public readonly Dictionary<TutorialPromptCategory, TutorialPrompt[]> categorizedPrompts = new Dictionary<TutorialPromptCategory, TutorialPrompt[]>();

	[field: SerializeField]
	public float LookAroundRequiredDuration { get; private set; }

	[field: SerializeField]
	public float MoveRequiredDuration { get; private set; }

	[field: SerializeField]
	public float AimSwingRequiredDuration { get; private set; }

	[field: SerializeField]
	public float ViewScoreRequiredDuration { get; private set; }

	[field: SerializeField]
	public float ChargeSwingMinimumSwingNormalizedPower { get; private set; }

	[field: SerializeField]
	public float PuttMinimumSwingNormalizedPower { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		categorizedPrompts.Clear();
		InfoPromptCategoryData[] categories = Categories;
		foreach (InfoPromptCategoryData infoPromptCategoryData in categories)
		{
			if (!categorizedPrompts.TryAdd(infoPromptCategoryData.Category, infoPromptCategoryData.Prompts))
			{
				Debug.LogError($"Duplicate tutorial prompt category of type {infoPromptCategoryData.Category} found");
			}
		}
	}
}
