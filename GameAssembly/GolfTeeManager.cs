using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GolfTeeManager : SingletonBehaviour<GolfTeeManager>
{
	[SerializeField]
	private HitGolfTee hitTeePrefab;

	[SerializeField]
	private int maxHitTeePoolSize;

	private readonly HashSet<GolfTeeingPlatform> allTeeingPlaforms = new HashSet<GolfTeeingPlatform>();

	private readonly HashSet<GolfTeeingPlatform> activeTeeingPlaforms = new HashSet<GolfTeeingPlatform>();

	private static Transform hitTeePoolParent;

	private static readonly Stack<HitGolfTee> hitTeePool = new Stack<HitGolfTee>();

	public static HashSet<GolfTeeingPlatform> ActiveTeeingPlaforms
	{
		get
		{
			if (!SingletonBehaviour<GolfTeeManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GolfTeeManager>.Instance.activeTeeingPlaforms;
		}
	}

	public static event Action<GolfTeeingPlatform> ActiveTeeingPlatformRegistered;

	public static void RegisterTeeingPlatform(GolfTeeingPlatform teeingPlatform)
	{
		if (SingletonBehaviour<GolfTeeManager>.HasInstance || SingletonBehaviour<GolfTeeManager>.Instance != null)
		{
			SingletonBehaviour<GolfTeeManager>.Instance.RegisterTeeingPlatformInternal(teeingPlatform);
		}
	}

	public static void RegisterActiveTeeingPlatform(GolfTeeingPlatform teeingPlatform)
	{
		if (SingletonBehaviour<GolfTeeManager>.HasInstance)
		{
			SingletonBehaviour<GolfTeeManager>.Instance.RegisterActiveTeeingPlatformInternal(teeingPlatform);
		}
	}

	public static void UpdateActivePlatforms()
	{
		if (SingletonBehaviour<GolfTeeManager>.HasInstance)
		{
			SingletonBehaviour<GolfTeeManager>.Instance.UpdateActivePlatformsInternal();
		}
	}

	public static TeeingSpot GetAvailableTeeingSpot()
	{
		if (!SingletonBehaviour<GolfTeeManager>.HasInstance)
		{
			return null;
		}
		return SingletonBehaviour<GolfTeeManager>.Instance.GetAvailableTeeingSpotInternal();
	}

	public static void SpawnHitTee(Vector3 worldPosition, Quaternion rotation, Vector3 hitDirection, float hitPower)
	{
		if (SingletonBehaviour<GolfTeeManager>.HasInstance)
		{
			SingletonBehaviour<GolfTeeManager>.Instance.SpawnHitTeeInternal(worldPosition, rotation, hitDirection, hitPower);
		}
	}

	public static void ReturnHitTee(HitGolfTee hitTee)
	{
		if (SingletonBehaviour<GolfTeeManager>.HasInstance)
		{
			SingletonBehaviour<GolfTeeManager>.Instance.ReturnHitTeeInternal(hitTee);
		}
	}

	private void RegisterTeeingPlatformInternal(GolfTeeingPlatform teeingPlatform)
	{
		allTeeingPlaforms.Add(teeingPlatform);
	}

	private void RegisterActiveTeeingPlatformInternal(GolfTeeingPlatform teeingPlatform)
	{
		activeTeeingPlaforms.Add(teeingPlatform);
		GolfTeeManager.ActiveTeeingPlatformRegistered?.Invoke(teeingPlatform);
	}

	[Server]
	private TeeingSpot GetAvailableTeeingSpotInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'TeeingSpot GolfTeeManager::GetAvailableTeeingSpotInternal()' called when server was not active");
			return null;
		}
		List<GolfTeeingPlatform> list = new List<GolfTeeingPlatform>();
		int num = int.MinValue;
		foreach (GolfTeeingPlatform allTeeingPlaform in allTeeingPlaforms)
		{
			if (allTeeingPlaform.IsActive)
			{
				int availableTeeingSpotCount = allTeeingPlaform.GetAvailableTeeingSpotCount();
				if (availableTeeingSpotCount == num)
				{
					list.Add(allTeeingPlaform);
				}
				else if (availableTeeingSpotCount > num)
				{
					num = availableTeeingSpotCount;
					list.Clear();
					list.Add(allTeeingPlaform);
				}
			}
		}
		if (list.Count <= 0)
		{
			Debug.LogError("No available teeing spots found", base.gameObject);
			return null;
		}
		GolfTeeingPlatform golfTeeingPlatform = list.Random();
		if (!golfTeeingPlatform.TryGetAvailableTeeingSpot(out var teeingSpot))
		{
			Debug.LogError("Attempted to get an available teeing spot from a platform, but it has none", golfTeeingPlatform);
			return null;
		}
		return teeingSpot;
	}

	[Server]
	private void UpdateActivePlatformsInternal()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void GolfTeeManager::UpdateActivePlatformsInternal()' called when server was not active");
			return;
		}
		int num = CourseManager.CountActivePlayers();
		foreach (GolfTeeingPlatform allTeeingPlaform in allTeeingPlaforms)
		{
			if (allTeeingPlaform.MinPlayerCount <= num)
			{
				allTeeingPlaform.ServerActivate();
			}
		}
	}

	private void SpawnHitTeeInternal(Vector3 position, Quaternion rotation, Vector3 hitDirection, float hitPower)
	{
		GetUnusedHitTee().Initialize(position, rotation, hitDirection, hitPower);
		static void EnsurePoolParentExists()
		{
			if (!(hitTeePoolParent != null))
			{
				GameObject obj = new GameObject("Hit tee pool");
				UnityEngine.Object.DontDestroyOnLoad(obj);
				hitTeePoolParent = obj.transform;
			}
		}
		HitGolfTee GetUnusedHitTee()
		{
			EnsurePoolParentExists();
			HitGolfTee result = null;
			while (result == null)
			{
				if (!hitTeePool.TryPop(out result))
				{
					result = UnityEngine.Object.Instantiate(hitTeePrefab);
				}
			}
			result.gameObject.SetActive(value: true);
			return result;
		}
	}

	private void ReturnHitTeeInternal(HitGolfTee hitTee)
	{
		if (hitTeePool.Count >= maxHitTeePoolSize)
		{
			UnityEngine.Object.Destroy(hitTee.gameObject);
			return;
		}
		hitTee.gameObject.SetActive(value: false);
		hitTee.transform.SetParent(hitTeePoolParent);
		hitTeePool.Push(hitTee);
	}
}
