using System;

namespace UnityEngine.Rendering.UnifiedRayTracing;

[Flags]
public enum BuildFlags
{
	None = 0,
	PreferFastTrace = 1,
	PreferFastBuild = 2,
	MinimizeMemory = 4
}
