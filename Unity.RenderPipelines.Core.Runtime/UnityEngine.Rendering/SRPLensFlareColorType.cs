using System;

namespace UnityEngine.Rendering;

[Serializable]
[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@e2a954003fc5\\Runtime\\PostProcessing\\LensFlareDataSRP.cs")]
public enum SRPLensFlareColorType
{
	Constant,
	RadialGradient,
	AngularGradient
}
