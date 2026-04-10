using System;
using System.Collections;
using System.Runtime.InteropServices;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

public class PlayerSpectator : NetworkBehaviour
{
	[SyncVar(hook = "OnIsSpectatingChanged")]
	private bool isSpectating;

	private Coroutine spectateStartDelayRoutine;

	private bool isInSpectateStartDelay;

	private Coroutine automaticCycleDelayRoutine;

	private LocalSpectatorCameraFollower spectatorCamera;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isSpectating;

	public PlayerInfo PlayerInfo { get; private set; }

	public bool IsSpectating => isSpectating;

	public PlayerInfo TargetPlayer { get; private set; }

	public GolfBall TargetBall { get; private set; }

	public GolfHole TargetHole { get; private set; }

	public Transform Target { get; private set; }

	public bool NetworkisSpectating
	{
		get
		{
			return isSpectating;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isSpectating, 1uL, _Mirror_SyncVarHookDelegate_isSpectating);
		}
	}

	public event Action IsSpectatingChanged;

	public event Action<PlayerSpectator> IsSpectatingChangedReferenced;

	public static event Action<PlayerSpectator> AnyPlayerIsSpectatingChanged;

	public static event Action LocalPlayerIsSpectatingChanged;

	public static event Action<bool> LocalPlayerSetSpectatingTarget;

	public static event Action LocalPlayerStoppedSpectating;

	public static event Action LocalPlayerSpectatingTargetIsInGolfCartChanged;

	private void Awake()
	{
		PlayerInfo = GetComponent<PlayerInfo>();
		syncDirection = SyncDirection.ClientToServer;
	}

	private void OnDestroy()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			if (TargetPlayer != null)
			{
				TargetPlayer.Movement.Teleported -= OnTargetPlayerTeleported;
				TargetPlayer.AsGolfer.MatchResolutionChanged -= OnTargetPlayerMatchResolutionChanged;
				TargetPlayer.NetworkedEquippedItemIndexChanged -= OnTargetPlayerEquippedItemIndexChanged;
				TargetPlayer.Inventory.ItemSlotSet -= OnTargetPlayerItemSlotsSet;
				TargetPlayer.IsInGolfCartChanged -= OnTargetPlayerIsInGolfCartChanged;
				TargetPlayer.Destroyed -= OnTargetPlayerDestroyed;
			}
			if (TargetBall != null)
			{
				TargetBall.AsEntity.WillBeDestroyed -= OnTargetBallWillBeDestroyed;
			}
		}
	}

	public override void OnStartLocalPlayer()
	{
		PlayerInfo.AsGolfer.MatchResolutionChanged += OnLocalPlayerMatchResolutionChanged;
		GameManager.RemotePlayerRegistered += OnLocalPlayerRemotePlayerRegistered;
		GameManager.RemotePlayerDeregistered += OnLocalPlayerRemotePlayerDeregistered;
		CourseManager.MatchStateChanged += OnLocalPlayerMatchStateChanged;
		CourseManager.OvertimeActiveBallsChanged += OnLocalPlayerOvertimeActiveBallsChanged;
	}

	public override void OnStopLocalPlayer()
	{
		GameManager.RemotePlayerRegistered -= OnLocalPlayerRemotePlayerRegistered;
		GameManager.RemotePlayerDeregistered -= OnLocalPlayerRemotePlayerDeregistered;
		CourseManager.MatchStateChanged -= OnLocalPlayerMatchStateChanged;
		CourseManager.OvertimeActiveBallsChanged -= OnLocalPlayerOvertimeActiveBallsChanged;
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			InputManager.DisableMode(InputMode.Spectate);
			return;
		}
		StopSpectating();
		PlayerInfo.AsGolfer.MatchResolutionChanged -= OnLocalPlayerMatchResolutionChanged;
	}

	public void CyclePreviousTarget(bool canBeginNewSpectate)
	{
		CycleTargetInternal(-1, canBeginNewSpectate);
	}

	public void CycleNextTarget(bool canBeginNewSpectate)
	{
		CycleTargetInternal(1, canBeginNewSpectate);
	}

	public bool CanCycleTarget()
	{
		if (!isSpectating)
		{
			return false;
		}
		if (RadialMenu.IsVisible)
		{
			return false;
		}
		if (RadialMenu.LastSelectionFrame == Time.frameCount)
		{
			return false;
		}
		if (!TryGetCycleTarget(1, out var ballTarget, out var playerTarget, out var holeTarget))
		{
			return false;
		}
		if (ballTarget != null)
		{
			return ballTarget != TargetBall;
		}
		if (playerTarget != null)
		{
			return playerTarget != TargetPlayer;
		}
		if (holeTarget != null)
		{
			return holeTarget != TargetHole;
		}
		return false;
	}

	private void CycleNextTargetDelayed(float delay, bool canBeginNewSpectate)
	{
		if (CourseManager.MatchState < MatchState.Ended)
		{
			if (automaticCycleDelayRoutine != null)
			{
				StopCoroutine(automaticCycleDelayRoutine);
			}
			automaticCycleDelayRoutine = StartCoroutine(SpectateDelayedRoutine());
		}
		IEnumerator SpectateDelayedRoutine()
		{
			yield return new WaitForSeconds(delay);
			CycleNextTarget(canBeginNewSpectate);
		}
	}

	public Bounds GetTargetLocalBounds()
	{
		if (!isSpectating)
		{
			return default(Bounds);
		}
		if (TargetPlayer != null)
		{
			return TargetPlayer.Movement.GetOrbitCameraSubjectLocalBounds();
		}
		if (TargetBall != null)
		{
			return TargetBall.GetOrbitCameraSubjectLocalBounds();
		}
		_ = TargetHole != null;
		return default(Bounds);
	}

	public void StartSpectating()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to start spectating", base.gameObject);
			return;
		}
		CancelSpectateStartDelay();
		CycleNextTarget(canBeginNewSpectate: true);
	}

	public void StartSpectatingDelayed(float delay, bool canRestartDelay)
	{
		if (CourseManager.MatchState < MatchState.Ended && (!isInSpectateStartDelay || canRestartDelay))
		{
			if (spectateStartDelayRoutine != null)
			{
				StopCoroutine(spectateStartDelayRoutine);
			}
			spectateStartDelayRoutine = StartCoroutine(SpectateDelayedRoutine(delay));
		}
		IEnumerator SpectateDelayedRoutine(float seconds)
		{
			isInSpectateStartDelay = true;
			yield return new WaitForSeconds(seconds);
			StartSpectating();
			isInSpectateStartDelay = false;
		}
	}

	public void SetSpectatorCamera(LocalSpectatorCameraFollower camera)
	{
		if (!base.isServer)
		{
			spectatorCamera = camera;
		}
	}

	public void ClearSpectatorCamera()
	{
		if (!base.isServer)
		{
			spectatorCamera = null;
		}
	}

	public void PlayEmote(SpectatorEmote emote)
	{
		PlayEmoteInternal(emote);
		CmdPlayEmoteForAllClients(emote);
	}

	[Command]
	private void CmdPlayEmoteForAllClients(SpectatorEmote emote, NetworkConnectionToClient sender = null)
	{
		if (base.isServer && base.isClient)
		{
			UserCode_CmdPlayEmoteForAllClients__SpectatorEmote__NetworkConnectionToClient(emote, sender);
			return;
		}
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_SpectatorEmote(writer, emote);
		SendCommandInternal("System.Void PlayerSpectator::CmdPlayEmoteForAllClients(SpectatorEmote,Mirror.NetworkConnectionToClient)", -1420198443, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	[TargetRpc]
	private void RpcPlayEmote(NetworkConnectionToClient connection, SpectatorEmote emote)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		GeneratedNetworkCode._Write_SpectatorEmote(writer, emote);
		SendTargetRPCInternal(connection, "System.Void PlayerSpectator::RpcPlayEmote(Mirror.NetworkConnectionToClient,SpectatorEmote)", -464936208, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	private void PlayEmoteInternal(SpectatorEmote emote)
	{
		if (spectatorCamera != null)
		{
			spectatorCamera.PlayEmote(emote);
		}
	}

	private void StopSpectating()
	{
		if (!base.isLocalPlayer)
		{
			Debug.LogError("Only the local player is allowed to stop spectating", base.gameObject);
			return;
		}
		ClearSpecificTargetReferences();
		NetworkisSpectating = false;
		CancelSpectateStartDelay();
		InputManager.DisableMode(InputMode.Spectate);
		PlayerSpectator.LocalPlayerStoppedSpectating?.Invoke();
	}

	private void CancelSpectateStartDelay()
	{
		if (spectateStartDelayRoutine != null)
		{
			StopCoroutine(spectateStartDelayRoutine);
		}
		isInSpectateStartDelay = false;
	}

	private void CycleTargetInternal(int direction, bool canBeginNewSpectate)
	{
		if (canBeginNewSpectate || isSpectating)
		{
			if (!TryGetCycleTarget(direction, out var ballTarget, out var playerTarget, out var holeTarget))
			{
				StopSpectating();
			}
			else if (ballTarget != null)
			{
				SetTarget(ballTarget);
			}
			else if (playerTarget != null)
			{
				SetTarget(playerTarget);
			}
			else if (holeTarget != null)
			{
				SetTarget(holeTarget);
			}
		}
	}

	private bool TryGetCycleTarget(int cycleDirection, out GolfBall ballTarget, out PlayerInfo playerTarget, out GolfHole holeTarget)
	{
		ballTarget = null;
		playerTarget = null;
		holeTarget = null;
		if (CourseManager.MatchState == MatchState.Overtime && CourseManager.OvertimeActiveBalls.Count > 0)
		{
			if (TryGetBallTarget(out ballTarget))
			{
				return true;
			}
		}
		else
		{
			if (TryGetPlayerTarget(out playerTarget))
			{
				return true;
			}
			if (GolfHoleManager.MainHole != null)
			{
				holeTarget = GolfHoleManager.MainHole;
				return true;
			}
		}
		Debug.LogError("Failed to find a spectating target", base.gameObject);
		return false;
		bool TryGetBallTarget(out GolfBall reference)
		{
			if (CourseManager.OvertimeActiveBalls.Count <= 0)
			{
				reference = null;
				return false;
			}
			if (TargetBall == null && CourseManager.OvertimeActiveBalls.Contains(PlayerInfo.AsGolfer.OwnBall))
			{
				reference = PlayerInfo.AsGolfer.OwnBall;
				return true;
			}
			if (TargetBall == null)
			{
				reference = CourseManager.OvertimeActiveBalls[0];
				return true;
			}
			int num = CourseManager.OvertimeActiveBalls.FindIndex((GolfBall ball) => ball == TargetBall);
			int i = BMath.Wrap(((num >= 0) ? num : 0) + cycleDirection, CourseManager.OvertimeActiveBalls.Count);
			reference = CourseManager.OvertimeActiveBalls[i];
			return true;
		}
		bool TryGetPlayerTarget(out PlayerInfo reference)
		{
			if (TargetPlayer == null || !GameManager.TryGetPlayerIndex(TargetPlayer, out var index))
			{
				index = 0;
			}
			for (int i = 0; i < GameManager.RemotePlayers.Count; i++)
			{
				index = BMath.Wrap(index + cycleDirection, GameManager.RemotePlayers.Count);
				PlayerInfo player = GameManager.RemotePlayers[index];
				if (CanSpectatePlayer(player))
				{
					reference = GameManager.RemotePlayers[index];
					return true;
				}
			}
			reference = null;
			return false;
		}
	}

	private void SetTarget(PlayerInfo player)
	{
		if (!(TargetPlayer == player))
		{
			ClearSpecificTargetReferences();
			TargetPlayer = player;
			TargetPlayer.Movement.Teleported += OnTargetPlayerTeleported;
			TargetPlayer.AsGolfer.MatchResolutionChanged += OnTargetPlayerMatchResolutionChanged;
			TargetPlayer.NetworkedEquippedItemIndexChanged += OnTargetPlayerEquippedItemIndexChanged;
			TargetPlayer.Inventory.ItemSlotSet += OnTargetPlayerItemSlotsSet;
			TargetPlayer.IsInGolfCartChanged += OnTargetPlayerIsInGolfCartChanged;
			TargetPlayer.Destroyed += OnTargetPlayerDestroyed;
			SetTargetInternal(player.transform);
			Hotkeys.ForceRefreshCurrentMode();
		}
	}

	private void SetTarget(GolfBall ball)
	{
		if (!(TargetBall == ball))
		{
			if (automaticCycleDelayRoutine != null)
			{
				StopCoroutine(automaticCycleDelayRoutine);
			}
			ClearSpecificTargetReferences();
			TargetBall = ball;
			TargetBall.AsEntity.WillBeDestroyed += OnTargetBallWillBeDestroyed;
			SetTargetInternal(ball.transform);
			Hotkeys.ForceRefreshCurrentMode();
		}
	}

	private void SetTarget(GolfHole hole)
	{
		if (!(TargetHole == hole))
		{
			ClearSpecificTargetReferences();
			TargetHole = hole;
			SetTargetInternal(hole.transform);
		}
	}

	private void SetTargetInternal(Transform target)
	{
		bool obj = !isSpectating;
		NetworkisSpectating = true;
		Target = target;
		InputManager.EnableMode(InputMode.Spectate);
		Hotkeys.SetMode(HotkeyMode.Spectating);
		HoleProgressBarUi.UpdateStrokes();
		PlayerSpectator.LocalPlayerSetSpectatingTarget?.Invoke(obj);
	}

	private void ClearSpecificTargetReferences()
	{
		if (TargetPlayer != null)
		{
			TargetPlayer.Movement.Teleported -= OnTargetPlayerTeleported;
			TargetPlayer.AsGolfer.MatchResolutionChanged -= OnTargetPlayerMatchResolutionChanged;
			TargetPlayer.NetworkedEquippedItemIndexChanged -= OnTargetPlayerEquippedItemIndexChanged;
			TargetPlayer.Inventory.ItemSlotSet -= OnTargetPlayerItemSlotsSet;
			TargetPlayer.IsInGolfCartChanged -= OnTargetPlayerIsInGolfCartChanged;
			TargetPlayer.Destroyed -= OnTargetPlayerDestroyed;
		}
		if (TargetBall != null)
		{
			TargetBall.AsEntity.WillBeDestroyed -= OnTargetBallWillBeDestroyed;
		}
		TargetPlayer = null;
		TargetBall = null;
		TargetHole = null;
		Hotkeys.ForceRefreshCurrentMode();
	}

	private bool CanSpectatePlayer(PlayerInfo player)
	{
		if (player == null)
		{
			return false;
		}
		if (player.AsGolfer.MatchResolution != PlayerMatchResolution.Uninitialized && player.AsGolfer.MatchResolution != PlayerMatchResolution.None)
		{
			return false;
		}
		return true;
	}

	private void OnLocalPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		if (!previousResolution.IsResolved() && currentResolution.IsResolved())
		{
			if (currentResolution == PlayerMatchResolution.JoinedAsSpectator)
			{
				StartSpectating();
			}
			else
			{
				StartSpectatingDelayed(GameManager.MatchSettings.MatchResolvedSpectateStartDelay, canRestartDelay: false);
			}
		}
		else if (previousResolution == PlayerMatchResolution.JoinedAsSpectator && previousResolution == PlayerMatchResolution.None)
		{
			StopSpectating();
		}
	}

	private void OnLocalPlayerRemotePlayerRegistered(PlayerInfo registeredPlayer)
	{
		if (isSpectating && TargetPlayer == null)
		{
			CycleNextTarget(canBeginNewSpectate: false);
		}
	}

	private void OnLocalPlayerRemotePlayerDeregistered(PlayerInfo DeregisteredPlayer)
	{
		if (isSpectating && !(DeregisteredPlayer != TargetPlayer))
		{
			CycleNextTarget(canBeginNewSpectate: false);
		}
	}

	private void OnLocalPlayerMatchStateChanged(MatchState previousState, MatchState currentState)
	{
		switch (currentState)
		{
		case MatchState.Overtime:
			StartSpectating();
			break;
		case MatchState.Ended:
			if (isSpectating && spectateStartDelayRoutine != null)
			{
				StopCoroutine(spectateStartDelayRoutine);
			}
			if (automaticCycleDelayRoutine != null)
			{
				StopCoroutine(automaticCycleDelayRoutine);
			}
			break;
		}
	}

	private void OnLocalPlayerOvertimeActiveBallsChanged(SyncList<GolfBall>.Operation operation, int ballIndex, GolfBall changedBall)
	{
		if (!isSpectating)
		{
			if (CourseManager.MatchState == MatchState.Overtime)
			{
				StartSpectating();
			}
		}
		else if (TargetBall != null && !CourseManager.OvertimeActiveBalls.Contains(TargetBall))
		{
			CycleNextTarget(canBeginNewSpectate: false);
		}
	}

	private void OnTargetPlayerTeleported()
	{
		GameplayCameraManager.ReachOrbitCameraSteadyState();
		if (CourseManager.MatchState <= MatchState.TeeOff && CameraModuleController.TryGetOrbitModule(out var orbitModule))
		{
			orbitModule.SetYaw(Target.transform.forward.GetYawDeg());
		}
	}

	private void OnTargetPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		if (!previousResolution.IsResolved() && currentResolution.IsResolved())
		{
			CycleNextTargetDelayed(GameManager.MatchSettings.SpectatedPlayerMatchResolvedAutoCycleDelay, canBeginNewSpectate: true);
		}
	}

	private void OnTargetPlayerItemSlotsSet(int index)
	{
		Hotkeys.UpdatePlayerInventoryIcon(index);
	}

	private void OnTargetPlayerIsInGolfCartChanged()
	{
		if (base.isLocalPlayer)
		{
			PlayerSpectator.LocalPlayerSpectatingTargetIsInGolfCartChanged?.Invoke();
		}
	}

	private void OnTargetPlayerEquippedItemIndexChanged()
	{
		int index = ((TargetPlayer.NetworkedEquippedItemIndex >= 0) ? Hotkeys.InventoryIndexToHotkeyIndex(TargetPlayer.NetworkedEquippedItemIndex) : 0);
		Hotkeys.Select(index, uiOnly: true);
	}

	private void OnTargetPlayerDestroyed()
	{
		CycleNextTarget(canBeginNewSpectate: false);
	}

	private void OnTargetBallWillBeDestroyed()
	{
		CycleNextTarget(canBeginNewSpectate: false);
	}

	private void OnIsSpectatingChanged(bool wasSpectating, bool isSpectating)
	{
		HoleProgressBarUi.UpdateStrokes();
		if (base.isLocalPlayer)
		{
			PlayerInfo.CancelEmote(canHideEmoteMenu: true);
			PlayerInfo.Inventory.CancelItemFlourish();
		}
		PlayerInfo.AsGolfer.InformIsSpectatingChanged();
		this.IsSpectatingChanged?.Invoke();
		PlayerSpectator.AnyPlayerIsSpectatingChanged?.Invoke(this);
		if (base.isLocalPlayer)
		{
			PlayerSpectator.LocalPlayerIsSpectatingChanged?.Invoke();
		}
		if (base.isServer && isSpectating && !BNetworkManager.IsChangingSceneOrShuttingDown && this != null && spectatorCamera == null)
		{
			spectatorCamera = GameManager.ServerInstantiateSpectatorCamera(base.gameObject);
		}
	}

	public PlayerSpectator()
	{
		_Mirror_SyncVarHookDelegate_isSpectating = OnIsSpectatingChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	protected void UserCode_CmdPlayEmoteForAllClients__SpectatorEmote__NetworkConnectionToClient(SpectatorEmote emote, NetworkConnectionToClient sender)
	{
		if (sender != null && sender != NetworkServer.localConnection)
		{
			PlayEmoteInternal(emote);
		}
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != NetworkServer.localConnection && value != sender)
			{
				RpcPlayEmote(value, emote);
			}
		}
	}

	protected static void InvokeUserCode_CmdPlayEmoteForAllClients__SpectatorEmote__NetworkConnectionToClient(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdPlayEmoteForAllClients called on client.");
		}
		else
		{
			((PlayerSpectator)obj).UserCode_CmdPlayEmoteForAllClients__SpectatorEmote__NetworkConnectionToClient(GeneratedNetworkCode._Read_SpectatorEmote(reader), senderConnection);
		}
	}

	protected void UserCode_RpcPlayEmote__NetworkConnectionToClient__SpectatorEmote(NetworkConnectionToClient connection, SpectatorEmote emote)
	{
		PlayEmoteInternal(emote);
	}

	protected static void InvokeUserCode_RpcPlayEmote__NetworkConnectionToClient__SpectatorEmote(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC RpcPlayEmote called on server.");
		}
		else
		{
			((PlayerSpectator)obj).UserCode_RpcPlayEmote__NetworkConnectionToClient__SpectatorEmote(null, GeneratedNetworkCode._Read_SpectatorEmote(reader));
		}
	}

	static PlayerSpectator()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(PlayerSpectator), "System.Void PlayerSpectator::CmdPlayEmoteForAllClients(SpectatorEmote,Mirror.NetworkConnectionToClient)", InvokeUserCode_CmdPlayEmoteForAllClients__SpectatorEmote__NetworkConnectionToClient, requiresAuthority: true);
		RemoteProcedureCalls.RegisterRpc(typeof(PlayerSpectator), "System.Void PlayerSpectator::RpcPlayEmote(Mirror.NetworkConnectionToClient,SpectatorEmote)", InvokeUserCode_RpcPlayEmote__NetworkConnectionToClient__SpectatorEmote);
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(isSpectating);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isSpectating);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isSpectating, _Mirror_SyncVarHookDelegate_isSpectating, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isSpectating, _Mirror_SyncVarHookDelegate_isSpectating, reader.ReadBool());
		}
	}
}
