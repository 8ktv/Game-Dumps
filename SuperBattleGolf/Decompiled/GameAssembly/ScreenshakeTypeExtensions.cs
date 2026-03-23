public static class ScreenshakeTypeExtensions
{
	public static bool HasType(this ScreenshakeType type, ScreenshakeType typeToCheck)
	{
		return (type & typeToCheck) != 0;
	}
}
