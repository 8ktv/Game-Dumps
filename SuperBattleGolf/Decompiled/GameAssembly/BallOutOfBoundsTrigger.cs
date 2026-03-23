using UnityEngine;

public class BallOutOfBoundsTrigger : MonoBehaviour
{
	private void Awake()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider obj in componentsInChildren)
		{
			obj.isTrigger = true;
			obj.includeLayers = GameManager.LayerSettings.BallMask;
			obj.excludeLayers = ~(int)GameManager.LayerSettings.BallMask;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.TryGetComponentInParent<GolfBall>(out var foundComponent, includeInactive: true))
		{
			Debug.LogError("Ball out of bounds trigger was triggered for an object that isn't a ball", other);
		}
		else
		{
			BallOutOfBoundsTriggerManager.RegisterOverlap(foundComponent, this);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.TryGetComponentInParent<GolfBall>(out var foundComponent, includeInactive: true))
		{
			Debug.LogError("Ball out of bounds trigger was triggered for an object that isn't a ball", other);
		}
		else
		{
			BallOutOfBoundsTriggerManager.DeregisterOverlap(foundComponent, this);
		}
	}
}
