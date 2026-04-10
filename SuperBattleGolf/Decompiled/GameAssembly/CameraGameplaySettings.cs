using UnityEngine;

[CreateAssetMenu(fileName = "Camera gameplay settings", menuName = "Settings/Gameplay/Camera")]
public class CameraGameplaySettings : ScriptableObject
{
	[field: SerializeField]
	public float IntoSwingAimTransitionDuration { get; private set; }

	[field: SerializeField]
	public float OutOfSwingAimTransitionDuration { get; private set; }

	[field: SerializeField]
	public float SwingAimVerticalTrackingOffset { get; private set; }

	[field: SerializeField]
	public float SwingAimDistanceAddition { get; private set; }

	[field: SerializeField]
	public float MaxSwingChargeDistanceAddition { get; private set; }

	[field: SerializeField]
	public float SwingChargeDistanceAdditionReleaseDuration { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings RocketLaunchScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings RocketExplosionScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public float RocketExplosionImpactFrameDistance { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings LandmineExplosionScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public float LandmineImpactFrameDistance { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings LandminePlantJamScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings LandminePlantStompScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings OrbitalLaserScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings FreezeBombShotScreenshakeSettings { get; private set; }

	[field: SerializeField]
	public ScreenshakeSettings FreezeBombExplosionScreenshakeSettings { get; private set; }

	public float RocketExplosionImpactFrameDistanceSquared { get; private set; }

	public float LandmineImpactFrameDistanceSquared { get; private set; }

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
		RocketExplosionImpactFrameDistanceSquared = RocketExplosionImpactFrameDistance * RocketExplosionImpactFrameDistance;
		LandmineImpactFrameDistanceSquared = LandmineImpactFrameDistance * LandmineImpactFrameDistance;
	}
}
