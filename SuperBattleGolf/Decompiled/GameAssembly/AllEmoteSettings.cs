using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Emote settings", menuName = "Settings/Gameplay/Emotes/Regular")]
public class AllEmoteSettings : ScriptableObject
{
	public readonly Dictionary<Emote, EmoteSettings> emotesByType = new Dictionary<Emote, EmoteSettings>();

	[field: SerializeField]
	[field: DynamicElementName("emote")]
	public EmoteSettings[] EmoteSettings { get; private set; }

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
		float num = 30f;
		emotesByType.Clear();
		EmoteSettings[] emoteSettings = EmoteSettings;
		foreach (EmoteSettings emoteSettings2 in emoteSettings)
		{
			emoteSettings2.duration = emoteSettings2.frameDuration / num;
			if (!emotesByType.TryAdd(emoteSettings2.emote, emoteSettings2))
			{
				Debug.LogError($"Duplicate emote of type {emoteSettings2.emote} found");
			}
		}
	}
}
