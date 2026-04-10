using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
	public Entity AsEntity { get; private set; }

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
	}

	private void Start()
	{
		LockOnTargetManager.RegisterLockOnTarget(this);
	}

	private void OnDestroy()
	{
		LockOnTargetManager.DeregisterLockOnTarget(this);
	}

	public Vector3 GetLockOnPosition()
	{
		if (AsEntity.IsPlayer)
		{
			if (AsEntity.PlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				return AsEntity.PlayerInfo.ActiveGolfCartSeat.golfCart.AsEntity.GetTargetReticleWorldPosition();
			}
			return AsEntity.PlayerInfo.Spine1Bone.position;
		}
		return AsEntity.GetTargetReticleWorldPosition();
	}

	public bool IsValidForLocalPlayer(bool ignoreKnockoutImmunity)
	{
		if (AsEntity.IsPlayer)
		{
			if (AsEntity.PlayerInfo.ActiveGolfCartSeat.IsValid())
			{
				return false;
			}
			if (!AsEntity.PlayerInfo.Movement.IsVisible)
			{
				return false;
			}
			if (AsEntity.PlayerInfo.AsGolfer.IsMatchResolved)
			{
				return false;
			}
			if (AsEntity.PlayerInfo.AsSpectator.IsSpectating)
			{
				return false;
			}
			if (AsEntity.PlayerInfo.Movement.KnockoutImmunityStatus.hasImmunity)
			{
				return false;
			}
			if (!ignoreKnockoutImmunity && GameManager.LocalPlayerInfo != null && AsEntity.PlayerInfo.Movement.IsKnockoutProtectedFromPlayer(GameManager.LocalPlayerInfo))
			{
				return false;
			}
		}
		return true;
	}
}
