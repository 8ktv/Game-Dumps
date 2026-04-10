using System;
using System.Collections.Generic;
using System.IO;
using Mirror;
using ProximityChat;
using Steamworks;
using UnityEngine;

public class PlayerVoiceChat : MonoBehaviour
{
	public struct PersistentPlayerStatus
	{
		public float volume;

		public bool isMuted;

		public long lastChangeUnixTime;

		public static PersistentPlayerStatus Default => new PersistentPlayerStatus
		{
			volume = 1f,
			isMuted = false,
			lastChangeUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
		};

		public readonly void Write(BinaryWriter writer)
		{
			writer.Write(volume);
			writer.Write(isMuted);
			writer.Write(lastChangeUnixTime);
		}

		public static PersistentPlayerStatus Read(BinaryReader reader)
		{
			return new PersistentPlayerStatus
			{
				volume = reader.ReadSingle(),
				isMuted = reader.ReadBoolean(),
				lastChangeUnixTime = reader.ReadInt64()
			};
		}

		public void UpdateTimestamp()
		{
			lastChangeUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		}

		public readonly bool IsDefault()
		{
			if (volume == 1f)
			{
				return !isMuted;
			}
			return false;
		}
	}

	private const int persistentPlayerStatusVersion = 0;

	private const long secondsInMonth = 2592000L;

	private static readonly Dictionary<ulong, PersistentPlayerStatus> playerStatusPerGuid = new Dictionary<ulong, PersistentPlayerStatus>();

	public VoiceNetworker voiceNetworker;

	private bool initialized;

	private bool wasTalking;

	private NetworkIdentity netId;

	private PlayerCosmeticsSwitcher cosmeticsSwitcher;

	private PlayerInfo playerInfo;

	private bool isMuted;

	private float volume;

	public static void InitializeStatics()
	{
		if (!File.Exists(GameSettings.voiceChatSettingsPath))
		{
			return;
		}
		using FileStream input = new FileStream(GameSettings.voiceChatSettingsPath, FileMode.Open, FileAccess.Read);
		using BinaryReader binaryReader = new BinaryReader(input);
		long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		try
		{
			if (binaryReader.ReadInt32() != 0)
			{
				return;
			}
			int num2 = binaryReader.ReadInt32();
			playerStatusPerGuid.EnsureCapacity(num2);
			for (int i = 0; i < num2; i++)
			{
				ulong key = binaryReader.ReadUInt64();
				PersistentPlayerStatus value = PersistentPlayerStatus.Read(binaryReader);
				if (num - value.lastChangeUnixTime <= 2592000)
				{
					playerStatusPerGuid[key] = value;
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception while deserializing voice chat data. See next log for details");
			Debug.LogException(exception);
			playerStatusPerGuid.Clear();
		}
	}

	public static void SetPlayerVolume(ulong guid, float volume)
	{
		if (!playerStatusPerGuid.TryGetValue(guid, out var value))
		{
			value = PersistentPlayerStatus.Default;
		}
		value.volume = volume;
		value.UpdateTimestamp();
		playerStatusPerGuid[guid] = value;
		UpdateInstancePersistentPlayerStatus(guid);
	}

	public static void SetIsPlayerMuted(ulong guid, bool muted)
	{
		if (!playerStatusPerGuid.TryGetValue(guid, out var value))
		{
			value = PersistentPlayerStatus.Default;
		}
		value.isMuted = muted;
		value.UpdateTimestamp();
		playerStatusPerGuid[guid] = value;
		UpdateInstancePersistentPlayerStatus(guid);
	}

	private static void UpdateInstancePersistentPlayerStatus(ulong guid)
	{
		if (GameManager.TryFindPlayerByGuid(guid, out var playerInfo))
		{
			playerInfo.VoiceChat.UpdatePersistentPlayerStatus();
		}
	}

	public static PersistentPlayerStatus GetPlayerStatus(ulong guid)
	{
		if (!playerStatusPerGuid.TryGetValue(guid, out var value))
		{
			return PersistentPlayerStatus.Default;
		}
		return value;
	}

	public static bool IsMuted(ulong guid)
	{
		if (playerStatusPerGuid.TryGetValue(guid, out var value))
		{
			return value.isMuted;
		}
		return false;
	}

	public static void OnApplicationShuttingDown()
	{
		using FileStream output = new FileStream(GameSettings.voiceChatSettingsPath, FileMode.Create, FileAccess.Write);
		using BinaryWriter binaryWriter = new BinaryWriter(output);
		binaryWriter.Write(0);
		binaryWriter.Write(playerStatusPerGuid.Count);
		int num = 0;
		foreach (KeyValuePair<ulong, PersistentPlayerStatus> item in playerStatusPerGuid)
		{
			if (!item.Value.IsDefault())
			{
				binaryWriter.Write(item.Key);
				item.Value.Write(binaryWriter);
				num++;
			}
		}
		binaryWriter.Seek(4, SeekOrigin.Begin);
		binaryWriter.Write(num);
	}

	private void Start()
	{
		playerInfo = GetComponent<PlayerInfo>();
		netId = GetComponent<NetworkIdentity>();
		cosmeticsSwitcher = GetComponentInChildren<PlayerCosmeticsSwitcher>();
		if (voiceNetworker.IsInitialized)
		{
			OnVoiceNetworkerInitialized();
		}
		else
		{
			voiceNetworker.Initialized += OnVoiceNetworkerInitialized;
		}
		if (playerInfo.PlayerId.Guid == 0L)
		{
			playerInfo.PlayerId.GuidChanged += OnPlayerGuidChanged;
		}
		GameSettings.AudioSettings.AnyMicSettingChanged += UpdatePersistentPlayerStatus;
		GameSettings.GeneralSettings.MuteChatChanged = (Action)Delegate.Combine(GameSettings.GeneralSettings.MuteChatChanged, new Action(UpdatePersistentPlayerStatus));
		BNetworkManager.SteamPlayerRelationshipChanged += SteamRelationshipUpdated;
	}

	private void UpdatePersistentPlayerStatus()
	{
		if (playerInfo.PlayerId.Guid != 0L && voiceNetworker.IsInitialized)
		{
			PersistentPlayerStatus playerStatus = GetPlayerStatus(playerInfo.PlayerId.Guid);
			volume = playerStatus.volume;
			isMuted = playerStatus.isMuted;
			isMuted |= GameSettings.All.General.MuteChat;
			isMuted |= playerInfo.IsBlockedOnSteam();
			voiceNetworker.SetOutputVolume(volume);
			voiceNetworker.muted = isMuted;
			if (netId.isLocalPlayer)
			{
				voiceNetworker.RecordingVolume = GameSettings.All.Audio.MicInputVolume;
				voiceNetworker.threshold = GameSettings.All.Audio.MicInputThreshold;
			}
		}
	}

	private void SteamRelationshipUpdated(ulong guid, Relationship relationship)
	{
		if (guid == playerInfo.PlayerId.Guid)
		{
			UpdatePersistentPlayerStatus();
		}
	}

	private void Update()
	{
		if (!netId.isLocalPlayer)
		{
			return;
		}
		bool flag = ShouldRecord();
		if (GameSettings.All.Audio.MicInputMode == GameSettings.AudioSettings.InputMode.PushToTalk || GameSettings.All.Audio.MicInputMode == GameSettings.AudioSettings.InputMode.PushToToggle)
		{
			voiceNetworker.threshold = (playerInfo.Input.IsPushingToTalk ? GameSettings.All.Audio.MicInputThreshold : 1f);
		}
		if (flag != voiceNetworker.IsRecording)
		{
			if (flag)
			{
				voiceNetworker.StartRecording();
			}
			else
			{
				voiceNetworker.StopRecording();
			}
		}
		bool ShouldRecord()
		{
			return !isMuted;
		}
	}

	public void InformPlayerFrozen(bool isFrozen)
	{
		voiceNetworker.SetFrozen(isFrozen);
	}

	private void LateUpdate()
	{
		if (voiceNetworker.IsTalking)
		{
			wasTalking = true;
			float slowSmoothedNormalizedVolume = voiceNetworker.SlowSmoothedNormalizedVolume;
			cosmeticsSwitcher.SetTalkingMagnitude(slowSmoothedNormalizedVolume);
		}
		else if (wasTalking)
		{
			wasTalking = false;
			cosmeticsSwitcher.StopTalking();
		}
	}

	private void OnDestroy()
	{
		GameSettings.AudioSettings.AnyMicSettingChanged -= UpdatePersistentPlayerStatus;
		GameSettings.GeneralSettings.MuteChatChanged = (Action)Delegate.Remove(GameSettings.GeneralSettings.MuteChatChanged, new Action(UpdatePersistentPlayerStatus));
		BNetworkManager.SteamPlayerRelationshipChanged -= SteamRelationshipUpdated;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			playerInfo.PlayerId.GuidChanged -= OnPlayerGuidChanged;
			voiceNetworker.Initialized -= OnVoiceNetworkerInitialized;
		}
	}

	private void OnVoiceNetworkerInitialized()
	{
		voiceNetworker.Initialized -= OnVoiceNetworkerInitialized;
		initialized = true;
		UpdateInputDevice(GameSettings.All.Audio.InputDeviceId);
		UpdateSpatialSetting();
		UpdateVoiceEffects();
		UpdatePersistentPlayerStatus();
	}

	public void UpdateInputDevice(int deviceId)
	{
		if (playerInfo.isLocalPlayer)
		{
			voiceNetworker.SetRecordDriver(deviceId);
		}
	}

	public void UpdateSpatialSetting()
	{
		voiceNetworker.Set2DFade(GameSettings.All.Audio.VoiceChatSpatialAudio ? 0.25f : 1f);
	}

	private void UpdateVoiceEffects()
	{
		voiceNetworker.SetFrozen(apply: false);
	}

	private void OnPlayerGuidChanged()
	{
		UpdatePersistentPlayerStatus();
	}
}
