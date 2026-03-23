public static class BoundsStateExtensions
{
	public static bool IsInOutOfBoundsHazard(this BoundsState boundsState)
	{
		if (!boundsState.HasFlag(BoundsState.InMainOutOfBoundsHazard))
		{
			return boundsState.HasFlag(BoundsState.InSecondaryOutOfBoundsHazard);
		}
		return true;
	}

	public static bool HasState(this BoundsState boundsState, BoundsState stateToCheck)
	{
		return (boundsState & stateToCheck) != 0;
	}
}
