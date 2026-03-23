using System;

[Flags]
public enum LevelBoundsTrackingType
{
	None = 0,
	Bounds = 1,
	OutOfBoundsHazards = 2,
	Green = 4
}
