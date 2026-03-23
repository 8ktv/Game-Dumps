using System.Collections.Generic;
using Mirror;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class LandmineManager : SingletonBehaviour<LandmineManager>
{
	public struct LandmineInstance
	{
		public float3 worldPosition;
	}

	private const int initializationArmedLandmineCount = 16;

	private const int maxColliderOverlapsPerLandmine = 64;

	private readonly List<Landmine> armedLandmines = new List<Landmine>();

	private readonly Dictionary<Landmine, int> armedLandmineIndices = new Dictionary<Landmine, int>();

	private static NativeList<LandmineInstance> armedLandmineInstances;

	private static TransformAccessArray armedLandmineTransformAccessArray;

	private static NativeArray<OverlapSphereCommand> overlapSphereCommands;

	private static NativeArray<ColliderHit> landmineColliderDetectionResultBuffer;

	protected override void Awake()
	{
		base.Awake();
		if (!armedLandmineInstances.IsCreated)
		{
			armedLandmineInstances = new NativeList<LandmineInstance>(16, Allocator.Persistent);
			armedLandmineTransformAccessArray = new TransformAccessArray(16);
			overlapSphereCommands = new NativeArray<OverlapSphereCommand>(16, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			landmineColliderDetectionResultBuffer = new NativeArray<ColliderHit>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		armedLandmineInstances.Clear();
		for (int num = armedLandmineTransformAccessArray.length - 1; num >= 0; num--)
		{
			armedLandmineTransformAccessArray.RemoveAtSwapBack(num);
		}
	}

	public static void RegisterArmedLandmine(Landmine landmine)
	{
		if (SingletonBehaviour<LandmineManager>.HasInstance)
		{
			SingletonBehaviour<LandmineManager>.Instance.RegisterArmedLandmineInternal(landmine);
		}
	}

	public static void DeregisterArmedLandmine(Landmine landmine)
	{
		if (SingletonBehaviour<LandmineManager>.HasInstance)
		{
			SingletonBehaviour<LandmineManager>.Instance.DeregisterArmedLandmineInternal(landmine);
		}
	}

	private void RegisterArmedLandmineInternal(Landmine landmine)
	{
		if (!armedLandmineIndices.TryAdd(landmine, armedLandmines.Count))
		{
			return;
		}
		armedLandmines.Add(landmine);
		armedLandmineTransformAccessArray.Add(landmine.transform);
		armedLandmineInstances.Add(default(LandmineInstance));
		if (overlapSphereCommands.Length < armedLandmines.Count)
		{
			if (overlapSphereCommands.IsCreated)
			{
				overlapSphereCommands.Dispose();
			}
			if (landmineColliderDetectionResultBuffer.IsCreated)
			{
				landmineColliderDetectionResultBuffer.Dispose();
			}
			overlapSphereCommands = new NativeArray<OverlapSphereCommand>(BMath.RoundUpToPowerOfTwo(armedLandmines.Count), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			landmineColliderDetectionResultBuffer = new NativeArray<ColliderHit>(BMath.RoundUpToPowerOfTwo(armedLandmines.Count * 16 * 64), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
	}

	private void DeregisterArmedLandmineInternal(Landmine landmine)
	{
		if (armedLandmineIndices.TryGetValue(landmine, out var value))
		{
			Dictionary<Landmine, int> dictionary = armedLandmineIndices;
			List<Landmine> list = armedLandmines;
			dictionary[list[list.Count - 1]] = value;
			armedLandmineIndices.Remove(landmine);
			armedLandmineTransformAccessArray.RemoveAtSwapBack(value);
			armedLandmines.RemoveAtSwapBack(value);
			armedLandmineInstances.RemoveAtSwapBack(value);
		}
	}

	public void FixedUpdate()
	{
		if (NetworkServer.active && armedLandmines.Count > 0)
		{
			UpdateLandmineTransformsJob jobData = new UpdateLandmineTransformsJob
			{
				landmines = armedLandmineInstances
			};
			SetUpLandmineOverlapChecksJob jobData2 = new SetUpLandmineOverlapChecksJob
			{
				landmines = armedLandmineInstances,
				overlapSphereCommands = overlapSphereCommands,
				detectionRadius = GameManager.ItemSettings.LandmineDetectionRange,
				queryParameters = new QueryParameters(GameManager.LayerSettings.LandmineDetectableMask, hitMultipleFaces: false, QueryTriggerInteraction.Ignore)
			};
			int length = armedLandmineInstances.Length;
			JobHandle dependsOn = jobData.Schedule(armedLandmineTransformAccessArray);
			JobHandle dependsOn2 = IJobParallelForExtensions.Schedule(jobData2, length, 1, dependsOn);
			OverlapSphereCommand.ScheduleBatch(overlapSphereCommands.GetSubArray(0, length), landmineColliderDetectionResultBuffer, 1, 64, dependsOn2).Complete();
			for (int num = armedLandmines.Count - 1; num >= 0; num--)
			{
				Landmine landmine = armedLandmines[num];
				NativeSlice<ColliderHit> detectedColliderBuffer = new NativeSlice<ColliderHit>(landmineColliderDetectionResultBuffer, num * 64, 64);
				landmine.ProcessDetectedColliders(detectedColliderBuffer);
			}
		}
	}
}
