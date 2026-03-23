using Mirror;
using UnityEngine;

public class OrbitalLaserManager : SingletonBehaviour<OrbitalLaserManager>
{
	[SerializeField]
	private OrbitalLaser laserPrefab;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public static void ServerActivateLaser(Hittable target, Vector3 fallbackWorldPosition, PlayerInventory owner, ItemUseId itemUseId)
	{
		if (SingletonBehaviour<OrbitalLaserManager>.HasInstance)
		{
			SingletonBehaviour<OrbitalLaserManager>.Instance.ServerActivateLaserInternal(target, fallbackWorldPosition, owner, itemUseId);
		}
	}

	public static Hittable GetTarget(out Vector3 fallbackPosition)
	{
		Vector3 holePosition = GolfHoleManager.MainHole.transform.position;
		PlayerInfo nearestPlayer = null;
		float nearestPlayerDistanceSquared = float.MaxValue;
		EvaluatePlayer(GameManager.LocalPlayerInfo);
		foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
		{
			EvaluatePlayer(remotePlayer);
		}
		if (nearestPlayer == null)
		{
			fallbackPosition = holePosition;
			return null;
		}
		fallbackPosition = nearestPlayer.transform.position;
		return nearestPlayer.AsHittable;
		void EvaluatePlayer(PlayerInfo player)
		{
			if (CanTarget(player.AsHittable))
			{
				float sqrMagnitude = (holePosition - player.transform.position).sqrMagnitude;
				if (!(sqrMagnitude >= nearestPlayerDistanceSquared))
				{
					nearestPlayer = player;
					nearestPlayerDistanceSquared = sqrMagnitude;
				}
			}
		}
	}

	public static bool CanTarget(Hittable potentialTarget)
	{
		if (potentialTarget == null)
		{
			return false;
		}
		if (!potentialTarget.AsEntity.IsPlayer)
		{
			return true;
		}
		PlayerInfo playerInfo = potentialTarget.AsEntity.PlayerInfo;
		if (playerInfo.AsGolfer.IsMatchResolved)
		{
			return false;
		}
		if (!playerInfo.Movement.IsVisible)
		{
			return false;
		}
		if (playerInfo.AsSpectator.IsSpectating)
		{
			return false;
		}
		return true;
	}

	[Server]
	private void ServerActivateLaserInternal(Hittable target, Vector3 fallbackWorldPosition, PlayerInventory owner, ItemUseId itemUseId)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void OrbitalLaserManager::ServerActivateLaserInternal(Hittable,UnityEngine.Vector3,PlayerInventory,ItemUseId)' called when server was not active");
			return;
		}
		OrbitalLaser orbitalLaser = Object.Instantiate(laserPrefab, base.transform);
		orbitalLaser.ServerActivate(target, fallbackWorldPosition, owner, itemUseId);
		NetworkServer.Spawn(orbitalLaser.gameObject);
	}
}
