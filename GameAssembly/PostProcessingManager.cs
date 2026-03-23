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

	private PostProcessVolume underOutOfBoundsHazardPostProcessing;

	private OutOfBoundsHazard nearbyOutOfBoundsHazard = (OutOfBoundsHazard)(-1);

	protected override void Awake()
	{
		base.Awake();
		underOutOfBoundsHazardPostProcessing = base.gameObject.AddComponent<PostProcessVolume>();
		underOutOfBoundsHazardPostProcessing.isGlobal = true;
		underOutOfBoundsHazardPostProcessing.profile = underwaterPostProcessingProfile;
		underOutOfBoundsHazardPostProcessing.priority = 9000f;
		underOutOfBoundsHazardPostProcessing.weight = 0f;
		underOutOfBoundsHazardPostProcessing.enabled = false;
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
}
