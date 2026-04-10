using System.Collections.Generic;
using UnityEngine;

public class TerrainAddition : MonoBehaviour
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
			if (collider.GetComponentInParent<TerrainAddition>() == this)
			{
				colliders.Add(collider);
				collider.hasModifiableContacts = true;
			}
		}
		PhysicsManager.RegisterTerrainAddition(colliders, terrainLayer);
	}

	private void OnDestroy()
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			PhysicsManager.DeregisterTerrainAddition(colliders);
		}
	}
}
