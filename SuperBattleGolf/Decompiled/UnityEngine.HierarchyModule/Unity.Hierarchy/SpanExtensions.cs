using System;

namespace Unity.Hierarchy;

internal static class SpanExtensions
{
	public static bool Contains<T>(this in ReadOnlySpan<T> span, T value) where T : IEquatable<T>
	{
		for (int i = 0; i < span.Length; i++)
		{
			if (span[i].Equals(value))
			{
				return true;
			}
		}
		return false;
	}
}
