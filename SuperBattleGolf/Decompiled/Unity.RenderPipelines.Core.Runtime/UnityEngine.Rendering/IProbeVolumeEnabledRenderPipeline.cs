using System;

namespace UnityEngine.Rendering;

public interface IProbeVolumeEnabledRenderPipeline
{
	bool supportProbeVolume { get; }

	ProbeVolumeSHBands maxSHBands { get; }

	[Obsolete("This field is no longer necessary. #from(2023.3)")]
	ProbeVolumeSceneData probeVolumeSceneData { get; }
}
