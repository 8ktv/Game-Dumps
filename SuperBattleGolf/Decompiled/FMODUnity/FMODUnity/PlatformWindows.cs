using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity;

public class PlatformWindows : Platform
{
	private static List<CodecChannelCount> staticCodecChannels;

	internal override string DisplayName => "Windows";

	internal override List<CodecChannelCount> DefaultCodecChannels => staticCodecChannels;

	static PlatformWindows()
	{
		staticCodecChannels = new List<CodecChannelCount>
		{
			new CodecChannelCount
			{
				format = CodecType.FADPCM,
				channels = 0
			},
			new CodecChannelCount
			{
				format = CodecType.Vorbis,
				channels = 32
			}
		};
		Settings.AddPlatformTemplate<PlatformWindows>("2c5177b11d81d824dbb064f9ac8527da");
	}

	internal override void DeclareRuntimePlatforms(Settings settings)
	{
		settings.DeclareRuntimePlatform(RuntimePlatform.WindowsPlayer, this);
		settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerX86, this);
		settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerX64, this);
		settings.DeclareRuntimePlatform(RuntimePlatform.MetroPlayerARM, this);
	}

	internal override string GetPluginPath(string pluginName)
	{
		return $"{GetPluginBasePath()}/{RuntimeUtils.GetPluginArchitectureFolder()}/{pluginName}.dll";
	}
}
