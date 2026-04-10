using UnityEngine;

[CreateAssetMenu(fileName = "Hittable settings", menuName = "Settings/Gameplay/Hittables/Hittable")]
public class HittableSettings : ScriptableObject
{
	[field: SerializeField]
	public SwingHittableSettings Swing { get; private set; }

	[field: SerializeField]
	public ProjectileHittableSettings Projectile { get; private set; }

	[field: SerializeField]
	public DiveHittableSettings Dive { get; private set; }

	[field: SerializeField]
	public ItemHittableSettings Item { get; private set; }

	[field: SerializeField]
	public ScoreKnockbackHittableSettings ScoreKnockback { get; private set; }

	[field: SerializeField]
	public WindHittableSettings Wind { get; private set; }
}
