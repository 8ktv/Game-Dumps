using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace UnityEngineInternal;

internal class DisplayInternal
{
	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("UnityDisplayManager_PrimaryDisplayIndex")]
	internal static extern int PrimaryDisplayIndex();

	internal static bool IsASecondaryDisplayIndex(int displayIndex)
	{
		return displayIndex >= 0 && displayIndex < Display.displays.Length && displayIndex != PrimaryDisplayIndex();
	}
}
