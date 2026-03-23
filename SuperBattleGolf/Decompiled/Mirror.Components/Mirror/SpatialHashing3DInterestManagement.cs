using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Spatial Hash/Grid Spatial Hash (3D)")]
public class SpatialHashing3DInterestManagement : InterestManagement
{
	[Tooltip("The maximum range that objects will be visible at.")]
	public int visRange = 30;

	[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
	public float rebuildInterval = 1f;

	private double lastRebuildTime;

	[Header("Debug Settings")]
	public bool showSlider;

	private Grid3D<NetworkConnectionToClient> grid = new Grid3D<NetworkConnectionToClient>(1024);

	public int resolution => visRange / 2;

	private Vector3Int ProjectToGrid(Vector3 position)
	{
		return Vector3Int.RoundToInt(position / resolution);
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		Vector3Int vector3Int = ProjectToGrid(identity.transform.position);
		Vector3Int vector3Int2 = ProjectToGrid(newObserver.identity.transform.position);
		return (vector3Int - vector3Int2).sqrMagnitude <= 2;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		Vector3Int position = ProjectToGrid(identity.transform.position);
		grid.GetWithNeighbours(position, newObservers);
	}

	[ServerCallback]
	public override void ResetState()
	{
		if (NetworkServer.active)
		{
			lastRebuildTime = 0.0;
		}
	}

	[ServerCallback]
	internal void Update()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		grid.ClearNonAlloc();
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value.isAuthenticated && value.identity != null)
			{
				Vector3Int position = ProjectToGrid(value.identity.transform.position);
				grid.Add(position, value);
			}
		}
		if (NetworkTime.localTime >= lastRebuildTime + (double)rebuildInterval)
		{
			RebuildAll();
			lastRebuildTime = NetworkTime.localTime;
		}
	}
}
