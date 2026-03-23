using UnityEngine;

[CreateAssetMenu(fileName = "Level bounds tracker settings", menuName = "Settings/Gameplay/Level bounds tracker")]
public class LevelBoundsTrackerSettings : ScriptableObject
{
	[field: SerializeField]
	public bool ServerOnly { get; private set; }

	[field: SerializeField]
	public AutomaticOutOfBoundsBehaviour DrivingRangeAutomaticOutOfBoundsBehaviour { get; private set; }

	[field: SerializeField]
	public AutomaticOutOfBoundsBehaviour MatchAutomaticOutOfBoundsBehaviour { get; private set; }

	[field: SerializeField]
	public LevelBoundsTrackingType TrackingType { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("TrackingType", LevelBoundsTrackingType.OutOfBoundsHazards)]
	public Vector3 OutOfBoundsHazardSubmersionLocalPoint { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("TrackingType", LevelBoundsTrackingType.OutOfBoundsHazards)]
	public float OutOfBoundsHazardSubmersionWorldVerticalOffset { get; private set; }

	[field: SerializeField]
	public bool AutomaticReturnToInitialPosition { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("AutomaticReturnToInitialPosition", true)]
	public float AutomaticReturnToBoundsVerticalOffset { get; private set; }
}
