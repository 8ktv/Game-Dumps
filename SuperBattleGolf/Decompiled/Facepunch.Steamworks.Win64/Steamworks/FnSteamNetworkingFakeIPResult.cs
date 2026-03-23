using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks;

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
internal delegate void FnSteamNetworkingFakeIPResult(ref SteamNetworkingFakeIPResult_t arg);
