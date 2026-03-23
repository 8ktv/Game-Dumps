using UnityEngine.Playables;

namespace UnityEngine;

internal struct ActivePlayable
{
	public PlayableSettings settings { get; }

	public PlayableHandle clipPlayableHandle { get; }
}
