using UnityEngine;

[CreateAssetMenu(fileName = "Dive hittable settings", menuName = "Settings/Gameplay/Hittables/Dive")]
public class DiveHittableSettings : ScriptableObject
{
	[field: SerializeField]
	public bool CanBeHit { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MinKnockbackHitRelativeSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MaxKnockbackHitRelativeSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MinUpwardsSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHit", true)]
	public float MaxUpwardsSpeed { get; private set; }
}
