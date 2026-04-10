using System;
using UnityEngine;

[Serializable]
public class SpectatorEmoteSettings
{
	public SpectatorEmote emote;

	public Sprite icon;

	public VfxType localPlayerVfxType = VfxType.None;

	public VfxType remotePlayerVfxType = VfxType.None;
}
