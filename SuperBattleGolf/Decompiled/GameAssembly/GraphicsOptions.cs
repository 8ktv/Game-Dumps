using Steamworks;
using UnityEngine;

public class GraphicsOptions
{
	public static void Initialize()
	{
		SetVSync(enabled: true);
	}

	[CCommand("setVSyncEnabled", "", false, false)]
	public static void SetVSync(bool enabled)
	{
		if (SteamEnabler.IsSteamEnabled && SteamUtils.IsRunningOnSteamDeck)
		{
			QualitySettings.vSyncCount = 0;
		}
		else
		{
			QualitySettings.vSyncCount = (enabled ? 1 : 0);
		}
	}
}
