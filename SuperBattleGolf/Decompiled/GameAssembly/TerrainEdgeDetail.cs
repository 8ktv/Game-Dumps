using System.Collections.Generic;
using UnityEngine;

public class TerrainEdgeDetail : MonoBehaviour
{
	[SerializeField]
	private TerrainLayer terrainLayer;

	private readonly List<Collider> colliders = new List<Collider>();

	public TerrainLayer TerrainLayer => terrainLayer;

	private void Awake()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider collider in componentsInChildren)
		{
			if (collider.GetComponentInParent<TerrainEdgeDetail>() == this)
			{
				colliders.Add(collider);
			}
		}
		PhysicsManager.RegisterTerrainEdgeDetail(colliders, terrainLayer);
	}

	private void OnDestroy()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PhysicsManager.DeregisterTerrainEdgeDetail(colliders);
		}
	}
}
