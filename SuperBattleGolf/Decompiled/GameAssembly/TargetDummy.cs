using UnityEngine;

public class TargetDummy : MonoBehaviour
{
	[SerializeField]
	private Entity asEntity;

	[SerializeField]
	private Animator animator;

	private static readonly int hitFrontHash = Animator.StringToHash("hit_front");

	private static readonly int hitBackHash = Animator.StringToHash("hit_back");

	private static readonly int hitSpinClockwiseHash = Animator.StringToHash("hit_spin_cw");

	private static readonly int hitSpinCounterClockwiseHash = Animator.StringToHash("hit_spin_ccw");

	private void Start()
	{
		asEntity.AsHittable.WasHitByGolfSwing += OnWasHitByGolfSwing;
		asEntity.AsHittable.WasHitBySwingProjectile += OnWasHitBySwingProjectile;
		asEntity.AsHittable.WasHitByItem += OnWasHitByItem;
		asEntity.AsHittable.WasHitByRocketLauncherBackBlast += OnWasHitByRocketLauncherBackBlast;
	}

	private void OnDestroy()
	{
		asEntity.AsHittable.WasHitByGolfSwing -= OnWasHitByGolfSwing;
		asEntity.AsHittable.WasHitBySwingProjectile -= OnWasHitBySwingProjectile;
		asEntity.AsHittable.WasHitByItem -= OnWasHitByItem;
		asEntity.AsHittable.WasHitByRocketLauncherBackBlast -= OnWasHitByRocketLauncherBackBlast;
	}

	private void PlayHitLocalOnly(Vector3 worldDirection)
	{
		float yawDeg = base.transform.InverseTransformDirection(worldDirection).GetYawDeg();
		if (yawDeg <= -135f)
		{
			HitBackward();
		}
		else if (yawDeg <= -90f)
		{
			SpinClockwise();
		}
		else if (yawDeg <= -45f)
		{
			SpinCounterClockwise();
		}
		else if (yawDeg <= 45f)
		{
			HitForward();
		}
		else if (yawDeg <= 90f)
		{
			SpinClockwise();
		}
		else if (yawDeg <= 135f)
		{
			SpinCounterClockwise();
		}
		else
		{
			HitBackward();
		}
		void HitBackward()
		{
			animator.SetTrigger(hitFrontHash);
		}
		void HitForward()
		{
			animator.SetTrigger(hitBackHash);
		}
		void SpinClockwise()
		{
			animator.SetTrigger(hitSpinClockwiseHash);
			if (VfxPersistentData.TryGetPooledVfx(VfxType.TargetDummySpinCW, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
		}
		void SpinCounterClockwise()
		{
			animator.SetTrigger(hitSpinCounterClockwiseHash);
			if (VfxPersistentData.TryGetPooledVfx(VfxType.TargetDummySpinCCW, out var particleSystem))
			{
				particleSystem.transform.SetParent(base.transform);
				particleSystem.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				particleSystem.Play();
			}
		}
	}

	private void OnWasHitByGolfSwing(PlayerGolfer hitter, Vector3 worldDirection, float power)
	{
		PlayHitLocalOnly(worldDirection);
	}

	private void OnWasHitBySwingProjectile(Hittable hitter, Vector3 worldHitVelocity)
	{
		PlayHitLocalOnly(worldHitVelocity);
	}

	private void OnWasHitByItem(PlayerInventory itemUser, ItemType itemType, ItemUseId itemUseId, Vector3 direction, float distance, bool isReflected)
	{
		PlayHitLocalOnly(direction);
	}

	private void OnWasHitByRocketLauncherBackBlast(PlayerInventory rocketLauncherUser, Vector3 direction)
	{
		PlayHitLocalOnly(direction);
	}
}
