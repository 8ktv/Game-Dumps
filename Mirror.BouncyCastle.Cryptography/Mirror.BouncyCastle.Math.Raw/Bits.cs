using System.Runtime.CompilerServices;

namespace Mirror.BouncyCastle.Math.Raw;

internal static class Bits
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint BitPermuteStep(uint x, uint m, int s)
	{
		uint num = (x ^ (x >> s)) & m;
		return num ^ (num << s) ^ x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BitPermuteStep(ulong x, ulong m, int s)
	{
		ulong num = (x ^ (x >> s)) & m;
		return num ^ (num << s) ^ x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void BitPermuteStep2(ref uint hi, ref uint lo, uint m, int s)
	{
		uint num = ((lo >> s) ^ hi) & m;
		lo ^= num << s;
		hi ^= num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void BitPermuteStep2(ref ulong hi, ref ulong lo, ulong m, int s)
	{
		ulong num = ((lo >> s) ^ hi) & m;
		lo ^= num << s;
		hi ^= num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static uint BitPermuteStepSimple(uint x, uint m, int s)
	{
		return ((x & m) << s) | ((x >> s) & m);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static ulong BitPermuteStepSimple(ulong x, ulong m, int s)
	{
		return ((x & m) << s) | ((x >> s) & m);
	}
}
