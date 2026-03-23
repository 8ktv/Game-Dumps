using System;
using UnityEngine;

[Serializable]
public class EmoteSettings
{
	public Emote emote;

	public Sprite icon;

	public bool loops;

	[DisplayIf("loops", false)]
	public float frameDuration;

	[HideInInspector]
	public float duration;
}
