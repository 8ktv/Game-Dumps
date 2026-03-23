using System;
using Mirror;
using UnityEngine;

public class CheckpointManager : SingletonNetworkBehaviour<CheckpointManager>
{
	private readonly SyncDictionary<ulong, Checkpoint> activeCheckpointPerPlayerGuid = new SyncDictionary<ulong, Checkpoint>();

	private Checkpoint visiblyActiveCheckpoint;

	public override void OnStartClient()
	{
		UpdateVisiblyActiveCheckpoint(suppressEffects: false);
		SyncDictionary<ulong, Checkpoint> syncDictionary = activeCheckpointPerPlayerGuid;
		syncDictionary.OnChange = (Action<SyncIDictionary<ulong, Checkpoint>.Operation, ulong, Checkpoint>)Delegate.Combine(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, Checkpoint>.Operation, ulong, Checkpoint>(OnClientActiveCheckpointPerPlayerChanged));
		PlayerSpectator.LocalPlayerIsSpectatingChanged += OnClientLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget += OnClientLocalPlayerSetSpectatingTarget;
	}

	public override void OnStopClient()
	{
		SyncDictionary<ulong, Checkpoint> syncDictionary = activeCheckpointPerPlayerGuid;
		syncDictionary.OnChange = (Action<SyncIDictionary<ulong, Checkpoint>.Operation, ulong, Checkpoint>)Delegate.Remove(syncDictionary.OnChange, new Action<SyncIDictionary<ulong, Checkpoint>.Operation, ulong, Checkpoint>(OnClientActiveCheckpointPerPlayerChanged));
		PlayerSpectator.LocalPlayerIsSpectatingChanged -= OnClientLocalPlayerIsSpectatingChanged;
		PlayerSpectator.LocalPlayerSetSpectatingTarget -= OnClientLocalPlayerSetSpectatingTarget;
	}

	public static void TryActivate(Checkpoint checkpoint, PlayerInfo player)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.TryActivateInternal(checkpoint, player);
		}
	}

	public static bool TryGetLocalPlayerActiveCheckpoint(out Checkpoint checkpoint)
	{
		if (!SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			checkpoint = null;
			return false;
		}
		return SingletonNetworkBehaviour<CheckpointManager>.Instance.TryGetLocalPlayerActiveCheckpointInternal(out checkpoint);
	}

	public static void ResetCheckpoint(PlayerInfo player)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.ResetCheckpointInternal(player);
		}
	}

	private void ResetCheckpointInternal(PlayerInfo player)
	{
		activeCheckpointPerPlayerGuid.Remove(player.PlayerId.Guid);
	}

	private void TryActivateInternal(Checkpoint checkpoint, PlayerInfo player)
	{
		Checkpoint value;
		if (player == null)
		{
			Debug.LogError("Attempted to activate a checkpoint for a null player", base.gameObject);
		}
		else if (player.PlayerId.Guid == 0L)
		{
			Debug.LogError("Attempted to activate a checkpoint for a player with an invalid GUID", player);
		}
		else if (!activeCheckpointPerPlayerGuid.TryGetValue(player.PlayerId.Guid, out value) || CanOvertakeCheckpoint(value))
		{
			activeCheckpointPerPlayerGuid[player.PlayerId.Guid] = checkpoint;
		}
		bool CanOvertakeCheckpoint(Checkpoint checkpointToOvertake)
		{
			if (checkpointToOvertake == null)
			{
				return true;
			}
			if (GolfHoleManager.MainHole == null)
			{
				return true;
			}
			return checkpoint.Order >= checkpointToOvertake.Order;
		}
	}

	private bool TryGetLocalPlayerActiveCheckpointInternal(out Checkpoint checkpoint)
	{
		if (GameManager.LocalPlayerId == null)
		{
			Debug.LogError("Attempted to get an active checkpoint for the local player, but they aren't registered", base.gameObject);
			checkpoint = null;
			return false;
		}
		if (GameManager.LocalPlayerId.Guid == 0L)
		{
			Debug.LogError("Attempted to get an active checkpoint for the local player, but their GUID is invalid", GameManager.LocalPlayerInfo);
			checkpoint = null;
			return false;
		}
		return activeCheckpointPerPlayerGuid.TryGetValue(GameManager.LocalPlayerId.Guid, out checkpoint);
	}

	private void UpdateVisiblyActiveCheckpoint(bool suppressEffects)
	{
		PlayerInfo viewedOrLocalPlayer = GameManager.GetViewedOrLocalPlayer();
		Checkpoint value;
		if (viewedOrLocalPlayer == null)
		{
			SetActiveCheckpoint(null);
		}
		else if (activeCheckpointPerPlayerGuid.TryGetValue(viewedOrLocalPlayer.PlayerId.Guid, out value))
		{
			SetActiveCheckpoint(value);
		}
		else
		{
			SetActiveCheckpoint(null);
		}
		void SetActiveCheckpoint(Checkpoint checkpoint)
		{
			Checkpoint checkpoint2 = visiblyActiveCheckpoint;
			visiblyActiveCheckpoint = checkpoint;
			if (!(visiblyActiveCheckpoint == checkpoint2))
			{
				if (checkpoint2 != null)
				{
					checkpoint2.SetIsVisuallyActive(isActive: false, suppressEffects);
				}
				if (visiblyActiveCheckpoint != null)
				{
					visiblyActiveCheckpoint.SetIsVisuallyActive(isActive: true, suppressEffects);
				}
			}
		}
	}

	private void OnClientActiveCheckpointPerPlayerChanged(SyncIDictionary<ulong, Checkpoint>.Operation operation, ulong playerGuid, Checkpoint checkpoint)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown && (playerGuid == 0L || GameManager.GetViewedOrLocalPlayer().PlayerId.Guid == playerGuid))
		{
			UpdateVisiblyActiveCheckpoint(suppressEffects: false);
		}
	}

	private void OnClientLocalPlayerIsSpectatingChanged()
	{
		UpdateVisiblyActiveCheckpoint(suppressEffects: true);
	}

	private void OnClientLocalPlayerSetSpectatingTarget(bool isInitialTarget)
	{
		UpdateVisiblyActiveCheckpoint(suppressEffects: true);
	}

	public CheckpointManager()
	{
		InitSyncObject(activeCheckpointPerPlayerGuid);
	}

	public override bool Weaved()
	{
		return true;
	}
}
