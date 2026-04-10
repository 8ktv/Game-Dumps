using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering;

[Obsolete("This is not longer supported Please use DebugDisplaySettingsVolume. #from(6000.2)")]
public interface IVolumeDebugSettings2 : IVolumeDebugSettings
{
	[Obsolete("This property is obsolete and kept only for not breaking user code. VolumeDebugSettings will use current pipeline when it needs to gather volume component types and paths. #from(2023.2)")]
	Type targetRenderPipeline { get; }

	[Obsolete("This property is obsolete and kept only for not breaking user code. VolumeDebugSettings will use current pipeline when it needs to gather volume component types and paths. #from(2023.2)")]
	List<(string, Type)> volumeComponentsPathAndType { get; }
}
