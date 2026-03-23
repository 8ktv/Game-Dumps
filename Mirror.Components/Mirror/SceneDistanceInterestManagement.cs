using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Scene/Scene Distance Interest Management")]
public class SceneDistanceInterestManagement : InterestManagement
{
	[Tooltip("The maximum range that objects will be visible at. Add DistanceInterestManagementCustomRange onto NetworkIdentities for custom ranges.")]
	public int visRange = 500;

	[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
	public float rebuildInterval = 1f;

	private double lastRebuildTime;

	private readonly Dictionary<NetworkIdentity, DistanceInterestManagementCustomRange> CustomRanges = new Dictionary<NetworkIdentity, DistanceInterestManagementCustomRange>();

	private readonly Dictionary<Scene, HashSet<NetworkIdentity>> sceneObjects = new Dictionary<Scene, HashSet<NetworkIdentity>>();

	private readonly Dictionary<NetworkIdentity, Scene> lastObjectScene = new Dictionary<NetworkIdentity, Scene>();

	private HashSet<Scene> dirtyScenes = new HashSet<Scene>();

	[ServerCallback]
	private int GetVisRange(NetworkIdentity identity)
	{
		if (!NetworkServer.active)
		{
			return default(int);
		}
		if (!CustomRanges.TryGetValue(identity, out var value))
		{
			return visRange;
		}
		return value.visRange;
	}

	[ServerCallback]
	public override void ResetState()
	{
		if (NetworkServer.active)
		{
			lastRebuildTime = 0.0;
			CustomRanges.Clear();
		}
	}

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
			if (identity.TryGetComponent<DistanceInterestManagementCustomRange>(out var component))
			{
				CustomRanges[identity] = component;
			}
			Scene scene = identity.gameObject.scene;
			lastObjectScene[identity] = scene;
			if (!sceneObjects.TryGetValue(scene, out var value))
			{
				value = new HashSet<NetworkIdentity>();
				sceneObjects.Add(scene, value);
			}
			value.Add(identity);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		CustomRanges.Remove(identity);
		if (lastObjectScene.TryGetValue(identity, out var value))
		{
			lastObjectScene.Remove(identity);
			if (sceneObjects.TryGetValue(value, out var value2) && value2.Remove(identity))
			{
				dirtyScenes.Add(value);
			}
		}
	}

	[ServerCallback]
	private void LateUpdate()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkIdentity value2 in NetworkServer.spawned.Values)
		{
			if (!lastObjectScene.TryGetValue(value2, out var value))
			{
				continue;
			}
			Scene scene = value2.gameObject.scene;
			if (scene == value)
			{
				if (NetworkTime.localTime >= lastRebuildTime + (double)rebuildInterval)
				{
					RebuildAll();
					lastRebuildTime = NetworkTime.localTime;
				}
				continue;
			}
			dirtyScenes.Add(value);
			dirtyScenes.Add(scene);
			sceneObjects[value].Remove(value2);
			lastObjectScene[value2] = scene;
			if (!sceneObjects.ContainsKey(scene))
			{
				sceneObjects.Add(scene, new HashSet<NetworkIdentity>());
			}
			sceneObjects[scene].Add(value2);
		}
		foreach (Scene dirtyScene in dirtyScenes)
		{
			RebuildSceneObservers(dirtyScene);
		}
		dirtyScenes.Clear();
	}

	private void RebuildSceneObservers(Scene scene)
	{
		foreach (NetworkIdentity item in sceneObjects[scene])
		{
			if (item != null)
			{
				NetworkServer.RebuildObservers(item, initialize: false);
			}
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (identity.gameObject.scene != newObserver.identity.gameObject.scene)
		{
			return false;
		}
		int num = GetVisRange(identity);
		return Vector3.Distance(identity.transform.position, newObserver.identity.transform.position) < (float)num;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!sceneObjects.TryGetValue(identity.gameObject.scene, out var value))
		{
			return;
		}
		int num = GetVisRange(identity);
		Vector3 position = identity.transform.position;
		foreach (NetworkIdentity item in value)
		{
			if (item != null && item.connectionToClient != null)
			{
				NetworkConnectionToClient connectionToClient = item.connectionToClient;
				if (connectionToClient != null && connectionToClient.isAuthenticated && connectionToClient.identity != null && Vector3.Distance(connectionToClient.identity.transform.position, position) < (float)num)
				{
					newObservers.Add(connectionToClient);
				}
			}
		}
	}
}
