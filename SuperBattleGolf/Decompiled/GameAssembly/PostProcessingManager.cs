using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : SingletonBehaviour<PostProcessingManager>
{
	[SerializeField]
	private PostProcessProfile underwaterPostProcessingProfile;

	[SerializeField]
	private PostProcessProfile underFogPostProcessingProfile;

	[SerializeField]
	private float undeOutOfBoundsHazardBlendDistance = 0.2f;

	private PostProcessVolume coursePostProcessing;

	private PostProcessVolume underOutOfBoundsHazardPostProcessing;

	private OutOfBoundsHazard nearbyOutOfBoundsHazard = (OutOfBoundsHazard)(-1);

	protected override void Awake()
	{
		base.Awake();
		UpdateCoursePostProcessing();
		underOutOfBoundsHazardPostProcessing = base.gameObject.AddComponent<PostProcessVolume>();
		underOutOfBoundsHazardPostProcessing.isGlobal = true;
		underOutOfBoundsHazardPostProcessing.profile = underwaterPostProcessingProfile;
		underOutOfBoundsHazardPostProcessing.priority = 9000f;
		underOutOfBoundsHazardPostProcessing.weight = 0f;
		underOutOfBoundsHazardPostProcessing.enabled = false;
		CourseManager.CurrentHoleGlobalIndexChanged += OnCurrentHoleGlobalIndexChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		CourseManager.CurrentHoleGlobalIndexChanged -= OnCurrentHoleGlobalIndexChanged;
	}

	public static void SetDistanceBelowOutOfBoundsHazardSurface(float distance, OutOfBoundsHazard hazard)
	{
		if (SingletonBehaviour<PostProcessingManager>.HasInstance)
		{
			SingletonBehaviour<PostProcessingManager>.Instance.SetDistanceBelowOutOfBoundsHazardSurfaceInternal(distance, hazard);
		}
	}

	private void SetDistanceBelowOutOfBoundsHazardSurfaceInternal(float distance, OutOfBoundsHazard hazard)
	{
		if (distance < 0f - undeOutOfBoundsHazardBlendDistance)
		{
			underOutOfBoundsHazardPostProcessing.enabled = false;
			return;
		}
		SetNearbyOutOfBoundsHazard(hazard);
		if (!(underOutOfBoundsHazardPostProcessing.profile == null))
		{
			underOutOfBoundsHazardPostProcessing.enabled = true;
			underOutOfBoundsHazardPostProcessing.weight = BMath.InverseLerpClamped(0f - undeOutOfBoundsHazardBlendDistance, undeOutOfBoundsHazardBlendDistance, distance);
		}
		void SetNearbyOutOfBoundsHazard(OutOfBoundsHazard outOfBoundsHazard)
		{
			if (outOfBoundsHazard != nearbyOutOfBoundsHazard)
			{
				nearbyOutOfBoundsHazard = outOfBoundsHazard;
				PostProcessVolume postProcessVolume = underOutOfBoundsHazardPostProcessing;
				postProcessVolume.profile = outOfBoundsHazard switch
				{
					OutOfBoundsHazard.Water => underwaterPostProcessingProfile, 
					OutOfBoundsHazard.Fog => underFogPostProcessingProfile, 
					_ => null, 
				};
			}
		}
	}

	private void OnCurrentHoleGlobalIndexChanged()
	{
		UpdateCoursePostProcessing();
	}

	private void UpdateCoursePostProcessing()
	{
		bool flag = coursePostProcessing != null;
		PostProcessProfile postProcessProfile;
		bool flag2 = ShouldBeEnabled(out postProcessProfile);
		if (flag2 == flag)
		{
			if (flag2)
			{
				coursePostProcessing.profile = postProcessProfile;
			}
		}
		else if (flag2)
		{
			coursePostProcessing = base.gameObject.AddComponent<PostProcessVolume>();
			coursePostProcessing.isGlobal = true;
			coursePostProcessing.profile = postProcessProfile;
			coursePostProcessing.priority = 1000f;
			coursePostProcessing.weight = 1f;
		}
		else
		{
			coursePostProcessing.enabled = false;
		}
		static bool ShouldBeEnabled(out PostProcessProfile reference)
		{
			reference = null;
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				return false;
			}
			if (CourseManager.CurrentHoleGlobalIndex < 0)
			{
				return false;
			}
			HoleData holeData = GameManager.AllCourses.allHoles[CourseManager.CurrentHoleGlobalIndex];
			reference = holeData.ParentCourse.PostProcessing;
			return reference != null;
		}
	}
}
