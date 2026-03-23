#define UNITY_ASSERTIONS
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine;

internal static class BitOperationUtils
{
	internal static int CountBits(int mask)
	{
		return CountBits((uint)mask);
	}

	internal static int CountBits(uint mask)
	{
		uint num = 0u;
		while (mask != 0)
		{
			num += mask & 1;
			mask >>= 1;
		}
		return (int)num;
	}

	private static bool IsValueWithinMaskedBitsRange(uint value, uint mask, int bitCount)
	{
		return AnyBitMatch(mask, value) && IsValueSmallerOrEqualThanIndex(value, BitCountToIndex(bitCount));
	}

	internal static uint ModifyMaskByValuesArrayAndBitCount(uint mask, IEnumerable<int> values, int bitCount = 32)
	{
		AssertBitCount(bitCount);
		uint num = 0u;
		foreach (int value in values)
		{
			uint num2 = (uint)value;
			if (IsValueWithinMaskedBitsRange(num2, mask, bitCount))
			{
				num += num2;
			}
		}
		return num;
	}

	internal static bool AreAllBitsSetForValues(uint mask, IEnumerable<int> values, int bitCount = 32)
	{
		AssertBitCount(bitCount);
		foreach (int value2 in values)
		{
			uint value = (uint)value2;
			if (!AnyBitMatch(mask, value) || IsValueBiggerOrEqualThanIndex(value, BitCountToIndex(bitCount)))
			{
				return false;
			}
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint IndexToValue(int index)
	{
		AssertIndex(index);
		return (uint)(1 << index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsValueSmallerThanIndex(uint value, int index)
	{
		AssertIndex(index);
		return value < IndexToValue(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsValueBiggerThanIndex(uint value, int index)
	{
		AssertIndex(index);
		return value > IndexToValue(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsValueSmallerOrEqualThanIndex(uint value, int index)
	{
		AssertIndex(index);
		return value <= IndexToValue(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool IsValueBiggerOrEqualThanIndex(uint value, int index)
	{
		AssertIndex(index);
		return value >= IndexToValue(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool AnyBitMatch(uint mask, uint value)
	{
		return (mask & value) != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int BitCountToIndex(int bitCount)
	{
		AssertBitCount(bitCount);
		return bitCount - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AssertBitCount(int bitCount)
	{
		Debug.Assert(bitCount >= 1 && bitCount <= 32, "Bit count must be between 1 and 32.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AssertIndex(int index)
	{
		Debug.Assert(index >= 0 && index <= 31, "Index must be between 0 and 31.");
	}
}
