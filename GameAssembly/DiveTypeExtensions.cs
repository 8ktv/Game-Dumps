public static class DiveTypeExtensions
{
	public static bool IsElephantGunDive(this DiveType diveType)
	{
		if (diveType != DiveType.ElephantGun)
		{
			return diveType == DiveType.ElephantGunFinalShot;
		}
		return true;
	}
}
