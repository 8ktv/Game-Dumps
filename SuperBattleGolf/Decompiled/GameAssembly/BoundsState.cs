using System;

[Flags]
public enum BoundsState : byte
{
	InBounds = 0,
	InMainOutOfBoundsHazard = 1,
	InSecondaryOutOfBoundsHazard = 2,
	OverSecondaryOutOfBoundsHazard = 4,
	OutOfBounds = 8
}
