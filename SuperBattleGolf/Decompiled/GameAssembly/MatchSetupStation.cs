using FMODUnity;
using UnityEngine;

public class MatchSetupStation : SingletonBehaviour<MatchSetupStation>
{
	[SerializeField]
	private Entity stationEntity;

	[SerializeField]
	private DrivingRangeNextCameraButton nextCameraButton;

	public static DrivingRangeNextCameraButton NextCameraButton
	{
		get
		{
			if (!SingletonBehaviour<MatchSetupStation>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<MatchSetupStation>.Instance.nextCameraButton;
		}
	}

	public static void PlayMenuOpenedEffectsLocalOnly()
	{
		if (SingletonBehaviour<MatchSetupStation>.HasInstance)
		{
			SingletonBehaviour<MatchSetupStation>.Instance.PlayMenuOpenedEffectsLocalOnlyInternal();
		}
	}

	private void PlayMenuOpenedEffectsLocalOnlyInternal()
	{
		RuntimeManager.PlayOneShot(GameManager.AudioSettings.MatchSetupStationInteractEvent, stationEntity.TargetReticlePosition.transform.position);
	}
}
