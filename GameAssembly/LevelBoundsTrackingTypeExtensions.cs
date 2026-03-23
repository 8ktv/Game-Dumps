public static class LevelBoundsTrackingTypeExtensions
{
	public static bool HasType(this LevelBoundsTrackingType type, LevelBoundsTrackingType typeToCheck)
	{
		return (type & typeToCheck) == typeToCheck;
	}
}
