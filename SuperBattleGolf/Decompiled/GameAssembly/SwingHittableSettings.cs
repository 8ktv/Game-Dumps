using UnityEngine;

[CreateAssetMenu(fileName = "Swing hittable settings", menuName = "Settings/Gameplay/Hittables/Swing")]
public class SwingHittableSettings : ScriptableObject
{
	[field: SerializeField]
	public float MaxPowerSwingHitSpeed { get; private set; }

	[field: SerializeField]
	public float MaxPowerPuttHitSpeed { get; private set; }

	[field: SerializeField]
	public float SwingHitSpinSpeed { get; private set; }

	[field: Header("As projectile")]
	[field: SerializeField]
	public bool CanBecomeSwingProjectile { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float MinProjectileSwingSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float GroundedMinProjectileStopSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float AirMinStopProjectileSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float ProjectileMinHitCollisionSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float ProjectileMaxHitCollisionSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float ProjectilePostHitBounceSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("CanBecomeSwingProjectile", true)]
	public float ProjectilePostHitSpinSpeed { get; private set; }

	public float GroundedMinProjectileStopSpeedSquared { get; private set; }

	public float AirMinProjectileStopSpeedSquared { get; private set; }

	private void OnValidate()
	{
		Initialze();
	}

	private void OnEnable()
	{
		Initialze();
	}

	private void Initialze()
	{
		GroundedMinProjectileStopSpeedSquared = GroundedMinProjectileStopSpeed * GroundedMinProjectileStopSpeed;
		AirMinProjectileStopSpeedSquared = AirMinStopProjectileSpeed * AirMinStopProjectileSpeed;
	}
}
