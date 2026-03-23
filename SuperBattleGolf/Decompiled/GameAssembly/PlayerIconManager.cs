using System;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public class PlayerIconManager : SingletonBehaviour<PlayerIconManager>
{
	public enum IconSize
	{
		Small,
		Medium,
		Large
	}

	public Sprite defaultIcon;

	private Dictionary<(IconSize, ulong), Sprite> loadedIcons = new Dictionary<(IconSize, ulong), Sprite>();

	public static Sprite GetPlayerIconFromSteamId(ulong steamId, IconSize size)
	{
		if (!SingletonBehaviour<PlayerIconManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<PlayerIconManager>.Instance.GetPlayerIconInternal(steamId, size);
	}

	public static Sprite GetPlayerIcon(ulong playerGuid, IconSize size)
	{
		if (!SingletonBehaviour<PlayerIconManager>.HasInstance)
		{
			return null;
		}
		if (!GameManager.TryFindPlayerByGuid(playerGuid, out var playerInfo))
		{
			return SingletonBehaviour<PlayerIconManager>.Instance.defaultIcon;
		}
		return SingletonBehaviour<PlayerIconManager>.Instance.GetPlayerIconInternal(playerInfo.PlayerId.Guid, size);
	}

	public static Sprite GetPlayerIcon(PlayerInfo player, IconSize size)
	{
		if (!SingletonBehaviour<PlayerIconManager>.HasInstance)
		{
			return null;
		}
		if (player == null)
		{
			return null;
		}
		return SingletonBehaviour<PlayerIconManager>.Instance.GetPlayerIconInternal(player.PlayerId.Guid, size);
	}

	private Sprite GetPlayerIconInternal(ulong steamId, IconSize size)
	{
		if (!SteamEnabler.IsSteamEnabled || steamId == 0L)
		{
			return defaultIcon;
		}
		if (loadedIcons.TryGetValue((size, steamId), out var value))
		{
			return value;
		}
		int num = size switch
		{
			IconSize.Small => 32, 
			IconSize.Medium => 64, 
			IconSize.Large => 128, 
			_ => -1, 
		};
		if (num < 0)
		{
			throw new Exception();
		}
		if (!TryGetPlayerAsSteamFriend(steamId, out var playerAsSteamFriend))
		{
			return defaultIcon;
		}
		Sprite sprite = (loadedIcons[(size, steamId)] = Sprite.Create(new Texture2D(num, num, TextureFormat.RGB565, mipChain: false), new Rect(0f, 0f, num, num), Vector2.zero));
		value = sprite;
		LoadIconAsync();
		return value;
		async void LoadIconAsync()
		{
			Image? image = size switch
			{
				IconSize.Small => await playerAsSteamFriend.GetSmallAvatarAsync(), 
				IconSize.Medium => await playerAsSteamFriend.GetMediumAvatarAsync(), 
				IconSize.Large => await playerAsSteamFriend.GetLargeAvatarAsync(), 
				_ => null, 
			};
			if (!SingletonBehaviour<PlayerIconManager>.HasInstance || !image.HasValue)
			{
				return;
			}
			try
			{
				Texture2D texture = loadedIcons[(size, steamId)].texture;
				Texture2D texture2D = new Texture2D((int)image.Value.Width, (int)image.Value.Height, TextureFormat.RGBA32, mipChain: false);
				texture2D.LoadRawTextureData(image.Value.Data);
				texture2D.Apply();
				RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.RGB565);
				Graphics.Blit(texture2D, temporary, new Vector2(1f, -1f), new Vector2(0f, 1f));
				Graphics.CopyTexture(temporary, texture);
				UnityEngine.Object.Destroy(texture2D);
				RenderTexture.ReleaseTemporary(temporary);
			}
			catch (Exception exception)
			{
				Debug.LogError("Failed to load player icon!");
				Debug.LogException(exception);
			}
		}
		static bool TryGetPlayerAsSteamFriend(ulong num2, out Friend friend)
		{
			if (NetworkClient.active && BNetworkManager.TryGetPlayerInLobby(num2, out var player))
			{
				friend = player;
				return true;
			}
			foreach (Friend friend in SteamFriends.GetFriends())
			{
				if ((ulong)friend.Id == num2)
				{
					friend = friend;
					return true;
				}
			}
			friend = default(Friend);
			return false;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (Sprite value in loadedIcons.Values)
		{
			UnityEngine.Object.Destroy(value.texture);
			UnityEngine.Object.Destroy(value);
		}
		loadedIcons.Clear();
		loadedIcons = null;
	}
}
