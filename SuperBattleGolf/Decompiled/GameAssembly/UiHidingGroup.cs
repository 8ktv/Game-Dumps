using System;
using UnityEngine;

[Flags]
public enum UiHidingGroup
{
	None = 0,
	HUD = 1,
	PowerBar = 2,
	NameTags = 4,
	Scoreboard = 8,
	WorldButtonPrompts = 0x10,
	HudButtonPrompts = 0x20,
	[InspectorName(null)]
	AllButtonPrompts = 0x30,
	[InspectorName(null)]
	Paused = 0x3F,
	[InspectorName(null)]
	ExceptPowerBar = 0x3D,
	[InspectorName(null)]
	ScoreboardOpen = 0x38,
	All = -1
}
