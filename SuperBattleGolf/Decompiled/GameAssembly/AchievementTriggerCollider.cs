using UnityEngine;

public class AchievementTriggerCollider : MonoBehaviour
{
	[SerializeField]
	private AchievementId achievement;

	[SerializeField]
	private bool awardInDrivingRange;

	[SerializeField]
	private bool awardInMatch;

	private void Awake()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider obj in componentsInChildren)
		{
			obj.isTrigger = true;
			obj.includeLayers = GameManager.LayerSettings.PlayersMask;
			obj.excludeLayers = ~(int)GameManager.LayerSettings.PlayersMask;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			if (!awardInDrivingRange)
			{
				return;
			}
		}
		else if (!awardInMatch)
		{
			return;
		}
		if (!(other.attachedRigidbody == null) && !(GameManager.LocalPlayerInfo == null) && other.attachedRigidbody.TryGetComponent<PlayerInfo>(out var component) && !(component != GameManager.LocalPlayerInfo))
		{
			GameManager.AchievementsManager.Unlock(achievement);
		}
	}
}
