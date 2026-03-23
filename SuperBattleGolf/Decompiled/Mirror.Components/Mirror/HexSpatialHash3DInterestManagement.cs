using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Spatial Hash/Hex Spatial Hash (3D)")]
public class HexSpatialHash3DInterestManagement : InterestManagement
{
	[Range(1f, 60f)]
	[Tooltip("Time interval in seconds between observer rebuilds")]
	public byte rebuildInterval = 1;

	[Range(1f, 60f)]
	[Tooltip("Time interval in seconds between static object rebuilds")]
	public byte staticRebuildInterval = 10;

	[Range(10f, 5000f)]
	[Tooltip("Radius of super hex.\nSet to 10% larger than camera far clip plane.")]
	public ushort visRange = 1100;

	[Range(10f, 5000f)]
	[Tooltip("Cell3D height effects all 3 layers")]
	public ushort cellHeight = 500;

	[Range(1f, 100f)]
	[Tooltip("Distance an object must move for updating cell positions")]
	public ushort minMoveDistance = 1;

	private double lastRebuildTime;

	private byte rebuildCounter;

	private HexGrid3D grid;

	private readonly List<HashSet<NetworkIdentity>> cells = new List<HashSet<NetworkIdentity>>();

	private readonly Dictionary<NetworkIdentity, (Cell3D cell, Vector3 worldPos)> lastIdentityPositions = new Dictionary<NetworkIdentity, (Cell3D, Vector3)>();

	private readonly Dictionary<NetworkConnectionToClient, (Cell3D cell, Vector3 worldPos)> lastConnectionPositions = new Dictionary<NetworkConnectionToClient, (Cell3D, Vector3)>();

	private readonly Cell3D[] neighborCells = new Cell3D[21];

	private readonly Dictionary<NetworkConnectionToClient, HashSet<NetworkIdentity>> connectionObservers = new Dictionary<NetworkConnectionToClient, HashSet<NetworkIdentity>>();

	private readonly List<NetworkIdentity> identityKeys = new List<NetworkIdentity>();

	private readonly Stack<HashSet<NetworkIdentity>> cellPool = new Stack<HashSet<NetworkIdentity>>();

	private readonly HashSet<NetworkIdentity> staticObjects = new HashSet<NetworkIdentity>();

	private const int MAX_Q = 19;

	private const int MAX_R = 23;

	private const int LAYER_OFFSET = 18;

	private const int MAX_LAYERS = 36;

	private const ushort MAX_AREA = 9000;

	private void Awake()
	{
		grid = new HexGrid3D(visRange, cellHeight);
		int num = 15732;
		for (int i = 0; i < num; i++)
		{
			cells.Add(null);
		}
	}

	private void LateUpdate()
	{
		if (!(NetworkTime.time - lastRebuildTime >= (double)(int)rebuildInterval))
		{
			return;
		}
		foreach (NetworkConnectionToClient value3 in NetworkServer.connections.Values)
		{
			if (value3?.identity != null)
			{
				Vector3 position = value3.identity.transform.position;
				if (!lastConnectionPositions.TryGetValue(value3, out (Cell3D, Vector3) value) || Vector3.Distance(position, value.Item2) >= (float)(int)minMoveDistance)
				{
					Cell3D item = grid.WorldToCell(position);
					lastConnectionPositions[value3] = (item, position);
				}
			}
		}
		identityKeys.Clear();
		identityKeys.AddRange(lastIdentityPositions.Keys);
		bool flag = rebuildCounter >= staticRebuildInterval;
		foreach (NetworkIdentity identityKey in identityKeys)
		{
			if (flag || !staticObjects.Contains(identityKey))
			{
				UpdateIdentityPosition(identityKey);
			}
		}
		if (flag)
		{
			rebuildCounter = 0;
		}
		else
		{
			rebuildCounter++;
		}
		connectionObservers.Clear();
		foreach (NetworkConnectionToClient value4 in NetworkServer.connections.Values)
		{
			if (value4?.identity == null || !lastConnectionPositions.TryGetValue(value4, out (Cell3D, Vector3) value2))
			{
				continue;
			}
			grid.GetNeighborCells(value2.Item1, neighborCells);
			HashSet<NetworkIdentity> hashSet = new HashSet<NetworkIdentity>();
			connectionObservers[value4] = hashSet;
			for (int i = 0; i < neighborCells.Length; i++)
			{
				int cellIndex = GetCellIndex(neighborCells[i]);
				if (cellIndex < 0 || cellIndex >= cells.Count || cells[cellIndex] == null)
				{
					continue;
				}
				foreach (NetworkIdentity item2 in cells[cellIndex])
				{
					hashSet.Add(item2);
				}
			}
		}
		RebuildAll();
		lastRebuildTime = NetworkTime.time;
	}

	public override void OnSpawned(NetworkIdentity identity)
	{
		UpdateIdentityPosition(identity);
		if (identity.gameObject.GetComponentsInChildren<Renderer>().Any((Renderer r) => r.isPartOfStaticBatch))
		{
			staticObjects.Add(identity);
		}
	}

	private void UpdateIdentityPosition(NetworkIdentity identity)
	{
		Vector3 position = identity.transform.position;
		Cell3D cell3D = grid.WorldToCell(position);
		if (Mathf.Abs(position.x) > 9000f || Mathf.Abs(position.y) > 9000f || Mathf.Abs(position.z) > 9000f)
		{
			return;
		}
		if (lastIdentityPositions.TryGetValue(identity, out (Cell3D, Vector3) value))
		{
			if (!(Vector3.Distance(position, value.Item2) >= (float)(int)minMoveDistance) && cell3D.Equals(value.Item1))
			{
				return;
			}
			if (!cell3D.Equals(value.Item1))
			{
				int cellIndex = GetCellIndex(value.Item1);
				if (cellIndex >= 0 && cellIndex < cells.Count && cells[cellIndex] != null)
				{
					cells[cellIndex].Remove(identity);
				}
				AddToCell(cell3D, identity);
			}
			lastIdentityPositions[identity] = (cell3D, position);
		}
		else
		{
			AddToCell(cell3D, identity);
			lastIdentityPositions[identity] = (cell3D, position);
		}
	}

	private void AddToCell(Cell3D cell, NetworkIdentity identity)
	{
		int cellIndex = GetCellIndex(cell);
		if (cellIndex >= 0 && cellIndex < cells.Count)
		{
			if (cells[cellIndex] == null)
			{
				cells[cellIndex] = ((cellPool.Count > 0) ? cellPool.Pop() : new HashSet<NetworkIdentity>());
			}
			cells[cellIndex].Add(identity);
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (!lastIdentityPositions.TryGetValue(identity, out (Cell3D, Vector3) value) || !lastConnectionPositions.TryGetValue(newObserver, out (Cell3D, Vector3) value2))
		{
			return false;
		}
		grid.GetNeighborCells(value2.Item1, neighborCells);
		for (int i = 0; i < neighborCells.Length; i++)
		{
			if (neighborCells[i].Equals(value.Item1))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!lastIdentityPositions.TryGetValue(identity, out (Cell3D, Vector3) _))
		{
			return;
		}
		foreach (NetworkConnectionToClient value3 in NetworkServer.connections.Values)
		{
			if (!(value3?.identity == null) && connectionObservers.TryGetValue(value3, out var value2) && value2.Contains(identity))
			{
				newObservers.Add(value3);
			}
		}
	}

	public override void ResetState()
	{
		lastRebuildTime = 0.0;
		for (int i = 0; i < cells.Count; i++)
		{
			if (cells[i] != null)
			{
				cells[i].Clear();
				cellPool.Push(cells[i]);
				cells[i] = null;
			}
		}
		lastIdentityPositions.Clear();
		lastConnectionPositions.Clear();
		connectionObservers.Clear();
		identityKeys.Clear();
		staticObjects.Clear();
		rebuildCounter = 0;
	}

	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (!lastIdentityPositions.TryGetValue(identity, out (Cell3D, Vector3) value))
		{
			return;
		}
		int cellIndex = GetCellIndex(value.Item1);
		if (cellIndex >= 0 && cellIndex < cells.Count && cells[cellIndex] != null)
		{
			cells[cellIndex].Remove(identity);
			if (cells[cellIndex].Count == 0)
			{
				cellPool.Push(cells[cellIndex]);
				cells[cellIndex] = null;
			}
		}
		lastIdentityPositions.Remove(identity);
		staticObjects.Remove(identity);
	}

	private int GetCellIndex(Cell3D cell)
	{
		int num = cell.q + 9;
		int num2 = cell.r + 11;
		int num3 = cell.layer + 18;
		return num + num2 * 19 + num3 * 19 * 23;
	}
}
