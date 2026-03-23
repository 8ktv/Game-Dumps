using System;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;

public class GolfTeeingPlatform : NetworkBehaviour
{
	[SerializeField]
	private TeeingPlatformSettings settings;

	[SerializeField]
	private int minPlayerCount;

	public TeeingSpot[] teeingSpots;

	[SyncVar(hook = "OnIsActiveChanged")]
	private bool isActive;

	public Action<bool, bool> _Mirror_SyncVarHookDelegate_isActive;

	public TeeingPlatformSettings Settings => settings;

	public int MinPlayerCount => minPlayerCount;

	public bool IsActive => isActive;

	public bool NetworkisActive
	{
		get
		{
			return isActive;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref isActive, 1uL, _Mirror_SyncVarHookDelegate_isActive);
		}
	}

	private void Awake()
	{
		GolfTeeManager.RegisterTeeingPlatform(this);
		if (NetworkServer.active)
		{
			CreateTeeingSpots();
		}
		void CreateTeeingSpots()
		{
			teeingSpots = new TeeingSpot[settings.MaxTeeCount];
			for (int i = 0; i < settings.MaxTeeCount; i++)
			{
				Vector3 teeWorldPosition = GetTeeWorldPosition(i);
				Matrix4x4 effectiveLocalToWorld = Matrix4x4.TRS(teeWorldPosition, base.transform.rotation, Vector3.one);
				teeingSpots[i] = new TeeingSpot
				{
					teeingPlatform = this,
					teePrefab = settings.TeePrefab,
					teeWorldPosition = teeWorldPosition,
					teeWorldRotation = base.transform.rotation,
					playerWorldPosition = settings.TeePrefab.GetPlayerWorldSpawnPosition(effectiveLocalToWorld),
					playerWorldRotation = base.transform.rotation
				};
			}
		}
	}

	public override void OnStartServer()
	{
		ApplyIsActive();
	}

	public override void OnStartClient()
	{
		ApplyIsActive();
	}

	[Server]
	public void ServerActivate()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfTeeingPlatform::ServerActivate()' called when server was not active");
		}
		else
		{
			NetworkisActive = true;
		}
	}

	public bool TryGetAvailableTeeingSpot(out TeeingSpot teeingSpot)
	{
		for (int i = 0; i < teeingSpots.Length; i++)
		{
			teeingSpot = teeingSpots[i];
			if (teeingSpot.Status == TeeingSpotStatus.Available)
			{
				return true;
			}
		}
		teeingSpot = null;
		return false;
	}

	public int GetAvailableTeeingSpotCount()
	{
		int num = 0;
		TeeingSpot[] array = teeingSpots;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Status == TeeingSpotStatus.Available)
			{
				num++;
			}
		}
		return num;
	}

	private Vector3 GetTeeWorldPosition(int teeIndex)
	{
		float num = settings.DistanceBetweenTees * (float)teeIndex - settings.FirstTeeOffset;
		return base.transform.position + num * base.transform.right + settings.TeeVerticalOffset * base.transform.up;
	}

	private void ApplyIsActive()
	{
		base.gameObject.SetActive(isActive);
		if (isActive)
		{
			GolfTeeManager.RegisterActiveTeeingPlatform(this);
		}
	}

	private void OnIsActiveChanged(bool wasActive, bool isActive)
	{
		ApplyIsActive();
	}

	private void OnDrawGizmosSelected()
	{
		if (!(settings == null))
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position - settings.FirstTeeOffset * base.transform.right, base.transform.position + settings.FirstTeeOffset * base.transform.right);
			Gizmos.color = Color.yellow;
			for (int i = 0; i < settings.MaxTeeCount; i++)
			{
				Gizmos.DrawSphere(GetTeeWorldPosition(i), 0.2f);
			}
		}
	}

	public GolfTeeingPlatform()
	{
		_Mirror_SyncVarHookDelegate_isActive = OnIsActiveChanged;
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
			writer.WriteBool(isActive);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteBool(isActive);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref isActive, _Mirror_SyncVarHookDelegate_isActive, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref isActive, _Mirror_SyncVarHookDelegate_isActive, reader.ReadBool());
		}
	}
}
