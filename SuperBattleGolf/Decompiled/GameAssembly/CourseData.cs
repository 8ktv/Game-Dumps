using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "Course settings", menuName = "Settings/Courses/Course")]
public class CourseData : ScriptableObject
{
	[SerializeField]
	private bool includeAllHoles;

	[SerializeField]
	private bool difficultyCourse;

	[SerializeField]
	[DisplayIf("difficultyCourse", true)]
	private HoleData.DifficultyLevel difficulty;

	[field: SerializeField]
	public LocalizedString LocalizedName { get; private set; }

	[field: SerializeField]
	public Sprite CategoryIcon { get; private set; }

	[field: SerializeField]
	public Sprite MenuBackground { get; private set; }

	[field: SerializeField]
	public Color HoleLabelColor { get; private set; }

	[field: SerializeField]
	public Color WindBackroundColor { get; private set; }

	[field: SerializeField]
	public CourseWindVfx[] WindVfxPrefabs { get; private set; }

	[field: SerializeField]
	public WindManager.WindAudioAmbienceType WindAmbienceType { get; private set; }

	[field: SerializeField]
	public PostProcessProfile PostProcessing { get; private set; }

	[field: SerializeField]
	[field: ElementName("Hole")]
	public HoleData[] Holes { get; private set; }

	public bool ShouldInitCourses
	{
		get
		{
			if (!difficultyCourse)
			{
				return !includeAllHoles;
			}
			return false;
		}
	}

	public void OverrideHoles(HoleData[] holes)
	{
		Holes = holes;
	}

	public void SetLocalizedName(LocalizedString name)
	{
		LocalizedName = name;
	}
}
