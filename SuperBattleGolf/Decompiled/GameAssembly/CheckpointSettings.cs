using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint settings", menuName = "Settings/Gameplay/Checkpoint")]
public class CheckpointSettings : ScriptableObject
{
	[field: SerializeField]
	public float MaxHorizontalRespawnDistance { get; private set; }

	[field: SerializeField]
	public Material InactiveMaterial { get; private set; }

	[field: SerializeField]
	public Material ActiveMaterial { get; private set; }

	[field: SerializeField]
	public Material ScreenInactiveMaterial { get; private set; }

	[field: SerializeField]
	public Material ScreenActiveMaterial { get; private set; }
}
