using System.Collections.Generic;
using Ara;
using UnityEngine;

public class GolfCartTireTrailVfx : MonoBehaviour, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private AraTrail trail;

	[SerializeField]
	private Transform wheelMesh;

	[SerializeField]
	private WheelCollider wheelCollider;

	[SerializeField]
	private float offset;

	private PoolableParticleSystem terrainParticles;

	private bool shouldEmit;

	private bool isGrounded;

	private Vector3 lastUpdatePosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

	private TerrainLayer previousTerrainLayer = TerrainLayer.OutOfBounds;

	private TerrainLayer currentTerrainLayer = TerrainLayer.OutOfBounds;

	private static readonly Dictionary<TerrainLayer, VfxType> terrainParticleType = new Dictionary<TerrainLayer, VfxType> { [TerrainLayer.Sand] = VfxType.None };

	private bool ShouldPlay
	{
		get
		{
			if (shouldEmit)
			{
				return isGrounded;
			}
			return false;
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

	private void Start()
	{
		UpdateTrailsInternal();
		UpdateTerrainParticlesInternal();
	}

	public void SetEmitting(bool emitting)
	{
		bool num = shouldEmit;
		shouldEmit = emitting;
		if (num != shouldEmit)
		{
			UpdateTrailsInternal();
			UpdateTerrainParticlesInternal();
		}
	}

	public void OnBUpdate()
	{
		bool num = isGrounded;
		isGrounded = wheelCollider.GetGroundHit(out var hit);
		if (num != isGrounded)
		{
			UpdateTrailsInternal();
		}
		if (!isGrounded)
		{
			UpdateTerrainParticlesInternal();
			return;
		}
		trail.transform.SetPositionAndRotation(wheelMesh.transform.position - wheelCollider.transform.up * offset, Quaternion.LookRotation(-hit.normal, wheelCollider.transform.forward));
		if (SingletonBehaviour<TerrainManager>.HasInstance && !((base.transform.position - lastUpdatePosition).sqrMagnitude <= 0.001f))
		{
			TerrainLayerSettings dominantLayerSettingsAtPoint = TerrainManager.GetDominantLayerSettingsAtPoint(base.transform.position);
			previousTerrainLayer = currentTerrainLayer;
			currentTerrainLayer = dominantLayerSettingsAtPoint.Layer;
			if (previousTerrainLayer != currentTerrainLayer)
			{
				UpdateTerrainParticlesInternal();
			}
			lastUpdatePosition = base.transform.position;
		}
	}

	private void UpdateTrailsInternal()
	{
		trail.emit = ShouldPlay;
	}

	private void UpdateTerrainParticlesInternal()
	{
		if (!terrainParticleType.TryGetValue(currentTerrainLayer, out var value))
		{
			value = VfxType.None;
		}
		if (ShouldPlay && value != VfxType.None)
		{
			if (VfxPersistentData.TryGetPooledVfx(value, out terrainParticles))
			{
				terrainParticles.Play();
			}
		}
		else if (terrainParticles != null)
		{
			terrainParticles.Stop();
			terrainParticles = null;
		}
	}
}
