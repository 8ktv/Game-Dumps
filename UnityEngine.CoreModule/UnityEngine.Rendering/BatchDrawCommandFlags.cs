using System;

namespace UnityEngine.Rendering;

[Flags]
public enum BatchDrawCommandFlags
{
	None = 0,
	FlipWinding = 1,
	HasMotion = 2,
	IsLightMapped = 4,
	HasSortingPosition = 8,
	LODCrossFadeKeyword = 0x10,
	LODCrossFadeValuePacked = 0x20,
	LODCrossFade = 0x30,
	UseLegacyLightmapsKeyword = 0x40
}
