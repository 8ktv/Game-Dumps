using UnityEngine.Audio;

namespace UnityEngine;

internal struct PlayableSettings
{
	public AudioContainerElement element { get; }

	public double scheduledTime { get; }

	public float pitchOffset { get; }

	public float volumeOffset { get; }

	public double triggerTimeOffset { get; }
}
