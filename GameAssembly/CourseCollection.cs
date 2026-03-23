using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Course collection", menuName = "Settings/Courses/Course collection")]
public class CourseCollection : ScriptableObject
{
	public readonly List<HoleData> allHoles = new List<HoleData>();

	public readonly Dictionary<HoleData, int> allHolesGlobalIndices = new Dictionary<HoleData, int>();

	[field: SerializeField]
	[field: ElementName("Course")]
	public CourseData[] Courses { get; private set; }

	public void RuntimeInitialize()
	{
		CourseData[] courses = Courses;
		foreach (CourseData courseData in courses)
		{
			HoleData[] holes = courseData.Holes;
			foreach (HoleData holeData in holes)
			{
				holeData.RuntimeInitialize(courseData, allHoles.Count);
				allHoles.Add(holeData);
			}
		}
	}
}
