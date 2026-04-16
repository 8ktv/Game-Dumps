using System;
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

	private enum HostVeto
	{
		None,
		ForceNo,
		ForceYes
	}

	private enum VoteKickResult
	{
		Success,
		Failed,
		NotEnoughVotes,
		HostVeto
	}

	public struct VoteResults
	{
		public byte yesVotes;

		public byte noVotes;
	}

	private const int MIN_VOTEKICK_PLAYERCOUNT = 2;

	[SyncVar]
	private VoteResults voteResults;

	private SyncDictionary<ulong, Vote> voters = new SyncDictionary<ulong, Vote>();

	private readonly Dictionary<ulong, double> voteInitCooldown = new Dictionary<ulong, double>();

	private ulong votekickTargetGuid;

	private ulong votekickRequesterGuid;

	private bool votekickTargetWasNewInLobbyWhenVoteStarted;

	private bool voteActive;

	private double voteStartTime;

	private string votePlayerName;

	private Coroutine votekickRoutine;

	private bool voteInputActive;

	[CVar("voteDuration", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static float voteDuration;

	[CVar("votekickCooldown", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static int votekickFailCooldown;

	[SyncVar]
	private int votekickPlayerMinActiveTime = 900;

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

	public static int VoteKickPlayerMinActiveTime
	{
		get
		{
			if (!SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
			{
				return -1;
			}
			return SingletonNetworkBehaviour<VoteKickManager>.Instance.votekickPlayerMinActiveTime;
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

	public int NetworkvotekickPlayerMinActiveTime
	{
		get
		{
			return votekickPlayerMinActiveTime;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref votekickPlayerMinActiveTime, 2uL, null);
		}
	}

	[CCommand("setVotekickPlayerMinActiveTime", "", false, false)]
	private static void SetVotekickMinActiveTime(int time)
	{
		if (SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VoteKickManager>.Instance.NetworkvotekickPlayerMinActiveTime = time;
		}
	}

	public static bool CanKickFreely()
	{
		if (!NetworkServer.active)
		{
			return false;
		}
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return true;
		}
		if (SingletonNetworkBehaviour<MatchSetupMenu>.Instance.lobbyMode != LobbyMode.Public)
		{
			return true;
		}
		if (CourseManager.CountActivePlayers() <= 2)
		{
			return true;
		}
		return false;
	}

	public static bool CanInitiateVotekickAtAll()
	{
		if (CanKickFreely())
		{
			return false;
		}
		if (CourseManager.CountActivePlayers() <= 2)
		{
			return false;
		}
		return true;
	}

	public static bool CanPlayerInitiateVotekick(ulong playerGuid, out bool dueToMinActiveTime)
	{
		if (!CourseManager.TryGetPlayerState(playerGuid, out var state))
		{
			dueToMinActiveTime = false;
			return false;
		}
		return CanPlayerInitiateVotekick(state, out dueToMinActiveTime);
	}

	public static bool CanPlayerInitiateVotekick(CourseManager.PlayerState playerState, out bool dueToMinActiveTime)
	{
		dueToMinActiveTime = false;
		if (!CanInitiateVotekickAtAll())
		{
			return false;
		}
		if (!CanParticipateInVote(playerState, out var remainingTime))
		{
			dueToMinActiveTime = remainingTime > 0f;
			return false;
		}
		return true;
	}

	public static bool CanKickPlayerImmediately(ulong playerToKickGuid)
	{
		if (!CourseManager.TryGetPlayerState(playerToKickGuid, out var state))
		{
			return false;
		}
		return CanKickPlayerImmediately(state);
	}

	public static bool CanKickPlayerImmediately(CourseManager.PlayerState playerToKickState)
	{
		if (!NetworkServer.active)
		{
			return false;
		}
		if (playerToKickState.playerGuid == GameManager.LocalPlayerId.Guid)
		{
			return false;
		}
		if (CanKickFreely())
		{
			return true;
		}
		if (CanParticipateInVote(playerToKickState, out var _))
		{
			return false;
		}
		return true;
	}

	public static void BeginKick(PlayerInfo playerToKick)
	{
		if (SingletonNetworkBehaviour<VoteKickManager>.HasInstance)
		{
			SingletonNetworkBehaviour<VoteKickManager>.Instance.BeginKickInternal(playerToKick);
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
		SyncDictionary<ulong, Vote> syncDictionary = voters;
		syncDictionary.OnChange = (Action<SyncIDictionary<ulong, Vote>.Operation, ulong, Vote>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, Vote>.Operation, ulong, Vote>(OnVotersChanged));
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

	private void BeginKickInternal(PlayerInfo playerToKick)
	{
		if (!CourseManager.TryGetPlayerState(playerToKick, out var state))
		{
			return;
		}
		if (state.isHost)
		{
			Debug.LogError("Attempted to kick server host");
		}
		else if (CanKickPlayerImmediately(state))
		{
			FullScreenMessage.Show(string.Format(Localization.UI.MATCHSETUP_KickPlayer, playerToKick.PlayerId.PlayerNameNoRichText), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
			{
				FullScreenMessage.Hide();
				BNetworkManager.singleton.ServerKickConnection(playerToKick.connectionToClient);
			}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
		}
		else
		{
			if (!CanInitiateVotekickAtAll())
			{
				return;
			}
			FullScreenMessage.Show(string.Format(Localization.UI.MATCHSETUP_Votekick, playerToKick.PlayerId.PlayerNameNoRichText), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Yes, delegate
			{
				FullScreenMessage.Hide();
				if (playerToKick != null)
				{
					CmdRequestVoteKick(playerToKick);
				}
			}), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Cancel, FullScreenMessage.Hide));
		}
	}

	private static bool CanParticipateInVote(CourseManager.PlayerState playerState, out float remainingTime)
	{
		remainingTime = 0f;
		if (playerState.isHost)
		{
			return true;
		}
		if (SingletonNetworkBehaviour<MatchSetupMenu>.Instance.lobbyMode != LobbyMode.Public)
		{
			return true;
		}
		float num = (float)(NetworkTime.time - playerState.joinTimestamp);
		remainingTime = (float)VoteKickPlayerMinActiveTime - num;
		if (remainingTime <= 0f)
		{
			return true;
		}
		return false;
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

	private int CountValidVoters()
	{
		int num = 0;
		List<CourseManager.PlayerState> value;
		using (CollectionPool<List<CourseManager.PlayerState>, CourseManager.PlayerState>.Get(out value))
		{
			CourseManager.GetConnectedPlayerStates(value);
			foreach (CourseManager.PlayerState item in value)
			{
				if (voters.ContainsKey(item.playerGuid))
				{
					num++;
				}
			}
			return num;
		}
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
	private void RpcEndVotekick(VoteKickResult result)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_VoteKickManager_002FVoteKickResult(writer, result);
		SendRPCInternal("System.Void VoteKickManager::RpcEndVotekick(VoteKickManager/VoteKickResult)", -2104078525, writer, 0, includeOwner: true);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcInformCooldown(NetworkConnectionToClient connection, bool dueToNewPlayer, float secondsRemaining)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(dueToNewPlayer);
		writer.WriteFloat(secondsRemaining);
		SendTargetRPCInternal(connection, "System.Void VoteKickManager::RpcInformCooldown(Mirror.NetworkConnectionToClient,System.Boolean,System.Single)", 1459178440, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void ShowCooldownMessage(bool dueToNewPlayer, float secondsRemaining)
	{
		string format = (dueToNewPlayer ? Localization.UI.VOTEKICK_Cooldown_NewPlayer : Localization.UI.VOTEKICK_Cooldown);
		TimeSpan timeSpan = TimeSpan.FromSeconds(secondsRemaining);
		string arg = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		FullScreenMessage.Show(string.Format(format, arg), new FullScreenMessage.ButtonEntry(Localization.UI.MISC_Ok, FullScreenMessage.Hide));
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

	private void UpdateIsUiShown()
	{
		bool flag = VoteKickUi.IsShown && VoteKickUi.TargetGuid == votekickTargetGuid && VoteKickUi.RequesterGuid == votekickRequesterGuid;
		bool flag2 = ShouldBeShown();
		if (flag2 != flag)
		{
			PlayerInfo playerInfo2;
			if (!GameManager.TryFindPlayerByGuid(votekickTargetGuid, out var playerInfo))
			{
				Debug.LogError("Could not find votekick target");
			}
			else if (!GameManager.TryFindPlayerByGuid(votekickRequesterGuid, out playerInfo2))
			{
				Debug.LogError("Could not find votekick requester");
			}
			else if (flag2)
			{
				VoteKickUi.Show(playerInfo, playerInfo2);
			}
			else
			{
				VoteKickUi.Hide();
			}
		}
		bool ShouldBeShown()
		{
			if (!voteActive)
			{
				return false;
			}
			if (GameManager.LocalPlayerId == null)
			{
				return false;
			}
			ulong guid = GameManager.LocalPlayerId.Guid;
			if (guid == votekickTargetGuid)
			{
				return true;
			}
			if (guid == votekickRequesterGuid)
			{
				return true;
			}
			if (voters.ContainsKey(guid))
			{
				return true;
			}
			return false;
		}
	}

	private void FinalizeVote(HostVeto hostVeto)
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
			RpcEndVotekick(VoteKickResult.Failed);
			return;
		}
		int num = BMath.CeilToInt((float)CountValidVoters() / 3f);
		bool flag = voteResults.yesVotes > voteResults.noVotes;
		bool flag2 = voteResults.yesVotes >= num;
		if (hostVeto == HostVeto.ForceYes || (hostVeto == HostVeto.None && flag && flag2))
		{
			Debug.Log($"Votekick against player with GUID {votekickTargetGuid} succeeded");
			RpcEndVotekick(VoteKickResult.Success);
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
			Debug.Log($"Votekick against player with GUID {votekickTargetGuid} failed");
			VoteKickResult result = ((hostVeto == HostVeto.ForceNo) ? VoteKickResult.HostVeto : ((!flag || flag2) ? VoteKickResult.Failed : VoteKickResult.NotEnoughVotes));
			RpcEndVotekick(result);
			if (votekickRequesterGuid != 0L)
			{
				voteInitCooldown[votekickRequesterGuid] = Time.timeAsDouble;
			}
		}
		voteActive = false;
		voters.Clear();
		UpdateIsUiShown();
		votekickRequesterGuid = 0uL;
		votekickTargetGuid = 0uL;
	}

	private void VoteYes(InputAction.CallbackContext context)
	{
		if (!voters.ContainsKey(GameManager.LocalPlayerId.Guid))
		{
			Debug.Log("Not allowed to vote!");
			return;
		}
		CmdVote(Vote.Yes);
		SetInputMode(enabled: false);
	}

	private void VoteNo(InputAction.CallbackContext context)
	{
		if (!voters.ContainsKey(GameManager.LocalPlayerId.Guid))
		{
			Debug.Log("Not allowed to vote!");
			return;
		}
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

	private void OnVotersChanged(SyncIDictionary<ulong, Vote>.Operation operation, ulong voteGuid, Vote vote)
	{
		switch (operation)
		{
		case SyncIDictionary<ulong, Vote>.Operation.OP_CLEAR:
			SetInputMode(enabled: false);
			UpdateIsUiShown();
			break;
		case SyncIDictionary<ulong, Vote>.Operation.OP_ADD:
			if (GameManager.LocalPlayerId != null && voteGuid == GameManager.LocalPlayerId.Guid)
			{
				SetInputMode(enabled: true);
				UpdateIsUiShown();
			}
			break;
		case SyncIDictionary<ulong, Vote>.Operation.OP_REMOVE:
			if (GameManager.LocalPlayerId != null && voteGuid == GameManager.LocalPlayerId.Guid)
			{
				SetInputMode(enabled: false);
				UpdateIsUiShown();
			}
			break;
		case SyncIDictionary<ulong, Vote>.Operation.OP_SET:
			break;
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
		RemoteProcedureCalls.RegisterCommand(typeof(VoteKickManager), "System.Void VoteKickManager::CmdRequestVoteKick(PlayerInfo,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdRequestVoteKick__PlayerInfo__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterCommand(typeof(VoteKickManager), "System.Void VoteKickManager::CmdVote(VoteKickManager/Vote,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdVote__Vote__NetworkConnectionToClient, requiresAuthority: false);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcBeginVotekick(System.UInt64,System.UInt64)", InvokeUserCode_RpcBeginVotekick__UInt64__UInt64);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcEndVotekick(VoteKickManager/VoteKickResult)", InvokeUserCode_RpcEndVotekick__VoteKickResult);
		RemoteProcedureCalls.RegisterRpc(typeof(VoteKickManager), "System.Void VoteKickManager::RpcInformCooldown(Mirror.NetworkConnectionToClient,System.Boolean,System.Single)", InvokeUserCode_RpcInformCooldown__NetworkConnectionToClient__Boolean__Single);
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
			if (playerInfo == null || playerInfo.PlayerId.Guid == votekickTargetGuid || !ServerTryGetGuidFromConnection(sender, out requesterGuid) || !GameManager.TryFindPlayerByGuid(requesterGuid, out requester) || !CourseManager.TryGetPlayerState(requester, out var state))
			{
				return;
			}
			if (!CanParticipateInVote(state, out var remainingTime))
			{
				Debug.Log($"{requester.PlayerId.name} attempted to initiate a votekick, but is new to the lobby ({remainingTime:0.0} seconds remaining)");
				RpcInformCooldown(sender, dueToNewPlayer: true, remainingTime);
				return;
			}
			if (IsOnCooldown(out remainingTime))
			{
				Debug.Log($"{requester.PlayerId.name} attempted to initiate a votekick, but is on cooldown ({remainingTime:0.0} seconds remaining)");
				if (sender == null)
				{
					ShowCooldownMessage(dueToNewPlayer: false, remainingTime);
				}
				else
				{
					RpcInformCooldown(sender, dueToNewPlayer: false, remainingTime);
				}
				return;
			}
			votekickTargetGuid = playerInfo.PlayerId.Guid;
			votekickRequesterGuid = requester.PlayerId.Guid;
			voters.Clear();
			foreach (CourseManager.PlayerState playerState in CourseManager.PlayerStates)
			{
				if (!playerState.isConnected)
				{
					continue;
				}
				Vote value;
				if (playerState.playerGuid == requesterGuid)
				{
					value = Vote.Yes;
				}
				else if (playerState.playerGuid == playerInfo.PlayerId.Guid)
				{
					value = Vote.No;
					votekickTargetWasNewInLobbyWhenVoteStarted = !CanParticipateInVote(playerState, out var remainingTime2) && remainingTime2 > 0f;
				}
				else
				{
					if (!CanParticipateInVote(playerState, out var _))
					{
						continue;
					}
					value = Vote.NotVoted;
				}
				voters[playerState.playerGuid] = value;
			}
			NetworkvoteResults = new VoteResults
			{
				yesVotes = 1,
				noVotes = 1
			};
			voteActive = true;
			voteStartTime = Time.timeAsDouble;
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
			if (SingletonNetworkBehaviour<MatchSetupMenu>.Instance.lobbyMode == LobbyMode.Public && !requester.isLocalPlayer && CourseManager.TryGetPlayerState(requester, out var state2) && NetworkTime.time - state2.joinTimestamp < (double)votekickPlayerMinActiveTime)
			{
				reference = BMath.Max((float)votekickPlayerMinActiveTime - (float)(NetworkTime.time - state2.joinTimestamp), reference);
			}
			return reference > 0f;
		}
		IEnumerator VotekickRoutine()
		{
			while (NormalizedProgress < 1f)
			{
				yield return null;
			}
			FinalizeVote(HostVeto.None);
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
		int num = CountValidVoters() / 2;
		HostVeto hostVeto = ((castVote == Vote.No && sender == null) ? HostVeto.ForceNo : ((castVote == Vote.Yes && sender == null && votekickTargetWasNewInLobbyWhenVoteStarted) ? HostVeto.ForceYes : HostVeto.None));
		if ((sender == null || (voters.TryGetValue(GameManager.LocalPlayerId.Guid, out var value2) && value2 != Vote.NotVoted)) && (hostVeto != HostVeto.None || voteResults.noVotes >= num || voteResults.yesVotes > num))
		{
			FinalizeVote(hostVeto);
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
			Debug.LogError("Could not find votekick target");
			return;
		}
		if (!GameManager.TryFindPlayerByGuid(requesterGuid, out var _))
		{
			Debug.LogError("Could not find votekick requester");
			return;
		}
		voteActive = true;
		votekickRequesterGuid = requesterGuid;
		votekickTargetGuid = targetGuid;
		BUpdate.RegisterCallback(this);
		voteStartTime = Time.timeAsDouble;
		votePlayerName = playerInfo.PlayerId.PlayerNameNoRichText;
		UpdateIsUiShown();
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

	protected void UserCode_RpcEndVotekick__VoteKickResult(VoteKickResult result)
	{
		SetInputMode(enabled: false);
		BUpdate.DeregisterCallback(this);
		switch (result)
		{
		case VoteKickResult.Success:
			TextChatUi.ShowMessage(string.Format(Localization.UI.VOTEKICK_Successful, GameManager.UiSettings.ApplyColorTag(votePlayerName, TextHighlight.Red)));
			break;
		case VoteKickResult.NotEnoughVotes:
			TextChatUi.ShowMessage(Localization.UI.VOTEKICK_Failed_NotEnoughVotes);
			break;
		case VoteKickResult.HostVeto:
			TextChatUi.ShowMessage(Localization.UI.VOTEKICK_Failed_HostVeto);
			break;
		default:
			TextChatUi.ShowMessage(Localization.UI.VOTEKICK_Failed);
			break;
		}
		voteActive = false;
		UpdateIsUiShown();
		votePlayerName = string.Empty;
		votekickRequesterGuid = 0uL;
		votekickTargetGuid = 0uL;
	}

	protected static void InvokeUserCode_RpcEndVotekick__VoteKickResult(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcEndVotekick called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcEndVotekick__VoteKickResult(GeneratedNetworkCode._Read_VoteKickManager_002FVoteKickResult(reader));
		}
	}

	protected void UserCode_RpcInformCooldown__NetworkConnectionToClient__Boolean__Single(NetworkConnectionToClient connection, bool dueToNewPlayer, float secondsRemaining)
	{
		ShowCooldownMessage(dueToNewPlayer, secondsRemaining);
	}

	protected static void InvokeUserCode_RpcInformCooldown__NetworkConnectionToClient__Boolean__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcInformCooldown called on server.");
		}
		else
		{
			((VoteKickManager)obj).UserCode_RpcInformCooldown__NetworkConnectionToClient__Boolean__Single(null, reader.ReadBool(), reader.ReadFloat());
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
			writer.WriteVarInt(votekickPlayerMinActiveTime);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			GeneratedNetworkCode._Write_VoteKickManager_002FVoteResults(writer, voteResults);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVarInt(votekickPlayerMinActiveTime);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref voteResults, null, GeneratedNetworkCode._Read_VoteKickManager_002FVoteResults(reader));
			GeneratedSyncVarDeserialize(ref votekickPlayerMinActiveTime, null, reader.ReadVarInt());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref voteResults, null, GeneratedNetworkCode._Read_VoteKickManager_002FVoteResults(reader));
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref votekickPlayerMinActiveTime, null, reader.ReadVarInt());
		}
	}
}
