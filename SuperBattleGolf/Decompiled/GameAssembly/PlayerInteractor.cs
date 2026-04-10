using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
	private PlayerInfo playerInfo;

	private void Awake()
	{
		playerInfo = GetComponent<PlayerInfo>();
	}

	public bool TryInteract()
	{
		if (!CanInteract())
		{
			return false;
		}
		playerInfo.AsTargeter.FirstTargetInteracable.LocalPlayerInteract();
		return true;
		bool CanInteract()
		{
			if (!playerInfo.AsTargeter.HasTarget)
			{
				return false;
			}
			if (playerInfo.Movement.IsKnockedOutOrRecovering)
			{
				return false;
			}
			if (playerInfo.AsHittable.IsFrozen)
			{
				return false;
			}
			if (playerInfo.AsGolfer.IsChargingSwing)
			{
				return false;
			}
			if (playerInfo.AsGolfer.IsSwinging)
			{
				return false;
			}
			return true;
		}
	}
}
