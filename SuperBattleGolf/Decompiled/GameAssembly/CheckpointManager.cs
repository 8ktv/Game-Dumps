using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CheckpointManager : SingletonNetworkBehaviour<CheckpointManager>
{
	private readonly SyncDictionary<ulong, Checkpoint> activeCheckpointPerPlayerGuid = new SyncDictionary<ulong, Checkpoint>();

	private readonly HashSet<Checkpoint> allCheckpoints = new HashSet<Checkpoint>();

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

	public static void RegisterCheckpoint(Checkpoint checkpoint)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.RegisterCheckpointInternal(checkpoint);
		}
	}

	public static void DeregisterCheckpoint(Checkpoint checkpoint)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.DeregisterCheckpointInternal(checkpoint);
		}
	}

	public static void TryActivate(Checkpoint checkpoint, PlayerInfo player)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.TryActivateInternal(checkpoint, player, forced: false);
		}
	}

	public static void DeactivateCheckpoint(PlayerInfo player)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.DeactivateCheckpointInternal(player);
		}
	}

	public static void ResetCheckpointForPlayerRestart(PlayerInfo player, Vector3 restartPosition)
	{
		if (SingletonNetworkBehaviour<CheckpointManager>.HasInstance)
		{
			SingletonNetworkBehaviour<CheckpointManager>.Instance.ResetCheckpointForPlayerRestartInternal(player, restartPosition);
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

	private void RegisterCheckpointInternal(Checkpoint checkpoint)
	{
		allCheckpoints.Add(checkpoint);
	}

	private void DeregisterCheckpointInternal(Checkpoint checkpoint)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			allCheckpoints.Remove(checkpoint);
		}
	}

	private void TryActivateInternal(Checkpoint checkpoint, PlayerInfo player, bool forced)
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
		else if (!activeCheckpointPerPlayerGuid.TryGetValue(player.PlayerId.Guid, out value) || forced || CanOvertakeCheckpoint(value))
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

	private void DeactivateCheckpointInternal(PlayerInfo player)
	{
		activeCheckpointPerPlayerGuid.Remove(player.PlayerId.Guid);
	}

	private void ResetCheckpointForPlayerRestartInternal(PlayerInfo player, Vector3 restartPosition)
	{
		if (player == null)
		{
			Debug.LogError("Attempted to reset a null player's checkpoint", base.gameObject);
		}
		else if (player.PlayerId.Guid == 0L)
		{
			Debug.LogError("Attempted to reset a player's checkpoint, but they have an invalid GUID", player);
		}
		else
		{
			if (!activeCheckpointPerPlayerGuid.TryGetValue(player.PlayerId.Guid, out var value))
			{
				return;
			}
			if (GolfHoleManager.MainHole == null)
			{
				Debug.LogError("Attempted to reset a player's checkpoint, but there is no main golf hole to reference", player);
				return;
			}
			Vector3 position = GolfHoleManager.MainHole.transform.position;
			float sqrMagnitude = (restartPosition - position).sqrMagnitude;
			float sqrMagnitude2 = (value.transform.position - position).sqrMagnitude;
			Checkpoint checkpoint = null;
			float num = float.MaxValue;
			foreach (Checkpoint allCheckpoint in allCheckpoints)
			{
				if (allCheckpoint == value)
				{
					continue;
				}
				float sqrMagnitude3 = (allCheckpoint.transform.position - position).sqrMagnitude;
				if (!(sqrMagnitude3 < sqrMagnitude) && !(sqrMagnitude3 < sqrMagnitude2))
				{
					float sqrMagnitude4 = (allCheckpoint.transform.position - restartPosition).sqrMagnitude;
					if (!(sqrMagnitude4 > num))
					{
						checkpoint = allCheckpoint;
						num = sqrMagnitude4;
					}
				}
			}
			if (checkpoint == null)
			{
				DeactivateCheckpointInternal(player);
			}
			else
			{
				TryActivateInternal(checkpoint, player, forced: true);
			}
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
