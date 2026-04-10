using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio;

internal readonly struct RealtimeAccess
{
	[NativeDisableUnsafePtrRestriction]
	private unsafe readonly void* m_Realtime;

	private readonly int m_Frame;

	private readonly int m_DTM;

	internal unsafe bool IsCreated => m_Realtime != null;
}
