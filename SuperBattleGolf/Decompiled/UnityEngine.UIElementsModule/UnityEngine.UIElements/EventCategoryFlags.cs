using System;

namespace UnityEngine.UIElements;

[Flags]
internal enum EventCategoryFlags
{
	None = 0,
	All = -1,
	TriggeredByOS = 0x6806E,
	TargetOnly = 0x15A0
}
