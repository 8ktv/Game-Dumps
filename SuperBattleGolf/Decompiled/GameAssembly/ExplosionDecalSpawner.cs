using UnityEngine;

public class ExplosionDecalSpawner : MonoBehaviour
{
	[SerializeField]
	private VfxType vfxType = VfxType.ExplosionDecal;

	[SerializeField]
	private ItemSettings itemSettings;

	[SerializeField]
	private ItemType itemType;

	private PoolableParticleSystem poolableParticles;

	private void OnEnable()
	{
		poolableParticles = GetComponent<PoolableParticleSystem>();
		if (poolableParticles != null)
		{
			poolableParticles.ParticlesPlayed += OnParticlesPlayed;
		}
	}

	private void OnDisable()
	{
		if (poolableParticles != null)
		{
			poolableParticles.ParticlesPlayed -= OnParticlesPlayed;
		}
		poolableParticles = null;
	}

	private void OnParticlesPlayed()
	{
		if (!(itemSettings == null) && SingletonBehaviour<TerrainManager>.HasInstance)
		{
			float y = base.transform.position.y;
			float worldHeightAtPoint = TerrainManager.GetWorldHeightAtPoint(base.transform.position);
			float explosionRange = GetExplosionRange();
			if (Mathf.Abs(y - worldHeightAtPoint) <= explosionRange * 0.5f)
			{
				SpawnDecal(worldHeightAtPoint, explosionRange);
			}
		}
	}

	private float GetExplosionRange()
	{
		return itemType switch
		{
			ItemType.RocketLauncher => itemSettings.RocketExplosionRange, 
			ItemType.Landmine => itemSettings.LandmineExplosionRange, 
			ItemType.OrbitalLaser => itemSettings.OrbitalLaserExplosionMaxRange, 
			ItemType.FreezeBomb => itemSettings.FreezeBombExplosionRange, 
			_ => 0f, 
		};
	}

	private void SpawnDecal(float terrainWorldHeight, float explosionRange)
	{
		VfxManager.PlayPooledVfxLocalOnly(vfxType, new Vector3(base.transform.position.x, terrainWorldHeight, base.transform.position.z), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Vector3.one * explosionRange);
	}
}
