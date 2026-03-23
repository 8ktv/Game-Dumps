using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolableParticleSystem;

	[SerializeField]
	private TrailVfx trail;

	private Hittable owningHittable;

	private bool isTeleporting;

	private void Awake()
	{
		trail.SetPlaying(playing: false, forced: true);
	}

	private void Update()
	{
		UpdateTrail();
	}

	public void Initialize(Hittable owningHittable)
	{
		this.owningHittable = owningHittable;
		base.transform.SetParent(this.owningHittable.transform);
		base.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		UpdateTrail();
	}

	public void BeforeTeleport()
	{
		isTeleporting = true;
		trail.Clear();
		UpdateTrail();
	}

	public void AfterTeleport()
	{
		isTeleporting = false;
		UpdateTrail();
	}

	private void UpdateTrail()
	{
		trail.SetPlaying(ShouldPlay());
		bool ShouldPlay()
		{
			if (owningHittable == null)
			{
				return false;
			}
			if (isTeleporting)
			{
				return false;
			}
			if (owningHittable.SwingProjectileState != SwingProjectileState.None)
			{
				return true;
			}
			if (owningHittable.AsEntity.IsGolfBall)
			{
				if (owningHittable.AsEntity.AsGolfBall.IsHidden)
				{
					return false;
				}
				if (owningHittable.AsEntity.AsGolfBall.OutOfBoundsReturnState != BallOutOfBoundsReturnState.None)
				{
					return true;
				}
			}
			return false;
		}
	}

	public void ReturnToPool()
	{
		owningHittable = null;
		trail.SetPlaying(playing: false);
		asPoolableParticleSystem.ReturnToPool();
	}
}
