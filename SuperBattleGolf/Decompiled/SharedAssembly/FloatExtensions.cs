public static class FloatExtensions
{
	public static bool Approximately(this float a, float b)
	{
		return BMath.Abs(b - a) < BMath.Max(1E-06f * BMath.Max(BMath.Abs(a), BMath.Abs(b)), BMath.Epsilon * 8f);
	}

	public static bool Approximately(this float a, float b, float tolerance)
	{
		return BMath.Abs(b - a) <= BMath.Max(tolerance, 0f);
	}

	public static float WrapAngleRad(this float angle)
	{
		return BMath.WrapAngleRad(angle);
	}

	public static float WrapAngleDeg(this float angle)
	{
		return BMath.WrapAngleDeg(angle);
	}

	public static float WrapAngleTauRad(this float angle)
	{
		return BMath.WrapAngleTauRad(angle);
	}

	public static float WrapAngle360Deg(this float angle)
	{
		return BMath.WrapAngle360Deg(angle);
	}

	public static float NearestMultipleOf(this float value, float factor)
	{
		return BMath.RoundToMultipleOf(value, factor);
	}
}
