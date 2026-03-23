using UnityEngine;

[CreateAssetMenu(fileName = "Projectile hittable settings", menuName = "Settings/Gameplay/Hittables/Projectile")]
public class ProjectileHittableSettings : ScriptableObject
{
	[field: SerializeField]
	public bool CanBeHitBySwingProjectiles { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHitBySwingProjectiles", true)]
	public float SwingProjectileMinResultingSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBeHitBySwingProjectiles", true)]
	public float SwingProjectileMaxResultingSpeed { get; private set; }
}
