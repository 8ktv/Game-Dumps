using UnityEngine;

[CreateAssetMenu(fileName = "Screenshake settings", menuName = "Settings/Camera/Screenshake")]
public class ScreenshakeSettings : ScriptableObject
{
	[field: SerializeField]
	public ScreenshakeType Type { get; private set; } = ScreenshakeType.Position | ScreenshakeType.Rotation;

	[field: SerializeField]
	[field: Min(0f)]
	public float Duration { get; private set; } = 1f;

	[field: SerializeField]
	[field: Min(0f)]
	public float SmoothingSpeed { get; private set; } = 20f;

	[field: SerializeField]
	[field: DisplayIf("Type", ScreenshakeType.Position)]
	public AnimationCurve PositionIntensityOverTime { get; private set; } = AnimationCurve.EaseInOut(0f, 0.3f, 1f, 0f);

	[field: SerializeField]
	[field: DisplayIf("Type", ScreenshakeType.Rotation)]
	public AnimationCurve RotationIntensityOverTime { get; private set; } = AnimationCurve.EaseInOut(0f, 5f, 1f, 0f);

	[field: SerializeField]
	public AnimationCurve DistanceIntensityFactor { get; private set; } = AnimationCurve.EaseInOut(0f, 1f, 50f, 0f);

	[field: SerializeField]
	public AnimationCurve DistanceDurationFactor { get; private set; } = AnimationCurve.EaseInOut(0f, 1f, 50f, 0f);

	private void OnValidate()
	{
		PositionIntensityOverTime.NormalizeTime();
		RotationIntensityOverTime.NormalizeTime();
	}

	public float GetPositionIntensity(float normalizedTime)
	{
		if (!Type.HasType(ScreenshakeType.Position))
		{
			return 0f;
		}
		return PositionIntensityOverTime.Evaluate(normalizedTime);
	}

	public float GetPositionIntensity3d(float normalizedTime, float distance)
	{
		if (!Type.HasType(ScreenshakeType.Position))
		{
			return 0f;
		}
		return PositionIntensityOverTime.Evaluate(normalizedTime) * DistanceIntensityFactor.Evaluate(distance);
	}

	public float GetRotationIntensity(float normalizedTime)
	{
		if (!Type.HasType(ScreenshakeType.Rotation))
		{
			return 0f;
		}
		return RotationIntensityOverTime.Evaluate(normalizedTime);
	}

	public float GetRotationIntensity3d(float normalizedTime, float distance)
	{
		if (!Type.HasType(ScreenshakeType.Rotation))
		{
			return 0f;
		}
		return RotationIntensityOverTime.Evaluate(normalizedTime) * DistanceIntensityFactor.Evaluate(distance);
	}
}
