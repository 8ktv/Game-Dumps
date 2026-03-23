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
			return AsEntity.PlayerInfo.Spine1Bone.position;
		}
		return AsEntity.GetTargetReticleWorldPosition();
	}

	public bool IsValid()
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
		}
		return true;
	}
}
