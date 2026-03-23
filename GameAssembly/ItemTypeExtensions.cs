public static class ItemTypeExtensions
{
	public static bool TryGetEliminationReason(this ItemType itemType, float distance, bool isReflected, out EliminationReason eliminationReason)
	{
		switch (itemType)
		{
		case ItemType.DuelingPistol:
			eliminationReason = (isReflected ? EliminationReason.DeflectedDuelingPistolShot : EliminationReason.DuelingPistol);
			return true;
		case ItemType.ElephantGun:
			eliminationReason = (isReflected ? EliminationReason.DeflectedElephantGunShot : EliminationReason.ElephantGun);
			return true;
		case ItemType.RocketLauncher:
			eliminationReason = (isReflected ? EliminationReason.ReflectedRocket : EliminationReason.Rocket);
			return true;
		case ItemType.Landmine:
			eliminationReason = EliminationReason.Landmine;
			return true;
		case ItemType.OrbitalLaser:
			if (distance <= GameManager.ItemSettings.OrbitalLaserExplosionCenterRadius)
			{
				eliminationReason = EliminationReason.OrbitalLaserCenter;
				return true;
			}
			eliminationReason = EliminationReason.None;
			return false;
		default:
			eliminationReason = EliminationReason.None;
			return false;
		}
	}
}
