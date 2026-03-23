using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamInputDeviceDisconnected_t : ICallbackData
{
	internal ulong DisconnectedDeviceHandle;

	public static int _datasize = Marshal.SizeOf(typeof(SteamInputDeviceDisconnected_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.SteamInputDeviceDisconnected;
}
