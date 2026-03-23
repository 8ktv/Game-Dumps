using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamInputActionEvent_t
{
	internal ulong ControllerHandle;

	internal SteamInputActionEventType EEventType;
}
