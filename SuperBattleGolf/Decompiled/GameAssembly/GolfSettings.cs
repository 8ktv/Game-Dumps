using UnityEngine;

[CreateAssetMenu(fileName = "Golf settings", menuName = "Settings/Gameplay/Golf")]
public class GolfSettings : ScriptableObject
{
	[field: Header("Prefabs")]
	[field: SerializeField]
	public GameObject BallPrefab { get; private set; }

	[field: Header("Tees")]
	[field: SerializeField]
	public float TeeBallLocalSpawnHeight { get; private set; }

	[field: SerializeField]
	public AnimationCurve TeePostHitSizeCurve { get; private set; }

	[field: SerializeField]
	public float TeePostHitMinSpeed { get; private set; }

	[field: SerializeField]
	public float TeePostHitMinAngularSpeed { get; private set; }

	[field: SerializeField]
	public float TeeMaxSwingPowerSpeed { get; private set; }

	[field: SerializeField]
	public float TeeMaxSwingAngularSpeed { get; private set; }

	[field: Header("Hole")]
	[field: SerializeField]
	public float PlayersOnGreenFlagRaiseHeight { get; private set; }

	[field: SerializeField]
	public float PlayersOnGreenFlagRaiseDuration { get; private set; }

	[field: SerializeField]
	public float PlayersOnGreenFlagLowerDuration { get; private set; }

	[field: Header("Swings")]
	[field: SerializeField]
	public Vector3 SwingHitBoxLocalCenter { get; private set; }

	[field: SerializeField]
	public Vector3 SwingHitBoxSize { get; private set; }

	[field: SerializeField]
	public float SwingChargeRiseDuration { get; private set; }

	[field: SerializeField]
	public float SwingRegularFullChargeCoyoteTime { get; private set; }

	[field: SerializeField]
	public float SwingHitStartFrame { get; private set; }

	[field: SerializeField]
	public float SwingHitFrames { get; private set; }

	[field: SerializeField]
	public float SwingFollowThroughFrames { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingFollowThroughFrames { get; private set; }

	[field: SerializeField]
	public float SwingMinInterruptionFrames { get; private set; }

	[field: SerializeField]
	public float RocketDriverSwingMinInterruptionFrames { get; private set; }

	[field: SerializeField]
	public float SwingVfxFrames { get; private set; }

	[field: SerializeField]
	public float InitialSwingPitch { get; private set; }

	[field: SerializeField]
	public float MaxSwingPitch { get; private set; }

	[field: SerializeField]
	public float MinSwingReleaseNormalizedPower { get; private set; }

	[field: SerializeField]
	public float MaxSwingOvercharge { get; private set; }

	[field: SerializeField]
	public float MaxSwingFumbleSideSpin { get; private set; }

	[field: SerializeField]
	[field: ElementName("Hotkey")]
	public float[] HotkeySwingPresets { get; private set; }

	[field: Header("Lock-on")]
	[field: SerializeField]
	public float LockOnMaxDistance { get; private set; }

	[field: SerializeField]
	public float LockOnMaxYawFromCenterScreen { get; private set; }

	[field: SerializeField]
	public float LockOnYawWeight { get; private set; }

	[field: SerializeField]
	public float HomingProjectileCancelDistancePastTarget { get; private set; }

	[field: SerializeField]
	public float HomingProjectileMaxVelocityRotationPerSecond { get; private set; }

	[field: SerializeField]
	public float HomingProjectileMaxAcceleration { get; private set; }

	[field: SerializeField]
	[field: Range(0f, 1f)]
	public float HomingProjectileInitialHorizontalDistanceFractionMaxHoming { get; private set; }

	[field: Header("Rules")]
	[field: SerializeField]
	public float OutOfBoundsEliminationTime { get; private set; }

	[field: SerializeField]
	public float AheadOfBallDistance { get; private set; }

	[field: SerializeField]
	public float BallReturnPositionRegistrationMaxSpeed { get; private set; }

	[field: SerializeField]
	public float BallMaxOutOfBoundsTime { get; private set; }

	[field: SerializeField]
	public float BallMinWaterTime { get; private set; }

	[field: SerializeField]
	public float BallMaxWaterTime { get; private set; }

	[field: SerializeField]
	public float BallReturnFromWaterMaxVerticalSpeed { get; private set; }

	[field: SerializeField]
	public float BallReturnFromWaterMaxHorizontalSpeed { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsHangOverHeadDefaultDuration { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsHangOverHeadPostReturnToBoundsDuration { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsHangOverHeadOverWaterTimeout { get; private set; }

	[field: SerializeField]
	public AnimationCurve BallReturnToBoundsDropOnHeadCurve { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsDropOnHeadKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsDropOnHeadUpwardsSpeed { get; private set; }

	[field: SerializeField]
	public float BallReturnToBoundsDropOnHeadSpinSpeed { get; private set; }

	[field: SerializeField]
	public float TeeOffPerfectShotSpeedBoostTimeWindow { get; private set; }

	[field: SerializeField]
	public float TeeOffPerfectShotSpeedBoostDuration { get; private set; }

	public float TeePostHitAnimationDuration { get; private set; }

	public float ChargeTimeForRegularFullCharge { get; private set; }

	public float SwingHitStartTime { get; private set; }

	public float SwingHitEndTime { get; private set; }

	public float SwingFollowThroughDuration { get; private set; }

	public float RocketDriverSwingFollowThroughDuration { get; private set; }

	public float SwingTotalDuration { get; private set; }

	public float RocketDriverSwingTotalDuration { get; private set; }

	public float SwingMinInterruptionTime { get; private set; }

	public float RocketDriverSwingMinInterruptionTime { get; private set; }

	public float SwingVfxTime { get; private set; }

	public float LockOnMaxDistanceSquared { get; private set; }

	public float BallReturnPositionRegistrationMaxSpeedSquared { get; private set; }

	public float BallReturnFromWaterMaxHorizontalSpeedSquared { get; private set; }

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
		TeePostHitAnimationDuration = ((TeePostHitSizeCurve != null) ? TeePostHitSizeCurve.keys[^1].time : 0f);
		float fromMax = 1f + MaxSwingOvercharge;
		ChargeTimeForRegularFullCharge = BMath.Remap(0f, fromMax, 0f, SwingChargeRiseDuration, 1f, BMath.InvertEaseIn);
		float num = 30f;
		SwingHitStartTime = SwingHitStartFrame / num;
		SwingHitEndTime = SwingHitStartTime + SwingHitFrames / num;
		SwingFollowThroughDuration = SwingFollowThroughFrames / num;
		RocketDriverSwingFollowThroughDuration = RocketDriverSwingFollowThroughFrames / num;
		SwingTotalDuration = SwingHitEndTime + SwingFollowThroughDuration;
		RocketDriverSwingTotalDuration = SwingHitEndTime + RocketDriverSwingFollowThroughDuration;
		SwingMinInterruptionTime = SwingMinInterruptionFrames / num;
		RocketDriverSwingMinInterruptionTime = RocketDriverSwingMinInterruptionFrames / num;
		SwingVfxTime = SwingVfxFrames / num;
		LockOnMaxDistanceSquared = LockOnMaxDistance * LockOnMaxDistance;
		BallReturnPositionRegistrationMaxSpeedSquared = BallReturnPositionRegistrationMaxSpeed * BallReturnPositionRegistrationMaxSpeed;
		BallReturnFromWaterMaxHorizontalSpeedSquared = BallReturnFromWaterMaxHorizontalSpeed * BallReturnFromWaterMaxHorizontalSpeed;
	}
}
