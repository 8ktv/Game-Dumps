public static class OutOfBoundsHazardExtensions
{
	public static EliminationReason GetEliminationReason(this OutOfBoundsHazard hazard)
	{
		if (hazard == OutOfBoundsHazard.Fog)
		{
			return EliminationReason.FellIntoFog;
		}
		return EliminationReason.FellIntoWater;
	}
}
