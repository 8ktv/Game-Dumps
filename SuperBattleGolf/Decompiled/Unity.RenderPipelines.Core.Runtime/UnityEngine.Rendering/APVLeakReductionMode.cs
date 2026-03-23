using System;

namespace UnityEngine.Rendering;

[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@e2a954003fc5\\Runtime\\Lighting\\ProbeVolume\\ShaderVariablesProbeVolumes.cs")]
public enum APVLeakReductionMode
{
	None = 0,
	Performance = 1,
	Quality = 2,
	[Obsolete("Performance")]
	ValidityBased = 1,
	[Obsolete("Quality")]
	ValidityAndNormalBased = 2
}
