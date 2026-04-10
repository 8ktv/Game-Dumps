using System;
using Unity.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio;

[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ControlHeader.h")]
[RequiredByNativeCode]
internal struct ControlHeader
{
	internal Handle Handle;

	internal IntPtr ManagedTransport;
}
