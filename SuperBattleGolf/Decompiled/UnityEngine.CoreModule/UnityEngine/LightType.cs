using System;

namespace UnityEngine;

public enum LightType
{
	Spot = 0,
	Directional = 1,
	Point = 2,
	[Obsolete("Enum member LightType.Area has been deprecated. Use LightType.Rectangle instead (UnityUpgradable) -> Rectangle", true)]
	Area = 3,
	Rectangle = 3,
	Disc = 4,
	Pyramid = 5,
	Box = 6,
	Tube = 7
}
