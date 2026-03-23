using Mirror;
using UnityEngine;

public class TeeingSpot
{
	public GolfTeeingPlatform teeingPlatform;

	public GolfTee teePrefab;

	public GolfTee tee;

	public Vector3 teeWorldPosition;

	public Quaternion teeWorldRotation;

	public Vector3 playerWorldPosition;

	public Quaternion playerWorldRotation;

	public TeeingSpotStatus Status { get; private set; }

	public int OwningPlayerConnectionId { get; private set; }

	public void ServerEnsureSpawned()
	{
		if (!(tee != null))
		{
			tee = Object.Instantiate(teeingPlatform.Settings.TeePrefab, teeWorldPosition, teeWorldRotation);
			NetworkServer.Spawn(tee.gameObject);
		}
	}

	public void ReserveFor(int connectionId)
	{
		if (Status != TeeingSpotStatus.Available)
		{
			Debug.LogError("Attempted to reserve a teeing spot that isn't available");
			return;
		}
		Status = TeeingSpotStatus.Reserved;
		OwningPlayerConnectionId = connectionId;
	}

	public void ClaimFor(int connectionId)
	{
		if (Status == TeeingSpotStatus.Claimed)
		{
			Debug.LogError($"Attempted to reserve a teeing spot that is already claimed by connection {OwningPlayerConnectionId}");
			return;
		}
		if (Status == TeeingSpotStatus.Reserved && connectionId != OwningPlayerConnectionId)
		{
			Debug.LogError($"Attempted to reserve a teeing spot that is already reserved for connection {OwningPlayerConnectionId}");
			return;
		}
		Status = TeeingSpotStatus.Claimed;
		OwningPlayerConnectionId = connectionId;
	}

	public void ReleaseBy(int connectionId)
	{
		if (Status != TeeingSpotStatus.Claimed)
		{
			Debug.LogError($"Connection {connectionId} attempted to release a teeing spot isn't claimed");
			return;
		}
		if (connectionId != OwningPlayerConnectionId)
		{
			Debug.LogError($"Connection {connectionId} attempted to release a teeing spot is claimed by {OwningPlayerConnectionId}");
			return;
		}
		Status = TeeingSpotStatus.Available;
		OwningPlayerConnectionId = 0;
	}
}
