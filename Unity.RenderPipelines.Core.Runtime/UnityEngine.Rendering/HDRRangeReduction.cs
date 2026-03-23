namespace UnityEngine.Rendering;

[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@e2a954003fc5\\Runtime\\PostProcessing\\HDROutputDefines.cs")]
public enum HDRRangeReduction
{
	None,
	Reinhard,
	BT2390,
	ACES1000Nits,
	ACES2000Nits,
	ACES4000Nits
}
