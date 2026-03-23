using Unity.Mathematics;
using UnityEngine.Splines;

public static class BezierCurveExtensions
{
	public static float GetBoundingLength(this BezierCurve curve)
	{
		return math.length(curve.P1 - curve.P0) + math.length(curve.P2 - curve.P1) + math.length(curve.P3 - curve.P2);
	}
}
