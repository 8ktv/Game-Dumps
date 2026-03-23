using UnityEngine;

[CreateAssetMenu(fileName = "Golf cart movement settings", menuName = "Settings/Gameplay/Golf cart/Movement")]
public class GolfCartMovementSettings : ScriptableObject
{
	[field: SerializeField]
	public Vector3 LocalCenterOfMass { get; private set; }

	[field: SerializeField]
	public float MaxForwardSpeed { get; private set; }

	[field: SerializeField]
	public float MaxBackwardSpeed { get; private set; }

	[field: SerializeField]
	[field: Min(1f)]
	public float DriverSpeedBoostSpeedFactor { get; private set; }

	[field: SerializeField]
	public float ForwardAccelerationAttenuationSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float BackwardAccelerationAttenuationSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float MaxForwardAccelerationTorque { get; private set; }

	[field: SerializeField]
	public float MaxBackwardAccelerationTorque { get; private set; }

	[field: SerializeField]
	public float DriverlessMinBrakeSpeed { get; private set; }

	[field: SerializeField]
	public float DriverlessMaxBrakeSpeed { get; private set; }

	[field: SerializeField]
	public float DriverlessMinBrakeTorque { get; private set; }

	[field: SerializeField]
	public float DriverlessMaxBrakeTorque { get; private set; }

	[field: SerializeField]
	public float ActiveBrakeTorque { get; private set; }

	[field: SerializeField]
	public float NoForwardInputBrakeSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float NoForwardInputBrakeTorque { get; private set; }

	[field: SerializeField]
	public float MaxSteeringAngle { get; private set; }

	[field: SerializeField]
	public float SteeringInputSensitivity { get; private set; }

	[field: SerializeField]
	public float DefaultWheelDamping { get; private set; }

	[field: SerializeField]
	public float UngroundedDefaultWheelDamping { get; private set; }

	[field: SerializeField]
	public float UngroundedActiveForwardInputWheelDamping { get; private set; }

	[field: SerializeField]
	public float DriverlessWheelDamping { get; private set; }

	[field: SerializeField]
	public float DriverlessUngroundedWheelDamping { get; private set; }

	[field: SerializeField]
	public float ActiveForwardInputWheelDamping { get; private set; }

	[field: SerializeField]
	public float MaxSpeedExceededMaxWheelDamping { get; private set; }

	[field: SerializeField]
	public float MaxSpeedExceededMaxWheelDampingForwardSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float MaxSpeedExceededMaxWheelDampingBackwardSpeedThreshold { get; private set; }

	[field: SerializeField]
	public float SlopeRollPitchThreshold { get; private set; }

	[field: SerializeField]
	public float JumpSpeed { get; private set; }

	[field: SerializeField]
	public float JumpMaxYawSpeed { get; private set; }

	[field: SerializeField]
	public AnimationCurve JumpGravityFactorCurve { get; private set; }

	[field: SerializeField]
	public float JumpDuration { get; private set; }

	[field: SerializeField]
	public Vector3 WaterWadingLocalBoundsCenter { get; private set; }

	[field: SerializeField]
	public Vector3 WaterWadingLocalBoundsSize { get; private set; }

	[field: SerializeField]
	public float WaterWadingSoundMinSpeed { get; private set; }

	[field: SerializeField]
	public float WaterWadingSoundMaxSpeed { get; private set; }
}
