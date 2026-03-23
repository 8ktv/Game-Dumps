using UnityEngine;

[CreateAssetMenu(fileName = "Hittable VFX Settings", menuName = "Settings/VFX/Hittable VFX Settings")]
public class HittableVfxSettings : ScriptableObject
{
	[SerializeField]
	private VfxType projectileTrail;

	[field: SerializeField]
	public VfxType CollisionVfx { get; private set; }

	[field: SerializeField]
	public float CollisionMinimumSpeed { get; private set; }

	[field: SerializeField]
	public float CollisionMinimumAlignment { get; private set; }

	public VfxType ProjectileTrail => projectileTrail;

	public float CollisionMinimumSpeedSquared { get; private set; }

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
		CollisionMinimumSpeedSquared = CollisionMinimumSpeed * CollisionMinimumSpeed;
	}
}
