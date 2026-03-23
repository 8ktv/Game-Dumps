public static class PlayerMatchResolutionExtensions
{
	public static bool IsResolved(this PlayerMatchResolution resolution)
	{
		if (resolution != PlayerMatchResolution.Uninitialized)
		{
			return resolution != PlayerMatchResolution.None;
		}
		return false;
	}
}
