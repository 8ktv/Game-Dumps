using UnityEngine;

[CreateAssetMenu(fileName = "Player targeting settings", menuName = "Settings/Gameplay/Player targeting")]
public class PlayerTargetingSettings : ScriptableObject
{
	[field: SerializeField]
	public float SearchConeDistance { get; private set; } = 2f;

	[field: SerializeField]
	public float SearchConeAperture { get; private set; } = 50f;

	[field: SerializeField]
	public float SearchConeBaseDiameter { get; private set; } = 0.5f;
}
