using System;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class PlayerId : NetworkBehaviour
{
	private string playerNameLocal;

	[SyncVar(hook = "OnGuidChanged")]
	private ulong guid;

	[SyncVar]
	private bool isPartyLeader;

	private string playerNameNoRichText;

	private NameTagUi nameTag;

	private ulong cachedGuid;

	private bool initialized;

	public Action<ulong, ulong> _Mirror_SyncVarHookDelegate_guid;

	public PlayerInfo PlayerInfo { get; private set; }

	public ulong Guid => guid;

	public string PlayerName => playerNameLocal;

	public string PlayerNameNoRichText => playerNameNoRichText;

	public bool IsPartyLeader => isPartyLeader;

	public ulong Networkguid
	{
		get
		{
			return guid;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref guid, 1uL, _Mirror_SyncVarHookDelegate_guid);
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
			GeneratedSyncVarSetter(value, ref isPartyLeader, 2uL, null);
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

	public override void OnStartClient()
	{
		if (base.isServer)
		{
			NetworkisPartyLeader = base.isLocalPlayer;
			Networkguid = cachedGuid;
		}
		initialized = true;
	}

	[Server]
	public void ServerSetGuid(ulong guid)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void PlayerId::ServerSetGuid(System.UInt64)' called when server was not active");
		}
		else if (initialized)
		{
			Networkguid = guid;
		}
		else
		{
			cachedGuid = guid;
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
			nameTag.Initialize(NameTagManager.PlayerNameTagSettings, base.transform, GameManager.UiSettings.PlayerNameTagLocalOffset, GameManager.UiSettings.PlayerNameTagWorldOffset, GameManager.RichTextNoParse(playerNameLocal), PlayerInfo, nameTagIsPlayer: true);
		}
		bool ShouldBeEnabled()
		{
			if (PlayerInfo == null || !PlayerInfo.Movement.IsVisible || Guid == 0L)
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
		base.name = playerNameLocal;
		playerNameNoRichText = GameManager.RichTextNoParse(playerNameLocal);
		if (nameTag != null)
		{
			nameTag.SetName(base.name);
		}
		BNetworkManager.singleton.TryShowConnectedMessage(this);
		this.NameChanged?.Invoke();
		PlayerId.AnyPlayerNameChanged?.Invoke(this);
	}

	private void OnGuidChanged(ulong previousGuid, ulong currentGuid)
	{
		BNetworkManager.singleton.TryShowConnectedMessage(this);
		PlayerInfo.OnGuidChanged(previousGuid, currentGuid);
		this.GuidChanged?.Invoke();
		PlayerId.AnyPlayerGuidChanged?.Invoke(this);
		string text = playerNameLocal;
		playerNameLocal = CourseManager.GetPlayerName(this);
		if (text != playerNameLocal)
		{
			OnPlayerNameChanged(text, playerNameLocal);
		}
		if (previousGuid != 0L && currentGuid != previousGuid)
		{
			Debug.Log($"Server changed guid of player \"{playerNameLocal}\" ({previousGuid} => {currentGuid}), host is suspicious!");
		}
		if (SteamEnabler.IsSteamEnabled && currentGuid != previousGuid && BNetworkManager.IsSteamLobbyValid() && !BNetworkManager.TryGetPlayerInLobby(currentGuid, out var _))
		{
			Debug.Log($"Server change guid of player \"{playerNameLocal}\" ({previousGuid} => {currentGuid}) which isn't present in lobby, host is suspicious!");
		}
	}

	public PlayerId()
	{
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
			writer.WriteVarULong(guid);
			writer.WriteBool(isPartyLeader);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarULong(guid);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteBool(isPartyLeader);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref guid, _Mirror_SyncVarHookDelegate_guid, reader.ReadVarULong());
			GeneratedSyncVarDeserialize(ref isPartyLeader, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref guid, _Mirror_SyncVarHookDelegate_guid, reader.ReadVarULong());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isPartyLeader, null, reader.ReadBool());
		}
	}
}
