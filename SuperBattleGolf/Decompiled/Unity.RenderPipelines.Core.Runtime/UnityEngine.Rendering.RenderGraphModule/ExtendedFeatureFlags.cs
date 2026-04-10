using System;

namespace UnityEngine.Rendering.RenderGraphModule;

[Flags]
public enum ExtendedFeatureFlags
{
	None = 0,
	TileProperties = 1,
	MultiviewRenderRegionsCompatible = 2,
	MultisampledShaderResolve = 4
}
