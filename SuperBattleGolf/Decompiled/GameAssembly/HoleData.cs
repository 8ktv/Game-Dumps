using System;
using System.Collections.Generic;
using Eflatun.SceneReference;
using FMODUnity;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "Hole data", menuName = "Settings/Courses/Hole")]
public class HoleData : ScriptableObject
{
	public enum DifficultyLevel
	{
		None = -1,
		Beginner,
		Intermediate,
		Expert
	}

	[field: SerializeField]
	public LocalizedString LocalizedName { get; private set; }

	[field: SerializeField]
	public SceneReference Scene { get; private set; }

	[field: SerializeField]
	[field: Min(1f)]
	public int Par { get; private set; }

	[field: SerializeField]
	public DifficultyLevel Difficulty { get; private set; }

	[field: SerializeField]
	public List<Sprite> ScreenshotsThumbnail { get; private set; } = new List<Sprite>();

	[field: SerializeField]
	public EventReference MusicEvent { get; private set; }

	public CourseData ParentCourse { get; private set; }

	public int ParentCourseIndex { get; private set; }

	public int GlobalIndex { get; private set; }

	public void RuntimeInitialize(CourseData parentCourse, int globalIndex)
	{
		if (ParentCourse != null)
		{
			Debug.LogError("Hole " + base.name + " attempted to initialize twice");
			return;
		}
		ParentCourse = parentCourse;
		ParentCourseIndex = Array.IndexOf(parentCourse.Holes, this);
		GlobalIndex = globalIndex;
	}
}
