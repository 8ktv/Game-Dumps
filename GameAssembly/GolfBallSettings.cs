using UnityEngine;

[CreateAssetMenu(fileName = "Golf ball settings", menuName = "Settings/Gameplay/Golf ball")]
public class GolfBallSettings : ScriptableObject
{
	[field: SerializeField]
	public float StationaryStateSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float GroundFullStopMaxPitch { get; private set; }

	[field: SerializeField]
	public float FullStopMinDampingSpeed { get; private set; }

	[field: SerializeField]
	public float FullStopMaxDampingSpeed { get; private set; }

	[field: SerializeField]
	public float FullStopLinearDamping { get; private set; }

	[field: SerializeField]
	public float GroundFrictionLinearDamping { get; private set; }

	[field: SerializeField]
	public float GroundFrictionAngularDamping { get; private set; }

	[field: SerializeField]
	public float LinearAirDragFactor { get; private set; }

	[field: SerializeField]
	public float AirFrictionAngularDamping { get; private set; }

	[field: SerializeField]
	public float LinearFoliageDragFactor { get; private set; }

	[field: Header("Visuals")]
	[field: SerializeField]
	public Material NotAllowedMaterial { get; private set; }

	public float StationaryStateSpeedThresholdSquared { get; private set; }

	public float FullStopMinDampingSpeedSquared { get; private set; }

	public float FullStopMaxDampingSpeedSquared { get; private set; }

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
		StationaryStateSpeedThresholdSquared = StationaryStateSpeedThreshold * StationaryStateSpeedThreshold;
		FullStopMinDampingSpeedSquared = FullStopMinDampingSpeed * FullStopMinDampingSpeed;
		FullStopMaxDampingSpeedSquared = FullStopMaxDampingSpeed * FullStopMaxDampingSpeed;
	}
}
