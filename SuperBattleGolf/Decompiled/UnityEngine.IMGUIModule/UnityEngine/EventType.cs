using System;
using System.ComponentModel;

namespace UnityEngine;

public enum EventType
{
	MouseDown = 0,
	MouseUp = 1,
	MouseMove = 2,
	MouseDrag = 3,
	KeyDown = 4,
	KeyUp = 5,
	ScrollWheel = 6,
	Repaint = 7,
	Layout = 8,
	DragUpdated = 9,
	DragPerform = 10,
	DragExited = 15,
	Ignore = 11,
	Used = 12,
	ValidateCommand = 13,
	ExecuteCommand = 14,
	ContextClick = 16,
	MouseEnterWindow = 20,
	MouseLeaveWindow = 21,
	TouchDown = 30,
	TouchUp = 31,
	TouchMove = 32,
	TouchEnter = 33,
	TouchLeave = 34,
	TouchStationary = 35,
	[Obsolete("Use MouseDown instead (UnityUpgradable) -> MouseDown", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	mouseDown = 0,
	[Obsolete("Use MouseUp instead (UnityUpgradable) -> MouseUp", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	mouseUp = 1,
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use MouseMove instead (UnityUpgradable) -> MouseMove", true)]
	mouseMove = 2,
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use MouseDrag instead (UnityUpgradable) -> MouseDrag", true)]
	mouseDrag = 3,
	[Obsolete("Use KeyDown instead (UnityUpgradable) -> KeyDown", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	keyDown = 4,
	[Obsolete("Use KeyUp instead (UnityUpgradable) -> KeyUp", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	keyUp = 5,
	[Obsolete("Use ScrollWheel instead (UnityUpgradable) -> ScrollWheel", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	scrollWheel = 6,
	[Obsolete("Use Repaint instead (UnityUpgradable) -> Repaint", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	repaint = 7,
	[Obsolete("Use Layout instead (UnityUpgradable) -> Layout", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	layout = 8,
	[Obsolete("Use DragUpdated instead (UnityUpgradable) -> DragUpdated", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	dragUpdated = 9,
	[Obsolete("Use DragPerform instead (UnityUpgradable) -> DragPerform", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	dragPerform = 10,
	[Obsolete("Use Ignore instead (UnityUpgradable) -> Ignore", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	ignore = 11,
	[Obsolete("Use Used instead (UnityUpgradable) -> Used", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	used = 12
}
