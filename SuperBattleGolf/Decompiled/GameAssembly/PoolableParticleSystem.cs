using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PoolableParticleSystem : MonoBehaviour, IPoolable<PoolableParticleSystem>, IReturnableToPool
{
	private const float checkFinishedPeriod = 0.25f;

	private ObjectPool<PoolableParticleSystem> pool;

	private ParticleSystem particleSystem;

	private float timer;

	private bool timed;

	private bool stoppedBeforeDelay;

	public bool IsPlaying => particleSystem.isPlaying;

	public ParticleSystem Particles => particleSystem;

	public event Action ParticlesPlayed;

	public event Action ParticlesStopped;

	public event Action ReturnedToPool;

	private void Awake()
	{
		particleSystem = GetComponent<ParticleSystem>();
		timed = false;
	}

	public void Play(float delay)
	{
		if (delay <= 0f)
		{
			Play();
			return;
		}
		timer = delay;
		timed = true;
		stoppedBeforeDelay = false;
	}

	public void Play(bool registerWhenFinished = true)
	{
		if (particleSystem != null)
		{
			particleSystem.Play(withChildren: true);
		}
		if (registerWhenFinished)
		{
			RegisterToPoolWhenFinished().Forget();
		}
		this.ParticlesPlayed?.Invoke();
	}

	private void Update()
	{
		if (!timed)
		{
			return;
		}
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			timed = false;
			if (stoppedBeforeDelay)
			{
				RegisterToPoolWhenFinished().Forget();
			}
			else
			{
				Play();
			}
		}
	}

	public void Clear()
	{
		if (particleSystem != null)
		{
			particleSystem.Clear(withChildren: true);
		}
	}

	public void Stop(ParticleSystemStopBehavior stopBehaviour = ParticleSystemStopBehavior.StopEmitting)
	{
		if (particleSystem != null)
		{
			particleSystem.Stop(withChildren: true, stopBehaviour);
		}
		this.ParticlesStopped?.Invoke();
		if (stopBehaviour == ParticleSystemStopBehavior.StopEmittingAndClear)
		{
			ReturnToPool();
		}
		else if (timed && timer > 0f)
		{
			stoppedBeforeDelay = true;
		}
	}

	public void TriggerSubEmitter(int subEmitterIndex)
	{
		particleSystem.TriggerSubEmitter(subEmitterIndex);
	}

	private async UniTaskVoid RegisterToPoolWhenFinished()
	{
		while (this != null && base.gameObject.activeInHierarchy && (bool)particleSystem && (particleSystem.isPlaying || particleSystem.IsAlive()))
		{
			await UniTask.WaitForSeconds(0.25f);
		}
		if (this != null)
		{
			ReturnToPool();
		}
	}

	public void SetPool(ObjectPool<PoolableParticleSystem> pool)
	{
		this.pool = pool;
	}

	public void ReturnToPool()
	{
		if (!(this == null))
		{
			ResetOverrides();
			if (pool != null)
			{
				pool.RegisterFreeInstance(this);
				this.ReturnedToPool?.Invoke();
			}
		}
	}

	private void ResetOverrides()
	{
		if (!(this == null))
		{
			base.transform.localScale = Vector3.one;
		}
	}
}
