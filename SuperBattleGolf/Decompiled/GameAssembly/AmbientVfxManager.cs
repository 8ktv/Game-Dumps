using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mirror;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class AmbientVfxManager : SingletonNetworkBehaviour<AmbientVfxManager>, IBUpdateCallback, IAnyBUpdateCallback
{
	[BurstCompile]
	public struct AmbientVfxDistanceJob : IJobParallelFor
	{
		[Unity.Collections.ReadOnly]
		public NativeArray<float3> positions;

		public float3 cameraPosition;

		public float activationRadiusSq;

		public NativeArray<bool> results;

		public void Execute(int index)
		{
			if (index < positions.Length && index < results.Length)
			{
				float num = math.distancesq(cameraPosition, positions[index]);
				results[index] = num < activationRadiusSq;
			}
		}
	}

	private const int initializationAmbientVfxCount = 32;

	[SerializeField]
	private AmbientVfxSettings vfxSettings;

	[SyncVar(hook = "OnSeedChanged")]
	private int seed;

	private List<AmbientVfx> ambientVfxs = new List<AmbientVfx>();

	private readonly List<AmbientVfx> pendingRegistrations = new List<AmbientVfx>();

	private NativeList<float3> vfxPositions;

	private NativeList<bool> visibilityResults;

	private JobHandle visibilityJob;

	private bool jobScheduled;

	private bool isSeedReady;

	public Action<int, int> _Mirror_SyncVarHookDelegate_seed;

	public int Networkseed
	{
		get
		{
			return seed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref seed, 1uL, _Mirror_SyncVarHookDelegate_seed);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (!vfxPositions.IsCreated)
		{
			vfxPositions = new NativeList<float3>(32, Allocator.Persistent);
		}
		if (!visibilityResults.IsCreated)
		{
			visibilityResults = new NativeList<bool>(32, Allocator.Persistent);
		}
		if (NetworkServer.active)
		{
			Networkseed = new System.Random().Next(1, int.MaxValue);
			isSeedReady = true;
			UpdatePendingRegistrations();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (jobScheduled)
		{
			visibilityJob.Complete();
		}
		if (vfxPositions.IsCreated)
		{
			vfxPositions.Dispose();
		}
		if (visibilityResults.IsCreated)
		{
			visibilityResults.Dispose();
		}
	}

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		if (ambientVfxs != null && ambientVfxs.Count != 0)
		{
			HandleVisibilityJob();
		}
	}

	private void HandleVisibilityJob()
	{
		if (jobScheduled)
		{
			visibilityJob.Complete();
			ApplyResults();
			jobScheduled = false;
		}
		for (int i = 0; i < ambientVfxs.Count; i++)
		{
			if (!(ambientVfxs[i] == null))
			{
				vfxPositions[i] = ambientVfxs[i].transform.position;
			}
		}
		AmbientVfxDistanceJob jobData = new AmbientVfxDistanceJob
		{
			positions = vfxPositions.AsArray(),
			cameraPosition = GameManager.Camera.transform.position,
			activationRadiusSq = vfxSettings.ActivationRadiusSqr,
			results = visibilityResults.AsArray()
		};
		visibilityJob = IJobParallelForExtensions.Schedule(jobData, ambientVfxs.Count, 32);
		jobScheduled = true;
	}

	private void ApplyResults()
	{
		for (int i = 0; i < ambientVfxs.Count; i++)
		{
			AmbientVfx ambientVfx = ambientVfxs[i];
			if (!(ambientVfx == null))
			{
				if (visibilityResults[i])
				{
					ambientVfx.PlayLocal();
				}
				else
				{
					ambientVfx.StopLocal();
				}
			}
		}
	}

	public static void RegisterAmbientVfx(AmbientVfx ambientVfx)
	{
		if (SingletonNetworkBehaviour<AmbientVfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<AmbientVfxManager>.Instance.RegisterAmbientVfxInternal(ambientVfx);
		}
	}

	public static void DeregisterAmbientVfx(AmbientVfx ambientVfx)
	{
		if (SingletonNetworkBehaviour<AmbientVfxManager>.HasInstance)
		{
			SingletonNetworkBehaviour<AmbientVfxManager>.Instance.DeregisterAmbientVfxInternal(ambientVfx);
		}
	}

	private void RegisterAmbientVfxInternal(AmbientVfx ambientVfx)
	{
		if (ambientVfxs.Contains(ambientVfx))
		{
			return;
		}
		if (!isSeedReady)
		{
			if (!pendingRegistrations.Contains(ambientVfx))
			{
				pendingRegistrations.Add(ambientVfx);
			}
			return;
		}
		if (vfxSettings == null)
		{
			Debug.LogWarning("AmbientVfxSettings is not assigned in AmbientVfxManager.");
			return;
		}
		if (!vfxSettings.SettingsByType.TryGetValue(ambientVfx.VfxType, out var value) || value == null)
		{
			Debug.LogWarning($"No settings found for AmbientVfxType {ambientVfx.VfxType} in AmbientVfxSettings.");
			return;
		}
		if (value.hasSpawnChance && !ShouldSpawnVfx(value.spawnChance, GetHashId(ambientVfx.transform)))
		{
			ambientVfx.DisableVfx();
			return;
		}
		ambientVfx.Initialize();
		ambientVfxs.Add(ambientVfx);
		if (ambientVfxs.Count > vfxPositions.Length)
		{
			vfxPositions.Add((float3)ambientVfx.transform.position);
		}
		if (ambientVfxs.Count > visibilityResults.Length)
		{
			visibilityResults.Add(false);
		}
		bool ShouldSpawnVfx(float spawnChance, int hashId)
		{
			return new System.Random(seed ^ (hashId * 397)).NextDouble() < (double)spawnChance;
		}
	}

	private void OnSeedChanged(int previousSeed, int currentSeed)
	{
		if (!NetworkServer.active)
		{
			isSeedReady = true;
			UpdatePendingRegistrations();
		}
	}

	private void UpdatePendingRegistrations()
	{
		if (pendingRegistrations.Count == 0)
		{
			return;
		}
		for (int i = 0; i < pendingRegistrations.Count; i++)
		{
			AmbientVfx ambientVfx = pendingRegistrations[i];
			if (!(ambientVfx == null))
			{
				RegisterAmbientVfxInternal(ambientVfx);
			}
		}
		pendingRegistrations.Clear();
	}

	private void DeregisterAmbientVfxInternal(AmbientVfx ambientVfx)
	{
		if (pendingRegistrations.Contains(ambientVfx))
		{
			pendingRegistrations.Remove(ambientVfx);
		}
		if (ambientVfxs.Contains(ambientVfx))
		{
			ambientVfxs.Remove(ambientVfx);
		}
	}

	private int GetHashId(Transform transform)
	{
		return (transform.name + transform.GetSiblingIndex()).GetHashCode();
	}

	public AmbientVfxManager()
	{
		_Mirror_SyncVarHookDelegate_seed = OnSeedChanged;
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
			writer.WriteVarInt(seed);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarInt(seed);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref seed, _Mirror_SyncVarHookDelegate_seed, reader.ReadVarInt());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref seed, _Mirror_SyncVarHookDelegate_seed, reader.ReadVarInt());
		}
	}
}
