using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spectator emote settings", menuName = "Settings/Gameplay/Emotes/Spectator")]
public class AllSpectatorEmoteSettings : ScriptableObject
{
	public readonly Dictionary<SpectatorEmote, SpectatorEmoteSettings> emotesByType = new Dictionary<SpectatorEmote, SpectatorEmoteSettings>();

	[field: SerializeField]
	[field: DynamicElementName("emote")]
	public SpectatorEmoteSettings[] EmoteSettings { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		emotesByType.Clear();
		SpectatorEmoteSettings[] emoteSettings = EmoteSettings;
		foreach (SpectatorEmoteSettings spectatorEmoteSettings in emoteSettings)
		{
			if (!emotesByType.TryAdd(spectatorEmoteSettings.emote, spectatorEmoteSettings))
			{
				Debug.LogError($"Duplicate spectator emote of type {spectatorEmoteSettings.emote} found");
			}
		}
	}
}
