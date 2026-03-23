using UnityEngine;

[CreateAssetMenu(fileName = "Player movement settings", menuName = "Settings/Gameplay/Player movement")]
public class PlayerMovementSettings : ScriptableObject
{
	[field: Header("Movement")]
	[field: SerializeField]
	public float DefaultMoveSpeed { get; private set; }

	[field: SerializeField]
	public float WalkMoveSpeed { get; private set; }

	[field: SerializeField]
	public float SwingAimingSpeed { get; private set; }

	[field: SerializeField]
	public float SwingChargingSpeed { get; private set; }

	[field: SerializeField]
	public float WadingInWaterSpeed { get; private set; }

	[field: SerializeField]
	public float SpeedBoostSpeedFactor { get; private set; }

	[field: SerializeField]
	public float MaxSpeedBoostDuration { get; private set; }

	[field: SerializeField]
	public float KnockOutSpeedBoostDuration { get; private set; }

	[field: SerializeField]
	public float ShallowWaterWadingHeightThreshold { get; private set; }

	[field: Header("Rotation")]
	[field: SerializeField]
	public float MaxPlayerRotationSpeedDeg { get; private set; }

	[field: SerializeField]
	public float AimingRotationSpeedFactor { get; private set; }

	[field: Header("Grounding")]
	[field: SerializeField]
	public float VerticalVelocityGroundingThreshold { get; private set; }

	[field: SerializeField]
	public float GroundCheckDistanceAdditionWhenGrounded { get; private set; }

	[field: SerializeField]
	public float AdditionalGroundingRaycastsRadius { get; private set; }

	[field: SerializeField]
	public int AdditionalGroundingRaycastCount { get; private set; }

	[field: SerializeField]
	public float UngroundableGroundPitchThreshold { get; private set; }

	[field: Header("Drag")]
	[field: SerializeField]
	public float DefaultHorizontalDrag { get; private set; }

	[field: Header("In air")]
	[field: SerializeField]
	public float BaseGravityFactor { get; private set; }

	[field: SerializeField]
	public float DefaultTerminalFallingSpeed { get; private set; }

	[field: SerializeField]
	public float JumpUpwardsSpeed { get; private set; }

	[field: Header("Diving")]
	[field: SerializeField]
	public float DiveHorizontalSpeed { get; private set; }

	[field: SerializeField]
	public float DiveUpwardsSpeed { get; private set; }

	[field: SerializeField]
	public float DivingGroundLinearDamping { get; private set; }

	[field: SerializeField]
	public float DivingGroundAngularDamping { get; private set; }

	[field: SerializeField]
	public float DivingAirLinearDamping { get; private set; }

	[field: SerializeField]
	public float DivingAirAngularDamping { get; private set; }

	[field: SerializeField]
	public float DiveMinGroundTimeToGetUp { get; private set; }

	[field: SerializeField]
	public float DiveGetUpFrames { get; private set; }

	[field: SerializeField]
	public float DiveTimeOut { get; private set; }

	[field: Header("Knock out")]
	[field: SerializeField]
	public float KnockoutDefaultGroundDuration { get; private set; }

	[field: SerializeField]
	public float KnockoutBallReturnedGroundDuration { get; private set; }

	[field: SerializeField]
	public float KnockoutTimeOutDuration { get; private set; }

	[field: SerializeField]
	public float KnockoutRecoveryFrames { get; private set; }

	[field: SerializeField]
	public float PostKnockoutImmunityDuration { get; private set; }

	[field: SerializeField]
	public float KnockOutGroundLinearDamping { get; private set; }

	[field: SerializeField]
	public float KnockOutGroundAngularDamping { get; private set; }

	[field: SerializeField]
	public float KnockOutAirLinearDamping { get; private set; }

	[field: SerializeField]
	public float KnockOutAirAngularDamping { get; private set; }

	[field: SerializeField]
	public float KnockoutDisallowSpeedBoostDuration { get; private set; }

	[field: SerializeField]
	public float ExitedGolfcartKnockoutImmunityDuration { get; private set; }

	public float WalkSpeedFactor { get; private set; }

	public float InverseAdditionalGroundingRaycastCount { get; private set; }

	public float DiveGetUpDuration { get; private set; }

	public float KnockoutRecoveryDuration { get; private set; }

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
		WalkSpeedFactor = WalkMoveSpeed / DefaultMoveSpeed;
		InverseAdditionalGroundingRaycastCount = 1f / (float)AdditionalGroundingRaycastCount;
		float num = 30f;
		DiveGetUpDuration = DiveGetUpFrames / num;
		KnockoutRecoveryDuration = KnockoutRecoveryFrames / num;
	}
}
