using System;

namespace Mirror.BouncyCastle.Utilities;

public static class Shorts
{
	public const int NumBits = 16;

	public const int NumBytes = 2;

	public static short ReverseBytes(short i)
	{
		return RotateLeft(i, 8);
	}

	[CLSCompliant(false)]
	public static ushort ReverseBytes(ushort i)
	{
		return RotateLeft(i, 8);
	}

	public static short RotateLeft(short i, int distance)
	{
		return (short)RotateLeft((ushort)i, distance);
	}

	[CLSCompliant(false)]
	public static ushort RotateLeft(ushort i, int distance)
	{
		return (ushort)((i << distance) | (i >> 16 - distance));
	}

	public static short RotateRight(short i, int distance)
	{
		return (short)RotateRight((ushort)i, distance);
	}

	[CLSCompliant(false)]
	public static ushort RotateRight(ushort i, int distance)
	{
		return (ushort)((i >> distance) | (i << 16 - distance));
	}
}
