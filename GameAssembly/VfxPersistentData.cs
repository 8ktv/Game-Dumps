using System.Collections.Generic;
using UnityEngine;

public class VfxPersistentData : SingletonBehaviour<VfxPersistentData>
{
	[SerializeField]
	private VfxPoolSettings poolSettings;

	[SerializeField]
	private BallVfxSettings defaultBallVfxSettings;

	[SerializeField]
	private SwingSlashVfxSettings defaultSwingSlashVfxSettings;

	private readonly Dictionary<VfxType, ObjectPool<PoolableParticleSystem>> vfxPools = new Dictionary<VfxType, ObjectPool<PoolableParticleSystem>>();

	public static BallVfxSettings DefaultBallVfxSettings
	{
		get
		{
			if (!SingletonBehaviour<VfxPersistentData>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<VfxPersistentData>.Instance.defaultBallVfxSettings;
		}
	}

	public static SwingSlashVfxSettings DefaultSwingSlashVfxSettings
	{
		get
		{
			if (!SingletonBehaviour<VfxPersistentData>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<VfxPersistentData>.Instance.defaultSwingSlashVfxSettings;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		InitializePools();
		void InitializePools()
		{
			for (int i = 0; i < poolSettings.Pools.Length; i++)
			{
				VfxPoolData vfxPoolData = poolSettings.Pools[i];
				if (vfxPools.ContainsKey(vfxPoolData.vfxType))
				{
					Debug.LogWarning($"Duplicate VFX sources! ({vfxPoolData.vfxType})");
				}
				else
				{
					ObjectPool<PoolableParticleSystem> value = new ObjectPool<PoolableParticleSystem>(poolSettings.BasePath + vfxPoolData.path + poolSettings.Extension, base.transform, vfxPoolData.initialPoolSize, vfxPoolData.maxPoolSize);
					vfxPools.Add(vfxPoolData.vfxType, value);
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		foreach (ObjectPool<PoolableParticleSystem> value in vfxPools.Values)
		{
			value.Dispose();
		}
	}

	public static bool TryGetPooledVfx(VfxType vfxType, out PoolableParticleSystem particleSystem)
	{
		if (!SingletonBehaviour<VfxPersistentData>.HasInstance)
		{
			particleSystem = null;
			return false;
		}
		return SingletonBehaviour<VfxPersistentData>.Instance.TryGetPooledVfxInternal(vfxType, out particleSystem);
	}

	private bool TryGetPooledVfxInternal(VfxType vfxType, out PoolableParticleSystem particleSystem)
	{
		particleSystem = null;
		if (vfxType == VfxType.None)
		{
			return false;
		}
		if (!vfxPools.TryGetValue(vfxType, out var value))
		{
			Debug.LogError($"Object pool of type {vfxType} does not exist!");
			return false;
		}
		particleSystem = value.GetInstance();
		particleSystem.gameObject.SetLayerRecursively(GameManager.LayerSettings.VfxLayer);
		return particleSystem != null;
	}
}
