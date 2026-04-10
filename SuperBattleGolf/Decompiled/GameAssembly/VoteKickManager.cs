using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class VoteKickManager : SingletonNetworkBehaviour<VoteKickManager>, IBUpdateCallback, IAnyBUpdateCallback
{
	public enum Vote
	{
		NotAllowed = -1,
		NotVoted,
		Yes,
		No
	}

	public struct VoteResults
	{
		public byte yesVotes;

		public byte noVotes;
	}

	private const int MIN_VOTEKICK_PLAYERCOUNT = 2;

	[SyncVar]
	private VoteResults voteResults;

	private readonly SyncDictionary<ulong, Vote> voters = new SyncDictionary<ulong, Vote>();

	private Dictionary<ulong, double> voteInitCooldown = new Dictionary<ulong, double>();

	private ulong votekickTargetGuid;

	private ulong votekickRequesterGuid;

	private bool voteActive;

	private double voteStartTime;

	private string votePlayerName;

	private Coroutine votekickRoutine;

	private bool voteInputActive;

	[CVar("voteDuration", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static float voteDuration;

	[CVar("votekickCooldown", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static int votekickFailCooldown;

	[CVar("votekickPlayerMinActiveTime", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static int votekickPlayerMinActiveTime;

	public static bool CanVotekick
	{
		get
		{
			if (CourseManager.CountActivePlayers() > 2)
			{
				return !CanKick;
			}
			return false;
		}
	}

	public static bool CanKickOrVotekick
	{
		get
		{
			if (!CanVotekick)
			{
				return NetworkServer.active;
			}
			return true;
		}
	}

	public static bool CanKick
	{
		get
		{
			if (NetworkServer.active)
			{
				if (SingletonNetworkBehaviour<MatchSetupMenu>.Instance.lobbyMode == LobbyMode.Public && CourseManager.CountActivePlayers() > 2)
				{
					return SingletonBehaviour<DrivingRangeManager>.HasInstance;
				}
				return true;
			}
			return false;
		}
	}

	public static float NormalizedProgress
	{
		get
		{
			if (SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
			{
				return BMath.GetTimeSince(SingletonNetworkBehaviour<VoteKickManager>.Instance.voteStartTime) / voteDuration;
			}
			return 0f;
		}
	}

	public static int YesVotes
	{
		get
		{
			if (!SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
			{
				return 0;
			}
			return SingletonNetworkBehaviour<VoteKickManager>.Instance.voteResults.yesVotes;
		}
	}

	public static int NoVotes
	{
		get
		{
			if (!SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
			{
				return 0;
			}
			return SingletonNetworkBehaviour<VoteKickManager>.Instance.voteResults.noVotes;
		}
	}

	public static int TotalVoterCount
	{
		get
		{
			if (!SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
			{
				return 0;
			}
			return SingletonNetworkBehaviour<VoteKickManager>.Instance.voters.Count;
		}
	}

	public VoteResults NetworkvoteResults
	{
		get
		{
			return voteResults;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref voteResults, 1uL, null);
		}
	}

	public static void BeginKick(PlayerInfo playerInfo)
	{
		if (SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VoteKickManager>.Instance.BeginKickInternal(playerInfo);
		}
	}

	public static Vote GetVote(PlayerInfo playerInfo)
	{
		if (!SingletonNetworkBehaviour<VoteKickManager>.HasInstance || !SingletonNetworkBehaviour<VoteKickManager>.Instance.voters.TryGetValue(playerInfo.PlayerId.Guid, out var value))
		{
			return Vote.NotAllowed;
		}
		return value;
	}

	protected override void Awake()
	{
		base.Awake();
		SetInputMode(enabled: false);
		InputManager.Controls.Vote.Yes.started += VoteYes;
		InputManager.Controls.Vote.No.started += VoteNo;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SetInputMode(enabled: false);
		InputManager.Controls.Vote.Yes.started -= VoteYes;
		InputManager.Controls.Vote.No.started -= VoteNo;
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		VoteKickUi.UpdateValues();
	}

	private void BeginKickInternal(PlayerInfo playerInfo)
	{
		if (CanKick)
		{
			FullScreenMessage.Show(string.Format(Localization.UI.MATCHSETUP_KickPlayer, playerInfo.PlayerId.PlayerNameNoRichText), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
			{
				FullScreenMessage.Hide();
				BNetworkManager.singleton.ServerKickConnection(playerInfo.connectionToClient);
			}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
		}
		else
		{
			if (!CanVotekick)
			{
				return;
			}
			FullScreenMessage.Show(string.Format(Localization.UI.MATCHSETUP_Votekick, playerInfo.PlayerId.PlayerNameNoRichText), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
			{
				FullScreenMessage.Hide();
				if (playerInfo != null)
				{
					CmdRequestVoteKick(playerInfo);
				}
			}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
		}
	}

	[Command(requiresAuthority = false)]
	private void CmdRequestVoteKick(PlayerInfo playerInfo, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient(playerInfo, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteNetworkBehaviour(playerInfo);
		SendCommandInternal("System.Void VoteKickManager::CmdRequestVoteKick(PlayerInfo,Mirror.NetworkConnectionToClient)", -677187361, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[Command(requiresAuthority = false)]
	private void CmdVote(Vote castVote, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdVote__Vote__NetworkConnectionToClient(castVote, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_VoteKickManager_002FVote(writer, castVote);
		SendCommandInternal("System.Void VoteKickManager::CmdVote(VoteKickManager/Vote,Mirror.NetworkConnectionToClient)", -1609632505, writer, 0, requiresAuthority: false);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcBeginVotekick(ulong targetGuid, ulong requesterGuid)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteVarULong(targetGuid);
		writer.WriteVarULong(requesterGuid);
		SendRPCInternal("System.Void VoteKickManager::RpcBeginVotekick(System.UInt64,System.UInt64)", 370612938, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[ClientRpc]
	private void RpcEndVotekick(bool success)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(success);
		SendRPCInternal("System.Void VoteKickManager::RpcEndVotekick(System.Boolean)", 1979572269, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformCooldown(NetworkConnectionToClient connection, float timeRemaining)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteFloat(timeRemaining);
		SendTargetRPCInternal(connection, "System.Void VoteKickManager::RpcInformCooldown(Mirror.NetworkConnectionToClient,System.Single)", -1886835601, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ShowCooldownMessage(float timeRemaining)
	{
		FullScreenMessage.Show(string.Format(Localization.UI.VOTEKICK_Cooldown, timeRemaining.ToString("0.0")), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Ok, FullScreenMessage.Hide));
	}

	[TargetRpc]
	private void RpcInformAlreadyOngoing(NetworkConnectionToClient connection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		SendTargetRPCInternal(connection, "System.Void VoteKickManager::RpcInformAlreadyOngoing(Mirror.NetworkConnectionToClient)", -1821195072, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ShowAlreadyOngoingMessage()
	{
		FullScreenMessage.Show(Localization.UI.VOTEKICK_Ongoing, new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Ok, FullScreenMessage.Hide));
	}

	private void FinalizeVote()
	{
		if (!voteActive)
		{
			return;
		}
		if (votekickRoutine != null)
		{
			StopCoroutine(votekickRoutine);
		}
		if (votekickTargetGuid == 0L)
		{
			RpcEndVotekick(success: false);
			return;
		}
		if (voteResults.yesVotes > voteResults.noVotes)
		{
			Debug.Log($"Vote successful! Kicking player with guid {votekickTargetGuid}");
			RpcEndVotekick(success: true);
			if (GameManager.TryFindPlayerByGuid(votekickTargetGuid, out var playerInfo))
			{
				BNetworkManager.singleton.ServerKickConnection(playerInfo.connectionToClient);
			}
			else
			{
				BNetworkManager.singleton.BanPlayerGuidThisSession(votekickTargetGuid);
			}
		}
		else
		{
			Debug.Log($"Vote failed! Don't kick player with guid {votekickTargetGuid}");
			RpcEndVotekick(success: false);
			if (votekickRequesterGuid != 0L)
			{
				voteInitCooldown[votekickRequesterGuid] = Time.timeAsDouble;
			}
		}
		voters.Clear();
		votekickRequesterGuid = 0uL;
		votekickTargetGuid = 0uL;
		voteActive = false;
	}

	private void VoteYes(InputAction.CallbackContext context)
	{
		CmdVote(Vote.Yes);
		SetInputMode(enabled: false);
	}

	private void VoteNo(InputAction.CallbackContext context)
	{
		CmdVote(Vote.No);
		SetInputMode(enabled: false);
	}

	[Server]
	private bool ServerTryGetGuidFromConnection(NetworkConnectionToClient conn, out ulong guid)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean VoteKickManager::ServerTryGetGuidFromConnection(Mirror.NetworkConnectionToClient,System.UInt64&)' called when server was not active");
			guid = default(ulong);
			return default(bool);
		}
		if (conn == null)
		{
			guid = GameManager.LocalPlayerId.Guid;
			return true;
		}
		return BNetworkManager.singleton.playerGuidPerConnectionId.TryGetValue(conn.connectionId, out guid);
	}

	private void SetInputMode(bool enabled)
	{
		if (voteInputActive != enabled)
		{
			voteInputActive = enabled;
			if (enabled)
			{
				InputManager.EnableOverrideMode(InputOverride.Vote);
			}
			else
			{
				InputManager.DisableOverrideMode(InputOverride.Vote);
			}
		}
	}

	public VoteKickManager()
	{
		InitSyncObject(voters);
	}

	static VoteKickManager()
	{
		voteDuration = 20f;
		votekickFailCooldown = 120;
		votekickPlayerMinActiveTime = 300;
		RemoteProcedureCalls.RegisterCommand(typeof(VoteKickManager), "System.Void VoteKickManager::CmdRequestVoteKick(PlayerInfo,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VoteKickManager), "System.Void VoteKickManager::CmdVote(VoteKickManager/Vote,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdVote__Vote__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcBeginVotekick(System.UInt64,System.UInt64)", InvokeUserCode_RpcBeginVotekick__UInt64__UInt64);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcEndVotekick(System.Boolean)", InvokeUserCode_RpcEndVotekick__Boolean);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcInformCooldown(Mirror.NetworkConnectionToClient,System.Single)", InvokeUserCode_RpcInformCooldown__NetworkConnectionToClient__Single);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcInformAlreadyOngoing(Mirror.NetworkConnectionToClient)", InvokeUserCode_RpcInformAlreadyOngoing__NetworkConnectionToClient);
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient(PlayerInfo playerInfo, NetworkConnectionToClient sender)
	{
		ulong requesterGuid;
		PlayerInfo requester;
		if (voteActive)
		{
			if (sender == null)
			{
				ShowAlreadyOngoingMessage();
			}
			else
			{
				RpcInformAlreadyOngoing(sender);
			}
		}
		else
		{
			if (playerInfo == null || playerInfo.PlayerId.Guid == votekickTargetGuid || !ServerTryGetGuidFromConnection(sender, out requesterGuid) || !GameManager.TryFindPlayerByGuid(requesterGuid, out requester))
			{
				return;
			}
			if (IsOnCooldown(out var remaining))
			{
				Debug.Log($"{requester.PlayerId.name} is on votekick cooldown! ({remaining}s)");
				if (sender == null)
				{
					ShowCooldownMessage(remaining);
				}
				else
				{
					RpcInformCooldown(sender, remaining);
				}
				return;
			}
			votekickTargetGuid = playerInfo.PlayerId.Guid;
			votekickRequesterGuid = requester.PlayerId.Guid;
			voters.Clear();
			foreach (CourseManager.PlayerState playerState in CourseManager.PlayerStates)
			{
				if (playerState.isConnected)
				{
					Vote value = ((playerState.playerGuid == requesterGuid) ? Vote.Yes : ((playerState.playerGuid == playerInfo.PlayerId.Guid) ? Vote.No : Vote.NotVoted));
					voters[playerState.playerGuid] = value;
				}
			}
			NetworkvoteResults = new VoteResults
			{
				yesVotes = 1,
				noVotes = 1
			};
			voteStartTime = Time.timeAsDouble;
			voteActive = true;
			RpcBeginVotekick(votekickTargetGuid, requester.PlayerId.Guid);
			if (votekickRoutine != null)
			{
				StopCoroutine(votekickRoutine);
			}
			votekickRoutine = StartCoroutine(VotekickRoutine());
		}
		bool IsOnCooldown(out float reference)
		{
			reference = float.MinValue;
			if (voteInitCooldown.TryGetValue(requesterGuid, out var value2) && BMath.GetTimeSince(value2) < (float)votekickFailCooldown)
			{
				reference = BMath.Max((float)votekickFailCooldown - BMath.GetTimeSince(value2), reference);
			}
			if (!requester.isLocalPlayer && CourseManager.TryGetPlayerState(requester, out var state) && NetworkTime.time - state.joinTimestamp < (double)votekickPlayerMinActiveTime)
			{
				reference = BMath.Max((float)votekickPlayerMinActiveTime - (float)(NetworkTime.time - state.joinTimestamp), reference);
			}
			return reference > 0f;
		}
		IEnumerator VotekickRoutine()
		{
			while (NormalizedProgress < 1f)
			{
				yield return null;
			}
			FinalizeVote();
		}
	}

	protected static void InvokeUserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdRequestVoteKick called on client.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient(reader.ReadNetworkBehaviour<PlayerInfo>(), senderConnection);
		}
	}

	protected void UserCode_CmdVote__Vote__NetworkConnectionToClient(Vote castVote, NetworkConnectionToClient sender)
	{
		if (castVote == Vote.NotVoted || !ServerTryGetGuidFromConnection(sender, out var guid))
		{
			return;
		}
		if (!voters.TryGetValue(guid, out var value) || value != Vote.NotVoted)
		{
			Debug.Log($"Player guid {guid} is not allowed to vote or has already voted");
			return;
		}
		voters[guid] = castVote;
		VoteResults networkvoteResults = voteResults;
		switch (castVote)
		{
		case Vote.Yes:
			networkvoteResults.yesVotes++;
			break;
		case Vote.No:
			networkvoteResults.noVotes++;
			break;
		}
		NetworkvoteResults = networkvoteResults;
		int num = 0;
		List<CourseManager.PlayerState> value2;
		using (CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Get(out value2))
		{
			CourseManager.GetConnectedPlayerStates(value2);
			foreach (CourseManager.PlayerState item in value2)
			{
				if (voters.ContainsKey(item.playerGuid))
				{
					num++;
				}
			}
			int num2 = num / 2;
			if (voteResults.noVotes >= num2 || voteResults.yesVotes > num2)
			{
				FinalizeVote();
			}
		}
	}

	protected static void InvokeUserCode_CmdVote__Vote__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdVote called on client.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_CmdVote__Vote__NetworkConnectionToClient(GeneratedNetworkCode._Read_VoteKickManager_002FVote(reader), senderConnection);
		}
	}

	protected void UserCode_RpcBeginVotekick__UInt64__UInt64(ulong targetGuid, ulong requesterGuid)
	{
		if (!GameManager.TryFindPlayerByGuid(targetGuid, out var playerInfo))
		{
			Debug.LogError("Could not find votekick target!");
			return;
		}
		if (!GameManager.TryFindPlayerByGuid(requesterGuid, out var playerInfo2))
		{
			Debug.LogError("Could not find votekick requester!");
			return;
		}
		VoteKickUi.Show(playerInfo, playerInfo2);
		SetInputMode(enabled: true);
		BUpdate.RegisterCallback(this);
		voteStartTime = Time.timeAsDouble;
		votePlayerName = playerInfo.PlayerId.PlayerNameNoRichText;
	}

	protected static void InvokeUserCode_RpcBeginVotekick__UInt64__UInt64(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcBeginVotekick called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcBeginVotekick__UInt64__UInt64(reader.ReadVarULong(), reader.ReadVarULong());
		}
	}

	protected void UserCode_RpcEndVotekick__Boolean(bool success)
	{
		VoteKickUi.Hide();
		SetInputMode(enabled: false);
		BUpdate.DeregisterCallback(this);
		if (success)
		{
			TextChatUi.ShowMessage(string.Format(Localization.UI.VOTEKICK_Successful, GameManager.UiSettings.ApplyColorTag(votePlayerName, TextHighlight.Red)));
		}
		else
		{
			TextChatUi.ShowMessage(Localization.UI.VOTEKICK_Failed);
		}
		votePlayerName = string.Empty;
	}

	protected static void InvokeUserCode_RpcEndVotekick__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcEndVotekick called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcEndVotekick__Boolean(reader.ReadBool());
		}
	}

	protected void UserCode_RpcInformCooldown__NetworkConnectionToClient__Single(NetworkConnectionToClient connection, float timeRemaining)
	{
		ShowCooldownMessage(timeRemaining);
	}

	protected static void InvokeUserCode_RpcInformCooldown__NetworkConnectionToClient__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformCooldown called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcInformCooldown__NetworkConnectionToClient__Single(null, reader.ReadFloat());
		}
	}

	protected void UserCode_RpcInformAlreadyOngoing__NetworkConnectionToClient(NetworkConnectionToClient connection)
	{
		ShowAlreadyOngoingMessage();
	}

	protected static void InvokeUserCode_RpcInformAlreadyOngoing__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformAlreadyOngoing called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcInformAlreadyOngoing__NetworkConnectionToClient(null);
		}
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			GeneratedNetworkCode._Write_VoteKickManager_002FVoteResults(writer, voteResults);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_VoteKickManager_002FVoteResults(writer, voteResults);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref voteResults, null, GeneratedNetworkCode._Read_VoteKickManager_002FVoteResults(reader));
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref voteResults, null, GeneratedNetworkCode._Read_VoteKickManager_002FVoteResults(reader));
		}
	}
}
