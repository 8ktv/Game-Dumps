using UnityEngine;

[CreateAssetMenu(fileName = "Input settings", menuName = "Settings/Gameplay/Input")]
public class BInputSettings : ScriptableObject
{
	[field: SerializeField]
	public float DefaultBufferDuration { get; private set; } = 0.2f;

	[field: SerializeField]
	public float InteractBufferDuration { get; private set; } = 0.1f;

	[field: SerializeField]
	public int MinNonPuttPitch { get; private set; }

	[field: SerializeField]
	public int AimingPitchChangePerTick { get; private set; }

	[field: SerializeField]
	public int NonAimingPitchChangePerTick { get; private set; }

	[field: SerializeField]
	public float GamepadPitchHoldRepeatStartTime { get; private set; }

	[field: SerializeField]
	public float GamepadPitchRepeatInterval { get; private set; }
}
