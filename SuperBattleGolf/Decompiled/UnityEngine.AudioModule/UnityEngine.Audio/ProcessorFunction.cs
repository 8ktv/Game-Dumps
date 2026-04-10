namespace UnityEngine.Audio;

internal enum ProcessorFunction : uint
{
	Process = 1u,
	Update,
	OutputProcessEarly,
	OutputProcess,
	OutputProcessEnd,
	OutputRemoved
}
