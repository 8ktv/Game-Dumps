using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
[Flags]
internal enum PseudoStates
{
	None = 0,
	Active = 1,
	Hover = 2,
	Checked = 8,
	Disabled = 0x20,
	Focus = 0x40,
	Root = 0x80
}
