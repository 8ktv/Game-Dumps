using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeHeader("Modules/Terrain/Public/SpeedTreeWindManager.h")]
internal enum SpeedTreeWindParamIndex
{
	WindVector = 0,
	WindGlobal = 1,
	TreeExtents_SharedHeightStart = 1,
	WindBranch = 2,
	BranchStretchLimits = 2,
	WindBranchTwitch = 3,
	Shared_NoisePosTurbulence_Independence = 3,
	WindBranchWhip = 4,
	Shared_Bend_Oscillation_Turbulence_Flexibility = 4,
	WindBranchAnchor = 5,
	Branch1_NoisePosTurbulence_Independence = 5,
	WindBranchAdherences = 6,
	Branch1_Bend_Oscillation_Turbulence_Flexibility = 6,
	WindTurbulences = 7,
	Branch2_NoisePosTurbulence_Independence = 7,
	WindLeaf1Ripple = 8,
	Branch2_Bend_Oscillation_Turbulence_Flexibility = 8,
	WindLeaf1Tumble = 9,
	Ripple_NoisePosTurbulence_Independence = 9,
	WindLeaf1Twitch = 10,
	Ripple_Planar_Directional_Flexibility_Shimmer = 10,
	WindLeaf2Ripple = 11,
	WindLeaf2Tumble = 12,
	WindLeaf2Twitch = 13,
	WindFrondRipple = 14,
	WindParamsCount_v8 = 15,
	WindParamsCount_v9 = 11,
	MaxWindParamsCount = 16
}
