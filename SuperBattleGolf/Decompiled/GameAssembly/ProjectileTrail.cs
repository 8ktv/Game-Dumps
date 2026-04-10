using UnityEngine;

public class ProjectileTrail : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolableParticleSystem;

	[SerializeField]
	private TrailVfx trail;

	private Hittable owningHittable;

	private bool isSuppressed;

	private bool isTeleporting;

	private void Awake()
	{
		trail.SetPlaying(playing: false, forced: true);
	}

	private void OnDestroy()
	{
		if (owningHittable != null)
		{
			owningHittable.SwingProjectileStateChanged -= OnOwningHittableSwingProjectileStateChanged;
			if (owningHittable.AsEntity.IsGolfBall)
			{
				owningHittable.AsEntity.AsGolfBall.IsHiddenChanged -= OnOwningBallIsHiddenChanged;
				owningHittable.AsEntity.AsGolfBall.OutOfBoundsReturnStateChanged -= OnOwningBallIsHiddenChanged;
			}
		}
	}

	public void Initialize(Hittable owningHittable)
	{
		SetOwningHittable(owningHittable);
		base.transform.SetParent(this.owningHittable.transform);
		base.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		UpdateTrail();
	}

	public void SetIsSuppressed(bool isSuppressed)
	{
		this.isSuppressed = isSuppressed;
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
			if (isSuppressed)
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

	private void SetOwningHittable(Hittable newOwningHittable)
	{
		if (owningHittable != null)
		{
			owningHittable.SwingProjectileStateChanged -= OnOwningHittableSwingProjectileStateChanged;
			if (owningHittable.AsEntity.IsGolfBall)
			{
				owningHittable.AsEntity.AsGolfBall.IsHiddenChanged -= OnOwningBallIsHiddenChanged;
				owningHittable.AsEntity.AsGolfBall.OutOfBoundsReturnStateChanged -= OnOwningBallIsHiddenChanged;
			}
		}
		owningHittable = newOwningHittable;
		if (owningHittable != null)
		{
			owningHittable.SwingProjectileStateChanged += OnOwningHittableSwingProjectileStateChanged;
			if (owningHittable.AsEntity.IsGolfBall)
			{
				owningHittable.AsEntity.AsGolfBall.IsHiddenChanged += OnOwningBallIsHiddenChanged;
				owningHittable.AsEntity.AsGolfBall.OutOfBoundsReturnStateChanged += OnOwningBallOutOfBoundsReturnStateChanged;
			}
		}
		UpdateTrail();
	}

	public void ReturnToPool()
	{
		SetOwningHittable(null);
		trail.SetPlaying(playing: false);
		asPoolableParticleSystem.ReturnToPool();
	}

	private void OnOwningHittableSwingProjectileStateChanged()
	{
		UpdateTrail();
	}

	private void OnOwningBallIsHiddenChanged()
	{
		UpdateTrail();
	}

	private void OnOwningBallOutOfBoundsReturnStateChanged()
	{
		UpdateTrail();
	}
}
