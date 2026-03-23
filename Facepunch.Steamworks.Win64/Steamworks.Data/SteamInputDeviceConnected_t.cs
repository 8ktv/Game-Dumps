using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct SteamInputDeviceConnected_t : ICallbackData
{
	internal ulong ConnectedDeviceHandle;

	public static int _datasize = Marshal.SizeOf(typeof(SteamInputDeviceConnected_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.SteamInputDeviceConnected;
}
