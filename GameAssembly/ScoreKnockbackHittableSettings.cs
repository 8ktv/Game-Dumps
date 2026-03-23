using UnityEngine;

[CreateAssetMenu(fileName = "Score knockback hittable settings", menuName = "Settings/Gameplay/Hittables/Score knockback")]
public class ScoreKnockbackHittableSettings : ScriptableObject
{
	[field: SerializeField]
	public float MinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float MaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float MinKnockbackAngularSpeed { get; private set; }

	[field: SerializeField]
	public float MaxKnockbackAngularSpeed { get; private set; }

	[field: SerializeField]
	public float MinMinUpwardsKnockback { get; private set; }

	[field: SerializeField]
	public float MaxMinUpwardsKnockback { get; private set; }
}
