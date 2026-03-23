using UnityEngine;

[CreateAssetMenu(fileName = "Golf hole settings", menuName = "Settings/Gameplay/Golf hole")]
public class GolfHoleSettings : ScriptableObject
{
	[field: SerializeField]
	public float MaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float MinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float MaxRange { get; private set; }
}
