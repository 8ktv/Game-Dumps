using Cysharp.Threading.Tasks;
using UnityEngine;

public class RivalryVfx : MonoBehaviour, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private PoolableParticleSystem poolable;

	[SerializeField]
	private ParticleSystem spawnParticles;

	[SerializeField]
	private ParticleSystem idleParticles;

	[SerializeField]
	private ParticleSystem despawnParticles;

	private Transform target;

	private Vector3 followVelocity;

	private void OnEnable()
	{
		BUpdate.RegisterCallback(this);
	}

	private void OnDisable()
	{
		BUpdate.DeregisterCallback(this);
	}

	public void OnLateBUpdate()
	{
		if (!(target == null))
		{
			base.transform.position = GetTargetPosition();
		}
	}

	public void Spawn(Transform target)
	{
		if (target == null)
		{
			poolable.ReturnToPool();
			return;
		}
		this.target = target;
		base.transform.position = GetTargetPosition();
		spawnParticles.Play(withChildren: true);
		idleParticles.Play(withChildren: true);
	}

	private Vector3 GetTargetPosition()
	{
		return target.position + GameManager.UiSettings.SpectatorNameTagWorldOffset;
	}

	public async void Despawn()
	{
		idleParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
		despawnParticles.Play(withChildren: true);
		await UniTask.WaitForSeconds(1.5f);
		if (!(this == null))
		{
			poolable.ReturnToPool();
		}
	}
}
