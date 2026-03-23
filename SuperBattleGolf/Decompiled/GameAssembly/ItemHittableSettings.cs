using UnityEngine;

[CreateAssetMenu(fileName = "Item hittable settings", menuName = "Settings/Gameplay/Hittables/Item")]
public class ItemHittableSettings : ScriptableObject
{
	[field: Header("Dueling pistol")]
	[field: SerializeField]
	public float DuelingPistolMaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float DuelingPistolMeleeAttackKnockbackSpeed { get; private set; }

	[field: Header("Elephant gun")]
	[field: SerializeField]
	public float ElephantGunMaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float ElephantGunMinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float ElephantGunMaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float ElephantGunMinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float ElephantGunMeleeAttackKnockbackSpeed { get; private set; }

	[field: Header("Rocket launcher")]
	[field: SerializeField]
	public float RocketLauncherMaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float RocketLauncherMinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float RocketLauncherMaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float RocketLauncherMinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public bool RocketLauncherHasMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("RocketLauncherHasMinUpwardsKnockbackSpeed", true)]
	public float RocketLauncherMaxMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("RocketLauncherHasMinUpwardsKnockbackSpeed", true)]
	public float RocketLauncherMinMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float RocketLauncherBackBlastKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float RocketLauncherMeleeAttackKnockbackSpeed { get; private set; }

	[field: Header("Landmine")]
	[field: SerializeField]
	public float LandmineMaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float LandmineMinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float LandmineMaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float LandmineMinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public bool LandmineHasMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("LandmineHasMinUpwardsKnockbackSpeed", true)]
	public float LandmineMaxMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("LandmineHasMinUpwardsKnockbackSpeed", true)]
	public float LandmineMinMinUpwardsKnockbackSpeed { get; private set; }

	[field: Header("Orbital laser")]
	[field: SerializeField]
	public float OrbitalLaserMaxKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserMinKnockbackDistance { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserMaxKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public float OrbitalLaserMinKnockbackSpeed { get; private set; }

	[field: SerializeField]
	public bool OrbitalLaserHasMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("OrbitalLaserHasMinUpwardsKnockbackSpeed", true)]
	public float OrbitalLaserMaxMinUpwardsKnockbackSpeed { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("OrbitalLaserHasMinUpwardsKnockbackSpeed", true)]
	public float OrbitalLaserMinMinUpwardsKnockbackSpeed { get; private set; }
}
