using Mirror;
using UnityEngine;

public class GolfHoleTrigger : MonoBehaviour
{
	[SerializeField]
	private GolfHole parentHole;

	[SerializeField]
	private Collider ballTrigger;

	[SerializeField]
	private Collider golfCartTrigger;

	[SerializeField]
	private Collider generalTrigger;

	private void Awake()
	{
		ballTrigger.includeLayers = GameManager.LayerSettings.HoleBallTriggerMask;
		ballTrigger.excludeLayers = ~(int)GameManager.LayerSettings.HoleBallTriggerMask;
		golfCartTrigger.includeLayers = GameManager.LayerSettings.HoleGolfCartTriggerMask;
		golfCartTrigger.excludeLayers = ~(int)GameManager.LayerSettings.HoleGolfCartTriggerMask;
		generalTrigger.includeLayers = GameManager.LayerSettings.HoleGeneralTriggerMask;
		generalTrigger.excludeLayers = ~(int)GameManager.LayerSettings.HoleGeneralTriggerMask;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && !(other.attachedRigidbody == null) && other.attachedRigidbody.TryGetComponent<Entity>(out var component))
		{
			parentHole.ServerInformFellIn(component);
		}
	}
}
