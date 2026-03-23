using System.Runtime.InteropServices;

namespace Steamworks.Data;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
internal struct UserSubscribedItemsListChanged_t : ICallbackData
{
	internal AppId AppID;

	public static int _datasize = Marshal.SizeOf(typeof(UserSubscribedItemsListChanged_t));

	public int DataSize => _datasize;

	public CallbackType CallbackType => CallbackType.UserSubscribedItemsListChanged;
}
