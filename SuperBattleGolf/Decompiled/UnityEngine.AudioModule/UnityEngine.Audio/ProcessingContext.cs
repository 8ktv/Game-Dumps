using System;
using System.Runtime.InteropServices;

namespace UnityEngine.Audio;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[Obsolete("ProcessingContext has been deprecated. Use RealtimeContext instead. (UnityUpgradable) -> RealtimeContext", true)]
public struct ProcessingContext
{
}
