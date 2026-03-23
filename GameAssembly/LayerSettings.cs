using UnityEngine;

[CreateAssetMenu(fileName = "Layer settings", menuName = "Settings/Gameplay/Layers")]
public class LayerSettings : ScriptableObject
{
	[field: Header("Layers")]
	[field: SerializeField]
	[field: Layer]
	public int StationaryBallLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int DynamicBallLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int HittablesLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int GolfCartPassengersLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int ElectromagnetShieldLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int PlayerLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int GolfCartLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int FoliageLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int VfxLayer { get; private set; }

	[field: SerializeField]
	[field: Layer]
	public int ItemsLayer { get; private set; }

	[field: Header("Masks")]
	[field: SerializeField]
	public LayerMask TerrainMask { get; private set; }

	[field: SerializeField]
	public LayerMask PlayersMask { get; private set; }

	[field: SerializeField]
	public LayerMask GolfCartsMask { get; private set; }

	[field: SerializeField]
	public LayerMask FoliageMask { get; private set; }

	[field: SerializeField]
	public LayerMask BallMask { get; private set; }

	[field: SerializeField]
	public LayerMask CameraCollidablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask PlayerGroundableMask { get; private set; }

	[field: SerializeField]
	public LayerMask HoleBallTriggerMask { get; private set; }

	[field: SerializeField]
	public LayerMask HoleGeneralTriggerMask { get; private set; }

	[field: SerializeField]
	public LayerMask SwingHittableMask { get; private set; }

	[field: SerializeField]
	public LayerMask ProjectileHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask GunHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask RocketHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask RocketBackBlastHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask LandmineDetectableMask { get; private set; }

	[field: SerializeField]
	public LayerMask LandmineHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask OrbitalLaserHeightSnappingMask { get; private set; }

	[field: SerializeField]
	public LayerMask OrbitalLaserHittablesMask { get; private set; }

	[field: SerializeField]
	public LayerMask ShoveHittableMask { get; private set; }

	[field: SerializeField]
	public LayerMask ScoreKnockbackMask { get; private set; }

	[field: SerializeField]
	public LayerMask LockOnLineOfSightBlockerMask { get; private set; }

	[field: SerializeField]
	public LayerMask FootprintMask { get; private set; }

	[field: SerializeField]
	public LayerMask BallGroundableMask { get; private set; }

	[field: SerializeField]
	public LayerMask BallTrajectoryDeflectorMask { get; private set; }

	[field: SerializeField]
	public LayerMask GolfCartGroundMask { get; private set; }

	[field: SerializeField]
	public LayerMask PotentiallyInteractableMask { get; private set; }

	[field: SerializeField]
	public LayerMask PlayerOcclusionMask { get; private set; }

	[field: SerializeField]
	public LayerMask ExplosionOccludersMask { get; private set; }

	public void SetPlayerCollisions(bool enabled)
	{
		PlayerGroundableMask = SetEnabled(PlayerGroundableMask, GolfCartsMask, enabled);
		SwingHittableMask = SetEnabled(SwingHittableMask, PlayersMask, enabled);
		GunHittablesMask = SetEnabled(GunHittablesMask, PlayersMask, enabled);
		RocketHittablesMask = SetEnabled(RocketHittablesMask, PlayersMask, enabled);
		RocketBackBlastHittablesMask = SetEnabled(RocketBackBlastHittablesMask, PlayersMask, enabled);
		LandmineHittablesMask = SetEnabled(LandmineHittablesMask, PlayersMask, enabled);
		OrbitalLaserHittablesMask = SetEnabled(OrbitalLaserHittablesMask, PlayersMask, enabled);
		ShoveHittableMask = SetEnabled(ShoveHittableMask, PlayersMask, enabled);
		ScoreKnockbackMask = SetEnabled(ScoreKnockbackMask, PlayersMask, enabled);
		Physics.IgnoreLayerCollision(PlayerLayer, PlayerLayer, !enabled);
		Physics.IgnoreLayerCollision(PlayerLayer, GolfCartLayer, !enabled);
		static LayerMask SetEnabled(LayerMask mask, LayerMask flag, bool flag2)
		{
			return flag2 ? ((int)mask | (int)flag) : ((int)mask & ~(int)flag);
		}
	}
}
