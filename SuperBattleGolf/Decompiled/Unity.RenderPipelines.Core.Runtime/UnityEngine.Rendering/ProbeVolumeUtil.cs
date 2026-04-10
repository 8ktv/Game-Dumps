namespace UnityEngine.Rendering;

internal static class ProbeVolumeUtil
{
	internal static int CellSize(int subdivisionLevel)
	{
		return (int)Mathf.Pow(3f, subdivisionLevel);
	}

	internal static float BrickSize(float minBrickSize, int subdivisionLevel)
	{
		return minBrickSize * (float)CellSize(subdivisionLevel);
	}

	internal static float MaxBrickSize(float minBrickSize, int maxSubDivision)
	{
		return BrickSize(minBrickSize, maxSubDivision - 1);
	}
}
