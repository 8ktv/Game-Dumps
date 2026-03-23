using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Course settings", menuName = "Settings/Courses/Course")]
public class CourseData : ScriptableObject
{
	[field: SerializeField]
	public LocalizedString LocalizedName { get; private set; }

	[field: SerializeField]
	public Color MenuBackgroundColor { get; private set; }

	[field: SerializeField]
	public Color MenuForegroundColor { get; private set; }

	[field: SerializeField]
	[field: ElementName("Hole")]
	public HoleData[] Holes { get; private set; }

	public void OverrideHoles(HoleData[] holes)
	{
		Holes = holes;
	}

	public void SetLocalizedName(LocalizedString name)
	{
		LocalizedName = name;
	}
}
