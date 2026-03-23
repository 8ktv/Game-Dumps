#define DEBUG_DRAW
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class PlayerOcclusionManager : SingletonBehaviour<PlayerOcclusionManager>
{
	private struct State
	{
		public byte visible;

		public double lastVisible;
	}

	[BurstCompile]
	private struct SetupRaycastsJob : IJobParallelForTransform
	{
		public float3 cameraPos;

		public int layerMask;

		[NativeDisableParallelForRestriction]
		public NativeArray<RaycastCommand> raycastCommands;

		public void Execute(int index, TransformAccess transformAccess)
		{
			float3 x = cameraPos - (float3)transformAccess.position;
			raycastCommands[index] = new RaycastCommand(transformAccess.position, math.normalize(x), new QueryParameters(layerMask, hitMultipleFaces: false, QueryTriggerInteraction.Ignore), math.length(x));
		}
	}

	[BurstCompile]
	private struct UpdateOcclusion : IJobParallelFor
	{
		[NativeDisableParallelForRestriction]
		public NativeList<State> visibility;

		[ReadOnly]
		public NativeArray<RaycastHit> raycastHits;

		public double time;

		public void Execute(int index)
		{
			State value = visibility[index];
			value.visible = 0;
			for (int i = 0; i < 2; i++)
			{
				if (raycastHits[index * 2 + i].colliderInstanceID == 0)
				{
					value.visible = 1;
					value.lastVisible = time;
					break;
				}
			}
			visibility[index] = value;
		}
	}

	private TransformAccessArray transforms;

	private NativeList<State> visibilty;

	private List<PlayerOcclusion> instances = new List<PlayerOcclusion>();

	private JobHandle currentJobHandle;

	[CVar("playerOcclusionDebug", "", "", false, true)]
	private static bool drawDebugRays;

	protected override void Awake()
	{
		base.Awake();
		transforms = new TransformAccessArray(16);
		visibilty = new NativeList<State>(8, Allocator.Persistent);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		currentJobHandle.Complete();
		transforms.Dispose();
		visibilty.Dispose();
	}

	public static void RegisterInstance(PlayerOcclusion playerOcclusion)
	{
		if (SingletonBehaviour<PlayerOcclusionManager>.HasInstance)
		{
			SingletonBehaviour<PlayerOcclusionManager>.Instance.RegisterInstanceInternal(playerOcclusion);
		}
	}

	public static void DeregisterInstance(PlayerOcclusion playerOcclusion)
	{
		if (SingletonBehaviour<PlayerOcclusionManager>.HasInstance)
		{
			SingletonBehaviour<PlayerOcclusionManager>.Instance.DeregisterInstanceInternal(playerOcclusion);
		}
	}

	public static bool IsOccluded(int instanceId)
	{
		if (!SingletonBehaviour<PlayerOcclusionManager>.HasInstance)
		{
			return false;
		}
		SingletonBehaviour<PlayerOcclusionManager>.Instance.currentJobHandle.Complete();
		return SingletonBehaviour<PlayerOcclusionManager>.Instance.visibilty[instanceId].visible == 0;
	}

	public static float TimeSinceVisible(int instanceId)
	{
		if (!SingletonBehaviour<PlayerOcclusionManager>.HasInstance)
		{
			return 0f;
		}
		SingletonBehaviour<PlayerOcclusionManager>.Instance.currentJobHandle.Complete();
		return (float)(Time.timeAsDouble - SingletonBehaviour<PlayerOcclusionManager>.Instance.visibilty[instanceId].lastVisible);
	}

	private void RegisterInstanceInternal(PlayerOcclusion playerOcclusion)
	{
		currentJobHandle.Complete();
		playerOcclusion.SetInstanceId(instances.Count);
		transforms.Add(playerOcclusion.pelvis);
		transforms.Add(playerOcclusion.head);
		instances.Add(playerOcclusion);
		visibilty.Add(default(State));
	}

	private void DeregisterInstanceInternal(PlayerOcclusion playerOcclusion)
	{
		currentJobHandle.Complete();
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		int num = instances.IndexOf(playerOcclusion);
		if (num >= 0)
		{
			for (int num2 = 1; num2 >= 0; num2--)
			{
				transforms.RemoveAtSwapBack(num * 2 + num2);
			}
			instances.RemoveAtSwapBack(num);
			visibilty.RemoveAtSwapBack(num);
			if (num < instances.Count)
			{
				instances[num].SetInstanceId(num);
			}
		}
	}

	private void LateUpdate()
	{
		currentJobHandle.Complete();
		NativeArray<RaycastCommand> nativeArray = new NativeArray<RaycastCommand>(transforms.length, Allocator.TempJob);
		NativeArray<RaycastHit> nativeArray2 = new NativeArray<RaycastHit>(transforms.length, Allocator.TempJob);
		JobHandle dependsOn = new SetupRaycastsJob
		{
			cameraPos = GameManager.Camera.transform.position,
			layerMask = GameManager.LayerSettings.PlayerOcclusionMask,
			raycastCommands = nativeArray
		}.Schedule(transforms);
		JobHandle dependsOn2 = RaycastCommand.ScheduleBatch(nativeArray, nativeArray2, 1, 1, dependsOn);
		JobHandle jobHandle = IJobParallelForExtensions.Schedule(new UpdateOcclusion
		{
			raycastHits = nativeArray2,
			visibility = visibilty,
			time = Time.timeAsDouble
		}, visibilty.Length, 1, dependsOn2);
		currentJobHandle = jobHandle;
		if (drawDebugRays)
		{
			currentJobHandle.Complete();
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Color color = ((nativeArray2[i].colliderInstanceID != 0) ? Color.red : Color.blue);
				if (nativeArray2[i].colliderInstanceID != 0)
				{
					BDebug.DrawWireSphere(nativeArray2[i].point, 0.2f, color);
				}
				BDebug.DrawRay(nativeArray[i].from, nativeArray[i].direction * nativeArray[i].distance, color);
			}
			nativeArray.Dispose();
			nativeArray2.Dispose();
		}
		else
		{
			nativeArray.Dispose(currentJobHandle);
			nativeArray2.Dispose(currentJobHandle);
		}
	}
}
