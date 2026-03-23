using System;
using System.Runtime.InteropServices;
using Mirror;
using Steamworks;
using UnityEngine;

public class PlayerId : NetworkBehaviour
{
	[SyncVar(hook = "OnPlayerNameChanged")]
	private string playerName;

	[SyncVar(hook = "OnGuidChanged")]
	private ulong guid;

	[SyncVar]
	private bool isPartyLeader;

	private string playerNameNoRichText;

	private NameTagUi nameTag;

	public Action<string, string> _Mirror_SyncVarHookDelegate_playerName;

	public Action<ulong, ulong> _Mirror_SyncVarHookDelegate_guid;

	public PlayerInfo PlayerInfo { get; private set; }

	public ulong Guid => guid;

	public string PlayerName => playerName;

	public string PlayerNameNoRichText => playerNameNoRichText;

	public bool IsPartyLeader => isPartyLeader;

	public string NetworkplayerName
	{
		get
		{
			return playerName;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref playerName, 1uL, _Mirror_SyncVarHookDelegate_playerName);
		}
	}

	public ulong Networkguid
	{
		get
		{
			return guid;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref guid, 2uL, _Mirror_SyncVarHookDelegate_guid);
		}
	}

	public bool NetworkisPartyLeader
	{
		get
		{
			return isPartyLeader;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isPartyLeader, 4uL, null);
		}
	}

	public event Action GuidChanged;

	public event Action NameChanged;

	public static event Action<PlayerId> AnyPlayerNameChanged;

	public static event Action<PlayerId> AnyPlayerGuidChanged;

	private void Awake()
	{
		PlayerInfo = GetComponent<PlayerInfo>();
	}

	private void Start()
	{
		UpdateNameTagEnabled();
	}

	public void OnWillBeDestroyed()
	{
		RemoveNameTag();
	}

	public override void OnStartServer()
	{
		NetworkisPartyLeader = base.isLocalPlayer;
		UpdateName();
	}

	private void UpdateName()
	{
		NetworkplayerName = GetName();
		string GetName()
		{
			if (SteamEnabler.IsSteamEnabled && guid != 0L)
			{
				return new Friend(guid).Name;
			}
			return "Player " + base.connectionToClient.connectionId;
		}
	}

	[Server]
	public void ServerSetGuid(ulong guid)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerId::ServerSetGuid(System.UInt64)' called when server was not active");
		}
		else
		{
			Networkguid = guid;
		}
	}

	public void OnIsVisibleChanged()
	{
		UpdateNameTagEnabled();
	}

	private void UpdateNameTagEnabled()
	{
		bool flag = nameTag != null;
		bool flag2 = ShouldBeEnabled();
		if (!flag && flag2)
		{
			InitializeNewNameTag();
		}
		else if (flag && !flag2)
		{
			RemoveNameTag();
		}
		void InitializeNewNameTag()
		{
			nameTag = NameTagManager.GetUnusedNameTag();
			nameTag.Initialize(NameTagManager.PlayerNameTagSettings, base.transform, GameManager.UiSettings.PlayerNameTagLocalOffset, GameManager.UiSettings.PlayerNameTagWorldOffset, GameManager.RichTextNoParse(playerName), PlayerInfo, nameTagIsPlayer: true);
		}
		bool ShouldBeEnabled()
		{
			if (PlayerInfo == null || !PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			return true;
		}
	}

	private void RemoveNameTag()
	{
		if (!(nameTag == null))
		{
			NameTagManager.ReturnNameTag(nameTag);
			nameTag = null;
		}
	}

	private void OnPlayerNameChanged(string previousName, string currentName)
	{
		base.name = playerName;
		playerNameNoRichText = GameManager.RichTextNoParse(playerName);
		if (nameTag != null)
		{
			nameTag.SetName(base.name);
		}
		BNetworkManager.singleton.TryShowConnectedMessage(this);
		this.NameChanged?.Invoke();
		PlayerId.AnyPlayerNameChanged?.Invoke(this);
		if (!string.IsNullOrEmpty(previousName) && currentName != previousName)
		{
			Debug.Log($"Server changed name of player \"{currentName}\" from \"{previousName}\" ({guid}), host is suspicious!");
		}
		if (SteamEnabler.IsSteamEnabled && guid != 0L)
		{
			if (!BNetworkManager.TryGetPlayerInLobby(guid, out var player))
			{
				Debug.Log($"Server changed name of player \"{currentName}\" from \"{previousName}\" ({guid}) which isn't present in lobby, host is suspicious!");
			}
			else if (player.Name != currentName)
			{
				Debug.Log($"Server changed name of player \"{currentName}\" from \"{previousName}\" ({guid}) mismatch with Steam name \"{player.Name}\", host is suspicious!");
			}
		}
	}

	private void OnGuidChanged(ulong previousGuid, ulong currentGuid)
	{
		if (base.isServer)
		{
			UpdateName();
		}
		BNetworkManager.singleton.TryShowConnectedMessage(this);
		PlayerInfo.OnGuidChanged(previousGuid, currentGuid);
		this.GuidChanged?.Invoke();
		PlayerId.AnyPlayerGuidChanged?.Invoke(this);
		if (previousGuid != 0L && currentGuid != previousGuid)
		{
			Debug.Log($"Server changed guid of player \"{playerName}\" ({previousGuid} => {currentGuid}), host is suspicious!");
		}
		if (SteamEnabler.IsSteamEnabled && currentGuid != previousGuid && !BNetworkManager.TryGetPlayerInLobby(currentGuid, out var _))
		{
			Debug.Log($"Server change guid of player \"{playerName}\" ({previousGuid} => {currentGuid}) which isn't present in lobby, host is suspicious!");
		}
	}

	public PlayerId()
	{
		_Mirror_SyncVarHookDelegate_playerName = OnPlayerNameChanged;
		_Mirror_SyncVarHookDelegate_guid = OnGuidChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(playerName);
			writer.WriteVarULong(guid);
			writer.WriteBool(isPartyLeader);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteString(playerName);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVarULong(guid);
		}
		if ((syncVarDirtyBits & 4L) != 0L)
		{
			writer.WriteBool(isPartyLeader);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref playerName, _Mirror_SyncVarHookDelegate_playerName, reader.ReadString());
			GeneratedSyncVarDeserialize(ref guid, _Mirror_SyncVarHookDelegate_guid, reader.ReadVarULong());
			GeneratedSyncVarDeserialize(ref isPartyLeader, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref playerName, _Mirror_SyncVarHookDelegate_playerName, reader.ReadString());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref guid, _Mirror_SyncVarHookDelegate_guid, reader.ReadVarULong());
		}
		if ((num & 4L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isPartyLeader, null, reader.ReadBool());
		}
	}
}
