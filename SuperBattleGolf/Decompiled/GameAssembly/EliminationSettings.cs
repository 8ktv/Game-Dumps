using UnityEngine;

[CreateAssetMenu(fileName = "Elimination settings", menuName = "Settings/Gameplay/Elimination")]
public class EliminationSettings : ScriptableObject
{
	[field: SerializeField]
	public EliminationReason Reason { get; private set; }

	[field: SerializeField]
	public EliminationDurationType DurationType { get; private set; }

	[field: SerializeField]
	public float Duration { get; private set; }
}
