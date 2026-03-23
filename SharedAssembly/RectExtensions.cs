using UnityEngine;

public static class RectExtensions
{
	public static bool Contains(this Rect rect, Rect other)
	{
		if (rect.Contains(other.min))
		{
			return rect.Contains(other.max);
		}
		return false;
	}
}
